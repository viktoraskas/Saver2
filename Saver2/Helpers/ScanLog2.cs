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
using Android.Graphics;

namespace Saver2.Helpers
{
    public class ScanLog2
    {
        //public ScanLog2(bool isGoodScan)
        //{
        //    if (isGoodScan)
        //    {
        //        color = Color.LightSeaGreen;
        //    }
        //    else
        //    {
        //        color = Color.LightPink;
        //    }
        //    //this.isGoodScan = isGoodScan;
        //}

        public string Scanned { get; set; }
        public string Message { get; set; }
        public Color color { get; set; }
    }
}