using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace Saver2.Helpers
{
    class HideAndShowKeyboard
    {
        /**
 * Shows the soft keyboard
 */
        public void showSoftKeyboard(Android.App.Activity activity, View view)
        {
            InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
            view.RequestFocus();
            inputMethodManager.ShowSoftInput(view, 0);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);//personal line added
        }

        /**
         * Hides the soft keyboard
         */
        public void hideSoftKeyboard(Android.App.Activity activity)
        {
            var currentFocus = activity.CurrentFocus;
            if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                //Toast.MakeText(activity, "Keyboard is hidden", ToastLength.Long).Show();
            }
        }

        public void hideSoftKeyboard(Android.App.Activity activity, View view)
        {
            //var currentFocus = activity.CurrentFocus;
            //if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                view.RequestFocus();
                inputMethodManager.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);
                //Toast.MakeText(activity, "Keyboard is hidden", ToastLength.Long).Show();
            }
        }

        public bool isAcceptingText(Android.App.Activity activity,  View view)
        {
            var currentFocus = activity.CurrentFocus;
            if (currentFocus != null)
            {
                InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                if (inputMethodManager.IsAcceptingText)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } else return false;
        }
    }
}