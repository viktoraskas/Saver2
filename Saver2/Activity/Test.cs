using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Saver2.Helpers;
using Android.Views.InputMethods;
using Saver2.BroadcastReceivers;
using Android.Net;
using Android.Media;
using Android.Support.V4.View;
using Saver2.Activity.fragments;
using Saver2.Adapters;
using Android.Graphics;
using Saver2.helpers;
using Android.Support.V7.Widget;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", MainLauncher = false, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class Test : AppCompatActivity
    {
        EditText editText;
        TextView textViewAutoIcon, textViewAutoAtiveLoc;
        HideAndShowKeyboard kb;
        BottomNavigationView navigationView;
        int selected_menu;
        TestReceiver receiver;
        IntentFilter filter;
        string message, AtiveLoc, description;
        private List<OperationList> operationList;
        private ViewPager viewPager;
        FragToDo FragmentToDo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Testas);
            selected_menu = Resource.Id.menu_todo;
            navigationView = FindViewById<BottomNavigationView>(Resource.Id.bottomNavigationView1);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            navigationView.SelectedItemId = selected_menu;

            editText = FindViewById<EditText>(Resource.Id.edtTxtBarcode);
            editText.KeyPress += EditText_KeyPress;

            kb = new HideAndShowKeyboard();
            kb.hideSoftKeyboard(this);

            receiver = new TestReceiver();
            receiver.ConnectivityChanged += _broadcastReceiver_ConnectivityChanged;
            receiver.VolumeChanged += _broadcastReceiver_VolumeChanged;


            filter = new IntentFilter();
            filter.AddAction("android.net.conn.CONNECTIVITY_CHANGE");
            filter.AddAction("android.media.VOLUME_CHANGED_ACTION");

            RegisterReceiver(receiver, filter);


            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager1);
            viewPager.CurrentItem = 0;
            string[] titles = { "FragToDo", "FragDone" };
            viewPager.Adapter = new viewPagerAdapter(SupportFragmentManager, titles);
            viewPager.OffscreenPageLimit = titles.Length;
            viewPager.PageMargin = 10;
            viewPager.SetPageMarginDrawable(Resource.Color.button_material_light);
            viewPager.PageSelected += ViewPager_PageSelected;
            //AdapterDone = new ItemsAdapter(this,operationList,true);
            //AdapterToDO = new ItemsAdapter(this, operationList, false);

        }

        private async void EditText_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                if (!IsInternetOn.CheckConnectivity(this))
                {
                    //scanLog.Insert(0, new ScanLog2 { Scanned = "", Message = GetString(Resource.String.message_no_internet), color = Color.Red });
                    Toast.MakeText(this, GetString(Resource.String.message_no_internet), ToastLength.Long).Show();
                    Signalize.Error(this);
                    editText.Text = string.Empty;
                    return;
                }

                editText.Enabled = false; //gesinam barkodu input boxa

                e.Handled = true;
                //scanLog.Scanned = editText.Text;  //dedam i skanavimo loga
                switch (wsParam.stage)
                {
                    case 0:
                        {
                            message = GetString(Resource.String.message_scan_operation);

                            if (!string.IsNullOrEmpty(editText.Text)
                                && editText.Text.Length > 3
                                && editText.Text.Substring(2, 1) == "_"
                            )
                            {
                                wsParam.scaned = editText.Text.Substring(3, editText.Text.Length - 3);
                                wsParam.wsMethodName = editText.Text.Substring(0, 2);
                                wsParam.session_id = string.Empty;
                                wsParam.stage = 1;
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(Intent.GetStringExtra("method")))
                                //{
                                //scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = GetString(Resource.String.message_wrong_operation), color = Color.Red });
                                wsParam.scaned = string.Empty;
                                wsParam.wsMethodName = string.Empty;
                                wsParam.session_id = string.Empty;
                                wsParam.stage = 0;
                                Toast.MakeText(this, GetString(Resource.String.message_wrong_operation), ToastLength.Short).Show();
                                Signalize.Error(this);
                                editText.Text = string.Empty;
                                //}
                                //else
                                //{
                                //    wsParam.stage = 1;
                                //    wsParam.wsMethodName = Intent.GetStringExtra("method");
                                //    wsParam.session_id = string.Empty;
                                //    wsParam.scaned = editText.Text;
                                //}


                            }
                            break;
                        }
                    default:
                        {
                            wsParam.scaned = editText.Text;
                            wsParam.stage += 1;
                            editText.Text = string.Empty;
                            break;
                        }
                }

                if (wsParam.stage > 0)
                {
                    try
                    {
                        using (var client = new ws2ApiClient(wsParam.ws_url))
                        {
                            var values = new Dictionary<string, string>
                            {
                             { "user_id", wsParam.user_id },
                             { "aparatoid", wsParam.aparatoid },
                             { "session_id",  wsParam.session_id },
                             { "stage", wsParam.stage.ToString() },
                             { "lang", wsParam.lang},
                             { "scaned", wsParam.scaned }
                            };

                            var Response = await client.PostAsync<rezAutoMode>(wsParam.wsMethodName, values);

                            operationList = Response.operation.operation_list;

                            if (Response.status == "00")
                            {
                                wsParam.stage = int.Parse(Response.stage);
                                wsParam.session_id = Response.session_id;
                                Signalize.Good(this);
                                //scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = message, color = Color.DarkGreen });
                                ////////////recyclerView.SetAdapter(currentAdapter);
                                message = Response.message;
                                editText.Hint = message;
                                if (int.Parse(Response.stage) == 0)
                                {
                                    message = GetString(Resource.String.message_scan_operation);
                                    editText.Hint = message;
                                    //////////recyclerView.SetAdapter(null);
                                    SetCurrentLoc(null);
                                    ///////////////currentAdapter = null;
                                }
                                else
                                {
                                    if (operationList != null) //&&  operationList.>0)
                                    {
                                        /////////////////currentAdapter = new ItemsAdapter(this, ShowAllItems ? operationList : operationList.FindAll(item => Convert.ToDecimal(item.kiekis, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")) == 00 ^ item.kiekis != item.kiekis_p), ShowAllItems);
                                       ////////////////recyclerView.SetAdapter(currentAdapter);
                                        SetCurrentLoc(Response.operation.active_loc); //Užpildome esamos lokacijos tekstą
                                    }
                                }
                            }
                            else
                            {
                                if (Response.stage != string.Empty)
                                {
                                    wsParam.stage = int.Parse(Response.stage);
                                }
                                else
                                {
                                    wsParam.stage = 0;
                                }
                                wsParam.session_id = Response.session_id;
                                Signalize.Error(this);
                                //scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = message, color = Color.Red });
                                message = Response.message;
                                editText.Hint = message;
                                description = Response.description;
                                Toast.MakeText(this, $"{Response.status} - {description}"
                                , ToastLength.Long).Show();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        wsParam.stage = 0;
                        wsParam.session_id = string.Empty;
                        wsParam.scaned = string.Empty;
                        operationList = null;
                        message = GetString(Resource.String.message_scan_operation);
                        editText.Hint = message;
                        SetCurrentLoc(null);
                        /////////////currentAdapter = null;
                        /////////////recyclerView.SetAdapter(null);
                        Signalize.Error(this);
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }
                    editText.Text = string.Empty;
                }

                editText.Enabled = true; //aktyvuojam barkodu input boxa
                editText.RequestFocus();
                editText.ShowSoftInputOnFocus = false;
            }

        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    navigationView.SelectedItemId = Resource.Id.menu_todo;
                    break;
                case 1:
                    navigationView.SelectedItemId = Resource.Id.menu_done;
                    break;
            }
        }

        private void _broadcastReceiver_VolumeChanged(object sender, EventArgs e)
        {
            Toast.MakeText(this, $"Volume changed to {wsParam.volume}", ToastLength.Short).Show();
        }

        private void _broadcastReceiver_ConnectivityChanged(object sender, EventArgs e)
        {
            if (IsOnline())
            {
                //Toast.MakeText(this, "Network Activated", ToastLength.Long).Show();
            }
            else
            {
                //Toast.MakeText(this, "Network NonActivated", ToastLength.Long).Show();
            }
        }

        private void NavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            int frag = 0;
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_barcode:
                    kb?.showSoftKeyboard(this, editText);
                    navigationView.SelectedItemId = selected_menu;
                    break;
                case Resource.Id.menu_todo:
                    kb?.hideSoftKeyboard(this);
                    frag = 0;
                    if (selected_menu != Resource.Id.menu_todo)
                    {
                        viewPager?.SetCurrentItem(frag, true);
                    }
                    selected_menu = Resource.Id.menu_todo;
                    break;
                case Resource.Id.menu_done:
                    kb?.hideSoftKeyboard(this);
                    frag = 1;
                    if (selected_menu != Resource.Id.menu_done)
                    {
                        viewPager?.SetCurrentItem(frag, true);
                    }
                    selected_menu = Resource.Id.menu_done;
                    break;
                case Resource.Id.menu_qty:
                    kb?.hideSoftKeyboard(this);
                    #region Quantity input dialog
                    View view = LayoutInflater.Inflate(Resource.Layout.QuantityInputLayout, null);
                    Android.Support.V7.App.AlertDialog builder = new Android.Support.V7.App.AlertDialog.Builder(this).Create();
                    builder.SetView(view);
                    builder.SetCanceledOnTouchOutside(false);
                    EditText quantity = view.FindViewById<EditText>(Resource.Id.editTextQuantity);
                    quantity.RequestFocus();
                    Button btnCancel = view.FindViewById<Button>(Resource.Id.btnExitAlertCancel);
                    Button btnOk = view.FindViewById<Button>(Resource.Id.btnExitAlertOK);
                    btnOk.Click += delegate
                    {
                        editText.Text = quantity.Text + "*";
                        editText.SetSelection(editText.Length());
                        builder.Dismiss();
                        builder.Dispose();
                        navigationView.SelectedItemId = selected_menu;
                    };
                    btnCancel.Click += delegate
                    {
                        builder.Dismiss();
                        builder.Dispose();
                        navigationView.SelectedItemId = selected_menu;
                    };
                    builder.Show();
                    #endregion
                    break;
            }
        }

        public void UpdateGui(string value)
        {
            //textView.Append(value);
            //textView.Text = textView.Text + value;
        }

        private bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (receiver != null)
            {
                try
                { RegisterReceiver(receiver, filter); }
                catch
                { }
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
            if (receiver != null)
            {
                try
                { UnregisterReceiver(receiver); }
                catch
                { }
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        private void SetCurrentLoc(string CurrentLoc)
        {
            if (!string.IsNullOrEmpty(CurrentLoc))
            {
                textViewAutoIcon.Visibility = ViewStates.Visible;
                textViewAutoAtiveLoc.Visibility = ViewStates.Visible;
                textViewAutoIcon.Text = "\uf1ad";
                textViewAutoAtiveLoc.Text = CurrentLoc;
            }
            else
            {
                textViewAutoIcon.Text = string.Empty;
                textViewAutoAtiveLoc.Text = string.Empty;
                textViewAutoIcon.Visibility = ViewStates.Gone;
                textViewAutoAtiveLoc.Visibility = ViewStates.Gone;
            }
        }
    }
}