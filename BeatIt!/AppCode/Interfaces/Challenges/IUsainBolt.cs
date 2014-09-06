using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;

namespace BeatIt_.AppCode.Interfaces.Challenges
{
    
    interface IUsainBolt
    {
        bool startGPS();

        double geMaxSpeed();

        int getPuntaje();

    }
}
