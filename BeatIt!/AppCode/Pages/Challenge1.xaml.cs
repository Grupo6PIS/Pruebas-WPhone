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


namespace BeatIt_.Pages
{
    public partial class Challenge1 : PhoneApplicationPage
    {

        /******************************************************************************************************************/
        private GeoCoordinateWatcher gps;              // Instancia del GPS que se utilizara para el calculo de la velocidad.
        private bool useEmulation = true;              // Indica si estamos corriendo la aplicacion en el emulador o en el dispositivo.
        private UsainBolt desafio;                     // Instancia del desafio que se esta corriendo.
        private DateTime startToPlay;                  // Se utiliza para tener referencia temporal de en que ronda se comenzo a jugar el desafio.                  
        /******************************************************************************************************************/

        private GPS_SpeedEmulator speedEmulator;
        private int seconds = 30;
        private DispatcherTimer temporizador;
        private Boolean tempIniciado = false,
                        completoTiempo = false;

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

            temporizador = new DispatcherTimer();
            temporizador.Interval = new TimeSpan(0, 0, 1);
            temporizador.Tick += tickTemp;
            

            // INICIALIZAMOS LAS ETIQUETAS.
            this.ShowTime.Text = UsainBolt.TIEMPO_MINIMO_SEG + " s";
            this.ShowSpeed.Text = "0.00 km/h";
            this.ShowST.Text = this.desafio.getDTChallenge().getStartTime().ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            this.ShowToBeat.Text = this.desafio.getPuntajeObtenido().ToString();
            this.ShowDuration.Text = this.desafio.getDTChallenge().getBestTime() + "s";

            if (this.useEmulation)
            {
                speedEmulator = new GPS_SpeedEmulator();
                speedEmulator.SpeedChange += methodSpeedChange;
                speedEmulator.Start();
            }
            else
            {
                // INICIALIZAMOS EL GPS.
                this.gps = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                //this.gps.MovementThreshold = 1f;
                this.gps.PositionChanged += positionChanged;
                //this.gps.StatusChanged += stateChange;
                this.gps.Start();
            }

           
        }

        private void tickTemp(object o, EventArgs e)
        {
            seconds--;
        }

        private void updateLabels(double speed)
        {
            ShowSpeed.Text = String.Format("{0:0.##}", speed) + "Km/h";
            if (seconds > 0)
            {
                ShowTime.Text = seconds + "s";
            }
            else
            {
                completoTiempo = true;
                ShowTime.Text = "0s +" + (Math.Abs(seconds) + 1);
            }
        }

        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            double speed = e.Position.Location.Speed * 3.6;
            if (speed > 20)
            {
                if (!tempIniciado)
                {
                    seconds = 30;
                    temporizador.Start();
                    tempIniciado = true;
                }
            }
            else if (completoTiempo && speed < 20)
            {
                if (completoTiempo)
                {
                    tempIniciado = false;
                    temporizador.Stop();
                    if (useEmulation)
                    {
                        speedEmulator.Stop();
                    }
                    else
                    {
                        gps.Stop();
                    }
                    int tiempoCorrido = 30 + Math.Abs(seconds);
                    MessageBox.Show("Desafío completado con exito!!! ha corrido " + tiempoCorrido + "segundos!");
                    this.desafio.finish(tiempoCorrido);
                }
            }
            else if (speed < 20)
            {
                temporizador.Stop();
                seconds = 30;
                MessageBox.Show("Desafio no completado. ¿Listo para intentarlo otra vez?");
            }
            ShowSpeed.Text = String.Format("{0:0.##}", speed) + "Km/h";
            this.ShowToBeat.Text = this.desafio.getPuntajeObtenido().ToString();
            
        }

        private void stateChange(object obj, GeoPositionStatusChangedEventArgs e)
        {

        }

        private void methodSpeedChange(double speed)
        {

            updateLabels(speed);

            if (speed > 20)
            {
                if (!tempIniciado)
                {
                    seconds = 30;
                    temporizador.Start();
                    tempIniciado = true;
                }
            }
            else if (completoTiempo && speed < 20)
            {
                if (completoTiempo)
                {
                    tempIniciado = false;
                    temporizador.Stop();
                    if (useEmulation)
                    {
                        speedEmulator.Stop();
                    }
                    else
                    {
                        gps.Stop();
                    }
                    int tiempoCorrido = 30 + Math.Abs(seconds);
                    MessageBox.Show("Desafío completado con exito!!! ha corrido " + tiempoCorrido + " segundos!");
                    this.desafio.finish(tiempoCorrido);
                    this.ShowToBeat.Text = this.desafio.getPuntajeObtenido().ToString();
                    this.ShowDuration.Text = (this.desafio.getDTChallenge().getBestTime() > tiempoCorrido ? this.desafio.getDTChallenge().getBestTime() : tiempoCorrido) + "s";
                }
            }
            else if (tempIniciado && speed < 20)
            {
                temporizador.Stop();
                seconds = 30;
                speedEmulator.Stop();
                MessageBox.Show("Desafio no completado. ¿Listo para intentarlo otra vez?");
            }
            
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

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
        private int mantenerVelocidadPor;

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
            int sumarRestar = randomNumber.Next(0, 2);
            double dec = (1 + 0.0) / randomNumber.Next(1, 11),
                    delta = randomNumber.Next(1, 4) + (sumarRestar - 1) * dec + sumarRestar*dec;

            if (state == Estados.LLEGAR_A_20KM)
            {
                speed += randomNumber.Next(1, 4) + (sumarRestar - 1) * dec + sumarRestar*dec;
                if (speed >= 20)
                {
                    state = Estados.MANTENER_TIEMPO;
                }
            }
            else if (state == Estados.MANTENER_TIEMPO && mantenerVelocidadPor > 0)
            {
                mantenerVelocidadPor--;
                double temp = speed + (sumarRestar - 1) * dec + sumarRestar * dec;
                speed += temp >= 20 && temp < 24 ? temp -speed : 0;

                if (mantenerVelocidadPor == 0)
                {
                    state = Estados.BAJAR_VELOCIDAD;
                }
            }
            else if (state == Estados.BAJAR_VELOCIDAD)
            {
                speed -= randomNumber.Next(1, 4) - dec;
                if (speed < 20)
                {
                    timer.Stop();
                }
            }
            SpeedChange(speed);
            
        }

    }
}