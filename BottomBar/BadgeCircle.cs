using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

namespace BottomBarSharp {
    class BadgeCircle {

        /// <summary>
        /// Creates a new circle for the Badge background.
        /// </summary>
        /// <param name="size">the width and height for the circle</param>
        /// <param name="color">the activeIconColor for the circle</param>
        /// <returns>a nice and adorable circle.</returns>
        internal static ShapeDrawable Make(int size,int color) {
            var indicator = new ShapeDrawable(new OvalShape());
            indicator.SetIntrinsicWidth(size);
            indicator.SetIntrinsicHeight(size);
            indicator.Paint.Color = new Android.Graphics.Color(color);
            return indicator;
        }
    }
}