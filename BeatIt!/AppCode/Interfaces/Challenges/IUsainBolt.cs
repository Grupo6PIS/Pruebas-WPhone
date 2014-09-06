using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;

namespace BeatIt_.AppCode.Interfaces.Challenges
{
    interface IUsainBolt
    {

        public delegate void getCurrentSpeed(double speed);
        public delegate void getCurrentState(GeoPositionStatus gs);
        // Disabled, Initializing, NoData, NoData estados posibles de GeoPositionStatus

        public bool startGPS();

        public double geMaxSpeed();

        public int getPuntaje();

    }
}
