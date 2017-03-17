using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

// disable not found xml document warning
#pragma warning disable 1591

namespace BottomBarSharp {

    public enum BottomBarTabType {
        Fixed, Shifting, Tablet
    }

    public class BottomBarTabConfig {
        public float InActiveTabAlpha { get; set; }
        public float ActiveTabAlpha { get; set; }
        public int InActiveTabColor { get; set; }
        public int ActiveTabColor { get; set; }
        public int BarColorWhenSelected { get; set; }
        public int BadgeBackgroundColor { get; set; }
        public int TitleTextAppearance { get; set; }
        public Typeface TitleTypeFace { get; set; }
    }

    public class BottomBarTab : LinearLayout {

        private const string stateBadgeCount = "STATE_BADGE_COUNT_FOR_TAB_";

        private const long animationDuration = 150;
        private const float activeTitleScale = 1;
        private const float inactiveFixedTitleScale = 0.86f;

        private readonly int sixDps;
        private readonly int eightDps;
        private readonly int sixteenDps;

        internal BottomBarTabType Type { get; set; } = BottomBarTabType.Fixed;
        public int IconResId { get; internal set; }

        private string _title;
        public string Title {
            get {
                return _title;
            }
            set {
                _title = value;
                updateTitle();
            }
        }

        private float _inActiveAlpha;
        public float InActiveAlpha {
            get {
                return _inActiveAlpha;
            }
            set {
                _inActiveAlpha = value;
                if (!IsActive) {
                    setAlphas(value);
                }
            }
        }

        private float _activeAlpha;
        public float ActiveAlpha {
            get {
                return _activeAlpha;
            }
            set {
                _activeAlpha = value;
                if (IsActive) {
                    setAlphas(value);
                }
            }
        }

        private int _inActiveColor;
        public int InActiveColor {
            get {
                return _inActiveColor;
            }
            set {
                _inActiveColor = value;
                if (!IsActive) {
                    setColors(value);
                }
            }
        }

        private int _activeColor;
        public int ActiveColor {
            get {
                return _activeColor;
            }
            set {
                _activeColor = value;
                if (IsActive) {
                    setColors(value);
                }
            }
        }

        public int BarColorWhenSelected { get; set; }

        private int _badgeBackgroundColor;
        public int BadgeBackgroundColor {
            get {
                return _badgeBackgroundColor;
            }
            set {
                _badgeBackgroundColor = value;
                Badge?.SetColoredCircleBackground(value);
            }
        }

        internal AppCompatImageView IconView { get; private set; }
        internal TextView TitleView { get; private set; }
        internal bool IsActive { get; private set; }

        internal int IndexInContainer { get; set; }

        internal BottomBarBadge Badge;

        private int _titleTextAppearanceResId;
        public int TitleTextAppearanceResId {
            get {
                return _titleTextAppearanceResId;
            }
            internal set {
                _titleTextAppearanceResId = value;
                updateCustomTextAppearance();
            }
        }

        private Typeface _titleTypeFace;
        public Typeface TitleTypeFace {
            get {
                return _titleTypeFace;
            }
            set {
                _titleTypeFace = value;
                updateCustomTypeface();
            }
        }

        public ViewGroup OuterView {
            get {
                return Parent as ViewGroup;
            }
        }

        internal int CurrentDisplayedIconColor {
            get {
                var tag = IconView.Tag;
                if (tag is Java.Lang.Integer) {
                    return (tag as Java.Lang.Integer).IntValue();
                }
                return 0;
            }
        }

        internal int CurrentDisplayedTitleColor => TitleView?.CurrentTextColor ?? 0;

        internal int CurrentDisplayedTextAppearance {
            get {
                var tag = TitleView.Tag;
                if (TitleView != null && tag is Java.Lang.Integer) {
                    return (tag as Java.Lang.Integer).IntValue();
                }
                return 0;
            }
        }

        internal bool HasActiveBadge => Badge != null;

        internal BottomBarTab(Context context) : base(context) {
            sixDps = MiscUtils.DpToPixel(context, 6);
            eightDps = MiscUtils.DpToPixel(context, 8);
            sixteenDps = MiscUtils.DpToPixel(context, 16);
        }

