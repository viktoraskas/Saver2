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
    class ManualMenuAdapter : BaseAdapter<MenuOperation>
    {

        Context context;
        List<MenuOperation> items;
        public ManualMenuAdapter(Context context, List<MenuOperation> list)
        {
            this.context = context;
            items = list;
        }

        public override MenuOperation this[int position]
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
            ManualMenuAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ManualMenuAdapterViewHolder;

            if (holder == null)
            {
                holder = new ManualMenuAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.ManualMenuItemLayout, parent, false);
                holder.Title = view.FindViewById<TextView>(Resource.Id.ManualMenuItem);
                view.Tag = holder;
            }


            //fill in your items
            holder.Title.Text = items[position].message(wsParam.lang);

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

    class ManualMenuAdapterViewHolder : Java.Lang.Object
    {
        public TextView Title { get; set; }
    }
}