﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeHttp.FiddlerHelper
{
    [Serializable]
    public class FiddlerModificHttpRuleCollection
    {
        List<FiddlerRequsetChange> requestRuleList;
        List<FiddlerResponseChange> responseRuleList;

        public List<FiddlerRequsetChange> RequestRuleList { get { return requestRuleList; } set { requestRuleList = value; } }
        public List<FiddlerResponseChange> ResponseRuleList { get { return responseRuleList; } set { responseRuleList = value; } }


        public FiddlerModificHttpRuleCollection()  // Serializable 需要空参数的构造函数
        {
            requestRuleList = null;
            responseRuleList = null;
        }

        public FiddlerModificHttpRuleCollection(List<FiddlerRequsetChange> yourRequestRuleList, List<FiddlerResponseChange> yourResponseRuleList)
        {
            requestRuleList = yourRequestRuleList;
            responseRuleList = yourResponseRuleList;
        }
    }
}
