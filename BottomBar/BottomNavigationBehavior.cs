using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Views.Animations;
using Java.Lang;

namespace BottomBarSharp {


    class BottomNavigationBehavior<V> : VerticalScrollingBehavior<V> where V : View {
        private static readonly IInterpolator Interpplator = new LinearOutSlowInInterpolator();
        private readonly int bottomNavHeight;
        private readonly int defaultOffset;
        private bool isTablet = false;

        private ViewPropertyAnimatorCompat mTranslationAnimator;
        private bool hidden = false;
        private int mSnackbarHeight = -1;
        private readonly IBottomNavigationWithSnackbar mWithSnackBarImpl;
        private bool mScrollingEnabled = true;

        public BottomNavigationBehavior(int bottomNavHeight,int defaultOffset,bool tablet) {
            this.bottomNavHeight = bottomNavHeight;
            this.defaultOffset = defaultOffset;
            isTablet = tablet;
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                mWithSnackBarImpl = new LollipopBottomNavWithSnackBarImpl(this);
            } else {
                mWithSnackBarImpl = new PreLollipopBottomNavWithSnackBarImpl(this);
            }
        }

        public override bool LayoutDependsOn(CoordinatorLayout parent,Java.Lang.Object child,View dependency) {
            mWithSnackBarImpl.UpdateSnackbar(parent,dependency,child as View);
            return dependency is Snackbar.SnackbarLayout;
        }

        protected override void OnNestedVerticalOverScroll(CoordinatorLayout coordinatorLayout,V child,[ScrollDirections] int direction,int currentOverScroll,int totalOverScroll) {
        }

        public override void OnDependentViewRemoved(CoordinatorLayout parent,Java.Lang.Object child,View dependency) {
            updateScrollingForSnackbar(dependency,true);
            base.OnDependentViewRemoved(parent,child,dependency);
        }

        private void updateScrollingForSnackbar(View dependency,bool enabled) {
            if(!isTablet && dependency is Snackbar.SnackbarLayout) {
                mScrollingEnabled = enabled;
            }
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent,Java.Lang.Object child,View dependency) {
            updateScrollingForSnackbar(dependency,false);
            return base.OnDependentViewChanged(parent,child,dependency);
        }

        protected override void OnDirectionNestedPreScroll(CoordinatorLayout coordinatorLayout,V child,View target,int dx,int dy,int[] consumed,[ScrollDirections] int scrollDirection) {
            handleDirection(child,scrollDirection);
        }

        private void handleDirection(V child,int scrollDirection) {
            if(!mScrollingEnabled)
                return;
            if(scrollDirection == ScrollDirections.ScrollDirectionDown && hidden) {
                hidden = false;
                animateOffset(child,defaultOffset);
            } else if(scrollDirection == ScrollDirections.ScrollDirectionUp && !hidden) {
                hidden = true;
                animateOffset(child,bottomNavHeight + defaultOffset);
            }
        }

        protected override bool OnNestedDirectionFling(CoordinatorLayout coordinatorLayout,V child,View target,float velocityX,float velocityY,[ScrollDirections] int scrollDirection) {
            handleDirection(child,scrollDirection);
            return true;
        }

        private void animateOffset(V child,int offset) {
            ensureOrCancelAnimator(child);
            mTranslationAnimator.TranslationY(offset).Start();
        }

        private void ensureOrCancelAnimator(V child) {
            if(mTranslationAnimator == null) {
                mTranslationAnimator = ViewCompat.Animate(child);
                mTranslationAnimator.SetDuration(300);
                mTranslationAnimator.SetInterpolator(Interpplator);
            } else {
                mTranslationAnimator.Cancel();
            }
        }

        internal void setHidden(V view,bool bottomLayoutHidden) {
            if(!bottomLayoutHidden && hidden) {
                animateOffset(view,defaultOffset);
            } else if(bottomLayoutHidden && !hidden) {
                animateOffset(view,bottomNavHeight + defaultOffset);
            }
            hidden = bottomLayoutHidden;
        }

        internal static BottomNavigationBehavior<V> From(V view) {
            ViewGroup.LayoutParams parameters = view.LayoutParameters;
            if(!(parameters is CoordinatorLayout.LayoutParams)) {
                throw new IllegalArgumentException("The view is not a child of CoordinatorLayout");
            }
            CoordinatorLayout.Behavior behavior = ((CoordinatorLayout.LayoutParams)parameters).Behavior;
            if(!(behavior is BottomNavigationBehavior<V>)) {
                throw new IllegalArgumentException("The view is not associated with BottomNavigationBehavior");
            }
            return behavior as BottomNavigationBehavior<V>;
        }

        interface IBottomNavigationWithSnackbar {
            void UpdateSnackbar(CoordinatorLayout parent,View dependency,View child);
        }

        private class PreLollipopBottomNavWithSnackBarImpl : IBottomNavigationWithSnackbar {

            private BottomNavigationBehavior<V> behavior;

            public PreLollipopBottomNavWithSnackBarImpl(BottomNavigationBehavior<V> behavior) {
                this.behavior = behavior;
            }

            public void UpdateSnackbar(CoordinatorLayout parent,View dependency,View child) {
                if(!behavior.isTablet && dependency is Snackbar.SnackbarLayout) {
                    if(behavior.mSnackbarHeight == -1) {
                        behavior.mSnackbarHeight = dependency.Height;
                    }
                    if(ViewCompat.GetTranslationY(child) != 0)
                        return;

                    int targetPadding = behavior.bottomNavHeight + behavior.mSnackbarHeight - behavior.defaultOffset;
                    var layoutParams = dependency.LayoutParameters as ViewGroup.MarginLayoutParams;
                    layoutParams.BottomMargin = targetPadding;
                    child.BringToFront();
                    child.Parent.RequestLayout();
                    if(Build.VERSION.SdkInt < BuildVersionCodes.Kitkat) {
                        (child.Parent as View).Invalidate();
                    }
                }
            }
        }

        private class LollipopBottomNavWithSnackBarImpl : IBottomNavigationWithSnackbar {

            private BottomNavigationBehavior<V> behavior;

            public LollipopBottomNavWithSnackBarImpl(BottomNavigationBehavior<V> behavior) {
                this.behavior = behavior;
            }

            public void UpdateSnackbar(CoordinatorLayout parent,View dependency,View child) {
                if(!behavior.isTablet && dependency is Snackbar.SnackbarLayout) {
                    if(behavior.mSnackbarHeight == -1) {
                        behavior.mSnackbarHeight = dependency.Height;
                    }
                    if(ViewCompat.GetTranslationY(child) != 0)
                        return;
                    int targetPadding = behavior.bottomNavHeight + behavior.mSnackbarHeight - behavior.defaultOffset;
                    dependency.SetPadding(dependency.PaddingLeft,dependency.PaddingTop,dependency.PaddingRight,targetPadding);
                }
            }
        }
    }
}