
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BottomBarSharp;

namespace BottomBarSharpApp {

    [Activity(Label = "FiveColorChangingTabsActivity",Theme = "@style/AppTheme.TransNav")]
    public class FiveColorChangingTabsActivity : AppCompatActivity {

        private TextView messageView;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_color_changing_tabs);

            messageView = FindViewById<TextView>(Resource.Id.messageView);

            var bottomBar = FindViewById<BottomBar>(Resource.Id.bottomBar);
            bottomBar.TabSelect += (s,e) => {
                messageView.Text = TabMessage.Get(e.TabId,false);
            };

            bottomBar.TabReSelect += (s,e) => {
                Toast.MakeText(ApplicationContext,TabMessage.Get(e.TabId,true),ToastLength.Long).Show();
            };
        }
    }
}