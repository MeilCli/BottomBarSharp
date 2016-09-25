namespace BottomBarSharp {
    public interface IOnTabSelectListener {

        /// <summary>
        /// The method being called when currently visible <see cref="BottomBarTab"/> changes.
        /// 
        /// This listener is fired for the first time after the items have been set and
        /// also after a configuration change, such as when screen orientation changes
        /// from portrait to landscape.
        /// </summary>
        /// <param name="tabId">the new visible <see cref="BottomBarTab"/></param>
        void OnTabSelected(int tabId);
    }
}