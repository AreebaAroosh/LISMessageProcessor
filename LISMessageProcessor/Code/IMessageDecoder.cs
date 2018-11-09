using RCL.LISConnector.DataEntity.SQL;
using System.Collections.Generic;

namespace LISMessageProcessor
{
    public interface IMessageDecoder<T>
    {
        Patient GetPatient(T message);
        DiagnosticReport GetDiagnosticReport(T message);
        List<Result> GetResults(T message);
    }
}
