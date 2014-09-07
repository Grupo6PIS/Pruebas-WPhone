using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BeatIt_.AppCode.Classes;
using System.Device.Location;
using BeatIt_.AppCode.Interfaces.Challenges;
using System.Collections.Generic;
using BeatIt_.AppCode.Interfaces;

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge, IUsainBolt 
    {

        public delegate void getCurrentSpeed(double speed);
        public delegate void getCurrentState(GeoPositionStatus gs);
        // Disabled, Initializing, NoData, NoData estados posibles de GeoPositionStatus

        public event getCurrentState stateChanged;
        public event getCurrentSpeed speedChanged;        

        private static int VELOCIDAD_MINIMA = 5;
        private GeoCoordinateWatcher gps;
        private double velocidadActual;
        private double velocidadMaxima;
        private int puntaje = 0;

        private List<IObserver> observadores;

        public UsainBolt()
        {
            this.name = "Usain Bolt";
            this.descripcion = "Se deberrá correr una velocidad minima de " + VELOCIDAD_MINIMA + "Km/h";

            gps = new GeoCoordinateWatcher();
            gps.PositionChanged += positionChanged;
            gps.StatusChanged += stateChange;

            this.velocidadActual = 0;
            this.velocidadMaxima = 0;

            this.observadores = new List<IObserver>();
        }
        
        
        public bool startGPS()
        {
            try
            {
                gps.Start();
            }
            catch(Exception e)
            {
                return false;
            }
            return true;            
        }

        public double geMaxSpeed()
        {
            return this.velocidadMaxima;
        }

        public int getPuntaje()
        {
            return Convert.ToInt32(this.velocidadMaxima) * 2;
        }

        
        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            if (speedChanged != null)
            {
                speedChanged(gps.Position.Location.Speed);

                foreach (IObserver io in this.observadores)
                    io.actualizar(gps.Position.Location.Speed);
            }

            if (this.velocidadActual > this.velocidadMaxima)
            {
                this.velocidadMaxima = this.velocidadActual;                
            }
        }

        private void stateChange(object obj, GeoPositionStatusChangedEventArgs e)
        {
            if (stateChanged != null)
            {
                stateChanged(e.Status); 
            }
        }


        public void registObserver(IObserver observer)
        {
            this.observadores.Add(observer);
        }

        // Faltaria desregistrar
    }
}
