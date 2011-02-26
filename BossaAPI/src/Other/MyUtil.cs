using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Extension methods używane w projekcie, głównie przy debugowaniu. 
	/// </summary>
	public static class MyUtil
	{

		public static void PrintError(this Exception e)
		{
			string txt = string.Format("\n##ERROR##  {0}: \"{1}\"  ##ERROR##{2}\n", e.GetType().Name, e.Message,
				(e.Data.Contains("ExtraMsg") ? "\n" + e.Data["ExtraMsg"] : ""));
			Trace.WriteLine(txt);
		}

		public static void PrintWarning(this Exception e)
		{
			PrintWarning(e.Message);
		}

		public static void PrintWarning(string message)
		{
			Trace.WriteLine("#WARNING#  \"" + message + "\"  #WARNING#");
		}

		public static string FormattedXml(this XmlDocument doc)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			StringBuilder builder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder, settings);
			doc.Save(writer);
			return builder.ToString();
		}

		public static string FormattedXml(this XmlElement elem)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(elem.OuterXml);
			return doc.FormattedXml();
		}

	}
}
