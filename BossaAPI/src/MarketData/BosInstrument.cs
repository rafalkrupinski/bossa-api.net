using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Identyfikuje intrument (papier wartościowy), którego dotyczy jakaś oferta, zlecenie, transakcja.
	/// </summary>
	public class BosInstrument
	{
		/// <summary>
		/// Rodzaj instrumentu (akcje, kontrakty, indeks).
		/// </summary>
		public BosInstrumentType Type { get; private set; }

		/// <summary>
		/// Unikalny kod ISIN tego instrumentu.
		/// Może być null, dopóki nie uda się skojarzyć ze sobą pary: Symbol-ISIN.
		/// </summary>
		public string ISIN { get; private set; }

		/// <summary>
		/// Stosowany normalnie symbol instrumentu - jego pełna wersja.
		/// Może występować kilka razy, z różnymi kodami ISIN (kolejne emisje akcji).
		/// Może też być null, dopóki nie uda się skojarzyć ze sobą pary: Symbol-ISIN.
		/// </summary>
		public string Symbol { get; private set; }

		/// <summary>
		/// TODO: Wersja skrócona (3-znakowa) stosowanego normalnie symbolu instrumentu.
		/// </summary>
		public string Sym { get; private set; }

		/// <summary>
		/// TODO: Lista widocznych w arkuszu ofert kupna dla tego instrumentu.
		/// </summary>
		public BosOffers BuyOffers { get; private set; }

		/// <summary>
		/// TODO: Lista widocznych w arkuszu ofert sprzedaży dla tego instrumentu.
		/// </summary>
		public BosOffers SellOffers { get; private set; }

		/// <summary>
		/// TODO: Zebrana do tej pory historia transakcji dla tego instrumentu.
		/// </summary>
		public BosTrades Trades { get; private set; }


		#region Internal library stuff

		// konstruktor wywoływany z klasy BosInstruments
		internal BosInstrument(string isin, string symbol)
		{
			ISIN = isin;
			Symbol = symbol;
			Sym = GetInstrumentSym();
			Type = GetInstrumentType();
			//TODO: BuyOffers, SellOffers, Trades
		}

		// dołącza dane z innego obiektu tego samego typu, wywoływane z klasy BosInstruments
		internal void Combine(BosInstrument source)
		{
			ISIN = ISIN ?? source.ISIN;
			Symbol = Symbol ?? source.Symbol;
			Sym = GetInstrumentSym();
			Type = GetInstrumentType();
			//TODO: BuyOffers, SellOffers, Trades
		}

		// konwersja obiektu transportowego na instancję tej klasy (nową lub już istniejącą)
		internal static BosInstrument Create(DTO.Instrument data)
		{
			return Bossa.Instruments.Find(data.ISIN, data.Symbol);
		}

		// konwersja tego obiektu na obiekt transportowy
		internal DTO.Instrument Convert()
		{
			return new DTO.Instrument { Symbol = Symbol, ISIN = ISIN };
		}

		#endregion

		/// <summary>
		/// Standardowa konwersja na stringa.
		/// Zwraca Symbol albo kod ISIN, jeśli nie znamy tego pierwszego.
		/// </summary>
		public override string ToString()
		{
			return Symbol ?? ISIN;
		}

		// ustalenie skróconego symbolu instrumentu
		private string GetInstrumentSym()
		{
			return null;
		}

		// ustalenie typu instrumentu
		private BosInstrumentType GetInstrumentType()
		{
			// TODO: Na pewno są gdzieś jakieś oficjalne zasady podziału tych numerów!?
			// Na razie to czysta zgadywanka z mojej strony, więc jeśli źle działa - daj znać!
			if (ISIN != null)
			{
				if (ISIN.StartsWith("PL99")) return BosInstrumentType.Index;
				if (ISIN.StartsWith("PL0G")) return BosInstrumentType.Futures;
				return BosInstrumentType.Default;
			}
			if (Symbol != null)
			{
				// szczególnie to poniżej to już kompletny HACK ;-)
				if (Symbol.StartsWith("FW")) return BosInstrumentType.Futures;
				if (Symbol.StartsWith("OW")) return BosInstrumentType.Futures;
				if (Symbol.Contains("WIG")) return BosInstrumentType.Index;
			}
			return BosInstrumentType.Default;
		}
	}
}
