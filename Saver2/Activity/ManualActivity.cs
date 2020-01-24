using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Saver2.Adapters;
using Saver2.helpers;
using Saver2.Helpers;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Saver2.Activity
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class ManualActivity : AppCompatActivity
    {
        EditText editText;
        TextView icon1TextView, textViewAutoIcon, textViewAutoAtiveLoc, textViewAutoLocTotal, textViewAutoTotal;
        private Button btnShowAllItems;
        string message, AtiveLoc;
        private RecyclerView recyclerView;
        private bool ShowAllItems;
        private List<OperationList> operationList;
        private List<OperationList> viewList;
        private ObservableCollection<OperationList> viewOList;
        private ManualAdapter manualAdapter;
        private string description;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AutoPageLayout);
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
            ShowAllItems = false;
            AtiveLoc = string.Empty;
            wsParam.wsMethodName = string.Empty;
            wsParam.wsMethodName = Intent.GetStringExtra("method");
            
            viewList = new List<OperationList>();
            viewOList = new ObservableCollection<OperationList>();
            manualAdapter = new ManualAdapter(this, viewOList);
            recyclerView.SetAdapter(manualAdapter);
        }

        protected override void OnStart()
        {
            base.OnStart();
            //Toast.MakeText(this,$"/{method}/",ToastLength.Long).Show();
        }

        private void BtnShowAllItems_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //viewList.RemoveAt(1);
            //itemsAdapter.NotifyItemRemoved(1);
        }

        private void EditText_LongClick(object sender, View.LongClickEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void EditText_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                //Toast.MakeText(this,$"Enter - {editText.Text}",ToastLength.Short).Show();
                //--
                if (string.IsNullOrEmpty(editText.Text))
                {
                    //beep error and vibrate
                    return;
                }
                
                if (wsParam.stage==0)
                {
                    // do job
                }

                wsParam.scaned = editText.Text;
                wsParam.stage += 1;
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

                        if (Response.status == "00")
                        {
                            wsParam.stage = int.Parse(Response.stage);
                            wsParam.session_id = Response.session_id;
                            Signalize.Good(this);

                            if (int.Parse(Response.stage) == 0)
                            {
                                message = GetString(Resource.String.message_scan_operation);
                                editText.Hint = message;
                                return;
                            }
                            operationList = Response.operation.operation_list;

                            if (operationList != null) 
                            {
                                recyclerView.SetAdapter(new ManualAdapter(this, new ObservableCollection<OperationList>(operationList)));
                                SetCurrentLoc(Response.operation.active_loc);
                            }

                            message = Response.message;
                            
                        }
                        else
                        {
                            Signalize.Error(this);
                            message = Response.message;
                            description = Response.description;
                            Toast.MakeText(this, $"{Response.status} - {description}"
                            , ToastLength.Long).Show();
                            if (Response.stage == String.Empty ^ Response.session_id==String.Empty)
                            {
                                wsParam.stage = 0;
                                wsParam.session_id = string.Empty;
                                wsParam.scaned = string.Empty;
                                operationList = null;
                                recyclerView.SetAdapter(null);
                            }
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
                    SetCurrentLoc(null);
                    recyclerView.SetAdapter(null);
                    Signalize.Error(this);
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

                //--
                editText.Hint = message;
                editText.Text = String.Empty;
            }
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