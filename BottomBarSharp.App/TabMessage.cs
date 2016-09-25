namespace BottomBarSharpApp {
    public class TabMessage {

        public static string Get(int menuItemId,bool isReselection) {
            string message = "Content for ";

            switch(menuItemId) {
                case Resource.Id.tab_recents:
                    message += "recents";
                    break;
                case Resource.Id.tab_favorites:
                    message += "favorites";
                    break;
                case Resource.Id.tab_nearby:
                    message += "nearby";
                    break;
                case Resource.Id.tab_friends:
                    message += "friends";
                    break;
                case Resource.Id.tab_food:
                    message += "food";
                    break;
            }

            if(isReselection) {
                message += " WAS RESELECTED! YAY!";
            }

            return message;
        }

    }
}