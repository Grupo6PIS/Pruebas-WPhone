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
using BeatIt_.AppCode.Interfaces.Controllers;
using System.Device.Location;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using System.Threading;
using Microsoft.Devices;


namespace BeatIt_.Pages
{
    public partial class Challenge1 : PhoneApplicationPage
    {

        /******************************************************************************************************************/
        private GeoCoordinateWatcher gps;              // Instancia del GPS que se utilizara para el calculo de la velocidad.
        private bool useEmulation = (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator);              // Indica si estamos corriendo la aplicacion en el emulador o en el dispositivo.
        private UsainBolt desafio;                     // Instancia del desafio que se esta corriendo.
        private DateTime startToPlay;                  // Se utiliza para tener referencia temporal de en que ronda se comenzo a jugar el desafio.                  
        /******************************************************************************************************************/

        private GPS_SpeedEmulator speedEmulator;
        private int seconds,
                    minSpeed = 20,
                    minTime = 30;
        private DispatcherTimer timer;
        private Boolean isRunning = false; //SE CAMBIA A TRUE CUANDO LA VELOCIDAD ACTUAL ES MAYOR O IGUAL A LA VELOCIDAD MINIMA DEL DESAFíO

        public Challenge1()
        {
            // INICIALIZAMOS LOS COMPONENTES GRAFICOS.
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            navigateInTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            navigateOutTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            this.inicializar();
        }

        

        private void inicializar()
        {
            // OBTENEMOS LA INSTANCIA DEL DESAFIO.
            /* hay que prolijear esto con una factory */
            IChallengeController ich = ChallengeController.getInstance();
            this.desafio = (UsainBolt)ich.getChallenge(AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);
            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += tickTemp;

            seconds = minTime;
            

            // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
            this.ShowST.Text = this.desafio.getDTChallenge().getStartTime().ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            this.ShowToBeat.Text = this.desafio.getPuntajeObtenido().ToString();
            DateTime roundDate = new DateTime(2014, 9, 21, 22, 0, 0);
            this.ShowDuration.Text = getDurationString(roundDate);

            this.ShowTime.Text =  minTime.ToString();
            this.ShowSpeed.Text = "0.00";

            if (this.useEmulation)
            {
                speedEmulator = new GPS_SpeedEmulator();
                speedEmulator.SpeedChange += speedChanged;
            }
            else
            {
                StartRunningButton.IsEnabled = false;
                startRunningRec.Opacity = 0.5;

                this.gps = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                this.gps.MovementThreshold = 3;
                this.gps.PositionChanged += positionChanged;
                this.gps.StatusChanged += statusChanged;
                this.gps.Start();
                this.ShowToBeat.Text = "MThreshold: " + this.gps.MovementThreshold.ToString() + " m";
            }
        }


        private string getDurationString(DateTime roundDate)
        {
            String result = "";
            DateTime dateToday = DateTime.Now;
            TimeSpan dif = roundDate - dateToday;
            int days = dif.Days;            
            if (days > 0)
            {
                result = days + " dias";
            }
            else
            {
                int hours = dif.Hours % 24;
                if (hours > 0)
                {
                    result = hours + " horas";
                }
                else
                {
                    int minutes = dif.Minutes % 60;
                    if (minutes > 0)
                    {
                        result = minutes + " minutos";
                    }
                    else
                    {
                        result = "Menos de un minuto!!";
                    }
                }
            }
            return result;
        }

        private void tickTemp(object o, EventArgs e)
        {
            if (isRunning)
            {
                seconds--;
                if (seconds == 0)
                {
                    //Desafio completado
                    timer.Stop();
                    seconds = minTime;
                    isRunning = false;

                    ShowTime.Text = "0.00";

                    //CAMBIO DE GRILLA (InProgressGrid ==> StartRunningGrid)
                    StartRunningGrid.Visibility = System.Windows.Visibility.Visible;
                    InProgressGrid.Visibility = System.Windows.Visibility.Collapsed;

                    if (useEmulation)
                    {
                        speedEmulator.Stop();
                    }

                    MessageBox.Show("Desafio completado!");
                }
                else
                {  
                    //Desafio en curso
                    ShowTime.Text = seconds.ToString();
                }
            }
        }

        private void updateLabels(double speed)
        {

            if (speed <= 0 || double.IsNaN(speed))
            {
                ShowSpeed.Text = "0.00";
            }
            else
            {
                ShowSpeed.Text = String.Format("{0:0.##}", speed);
            }
        }

        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            double speed = e.Position.Location.Speed * 3.6;
            this.speedChanged(speed);
        }

