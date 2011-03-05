using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum StatementFundType
	{
		Cash,					// gotówka do dyspozycji (konto zwykłe)
		CashBlocked,			// gotówka blokowana pod zlecenia
		CashReceivables,		// razem do dyspozycji (wolna gotówka + należności)
		Receivables,			// należności odblokowane, do dyspozycji
		ReceivablesBlocked,		// należności blokowane pod zlecenia
		SecuritiesValue,		// wartość papierów / wartość teoretyczna otwartych pozycji
		PortfolioValue,			// wartość całego portfela / wartość środków własnych
		Deposit,				// depozyt - razem
		DepositBlocked,			// depozyt - zablokowany
		DepositFree,			// depozyt - do dyspozycji
		DepositDeficit,			// depozyt - do uzupełnienia
		DepositSurplus,			// depozyt - nadwyżka (ponad minimalny depozyt)
		SecSafeties,			// zabezpieczenie pap.wart. - ogółem
		SecSafetiesUsed,		// zabezpieczenie pap.wart. - wykorzystane
		Loans,					// kredyty - aktualne zaangażowanie
		Liabilities,			// zobowiązania wobec DM BOŚ - łączne (P+T)
		LiabilitiesP,			// zobow. wobec DM BOŚ - typu P (zabezpieczone)
		LiabilitiesT,			// zobow. wobec DM BOŚ - typu T (niezabezpieczone)
		LiabilitiesLimitMax,	// kwota, o jaką można zwiększyć limit należności
		LiabilitiesPLimitMax,	// o ile można zwiększyć limit nal. typu P (zabezp.)
		LiabilitiesTLimitMax,	// o ile można zwiększyć limit nal. typu T (niezabezp.)
		MaxBuy,					// maksymalne kupno
		MaxOtpBuy,				// maksymalne kupno na Odroczony Termin Płatności
		MaxOtpPBuy,				// maksymalne kupno na OTP typu P (zabezpieczone)
		MaxOtpTBuy,				// maksymalne kupno na OTP typu T (niezabezpieczone)
	}

	internal class StatementFundUtil
	{
		private static Dictionary<string, StatementFundType> dict =
			new Dictionary<string, StatementFundType> {
				{ "Cash",					StatementFundType.Cash },
				{ "Recivables",				StatementFundType.Receivables },
				{ "CashRecivables",			StatementFundType.CashReceivables },
				{ "CashBlocked",			StatementFundType.CashBlocked },
				{ "RecivablesBlocked",		StatementFundType.ReceivablesBlocked },
				{ "Loans",					StatementFundType.Loans },
				{ "Liabilities",			StatementFundType.Liabilities },
				{ "LiabilitiesP",			StatementFundType.LiabilitiesP },
				{ "LiabilitiesT",			StatementFundType.LiabilitiesT },
				{ "MaxBuy",					StatementFundType.MaxBuy },
				{ "MaxOtpBuy",				StatementFundType.MaxOtpBuy },
				{ "MaxOtpPBuy",				StatementFundType.MaxOtpPBuy },
				{ "MaxOtpTBuy",				StatementFundType.MaxOtpTBuy },
				{ "LiabilitiesLimitMax",	StatementFundType.LiabilitiesLimitMax },
				{ "LiabilitiesPLimitMax",	StatementFundType.LiabilitiesPLimitMax },
				{ "LiabilitiesTLimitMax",	StatementFundType.LiabilitiesTLimitMax },
				{ "Deposit",				StatementFundType.Deposit },
				{ "BlockedDeposit",			StatementFundType.DepositBlocked },
				{ "SecSafeties",			StatementFundType.SecSafeties },
				{ "SecSafetiesUsed",		StatementFundType.SecSafetiesUsed },
				{ "FreeDeposit",			StatementFundType.DepositFree },
				{ "DepositSurplus",			StatementFundType.DepositSurplus },
				{ "DepositDeficit",			StatementFundType.DepositDeficit },
				{ "SecValueSum",			StatementFundType.SecuritiesValue },
				{ "PortfolioValue",			StatementFundType.PortfolioValue },
			};
		public static StatementFundType Read(XmlElement xml, string name)
		{
			string str = FixmlUtil.ReadString(xml, name, true);
			if (!dict.ContainsKey(str))
				throw new FixmlException(string.Format("Unknown StatementFundType: '{0}'", str));
			return dict[str];
		}
	}
}
