using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Saver2.Adapters;
using Saver2.helpers;
using Saver2.Helpers;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ManualMenuActivity : AppCompatActivity
    {
        List<MenuOperation> menu;
        ListView ListView;
        ManualMenuAdapter MenuAdapter;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ManualMenuLayout);
            ListView = this.FindViewById<ListView>(Resource.Id.MenulistView1);
            ListView.ItemClick += ListView_ItemClick;
            #region Call ws for menu
            try
            {
                using (var client = new ws2ApiClient(wsParam.ws_url))
                {
                    var values = new Dictionary<string, string>
                            {
                             { "user_id", wsParam.user_id },
                             { "aparatoid", wsParam.aparatoid },
                             { "lang", wsParam.lang},
                            };

                    var Response = await client.PostAsync<MenuClass>("c_menu", values);

                    if (Response.status == "00")
                    {
                        menu = Response.operation;
                        //Toast.MakeText(this, $"{menu.Count}", ToastLength.Long).Show();
                        MenuAdapter = new ManualMenuAdapter(this, menu);
                    }
                    else
                    {
                        menu = null;
                        MenuAdapter = null;
                        Signalize.Error(this);
                        Toast.MakeText(this, $"{Response.status} - {Response.description}", ToastLength.Long).Show();
                    }

                }
            }
            catch (Exception ex)
            {
                menu = null;
                MenuAdapter = null;
                Signalize.Error(this);
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            #endregion

            ListView.Adapter = MenuAdapter;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(ManualActivity));
            intent.PutExtra("method", menu[e.Position].module.ToString());
            StartActivity(intent);
        }

        protected override void OnStart()
        {
            base.OnStart();
            ListView.Adapter = MenuAdapter;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ListView.Adapter = MenuAdapter;
        }

        protected override void OnRestart()
        {
            base.OnResume();
            ListView.Adapter = MenuAdapter;
        }
    }
}