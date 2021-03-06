﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeHttp.FiddlerHelper
{
    public interface IFiddlerHttpTamper
    {
        bool IsEnable { get; set; }
        FiddlerHttpFilter HttpFilter { get; set; }
        List<FreeHttp.AutoTest.ParameterizationPick.ParameterPick> ParameterPickList { get; set; }
        FiddlerActuatorStaticDataCollectionController ActuatorStaticDataController { get; set; }
        bool IsRawReplace { get; }
    }
}
