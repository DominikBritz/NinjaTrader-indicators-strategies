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
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder
	/// in his book 'New Concepts in Technical Trading Systems' and has since been used as a component
	/// of many indicators and trading systems.
	/// </summary>
	public class ATR : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionATR;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameATR;
				IsSuspendedWhileInactive	= true;
				Period						= 14;

				AddPlot(Brushes.DarkCyan, NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameATR);
			}
		}

		protected override void OnBarUpdate()
		{
			double high0	= High[0];
			double low0		= Low[0];

			if (CurrentBar == 0)
				Value[0] = high0 - low0;
			else
			{
				double close1		= Close[1];
				double trueRange	= Math.Max(Math.Abs(low0 - close1), Math.Max(high0 - low0, Math.Abs(high0 - close1)));
				Value[0]			= ((Math.Min(CurrentBar + 1, Period) - 1 ) * Value[1] + trueRange) / Math.Min(CurrentBar + 1, Period);
			}
		}

		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ATR[] cacheATR;
		public ATR ATR(int period)
		{
			return ATR(Input, period);
		}

		public ATR ATR(ISeries<double> input, int period)
		{
			if (cacheATR != null)
				for (int idx = 0; idx < cacheATR.Length; idx++)
					if (cacheATR[idx] != null && cacheATR[idx].Period == period && cacheATR[idx].EqualsInput(input))
						return cacheATR[idx];
			return CacheIndicator<ATR>(new ATR(){ Period = period }, input, ref cacheATR);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ATR ATR(int period)
		{
			return indicator.ATR(Input, period);
		}

		public Indicators.ATR ATR(ISeries<double> input , int period)
		{
			return indicator.ATR(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ATR ATR(int period)
		{
			return indicator.ATR(Input, period);
		}

		public Indicators.ATR ATR(ISeries<double> input , int period)
		{
			return indicator.ATR(input, period);
		}
	}
}

#endregion
