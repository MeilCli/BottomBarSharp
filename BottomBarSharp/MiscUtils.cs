using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Java.Lang;

namespace BottomBarSharp {

    class MiscUtils {

        internal static int GetColor(Context context, int color) {
            var tv = new TypedValue();
            context.Theme.ResolveAttribute(color, tv, true);
            return tv.Data;
        }

        /// <summary>
        /// Converts dps to pixels nicely.
        /// </summary>
        /// <param name="context">the Context for getting the resources</param>
        /// <param name="dp">dimension in dps</param>
        /// <returns>dimension in pixels</returns>
        internal static int DpToPixel(Context context, float dp) {
            var resources = context.Resources;
            var metrics = resources.DisplayMetrics;
            try {
                return (int)(dp * ((int)metrics.DensityDpi / 160f));
            } catch (NoSuchFieldError) {
                return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, metrics);
            }
        }

        /// <summary>
        /// Converts pixels to dps just as well.
        /// </summary>
        /// <param name="context">the Context for getting the resources</param>
        /// <param name="px">dimension in pixels</param>
        /// <returns>dimension in dps</returns>
        internal static int PixelToDp(Context context, int px) {
            DisplayMetrics displayMetrics = context.Resources.DisplayMetrics;
            return Math.Round(px / (displayMetrics.Xdpi / (int)DisplayMetricsDensity.Default));
        }

        /// <summary>
        /// Returns screen width.
        /// </summary>
        /// <param name="context">Context to get resources and device specific display metrics</param>
        /// <returns>screen width</returns>
        internal static int GetScreenWidth(Context context) {
            var displayMetrics = context.Resources.DisplayMetrics;
            return (int)(displayMetrics.WidthPixels / displayMetrics.Density);
        }

        /// <summary>
        /// A convenience method for setting text appearance.
        /// </summary>
        /// <param name="textView">a TextView which textAppearance to modify.</param>
        /// <param name="resId">a style resource for the text appearance.</param>
        [SuppressWarnings]
        internal static void SetTextAppearance(TextView textView, int resId) {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M) {
                textView.SetTextAppearance(resId);
            } else {
#pragma warning disable 0618
                textView.SetTextAppearance(textView.Context, resId);
#pragma warning restore 0618
            }
        }

        /// <summary>
        /// Determine if the current UI Mode is Night Mode.
        /// </summary>
        /// <param name="context">Context to get the configuration.</param>
        /// <returns>true if the night mode is enabled, otherwise false.</returns>
        internal static bool IsNightMode(Context context) {
            var currentNightMode = context.Resources.Configuration.UiMode & Android.Content.Res.UiMode.NightMask;
            return currentNightMode == Android.Content.Res.UiMode.NightYes;
        }
    }
}