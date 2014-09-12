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
using System.Collections.Generic;
using BeatIt_.AppCode.Interfaces;

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge 
    {
        public const double VELOCIDAD_MINIMA = 20;
        public const double TIEMPO_MINIMO_SEG = 30;

        private int puntajeObtenido;

        public UsainBolt()
        {
            this.name = "Usain Bolt";
            this.descripcion = "Se deberrá correr una velocidad minima de " + VELOCIDAD_MINIMA + "Km/h durante " + TIEMPO_MINIMO_SEG.ToString() + " s.";
        }
    }
}
