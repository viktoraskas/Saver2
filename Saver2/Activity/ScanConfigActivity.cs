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

namespace Saver2.Activity
{
    [Activity(Label = "ScanConfigcs", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ScanConfigActivity : AppCompatActivity
    {
        EditText editText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ConfigScanLayout);
            editText = FindViewById<EditText>(Resource.Id.editTextScanConfig);
            editText.RequestFocus();
            editText.ShowSoftInputOnFocus = false;
            editText.KeyPress += EditText_KeyPress;
            // Create your application here
        }
        private async void EditText_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                Toast.MakeText(this, GetString(Resource.String.message_not_ready_yet), ToastLength.Long).Show();
            }
            //------- Kvieciame konfigo API

        }
    }
}