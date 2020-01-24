using System;
using Android.Support.V4.App;
using Java.Lang;
using Saver2.Activity.fragments;

namespace Saver2.Adapters
{
    public class viewPagerAdapter : FragmentPagerAdapter
    {
        private string[] Titles;

        public viewPagerAdapter(FragmentManager fm, string[] titles)
            : base(fm)
        {
            Titles = titles;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {

            return new Java.Lang.String(Titles[position]);
        }

        public override int Count
        {
            get { return Titles.Length; }
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new FragToDo();
                case 1:
                    return new FragDone();
                case 2:
                default:
                    return new FragToDo();
            }
        }
    }
}