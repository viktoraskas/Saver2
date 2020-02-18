using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Saver2.Adapters;
using Saver2.helpers;
using Saver2.Helpers;
using static Saver2.helpers.ConstantsClass;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Plugin.Connectivity;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ConfigPageActivity : AppCompatActivity
    {
        private AppConfigRoot AppConfig;
        private Button buttonSave, buttonTest;
        private ListView listView;
        private ISharedPreferences sharedprefs;
        EditText GetConfEdtTxt;
        Dictionary<string, string> cnfg;
        HideAndShowKeyboard kb;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.AppConfigPageHeaderLayout);
            SetContentView(Resource.Layout.AppConfigPageHeaderLayout);
            sharedprefs = GetSharedPreferences(prefs, FileCreationMode.Private);
            listView = FindViewById<ListView>(Resource.Id.ConfiglistView);
            listView.ItemClick += ListView_ItemClick;
            buttonSave = FindViewById<Button>(Resource.Id.ButtonConfigSave);
            buttonTest = FindViewById<Button>(Resource.Id.ButtonConfigTest);
            buttonSave.Click += ButtonSave_Click;
            buttonTest.Click += ButtonTest_Click;

            TextView textViewAppVersion = FindViewById<TextView>(Resource.Id.txtAppVerion);
            textViewAppVersion.Text = GetString(Resource.String.TextAppVersion) + " - " + AppConfigClass.AppVersion;

            GetConfEdtTxt = FindViewById<EditText>(Resource.Id.acphlGetConfEdtTxt);
            GetConfEdtTxt.ShowSoftInputOnFocus = false;
            GetConfEdtTxt.KeyPress += GetConfEdtTxt_KeyPress;
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
            kb = new HideAndShowKeyboard();
            kb.hideSoftKeyboard(this);

        }

        private void GetConfEdtTxt_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                if (IsValidJson(GetConfEdtTxt.Text))
                {
                    cnfg = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetConfEdtTxt.Text);
                    wsParam.aparatoid = cnfg["aparato_id"];
                    wsParam.user_id = cnfg["service_key"];
                    wsParam.ws_url = cnfg["ws2Url"];
                    wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                    saveConfig(cnfg);
                    // reikia uzkrauti parametru sarasa
                    listView.Adapter = new AppConfAdapter(this, cnfg);
                }
                else
                {
                    Signalize.Error(this);
                    ShowMe(GetString(Resource.String.message_wrong_config_parameters));
                }
                GetConfEdtTxt.Text = string.Empty;
            }
        }
        private async void ButtonTest_Click(object sender, EventArgs e)
        {
            if (!IsInternetOn.CheckConnectivity(this) || string.IsNullOrEmpty(sharedprefs.GetString(AppConfigJson, string.Empty)))
            {
                Signalize.Error(this);
                return;
            }
            using (var client = new ws2ApiClient(wsParam.ws_url))
            {
                try
                {
                    var result = await client.GetAsync<OnlineClass>(Resources.GetString(Resource.String.ws_online));
                    if (result.status == "00")
                        ShowMe($"{GetString(Resource.String.message_ws_allive)} - {result.status}:{result.description}.");
                }
                catch (Exception exception)
                {
                    ShowMe(exception.Message);
                }
            }
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            saveConfig(cnfg);
        }
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.AppConfigEditItemLayout, null);
            AlertDialog builder = new AlertDialog.Builder(this).Create();
            builder.SetView(view);
            builder.SetCanceledOnTouchOutside(false);
            Button bnt_cancel = view.FindViewById<Button>(Resource.Id.buttonConfigParameterCancel);
            Button btn_save = view.FindViewById<Button>(Resource.Id.buttonConfigParameterSave);
            TextView text = view.FindViewById<TextView>(Resource.Id.textViewConfigParameter);
            text.Text = text.Text+cnfg.ElementAt(e.Position).Key;
            EditText editTextConfigValue = view.FindViewById<EditText>(Resource.Id.ConfigValue);
            editTextConfigValue.Text =cnfg.ElementAt(e.Position).Value ;
            editTextConfigValue.SetSelection(editTextConfigValue.Length());
            bnt_cancel.Click += delegate
            {
                builder.Dismiss();
                builder.Dispose();
            };
            btn_save.Click += delegate
            {
                string _key = cnfg.ElementAt(e.Position).Key;
                //string _value = cnfg.ElementAt(e.Position).Value;
                string _value = editTextConfigValue.Text;

                if (cnfg.Remove(_key))
                {
                    cnfg.Add(_key, _value);
                }
                else
                    ShowMe($"Cant remove key - {_key}");
               
                saveConfig(cnfg);
                //ShowMe(cnfg.ElementAt(e.Position).Key);

                builder.Dismiss();
                builder.Dispose();
                
            };
            builder.Show();
        }
        protected override void OnStart()
        {
            base.OnStart();
            GetView();
            GetConfEdtTxt.RequestFocus();
            GetConfEdtTxt.ShowSoftInputOnFocus = false;

        }

        protected override void OnResume()
        {
            base.OnResume();
            GetView();
            GetConfEdtTxt.RequestFocus();
            GetConfEdtTxt.ShowSoftInputOnFocus = false;
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            GetView();
            GetConfEdtTxt.RequestFocus();
            GetConfEdtTxt.ShowSoftInputOnFocus = false;
        }
        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void saveConfig(Dictionary<string, string> _cnfg)
        {
            if (_cnfg!=null && _cnfg.Count>0)
            {
                var str = JsonConvert.SerializeObject(_cnfg);
                sharedprefs.Edit().PutString(AppConfigJson, str).Commit();
                //ShowMe(GetString(Resource.String.message_saved));
                //ShowMe(str);
                Signalize.Good(this);
                GetView();
            }
        }
        private void ShowMe(string message )
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        private void GetView()
        {
            var jsonStr = sharedprefs.GetString(AppConfigJson, string.Empty);
            //ShowMe(jsonStr);
            if (!string.IsNullOrEmpty(jsonStr) && IsValidJson(jsonStr))
            {
                cnfg = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                wsParam.aparatoid = cnfg["aparato_id"];
                wsParam.user_id = cnfg["service_key"];
                wsParam.ws_url = cnfg["ws2Url"];
                wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                // reikia uzkrauti parametru sarasa
                listView.Adapter = new AppConfAdapter(this, cnfg);
            }
        }
    }
}