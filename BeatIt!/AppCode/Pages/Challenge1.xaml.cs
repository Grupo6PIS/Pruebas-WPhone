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


namespace BeatIt_.Pages
{
    public partial class Challenge1 : PhoneApplicationPage
    {
        /******************************************************************************************************************/
        private GeoCoordinateWatcher gps;              // Instancia del GPS que se utilizara para el calculo de la velocidad.

        private bool useEmulation = true;              // Indica si estamos corriendo la aplicacion en el emulador o en el dispositivo.

        private TimeSpan timeInMinSpeed;               // Tiempo transcurrido a la velocidad minima requerida para cumplir el desafio.
        private DispatcherTimer timer;                 // Timer que se ejecuta al alcanzar la velocidad minima requerida para contabilizar el tiempo transcurido en la misma.

        private TimeSpan transcurredTime;              // Tiempo transcurrido desde que se comenzo e ejecutar el desafio, utilizado para calcular la velocidad actual en el caso del emulador.
        private DispatcherTimer timerToCalculateSpeed; // Timer que se ejecuta al comenzar el desafio para registrar el tiempo transcurrido.

        private double lastLongitude;                  // Ultima longitud registrada, utilizada para calcular la velocidad en caso de ejecutar la app sobre el emulador.
        private double lastLatitude;                   // Ultima latitud registrada, utilizada para calcular la velocidad en caso de ejecutar la app sobre el emulador.
        private TimeSpan lastTime;                     // Ultimo momento en el que se registro un cambio de posición.

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

            this.inicializar();
        }


        private void inicializar()
        {
            /***************************************************************************************************************/
            // INICIALIZAMOS LAS VARIABLES Y CARACTERISTICAS DEL DISPOSITIVO UTILIZADAS.
            /***************************************************************************************************************/

            // TIMER
            this.timeInMinSpeed = new TimeSpan(0, 0, 0, 0, 0);
            // creating timer instance
            this.timer = new DispatcherTimer();
            // timer interval specified as 10 milliseconds
            this.timer.Interval = TimeSpan.FromMilliseconds(1);
            // Sub-routine OnTimerTick will be called at every 1 second
            this.timer.Tick += OnTimerTick;

            
            // TIMER TO CALCULATE SPEED
            if (this.useEmulation)
            {
                this.transcurredTime = new TimeSpan(0, 0, 0, 0, 0);
                // creating timer instance
                this.timerToCalculateSpeed = new DispatcherTimer();
                // timer interval specified as 10 milliseconds
                this.timerToCalculateSpeed.Interval = TimeSpan.FromMilliseconds(1);
                // Sub-routine OnTimerTick will be called at every 1 second
                this.timerToCalculateSpeed.Tick += OnTimerToSpeedTick;
                // Lo iniciamos ya que se inicia cuando inicia el desafio.
                this.timerToCalculateSpeed.Start();
            }


            // INICIALIZAMOS EL GPS.
            this.gps = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            this.gps.MovementThreshold = 10f;
            this.gps.PositionChanged += positionChanged;
            this.gps.StatusChanged += stateChange;
            this.gps.Start();


            // INICIALIZAMOS LAS ETIQUETAS.
            this.ShowTime.Text = (new TimeSpan(0, 0, 0, 30, 0)).ToString("ss") + " s";
            this.ShowSpeed.Text = "0.00 km/h";


            // INICIALIZAMOS LOS VALORES INICIALES DE LONGITUD Y LATITUD.
            this.lastLatitude = 0;
            this.lastLongitude = 0;
            this.lastTime = new TimeSpan(0, 0, 0, 0, 0);

            
            // OBTENEMOS LA INSTANCIA DEL DESAFIO.
            /* hay que prolijear esto con una factory */
            IChallengeController ich = ChallengeController.getInstance();
            this.desafio = (UsainBolt)ich.getChallenge(AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);
        }


        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        private void OnTimerTick(object sender, EventArgs e)
        {
            // Actualizo la variable que mantiene el tiempo transcurrido a la velocidad mínima requerida.
            this.timeInMinSpeed = this.timeInMinSpeed.Add(TimeSpan.FromMilliseconds(1));
            this.ShowTime.Text = Math.Round(this.timeInMinSpeed.TotalSeconds, 2).ToString() + " s";
        }


