using System;
using System.Collections.Generic;
using System.Diagnostics;
using LISMessageProcessor.Category;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.POCTParser;
using RCL.LISConnector.POCTParser.Models;

namespace LISMessageProcessor.Test.CategoryA
{
    [TestClass]
    public class POCT
    {
        private POCTDecoderA _decoder;
        private POCTMessageProcessor _processor;
        private POCTMessage _message;
        private DeviceMessage _deviceMessage;
        private const string SendingFacility = "Test_Sending_Facility";
        private const string ClientId = "1234";

        public POCT()
        {
            _decoder = new POCTDecoderA();
            string strHELR01 = Helpers.GetStringFromFile(@"CategoryA\SampleMessages\POCT_HELR01.xml");
            string strOBSR01 = Helpers.GetStringFromFile(@"CategoryA\SampleMessages\POCT_OBSR01.xml");
            OBSR01 _obsr01 = Serialization.DeserializeObject<OBSR01>(strOBSR01);
            Service _svc = _obsr01.services[0];
            _message = new POCTMessage
            {
                svc = _svc,
                helr01 = strHELR01
                
            };
            _deviceMessage = new DeviceMessage
            {
                ClientId = ClientId,
                DeviceCategory = "A",
                SendingFacility = SendingFacility,
                MessageType = "POCT",
                ContentsList = new List<string> { strOBSR01, strHELR01 }
            };
            _processor = new POCTMessageProcessor(_decoder);
        }

        #region Decoder

        [TestMethod]
        public void GetPatient()
        {
            bool b = false;

            RCL.LISConnector.DataEntity.SQL.Patient patient = _decoder.GetPatient(_message);

            try
            {
                if (patient?.InternalPatientId != "PT222-55-7777")
                    Assert.Fail();
                if (patient?.FamilyName != "Patient")
                    Assert.Fail();
                if (patient?.GivenName != "Janet")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyy-MM-dd") != "1960-08-29")
                    Assert.Fail();
                if (patient?.Sex != "F")
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
                if (report?.SendingApplication != "ICU-4 Glucose")
                    Assert.Fail();
                if (report?.PatientInternalId != "PT222-55-7777")
                    Assert.Fail();
                if (report?.FamilyName != "Patient")
                    Assert.Fail();
                if (report?.GivenName != "Janet")
                    Assert.Fail();
                if (report?.Sex != "F")
                    Assert.Fail();
                if (report?.DateOfBirth.Value.ToString("yyyy-MM-dd") != "1960-08-29")
                    Assert.Fail();
                if (report?.AnalyzerName != "ICU-4 Glucose")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyy-MM-dd") != "2001-11-01")
                    Assert.Fail();
                if (report?.OperatorId != "OP777-88-9999")
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
                if (_result?.TestCode != "1517-2")
                    Assert.Fail();
                if (_result?.Value != "85")
                    Assert.Fail();
                if (_result?.Units != "mg/dL")
                    Assert.Fail();
                if (_result?.ReferenceRange != "[Normal][80;120] [Critical][30;160] mg/dL")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyy-MM-dd") != "2001-11-01")
                    Assert.Fail();
                if (_result?.Comments != "Temp warning")
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
                RCL.LISConnector.DataEntity.SQL.Patient patient = _diagnosticRecord.patient;

                if (patient?.InternalPatientId != "PT222-55-7777")
                    Assert.Fail();
                if (patient?.FamilyName != "Patient")
                    Assert.Fail();
                if (patient?.GivenName != "Janet")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyy-MM-dd") != "1960-08-29")
                    Assert.Fail();
                if (patient?.Sex != "F")
                    Assert.Fail();
                if (patient?.ClientId != ClientId)
                    Assert.Fail();

                DiagnosticReport report = _diagnosticRecord.diagnosticReport;

                if (report?.SendingApplication != "ICU-4 Glucose")
                    Assert.Fail();
                if (report?.ReceivingApplication != LISMessageProcessor.Helpers.Constants.ReceivingApplicationName)
                    Assert.Fail();
                if (report?.ReceivingFacility != LISMessageProcessor.Helpers.Constants.ReceivingFacility)
                    Assert.Fail();
                if (patient?.InternalPatientId != "PT222-55-7777")
                    Assert.Fail();
                if (patient?.FamilyName != "Patient")
                    Assert.Fail();
                if (patient?.GivenName != "Janet")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyy-MM-dd") != "1960-08-29")
                    Assert.Fail();
                if (patient?.Sex != "F")
                    Assert.Fail();
                if (report?.AnalyzerName != "ICU-4 Glucose")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyy-MM-dd") != "2001-11-01")
                    Assert.Fail();
                if (report?.OperatorId != "OP777-88-9999")
                    Assert.Fail();
                if (report?.TestCodes != "1517-2")
                    Assert.Fail();
                if (report?.ClientId != ClientId)
                    Assert.Fail();

                List<Result> _results = _diagnosticRecord.results;
                Result _result = _results[0];

                if (_result?.TestCode != "1517-2")
                    Assert.Fail();
                if (_result?.Value != "85")
                    Assert.Fail();
                if (_result?.Units != "mg/dL")
                    Assert.Fail();
                if (_result?.ReferenceRange != "[Normal][80;120] [Critical][30;160] mg/dL")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyy-MM-dd") != "2001-11-01")
                    Assert.Fail();
                if (_result?.Comments != "Temp warning")
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
