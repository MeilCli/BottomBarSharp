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

namespace BottomBarSharp {
    public class TabEvent :EventArgs {

        public int TabId { get; }

        public TabEvent(int tabId) {
            TabId = tabId;
        }
    }
}