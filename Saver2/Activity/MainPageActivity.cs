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
using Newtonsoft.Json.Linq;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", MainLauncher = false, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainPageActivity : AppCompatActivity
    {
        ListView listView;
        private List<MenuClass1> app_menu;
        AppBroadcastReceiver receiver;
        IntentFilter filter;
        ImageView imageView;
        AnimatedVectorDrawableCompat avd;
        AlarmManager alarmManager;
        Intent logout;
        PendingIntent pendingIntent;

        protected override void OnCreate(Bundle savedInstanceState)
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
            //GetAppConfig();
            vsUtils.GetAppConfig(this);

            app_menu = new List<MenuClass1>
            {
                new MenuClass1() { MenuName = "Auto", MenuIcon = "\uf02a" },
                new MenuClass1() { MenuName = "Manual", MenuIcon = "\uf468" },
                new MenuClass1() { MenuName = "Settings", MenuIcon = "\uf085" },
                new MenuClass1() { MenuName = "Logout", MenuIcon = "\uf2f5" },
            };

            receiver = new AppBroadcastReceiver();
            receiver.ConnectivityChanged += Receiver_ConnectivityChanged;
            receiver.LogOut += Receiver_LogOut;
            filter = new IntentFilter();
            filter.AddAction("android.net.conn.CONNECTIVITY_CHANGE");
            filter.AddAction("com.saver2.logout");
            filter.AddAction(Intent.ActionTimeTick);

            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            avd = AnimatedVectorDrawableCompat.Create(this, Resource.Drawable.avd_feed);
            avd.RegisterAnimationCallback(new AnimatedCallback(this,imageView, avd));
            logout = new Intent("com.saver2.logout");
            setAlarm(logout);
        }

        private void Receiver_LogOut(object sender, EventArgs e)
        {
            FinishAffinity();
            vsUtils.logOut(this);
        }

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (wsParam.timeout!="0")
            {
                wsParam.logout_time = DateTime.Now.AddMinutes(int.Parse(wsParam.timeout));
            }
            //setAlarm(logout);
            //SessionManager.Instance.ExtendSession();
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
            vsUtils.GetAppConfig(this);
            RegisterReceiver(receiver, filter);
            if (wsParam.logout_time < DateTime.Now)
            {
                FinishAffinity();
                vsUtils.logOut(this);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            //SessionManager.Instance.EndTrackSession();
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(receiver);
        }
        protected override void OnRestart()
        {
            base.OnRestart();
            //vsUtils.GetAppConfig(this);
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
                        //    using (var client = new ws2ApiClient(wsParam.ws2Url))
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
                case 3:
                    {
                        SendBroadcast(logout);
                        break;
                    }
                default:
                    break;
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
        
        private void setAlarm(Intent intent)
        {
            if (wsParam.timeout != "0")
            {
                var t = 0;
                int.TryParse(wsParam.timeout, out t);
                pendingIntent = PendingIntent.GetBroadcast(this, 0, logout, PendingIntentFlags.UpdateCurrent);
                alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
                //alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + t * 60 * 1000, pendingIntent);
                long triggerAtTime = SystemClock.ElapsedRealtime() + (t * 60 * 1000);
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.Cancel(pendingIntent);
                    alarmManager.SetAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, triggerAtTime, pendingIntent);
                    //Log.Info(TAG, "Alarm SetAndAllowWhileIdle Set");

                }
                else if (Android.OS.Build.VERSION.SdkInt == BuildVersionCodes.Kitkat || Android.OS.Build.VERSION.SdkInt == BuildVersionCodes.Lollipop)
                {
                    alarmManager.Cancel(pendingIntent);
                    alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, triggerAtTime, pendingIntent);
                }

            }
        }
    }
}