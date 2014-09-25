using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.CustomControls;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.Pages
{
    public partial class Home : PhoneApplicationPage
    {

        IFacadeController ifc;

        public Home()
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


            ifc = FacadeController.getInstance();
            User loggedUser = ifc.getCurrentUser();

            profileNameTxtBlock.Text = loggedUser.FirstName + " " + loggedUser.LastName;
            profileCountryTxtBlock.Text = loggedUser.Country;
            profileEmailTextBlock.Text = loggedUser.Email;
            Uri uri = new Uri(loggedUser.ImageUrl, UriKind.Absolute);
            profileImage.Source = new BitmapImage(uri);

            this.initChallengesListBox();
            this.InitRankingListBox();
        }

        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        private void initChallengesListBox()
        {

            string[] colors = new string[10];
            colors[0] = "#FF008A00";
            colors[1] = "#FF1CA1E3";
            colors[2] = "#FFFA6800";
            colors[3] = "#FFE3C900";
            colors[4] = "#FFAA00FF";
            colors[5] = "#FFE61300";
            colors[6] = "#FFFA6800";
            colors[7] = "#FF008A02";
            colors[8] = "#FF1CA1E3";
            colors[9] = "#FFE3C900";

            string[] images = new string[10];
            images[0] = "/BeatIt!;component/Images/Correr.png";
            images[1] = "/BeatIt!;component/Images/Jugar.png";
            images[2] = "/BeatIt!;component/Images/Musica.png";
            images[3] = "/BeatIt!;component/Images/camara.png";
            images[4] = "/BeatIt!;component/Images/Mapa.png";
            images[5] = "/BeatIt!;component/Images/Gps.png";
            images[6] = "/BeatIt!;component/Images/Musica.png";
            images[7] = "/BeatIt!;component/Images/Correr.png";
            images[8] = "/BeatIt!;component/Images/Jugar.png";
            images[9] = "/BeatIt!;component/Images/camara.png";

            for (int i = 0; i < 10; i++)
            {
                ChallenesListItem listItem = new ChallenesListItem();
                listItem.backgroundRec.Fill = GetColorFromHexa(colors[i]);
                Uri uri = new Uri(images[i], UriKind.Relative);
                listItem.image.Source = new BitmapImage(uri);
                listItem.linkBtn.Click += linkBtn_Click;
                listItem.linkBtn.Tag = i + 1;
                ChallengesListBox.Items.Add(listItem);
            }
        }

        private void linkBtn_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton linkBtn = sender as HyperlinkButton;
            String pagePath = "/BeatIt!;component/AppCode/Pages/Challenge" + linkBtn.Tag + ".xaml";
            NavigationService.Navigate(new Uri(pagePath, UriKind.Relative));
        }

        private void InitRankingListBox()
        {
            List<DTRanking> ranking = ifc.getRanking();

            for (int i = 0; i < ranking.Count; i++)
            {
                DTRanking dtr = (DTRanking)ranking[i];
                RankingListItem listItem = new RankingListItem();
                listItem.selectedRec.Visibility = (ifc.getCurrentUser().UserId == dtr.getUserId()) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                listItem.positionTxtBlock.Text = dtr.getPosition().ToString();
                listItem.scoreTxtBlock.Text = dtr.getScore().ToString();
                listItem.nameTxtBlock.Text = dtr.getName();
                Uri uri = new Uri(dtr.getImageUrl(), UriKind.Absolute);
                listItem.userImage.Source = new BitmapImage(uri);
                RankingListBox.Items.Add(listItem);
            }
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var fb = new FacebookClient();
            var parameters = new Dictionary<string, object>();
            parameters["next"] = "https://www.facebook.com/connect/login_success.html";
            parameters["access_token"] = ifc.getCurrentUser().FbAccessToken;
            var logoutUrl = fb.GetLogoutUrl(parameters);
            var webBrowser = new WebBrowser();
            webBrowser.Navigate(logoutUrl);
            webBrowser.Navigated += (o, args) =>
            {
                if (args.Uri.AbsoluteUri == "https://www.facebook.com/connect/login_success.html") 
                {
                    ifc.logoutUser();
                    NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml", UriKind.Relative));
                }
            };
        }


        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Challenge1.xaml", UriKind.Relative));
        }

        private void hyperlinkButton2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }
    }
}