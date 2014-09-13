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
using BeatIt_.AppCode.Enums;

namespace BeatIt_.AppCode.Classes
{
    public class State
    {
        private bool finished;
        private int puntaje;
        private DateTime fechaDeInicio;
        private int currentAttempt;

        private Round round;
        private Challenge challenge;


        public State()
        {
            this.finished = false;
            this.puntaje = 0;
            this.fechaDeInicio = System.DateTime.Now;
            this.currentAttempt = 0;
        }




        public bool getFinished()
        {
            return this.finished;
        }

        public void setFinished(bool finished)
        {
            this.finished = finished;
        }

        public int getPuntaje()
        {
            return this.puntaje;
        }

        public void setPuntaje(int puntaje)
        {
            this.puntaje = puntaje;
        }

        public DateTime getFechaInicio()
        {
            return this.fechaDeInicio;
        }

        public void setFechaInicio(DateTime fechaInicio)
        {
            this.fechaDeInicio = fechaInicio;
        }

        public int getCurrentAttempt()
        {
            return this.currentAttempt;
        }

        public void setCurrentAttempt(int attempt)
        {
            this.currentAttempt = attempt;
        }
    }
}
