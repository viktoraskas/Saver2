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
using System.IO;
using System.Net;
using Android.Support.V4.Content;
using Xamarin.Essentials;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ConfigPageActivity : AppCompatActivity
    {
       private Button buttonSave, buttonTest;
        private ListView listView;
        private ISharedPreferences sharedprefs;
        EditText GetConfEdtTxt;
        Dictionary<string, dynamic> cnfg;
        HideAndShowKeyboard kb;
        Button getConfig;
        string apkName = "saver2.apk";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AppConfigPageHeaderLayout);
            sharedprefs = GetSharedPreferences(AppConfigJson, FileCreationMode.Private);
            listView = FindViewById<ListView>(Resource.Id.ConfiglistView);
            listView.ItemClick += ListView_ItemClick;
            buttonSave = FindViewById<Button>(Resource.Id.ButtonConfigSave);
            buttonTest = FindViewById<Button>(Resource.Id.ButtonConfigTest);
            buttonSave.Click += ButtonSave_Click;
            buttonTest.Click += ButtonTest_Click;

            TextView textViewAppVersion = FindViewById<TextView>(Resource.Id.txtAppVerion);
            textViewAppVersion.Text = GetString(Resource.String.TextAppVersion) + " - " + AppConfigClass.AppVersion;

            getConfig = FindViewById<Button>(Resource.Id.ButtonConfigScanQR);
            getConfig.Click += GetConfig_Click;

            GetConfEdtTxt = FindViewById<EditText>(Resource.Id.acphlGetConfEdtTxt);
            GetConfEdtTxt.ShowSoftInputOnFocus = false;
            GetConfEdtTxt.KeyPress += GetConfEdtTxt_KeyPress;
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
            kb = new HideAndShowKeyboard();
            kb.hideSoftKeyboard(this);

        }

        private void GetConfig_Click(object sender, EventArgs e)
        {
            
            string av = AppInfo.Version.ToString();
            string xv = DeviceInfo.Version.ToString();
            string file = "saver.apk";
            string vfile = "version.txt";
            var xx = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //var xx = (string)Android.OS.Environment.ExternalStorageDirectory + "/Android/data/"+ AppInfo.PackageName + "/files";
            //Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), file);
            //ShowMe(zzz);
            //return;
            //string xxx = Path.Combine(Android.OS.Environment.GetFolderPath(Android.OS.Environment.SpecialFolder.LocalApplicationData);
            //string directory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

            string filelocal = Path.Combine(xx, file);
            //Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, file);
            //Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), file);
            string vfilelocal = Path.Combine(xx, vfile);
            //Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, vfile);
            //Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), vfile);
            //var tt = new Java.IO.File(filelocal);
            //var xx = tt.AbsolutePath;
            //ShowMe(filelocal);
            //return;
            //string path = wsParam.ws2Url +"cupd/"+ xv.Substring(0, xv.IndexOf(".")) + "/" + file;
            //ShowMe(path);
            Uri fileUrl = new Uri(wsParam.ws2Url + "cupd/" + xv.Substring(0, xv.IndexOf(".")) + "/" + file);
            Uri vUrl = new Uri(wsParam.ws2Url + "cupd/" + xv.Substring(0, xv.IndexOf(".")) + "/" + vfile);
            //return;

            getConfig.Enabled = false;

            if (File.Exists(filelocal) || File.Exists(vfilelocal))
            {
                try
                {
                    System.IO.File.Delete(filelocal);
                    System.IO.File.Delete(vfilelocal);
                }
                catch (Exception)
                {
                    //ShowMe(GetString(Resource.String.message_unknown_error));
                    //return;
                }
            }

            try
            {
                WebClient wc = new WebClient();
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls 
                                                                | System.Net.SecurityProtocolType.Tls11 
                                                                | System.Net.SecurityProtocolType.Tls12;
                wc.DownloadFile(fileUrl, filelocal);
                wc.DownloadFile(vUrl, vfilelocal);
            }
            catch (WebException we)
            {
                //throw;
                ShowMe(we.ToString());
                //ShowMe(fileUrl.ToString());
                //ShowMe(GetString(Resource.String.message_no_update_path));
                getConfig.Enabled = true;
                return;
            }

            if (File.Exists(vfilelocal)) //check version
            {
                string uversion = File.ReadLines(vfilelocal).First().Trim();
                
                if (av!=uversion) //update app
                {
                    if (File.Exists(filelocal))
                    {
                        var t = new Java.IO.File(filelocal);
                        var x = t.AbsolutePath;
                        //ShowMe(x);
                        Java.IO.File xpath = new Java.IO.File(filelocal);
                        Intent intent = new Intent(Intent.ActionInstallPackage, Android.Support.V4.Content.FileProvider.GetUriForFile(this, PackageName + ".fileProvider", xpath));
                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                        intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                        intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
                        //intent.AddFlags(ActivityFlags.NewTask);
                        StartActivity(intent);
                    }
                    else
                    {
                        ShowMe(GetString(Resource.String.message_no_update));
                    }
                } 
                else
                {
                    ShowMe(GetString(Resource.String.message_no_update));
                }
             }

            getConfig.Enabled = true;

        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //ShowMe("Please wait until download complete");
        }

        private void GetConfEdtTxt_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                if (vsUtils.IsValidJson(GetConfEdtTxt.Text))
                {
                    cnfg = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(GetConfEdtTxt.Text);
                    wsParam.aparato_id = cnfg["aparato_id"];
                    wsParam.service_key = cnfg["service_key"];
                    wsParam.ws2Url = cnfg["ws2Url"];
                    wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                    //saveConfig(cnfg);
                    vsUtils.SaveConfig(this, vsUtils.WsParamToDictionary());
                    // reikia uzkrauti parametru sarasa
                    
                    
                    
                    listView.Adapter = new AppConfAdapter(this, vsUtils.WsParamToDictionary());
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
            using (ISharedPreferences sharedprefs = this.GetSharedPreferences(AppConfigJson, FileCreationMode.Private))
            {
                if (!IsInternetOn.CheckConnectivity(this) || string.IsNullOrEmpty(sharedprefs.GetString(AppConfigJson, string.Empty)))
                {
                    Signalize.Error(this);
                    return;
                }
                using (var client = new ws2ApiClient(wsParam.ws2Url))
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
            
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            vsUtils.SaveConfig(this, vsUtils.WsParamToDictionary());
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
            editTextConfigValue.Text =cnfg.ElementAt(e.Position).Value.ToString() ;
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
               
                vsUtils.SaveConfig(this,cnfg);
                GetView();
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
        //private void saveConfig(Dictionary<string, string> _cnfg)
        //{
        //    if (_cnfg!=null && _cnfg.Count>0)
        //    {
        //        var str = JsonConvert.SerializeObject(_cnfg);
        //        sharedprefs.Edit().PutString(AppConfigJson, str).Commit();
        //        //ShowMe(GetString(Resource.String.message_saved));
        //        //ShowMe(str);
        //        Signalize.Good(this);
        //        GetView();
        //    }
        //}
        private void ShowMe(string message )
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }
        private void GetView()
        {
            using (ISharedPreferences sharedprefs = this.GetSharedPreferences(AppConfigJson, FileCreationMode.Private))
            {
                var jsonStr = sharedprefs.GetString(AppConfigJson, string.Empty);
                //ShowMe(jsonStr);
                if (!string.IsNullOrEmpty(jsonStr) && vsUtils.IsValidJson(jsonStr))
                {
                    cnfg = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonStr);
                    wsParam.aparato_id = cnfg["aparato_id"];
                    wsParam.service_key = cnfg["service_key"];
                    wsParam.ws2Url = cnfg["ws2Url"];
                    wsParam.timeout = 30.ToString();
                    if (cnfg.ContainsKey("timeout"))
                    {
                        wsParam.timeout = cnfg["timeout"];
                    }

                    wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                    // reikia uzkrauti parametru sarasa
                    listView.Adapter = new AppConfAdapter(this, cnfg);
                }
            }
        }

        private void ProgressBarr()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetCancelable(false); // if you want user to wait for some process to finish,
            builder.SetView(Resource.Layout.WaitingLayout);
            dialog = builder.Create();
            dialog.Window.SetBackgroundDrawableResource(Resource.Color.mtrl_btn_transparent_bg_color);
        }
    }
}