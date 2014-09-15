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
using System.Collections.Generic;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Classes
{
    public class Challenge
    {
        protected string id;
        protected string name;
        protected string descripcion;
        protected int duration; // Duracion en enteros? que significa?
        protected int level;
        protected int maxAttempt;

        protected List<State> states;

        public Challenge()
        {
        }

        public string getId()
        {
            return this.id;
        }

        public string getName()
        {
            return this.name;
        }

        public string getDescripcion()
        {
            return this.descripcion;
        }

        public int getDuration()
        {
            return this.duration;
        }

        public int getLevel()
        {
            return this.level;
        }

        public int getMaxAttempt()
        {
            return this.maxAttempt;
        }

        public DTChallenge getDTChallenge()
        {
            List<State>.Enumerator e = this.states.GetEnumerator();
            e.MoveNext();

            State estadoActual = e.Current; // ojo, esto es en el prototipo, hay que buscar el estado en la lista por la fecha.

            return new DTChallenge(this.id,
                                   this.name,
                                   this.descripcion,
                                   this.duration,
                                   this.level,
                                   estadoActual.getFinished(),
                                   estadoActual.getCurrentAttempt(),
                                   estadoActual.getPuntaje(),
                                   estadoActual.getFechaInicio(),
                                   estadoActual.getBestTime());
        }
    }
}
