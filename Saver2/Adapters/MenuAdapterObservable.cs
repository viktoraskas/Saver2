﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Validation;

namespace Saver2.Adapters
{
    public abstract class ObservableCollectionAdapter<T> : BaseAdapter<T>
    {
        private readonly ObservableCollection<T> items;
        private readonly int resource;

        public ObservableCollectionAdapter(Android.App.Activity context, int resource, ObservableCollection<T> items)
        {
            Requires.NotNull(context, "context");
            Requires.NotNull(items, "items");

            this.Context = context;
            this.resource = resource;
            this.items = items;
            this.items.CollectionChanged += this.OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataSetChanged();
        }

        private void OnItemChanged(object sender, EventArgs e)
        {
            this.NotifyDataSetChanged();
        }

        public override T this[int position]
        {
            get { return this.items[position]; }
        }

        protected Android.App.Activity Context { get; private set; }

        public override int Count
        {
            get { return this.items.Count; }
        }

        public override long GetItemId(int position)
        {
            return this.GetItemId(this.items[position], position);
        }

        private Dictionary<View, T> initializedViews = new Dictionary<View, T>();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView != null)
            {
                T oldItem;
                if (this.initializedViews.TryGetValue(convertView, out oldItem))
                {
                    var oldObservable = oldItem as INotifyPropertyChanged;
                    if (oldObservable != null)
                    {
                        oldObservable.PropertyChanged -= this.OnItemChanged;
                    }
                }
            }

            View view = convertView;
            if (view == null)
            {
                view = this.Context.LayoutInflater.Inflate(resource, null);
                this.InitializeNewView(view);
            }

            T item = this[position];
            this.initializedViews[view] = item;
            this.PrepareView(item, view);

            var observable = item as INotifyPropertyChanged;
            if (observable != null)
            {
                observable.PropertyChanged += this.OnItemChanged;
            }

            return view;
        }

        protected virtual void InitializeNewView(View view)
        {
        }

        protected abstract void PrepareView(T item, View view);

        protected abstract long GetItemId(T item, int position);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.items.CollectionChanged -= this.OnCollectionChanged;
            }

            base.Dispose(disposing);
        }
    }
}