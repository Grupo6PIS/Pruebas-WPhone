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

namespace BeatIt_.AppCode.Classes
{
    public class Challenge
    {
        protected string name;
        protected string descripcion;
        protected DateTime dateInit;
        private State estado;
        private Round ronda;

        public Challenge()
        {
            dateInit = new DateTime();
        }

        public string getName()
        {
            return name;
        }

        public string getDescripcion()
        {
            return descripcion;
        }

        public DateTime getFechaDeInicio()
        {
            return dateInit;
        }


    }
}
