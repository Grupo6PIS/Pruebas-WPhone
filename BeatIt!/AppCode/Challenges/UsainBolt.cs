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
using System.Collections.Generic;
using BeatIt_.AppCode.Interfaces;

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge
    {
        public const double VELOCIDAD_MINIMA = 20;
        public const double TIEMPO_MINIMO_SEG = 30;

        public UsainBolt()
        {
            this.id = Enums.ChallengeType.ToString(Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);
            this.name = "Usain Bolt";
            this.descripcion = "Se deberrá correr una velocidad minima de " + VELOCIDAD_MINIMA + "Km/h durante " + TIEMPO_MINIMO_SEG.ToString() + " s.";
            this.duration = 0;
            this.level = 0;
            this.maxAttempt = 0;
            this.states = new List<State>();
            this.states.Add(new State());
        }

        public int getPuntajeObtenido()
        {
            // OJOOOOOO HARDCODEADO PARA EL PROTOTIPO, EL STATE HAY QUE OBTENERLO DE LA LISTA.
            List<State>.Enumerator e = this.states.GetEnumerator();
            e.MoveNext();
            return e.Current.getPuntaje();
        }

        public void finish(TimeSpan timeSpan)
        {
            // OJOOOOOO HARDCODEADO PARA EL PROTOTIPO, EL STATE HAY QUE OBTENERLO DE LA LISTA.
            List<State>.Enumerator e = this.states.GetEnumerator();
            e.MoveNext();

            e.Current.setPuntaje(this.calculatePuntaje(timeSpan));
            e.Current.setFinished(true);
            e.Current.setCurrentAttempt(e.Current.getCurrentAttempt() + 1);
        }

        private int calculatePuntaje(TimeSpan timeSpan)
        {
            return 10 + Convert.ToInt32(timeSpan.TotalSeconds - UsainBolt.TIEMPO_MINIMO_SEG) * 2;
        }
    }
}
