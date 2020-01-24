using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Saver2.helpers;


namespace Saver2.Adapters
{
    class AppConfigAdapter : BaseAdapter<AppConfigItemsClass>
    {

        Context context;
        List<AppConfigItemsClass> items;

        public AppConfigAdapter(Context context, List<AppConfigItemsClass> list)
        {
            this.context = context;
            items = list;
        }

        public override AppConfigItemsClass this[int position]
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
            AppConfigAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as AppConfigAdapterViewHolder;

            if (holder == null)
            {
                holder = new AppConfigAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.AppConfigPageItemsLayout, parent, false);
                holder.ConfigParameterName = view.FindViewById<TextView>(Resource.Id.AppConfigParameterName);
                //holder.ConfigParameterValue = view.FindViewById<TextView>(Resource.Id.AppConfigParameterValue);
                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";
                holder.ConfigParameterName.Text = items[position].param_name.ToString();
                //holder.ConfigParameterValue.Text = items[position].param_value.ToString();
            return view;
        }

        //Fill in cound here, currently 0
        public override int Count => items.Count;

    }

    class AppConfigAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
        public TextView ConfigParameterName { get; set; }
        //public TextView ConfigParameterValue { get; set; }
    }
}