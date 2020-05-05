using System;
using Android.App;
using Android.Content;
using Android.Media;
using Saver2.Helpers;

namespace Saver2.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true)]

    [IntentFilter(new[] { "android.net.conn.CONNECTIVITY_CHANGE", "android.media.VOLUME_CHANGED_ACTION","com.saver2.logout",
                            Intent.ActionScreenOff,Intent.ActionTimeTick })]

    public class AppBroadcastReceiver : BroadcastReceiver
    {
        //private static RingerMode;

        public event EventHandler ConnectivityChanged;
        public event EventHandler VolumeChanged;
        public event EventHandler LogOut;
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "android.net.conn.CONNECTIVITY_CHANGE")
            {
                ConnectivityChanged?.Invoke(this, EventArgs.Empty);
            }
            if (intent.Action == "android.media.VOLUME_CHANGED_ACTION")
            {
                VolumeChanged?.Invoke(this, EventArgs.Empty);
                //if (audioManager==null)
                //{
                //    audioManager = (AudioManager)context.GetSystemService(Context.AudioService);
                //}
                //wsParam.volume=audioManager.GetStreamVolume(Android.Media.);
            }
            if (intent.Action == "com.saver2.logout")
            {
                LogOut?.Invoke(this, EventArgs.Empty);
            }
            if (intent.Action == Intent.ActionTimeTick)
            {
                if (wsParam.logout_time< DateTime.Now && wsParam.timeout != "0")
                {
                    LogOut?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}