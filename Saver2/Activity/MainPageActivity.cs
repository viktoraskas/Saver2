using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using static Saver2.helpers.ConstantsClass;
using Saver2.Helpers;
using Saver2.Activity;
using Android.Views;
using Saver2.helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Android.Views.InputMethods;
using Saver2.Adapters;
using System.Globalization;
using System;
using Android.Media;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using System.Threading;
using Android.Graphics;
using Saver2.BroadcastReceivers;
using Android.Net;
using Android.Support.Graphics.Drawable;
using Android.Graphics.Drawables;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainPageActivity : AppCompatActivity
    {
        List<MenuOperation> menu;
        ListView listView;
        private AppConfigRoot AppConfig;
        ISharedPreferences sharedprefs;
        AlertDialog.Builder builder;
        AlertDialog dialog;
        private List<MenuClass1> app_menu;
        bool internet;
        AppBroadcastReceiver receiver;
        IntentFilter filter;
        ImageView imageView;
        AnimatedVectorDrawableCompat avd;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            wsParam.online = false;
            SetContentView(Resource.Layout.MainPageLayout);
            listView = FindViewById<ListView>(Resource.Id.MenulistView);
            listView.ItemClick += ListView_ItemClick;

            TextView textViewCopyright1 = FindViewById<TextView>(Resource.Id.copyright1);
            textViewCopyright1.Text += " ©";

            //Typeface font = Typeface.CreateFromAsset(Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            //TextView txtIntenet = FindViewById<TextView>(Resource.Id.txtInternet);
            //txtIntenet.Typeface = font;
            //txtIntenet.SetTextColor(Color.Red);
            //internet = Intent.GetBooleanExtra("internet", false);
            //if (internet) txtIntenet.SetTextColor(Color.Green);
            //txtIntenet.Text = string.Empty;

            Context context = this.ApplicationContext;
            AppConfigClass.AppVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

            TextView textViewAppVersion = FindViewById<TextView>(Resource.Id.txtAppVerion);
            textViewAppVersion.Text = GetString(Resource.String.TextAppVersion)+" - "+AppConfigClass.AppVersion;
            GetAppConfig();

            app_menu = new List<MenuClass1>
            {
                new MenuClass1() { MenuName = "Auto", MenuIcon = "\uf02a" },
                new MenuClass1() { MenuName = "Manual", MenuIcon = "\uf468" },
                new MenuClass1() { MenuName = "Settings", MenuIcon = "\uf085" }
            };

            receiver = new AppBroadcastReceiver(); ;
            receiver.ConnectivityChanged += Receiver_ConnectivityChanged;
            filter = new IntentFilter();
            filter.AddAction("android.net.conn.CONNECTIVITY_CHANGE");

            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            avd = AnimatedVectorDrawableCompat.Create(this, Resource.Drawable.avd_feed);
            avd.RegisterAnimationCallback(new AnimatedCallback(this,imageView, avd));
        }

        private void Receiver_ConnectivityChanged(object sender, EventArgs e)
        {
            if (IsOnline())
            {
                wsParam.online = true;
                imageView.SetImageDrawable(GetDrawable(Resource.Drawable.feed_green));
            }
            else
            {
                wsParam.online = false;
                imageView.SetImageDrawable(avd);
                avd.Start();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();
            listView.Adapter = new MenuAdapter(this, app_menu);
        }

        protected override void OnResume()
        {
            base.OnResume();
            GetAppConfig();
            RegisterReceiver(receiver, filter);
        }
        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(receiver);
        }
        protected override void OnRestart()
        {
            base.OnRestart();
            //GetAppConfig();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    {
                        //ProgressBarr();
                        //dialog.Show();
                        //try
                        //{
                        //    using (var client = new ws2ApiClient(wsParam.ws_url))
                        //    {
                        //        string metod = Resources.GetString((Resource.String.ws_online)); // Checking is server alive
                        //        var result = await client.GetAsync<OnlineClass>(metod);

                        //        if (result.status == "00")
                        //        {
                                    Intent intent = new Intent(this, typeof(AutoActivity));
                                    intent.AddFlags(ActivityFlags.ReorderToFront);
                                    StartActivity(intent);
                        //        }
                        //        else
                        //        {
                        //            Signalize.Error(this);
                        //            Toast.MakeText(this, $"{GetString(Resource.String.message_ws_return_not_00)} - {result.status}:{result.description}.", ToastLength.Short).Show();
                        //        }
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Signalize.Error(this);
                        //    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                        //}
                        //dialog.Dismiss();
                        break;
                    }
                case 1:
                    {
                        Toast.MakeText(this, GetString(Resource.String.message_not_ready_yet), ToastLength.Long).Show();
                        Signalize.Error(this);
                        //StartActivity(typeof(ManualMenuActivity));
                        break;
                    }
                case 2:
                    {
                        StartActivity(typeof(ConfigPageActivity));
                        break;
                    }
                default:
                    break;
            }
        }
        private void ProgressBarr()
        {
            builder = new AlertDialog.Builder(this);
            builder.SetCancelable(false); // if you want user to wait for some process to finish,
            builder.SetView(Resource.Layout.WaitingLayout);
            dialog = builder.Create();
            dialog.Window.SetBackgroundDrawableResource(Resource.Color.mtrl_btn_transparent_bg_color);
        }

        private void GetAppConfig()
        {
            sharedprefs = GetSharedPreferences(prefs, FileCreationMode.Private);
            if (string.IsNullOrEmpty(sharedprefs.GetString(AppConfigJson, string.Empty).ToString()))
            {
                StartActivity(typeof(ConfigPageActivity));
            }
            else
            {
                AppConfig = JsonConvert.DeserializeObject<AppConfigRoot>(sharedprefs
                    .GetString(AppConfigJson, string.Empty).ToString());

                int index = AppConfig.list.FindIndex(a => a.param_name == "web_URL");
                if (index >= 0)
                {
                    wsParam.ws_url = AppConfig.list[index].param_value;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.message_no_webURL), ToastLength.Short).Show();
                    return;
                }


                index = AppConfig.list.FindIndex(a => a.param_name == "user_id");
                if (index >= 0)
                {
                    wsParam.user_id = AppConfig.list[index].param_value;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.message_no_wsPass), ToastLength.Short).Show();
                    return;
                }


                index = AppConfig.list.FindIndex(a => a.param_name == "aparatoid");
                if (index >= 0)
                {
                    wsParam.aparatoid = AppConfig.list[index].param_value;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.message_no_ID), ToastLength.Short).Show();
                    return;
                }
                wsParam.lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
                //Toast.MakeText(this, wsParam.lang, ToastLength.Long).Show();
            }
        }
        private bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }
        public class AnimatedCallback : Animatable2CompatAnimationCallback
        {
            private Context context;
            private ImageView imageView;
            private AnimatedVectorDrawableCompat vectorDrawable;
            public AnimatedCallback(Context context, ImageView iv, AnimatedVectorDrawableCompat avd)
            {
                this.context = context;
                imageView = iv;
                vectorDrawable = avd;
            }
            public override void OnAnimationEnd(Drawable drawable)
            {
                //imageView.SetImageDrawable( context.GetDrawable(Resource.Drawable.feed_red));
                imageView.Post(vectorDrawable.Start);
            }
        }
    }
}