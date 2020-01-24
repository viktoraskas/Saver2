using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Saver2.Adapters;

namespace Saver2.Activity.fragments
{
    public class FragToDo : Android.Support.V4.App.Fragment
    {
        private RecyclerView recyclerViewToDo;
        private static ItemsAdapter ItemsAdapter;
        //public List<OperationList> operationList;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //ItemsAdapter = new ItemsAdapter(this,operationList,false);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.ToDoLayout, container, false);
            recyclerViewToDo = view.FindViewById<RecyclerView>(Resource.Id.RecyclerViewToDo);
            return view;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}