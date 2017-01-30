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

    /// <summary>
    /// Tab Event for seleceted or reselected
    /// </summary>
    public class TabEventArgs : EventArgs {

        /// <summary>
        /// An event tab id
        /// </summary>
        public int TabId { get; }

        public TabEventArgs(int tabId) {
            TabId = tabId;
        }
    }
}