        internal void SetConfig(BottomBarTabConfig config) {
            InActiveAlpha = config.InActiveTabAlpha;
            ActiveAlpha = config.ActiveTabAlpha;
            InActiveColor = config.InActiveTabColor;
            ActiveColor = config.ActiveTabColor;
            BarColorWhenSelected = config.BarColorWhenSelected;
            BadgeBackgroundColor = config.BadgeBackgroundColor;
            TitleTextAppearanceResId = config.TitleTextAppearance;
            TitleTypeFace = config.TitleTypeFace;
        }

        internal void PrepareLayout() {
            Inflate(Context, GetLayoutResource(), this);
            Orientation = Orientation.Vertical;
            SetGravity(GravityFlags.CenterHorizontal);
            LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            IconView = FindViewById<AppCompatImageView>(Resource.Id.bb_bottom_bar_icon);
            IconView.SetImageResource(IconResId);

            if (Type != BottomBarTabType.Tablet) {
                TitleView = FindViewById<TextView>(Resource.Id.bb_bottom_bar_title);
                updateTitle();
            }

            updateCustomTextAppearance();
            updateCustomTypeface();
        }

        private void updateTitle() {
            if (TitleView != null) {
                TitleView.Text = Title;
            }
        }

        internal int GetLayoutResource() {
            int layoutResource;
            switch (Type) {
                case BottomBarTabType.Fixed:
                    layoutResource = Resource.Layout.bb_bottom_bar_item_fixed;
                    break;
                case BottomBarTabType.Shifting:
                    layoutResource = Resource.Layout.bb_bottom_bar_item_shifting;
                    break;
                case BottomBarTabType.Tablet:
                    layoutResource = Resource.Layout.bb_bottom_bar_item_fixed_tablet;
                    break;
                default:
                    // should never happen
                    throw new RuntimeException("Unknown BottomBarTab type.");
            }
            return layoutResource;
        }

        private void updateCustomTextAppearance() {
            if (TitleView == null || TitleTextAppearanceResId == 0) {
                return;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M) {
                TitleView.SetTextAppearance(TitleTextAppearanceResId);
            } else {
#pragma warning disable 0618
                TitleView.SetTextAppearance(Context, TitleTextAppearanceResId);
#pragma warning restore 0618
            }

            TitleView.Tag = new Java.Lang.Integer(TitleTextAppearanceResId);
        }

        private void updateCustomTypeface() {
            if (TitleTypeFace != null && TitleView != null) {
                TitleView.Typeface = TitleTypeFace;
            }
        }

        public void SetBadgeCount(int count) {
            if (count <= 0) {
                if (Badge != null) {
                    Badge.RemoveFromTab(this);
                    Badge = null;
                }

                return;
            }

            if (Badge == null) {
                Badge = new BottomBarBadge(Context);
                Badge.AttachToTab(this, BadgeBackgroundColor);
            }

            Badge.Count = count;
        }

        public void RemoveBadge() {
            SetBadgeCount(0);
        }

        internal void SetIconTint(int tint) {
            IconView.SetColorFilter(new Color(tint));
        }

        internal void Select(bool animate) {
            IsActive = true;

            if (animate) {
                setTopPaddingAnimated(IconView.PaddingTop, sixDps);
                animateIcon(ActiveAlpha);
                animateTitle(activeTitleScale, ActiveAlpha);
                animateColors(InActiveColor, ActiveColor);
            } else {
                setTitleScale(activeTitleScale);
                setTopPadding(sixDps);
                setColors(ActiveColor);
                setAlphas(ActiveAlpha);
            }

            Badge?.Hide();
        }

        internal void Deselect(bool animate) {
            IsActive = false;

            bool isShifting = Type == BottomBarTabType.Shifting;

            float scale = isShifting ? 0 : inactiveFixedTitleScale;
            int iconPaddingTop = isShifting ? sixteenDps : eightDps;

            if (animate) {
                setTopPaddingAnimated(IconView.PaddingTop, iconPaddingTop);
                animateTitle(scale, InActiveAlpha);
                animateIcon(InActiveAlpha);
                animateColors(ActiveColor, InActiveColor);
            } else {
                setTitleScale(scale);
                setTopPadding(iconPaddingTop);
                setColors(InActiveColor);
                setAlphas(InActiveAlpha);
            }

            if (!isShifting && Badge != null) {
                Badge.Show();
            }
        }

