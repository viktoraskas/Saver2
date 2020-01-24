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
using Microsoft.CSharp;

namespace Saver2.Helpers
{
    public class rezAutoMode
    {
        public string status { get; set; }
        public string description { get; set; }
        public string stage { get; set; }
        public string message { get; set; }
        public Operation operation { get; set; }
        public string session_id { get; set; }
    }

    public class OperationList
    {
        public string kodas_ll { get; set; }
        public string kodas_ln { get; set; }
        public string kodas_lc_a { get; set; }
        public string kodas_ls { get; set; }
        public string pav { get; set; }
        public string kodas_ps { get; set; }
        public string kodas_os { get; set; }
        public string serija { get; set; }
        public string kiekis { get; set; }
        public string kiekis_p { get; set; }
    }

    public class Operation
    {
        public string active_loc { get; set; }
        public List<OperationList> operation_list { get; set; }
    }
}