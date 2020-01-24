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

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ConfigPageActivity : AppCompatActivity
    {
        private AppConfigRoot AppConfig;
        private Button buttonSave, buttonTest, buttonScanQR;
        private ListView listView;
        private ISharedPreferences sharedprefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += new EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.AppConfigPageHeaderLayout);
            SetContentView(Resource.Layout.AppConfigPageHeaderLayout);
            sharedprefs = GetSharedPreferences(prefs, FileCreationMode.Private);
            listView = FindViewById<ListView>(Resource.Id.ConfiglistView);
            listView.ItemClick += ListView_ItemClick;
            buttonSave = FindViewById<Button>(Resource.Id.ButtonConfigSave);
            buttonTest = FindViewById<Button>(Resource.Id.ButtonConfigTest);
            buttonScanQR = FindViewById<Button>(Resource.Id.ButtonConfigScanQR);
            buttonSave.Click += ButtonSave_Click;
            buttonTest.Click += ButtonTest_Click;
            buttonScanQR.Click += ButtonScanQR_Click;

            TextView textViewAppVersion = FindViewById<TextView>(Resource.Id.txtAppVerion);
            textViewAppVersion.Text = GetString(Resource.String.TextAppVersion) + " - " + AppConfigClass.AppVersion;

        }
        private void ButtonScanQR_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ScanConfigActivity));
            //Toast.MakeText(this, GetString(Resource.String.message_not_ready_yet), ToastLength.Short).Show();
        }
        private async void ButtonTest_Click(object sender, EventArgs e)
        {
            if (!IsInternetOn.CheckConnectivity(this))
            {
                return;
            }

            var index = AppConfig.list.FindIndex(a => a.param_name == "web_URL");
            var ws_url = AppConfig.list[index].param_value;
            using (var client = new ws2ApiClient(ws_url))
            {
                try
                {
                    var result = await client.GetAsync<OnlineClass>(Resources.GetString(Resource.String.ws_online));
                        if (result.status == "00")
                            Toast.MakeText(this, GetString(Resource.String.message_ws_allive), ToastLength.Short).Show();
                        else
                            Toast.MakeText(this,
                                $"{GetString(Resource.String.message_ws_return_not_00)} - {result.status}:{result.description}.",
                                ToastLength.Short).Show();
                }
                catch (Exception exception)
                {
                    Toast.MakeText(this, exception.Message, ToastLength.Short).Show();
                }
                    //Toast.MakeText(this, GetString(Resource.String.message_no_internet), ToastLength.Long).Show();
            }
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            var str = JsonConvert.SerializeObject(AppConfig);
            sharedprefs.Edit().PutString(AppConfigJson, str).Commit();
            Toast.MakeText(this, GetString(Resource.String.message_saved), ToastLength.Short).Show();
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
            text.Text = text.Text + AppConfig.list[e.Position].param_name;
            EditText editTextConfigValue = view.FindViewById<EditText>(Resource.Id.ConfigValue);
            editTextConfigValue.Text = AppConfig.list[e.Position].param_value;
            editTextConfigValue.SetSelection(editTextConfigValue.Length());
            bnt_cancel.Click += delegate
            {
                builder.Dismiss();
                builder.Dispose();
            };
            btn_save.Click += delegate
            {
                AppConfig.list[e.Position].param_value = editTextConfigValue.Text;
                listView.Adapter = new AppConfigAdapter(this, AppConfig.list);
                builder.Dismiss();
                builder.Dispose();
            };
            builder.Show();
        }
        protected override void OnStart()
        {
            base.OnStart();
            if (!string.IsNullOrEmpty(sharedprefs.GetString(AppConfigJson, string.Empty)))
            {
                AppConfig = JsonConvert.DeserializeObject<AppConfigRoot>(sharedprefs.GetString(AppConfigJson,
                    string.Empty));
                listView.Adapter = new AppConfigAdapter(this, AppConfig.list);
            }
            else
            {
                //create default config
                var jsonString = "";
                
                AppConfig = JsonConvert.DeserializeObject<AppConfigRoot>(jsonString);
                listView.Adapter = new AppConfigAdapter(this, AppConfig.list);
            }
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            //System.Diagnostics.Debug.WriteLine("Unhandled Exception. Message: {0}, Stack: {1}", ex.Message, ex.StackTrace);
            Toast.MakeText(this,"Domain klaida",ToastLength.Short).Show();
        }
        void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Toast.MakeText(this, "task klaida", ToastLength.Short).Show();
            //System.Diagnostics.Debug.WriteLine("Unobserved Exception. Message: {0}, Stack: {1}", e.Exception.Message, e.Exception.StackTrace);
        }
    }
}