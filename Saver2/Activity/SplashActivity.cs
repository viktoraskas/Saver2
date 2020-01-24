using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Saver2.helpers;
using Saver2.Helpers;
using static Saver2.helpers.ConstantsClass;

namespace Saver2.Activity
{
    [Activity(Theme = "@style/MyTheme.Splash", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        bool internet;
        ISharedPreferences sharedprefs;
        private AppConfigRoot AppConfig;

        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            wsParam.Init();

            //Log.Debug(TAG, "SplashActivity.OnCreate");
        }
        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            internet = false;

            sharedprefs = GetSharedPreferences(prefs, FileCreationMode.Private);
            if (!string.IsNullOrEmpty(sharedprefs.GetString(AppConfigJson, string.Empty).ToString()))
            {
                AppConfig = JsonConvert.DeserializeObject<AppConfigRoot>(sharedprefs
                    .GetString(AppConfigJson, string.Empty).ToString());

                int index = AppConfig.list.FindIndex(a => a.param_name == "web_URL");
                if (index >= 0)
                {
                    wsParam.ws_url = AppConfig.list[index].param_value;
                }
            }

            if (!IsInternetOn.CheckConnectivity(this))
            {
                Toast.MakeText(this, GetString(Resource.String.message_no_internet), ToastLength.Long).Show();
                Signalize.Error(this);
            }
            else
            {
                if (!string.IsNullOrEmpty(wsParam.ws_url))
                {
                    using (var client = new ws2ApiClient(wsParam.ws_url))
                    {
                        try
                        {
                            OnlineClass result = client.GetAsync<OnlineClass>(Resources.GetString(Resource.String.ws_online)).Result;
                            if (result.status == "00")
                            {
                                internet = true;
                                Toast.MakeText(this, "TRUE", ToastLength.Short).Show();
                            }
                        }
                        catch (Exception exception)
                        {
                            Toast.MakeText(this, exception.Message, ToastLength.Short).Show();
                        }
                    }
                }
            }
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        void SimulateStartup()
        {
            //Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            //await Task.Delay(8000); // Simulate a bit of startup work.
            //Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            Intent intent = new Intent(Application.Context, typeof(MainPageActivity));
            intent.PutExtra("internet", internet);
            StartActivity(intent);
        }
    }
}