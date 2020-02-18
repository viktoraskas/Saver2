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

namespace Saver2.Adapters
{
    class AppConfAdapter : BaseAdapter
    {

        Context context;
        private Dictionary<string, string> items;

        public AppConfAdapter(Context context, Dictionary<string, string> _items)
        {
            this.context = context;
            this.items = _items;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
            //var item = Items[position];
            //return item as Object;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            AppConfAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as AppConfAdapterViewHolder;

            if (holder == null)
            {
                holder = new AppConfAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.AppConfigPageItemsLayout, parent, false);
                holder.ConfigParameterName = view.FindViewById<TextView>(Resource.Id.AppConfigParameterName);
                holder.ConfigParameterValue = view.FindViewById<TextView>(Resource.Id.AppConfigParameterValue);
                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";
            holder.ConfigParameterName.Text = items.ElementAt(position).Key;
            holder.ConfigParameterValue.Text = items.ElementAt(position).Value;
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

    class AppConfAdapterViewHolder : Java.Lang.Object
    {
        public TextView ConfigParameterName { get; set; }
        public TextView ConfigParameterValue { get; set; }
    }
}