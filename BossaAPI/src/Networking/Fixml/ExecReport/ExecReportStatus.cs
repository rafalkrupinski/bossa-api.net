using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum ExecReportStatus
	{
		New,              // nowe
		Expired,          // archiwalne
		PendingReplace,   // w trakcie modyfikacji
		PartiallyFilled,  // wykonane/aktywne
		Filled,           // wykonane
		Canceled,         // anulowane
		PendingCancel,    // w trakcie anulaty
		Rejected          // odrzucone
	}

	public static class ExecRptStatUtil
	{
		private static Dictionary<string, ExecReportStatus> dict =
			new Dictionary<string, ExecReportStatus> {
                    { "0", ExecReportStatus.New },
                    { "C", ExecReportStatus.Expired },
                    { "E", ExecReportStatus.PendingReplace },
                    { "1", ExecReportStatus.PartiallyFilled },
                    { "2", ExecReportStatus.Filled },
                    { "4", ExecReportStatus.Canceled },
                    { "6", ExecReportStatus.PendingCancel },
                    { "8", ExecReportStatus.Rejected },
                    { "80", ExecReportStatus.Canceled },     // tymczasowo(?)
                    { "81", ExecReportStatus.Expired }       // tymczasowo(?)
            };

		public static ExecReportStatus? Read(XmlElement xml, string name, bool optional)
		{
			string str = FixmlUtil.ReadString(xml, name, optional);
			if (str == null) return null;
			if (!dict.ContainsKey(str))
				throw new FixmlException(string.Format("Unknown OrderStatus: '{0}'", str));
			return dict[str];
		}
	}
}
