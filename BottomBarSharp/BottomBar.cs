using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using static Android.Views.View;

// disable not found xml document warning
#pragma warning disable 1591

namespace BottomBarSharp {

    /// <summary>
    /// A view of BottomNavigationBar
    /// In axml, write bottombarsharp.BottomBar namespace
    /// </summary>
    public class BottomBar : LinearLayout, IOnClickListener, IOnLongClickListener {

        private const string stateCurrentSelectedTab = "STATE_CURRENT_SELECTED_TAB";
        private const float defaultInactiveShiftingTabAlpha = 0.6f;

        private BatchTabPropertyApplier batchPropertyApplier;

        // Behaviors
        private const int behaviorNone = 0;
        private const int behaviorShifting = 1;
        private const int behaviorShy = 2;
        private const int behaviorDrawUnderNav = 4;

        private int primaryColor;
        private int screenWidth;
        private int tenDp;
        private int maxFixedItemWidth;

        // XML Attributes
        private int tabXmlResource;
        private bool isTabletMode;
        private int behaviors;
        private float inActiveTabAlpha;
        private float activeTabAlpha;
        private int inActiveTabColor;
        private int activeTabColor;
        private int badgeBackgroundColor;
        private int titleTextAppearance;
        private Typeface titleTypeFace;
        private bool showShadow;

        private View backgroundOverlay;
        private ViewGroup outerContainer;
        private ViewGroup tabContainer;
        private View shadowView;

        private int defaultBackgroundColor = Color.White.ToArgb();
        private int currentBackgroundColor;

        /// <summary>
        /// Get the currently selected tab position.
        /// </summary>
        public int CurrentTabPosition { get; private set; }

        private int inActiveShiftingItemWidth;
        private int activeShiftingItemWidth;

        private IOnTabSelectListener onTabSelectListener;
        private IOnTabReselectListener onTabReselectListener;

        /// <summary>
        /// An event of tab selected
        /// </summary>
        public event EventHandler<TabEventArgs> TabSelect;

        /// <summary>
        /// An event of tab reselected
        /// </summary>
        public event EventHandler<TabEventArgs> TabReSelect;

        private bool isComingFromRestoredState;
        private bool ignoreTabReselectionListener;

        private bool shyHeightAlreadyCalculated;
        private bool navBarAccountedHeightCalculated;

        /// <summary>
        /// Number of BottomBar tab
        /// </summary>
        public int TabCount => tabContainer.ChildCount;

        /// <summary>
        /// Current visible tab
        /// </summary>
        public BottomBarTab CurrentTab => GetTabAtPosition(CurrentTabPosition);

        /// <summary>
        /// Get the resource id for the currently selected tab.
        /// </summary>
        public int CurrentTabId => CurrentTab.Id;

        private BottomBarTab[] currentTabs;

        public BottomBar(Context context) : base(context) {
            init(context, null);
        }

        public BottomBar(Context context, IAttributeSet attrs) : base(context, attrs) {
            init(context, attrs);
        }

        public BottomBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void init(Context context, IAttributeSet attrs) {
            batchPropertyApplier = new BatchTabPropertyApplier(this);

            populateAttributes(context, attrs);
            initializeViews();
            determineInitialBackgroundColor();

            if (tabXmlResource != 0) {
                SetItems(tabXmlResource);
            }
        }

        private void populateAttributes(Context context, IAttributeSet attrs) {
            primaryColor = MiscUtils.GetColor(Context, Resource.Attribute.colorPrimary);
            screenWidth = MiscUtils.GetScreenWidth(Context);
            tenDp = MiscUtils.DpToPixel(Context, 10);
            maxFixedItemWidth = MiscUtils.DpToPixel(Context, 168);

            TypedArray ta = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.BottomBar, 0, 0);

