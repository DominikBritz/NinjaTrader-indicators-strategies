//
// Copyright (C) 2023, NinjaTrader LLC <www.ninjatrader.com>.
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
	/// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average
	/// value of a security's price over a period of time with special emphasis on the more recent
	/// portions of the time period under analysis as opposed to the earlier.
	/// </summary>
	public class WMA : Indicator
	{
		private	int		myPeriod;
		private double	priorSum;
		private double	priorWsum;
		private double	sum;
		private double	wsum;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{

				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionWMA;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameWMA;
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;
				Period						= 14;

				AddPlot(Brushes.Goldenrod, NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameWMA);
			}
			else if (State == State.Configure)
			{
				priorSum	= 0;
				priorWsum	= 0;
				sum			= 0;
				wsum		= 0;
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsArray[0].BarsType.IsRemoveLastBarSupported)
			{
				if (CurrentBar == 0)
					Value[0] = Input[0];
				else
				{
					int back = Math.Min(Period - 1, CurrentBar);
					double val = 0;
					int weight = 0;
					for (int idx = back; idx >= 0; idx--)
					{
						val += (idx + 1) * Input[back - idx];
						weight += (idx + 1);
					}
					Value[0] = val / weight;
				}
			}
			else
			{
				if (IsFirstTickOfBar)
				{
					priorWsum = wsum;
					priorSum = sum;
					myPeriod = Math.Min(CurrentBar + 1, Period);
				}

				wsum = priorWsum - (CurrentBar >= Period ? priorSum : 0) + myPeriod * Input[0];
				sum = priorSum + Input[0] - (CurrentBar >= Period ? Input[Period] : 0);
				Value[0] = wsum / (0.5 * myPeriod * (myPeriod + 1));
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
		private WMA[] cacheWMA;
		public WMA WMA(int period)
		{
			return WMA(Input, period);
		}

		public WMA WMA(ISeries<double> input, int period)
		{
			if (cacheWMA != null)
				for (int idx = 0; idx < cacheWMA.Length; idx++)
					if (cacheWMA[idx] != null && cacheWMA[idx].Period == period && cacheWMA[idx].EqualsInput(input))
						return cacheWMA[idx];
			return CacheIndicator<WMA>(new WMA(){ Period = period }, input, ref cacheWMA);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.WMA WMA(int period)
		{
			return indicator.WMA(Input, period);
		}

		public Indicators.WMA WMA(ISeries<double> input , int period)
		{
			return indicator.WMA(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.WMA WMA(int period)
		{
			return indicator.WMA(Input, period);
		}

		public Indicators.WMA WMA(ISeries<double> input , int period)
		{
			return indicator.WMA(input, period);
		}
	}
}

#endregion