        private void animateColors(int previousColor, int color) {
            var anim = new ValueAnimator();
            anim.SetIntValues(previousColor, color);
            anim.SetEvaluator(new ArgbEvaluator());
            anim.Update += (s, e) => {
                setColors((e.Animation.AnimatedValue as Java.Lang.Integer).IntValue());
            };
            anim.SetDuration(150);
            anim.Start();
        }

        private void setColors(int color) {
            if (IconView != null) {
                IconView.SetColorFilter(new Color(color));
                IconView.Tag = new Java.Lang.Integer(color);
            }

            TitleView?.SetTextColor(new Color(color));
        }

        private void setAlphas(float alpha) {
            if (IconView != null) {
                ViewCompat.SetAlpha(IconView, alpha);
            }

            if (TitleView != null) {
                ViewCompat.SetAlpha(TitleView, alpha);
            }
        }

        internal void UpdateWidth(float endWidth, bool animated) {
            if (!animated) {
                LayoutParameters.Width = (int)endWidth;

                if (!IsActive && Badge != null) {
                    Badge.AdjustPositionAndSize(this);
                    Badge.Show();
                }
                return;
            }

            float start = Width;

            var animator = ValueAnimator.OfFloat(start, endWidth);
            animator.SetDuration(150);
            animator.Update += (s, e) => {
                var parameters = LayoutParameters;
                if (parameters == null)
                    return;

                parameters.Width = Java.Lang.Math.Round((animator.AnimatedValue as Java.Lang.Float).FloatValue());
                LayoutParameters = parameters;
            };
            animator.AnimationEnd += (s, e) => {
                if (!IsActive && Badge != null) {
                    Badge.AdjustPositionAndSize(this);
                    Badge.Show();
                }
            };
            animator.Start();
        }

        private void updateBadgePosition() => Badge?.AdjustPositionAndSize(this);

        private void setTopPaddingAnimated(int start, int end) {
            if (Type == BottomBarTabType.Tablet) {
                return;
            }

            var paddingAnimator = ValueAnimator.OfInt(start, end);
            paddingAnimator.Update += (s, e) => {
                IconView.SetPadding(
                    IconView.PaddingLeft,
                    (e.Animation.AnimatedValue as Java.Lang.Integer).IntValue(),
                    IconView.PaddingRight,
                    IconView.PaddingBottom
                );
            };
            paddingAnimator.SetDuration(animationDuration);
            paddingAnimator.Start();
        }

        private void animateTitle(float finalScale, float finalAlpha) {
            if (Type == BottomBarTabType.Tablet) {
                return;
            }

            ViewPropertyAnimatorCompat titleAnimator = ViewCompat.Animate(TitleView)
                .SetDuration(animationDuration)
                .ScaleX(finalScale)
                .ScaleY(finalScale);
            titleAnimator.Alpha(finalAlpha);
            titleAnimator.Start();
        }

        private void animateIcon(float finalAlpha) {
            ViewCompat.Animate(IconView)
                    .SetDuration(animationDuration)
                    .Alpha(finalAlpha)
                    .Start();
        }

        private void setTopPadding(int topPadding) {
            if (Type == BottomBarTabType.Tablet) {
                return;
            }

            IconView.SetPadding(
                    IconView.PaddingLeft,
                    topPadding,
                    IconView.PaddingRight,
                    IconView.PaddingBottom
            );
        }

        private void setTitleScale(float scale) {
            if (Type == BottomBarTabType.Tablet) {
                return;
            }

            ViewCompat.SetScaleX(TitleView, scale);
            ViewCompat.SetScaleY(TitleView, scale);
        }

        protected override IParcelable OnSaveInstanceState() {
            if (Badge != null) {
                Bundle bundle = saveState();
                bundle.PutParcelable("superstate", base.OnSaveInstanceState());
                return bundle;
            }

            return base.OnSaveInstanceState();
        }

        private Bundle saveState() {
            Bundle outState = new Bundle();
            outState.PutInt(stateBadgeCount + IndexInContainer, Badge.Count);

            return outState;
        }

        protected override void OnRestoreInstanceState(IParcelable state) {
            if (state is Bundle) {
                var bundle = state as Bundle;
                restoreState(bundle);

                state = bundle.GetParcelable("superstate") as IParcelable;
            }
            base.OnRestoreInstanceState(state);
        }

        private void restoreState(Bundle savedInstanceState) {
            int previousBadgeCount = savedInstanceState.GetInt(stateBadgeCount + IndexInContainer);
            SetBadgeCount(previousBadgeCount);
        }
    }
}