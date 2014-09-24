﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Interfaces
{
    interface IFacadeController
    {
        bool isLoggedUser();
        User getCurrentUser();
        List<DTRanking> getRanking();
    }
}
