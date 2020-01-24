using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Saver2.Helpers;

namespace Saver2.Adapters
{
    class ScanLogAdapter : RecyclerView.Adapter 
    {
        List<ScanLog2> items;
        Context context;
        public ScanLogAdapter(Context context , List<ScanLog2> items)
        {
            this.context = context;
            this.items = items;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Typeface font = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            //Typeface font2 = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Regular-400.otf");

            ViewHolder h = holder as ViewHolder;
            h.Scanned.Text = items[position].Scanned;
            h.Message.Text = items[position].Message;
            // Spalviname
            h.Scanned.SetTextColor(items[position].color);
            h.Message.SetTextColor(items[position].color);
            h.Line.SetTextColor(items[position].color);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewLogItem, parent, false);
            ViewHolder holder = new ViewHolder(v);
            return holder;
        }
        public override int ItemCount => items.Count;

        internal class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Scanned { get; set; }
            public TextView Message { get; set; }
            public TextView Line { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Scanned = itemView.FindViewById<TextView>(Resource.Id.textViewLogScanned);
                Message = itemView.FindViewById<TextView>(Resource.Id.textViewLogMessage);
                Line = itemView.FindViewById<TextView>(Resource.Id.textViewLogLine);
            }
        }
    }
}