            try {
                tabXmlResource = ta.GetResourceId(Resource.Styleable.BottomBar_bb_tabXmlResource, 0);
                isTabletMode = ta.GetBoolean(Resource.Styleable.BottomBar_bb_tabletMode, false);
                behaviors = ta.GetInteger(Resource.Styleable.BottomBar_bb_behavior, behaviorNone);
                inActiveTabAlpha = ta.GetFloat(Resource.Styleable.BottomBar_bb_inActiveTabAlpha, isShiftingMode() ? defaultInactiveShiftingTabAlpha : 1);
                activeTabAlpha = ta.GetFloat(Resource.Styleable.BottomBar_bb_activeTabAlpha, 1);

                int defaultInActiveColor = isShiftingMode() ? Color.White.ToArgb() : ContextCompat.GetColor(context, Resource.Color.bb_inActiveBottomBarItemColor);
                int defaultActiveColor = isShiftingMode() ? Color.White.ToArgb() : primaryColor;

                inActiveTabColor = ta.GetColor(Resource.Styleable.BottomBar_bb_inActiveTabColor, defaultInActiveColor);
                activeTabColor = ta.GetColor(Resource.Styleable.BottomBar_bb_activeTabColor, defaultActiveColor);
                badgeBackgroundColor = ta.GetColor(Resource.Styleable.BottomBar_bb_badgeBackgroundColor, Color.Red.ToArgb());
                titleTextAppearance = ta.GetResourceId(Resource.Styleable.BottomBar_bb_titleTextAppearance, 0);
                titleTypeFace = getTypeFaceFromAsset(ta.GetString(Resource.Styleable.BottomBar_bb_titleTypeFace));
                showShadow = ta.GetBoolean(Resource.Styleable.BottomBar_bb_showShadow, true);
            } finally {
                ta.Recycle();
            }
        }

        private bool isShiftingMode() => !isTabletMode && hasBehavior(behaviorShifting);

        private bool drawUnderNav() {
            return !isTabletMode
                    && hasBehavior(behaviorDrawUnderNav)
                    && NavbarUtils.ShouldDrawBehindNavbar(Context);
        }

        private bool isShy() => !isTabletMode && hasBehavior(behaviorShy);

        private bool hasBehavior(int behavior) => (behaviors | behavior) == behaviors;

        private Typeface getTypeFaceFromAsset(string fontPath) {
            if (fontPath != null) {
                return Typeface.CreateFromAsset(Context.Assets, fontPath);
            }

            return null;
        }

        private void initializeViews() {
            int width = isTabletMode ? LayoutParams.WrapContent : LayoutParams.MatchParent;
            int height = isTabletMode ? LayoutParams.WrapContent : LayoutParams.WrapContent;
            var parameters = new LayoutParams(width, height);

            LayoutParameters = parameters;
            Orientation = isTabletMode ? Android.Widget.Orientation.Horizontal : Android.Widget.Orientation.Vertical;
            ViewCompat.SetElevation(this, MiscUtils.DpToPixel(Context, 8));

            View rootView = Inflate(Context, isTabletMode ? Resource.Layout.bb_bottom_bar_item_container_tablet : Resource.Layout.bb_bottom_bar_item_container, this);
            rootView.LayoutParameters = parameters;

            backgroundOverlay = rootView.FindViewById<View>(Resource.Id.bb_bottom_bar_background_overlay);
            outerContainer = rootView.FindViewById<ViewGroup>(Resource.Id.bb_bottom_bar_outer_container);
            tabContainer = rootView.FindViewById<ViewGroup>(Resource.Id.bb_bottom_bar_item_container);
            shadowView = rootView.FindViewById<View>(Resource.Id.bb_bottom_bar_shadow);

            if (!showShadow) {
                shadowView.Visibility = ViewStates.Gone;
            }
        }

        private void determineInitialBackgroundColor() {
            if (isShiftingMode()) {
                defaultBackgroundColor = primaryColor;
            }

            Drawable userDefinedBackground = Background;

            bool userHasDefinedBackgroundColor = userDefinedBackground != null && userDefinedBackground is ColorDrawable;

            if (userHasDefinedBackgroundColor) {
                defaultBackgroundColor = ((ColorDrawable)userDefinedBackground).Color.ToArgb();
                SetBackgroundColor(Color.Transparent);
            }
        }

        /// <summary>
        /// Set the items for the BottomBar from XML Resource.
        /// </summary>
        /// <param name="xmlRes"></param>
        public void SetItems(int xmlRes) {
            SetItems(xmlRes, null);
        }

