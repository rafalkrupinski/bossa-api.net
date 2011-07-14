using System;
using System.Globalization;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	/// <summary>
	/// Wewnętrzne funkcje pomocnicze. Głównie do parsowania parametrów FIXML'a. 
	/// </summary>
	internal static class FixmlUtil
	{

		internal static void Error(XmlElement xml, string name, object value, string msg)
		{
			msg = "{0}, attribute {1} " + ((value != null) ? "= '{2}' " : "") + msg;
			Exception error = new FixmlException(string.Format(msg, xml.Name, name, value));
			if (!FixmlMsg.DebugFormattedXml.Enabled && !FixmlMsg.DebugOriginalXml.Enabled)
				error.Data["ExtraMsg"] = MyUtil.FormattedXml(xml);
			throw error;
		}


		internal static XmlElement Element(XmlElement xml, string xpath)
		{
			return Element(xml, xpath, false);
		}

		internal static XmlElement Element(XmlElement xml, string xpath, bool optional)
		{
			XmlElement el = xml.SelectSingleNode(xpath) as XmlElement;
			if ((el == null) && !optional)
				throw new FixmlException(string.Format("{0}, element '{1}' not found", xml.Name, xpath));
			return el;
		}


		internal static string ReadString(XmlElement xml, string name)
		{
			return ReadString(xml, name, false);
		}

		internal static string ReadString(XmlElement xml, string name, bool optional)
		{
			int i = name.LastIndexOf('/');
			XmlElement elem = (i < 0) ? xml : Element(xml, name.Substring(0, i), optional);
			if (elem == null) return null;
			string value = elem.GetAttribute(name.Substring(i + 1));
			if (value != "") return value;
			if (!optional) Error(xml, name, null, "missing");
			return null;
		}


		internal static char ReadChar(XmlElement xml, string name)
		{
			return (char)ReadChar(xml, name, false);
		}

		internal static char? ReadChar(XmlElement xml, string name, bool optional)
		{
			string str = ReadString(xml, name, optional);
			if (str == null) return null;
			char ch;
			if (!char.TryParse(str, out ch))
				Error(xml, name, str, "- single character expected");
			return ch;
		}


		internal static int ReadInt(XmlElement xml, string name)
		{
			return (int)ReadInt(xml, name, false);
		}

		internal static int? ReadInt(XmlElement xml, string name, bool optional)
		{
			string str = ReadString(xml, name, optional);
			if (str == null) return null;
			int number;
			if (!int.TryParse(str, out number))
				Error(xml, name, str, "- not a valid number");
			return number;
		}


		internal static uint ReadUInt(XmlElement xml, string name)
		{
			return (uint)ReadUInt(xml, name, false);
		}

		internal static uint? ReadUInt(XmlElement xml, string name, bool optional)
		{
			int? number = ReadInt(xml, name, optional);
			if (number == null) return null;
			if (number < 0)
				Error(xml, name, number.ToString(), "- unexpected negative number");
			return (uint)number;
		}


		internal static decimal ReadDecimal(XmlElement xml, string name)
		{
			return (decimal)ReadDecimal(xml, name, false);
		}

		internal static decimal? ReadDecimal(XmlElement xml, string name, bool optional)
		{
			string str = ReadString(xml, name, optional);
			if (str == null) return null;
			decimal value;
			NumberFormatInfo format = new NumberFormatInfo() { NumberGroupSeparator = " " };
			if (!decimal.TryParse(str, NumberStyles.Number, format, out value))
				Error(xml, name, str, "- not a valid decimal number");
			return value;
		}

		internal static string WriteDecimal(decimal? value)
		{
			return ((decimal)value).ToString("0.00", CultureInfo.InvariantCulture);
		}



		internal static DateTime ReadDateTime(XmlElement xml, string name)
		{
			return ReadDateTime(xml, name, null);
		}

		internal static DateTime? ReadDateTime(XmlElement xml, string name, bool optional)
		{
			return ReadDateTime(xml, name, null, optional);
		}

		internal static DateTime ReadDateTime(XmlElement xml, string name, string name2)
		{
			return (DateTime)ReadDateTime(xml, name, name2, false);
		}

		internal static DateTime? ReadDateTime(XmlElement xml, string name, string name2, bool optional)
		{
			string str = ReadString(xml, name, optional);
			if (str == null) return null;
			if (name2 != null)
				str += "-" + ReadString(xml, name2, false);
			DateTime date;
			string[] formats = new[] { "yyyyMMdd-HH:mm:ss", "yyyyMMdd-HH:mm", "yyyyMMdd" };
			IFormatProvider provider = CultureInfo.InvariantCulture;
			DateTimeStyles styles = DateTimeStyles.None;
			if (!DateTime.TryParseExact(str, formats, provider, styles, out date))
				Error(xml, (name2 == null ? name : name + "+" + name2), str, "- not a valid date/time");
			return date;
		}

		internal static string WriteDate(DateTime date)
		{
			return date.ToString("yyyyMMdd");
		}

		internal static string WriteDateTime(DateTime dt)
		{
			return dt.ToString("yyyyMMdd-HH:mm:ss");
		}

	}
}
