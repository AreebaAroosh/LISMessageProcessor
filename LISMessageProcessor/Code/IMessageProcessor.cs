using RCL.LISConnector.DataEntity.IOT;
using System;
using System.Collections.Generic;
using System.Text;

namespace LISMessageProcessor
{
    public interface IMessageProcessor
    {
        List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage);
    }
}
