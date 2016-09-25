using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace BottomBarSharpApp {
    [Activity(Label = "MainActivity",MainLauncher = true,LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {


        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_main);

            FindViewById(Resource.Id.simple_three_tabs).Click += (s,e) => {
                StartActivity(new Android.Content.Intent(this,typeof(ThreeTabsActivity)));
            };
            FindViewById(Resource.Id.five_tabs_changing_colors).Click += (s,e) => {
                StartActivity(new Android.Content.Intent(this,typeof(FiveColorChangingTabsActivity)));
            };
            FindViewById(Resource.Id.three_tabs_quick_return).Click += (s,e) => {
                StartActivity(new Android.Content.Intent(this,typeof(ThreeTabsQRActivity)));
            };
            FindViewById(Resource.Id.five_tabs_custom_colors).Click += (s,e) => {
                StartActivity(new Android.Content.Intent(this,typeof(CustomColorAndFontActivity)));
            };
            FindViewById(Resource.Id.badges).Click += (s,e) => {
                StartActivity(new Android.Content.Intent(this,typeof(BadgeActivity)));
            };
        }

    }
}

