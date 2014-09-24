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
using System.IO.IsolatedStorage;
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

            State state = new State();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("UsainBoltScore"))
            {
                state.setPuntaje((int)settings["UsainBoltScore"]);
            }
            else
            {
                state.setPuntaje(0);
            }

            if (settings.Contains("UsainBoltBestTime"))
            {
                state.setBestTime((int)settings["UsainBoltBestTime"]);
            }

            this.states.Add(state);
        }

        public int getPuntajeObtenido()
        {
            // OJOOOOOO HARDCODEADO PARA EL PROTOTIPO, EL STATE HAY QUE OBTENERLO DE LA LISTA.
            List<State>.Enumerator e = this.states.GetEnumerator();
            e.MoveNext();
            return e.Current.getPuntaje();

        }

        public void finish(int tiempo)
        {
            // OJOOOOOO HARDCODEADO PARA EL PROTOTIPO, EL STATE HAY QUE OBTENERLO DE LA LISTA.
            List<State>.Enumerator e = this.states.GetEnumerator();
            e.MoveNext();

            e.Current.setPuntaje(this.calculatePuntaje(tiempo));
            e.Current.setFinished(true);
            e.Current.setCurrentAttempt(e.Current.getCurrentAttempt() + 1);
            
            // FALTARIA GUARDAR EL PUNTAJE.
            
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("UsainBoltScore"))
            {
                settings.Add("UsainBoltScore", e.Current.getPuntaje());
            }

            if (!settings.Contains("UsainBoltBestTime"))
            {
                settings.Add("UsainBoltBestTime", tiempo);
            }

            settings["UsainBoltScore"] = e.Current.getPuntaje();
            settings["UsainBoltBestTime"] = tiempo;

            settings.Save();
        }

        private int calculatePuntaje(int timeSpan)
        {
            return 10 + Convert.ToInt32(timeSpan - UsainBolt.TIEMPO_MINIMO_SEG) * 2;
        }
    }
}