        private void OnTimerToSpeedTick(object sender, EventArgs e)
        {
            // Actualizo la variable que mantiene el tiempo global, que se utiliza para el calculo de la velocidad.
            this.transcurredTime = this.transcurredTime.Add(TimeSpan.FromMilliseconds(1));            
        }


        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Vemos si ya habia comenzado a correr.
            if (this.lastLatitude == 0 && lastLongitude == 0) // Si es la primer medicion del gps solo asignamos los valores.
            {
                this.lastLongitude = e.Position.Location.Longitude;
                this.lastLatitude = e.Position.Location.Latitude;
            }
            else // Ya habia alguna medicion del gps.
            {
                // Lo primero que debemos hacer es averiguar la velocidad actual, lo cual depende de si estamos en el emulador o en el dispositivo.
                double velocidad; // Km/h.

                if (this.useEmulation)
                {
                    double metrosRecorridos = this.distance_on_geoid(this.lastLatitude, this.lastLongitude, e.Position.Location.Latitude, e.Position.Location.Longitude);
                    velocidad = (this.distance_on_geoid(this.lastLatitude, this.lastLongitude, e.Position.Location.Latitude, e.Position.Location.Longitude) / 1000) /
                                ((this.transcurredTime - this.lastTime).TotalMilliseconds / (1000 * 60 * 60));

                    // Luego de calculada la velocidad, actualizamos los valores de la ultima posicion y tiempo.
                    this.lastTime = this.transcurredTime;
                    this.lastLatitude = e.Position.Location.Latitude;
                    this.lastLongitude = e.Position.Location.Longitude;
                }
                else
                    velocidad = (e.Position.Location.Speed * 60 * 60) / 1000;
                
                // Mostramos la velocidad en pantalla:
                this.ShowSpeed.Text = Math.Round(velocidad, 2).ToString("#.00");

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
                        }
                        else // 3)
                        {
                            // EL desafio se termina conexito!!!!!!!
                            // VER QUE SE HACE ACA..... LANZAE MENSAJE, INICIALIZAR LOS COMPONENTES DE NUEVO PARA QUE PUEDA VOLVER A INTENTARLO....?
                        }
                    }
                }
                else
                {
                    if (this.timeInMinSpeed.TotalMilliseconds == 0) // 4)
                    {
                        this.timer.Start();
                    }
                    else // 5)
                    {
                    }
                }
            }
        }


        private void stateChange(object obj, GeoPositionStatusChangedEventArgs e)
        {
            //if(e.Status == GeoPositionStatus.Disabled)
            //    VER QUE SE HACE SI NO ESTA DISPONIBLE EL GPS.
        }

        /// <summary>
        /// Devulelve la distancia en metros entre dos puntos referenciados por latitud y longitud global.
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        private double distance_on_geoid(double lat1, double lon1, double lat2, double lon2) {
 
            // Convert degrees to radians
            //lat1 = lat1 * Math.PI / 180.0;
            //lon1 = lon1 * Math.PI / 180.0;
 
            //lat2 = lat2 * Math.PI / 180.0;
            //lon2 = lon2 * Math.PI / 180.0;
 
            // radius of earth in metres
            double r = 6378100;
 
            // P
            //double rho1 = r * Math.Cos(lat1);
            //double z1 = r * Math.Sin(lat1);
            //double x1 = rho1 * Math.Cos(lon1);
            //double y1 = rho1 * Math.Sin(lon1);
 
            // Q
            //double rho2 = r * Math.Cos(lat2);
            //double z2 = r * Math.Sin(lat2);
            //double x2 = rho2 * Math.Cos(lon2);
            //double y2 = rho2 * Math.Sin(lon2);
 
            // Dot product
            //double dot = (x1 * x2 + y1 * y2 + z1 * z2);
            //double cos_theta = dot / (r * r);
 
            //double theta = Math.Acos(cos_theta);
 
            // Distance in Metres
            //double distance = r * theta;

            double circunferencia = Math.PI * r * 2;
            double unGradoEquivaleMetros = circunferencia / 360;

            double x = (lat1 - lat2) * unGradoEquivaleMetros;
            double y = (lon1 - lon2) * unGradoEquivaleMetros;

            double distancia = Math.Pow((x * x) + (y * y), 0.5);
            
            return distancia;
        }
    }
}