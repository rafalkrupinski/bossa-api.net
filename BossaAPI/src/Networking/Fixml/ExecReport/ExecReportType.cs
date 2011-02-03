using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    // określa powód dostarczenia danego raportu
    public enum ExecReportType  
    {
        New = '0',              // nowe zlecenie
        Trade = 'F',            // wykonane (może być częściowo)
        Canceled = '4',         // anulowane
        Replaced = '5',         // zmodyfikowane
        PendingCancel = '6',    // w trakcie anulowania
        PendingReplace = 'E',   // w trakcie modyfikacji
        Rejected = '8',         // odrzucenie zlecenia
        OrderStatus = 'I'       // status zlecenia(?)
    }

    internal static class ExecRptTypeUtil
    {
        public static ExecReportType? Read(XmlElement xml, string name, bool optional)
        {
            char? ch = FixmlUtil.ReadChar(xml, name, optional);
            if (ch == null) return null;
            if (!Enum.IsDefined(typeof(ExecReportType), (ExecReportType)ch))
                FixmlUtil.Error(xml, name, ch, "- unknown ExecType");
            return (ExecReportType)ch;
        }
    }
}