        /// <summary>
        /// Set the item for the BottomBar from XML Resource with a default configuration
        /// for each tab.
        /// </summary>
        /// <param name="xmlRes"></param>
        /// <param name="defaultTabConfig"></param>
        public void SetItems(int xmlRes, BottomBarTabConfig defaultTabConfig) {
            if (xmlRes == 0) {
                throw new RuntimeException("No items specified for the BottomBar!");
            }

            if (defaultTabConfig == null) {
                defaultTabConfig = getTabConfig();
            }

            TabParser parser = new TabParser(Context, defaultTabConfig, xmlRes);
            updateItems(parser.Tabs);
        }

        /// <summary>
        /// To added, Set the item for the BottomBar from dynamic item
        /// </summary>
        /// <param name="tabs"></param>
        public void SetItems(List<BottomBarTab> tabs) {
            for (int i = 0; i < tabs.Count; i++) {
                tabs[i].IndexInContainer = i;
            }
            updateItems(tabs);
        }

        /// <summary>
        /// To added, new BottomBarTab with this instance for dynamic to set item, this method is not adding to BottomBar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="icon"></param>
        /// <param name="title"></param>
        /// <param name="defaultTabConfig"></param>
        /// <returns></returns>
        public BottomBarTab NewTab(int id, int icon, string title, BottomBarTabConfig defaultTabConfig = null) {
            if (defaultTabConfig == null) {
                defaultTabConfig = getTabConfig();
            }

            var tab = new BottomBarTab(Context);
            tab.SetConfig(defaultTabConfig);
            tab.Id = id;
            tab.IconResId = icon;
            tab.Title = title;
            return tab;
        }

        private BottomBarTabConfig getTabConfig() {
            return new BottomBarTabConfig() {
                InActiveTabAlpha = inActiveTabAlpha,
                ActiveTabAlpha = activeTabAlpha,
                InActiveTabColor = inActiveTabColor,
                ActiveTabColor = activeTabColor,
                BarColorWhenSelected = defaultBackgroundColor,
                BadgeBackgroundColor = badgeBackgroundColor,
                TitleTextAppearance = titleTextAppearance,
                TitleTypeFace = titleTypeFace
            };
        }

        private void updateItems(List<BottomBarTab> bottomBarItems) {
            tabContainer.RemoveAllViews();

            int index = 0;
            int biggestWidth = 0;

            var viewsToAdd = new BottomBarTab[bottomBarItems.Count];

            foreach (BottomBarTab bottomBarTab in bottomBarItems) {
                BottomBarTabType type;

                if (isShiftingMode()) {
                    type = BottomBarTabType.Shifting;
                } else if (isTabletMode) {
                    type = BottomBarTabType.Tablet;
                } else {
                    type = BottomBarTabType.Fixed;
                }

                bottomBarTab.Type = type;
                bottomBarTab.PrepareLayout();

                if (index == CurrentTabPosition) {
                    bottomBarTab.Select(false);

                    handleBackgroundColorChange(bottomBarTab, false);
                } else {
                    bottomBarTab.Deselect(false);
                }

                if (!isTabletMode) {
                    if (bottomBarTab.Width > biggestWidth) {
                        biggestWidth = bottomBarTab.Width;
                    }

                    viewsToAdd[index] = bottomBarTab;
                } else {
                    tabContainer.AddView(bottomBarTab);
                }

                bottomBarTab.SetOnClickListener(this);
                bottomBarTab.SetOnLongClickListener(this);
                index++;
            }

            currentTabs = viewsToAdd;

            if (!isTabletMode) {
                resizeTabsToCorrectSizes(viewsToAdd);
            }
        }

