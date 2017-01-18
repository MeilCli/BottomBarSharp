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
    internal class BatchTabPropertyApplier {

        private readonly BottomBar bottomBar;

        internal delegate void TabPropertyUpdate(BottomBarTab tab);

        public BatchTabPropertyApplier(BottomBar bottomBar) {
            this.bottomBar = bottomBar;
        }

        internal void ApplyToAllTabs(TabPropertyUpdate propertyUpdater) {
            int tabCount = bottomBar.TabCount;

            if(tabCount > 0) {
                for(int i = 0;i < tabCount;i++) {
                    var tab = bottomBar.GetTabAtPosition(i);
                    propertyUpdater(tab);
                }
            }
        }
    }
}