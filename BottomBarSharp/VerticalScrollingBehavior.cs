using System;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace BottomBarSharp {

    class ScrollDirections : Attribute {
        internal const int ScrollDirectionUp = 1;
        internal const int ScrollDirectionDown = -1;
        internal const int ScrollNone = 0;
    }

    abstract class VerticalScrollingBehavior<V> : CoordinatorLayout.Behavior where V : View {

        private int totalDyUnconsumed = 0;
        private int totalDy = 0;

        /// <summary>
        /// Scroll direction: ScrollDirectionUp, ScrollDirectionDown, ScrollNone
        /// </summary>
        [ScrollDirections]
        public int OverScrollDirection { get; private set; } = ScrollDirections.ScrollNone;

        /// <summary>
        /// Scroll direction: ScrollDirectionUp, ScrollDirectionDown, ScrollNone
        /// </summary>
        [ScrollDirections]
        public int ScrollDirection { get; private set; } = ScrollDirections.ScrollNone;

        internal VerticalScrollingBehavior(Context context, IAttributeSet attrs) : base(context, attrs) { }

        internal VerticalScrollingBehavior() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinatorLayout"></param>
        /// <param name="child"></param>
        /// <param name="direction">Direction of the overscroll: ScrollDirectionUp, ScrollDirectionDown</param>
        /// <param name="currentOverScroll">Unconsumed value, negative or positive based on the direction;</param>
        /// <param name="totalOverScroll">Cumulative value for current direction</param>
        protected abstract void OnNestedVerticalOverScroll(CoordinatorLayout coordinatorLayout, V child, [ScrollDirections] int direction, int currentOverScroll, int totalOverScroll);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinatorLayout"></param>
        /// <param name="child"></param>
        /// <param name="target"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="consumed"></param>
        /// <param name="scrollDirection">Direction of the overscroll: ScrollDirectionUp, ScrollDirectionDown</param>
        protected abstract void OnDirectionNestedPreScroll(CoordinatorLayout coordinatorLayout, V child, View target, int dx, int dy, int[] consumed, [ScrollDirections] int scrollDirection);

        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
            => (nestedScrollAxes & (int)ScrollAxis.Vertical) != 0;

        public override void OnNestedScrollAccepted(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
            => base.OnNestedScrollAccepted(coordinatorLayout, child, directTargetChild, target, nestedScrollAxes);

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target)
            => base.OnStopNestedScroll(coordinatorLayout, child, target);

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed) {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
            if (dyUnconsumed > 0 && totalDyUnconsumed < 0) {
                totalDyUnconsumed = 0;
                OverScrollDirection = ScrollDirections.ScrollDirectionUp;
            } else if (dyUnconsumed < 0 && totalDyUnconsumed > 0) {
                totalDyUnconsumed = 0;
                OverScrollDirection = ScrollDirections.ScrollDirectionDown;
            }
            totalDyUnconsumed += dyUnconsumed;

            OnNestedVerticalOverScroll(coordinatorLayout, child as V, OverScrollDirection, dyConsumed, totalDyUnconsumed);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed) {
            base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
            if (dy > 0 && totalDy < 0) {
                totalDy = 0;
                ScrollDirection = ScrollDirections.ScrollDirectionUp;
            } else if (dy < 0 && totalDy > 0) {
                totalDy = 0;
                ScrollDirection = ScrollDirections.ScrollDirectionDown;
            }
            totalDy += dy;
            OnDirectionNestedPreScroll(coordinatorLayout, child as V, target, dx, dy, consumed, ScrollDirection);
        }

        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed) {
            base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed);
            ScrollDirection = velocityY > 0 ? ScrollDirections.ScrollDirectionUp : ScrollDirections.ScrollDirectionDown;
            return OnNestedDirectionFling(coordinatorLayout, child as V, target, velocityX, velocityY, ScrollDirection);
        }

        protected abstract bool OnNestedDirectionFling(CoordinatorLayout coordinatorLayout, V child, View target, float velocityX, float velocityY, [ScrollDirections] int scrollDirection);

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
            => base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY);

        public override WindowInsetsCompat OnApplyWindowInsets(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, WindowInsetsCompat insets)
            => base.OnApplyWindowInsets(coordinatorLayout, child, insets);

        public override IParcelable OnSaveInstanceState(CoordinatorLayout parent, Java.Lang.Object child)
            => base.OnSaveInstanceState(parent, child);

    }
}