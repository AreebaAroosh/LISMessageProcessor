using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.POCTParser;
using RCL.LISConnector.POCTParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LISMessageProcessor.Category
{
    public class POCTDecoderA : IMessageDecoder<POCTMessage>
    {
        public RCL.LISConnector.DataEntity.SQL.Patient GetPatient(POCTMessage message)
        {
            RCL.LISConnector.DataEntity.SQL.Patient patient = new RCL.LISConnector.DataEntity.SQL.Patient();
            
            try
            {
                Service svc = message.svc;

                if (!string.IsNullOrEmpty(svc?.patient?.patient_id?.Value))
                    patient.InternalPatientId = svc.patient.patient_id.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.family?.Value))
                    patient.FamilyName = svc.patient.patientName.family.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.given?.Value))
                    patient.GivenName = svc.patient.patientName.given.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.birth_date?.Value))
                    patient.DateOfBirth = Helpers.Converters.ConvertPOCTStringToDateTime(svc.patient.birth_date.Value);

                if (!string.IsNullOrEmpty(svc?.patient?.gender_cd?.Value))
                    patient.Sex = svc?.patient?.gender_cd?.Value;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return patient;
        }

        public DiagnosticReport GetDiagnosticReport(POCTMessage message)
        {
            DiagnosticReport _report = new DiagnosticReport();
            List<Service> lstService = new List<Service>();

            try
            {
                HELR01 _helr01 = Serialization.DeserializeObject<HELR01>(message.helr01);
                Service svc = message.svc;

                if (!string.IsNullOrEmpty(_helr01?.device?.device_name?.Value))
                    _report.SendingApplication = _helr01.device.device_name.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patient_id?.Value))
                    _report.PatientInternalId = svc.patient.patient_id.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.family?.Value))
                    _report.FamilyName = svc.patient.patientName.family.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.given?.Value))
                    _report.GivenName = svc.patient.patientName.given.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.birth_date?.Value))
                    _report.DateOfBirth = Helpers.Converters.ConvertPOCTStringToDateTime(svc.patient.birth_date.Value);

                if (!string.IsNullOrEmpty(svc?.patient?.gender_cd?.Value))
                    _report.Sex = svc?.patient?.gender_cd?.Value;

                if (!string.IsNullOrEmpty(_helr01?.header?.creation_dttm?.Value))
                    _report.AnalyzerDateTime = Helpers.Converters.ConvertPOCTStringToDateTime(_helr01.header.creation_dttm.Value);
                else
                    _report.AnalyzerDateTime = DateTime.Now;

                if (!string.IsNullOrEmpty(_helr01?.device?.device_name?.Value))
                    _report.AnalyzerName = _helr01.device.device_name.Value;

                if (!string.IsNullOrEmpty(svc?.operatorid?.operator_id?.Value))
                    _report.OperatorId = svc.operatorid.operator_id.Value;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _report;
        }

        public List<Result> GetResults(POCTMessage message)
        {
            List<Result> _results = new List<Result>();

            try
            {
                Service svc = message.svc;
                Observation[] obs = svc.patient.observations;
                List<Observation> lstObs = obs.OfType<Observation>().ToList();
                foreach (Observation item in lstObs)
                {
                    Result result = new Result();

                    if (!string.IsNullOrEmpty(item?.normal_lo_hi_limit?.Value) && !string.IsNullOrEmpty(item?.critical_lo_hi_limit?.Value))
                        result.ReferenceRange = $"[Normal]{item.normal_lo_hi_limit.Value} [Critical]{item.critical_lo_hi_limit.Value} {item.value.Unit}";

                    if (!string.IsNullOrEmpty(svc?.observation_dttm?.Value))
                        result.ResultDateTime = Helpers.Converters.ConvertPOCTStringToDateTime(svc.observation_dttm.Value);
                    else
                        result.ResultDateTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(item?.observation_id?.Value))
                        result.TestCode = item.observation_id.Value;

                    if (!string.IsNullOrEmpty(item?.value?.Unit))
                        result.Units = item.value.Unit;

                    if (!string.IsNullOrEmpty(item?.value?.Value))
                        result.Value = item.value.Value;

                    if (item?.notes != null)
                    {
                        Note defaultNote = item.notes[0];
                        if (!string.IsNullOrEmpty(defaultNote?.text?.Value))
                        {
                            result.Comments = defaultNote.text.Value;
                        }
                    }

                    _results.Add(result);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _results;
        }
    }
}
