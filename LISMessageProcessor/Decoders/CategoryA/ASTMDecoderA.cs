using RCL.LISConnector.ASTMParser;
using RCL.LISConnector.DataEntity.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LISMessageProcessor.Category
{
    public class ASTMDecoderA : IMessageDecoder<Message>
    {
        public  Patient GetPatient(Message message)
        {
            Patient patient = new Patient();

            try
            {
                string INTERNALPATIENTID = message.GetValue("P.3");
                if (!string.IsNullOrEmpty(INTERNALPATIENTID))
                    patient.InternalPatientId = INTERNALPATIENTID;

                string FAMILYNAME = message.GetValue("P.6.1");
                if (!string.IsNullOrEmpty(FAMILYNAME))
                    patient.FamilyName = FAMILYNAME;

                string GIVENNAME = message.GetValue("P.6.2");
                if (!string.IsNullOrEmpty(GIVENNAME))
                    patient.GivenName = GIVENNAME;

                string MIDDLENAME = message.GetValue("P.6.3");
                if (!string.IsNullOrEmpty(MIDDLENAME))
                    patient.MiddleName = MIDDLENAME;

                string DATEOFBIRTH = message.GetValue("P.8");
                if (!string.IsNullOrEmpty(DATEOFBIRTH))
                    patient.DateOfBirth = Helpers.Converters.ConvertStringToDate(message.GetValue("P.8"), "yyyyMMdd");

                string SEX = message.GetValue("P.9");
                if (!string.IsNullOrEmpty(SEX))
                    patient.Sex = SEX;

                string RACE = message.GetValue("P.10");
                if (!string.IsNullOrEmpty(RACE))
                    patient.Race = RACE;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return patient;
        }

        public  DiagnosticReport GetDiagnosticReport(Message message)
        {
            DiagnosticReport diagnosticReport = new DiagnosticReport();

            try
            {
                string SendingApplication = message.GetValue("H.5");
                if (!string.IsNullOrEmpty(SendingApplication))
                    diagnosticReport.SendingApplication = SendingApplication;

                string FAMILYNAME = message.GetValue("P.6.1");
                if (!string.IsNullOrEmpty(FAMILYNAME))
                    diagnosticReport.FamilyName = FAMILYNAME;

                string GIVENNAME = message.GetValue("P.6.2");
                if (!string.IsNullOrEmpty(GIVENNAME))
                    diagnosticReport.GivenName = GIVENNAME;

                string DATEOFBIRTH = message.GetValue("P.8");
                if (!string.IsNullOrEmpty(DATEOFBIRTH))
                    diagnosticReport.DateOfBirth = Helpers.Converters.ConvertStringToDate(message.GetValue("P.8"), "yyyyMMdd");

                string SEX = message.GetValue("P.9");
                if (!string.IsNullOrEmpty(SEX))
                    diagnosticReport.Sex = SEX;

                string INTERNALPATIENTID = message.GetValue("P.3");
                if (!string.IsNullOrEmpty(INTERNALPATIENTID))
                    diagnosticReport.PatientInternalId = INTERNALPATIENTID;

                string AnalyzerName = message.GetValue("H.5");
                if (!string.IsNullOrEmpty(AnalyzerName))
                    diagnosticReport.AnalyzerName = AnalyzerName;

                string AnalyzerDateTime = message.GetValue("H.14");
                if (!string.IsNullOrEmpty(AnalyzerDateTime))
                    diagnosticReport.AnalyzerDateTime = Helpers.Converters.ConvertStringToDate(AnalyzerDateTime, "yyyyMMddHHmmss");

                List<Record> RList = message.Records("R");
                string OperatorId = message.Records("R")[0]?.Fields(10)?.Value;
                if (!string.IsNullOrEmpty(OperatorId))
                    diagnosticReport.OperatorId = OperatorId;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return diagnosticReport;
        }

        public  List<Result> GetResults(Message message)
        {
            List<Result> lstResults = new List<Result>();

            try
            {
                List<Record> RList = message.Records("R");
                int NoRRecords = RList.Count;
              
                for (int i = 0; i < NoRRecords; i++)
                {
                    Result result = new Result();

                    string TestCode = message.Records("R")[i]?.Fields(2)?.Value;
                    if (!string.IsNullOrEmpty(TestCode))
                        result.TestCode = TestCode.Replace("^", string.Empty);

                    string Value = message.Records("R")[i]?.Fields(3)?.Value;
                    if (!string.IsNullOrEmpty(Value))
                        result.Value = Value;

                    string Units = message.Records("R")[i]?.Fields(4)?.Value;
                    if (!string.IsNullOrEmpty(Units))
                        result.Units = Units;

                    string RefRange = message.Records("R")[i]?.Fields(5)?.Value;
                    if (!string.IsNullOrEmpty(RefRange))
                        result.ReferenceRange = RefRange;

                    string AbFlags = message.Records("R")[i]?.Fields(6)?.Value;
                    if (!string.IsNullOrEmpty(AbFlags))
                        result.AbnormalFlags = AbFlags;

                    string RsltDateTime = message.Records("R")[i]?.Fields(12)?.Value;
                    result.ResultDateTime = Helpers.Converters.ConvertStringToDate(RsltDateTime, "yyyyMMddHHmmss");

                    lstResults.Add(result);
                }

                List<Record> CList = message.Records("C");
                int NoCRecords = CList.Count;
                Dictionary<int, string> dictComments = new Dictionary<int, string>();

                if (NoCRecords > 0)
                {
                    for(int i = 0; i < NoCRecords; i++)
                    {
                        string Comments = message.Records("C")[i]?.Fields(3)?.Value;
                        if (!string.IsNullOrEmpty(Comments))
                            dictComments.Add(i, Comments);
                    }
                }

                if (lstResults?.Count > 0)
                {
                    for (int i = 0; i < lstResults.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dictComments[i]))
                        {
                            lstResults[i].Comments = dictComments[i];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return lstResults;
        }
    }
}
