using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Saver2.Helpers
{
    public static class TaskExtensionsClass
    {
        public static async Task<T> ExecuteAsyncOperation<T>(this Task<T> operation)
        {
            try
            {
                return await operation;
            }
            catch
            {
                return default(T);
            }
        }
    }
}