using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Facebook;
using Microsoft.Phone.Controls;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Controllers;


namespace BeatIt_.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            navigateInTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            navigateOutTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }

        private readonly FacebookClient _fb = new FacebookClient();

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            IFacadeController ifc = FacadeController.getInstance();

            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = "829846350380023";
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "page";
            parameters["scope"] = "email, user_friends, user_hometown";

            var urlBuilder = new StringBuilder();
            foreach (var current in parameters)
            {
                if (urlBuilder.Length > 0)
                {
                    urlBuilder.Append("&");
                }
                var encoded = HttpUtility.UrlEncode((string)current.Value);
                urlBuilder.AppendFormat("{0}={1}", current.Key, encoded);
            }
            var loginUrl = "http://www.facebook.com/dialog/oauth?" + urlBuilder.ToString();

            AuthenticationBrowser.Navigate(new Uri(loginUrl));
            AuthenticationBrowser.Visibility = Visibility.Visible;

        }

        public string AccessToken { get; set; }

        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {

            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                MessageBox.Show(oauthResult.ErrorDescription);
            }

        }


        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var id = (string)result["id"];

                var url = string.Format("/BeatIt!;component/AppCode/Pages/Home.xaml?access_token={0}&id={1}", accessToken, id);

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri(url, UriKind.Relative)));
            };

            fb.GetAsync("me?fields=id");
        }



    }
}