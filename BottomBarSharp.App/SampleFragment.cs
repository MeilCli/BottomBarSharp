using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SFragment = Android.Support.V4.App.Fragment;

namespace BottomBarSharpApp {

    public class SampleFragment : SFragment {

        private const string ArgText = "ARG_TEXT";

        public SampleFragment() { }

        public static SampleFragment NewInstance(String text) {
            var args = new Bundle();
            args.PutString(ArgText, text);

            var sampleFragment = new SampleFragment();
            sampleFragment.Arguments = args;

            return sampleFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var textView = new TextView(Activity);
            textView.Text = Arguments.GetString(ArgText);

            return textView;
        }
    }
}