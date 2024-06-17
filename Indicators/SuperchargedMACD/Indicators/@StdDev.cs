//
// Copyright (C) 2024, NinjaTrader LLC <www.ninjatrader.com>.
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
	/// Standard Deviation is a statistical measure of volatility.
	/// Standard Deviation is typically used as a component of other indicators,
	/// rather than as a stand-alone indicator. For example, Bollinger Bands are
	/// calculated by adding a security's Standard Deviation to a moving average.
	/// </summary>
	public class StdDev : Indicator
	{
		private Series<double> sumSeries;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionStdDev;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameStdDev;
				IsOverlay					= false;
				IsSuspendedWhileInactive	= true;
				Period						= 14;

				AddPlot(Brushes.DarkCyan, NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameStdDev);
			}
			else if (State == State.DataLoaded)
			{
				sumSeries = new Series<double>(this, Period <= 256 ? MaximumBarsLookBack.TwoHundredFiftySix : MaximumBarsLookBack.Infinite);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1)
			{
				Value[0] = 0;
				sumSeries[0] = Input[0];
			}
			else
			{
				sumSeries[0] = Input[0] + sumSeries[1] - (CurrentBar >= Period ? Input[Period] : 0);
				double avg = sumSeries[0] / Math.Min(CurrentBar + 1, Period);
				double sum = 0;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack >= 0; barsBack--)
					sum += (Input[barsBack] - avg) * (Input[barsBack] - avg);

				Value[0] = Math.Sqrt(sum / Math.Min(CurrentBar + 1, Period));
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
		private StdDev[] cacheStdDev;
		public StdDev StdDev(int period)
		{
			return StdDev(Input, period);
		}

		public StdDev StdDev(ISeries<double> input, int period)
		{
			if (cacheStdDev != null)
				for (int idx = 0; idx < cacheStdDev.Length; idx++)
					if (cacheStdDev[idx] != null && cacheStdDev[idx].Period == period && cacheStdDev[idx].EqualsInput(input))
						return cacheStdDev[idx];
			return CacheIndicator<StdDev>(new StdDev(){ Period = period }, input, ref cacheStdDev);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.StdDev StdDev(int period)
		{
			return indicator.StdDev(Input, period);
		}

		public Indicators.StdDev StdDev(ISeries<double> input , int period)
		{
			return indicator.StdDev(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.StdDev StdDev(int period)
		{
			return indicator.StdDev(Input, period);
		}

		public Indicators.StdDev StdDev(ISeries<double> input , int period)
		{
			return indicator.StdDev(input, period);
		}
	}
}

#endregion
