using RCL.LISConnector.ASTMParser;
using RCL.LISConnector.DataEntity.IOT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace LISMessageProcessor
{
    public class ASTMMessageProcessor : ProcessorBase<Message>, IMessageProcessor
    {
        public ASTMMessageProcessor(IMessageDecoder<Message> messageDecoder)
             : base(messageDecoder)
        {
        }

        public  List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage)
        {
            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {
                string strRawMessage = deviceMessage.ContentsList[0];
                string strMessage = ExtractMessage(strRawMessage);
                Message message = new Message(strMessage);
                bool b = message.ParseMessage();

                records = Helpers.GetPatientDiagnosticRecords<Message>(
                                        _messageDecoder,
                                        message,
                                        deviceMessage);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return records;
        }

        // [TODO : Move to ASTMParser Lib : MessageHelper class]
        public string ExtractMessage(string message)
        {
            string msg = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                var expr = "\x02(.*?)\x03";
                var matches = Regex.Matches(message, expr);
                var list = new List<string>();
                foreach (Match m in matches)
                {
                    sb = sb.Append(m.Groups[1].Value);
                }

                msg = sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return msg;
        }
    }
}
