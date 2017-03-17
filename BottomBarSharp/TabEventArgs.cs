using System;

// disable not found xml document warning
#pragma warning disable 1591

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