using RCL.LISConnector.POCTParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LISMessageProcessor
{
    public class POCTMessage
    {
        public Service svc { get; set; }
        public string helr01 { get; set; }
    }
}
