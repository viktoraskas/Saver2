using System;
using System.Collections.Generic;
using System.Globalization;
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
using Xamarin.Essentials;

namespace Saver2.Adapters
{
    class ItemsAdapter : RecyclerView.Adapter  //<OperationList>
    {
        //----------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------
        

        List<OperationList> items;
        Context context;
        private bool allitems;
        //private Color cardViewBacgroungColor;
        public ItemsAdapter(Context context , List<OperationList> items, bool allItems)
        {
            this.context = context;
            this.items = items;
            this.allitems = allItems;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Typeface font = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            Typeface font2 = Typeface.CreateFromAsset(context.Assets, "fonts/Font Awesome 5 Free-Regular-400.otf");

            ViewHolder h = holder as ViewHolder;

            h.textViewIcon1.Typeface = font2;
            h.textViewIcon1.Text = "\uf1ad";
            string separator = " : ";
            if (string.IsNullOrEmpty(items[position].kodas_lc_a) ^ string.IsNullOrEmpty(items[position].kodas_ls))
            {
                separator = String.Empty;
            }
            h.kodas_lc_a_ls.Text = $"{items[position].kodas_lc_a}{separator}{items[position].kodas_ls}";
            if (items[position].kodas_lc_a == string.Empty)
            {
                h.textViewIcon1.Text = string.Empty;
                h.kodas_lc_a_ls.Text = string.Empty;
                h.textViewIcon1.Visibility = ViewStates.Gone;
                h.kodas_lc_a_ls.Visibility = ViewStates.Gone;
            }

            h.pav.Text = items[position].pav;
            if (items[position].pav==string.Empty)
            {
                h.pav.Text = string.Empty;
                h.pav.Visibility = ViewStates.Gone;
            }

            h.textViewIcon3.Typeface = font;
            h.textViewIcon3.Text = "\uf187";
            h.kodas_ps.Text = items[position].kodas_ps;
            if (items[position].kodas_ps==string.Empty)
            {
                h.textViewIcon3.Text = string.Empty;
                h.textViewIcon3.Visibility = ViewStates.Gone;
                h.kodas_ps.Visibility = ViewStates.Gone;
            }


            h.kodas_os.Text = items[position].kodas_os;
            if (items[position].kodas_os==string.Empty)
            {
                h.kodas_os.Visibility = ViewStates.Gone;
            }

            h.serija.Text = items[position].serija;
            if (items[position].serija==string.Empty)
            {
                h.serija.Visibility = ViewStates.Gone;
            }

            h.textViewIcon2.Typeface = font;
            h.textViewIcon2.Text = "\uf07a";
            var _kiekis = Convert.ToDecimal(items[position].kiekis, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US"));
            var _kiekis_p = Convert.ToDecimal(items[position].kiekis_p, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US"));
            string kiekis = (_kiekis - _kiekis_p).ToString("G29");
            if (allitems)
            {
                kiekis = $"{_kiekis_p.ToString("G29")} / {_kiekis.ToString("G29")}";
            }
            
            h.cardViewItems1.SetCardBackgroundColor(_kiekis == _kiekis_p && _kiekis!=0
                ? Color.ParseColor("#ccff90")
                : Color.ParseColor("#ffffff") ); //Color.White);
            h.kiekis.Text = kiekis;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);
            ViewHolder holder = new ViewHolder(v);
            return holder;
        }
        public override int ItemCount
        {
            get { return items.Count; }
        }
        internal class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView kodas_lc_a_ls { get; set; }
            public TextView pav { get; set; }
            public TextView kodas_ps { get; set; }
            public TextView kodas_os { get; set; }
            public TextView serija { get; set; }
            public TextView kiekis { get; set; }
            public TextView textViewIcon1 { get; set; }
            public TextView textViewIcon2 { get; set; }
            public TextView textViewIcon3 { get; set; }
            public CardView cardViewItems1 { get; set; }


            public ViewHolder(View itemView) : base(itemView)
            {
                kodas_lc_a_ls = itemView.FindViewById<TextView>(Resource.Id.txtvKodas_lc_a_ls);
                pav = itemView.FindViewById<TextView>(Resource.Id.txtvPav);
                kodas_ps = itemView.FindViewById<TextView>(Resource.Id.txtvKodas_ps);
                kodas_os = itemView.FindViewById<TextView>(Resource.Id.txtvKodas_os);
                serija = itemView.FindViewById<TextView>(Resource.Id.txtvSerija);
                kiekis = itemView.FindViewById<TextView>(Resource.Id.txtvKiekis);
                textViewIcon1 = itemView.FindViewById<TextView>(Resource.Id.textViewIcon1);
                textViewIcon2 = itemView.FindViewById<TextView>(Resource.Id.textViewIcon2);
                textViewIcon3 = itemView.FindViewById<TextView>(Resource.Id.textViewIcon3);
                cardViewItems1 = itemView.FindViewById<CardView>(Resource.Id.cardViewItems1);
            }
        }
    }
}