        private void speedChanged(double speed)
        {
            updateLabels(speed);

            if (speed >= minSpeed)
            {
                isRunning = true;
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 229, 20, 0);
                SpeedRec.Fill = mySolidColorBrush;     

                if (!timer.IsEnabled) {
                    timer.Start();
                }
            }
            else
            {
                isRunning = false;
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 0, 138, 0);
                SpeedRec.Fill = mySolidColorBrush;
                if (timer.IsEnabled)
                {
                    //Desafío no completado
                    timer.Stop();
                    seconds = minTime;

                    //CAMBIO DE GRILLA (InProgressGrid ==> StartRunningGrid)
                    StartRunningGrid.Visibility = System.Windows.Visibility.Visible;
                    InProgressGrid.Visibility = System.Windows.Visibility.Collapsed;

                    if (useEmulation)
                    {
                        speedEmulator.Stop();
                    }

                    MessageBox.Show("Desafio no completado.");
                }
            }
        }

        private void statusChanged(object obj, GeoPositionStatusChangedEventArgs e)
        {
            String statusType = "";
            if (e.Status == GeoPositionStatus.NoData)
            {
                statusType = "NoData";
            }
            if (e.Status == GeoPositionStatus.Initializing)
            {
                statusType = "Initializing";
            }
            if (e.Status == GeoPositionStatus.Ready)
            {
                statusType = "Ready";
                StartRunningButton.IsEnabled = true;
                startRunningRec.Opacity = 1.0;
            }
            if (e.Status == GeoPositionStatus.Disabled)
            {
                statusType = "Disabled";
            }
            //this.ShowDuration.Text = "Status: " + statusType;
        }

        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                MessageBox.Show("Debes bajar la velocidad para comenzar el desafío");
            }
            else
            {
                if (useEmulation) {
                    speedEmulator.Start();
                }
                this.ShowTime.Text = minTime.ToString();
                this.ShowSpeed.Text = "0.00";

                //CAMBIO DE GRILLA (StartRunningGrid ==> InProgressGrid)
                StartRunningGrid.Visibility = System.Windows.Visibility.Collapsed;
                InProgressGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            if (useEmulation)
            {
                speedEmulator.Stop();
            }
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }

    class GPS_SpeedEmulator
    {
        private enum Estados { LLEGAR_A_20KM, MANTENER_TIEMPO , BAJAR_VELOCIDAD};
        public delegate void EventSpeedChange(double s);

        public event EventSpeedChange SpeedChange;

        private DispatcherTimer timer;
        private Random randomNumber;
        private double speed = 0;
        private Estados state = Estados.LLEGAR_A_20KM;
        private int mantenerVelocidadPor,
                    velocidadMinima = 20;

        public GPS_SpeedEmulator()
        {
            randomNumber = new Random();
            mantenerVelocidadPor = 25 + randomNumber.Next(1,11);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1); 
            timer.Tick += tickTimer;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop(){
            timer.Stop();
        }

        private void tickTimer(object o, EventArgs e)
        {

            if (timer.IsEnabled)
            {
                int sumarRestar = randomNumber.Next(0, 2);
                double dec = (1 + 0.0) / randomNumber.Next(1, 11),
                        delta = randomNumber.Next(1, 4) + (sumarRestar - 1) * dec + sumarRestar * dec;

                if (state == Estados.LLEGAR_A_20KM)
                {
                    speed += randomNumber.Next(1, 4) + (sumarRestar - 1) * dec + sumarRestar * dec;
                    if (speed >= velocidadMinima)
                    {
                        state = Estados.MANTENER_TIEMPO;
                    }
                }
                else if (state == Estados.MANTENER_TIEMPO && mantenerVelocidadPor > 0)
                {
                    mantenerVelocidadPor--;
                    double temp = speed + (sumarRestar - 1) * dec + sumarRestar * dec;
                    speed += temp >= velocidadMinima && temp < 24 ? temp - speed : 0;

                    if (mantenerVelocidadPor == 0)
                    {
                        state = Estados.BAJAR_VELOCIDAD;
                    }
                }
                else if (state == Estados.BAJAR_VELOCIDAD)
                {
                    speed -= randomNumber.Next(1, 4) - dec;
                    if (speed < velocidadMinima)
                    {
                        timer.Stop();
                    }
                }
                SpeedChange(speed);
            }
        }

    }
}