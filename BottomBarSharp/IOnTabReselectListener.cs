namespace BottomBarSharp {

    /// <summary>
    /// For Java-like programming interface
    /// </summary>
    public interface IOnTabReselectListener {

        /// <summary>
        /// The method being called when currently visible <see cref="BottomBarTab"/> is
        /// reselected. Use this method for scrolling to the top of your content,
        /// as recommended by the Material Design spec
        /// </summary>
        /// <param name="tabId">the <see cref="BottomBarTab"/> that was reselected.</param>
        void OnTabReSelected(int tabId);
    }
}