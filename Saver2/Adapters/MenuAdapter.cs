using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Saver2.helpers;
using Saver2.Helpers;

namespace Saver2.Adapters
{
    class MenuAdapter : BaseAdapter<MenuClass1>
    {

        Context context;
        List<MenuClass1> items;

        public MenuAdapter(Context context, List<MenuClass1> list)
        {
            this.context = context;
            items = list;
        }

        public override MenuClass1 this[int position]
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
            Typeface font = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            Typeface font2 = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Regular-400.otf");

            var view = convertView;
            MenuAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as MenuAdapterViewHolder;

            if (holder == null)
            {
                holder = new MenuAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.MenuItemsLayout, parent, false);
                holder.MenuTitle = view.FindViewById<TextView>(Resource.Id.MenuItem);
                holder.MenuIcon = view.FindViewById<TextView>(Resource.Id.MenuIcon);
                
                view.Tag = holder;
            }


            //fill in your items
            holder.MenuTitle.Text = items[position].MenuName;
            holder.MenuIcon.Typeface = font;
            holder.MenuIcon.Text = items[position].MenuIcon;

            // krumpliaratis - f085
            // barkodas - f02a
            // boxes - f468
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

    class MenuAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView MenuTitle { get; set; }
        public TextView MenuIcon { get; set; }
    }
}