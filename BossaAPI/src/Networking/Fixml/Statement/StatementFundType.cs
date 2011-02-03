using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum StatementFundType
    {
        Cash,                   // gotówka do dyspozycji
        CashBlocked,            // gotówka blokowana pod zlecenia 
        CashRecivables,         // należności niezablokowane, do dyspozycji
        Liabilities,            // ? zobowiązania
        LiabilitiesLimitMax,    // kwota, o jaką można zwiększać limit należności
        MaxBuy,
        MaxOtpBuy,              // maksymalne kupno z odroczonym term. płatności
        Recivables,             // ? należności
        RecivablesBlocked,
        SecuritiesValue,        // wartość papierów / wartość teoretyczna otwartych pozycji
        SecSafeties,
        SecSafetiesUsed,
        PortfolioValue,         // wartość całego portfela / wartość środków własnych
        Deposit,
        BlockedDeposit,
        FreeDeposit,
        DepositDeficit,
    }

    internal class StatementFundUtil
    {
        private static Dictionary<string, StatementFundType> dict =
            new Dictionary<string, StatementFundType> {
                { "Cash",                   StatementFundType.Cash },
                { "CashBlocked",            StatementFundType.CashBlocked },
                { "CashRecivables",         StatementFundType.CashRecivables },
                { "Liabilities",            StatementFundType.Liabilities },
                { "LiabilitiesLimitMax",    StatementFundType.LiabilitiesLimitMax },
                { "MaxBuy",                 StatementFundType.MaxBuy },
                { "MaxOtpBuy",              StatementFundType.MaxOtpBuy },
                { "Recivables",             StatementFundType.Recivables },
                { "RecivablesBlocked",      StatementFundType.RecivablesBlocked },
                { "SecValueSum",            StatementFundType.SecuritiesValue },
                { "SecSafeties",            StatementFundType.SecSafeties },
                { "SecSafetiesUsed",        StatementFundType.SecSafetiesUsed },
                { "PortfolioValue",         StatementFundType.PortfolioValue },
                { "Deposit",                StatementFundType.Deposit },
                { "BlockedDeposit",         StatementFundType.BlockedDeposit },
                { "FreeDeposit",            StatementFundType.FreeDeposit },
                { "DepositDeficit",         StatementFundType.DepositDeficit },
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