        private void resizeTabsToCorrectSizes(BottomBarTab[] tabsToAdd) {
            int viewWidth = MiscUtils.PixelToDp(Context, Width);
            if (viewWidth <= 0 || viewWidth > screenWidth) {
                viewWidth = screenWidth;
            }

            int proposedItemWidth = Java.Lang.Math.Min(
                    MiscUtils.DpToPixel(Context, viewWidth / tabsToAdd.Length),
                    maxFixedItemWidth
            );

            inActiveShiftingItemWidth = (int)(proposedItemWidth * 0.9);
            activeShiftingItemWidth = (int)(proposedItemWidth + (proposedItemWidth * (tabsToAdd.Length * 0.1)));
            int height = Java.Lang.Math.Round(Context.Resources.GetDimension(Resource.Dimension.bb_height));

            foreach (BottomBarTab tabView in tabsToAdd) {
                ViewGroup.LayoutParams parameters = tabView.LayoutParameters;
                parameters.Height = height;

                if (isShiftingMode()) {
                    if (tabView.IsActive) {
                        parameters.Width = activeShiftingItemWidth;
                    } else {
                        parameters.Width = inActiveShiftingItemWidth;
                    }
                } else {
                    parameters.Width = proposedItemWidth;
                }

                if (tabView.Parent == null) {
                    tabContainer.AddView(tabView);
                }

                tabView.RequestLayout();
            }
        }

        /// <summary>
        /// Set a listener that gets fired when the selected tab changes.
        /// 
        /// Note: Will be immediately called for the currently selected tab
        /// once when set.
        /// </summary>
        /// <param name="listener">a listener for monitoring changes in tab selection.</param>
        public void SetOnTabSelectListener(IOnTabSelectListener listener) {
            SetOnTabSelectListener(listener, true);
        }

        /// <summary>
        /// Set a listener that gets fired when the selected tab changes.
        /// 
        /// If shouldFireInitially is set to false, this listener isn't fired straight away
        /// it's set, but you'll get all events normally for consecutive tab selection changes.
        /// </summary>
        /// <param name="listener">a listener for monitoring changes in tab selection.</param>
        /// <param name="shouldFireInitially">whether the listener should be fired the first time it's set.</param>
        public void SetOnTabSelectListener(IOnTabSelectListener listener, bool shouldFireInitially) {
            onTabSelectListener = listener;

            if (shouldFireInitially && listener != null && TabCount > 0) {
                listener.OnTabSelected(CurrentTabId);
            }
        }

        /// <summary>
        /// Set a listener that gets fired when a currently selected tab is clicked.
        /// </summary>
        /// <param name="listener">a listener for handling tab reselections.</param>
        public void SetOnTabReselectListener(IOnTabReselectListener listener) {
            onTabReselectListener = listener;
        }

        /// <summary>
        /// Set the default selected to be the tab with the corresponding tab id.
        /// By default, the first tab in the container is the default tab.
        /// </summary>
        /// <param name="defaultTabId"></param>
        public void SetDefaultTab(int defaultTabId) {
            int defaultTabPosition = FindPositionForTabWithId(defaultTabId);
            SetDefaultTabPosition(defaultTabPosition);
        }

        /// <summary>
        /// Sets the default tab for this BottomBar that is shown until the user changes
        /// the selection.
        /// </summary>
        /// <param name="defaultTabPosition">the default tab position.</param>
        public void SetDefaultTabPosition(int defaultTabPosition) {
            if (isComingFromRestoredState)
                return;

            SelectTabAtPosition(defaultTabPosition);
        }

        /// <summary>
        /// Select the tab with the corresponding id.
        /// </summary>
        /// <param name="tabResId"></param>
        public void SelectTabWithId(int tabResId) {
            int tabPosition = FindPositionForTabWithId(tabResId);
            SelectTabAtPosition(tabPosition);
        }

        /// <summary>
        /// Select a tab at the specified position.
        /// </summary>
        /// <param name="position">the position to select.</param>
        public void SelectTabAtPosition(int position) {
            SelectTabAtPosition(position, false);
        }

        /// <summary>
        /// Select a tab at the specified position.
        /// </summary>
        /// <param name="position">the position to select.</param>
        /// <param name="animate">should the tab change be animated or not.</param>
        public void SelectTabAtPosition(int position, bool animate) {
            if (position > TabCount - 1 || position < 0) {
                throw new IndexOutOfBoundsException("Can't select tab at position " +
                        position + ". This BottomBar has no items at that position.");
            }

            BottomBarTab oldTab = CurrentTab;
            BottomBarTab newTab = GetTabAtPosition(position);

            oldTab.Deselect(animate);
            newTab.Select(animate);

            updateSelectedTab(position);
            shiftingMagic(oldTab, newTab, animate);
            handleBackgroundColorChange(newTab, animate);
        }

