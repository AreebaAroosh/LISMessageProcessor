using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.POCTParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LISMessageProcessor
{
    public static class Helpers
    {
        public static class Constants
        {
            public static string ReceivingApplicationName = "Health_IoT_Hub";
            public static string ReceivingFacility = "Health_IoT_Facility";

            public static string HL7 = "HL7";
            public static string ASTM = "ASTM";
            public static string POCT = "POCT";
        }

        public static class Converters
        {
            public static DateTime? ConvertStringToDate(string Value, string Format)
            {
                DateTime? dtr = null;
                try
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(Value, Format,
                                              CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt))
                    {
                        dtr = (DateTime?)dt;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                return dtr;
            }

            public static DateTime ConvertPOCTStringToDate(string date)
            {
                DateTime dt = new DateTime();

                try
                {
                    if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ss:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return dt.Date;
            }

            public static DateTime ConvertPOCTStringToDateTime(string date)
            {
                DateTime dt = new DateTime();

                try
                {
                    if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ss:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return dt;
            }
        }

        public static string GetTestCodesFromResults(List<Result> Results)
        {
            string testCodes = string.Empty;

            try
            {
                string r = string.Empty;

                try
                {
                    if (Results?.Count > 0)
                    {
                        foreach (var item in Results)
                        {
                            if (r?.Length < 350)
                            {
                                r = $"{r},{item.TestCode}";
                            }
                        }

                        r = r.TrimStart(',');
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                return r;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return testCodes;
        }

        //public static string GetPOCTTestCodesFromOBS(Observation[] Obs)
        //{
        //    string r = string.Empty;

        //    try
        //    {
        //        List<Observation> lstObs = Obs.OfType<Observation>().ToList();

        //        if (lstObs?.Count > 0)
        //        {
        //            foreach (Observation obs in lstObs)
        //            {
        //                if (r?.Length < 350)
        //                {
        //                    r = $"{r},{obs.observation_id?.Value ?? string.Empty}";
        //                }
        //            }

        //            r = r.TrimStart(',');
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }

        //    return r;
        //}

        public static List<PatientDiagnosticRecord> GetPatientDiagnosticRecords<T>(IMessageDecoder<T> Decoder, T Message, DeviceMessage DeviceMessage) 
        {
            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {
                RCL.LISConnector.DataEntity.SQL.Patient _patient = Decoder.GetPatient(Message);
                string PatientInternalId = _patient?.InternalPatientId;
                if (!string.IsNullOrEmpty(PatientInternalId))
                {
                    _patient.ClientId = DeviceMessage.ClientId;

                    var _diagnosticReport = Decoder.GetDiagnosticReport(Message);
                    if (_diagnosticReport != null)
                    {
                        _diagnosticReport.ReceivingApplication = Constants.ReceivingApplicationName;
                        _diagnosticReport.ReceivingFacility = Constants.ReceivingFacility;
                        _diagnosticReport.SendingFacility = DeviceMessage.SendingFacility;
                        _diagnosticReport.ClientId = DeviceMessage.ClientId;

                        var _results = Decoder.GetResults(Message);
                        if (_results?.Count > 0)
                        {
                            List<Result> ulResult = new List<Result>();
                            foreach (Result r in _results)
                            {
                                r.ClientId = DeviceMessage.ClientId;
                                ulResult.Add(r);
                            }

                            string TestCodes = Helpers.GetTestCodesFromResults(_results);

                            if (!string.IsNullOrEmpty(TestCodes))
                            {
                                _diagnosticReport.TestCodes = TestCodes;
                            }

                            PatientDiagnosticRecord _record = new PatientDiagnosticRecord
                            {
                                patient = _patient,
                                diagnosticReport = _diagnosticReport,
                                results = ulResult
                            };

                            records.Add(_record);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return records;
        }
    }
}
