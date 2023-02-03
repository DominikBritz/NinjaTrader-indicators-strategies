//
// Copyright (C) 2022, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class BarTimer : Indicator
	{
		private string			timeLeft	= string.Empty;
		private DateTime		now		 	= Core.Globals.Now;
		private bool			connected,
								hasRealtimeData;
		private SessionIterator sessionIterator;

		private System.Windows.Threading.DispatcherTimer timer;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description 		= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionBarTimer;
				Name 				= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameBarTimer;
				Calculate			= Calculate.OnEachTick;
				DrawOnPricePanel	= false;
				IsChartOnly			= true;
				IsOverlay			= true;
				DisplayInDataBox	= false;
			}
			else if (State == State.Realtime)
			{
				if (timer == null && IsVisible)
				{
					if (Bars.BarsType.IsTimeBased && Bars.BarsType.IsIntraday)
					{
						lock (Connection.Connections)
						{
							if (Connection.Connections.ToList().FirstOrDefault(c => c.Status == ConnectionStatus.Connected && c.InstrumentTypes.Contains(Instrument.MasterInstrument.InstrumentType)) == null)
								Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerDisconnectedError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
							else
							{
								if (!SessionIterator.IsInSession(Now, false, true))
									Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerSessionTimeError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
								else
									Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerWaitingOnDataError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
							}
						}
					}
					else
						Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerTimeBasedError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
				}
			}
			else if (State == State.Terminated)
			{
				if (timer == null)
					return;

				timer.IsEnabled = false;
				timer = null;
			}
		}

		protected override void OnBarUpdate()
		{
			if (State == State.Realtime)
			{
				hasRealtimeData = true;
				connected = true;
			}
		}

		protected override void OnConnectionStatusUpdate(ConnectionStatusEventArgs connectionStatusUpdate)
		{
			if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Connected
				&& connectionStatusUpdate.Connection.InstrumentTypes.Contains(Instrument.MasterInstrument.InstrumentType)
				&& Bars.BarsType.IsTimeBased
				&& Bars.BarsType.IsIntraday)
			{
				connected = true;

				if (DisplayTime() && timer == null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						timer			= new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 1), IsEnabled = true };
						timer.Tick		+= OnTimerTick;
					});
				}
			}
			else if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Disconnected)
				connected = false;
		}

		private bool DisplayTime()
		{
			return ChartControl != null
					&& Bars != null
					&& Bars.Instrument.MarketData != null
					&& IsVisible;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			ForceRefresh();

			if (DisplayTime())
			{
				if (timer != null && !timer.IsEnabled)
					timer.IsEnabled = true;

				if (connected)
				{
					if (SessionIterator.IsInSession(Now, false, true))
					{
						if (hasRealtimeData)
						{
							TimeSpan barTimeLeft = Bars.GetTime(Bars.Count - 1).Subtract(Now);

							timeLeft = (barTimeLeft.Ticks < 0
								? "00:00:00"
								: barTimeLeft.Hours.ToString("00") + ":" + barTimeLeft.Minutes.ToString("00") + ":" + barTimeLeft.Seconds.ToString("00"));

							Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerTimeRemaining + timeLeft, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
						}
						else
							Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerWaitingOnDataError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
					}
					else
						Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerSessionTimeError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);
				}
				else
				{
					Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerDisconnectedError, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);

					if (timer != null)
						timer.IsEnabled = false;
				}
			}
		}

		private SessionIterator SessionIterator
		{
			get
			{
				if (sessionIterator == null)
					sessionIterator = new SessionIterator(Bars);
				return sessionIterator;
			}
		}

		private DateTime Now
		{
			get
			{
				now = (Cbi.Connection.PlaybackConnection != null ? Cbi.Connection.PlaybackConnection.Now : Core.Globals.Now);

				if (now.Millisecond > 0)
					now = Core.Globals.MinDate.AddSeconds((long)Math.Floor(now.Subtract(Core.Globals.MinDate).TotalSeconds));

				return now;
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private BarTimer[] cacheBarTimer;
		public BarTimer BarTimer()
		{
			return BarTimer(Input);
		}

		public BarTimer BarTimer(ISeries<double> input)
		{
			if (cacheBarTimer != null)
				for (int idx = 0; idx < cacheBarTimer.Length; idx++)
					if (cacheBarTimer[idx] != null &&  cacheBarTimer[idx].EqualsInput(input))
						return cacheBarTimer[idx];
			return CacheIndicator<BarTimer>(new BarTimer(), input, ref cacheBarTimer);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.BarTimer BarTimer()
		{
			return indicator.BarTimer(Input);
		}

		public Indicators.BarTimer BarTimer(ISeries<double> input )
		{
			return indicator.BarTimer(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.BarTimer BarTimer()
		{
			return indicator.BarTimer(Input);
		}

		public Indicators.BarTimer BarTimer(ISeries<double> input )
		{
			return indicator.BarTimer(input);
		}
	}
}

#endregion
