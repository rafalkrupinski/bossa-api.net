using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Win32;
using pjank.BossaAPI.Fixml;
using pjank.BossaAPI.DTO;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Klasa obsługująca połączenie z aplikacją NOL3 
	/// - lokalna komunikacja z wykorzystaniem protokołu FIXML
	/// - nawiązywanie połączenia, logowanie, wylogowanie
	/// - odbiór komunikatów w kanale asynchronicznym
	/// - zbieranie bieżących notowań podczas sesji
	/// - TODO: składanie zleceń (na razie korzystać z klas Networking/Fixml/*)
	/// </summary>
	public class NolClient : IDisposable, IBosClient
	{

		public NolClient() : this(true) { }

		public NolClient(bool login) : this(login, login) { }

		public NolClient(bool login, bool thread)
		{
			FixmlInstrument.DictionaryLoad();
			if (login) Login();
			if (login && thread) ThreadStart();
		}


		#region IDisposable support

		private bool loggedIn = false;
		private Thread thread;

		~NolClient()
		{
			Dispose();
		}

		public void Dispose()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.Dispose...", FixmlMsg.DebugCategory);
			try
			{
				if (thread != null) ThreadStop();
				if (loggedIn) Logout();
				FixmlInstrument.DictionarySave();
			}
			catch (FixmlException e)
			{
				e.PrintError();
			}
			GC.SuppressFinalize(this);
		}

		#endregion


		#region NOL3 connection sockets

		private static int getRegistryPort(String name)
		{
			string regname = "Software\\COMARCH S.A.\\NOL3\\7\\Settings";
			RegistryKey regkey = Registry.CurrentUser.OpenSubKey(regname);
			Object value = regkey.GetValue("ncaset_" + name);
			if (value == null) throw new NolClientException("NOL3 registry settings missing!");
			if (value.ToString() == "0") throw new NolClientException("NOL3 port '" + name + "' not ready!");
			value = regkey.GetValue("nca_" + name);
			if (value == null) throw new NolClientException("NOL3 port '" + name + "' not defined!");
			return int.Parse(value.ToString());
		}

		public static Socket GetSyncSocket()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.GetSyncSocket...", FixmlMsg.DebugCategory);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect("localhost", getRegistryPort("psync"));
			socket.ReceiveTimeout = 10000;  // <- nie mniej, niż max. czas odpowiedzi na request synchr.
			socket.SendTimeout = 10000;
			return socket;
		}

		public static Socket GetAsyncSocket()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.GetAsyncSocket...", FixmlMsg.DebugCategory);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect("localhost", getRegistryPort("pasync"));
			socket.ReceiveTimeout = 10000;  // <- nie mniej, niż odstęp między Heartbeat (zwykle co 1sek?)
			socket.SendTimeout = 10000;
			return socket;
		}

		#endregion


		#region Asynchronous connection thread

		private void ThreadStart()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.ThreadStart...", FixmlMsg.DebugCategory);
			thread = new Thread(ThreadProc);
			thread.Name = "NOL3 async connection";
			thread.IsBackground = true;
			thread.Start();
		}

		private void ThreadStop()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.ThreadStop...", FixmlMsg.DebugCategory);
			thread.Abort();
			thread = null;
		}

		private void ThreadProc()
		{
			try
			{
				using (Socket socket = GetAsyncSocket())
				{
					while (socket.Connected)
					{
						Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "");
						FixmlMsg m = new FixmlMsg(socket);
						/* wersja dla .NET 4  +  "using Microsoft.CSharp;"
						dynamic msg = FixReceivedMessageType(m);
						HandleAsyncMessage(msg);
						*/
						FixmlMsg msg = FixReceivedMessageType(m);
						if (AsyncMessageHandler != null) AsyncMessageHandler(msg);
						GetType().InvokeMember("HandleAsyncMessage",
							System.Reflection.BindingFlags.InvokeMethod |
							System.Reflection.BindingFlags.Instance |
							System.Reflection.BindingFlags.NonPublic,
							null, this, new[] { msg });
					}
				}
			}
			//catch (FixmlSocketException e) { socket.Close(); e.PrintError(); }
			catch (ThreadAbortException) { /*socket.Close();*/ }
			catch (Exception e) { e.PrintError(); }
		}

		#endregion


		public event Action<FixmlMsg> AsyncMessageHandler;
		public event Action<AppMessageReportMsg> AsyncAppReportHandler;
		public event Action<ExecutionReportMsg> AsyncExecReportHandler;
		public event Action<MarketDataIncRefreshMsg> AsyncMarketDataHandler;
		public event Action<TradingSessionStatusMsg> AsyncSessStatusHandler;
		public event Action<NewsMsg> AsyncNewsHandler;
		public event Action<StatementMsg> AsyncStatementHandler;

		public event Action<Account> AccountUpdateHandler;


		#region Asynchronous messages handling

		private FixmlMsg FixReceivedMessageType(FixmlMsg msg)
		{
			if (msg.GetType() != typeof(FixmlMsg))
				throw new ArgumentException("Recurrency warning - base 'FixmlMsg' class object required", "msg");
			switch (msg.Xml.Name)
			{
				case AppMessageReportMsg.MsgName: return new AppMessageReportMsg(msg);
				case ExecutionReportMsg.MsgName: return new ExecutionReportMsg(msg);
				case MarketDataIncRefreshMsg.MsgName: return new MarketDataIncRefreshMsg(msg);
				case TradingSessionStatusMsg.MsgName: return new TradingSessionStatusMsg(msg);
				case NewsMsg.MsgName: return new NewsMsg(msg);
				case StatementMsg.MsgName: return new StatementMsg(msg);
				case UserResponseMsg.MsgName: return new UserResponseMsg(msg);
				case "Heartbeat": return msg;
				default:
					string txt = string.Format("Unexpected async message '{0}'", msg.Xml.Name);
					MyUtil.PrintWarning(txt);
					if (!FixmlMsg.DebugOriginalXml.Enabled && !FixmlMsg.DebugFormattedXml.Enabled)
						Trace.WriteLine(string.Format("'{0}'", MyUtil.FormattedXml(msg.Xml.OwnerDocument)));
					return msg;
			}
		}


		private void HandleAsyncMessage(AppMessageReportMsg msg)
		{
			if (AsyncAppReportHandler != null) AsyncAppReportHandler(msg);
		}

		private void HandleAsyncMessage(ExecutionReportMsg msg)
		{
			if (AsyncExecReportHandler != null) AsyncExecReportHandler(msg);
		}

		private void HandleAsyncMessage(MarketDataIncRefreshMsg msg)
		{
			// blokada/synchronizacja tutaj głównie po to, by opóźnić obsługę tego komunikatu 
			// aż zakończymy metodę MarketDataStart(), gdzie m.in. dopiero jest ustawiany nowy mdReqId
			// (asynchroniczne MktDataInc zaczynają tu przychodzić jeszcze zanim tam odbierzemy MktDataFull)
			lock (mdResults)
			{
				// bezpośrednio po zmianie subskrypcji (co jest w innym wątku) może nam tu jeszcze dotrzeć
				// komunikat z poprzednim Id - ignorujemy, by nie powstały duplikaty w historii transakcji
				if (msg.RequestId != mdReqId) return;
				foreach (MDEntry entry in msg.Entries)
					NewMarketDataEntry(entry);
			}
			if (AsyncMarketDataHandler != null) AsyncMarketDataHandler(msg);
		}

		private void HandleAsyncMessage(TradingSessionStatusMsg msg)
		{
			if (AsyncSessStatusHandler != null) AsyncSessStatusHandler(msg);
		}

		private void HandleAsyncMessage(NewsMsg msg)
		{
			if (AsyncNewsHandler != null) AsyncNewsHandler(msg);
		}

		private void HandleAsyncMessage(StatementMsg msg)
		{
			if (AsyncStatementHandler != null) AsyncStatementHandler(msg);
			if (AccountUpdateHandler != null)
				foreach (var statement in msg.Statements)
				{
					var account = new Account();
					account.Number = statement.Account;
					account.Papers = statement.Positions.
						Select(p => new Paper {
							Instrument = new Instrument { 
								Symbol = p.Key.Symbol, ISIN = p.Key.SecurityId },
							Quantity = p.Value
						}).ToArray();
					account.Cash = statement.Funds[StatementFundType.Cash];
					AccountUpdateHandler(account);
				}
		}

		private void HandleAsyncMessage(UserResponseMsg msg)
		{
			// (msg.Status == UserStatus.Other)
			// msg.StatusText:
			// 1 - zamkniecie aplikacji NOL3 
			// 2 - NOL3 jest offline 
			// 3 - NOL3 jest online 
			// 4 - aplikacja NOL3 nie jest uruchomiona
			Trace.WriteLine(" connection terminated ?! ");
			ThreadStop();
		}

		private void HandleAsyncMessage(FixmlMsg msg)
		{
			//Trace.WriteLine("-fixml-");
		}

		#endregion


		#region User login, logout

		public void Login()
		{
			Debug.WriteLine("\nLogin...");
		StartLogin:
			using (Socket socket = GetSyncSocket())
			{
				UserRequestMsg request = new UserRequestMsg();
				request.Type = UserRequestType.Login;
				request.Username = "BOS";
				request.Password = "BOS";
				request.Send(socket);
				try
				{
					UserResponseMsg response = new UserResponseMsg(socket);
					if ((response.Status == UserStatus.Other) && (response.StatusText == "User is already logged"))
						MyUtil.PrintWarning("NOL says: We're already logged in !?");
					else
						if (response.Status != UserStatus.LoggedIn)
							throw new FixmlErrorMsgException(response);
				}
				catch (BizMessageRejectException e)
				{
					// całe to przechwytywanie wyjątków i powtórki możnaby pominąć, gdyby NOL nie blokował 
					// numerku ReqID, jeśli jego poprzedni klient nie zrealizował prawidłowo logowania/wylogowania
					if (e.Msg.RejectText == "Incorrect UserReqID")
						if (request.Id < 100) goto StartLogin;  // każdy kolejny UserRequestMsg z większym "Id"
						else throw new FixmlException("UserReqID limit reached!");
					else throw;
				}
			}
			loggedIn = true;
			Debug.WriteLine("Login OK\n");
		}

		public void Logout()
		{
			Debug.WriteLine("\nLogout...");
			loggedIn = false;  // <- nawet, jeśli niżej będzie błąd (żeby się nie powtarzał w destruktorze)
			using (Socket socket = GetSyncSocket())
			{
				UserRequestMsg request = new UserRequestMsg();
				request.Type = UserRequestType.Logout;
				request.Username = "BOS";
				request.Send(socket);
				UserResponseMsg response = new UserResponseMsg(socket);
				if (response.Status != UserStatus.LoggedOut)
					throw new FixmlErrorMsgException(response);
			}
			Debug.WriteLine("Logout OK\n");
		}

		#endregion


		#region MarketData subscription

		private uint? mdReqId = null;
		private HashSet<MDEntryType> mdEntryTypes = new HashSet<MDEntryType>();
		private HashSet<FixmlInstrument> mdInstruments = new HashSet<FixmlInstrument>();
		private Dictionary<FixmlInstrument, MDResults> mdResults = new Dictionary<FixmlInstrument, MDResults>();

		public void MarketDataStart()
		{
			if (mdReqId != null) MarketDataStop();
			lock (mdResults)
			{
				mdResults.Clear();
				Debug.WriteLine("\nMarketDataStart...");
				using (Socket socket = GetSyncSocket())
				{
					MarketDataRequestMsg request = new MarketDataRequestMsg();
					request.Type = SubscriptionRequestType.StartSubscription;
					request.MarketDepth = 1;
					request.EntryTypes = mdEntryTypes.ToArray();
					request.Instruments = mdInstruments.ToArray();
					request.Send(socket);
					MarketDataFullRefreshMsg response = new MarketDataFullRefreshMsg(socket);
					if (response.RequestId != request.Id)
						throw new FixmlException("Unexpected MktDataFull ReqID.");
					mdReqId = request.Id;
				}
				Debug.WriteLine("MarketDataStart OK\n");
			}
		}

		public void MarketDataStop()
		{
			if (mdReqId == null) return;
			lock (mdResults)
			{
				Debug.WriteLine("\nMarketDataStop...");
				using (Socket socket = GetSyncSocket())
				{
					MarketDataRequestMsg request = new MarketDataRequestMsg();
					request.Type = SubscriptionRequestType.CancelSubscription;
					request.Send(socket);
					MarketDataFullRefreshMsg response = new MarketDataFullRefreshMsg(socket);
					if (response.RequestId != request.Id)
						throw new FixmlException("Unexpected MktDataFull ReqID.");
				}
				mdReqId = null;
				Debug.WriteLine("MarketDataStop OK\n");
			}
		}

		private void NewMarketDataEntry(MDEntry entry)
		{
			MarketDataResults(entry.Instrument).AddEntry(entry);
		}

		public IEnumerable<KeyValuePair<FixmlInstrument, MDResults>> MarketDataResults()
		{
			return mdResults;
		}

		public MDResults MarketDataResults(FixmlInstrument instrument)
		{
			if (!mdResults.ContainsKey(instrument))
				mdResults.Add(instrument, new MDResults());
			return mdResults[instrument];
		}

		public MDResults MarketDataResults(string symbol)
		{
			return MarketDataResults(FixmlInstrument.FindBySym(symbol));
		}

		private void MarketDataSubscriptionChange(IEnumerable<MDEntryType> types, IEnumerable<FixmlInstrument> instr)
		{
			bool wasActive = (mdReqId != null);
			if (wasActive) MarketDataStop();
			mdEntryTypes = new HashSet<MDEntryType>(types);
			mdInstruments = new HashSet<FixmlInstrument>(instr);
			if (wasActive && types.Any() && instr.Any()) MarketDataStart();
		}

		public void MarketDataSubscriptionClear()
		{
			MarketDataStop();
			mdEntryTypes.Clear();
			mdInstruments.Clear();
		}

		public void MarketDataSubscriptionAdd(params MDEntryType[] types)
		{
			MarketDataSubscriptionChange(mdEntryTypes.Union(types), mdInstruments);
		}

		public void MarketDataSubscriptionRemove(params MDEntryType[] types)
		{
			MarketDataSubscriptionChange(mdEntryTypes.Except(types), mdInstruments);
		}

		public void MarketDataSubscriptionAdd(params FixmlInstrument[] instruments)
		{
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Union(instruments));
		}

		public void MarketDataSubscriptionRemove(params FixmlInstrument[] instruments)
		{
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Except(instruments));
		}

		public void MarketDataSubscriptionAdd(params string[] symbols)
		{
			IEnumerable<FixmlInstrument> instruments = symbols.Select(sym => FixmlInstrument.FindBySym(sym));
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Union(instruments));
		}

		public void MarketDataSubscriptionRemove(params string[] symbols)
		{
			IEnumerable<FixmlInstrument> instruments = symbols.Select(sym => FixmlInstrument.FindBySym(sym));
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Except(instruments));
		}

		#endregion


		#region TradingSessionStatus subscription

		public void TradingSessionStatusStart()
		{
			Debug.WriteLine("\nTradingSessionStatusStart...");
			using (Socket socket = GetSyncSocket())
			{
				TrdSesStatusRequestMsg request = new TrdSesStatusRequestMsg();
				request.Type = SubscriptionRequestType.StartSubscription;
				request.Send(socket);
				TradingSessionStatusMsg response = new TradingSessionStatusMsg(socket);
				if (response.RequestId != request.Id)
					throw new FixmlException("Unexpected TrdgSesStat ReqID.");
				// mamy taki status - ale czy to tego requesta dotyczy, to ja tylko zgadywać mogę... :/
				if (response.SessionStatus == TradingSessionStatus.RequestRejected)
					throw new FixmlException("TrdgSesStat rejected, reason = " + response.RejectReason);
			}
			Debug.WriteLine("TradingSessionStatusStart OK\n");
		}

		public void TradingSessionStatusStop()
		{
			Debug.WriteLine("\nTradingSessionStatusStop...");
			using (Socket socket = GetSyncSocket())
			{
				TrdSesStatusRequestMsg request = new TrdSesStatusRequestMsg();
				request.Type = SubscriptionRequestType.CancelSubscription;
				request.Send(socket);
				TradingSessionStatusMsg response = new TradingSessionStatusMsg(socket);
				if (response.RequestId != request.Id)
					throw new FixmlException("Unexpected TrdgSesStat ReqID.");
				// mamy taki status - ale czy to tego requesta dotyczy, to ja tylko zgadywać mogę... :/
				if (response.SessionStatus == TradingSessionStatus.RequestRejected)
					throw new FixmlException("TrdgSesStat rejected, reason = " + response.RejectReason);
			}
			Debug.WriteLine("TradingSessionStatusStop OK\n");
		}

		#endregion


	}
}
