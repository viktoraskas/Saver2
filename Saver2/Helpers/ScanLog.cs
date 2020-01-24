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
    public class ScanLog
    {
        public string Scanned { get; set; }

        private bool isGoodScan;

        public bool GetIsGoodScan()
        {
            return isGoodScan;
        }

        public void SetIsGoodScan(bool value)
        {
            if (value)
            {
                SetColor(Color.LightSeaGreen);
            }
            else
            {
                SetColor(Color.Red);
            }
            isGoodScan = value;
        }

        private Color color;

        public Color GetColor()
        {
            return color;
        }

        private void SetColor(Color value)
        {
            color = value;
        }
    }
}