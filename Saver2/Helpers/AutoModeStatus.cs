using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Saver2.Helpers
{
    public class AutoModeStatus
    {
        public AutoModeStatus(string status)
        {
            Status = status;
        }
        public string Status { get; set; }
    }
}