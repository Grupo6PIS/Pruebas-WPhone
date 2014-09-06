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

        private TState estado;
        private int puntate = 0;
        private DateTime fechaDeInicio;
        private User usuario;


        public TState getEstado()
        {
            return this.estado;
        }

        public int getPuntaje()
        {
            return this.puntate;
        }

        public DateTime getFechaInicio()
        {
            return fechaDeInicio;
        }

    }
}
