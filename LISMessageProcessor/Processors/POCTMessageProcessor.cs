using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.POCTParser;
using RCL.LISConnector.POCTParser.Models;

namespace LISMessageProcessor
{
    public class POCTMessageProcessor : ProcessorBase<POCTMessage>, IMessageProcessor
    {
        public POCTMessageProcessor(IMessageDecoder<POCTMessage> messageDecoder)
            :base(messageDecoder)
        {
        }

        public List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage)
        {
            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {
                string strOBSR01 = deviceMessage.ContentsList[0];
                string strHELR01 = deviceMessage.ContentsList[1];

                OBSR01 _obsr01 = Serialization.DeserializeObject<OBSR01>(strOBSR01);
                Service[] services = _obsr01.services;
                List<Service> lstService = services.OfType<Service>().ToList();

                foreach (Service svc in lstService)
                {
                    POCTMessage message = new POCTMessage
                    {
                        svc = svc,
                        helr01 = strHELR01
                    };

                    records = Helpers.GetPatientDiagnosticRecords<POCTMessage>(
                        _messageDecoder,
                        message,
                        deviceMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return records;
        }
    }
}
