using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	/// <summary>
	/// Klasa opisujący konkretny papier wartościowy przekazywany w parametrach komunikatu FIXML.
	/// Implementuje wewnętrznie słownik poznanych dotąd odwzorowań kodów ISIN na zwykłe symbole papierów.
	/// </summary>
	public class FixmlInstrument
	{
		public string Symbol { get; private set; }
		public string SecurityId { get; private set; }

		private static Dictionary<string, FixmlInstrument> symbDict;
		private static Dictionary<string, FixmlInstrument> isinDict;

		static FixmlInstrument()
		{
			symbDict = new Dictionary<string, FixmlInstrument>();
			isinDict = new Dictionary<string, FixmlInstrument>();
		}

		private FixmlInstrument(string symb, string isin)
		{
			Symbol = symb;
			SecurityId = isin;
			if (symb != null) symbDict.Add(symb, this);
			if (isin != null) isinDict.Add(isin, this);
		}

		public static FixmlInstrument Find(DTO.Instrument instr)
		{
			return Find(instr.Symbol, instr.ISIN);
		}

		public static FixmlInstrument Find(string symb, string isin)
		{
			if (isin == null) return FindBySym(symb);
			if (symb == null) return FindById(isin);
			FixmlInstrument xi = isinDict.ContainsKey(isin) ? isinDict[isin] : null;
			FixmlInstrument xs = symbDict.ContainsKey(symb) ? symbDict[symb] : null;
			if ((xi == null) && (xs == null))
				return new FixmlInstrument(symb, isin);
			if (xi == null)
			{
				xs.SecurityId = isin;
				isinDict.Add(isin, xs);
				return xs;
			}
			if (xs == null)
			{
				xi.Symbol = symb;
				symbDict.Add(symb, xi);
				return xi;
			}
			if (xs != xi)
			{
				if (xi.Symbol == null) xi.Symbol = symb;
				if (xs.SecurityId == null) xs.SecurityId = isin;
				if ((xs.Symbol != xi.Symbol) || (xs.SecurityId != xi.SecurityId))
					throw new FixmlException(string.Format("Ambiguous symbol {0}: {1}, {2}", xi.SecurityId, xs.Symbol, symb));
			}
			return xi;
		}

		public static FixmlInstrument FindBySym(string symb)
		{
			if (symbDict.ContainsKey(symb))
				return symbDict[symb];
			else
				return new FixmlInstrument(symb, null);
		}

		public static FixmlInstrument FindById(string isin)
		{
			if (isinDict.ContainsKey(isin))
				return isinDict[isin];
			else
				return new FixmlInstrument(null, isin);
		}


		public override string ToString()
		{
			return Symbol ?? SecurityId;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			FixmlInstrument ins = (FixmlInstrument)obj;
			// porównujemy albo Symbol albo SecurityId (to drugie może być puste)
			return (Symbol == ins.Symbol) && (SecurityId == ins.SecurityId);
		}

		public static bool Compare(FixmlInstrument[] array1, FixmlInstrument[] array2)
		{
			if (array1 == array2) return true;
			if (array1 == null) return false;
			if (array2 == null) return false;
			if (array1.Length != array2.Length) return false;
			foreach (FixmlInstrument ins in array1)
				if (!array2.Contains(ins)) return false;
			return true;
		}


		#region FIXML read/write support

		private const char isinSrc = '4';

		public void Write(XmlDocument doc, XmlElement parent, string name)
		{
			XmlElement elem = doc.CreateElement("Instrmt");
			elem.SetAttribute("Sym", Symbol);
			if (SecurityId != null)
			{
				elem.SetAttribute("ID", SecurityId);
				elem.SetAttribute("Src", isinSrc.ToString());
			}
			parent.AppendChild(elem);
		}

		public static FixmlInstrument Read(XmlElement parent, string name)
		{
			XmlElement elem = parent.SelectSingleNode("Instrmt") as XmlElement;
			if (elem == null) return null;
			string sym = FixmlUtil.ReadString(elem, "Sym");
			string id = FixmlUtil.ReadString(elem, "ID");
			char src = FixmlUtil.ReadChar(elem, "Src");
			if (src != isinSrc)
				throw new FixmlException("Unsupported SecurityIdSource: " + src);
			return Find(sym, id);
		}

		#endregion


		#region Sym<=>ISIN dictionary persistance

		private const string dictFile = "instruments.txt";

		public static void DictionaryLoad()
		{
			symbDict.Clear();
			isinDict.Clear();
			if (!File.Exists(dictFile)) return;
			StreamReader stream = new StreamReader(dictFile);
			Regex regex = new Regex("^([A-Z0-9]+)\t([A-Z0-9]+)$");
			while (stream.Peek() >= 0)
			{
				Match match = regex.Match(stream.ReadLine());
				if (match.Success)
					new FixmlInstrument(match.Groups[2].Value, match.Groups[1].Value);
			}
			stream.Close();
		}

		public static void DictionarySave()
		{
			StreamWriter stream = new StreamWriter(dictFile);
			foreach (FixmlInstrument x in isinDict.Values)
				if (x.Symbol != null)
					stream.WriteLine(x.SecurityId + "\t" + x.Symbol);
			stream.Close();
		}

		#endregion

	}
}
