using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.HL7Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LISMessageProcessor
{
    public class HL7MessageProcessor : ProcessorBase<Message>, IMessageProcessor
    {
        public HL7MessageProcessor(IMessageDecoder<Message> messageDecoder)
            :base(messageDecoder)
        {
        }

        public List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage)
        {
            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {
                string strMessage = deviceMessage.ContentsList[0];
                List<Message> Messages = GetMessages(strMessage);
                foreach (Message message in Messages)
                {
                    records = Helpers.GetPatientDiagnosticRecords<Message>(
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

        public List<Message> GetMessages(string strMessage)
        {
            List<Message> messages = new List<Message>();

            try
            {
                var _messages = MessageHelper.ExtractMessages(strMessage);
                foreach (var strMsg in _messages)
                {
                    Message _message = new Message(strMsg);
                    bool isParsed = false;
                    try
                    {
                        isParsed = _message.ParseMessage();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    messages.Add(_message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return messages;
        }

    }
}
