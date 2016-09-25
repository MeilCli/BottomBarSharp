using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace BottomBarSharp {
    sealed class NavbarUtils {

        internal static int GetNavbarHeight(Context context) {
            var res = context.Resources;

            int navBarIdentifier = res.GetIdentifier("navigation_bar_height","dimen","android");

            int navBarHeight = 0;

            if(navBarIdentifier > 0) {
                navBarHeight = res.GetDimensionPixelSize(navBarIdentifier);
            }

            return navBarHeight;
        }

        internal static bool ShouldDrawBehindNavbar(Context context) => isPortrait(context) && hasSoftKeys(context);

        private static bool isPortrait(Context context) {
            var res = context.Resources;

            return res.GetBoolean(Resource.Boolean.bb_bottom_bar_is_portrait_mode);
        }

        // http://stackoverflow.com/a/14871974
        private static bool hasSoftKeys(Context context) {
            bool hasSoftwareKeys = true;

            if(Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1) {
                Display d = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay;

                var realDisplayMetrics = new DisplayMetrics();
                d.GetRealMetrics(realDisplayMetrics);

                int realHeight = realDisplayMetrics.HeightPixels;
                int realWidth = realDisplayMetrics.WidthPixels;

                var displayMetrics = new DisplayMetrics();
                d.GetMetrics(displayMetrics);

                int displayHeight = displayMetrics.HeightPixels;
                int displayWidth = displayMetrics.WidthPixels;

                hasSoftwareKeys = (realWidth - displayWidth) > 0 || (realHeight - displayHeight) > 0;
            } else if(Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich) {
                bool hasMenuKey = ViewConfiguration.Get(context).HasPermanentMenuKey;
                bool hasBackKey = KeyCharacterMap.DeviceHasKey(Keycode.Back);
                hasSoftwareKeys = !hasMenuKey && !hasBackKey;
            }

            return hasSoftwareKeys;
        }

    }
}