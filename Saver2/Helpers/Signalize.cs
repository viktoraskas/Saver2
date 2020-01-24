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

namespace Saver2.Helpers
{
    class Signalize
    {
        private static ToneGenerator tone;
        public static void Error(Context context)
        {
            try
            {
                if (tone==null)
                {
                    tone = new ToneGenerator(Android.Media.Stream.Alarm, 100);
                }
                    tone.StartTone(Android.Media.Tone.SupError, 1000);
            }
            catch (Exception)
            {

                
            }

            Vibrator vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
            if (vibrator.HasVibrator)
            {
                vibrator.Vibrate(500); // for 500 ms
                //vibrator.Vibrate(VibrationEffect.CreateOneShot(500,150));
            }
        }

        public static void Good(Context context)
        {
            try
            {
                if (tone == null)
                {
                    tone = new ToneGenerator(Android.Media.Stream.Alarm, 100);
                }
                tone.StartTone(Android.Media.Tone.CdmaSoftErrorLite, 500);
            }
            catch (Exception)
            {


            }

        }
    }
}