
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using BottomBarSharp;

namespace BottomBarSharpApp {
    [Activity(Label = "ThreeTabsQRActivity",Theme = "@style/AppTheme.TransNav")]
    public class ThreeTabsQRActivity : AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_three_tabs_quick_return);

            var bottomBar = FindViewById<BottomBar>(Resource.Id.bottomBar);

            // We're doing nothing with this listener here this time. Check example usage
            // from ThreeTabsActivity on how to use it.
            bottomBar.TabSelect += (s,e) => {

            };
        }
    }
}