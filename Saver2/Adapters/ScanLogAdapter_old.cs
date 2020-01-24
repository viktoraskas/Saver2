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
using Saver2.Helpers;

namespace Saver2.Adapters
{
    class ScanLogAdapter_old : BaseAdapter<ScanLog2>
    {
        List<ScanLog2> items;
        Context context;

        public ScanLogAdapter_old(Context context, List<ScanLog2> list)
        {
            this.context = context;
            items = list;
        }

        public override ScanLog2 this[int position]
        {
            get { return items[position]; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ScanLogAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ScanLogAdapterViewHolder;

            if (holder == null)
            {
                holder = new ScanLogAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.ScanLogItemLayout, parent, false);
                holder.Scanned = view.FindViewById<TextView>(Resource.Id.textViewScanLogScaned);
                holder.Message = view.FindViewById<TextView>(Resource.Id.textViewScanLogMessage);
                view.Tag = holder;
            }


            //fill in your items
            holder.Scanned.Text = items[position].Scanned;
            holder.Message.Text = items[position].Message+" - ";
            holder.Scanned.SetTextColor(items[position].color);
            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

    }

    class ScanLogAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView Scanned { get; set; }
        public TextView Message { get; set; }
    }
}