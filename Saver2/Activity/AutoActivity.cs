﻿using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using static Saver2.helpers.ConstantsClass;
using Saver2.Helpers;
using Saver2.Activity;
using Android.Views;
using Saver2.helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Android.Views.InputMethods;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using Android.Support.V4.OS;
using Java.Util;
using System.Globalization;
using System.Linq;
using Android.Media;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Saver2.Adapters;
using Android.Graphics;
using Android.Support.V7.Widget;


namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    //[Activity(Label = "YourActivityName", Theme = "@android:style/Theme.NoTitleBar")]
    public class AutoActivity : AppCompatActivity
    {
        EditText editText;
        TextView icon1TextView, textViewAutoIcon, textViewAutoAtiveLoc, textViewAutoLocTotal, textViewAutoTotal;
        private Button btnShowAllItems;
        string message, AtiveLoc;
        private RecyclerView recyclerView;
        private ItemsAdapter itemsAdapter;
        private bool ShowAllItems;
        List<ScanLog2> scanLog;
        private List<OperationList> operationList;
        private dynamic currentAdapter;
        private string description;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AutoPageLayout);
            //Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn); //Prevent device to go to SLEEP
            Typeface font = Typeface.CreateFromAsset(Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            Typeface font2 = Typeface.CreateFromAsset(Assets, "fonts/Font Awesome 5 Free-Regular-400.otf");

            textViewAutoIcon = FindViewById<TextView>(Resource.Id.textViewAutoIcon);
            textViewAutoIcon.Typeface = font2;
            textViewAutoAtiveLoc = FindViewById<TextView>(Resource.Id.textViewAutoAtiveLoc);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.cardList);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.SetItemAnimator(new DefaultItemAnimator());
            editText = FindViewById<EditText>(Resource.Id.editTextBarcode);
            editText.Hint = GetString(Resource.String.hintScanBarcode);
            editText.KeyPress += EditText_KeyPress; 
            editText.RequestFocus();
            editText.LongClick += EditText_LongClick;
            editText.ShowSoftInputOnFocus = false;
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
            textViewAutoIcon.Visibility = ViewStates.Gone;
            textViewAutoAtiveLoc.Visibility = ViewStates.Gone;

            btnShowAllItems = FindViewById<Button>(Resource.Id.btnShowAllItems);
            btnShowAllItems.Text = GetString(Resource.String.ActionScannedItems);
            btnShowAllItems.Click += BtnShowAllItems_Click;

            wsParam.stage = 0;
            message = GetString(Resource.String.message_scan_operation);
            scanLog = new List<ScanLog2>();

            ShowAllItems = false;
            AtiveLoc = string.Empty;
        }

        private void EditText_LongClick(object sender, View.LongClickEventArgs e)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.QuantityInputLayout, null);
            AlertDialog builder = new AlertDialog.Builder(this).Create();
            builder.SetView(view);
            builder.SetCanceledOnTouchOutside(false);
            EditText quantity = view.FindViewById<EditText>(Resource.Id.editTextQuantity);
            quantity.RequestFocus();
            //InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //inputMethodManager.ShowSoftInput(quantity, ShowFlags.Implicit);
            //.ShowSoftInput(editText_name, InputMethodManager.ShowImplicit);
            Button btnCancel = view.FindViewById<Button>(Resource.Id.btnExitAlertCancel);
            Button btnOk = view.FindViewById<Button>(Resource.Id.btnExitAlertOK);
            btnOk.Click += delegate
            {
               editText.Text = quantity.Text+"*";
               editText.SetSelection(editText.Length());
               builder.Dismiss();
               builder.Dispose();
            };
            btnCancel.Click += delegate
            {
                builder.Dismiss();
                builder.Dispose();
            };
            builder.Show();

        }
        private void BtnShowAllItems_Click(object sender, System.EventArgs e)
        {
            if (operationList==null)
            {
                Toast.MakeText(this, GetString(Resource.String.message_nothing_to_show), ToastLength.Short).Show();
                return;
            }
            if (ShowAllItems)
            {
                ShowAllItems = false;
                btnShowAllItems.Text = GetString(Resource.String.ActionScannedItems);
                currentAdapter = new ItemsAdapter(this, ShowAllItems ? operationList : operationList.FindAll(item => Convert.ToDecimal(item.kiekis, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")) == 0 ^ item.kiekis != item.kiekis_p), ShowAllItems);
                recyclerView.SetAdapter(currentAdapter);
            }
            else
            {
                ShowAllItems = true;
                btnShowAllItems.Text = GetString(Resource.String.ActionLeftToScanItems);
                currentAdapter = new ItemsAdapter(this, operationList, ShowAllItems);
                recyclerView.SetAdapter(currentAdapter);
            }
        }
        private async void EditText_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                if (!IsInternetOn.CheckConnectivity(this))
                {
                    scanLog.Insert(0, new ScanLog2 { Scanned = "", Message = GetString(Resource.String.message_no_internet), color = Color.Red });
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
                                    scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = GetString(Resource.String.message_wrong_operation), color = Color.Red });
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
                                scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = message, color = Color.DarkGreen });
                                recyclerView.SetAdapter(currentAdapter);
                                message = Response.message;
                                editText.Hint = message;
                                if (int.Parse(Response.stage) == 0)
                                {
                                    message = GetString(Resource.String.message_scan_operation);
                                    editText.Hint = message;
                                    recyclerView.SetAdapter(null);
                                    SetCurrentLoc(null);
                                    currentAdapter = null;
                                }
                                else
                                {
                                    if (operationList!=null ) //&&  operationList.>0)
                                    {
                                        currentAdapter = new ItemsAdapter(this, ShowAllItems ? operationList : operationList.FindAll(item => Convert.ToDecimal(item.kiekis, CultureInfo.GetCultureInfoByIetfLanguageTag("en-US")) == 00 ^ item.kiekis != item.kiekis_p), ShowAllItems);
                                        recyclerView.SetAdapter(currentAdapter);
                                        SetCurrentLoc(Response.operation.active_loc); //Užpildome esamos lokacijos tekstą
                                    }
                                }
                            }
                            else
                            {
                                if (Response.stage!=string.Empty)
                                {
                                    wsParam.stage = int.Parse(Response.stage);
                                }
                                else
                                {
                                    wsParam.stage = 0;
                                }
                                wsParam.session_id = Response.session_id;
                                Signalize.Error(this);
                                scanLog.Insert(0, new ScanLog2 { Scanned = wsParam.scaned, Message = message, color = Color.Red });
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
                        currentAdapter = null;
                        recyclerView.SetAdapter(null);
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

        protected override void OnStart()
        {
            
            base.OnStart();
            editText.RequestFocus();
            editText.ShowSoftInputOnFocus = false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            editText.RequestFocus();
            editText.ShowSoftInputOnFocus = false;
            recyclerView.SetAdapter(currentAdapter);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            editText.RequestFocus();
            editText.ShowSoftInputOnFocus = false;
            recyclerView.SetAdapter(currentAdapter);
        }

        protected override void OnPause()
        {
            base.OnPause();
            currentAdapter=recyclerView.GetAdapter();
        }

        protected override void OnStop()
        {
            base.OnStop();
            currentAdapter = recyclerView.GetAdapter();
        }

        public override async void OnBackPressed()
        {
            if (wsParam.stage == 0)
            {
                base.OnBackPressed();
            }
            else
            {
                View view = LayoutInflater.Inflate(Resource.Layout.ShowExitAlertLayout, null);
                AlertDialog builder = new AlertDialog.Builder(this).Create();
                builder.SetView(view);
                builder.SetCanceledOnTouchOutside(false);
                Button bnt_cancel = view.FindViewById<Button>(Resource.Id.btnExitAlertCancel);
                Button btn_ok = view.FindViewById<Button>(Resource.Id.btnExitAlertOK);
                TextView text =
                    view.FindViewById<TextView>(Resource.Id.textAlert);
                builder.Show();
                bnt_cancel.Click += delegate
                {
                    builder.Dismiss();

                };
                btn_ok.Click += async (sender, e) =>
                {
                    btn_ok.Enabled = false;
                    bnt_cancel.Enabled = false;

                    wsParam.stage = 0;
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
                             { "scaned", string.Empty }
                        };
                            var Response = await client.PostAsync<rezAutoMode>(wsParam.wsMethodName, values);
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                        Signalize.Error(this);
                    }
                    base.OnBackPressed();
                    builder.Dismiss();
                };

            }
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