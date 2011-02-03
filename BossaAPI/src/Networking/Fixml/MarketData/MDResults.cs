using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.Fixml
{
    /// <summary>
    /// Klasa używana na razie przy zbieraniu historii notowań w klasie NolClient.
    /// Ostatecznie będzie prawdopodobnie zastąpiona przez src/MarketData/BosTrades.
    /// </summary>
    public class MDResults
    {
        public class Trade
        {
            public readonly DateTime Time;
            public readonly decimal Price;
            public readonly uint Volume;
            public uint? Lop { get; internal set; }
            internal Trade(DateTime time, decimal price, uint volume, uint? lop)
            {
                this.Time = time;
                this.Price = price;
                this.Volume = volume;
                this.Lop = lop;
            }
            public override string ToString()
            {
                return string.Format("{2}  {0,7} x {1,-5}  {3,8:(0)}", Price, Volume, Time.TimeOfDay, Lop);
            }
        }
        public class Offer
        {
            public readonly decimal? Price;
            public readonly string PriceStr;
            public readonly uint Volume;
            public readonly uint Count;
            internal Offer(decimal? price, string pricestr, uint volume, uint count)
            {
                this.Price = price;
                this.PriceStr = pricestr;
                this.Volume = volume;
                this.Count = count;
            }
            public override string ToString()
            {
                return string.Format("{0,7} x {1,-5} ({2})", PriceStr ?? Price.ToString(), Volume, Count);
            }
        }

        private readonly List<Offer> bids;
        private readonly List<Offer> offers;
        private readonly List<Trade> trades;
        public readonly ReadOnlyCollection<Offer> Bids;
        public readonly ReadOnlyCollection<Offer> Offers;
        public readonly ReadOnlyCollection<Trade> Trades;
        public decimal? OpenPrice { get; private set; }
        public decimal? HighPrice { get; private set; }
        public decimal? LowPrice { get; private set; }
        public decimal? ClosePrice { get; private set; }
        public decimal? RefPrice { get; private set; }
        public decimal? OpenTurnover { get; private set; }
        public decimal? CloseTurnover { get; private set; }
        public decimal? SessionTurnover { get; private set; }
        public uint? SessionVolume { get; private set; }
        public uint? Lop { get; private set; }

        public decimal? CurrentPrice { get { return trades.Count > 0 ? trades.Last().Price : (decimal?)null; } }

        public MDResults()
        {
            bids = new List<Offer>();
            offers = new List<Offer>();
            trades = new List<Trade>();
            Bids = bids.AsReadOnly();
            Offers = offers.AsReadOnly();
            Trades = trades.AsReadOnly();
        }

        public void AddEntry(MDEntry entry)
        {
            switch (entry.EntryType)
            {
                case MDEntryType.Buy:
                    AddOffer(bids, entry);
                    break;
                case MDEntryType.Sell:
                    AddOffer(offers, entry);
                    break;
                case MDEntryType.Trade:
                    AddTrade(entry);
                    break;
                case MDEntryType.Lop:
                    Lop = entry.Size;
                    if (trades.Count() > 0)
                        trades.Last().Lop = Lop;
                    break;
                case MDEntryType.Vol:
                    SessionVolume = entry.Size;
                    SessionTurnover = entry.Turnover;
                    break;
                case MDEntryType.Index:
                    SessionTurnover = entry.Turnover;
                    break;
                case MDEntryType.Open:
                    OpenPrice = entry.Price;
                    OpenTurnover = entry.Turnover;
                    break;
                case MDEntryType.Close:
                    ClosePrice = entry.Price;
                    CloseTurnover = entry.Turnover;
                    break;
                case MDEntryType.High:
                    HighPrice = entry.Price;
                    break;
                case MDEntryType.Low:
                    LowPrice = entry.Price;
                    break;
                case MDEntryType.Ref:
                    RefPrice = entry.Price;
                    break;
            }
        }

        private void AddTrade(MDEntry entry)
        {
            Trade trade = new Trade(entry.DateTime, (decimal)entry.Price, entry.Size ?? 0, this.Lop);
            trades.Add(trade);
        }

        private void AddOffer(List<Offer> list, MDEntry entry)
        {
            int level = (int)entry.Level - 1;
            Offer offer = new Offer(entry.Price, entry.PriceStr, (uint)entry.Size, (uint)entry.Orders);
            if (list.Count > 0) list[0] = offer;
            else list.Add(offer);
            //switch (entry.UpdateAction) //TODO: UpdateAction - na razie nie jest przesyłane
            //{
            //    case MDUpdateAction.New: list.Insert(level, offer); break;
            //    case MDUpdateAction.Change: list[level] = offer; break;
            //    case MDUpdateAction.Delete: list.RemoveAt(level); break;
            //}
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("O = {0,7}  H = {1,7}  L = {2,7}  C = {3,7}   {4:V = 0} {5:[.0mln]}",
                OpenPrice, HighPrice, LowPrice, ClosePrice, SessionVolume, SessionTurnover / 1000000));
            for (int i = bids.Count - 1; i >= 0; i--) sb.Append("\n - Buy   " + bids[i]);
            for (int i = 0; i < offers.Count; i++) sb.Append("\n - Sell  " + offers[i]);
            for (int i = trades.Count - 1; i >= 0; i--) sb.Append("\n " + trades[i]);
            return sb.ToString();
        }

    }
}
