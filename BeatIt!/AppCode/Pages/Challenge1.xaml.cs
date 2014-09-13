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

        private TimeSpan timeInMinSpeed;               // Tiempo transcurrido a la velocidad minima requerida para cumplir el desafio.
        private DispatcherTimer timer;                 // Timer que se ejecuta al alcanzar la velocidad minima requerida para contabilizar el tiempo transcurido en la misma.
        private DateTime startedTime;                  // Se utiliza para marcar el tiempo exacto en el cual comenzo a correr a la velocidad minima requerida.
        private bool isStartedTimer;                   // Se utiliza como marca de que el timer esta iniciado.


        private TimeSpan transcurredTime;              // Tiempo transcurrido desde que se comenzo e ejecutar el desafio, utilizado para calcular la velocidad actual en el caso del emulador.
        private DispatcherTimer timerToCalculateSpeed; // Timer que se ejecuta al comenzar el desafio para registrar el tiempo transcurrido.
        private DateTime startedTimeAux;               // Se utiliza para marcar el tiempo exacto en que comenzo a jugarse el desafio.

        private UsainBolt desafio;                     // Instancia del desafio que se esta corriendo.
        /******************************************************************************************************************/




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


            // OBTENEMOS LA INSTANCIA DEL DESAFIO.
            /* hay que prolijear esto con una factory */
            IChallengeController ich = ChallengeController.getInstance();
            this.desafio = (UsainBolt)ich.getChallenge(AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);


            this.inicializar();
        }


        private void inicializar()
        {
            /***************************************************************************************************************/
            // INICIALIZAMOS LAS VARIABLES Y CARACTERISTICAS DEL DISPOSITIVO UTILIZADAS.
            /***************************************************************************************************************/

            // TIMER
            this.timeInMinSpeed = new TimeSpan(0, 0, 0, 0, 0);
            this.isStartedTimer = false;
            // creating timer instance
            this.timer = new DispatcherTimer();
            // timer interval specified as 100 milisegundos
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            // Sub-routine OnTimerTick will be called at every 1 second
            this.timer.Tick += OnTimerTick;


            // TIMER TO CALCULATE SPEED
            if (this.useEmulation)
            {
                this.transcurredTime = new TimeSpan(0, 0, 0, 0, 0);
                this.startedTimeAux = System.DateTime.Now;
                // creating timer instance
                this.timerToCalculateSpeed = new DispatcherTimer();
                // timer interval specified as 100 milliseconds
                this.timerToCalculateSpeed.Interval = new TimeSpan(0, 0, 0, 0, 100);
                // Sub-routine OnTimerTick will be called at every 1 second
                this.timerToCalculateSpeed.Tick += OnTimerToSpeedTick;
                // Lo iniciamos ya que se inicia cuando inicia el desafio.
                this.timerToCalculateSpeed.Start();
            }


            // INICIALIZAMOS EL GPS.
            this.gps = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            //this.gps.MovementThreshold = 1f;
            this.gps.PositionChanged += positionChanged;
            this.gps.StatusChanged += stateChange;
            this.gps.Start();


            // INICIALIZAMOS LAS ETIQUETAS.
            this.ShowTime.Text = (new TimeSpan(0, 0, 0, Convert.ToInt32(UsainBolt.TIEMPO_MINIMO_SEG), 0)).ToString("ss") + " s";
            this.ShowSpeed.Text = "0.00 km/h";
            this.ShowST.Text = this.desafio.getDTChallenge(System.DateTime.Now).getStartTime().ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            this.ShowToBeat.Text = this.desafio.getPuntajeObtenido().ToString();
        }

        private void inicializarEtiquetas()
        {
        }


        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        private void OnTimerTick(object sender, EventArgs e)
        {
            if (!this.isStartedTimer)
            {
                this.startedTime = System.DateTime.Now;
                this.isStartedTimer = true;
            }
            else
            {
                // Actualizo la variable que mantiene el tiempo transcurrido a la velocidad mínima requerida.
                this.timeInMinSpeed = System.DateTime.Now - startedTime;
                this.ShowTime.Text = Math.Round(UsainBolt.TIEMPO_MINIMO_SEG - this.timeInMinSpeed.TotalSeconds, 2).ToString("#.00") + " s";
            }
        }


        private void OnTimerToSpeedTick(object sender, EventArgs e)
        {
            // Actualizo la variable que mantiene el tiempo global, que se utiliza para el calculo de la velocidad.
            this.transcurredTime = System.DateTime.Now - this.startedTimeAux;
        }


        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Lo primero que debemos hacer es averiguar la velocidad actual, lo cual depende de si estamos en el emulador o en el dispositivo.
            double velocidad; // Km/h.

            if (this.useEmulation)
                velocidad = this.simulateSpeed(this.transcurredTime);
            else
                velocidad = (e.Position.Location.Speed * 60 * 60) / 1000;

            // Mostramos la velocidad en pantalla:
            this.ShowSpeed.Text = Math.Round(velocidad, 2).ToString("#.00") + " km/h";

            // DIVIDIMOS EN 5 CASOS: 1) (VELOCIDAD ACTUAL ES INFERIOR A LA MINIMA) && 
            //                          (AHUN NO HABIA ALCANZADO LA VELOCIDAD MINIMA) --> EL DESAFIO CONTINUA.

            //                       2) (VELOCIDAD ACTUAL ES INFERIOR A LA MINIMA) && 
            //                          (HABIA ALCANZADO LA VELOCIDAD MINIMA) && 
            //                          (TIEMPO TRANSCURRIDO EN VELOCIDAD MINIMA < TIEMPO_MINIMO_SEG s) --> DESAFIO TERMINA SIN COMPLETAR.

            //                       3) (VELOCIDAD ACTUAL ES INFERIOR A LA MINIMA) && 
            //                          (HABIA ALCANZADO LA VELOCIDAD MINIMA) && 
            //                          (TIEMPO TRANSCURRIDO EN VELOCIDAD MINIMA > TIEMPO_MINIMO_SEG s) --> DESAFIO TERMINA CON EXITO!!!!!                                

            //                       4) (VELOCIDAD ACTUAL ES SUPERIOR A LA MINIMA) && 
            //                          (AHUN NO HABIA ALCANZADO LA VELOCIDAD MINIMA) --> COMIENZA A REGISTRARSE EL TIEMPO QUE MANTIENE LA VELOCIDAD MINIMA.

            //                       5) (VELOCIDAD ACTUAL ES SUPERIOR A LA MINIMA) && 
            //                          (HABIA ALCANZADO LA VELOCIDAD MINIMA) --> SE SIGUE REGISTRANDO EL TIEMPO (ESTA CORRIENDO A MAS DE LA VELOCIDAD MINIMA).

            if (velocidad < UsainBolt.VELOCIDAD_MINIMA)
            {
                if (this.timeInMinSpeed.TotalMilliseconds == 0) // 1)
                {
                    // No hacer nada.
                }
                else
                {
                    if (this.timeInMinSpeed.TotalSeconds < UsainBolt.TIEMPO_MINIMO_SEG) // 2)
                    {
                        // El desafio termina sin completar .............
                        // VER QUE SE HACE ACA..... LANZAR MENSAJE, INICIALIZAR LOS COMPONENTES DE NUEVO PARA QUE PUEDA VOLVER A INTENTERLO....?
                        this.timer.Stop();
                        this.timerToCalculateSpeed.Stop();
                        this.gps.Stop();

                        // AUMENTEAR LOS INENTOS EN 1.

                        // MOSTRAR MENSAJE DE DESAFIO SIN COMPLETAR!
                        Thread.Sleep(5000); // Solo para mostrar los daros.

                        // Volvemos a iniciar...
                        this.inicializar();
                    }
                    else // 3)
                    {
                        // EL desafio se termina conexito!!!!!!!
                        // VER QUE SE HACE ACA..... LANZAE MENSAJE, INICIALIZAR LOS COMPONENTES DE NUEVO PARA QUE PUEDA VOLVER A INTENTARLO....?
                        this.timer.Stop();
                        this.timerToCalculateSpeed.Stop();
                        this.gps.Stop();

                        this.desafio.finish(this.timeInMinSpeed);

                        // MOSTRAR MENSAJE DE DESAFIO COMPLETADO CON EXITO!!!!!!
                        Thread.Sleep(5000); // Solo para mostrar los daros.

                        this.inicializar();
                    }
                }
            }
            else
            {
                if (!this.isStartedTimer) // 4)
                {
                    this.timer.Start();
                }
                else // 5)
                {
                }
            }
        }


        private void stateChange(object obj, GeoPositionStatusChangedEventArgs e)
        {
            //if(e.Status == GeoPositionStatus.Disabled)
            //    VER QUE SE HACE SI NO ESTA DISPONIBLE EL GPS.
        }


        public double simulateSpeed(TimeSpan time)
        {
            if (time.TotalSeconds < 3)
                return 0;
            else if (time.TotalSeconds < 5)
                return 5;
            else if (time.TotalSeconds < 7)
                return 10;
            else if (time.TotalSeconds < 10)
                return 15;
            else if (time.TotalSeconds < 45)
                return 21;
            else if (time.TotalSeconds < 50)
                return 15;
            else if (time.TotalSeconds < 55)
                return 10;
            else if (time.TotalSeconds < 60)
                return 5;
            else
                return 0;
        }
    }
}