        /// <summary>
        /// Get the tab at the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public BottomBarTab GetTabAtPosition(int position) {
            View child = tabContainer.GetChildAt(position);

            if (child is BadgeContainer) {
                return findTabInLayout((BadgeContainer)child);
            }

            return (BottomBarTab)child;
        }

        /// <summary>
        /// Find the tabs' position in the container by id.
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public int FindPositionForTabWithId(int tabId) {
            return GetTabWithId(tabId).IndexInContainer;
        }

        /// <summary>
        /// Find a BottomBarTab with the corresponding id.
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public BottomBarTab GetTabWithId(int tabId) {
            return tabContainer.FindViewById<BottomBarTab>(tabId);
        }

        /// <summary>
        /// Set alpha value used for inactive BottomBarTabs.
        /// </summary>
        /// <param name="alpha"></param>
        public void SetInActiveTabAlpha(float alpha) {
            inActiveTabAlpha = alpha;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.InActiveAlpha = inActiveTabAlpha);
        }

        /// <summary>
        /// Set alpha value used for active BottomBarTabs.
        /// </summary>
        /// <param name="alpha"></param>
        public void SetActiveTabAlpha(float alpha) {
            activeTabAlpha = alpha;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.ActiveAlpha = activeTabAlpha);
        }

        /// <summary>
        /// Set inactive color used for selected BottomBarTabs.
        /// </summary>
        /// <param name="color"></param>
        public void SetInActiveTabColor(int color) {
            inActiveTabColor = color;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.InActiveColor = inActiveTabColor);
        }

        /// <summary>
        /// Set active color used for selected BottomBarTabs.
        /// </summary>
        /// <param name="color"></param>
        public void SetActiveTabColor(int color) {
            activeTabColor = color;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.ActiveColor = activeTabColor);
        }

        /// <summary>
        /// Set background color for the badge.
        /// </summary>
        /// <param name="color"></param>
        public void SetBadgeBackgroundColor(int color) {
            badgeBackgroundColor = color;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.BadgeBackgroundColor = badgeBackgroundColor);
        }

        /// <summary>
        /// Set custom text apperance for all BottomBarTabs.
        /// </summary>
        /// <param name="textAppearance"></param>
        public void SetTabTitleTextAppearance(int textAppearance) {
            titleTextAppearance = textAppearance;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.TitleTextAppearanceResId = titleTextAppearance);
        }

        /// <summary>
        /// Set a custom typeface for all tab's titles.
        /// </summary>
        /// <param name="fontPath">
        /// path for your custom font file, such as fonts/MySuperDuperFont.ttf.
        /// In that case your font path would look like src/main/assets/fonts/MySuperDuperFont.ttf,
        /// but you only need to provide fonts/MySuperDuperFont.ttf, as the asset folder
        /// will be auto-filled for you.
        /// </param>
        public void SetTabTitleTypeface(string fontPath) {
            Typeface actualTypeface = getTypeFaceFromAsset(fontPath);
            SetTabTitleTypeface(actualTypeface);
        }

        /// <summary>
        /// Set a custom typeface for all tab's titles.
        /// </summary>
        /// <param name="typeface"></param>
        public void SetTabTitleTypeface(Typeface typeface) {
            titleTypeFace = typeface;

            batchPropertyApplier.ApplyToAllTabs(tab => tab.TitleTypeFace = titleTypeFace);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
            base.OnLayout(changed, left, top, right, bottom);
            if (changed) {
                if (!isTabletMode) {
                    resizeTabsToCorrectSizes(currentTabs);
                }

                updateTitleBottomPadding();

                if (isShy()) {
                    initializeShyBehavior();
                }

                if (drawUnderNav()) {
                    resizeForDrawingUnderNavbar();
                }
            }
        }

        private void updateTitleBottomPadding() {
            int tabCount = TabCount;

            if (tabContainer == null || tabCount == 0 || !isShiftingMode()) {
                return;
            }

            for (int i = 0; i < tabCount; i++) {
                BottomBarTab tab = GetTabAtPosition(i);
                TextView title = tab.TitleView;

                if (title == null) {
                    continue;
                }

                int baseline = title.Baseline;
                int height = title.Height;
                int paddingInsideTitle = height - baseline;
                int missingPadding = tenDp - paddingInsideTitle;

                if (missingPadding > 0) {
                    title.SetPadding(title.PaddingLeft, title.PaddingTop, title.PaddingRight, missingPadding + title.PaddingBottom);
                }
            }
        }

        private void initializeShyBehavior() {
            IViewParent parent = Parent;

            bool hasAbusiveParent = parent != null && parent is CoordinatorLayout;

            if (!hasAbusiveParent) {
                throw new RuntimeException("In order to have shy behavior, the " +
                        "BottomBar must be a direct child of a CoordinatorLayout.");
            }

            if (!shyHeightAlreadyCalculated) {
                int height = Height;

                if (height != 0) {
                    updateShyHeight(height);
                    shyHeightAlreadyCalculated = true;
                }
            }
        }

        private void updateShyHeight(int height) {
            ((CoordinatorLayout.LayoutParams)LayoutParameters).Behavior = new BottomNavigationBehavior<View>(height, 0, false);
        }

        private void resizeForDrawingUnderNavbar() {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat) {
                int currentHeight = Height;

                if (currentHeight != 0 && !navBarAccountedHeightCalculated) {
                    navBarAccountedHeightCalculated = true;
                    tabContainer.LayoutParameters.Height = currentHeight;

                    int navbarHeight = NavbarUtils.GetNavbarHeight(Context);
                    int finalHeight = currentHeight + navbarHeight;
                    LayoutParameters.Height = finalHeight;

                    if (isShy()) {
                        updateShyHeight(finalHeight);
                    }
                }
            }
        }

        protected override IParcelable OnSaveInstanceState() {
            Bundle bundle = SaveState();
            bundle.PutParcelable("superstate", base.OnSaveInstanceState());
            return bundle;
        }

        internal Bundle SaveState() {
            var outState = new Bundle();
            outState.PutInt(stateCurrentSelectedTab, CurrentTabPosition);

            return outState;
        }

        protected override void OnRestoreInstanceState(IParcelable state) {
            if (state is Bundle) {
                var bundle = state as Bundle;
                RestoreState(bundle);

                state = bundle.GetParcelable("superstate") as IParcelable;
            }
            base.OnRestoreInstanceState(state);
        }

        internal void RestoreState(Bundle savedInstanceState) {
            if (savedInstanceState != null) {
                isComingFromRestoredState = true;
                ignoreTabReselectionListener = true;

                int restoredPosition = savedInstanceState.GetInt(stateCurrentSelectedTab, CurrentTabPosition);
                SelectTabAtPosition(restoredPosition, false);
            }
        }

        public void OnClick(View v) => handleClick(v);

        public bool OnLongClick(View v) => handleLongClick(v);

        private BottomBarTab findTabInLayout(ViewGroup child) {
            for (int i = 0; i < child.ChildCount; i++) {
                View candidate = child.GetChildAt(i);

                if (candidate is BottomBarTab) {
                    return candidate as BottomBarTab;
                }
            }

            return null;
        }

        private void handleClick(View v) {
            BottomBarTab oldTab = CurrentTab;
            var newTab = v as BottomBarTab;

            oldTab.Deselect(true);
            newTab.Select(true);

            shiftingMagic(oldTab, newTab, true);
            handleBackgroundColorChange(newTab, true);
            updateSelectedTab(newTab.IndexInContainer);
        }

        private bool handleLongClick(View v) {
            if (v is BottomBarTab) {
                var longClickedTab = v as BottomBarTab;

                if ((isShiftingMode() || isTabletMode) && !longClickedTab.IsActive) {
                    Toast.MakeText(Context, longClickedTab.Title, ToastLength.Short).Show();
                }
            }

            return true;
        }

        private void updateSelectedTab(int newPosition) {
            int newTabId = GetTabAtPosition(newPosition).Id;

            if (newPosition != CurrentTabPosition) {
                onTabSelectListener?.OnTabSelected(newTabId);
                TabSelect?.Invoke(this, new TabEventArgs(newTabId));
            } else if (!ignoreTabReselectionListener) {
                onTabReselectListener?.OnTabReSelected(newTabId);
                TabReSelect?.Invoke(this, new TabEventArgs(newTabId));
            }

            CurrentTabPosition = newPosition;

            if (ignoreTabReselectionListener) {
                ignoreTabReselectionListener = false;
            }
        }

        private void shiftingMagic(BottomBarTab oldTab, BottomBarTab newTab, bool animate) {
            if (isShiftingMode()) {
                oldTab.UpdateWidth(inActiveShiftingItemWidth, animate);
                newTab.UpdateWidth(activeShiftingItemWidth, animate);
            }
        }

        private void handleBackgroundColorChange(BottomBarTab tab, bool animate) {
            int newColor = tab.BarColorWhenSelected;

            if (currentBackgroundColor == newColor) {
                return;
            }

            if (!animate) {
                outerContainer.SetBackgroundColor(new Color(newColor));
                return;
            }

            View clickedView = tab;

            if (tab.HasActiveBadge) {
                clickedView = tab.OuterView;
            }

            animateBGColorChange(clickedView, newColor);
            currentBackgroundColor = newColor;
        }

        private void animateBGColorChange(View clickedView, int newColor) {
            prepareForBackgroundColorAnimation(newColor);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                if (!outerContainer.IsAttachedToWindow) {
                    return;
                }

                backgroundCircularRevealAnimation(clickedView, newColor);
            } else {
                backgroundCrossfadeAnimation(newColor);
            }
        }

        private void prepareForBackgroundColorAnimation(int newColor) {
            outerContainer.ClearAnimation();
            backgroundOverlay.ClearAnimation();

            backgroundOverlay.SetBackgroundColor(new Color(newColor));
            backgroundOverlay.Visibility = ViewStates.Visible;
        }

        private void backgroundCircularRevealAnimation(View clickedView, int newColor) {
            int centerX = (int)(ViewCompat.GetX(clickedView) + (clickedView.MeasuredWidth / 2));
            int yOffset = isTabletMode ? (int)ViewCompat.GetY(clickedView) : 0;
            int centerY = yOffset + clickedView.MeasuredHeight / 2;
            int startRadius = 0;
            int finalRadius = isTabletMode ? outerContainer.Height : outerContainer.Width;

            Animator animator = ViewAnimationUtils.CreateCircularReveal(
                    backgroundOverlay,
                    centerX,
                    centerY,
                    startRadius,
                    finalRadius
            );

            if (isTabletMode) {
                animator.SetDuration(500);
            }

            var handler = new EventHandler((s, e) => {
                outerContainer.SetBackgroundColor(new Color(newColor));
                backgroundOverlay.Visibility = ViewStates.Invisible;
                ViewCompat.SetAlpha(backgroundOverlay, 1);
            });

            animator.AnimationEnd += handler;
            animator.AnimationCancel += handler;

            animator.Start();
        }

        private void backgroundCrossfadeAnimation(int newColor) {
            ViewCompat.SetAlpha(backgroundOverlay, 0);
            var animator = ViewCompat.Animate(backgroundOverlay)
                .Alpha(1);

            var handler = new EventHandler((s, e) => {
                outerContainer.SetBackgroundColor(new Color(newColor));
                backgroundOverlay.Visibility = ViewStates.Invisible;
                ViewCompat.SetAlpha(backgroundOverlay, 1);
            });

            var listener = new ViewPropertyAnimatorListenerAdapter();
            listener.Handler += handler;
            animator.SetListener(listener);

            animator.Start();
        }

        /// <summary>
        /// Toggle translation of BottomBar to hidden and visible in a CoordinatorLayout.
        /// </summary>
        /// <param name="visible">true resets translation to 0, false translates view to hidden</param>
        private void toggleShyVisibility(bool visible) {
            BottomNavigationBehavior<BottomBar> from = BottomNavigationBehavior<BottomBar>.From(this);
            if (from != null) {
                from.SetHidden(this, visible);
            }
        }

        private class ViewPropertyAnimatorListenerAdapter : IViewPropertyAnimatorListener {

            public event EventHandler Handler;

            public IntPtr Handle { get; set; }

            public void Dispose() {
            }

            public void OnAnimationCancel(View view) {
                Handler.Invoke(this, new EventArgs());
            }

            public void OnAnimationEnd(View view) {
                Handler.Invoke(this, new EventArgs());
            }

            public void OnAnimationStart(View view) {
            }
        }
    }
}