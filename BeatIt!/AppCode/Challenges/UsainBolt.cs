﻿using System;
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

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge, IUsainBolt 
    {

        public event IUsainBolt.getCurrentState stateChanged;
        public event IUsainBolt.getCurrentSpeed speedChanged;        

        private static int VELOCIDAD_MINIMA = 5;
        private GeoCoordinateWatcher gps;
        private double velocidadActual;
        private double velocidadMaxima;
        private int puntaje = 0;        

        public UsainBolt()
        {
            this.name = "Usain Bolt";
            this.descripcion = "Se deberrá correr una velocidad minima de " + VELOCIDAD_MINIMA + "Km/h";

            gps = new GeoCoordinateWatcher();
            gps.PositionChanged += positionChanged;
            gps.StatusChanged += stateChange;

            this.velocidadActual = 0;
            this.velocidadMaxima = 0;
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
    }
}
