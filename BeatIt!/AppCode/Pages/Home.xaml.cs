using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.CustomControls;
using BeatIt_.AppCode.Classes;

namespace BeatIt_.Pages
{
    public partial class Home : PhoneApplicationPage
    {
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

            this.initRankingListBox();
        }

        private void initRankingListBox()
        {
            for (int i = 1; i <= 20; i++) 
            {
                RankingListItem listItem = new RankingListItem();
                listItem.selectedRec.Visibility = (i == 2) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                listItem.positionTxtBlock.Text = i.ToString();
                listItem.scoreTxtBlock.Text = "128";
                listItem.nameTxtBlock.Text = "Martín Alayón";
                Uri uri = new Uri("http://graph.facebook.com/tincho.alayon/picture?type=square", UriKind.Absolute);
                listItem.userImage.Source = new BitmapImage(uri);
                RankingListBox.Items.Add(listItem);
            }

            //   //FOTOS
            //   Uri uri1 = new Uri("http://graph.facebook.com/tincho.alayon/picture?type=square", UriKind.Absolute);
            //   ph1.Source = new BitmapImage(uri1);
            ////   Uri uri2 = new Uri("http://graph.facebook.com/100002316914037/picture?type=square", UriKind.Absolute);
            ////   ph2.Source = new BitmapImage(uri2);
            //   Uri uri3 = new Uri("http://graph.facebook.com/100002316914037/picture?type=square", UriKind.Absolute);
            //   ph3.Source = new BitmapImage(uri3);
            //   Uri uri4 = new Uri("http://graph.facebook.com/cristian.bauza/picture", UriKind.Absolute);
            //   ph4.Source = new BitmapImage(uri4);
            //   Uri uri5 = new Uri("http://graph.facebook.com/pablo.olivera/picture", UriKind.Absolute);
            //   ph5.Source = new BitmapImage(uri5);
            //   Uri uri6 = new Uri("http://graph.facebook.com/alejandro.brusco/picture?type=square", UriKind.Absolute);
            //   ph6.Source = new BitmapImage(uri6);
            //   Uri uri7 = new Uri("http://graph.facebook.com/felipe92/picture?type=square", UriKind.Absolute);
            //   ph7.Source = new BitmapImage(uri7);
            //   Uri uri8 = new Uri("http://graph.facebook.com/tinchoste/picture?type=square", UriKind.Absolute);
            //   ph8.Source = new BitmapImage(uri8);
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml", UriKind.Relative));
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