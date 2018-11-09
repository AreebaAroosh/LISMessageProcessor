using System;
using System.Collections.Generic;
using System.Diagnostics;
using LISMessageProcessor.Category;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCL.LISConnector.ASTMParser;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;

namespace LISMessageProcessor.Test.CategoryA
{
    [TestClass]
    public class ASTM
    {
        private ASTMDecoderA _decoder;
        private ASTMMessageProcessor _processor;
        private Message _message;
        private DeviceMessage _deviceMessage;
        private const string SendingFacility = "Test_Sending_Facility";
        private const string ClientId = "1234";

        public ASTM()
        {
            _decoder = new ASTMDecoderA();
            string strMessage = Helpers.GetStringFromFile(@"CategoryA\SampleMessages\ASTM.txt");
            _message = new Message(strMessage);
            _message.ParseMessage();
            string strEncodedMessage = $"{(char)0x02}{strMessage.Replace("\r\n", "\r")}{(char)0x03}";
            _deviceMessage = new DeviceMessage
            {
                ClientId = ClientId,
                DeviceCategory = "A",
                SendingFacility = SendingFacility,
                MessageType = "ASTM",
                ContentsList = new List<string> { strEncodedMessage }
            };
            _processor = new ASTMMessageProcessor(_decoder);
        }

        #region Decoder

        [TestMethod]
        public void GetPatient()
        {
            bool b = false;

            Patient patient = _decoder.GetPatient(_message);

            try
            {
                if (patient?.InternalPatientId != "0003")
                    Assert.Fail();
                if (patient?.FamilyName != "Fab")
                    Assert.Fail();
                if (patient?.GivenName != "Cesc")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (patient?.Sex != "M")
                    Assert.Fail();

                b = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void GetDiagnosticReport()
        {
            bool b = false;

            DiagnosticReport report = _decoder.GetDiagnosticReport(_message);

            try
            {
                if (report?.SendingApplication != "ACI2 Ser.# :UU13013667")
                    Assert.Fail();
                if (report?.PatientInternalId != "0003")
                    Assert.Fail();
                if (report?.FamilyName != "Fab")
                    Assert.Fail();
                if (report?.GivenName != "Cesc")
                    Assert.Fail();
                if (report?.Sex != "M")
                    Assert.Fail();
                if (report?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (report?.AnalyzerName != "ACI2 Ser.# :UU13013667")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyyMMddHHmmss") != "20130515143513")
                    Assert.Fail();
                if (report?.OperatorId != "ROCHE")
                    Assert.Fail();

                b = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void GetResults()
        {
            bool b = false;

            List<Result> _results = _decoder.GetResults(_message);
            Result _result = _results[0];

            try
            {
                if (_result?.TestCode != "Glu2")
                    Assert.Fail();
                if (_result?.Value != "87")
                    Assert.Fail();
                if (_result?.Units != "mg/dl")
                    Assert.Fail();
                if (_result?.ReferenceRange != " to")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyyMMddHHmmss") != "20130515143359")
                    Assert.Fail();
                if (_result?.Comments != "This is a patient comment")
                    Assert.Fail();

                b = true;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);
        }

        #endregion

        #region Processor

        [TestMethod]
        public void ProcessMessage()
        {
            bool b = false;

            List<PatientDiagnosticRecord> _diagnosticRecords = _processor.ProcessMessage(_deviceMessage);
            PatientDiagnosticRecord _diagnosticRecord = _diagnosticRecords[0];

            try
            {
                Patient patient = _diagnosticRecord.patient;

                if (patient?.InternalPatientId != "0003")
                    Assert.Fail();
                if (patient?.FamilyName != "Fab")
                    Assert.Fail();
                if (patient?.GivenName != "Cesc")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (patient?.Sex != "M")
                    Assert.Fail();
                if (patient?.ClientId != ClientId)
                    Assert.Fail();

                DiagnosticReport report = _diagnosticRecord.diagnosticReport;

                if (report?.SendingApplication != "ACI2 Ser.# :UU13013667")
                    Assert.Fail();
                if (report?.ReceivingApplication != LISMessageProcessor.Helpers.Constants.ReceivingApplicationName)
                    Assert.Fail();
                if (report?.ReceivingFacility != LISMessageProcessor.Helpers.Constants.ReceivingFacility)
                    Assert.Fail();
                if (report?.PatientInternalId != "0003")
                    Assert.Fail();
                if (report?.FamilyName != "Fab")
                    Assert.Fail();
                if (report?.GivenName != "Cesc")
                    Assert.Fail();
                if (report?.Sex != "M")
                    Assert.Fail();
                if (report?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (report?.AnalyzerName != "ACI2 Ser.# :UU13013667")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyyMMddHHmmss") != "20130515143513")
                    Assert.Fail();
                if (report?.OperatorId != "ROCHE")
                    Assert.Fail();
                if (report?.TestCodes != "Glu2")
                    Assert.Fail();
                if (report?.ClientId != ClientId)
                    Assert.Fail();

                List<Result> _results = _diagnosticRecord.results;
                Result _result = _results[0];

                if (_result?.TestCode != "Glu2")
                    Assert.Fail();
                if (_result?.Value != "87")
                    Assert.Fail();
                if (_result?.Units != "mg/dl")
                    Assert.Fail();
                if (_result?.ReferenceRange != " to")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyyMMddHHmmss") != "20130515143359")
                    Assert.Fail();
                if (_result?.Comments != "This is a patient comment")
                    Assert.Fail();
                if (_result?.ClientId != ClientId)
                    Assert.Fail();

                b = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);

        }

        #endregion
    }
}
