using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BottomBarSharp {
    public class BottomBarBadge : TextView {

        const string StateCount = "STATE_BADGE_COUNT_FOR_TAB_";

        private int _count;
        /// <summary>
        /// Get the currently showing count for this Badge.
        /// Set the unread / new item / whatever count for this Badge.
        /// </summary>
        internal int Count {
            get {
                return _count;
            }
            set {
                _count = value;
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Is this badge currently visible?
        /// </summary>
        internal bool IsVisible { get; private set; }

        private FrameLayout badgeContainer;

        internal BottomBarBadge(Context context) : base(context) { }

        /// <summary>
        /// Shows the badge with a neat little scale animation.
        /// </summary>
        internal void Show() {
            IsVisible = true;
            ViewCompat.Animate(this)
                .SetDuration(150)
                .Alpha(1)
                .ScaleX(1)
                .ScaleY(1)
                .Start();
        }

        /// <summary>
        /// Hides the badge with a neat little scale animation.
        /// </summary>
        internal void Hide() {
            IsVisible = false;
            ViewCompat.Animate(this)
                .SetDuration(150)
                .Alpha(0)
                .ScaleX(0)
                .ScaleY(0)
                .Start();
        }

        internal void AttachToTab(BottomBarTab tab,int backgroundColor) {
            var parameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent);

            LayoutParameters = parameters;
            Gravity = GravityFlags.Center;
            MiscUtils.SetTextAppearance(this,Resource.Style.BB_BottomBarBadge_Text);

            SetColoredCircleBackground(backgroundColor);
            wrapTabAndBadgeInSameContainer(tab);
        }

        internal void SetColoredCircleBackground(int circleColor) {
            int innerPadding = MiscUtils.DpToPixel(Context,1);
            ShapeDrawable backgroundCircle = BadgeCircle.Make(innerPadding * 3,circleColor);
            SetPadding(innerPadding,innerPadding,innerPadding,innerPadding);
            setBackgroundCompat(backgroundCircle);
        }

        private void wrapTabAndBadgeInSameContainer(BottomBarTab tab) {
            var tabContainer = tab.Parent as ViewGroup;
            tabContainer.RemoveView(tab);

            badgeContainer = new FrameLayout(Context);
            badgeContainer.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent);

            badgeContainer.AddView(tab);
            badgeContainer.AddView(this);

            tabContainer.AddView(badgeContainer,tab.IndexInContainer);

            EventHandler handler = null;
            handler = (s,e) => {
                badgeContainer.ViewTreeObserver.GlobalLayout -= handler;
                AdjustPositionAndSize(tab);
            };
            badgeContainer.ViewTreeObserver.GlobalLayout += handler;
        }

        internal void RemoveFromTab(BottomBarTab tab) {
            FrameLayout badgeAndTabContainer = Parent as FrameLayout;
            ViewGroup originalTabContainer = badgeAndTabContainer.Parent as ViewGroup;

            badgeAndTabContainer.RemoveView(tab);
            originalTabContainer.RemoveView(badgeAndTabContainer);
            originalTabContainer.AddView(tab,tab.IndexInContainer);
        }

        internal void AdjustPositionAndSize(BottomBarTab tab) {
            AppCompatImageView iconView = tab.IconView;
            ViewGroup.LayoutParams parameters = LayoutParameters;

            int size = Math.Max(Width,Height);
            float xOffset = iconView.Width;

            if(tab.Type == BottomBarTabType.Tablet) {
                xOffset /= 1.25f;
            }

            SetX(iconView.GetX() + xOffset);
            TranslationY = 10;

            if(parameters.Width != size || parameters.Height != size) {
                parameters.Width = size;
                parameters.Height = size;
                LayoutParameters = parameters;
            }
        }

        private void setBackgroundCompat(Drawable background) {
            if(Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean) {
                Background = background;
            } else {
                SetBackgroundDrawable(background);
            }
        }

        internal Bundle SaveState(int tabIndex) {
            Bundle state = new Bundle();
            state.PutInt(StateCount + tabIndex,Count);
            return state;
        }

        internal void RestoreState(Bundle bundle,int tabIndex) {
            Count = bundle.GetInt(StateCount + tabIndex,Count);
        }
    }
}