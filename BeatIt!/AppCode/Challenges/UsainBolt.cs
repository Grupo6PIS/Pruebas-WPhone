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

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge
    {
        private GeoCoordinateWatcher gps;
        private double velocidadActual;
        private double velocidadMaxima;


        public delegate void getCurrentSpeed(double speed);
        public event getCurrentSpeed speedChange;

        public UsainBolt()
        {
            gps = new GeoCoordinateWatcher();
            gps.PositionChanged += positionChanged;

            this.velocidadActual = 0;
            this.velocidadMaxima = 0;
        }
        
        
        public void startGPS()
        {
            if (gps != null)
            {
                gps.Start();
            }
        }

        public double geMaxSpeed()
        {
            return this.velocidadMaxima;
        }

        
        private void positionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //this.velocidadActual = gps.Position.Location.Speed;

            /*
             * UB.speedChange+= metodoDeMartin,
             * metodoDemartin( double speed){
             * }
             * 
             * */

            if (speedChange != null)
            {
                speedChange(gps.Position.Location.Speed);
            }

            if (this.velocidadActual > this.velocidadMaxima)
            {
                this.velocidadMaxima = this.velocidadActual;                
            }
        }
    }
}
