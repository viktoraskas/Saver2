using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Connectivity;

namespace Saver2.Helpers
{
    class IsInternetOn
    {
        public static bool CheckConnectivity(Context context)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                wsParam.online = false;
                Toast.MakeText(context, context.GetString(Resource.String.message_no_internet), ToastLength.Short).Show();
                Signalize.Error(context);
                return false;
            }
            wsParam.online = true;
            return true;

        }
    }
}