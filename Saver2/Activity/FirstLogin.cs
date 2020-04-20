using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saver2.Helpers;
using static Saver2.helpers.ConstantsClass;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar", NoHistory = true)]
    public class FirstLogin : AppCompatActivity
    {
        CheckBox checkBox;
        EditText editText;
        private ISharedPreferences sharedprefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FirstLogin);
            // Create your application here
            wsParam.Init();
            sharedprefs = GetSharedPreferences(AppConfigJson, FileCreationMode.Private);
            checkBox = FindViewById<CheckBox>(Resource.Id.FLchk);
            editText = FindViewById<EditText>(Resource.Id.FLmtxtQR);
            editText.KeyPress += EditText_KeyPress;
            ImageView imageView = FindViewById<ImageView>(Resource.Id.FLiv1);
            imageView.SetImageResource(Resource.Drawable.qr_code);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (vsUtils.GetAppConfig(this))
            {
                Intent intent = new Intent(Application.Context, typeof(MainPageActivity));
                //intent.PutExtra("internet", internet);
                StartActivity(intent);
                //ShowMe("Startuoja naujas app");
            }
        }
        private void EditText_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            bool error = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                if (vsUtils.IsValidJson(editText.Text))
                {
                    var cnfg = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(editText.Text);
                    wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                    if (string.IsNullOrWhiteSpace(wsParam.timeout) || wsParam.timeout == "0")
                    {
                        wsParam.timeout = "30";
                        wsParam.logout_time = DateTime.Now.AddMinutes(int.Parse(wsParam.timeout));
                    }
                    if (checkBox.Checked)
                    {
                        wsParam.timeout = "0";
                    }
                    if (cnfg.ContainsKey("ws2Url"))
                    {
                        if (!string.IsNullOrWhiteSpace(cnfg["ws2Url"]))
                        {
                            if (vsUtils.CheckURLValid(cnfg["ws2Url"]))
                            {
                                wsParam.ws2Url = cnfg["ws2Url"];
                            }
                            else
                            {
                                error = true;
                                //ShowMe("No url");
                            }
                        }
                        else
                        {
                            //wsParam.ws2Url bus paimtas ir pries tai issaugoto konfigo
                        }
                    }
                    else
                    {
                        //wsParam.ws2Url bus paimtas ir pries tai issaugoto konfigo
                    }
                    if (cnfg.ContainsKey("aparato_id"))
                    {
                        wsParam.aparato_id = cnfg["aparato_id"];
                    }
                    else
                    {
                        //ShowMe("No id");
                        error = true;
                    }
                    if (cnfg.ContainsKey("service_key"))
                    {
                        wsParam.service_key = cnfg["service_key"];
                    }
                    else
                    {
                        //ShowMe("No pass");
                        error = true;
                    }
                    
                    if (!error)
                    {
                        
                        vsUtils.SaveConfig(this,vsUtils.WsParamToDictionary());
                        Intent intent = new Intent(Application.Context, typeof(MainPageActivity));
                        //intent.PutExtra("internet", internet);
                        StartActivity(intent);
                        //ShowMe("Startuoja naujas app");
                    }
                    else
                    {
                        Signalize.Error(this);
                        ShowMe(GetString(Resource.String.message_wrong_config_parameters));
                    }
                }
                else
                {
                    Signalize.Error(this);
                    //ShowMe("Not valid Json");
                    ShowMe(GetString(Resource.String.message_wrong_config_parameters));
                }
                editText.Text = string.Empty;
            }
        }
        private void ShowMe(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

    }
}