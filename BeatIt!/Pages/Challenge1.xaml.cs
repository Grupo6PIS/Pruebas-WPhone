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
using BeatIt_.AppCode.Interfaces.Challenges;
using BeatIt_.AppCode.Interfaces.Controllers;
// OJO ES PRUEBA, DESPUES SACAR.
using BeatIt_.AppCode.Controllers;


namespace BeatIt_.Pages
{
    public partial class Challenge1 : PhoneApplicationPage, BeatIt_.AppCode.Interfaces.IObserver
    {
        public Challenge1()
        {
            InitializeComponent();

            IChallengeController ICC = ChallengeController.getInstance();

            IUsainBolt IUB = (IUsainBolt)ICC.getIChallenge(AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);

            IUB.startGPS();

            IUB.registObserver(this);

        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        public void actualizar(double velocidadActual)
        {
            this.ShowSpeed.Text = velocidadActual.ToString();
        }
    }
}