using System;
using Android.App;
using Android.Content;


namespace Saver2.BroadcastReceivers
{

    //[BroadcastReceiver( Name= "com.companyname.saver2.test", Enabled = true, Exported = true)]
    [BroadcastReceiver(Enabled = true)]

    [IntentFilter(new[] { "android.net.conn.CONNECTIVITY_CHANGE" , "android.media.VOLUME_CHANGED_ACTION"})]

    public class TestReceiver : BroadcastReceiver
    {
        public event EventHandler ConnectivityChanged;
        public event EventHandler VolumeChanged;
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action== "android.net.conn.CONNECTIVITY_CHANGE")
            {
                ConnectivityChanged?.Invoke(this, EventArgs.Empty);
            }
            if (intent.Action == "android.media.VOLUME_CHANGED_ACTION")
            {
                VolumeChanged?.Invoke(this, EventArgs.Empty);
            }
            
        }
    }
}
