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
	/// Keltner Channel. The Keltner Channel is a similar indicator to Bollinger Bands.
	/// Here the midline is a standard moving average with the upper and lower bands offset
	/// by the SMA of the difference between the high and low of the previous bars.
	/// The offset multiplier as well as the SMA period is configurable.
	/// </summary>
	public class KeltnerChannel : Indicator
	{
		private Series<double>		diff;
		private	SMA					smaDiff;
		private	SMA					smaTypical;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionKeltnerChannel;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameKelterChannel;
				Period						= 10;
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;
				OffsetMultiplier			= 1.5;

				AddPlot(Brushes.DarkGray,		NinjaTrader.Custom.Resource.KeltnerChannelMidline);
				AddPlot(Brushes.DodgerBlue,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorUpper);
				AddPlot(Brushes.DodgerBlue,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorLower);
			}
			else if (State == State.DataLoaded)
			{
				diff				= new Series<double>(this);
				smaDiff				= SMA(diff, Period);
				smaTypical			= SMA(Typical, Period);
			}
		}

		protected override void OnBarUpdate()
		{
			diff[0]			= High[0] - Low[0];

			double middle	= smaTypical[0];
			double offset	= smaDiff[0] * OffsetMultiplier;

			double upper	= middle + offset;
			double lower	= middle - offset;

			Midline[0]		= middle;
			Upper[0]		= upper;
			Lower[0]		= lower;
		}

		#region Properties
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Lower
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Midline
		{
			get { return Values[0]; }
		}

		[Range(0.01, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "OffsetMultiplier", GroupName = "NinjaScriptParameters", Order = 0)]
		public double OffsetMultiplier
		{ get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 1)]
		public int Period
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Upper
		{
			get { return Values[1]; }
		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private KeltnerChannel[] cacheKeltnerChannel;
		public KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
		{
			return KeltnerChannel(Input, offsetMultiplier, period);
		}

		public KeltnerChannel KeltnerChannel(ISeries<double> input, double offsetMultiplier, int period)
		{
			if (cacheKeltnerChannel != null)
				for (int idx = 0; idx < cacheKeltnerChannel.Length; idx++)
					if (cacheKeltnerChannel[idx] != null && cacheKeltnerChannel[idx].OffsetMultiplier == offsetMultiplier && cacheKeltnerChannel[idx].Period == period && cacheKeltnerChannel[idx].EqualsInput(input))
						return cacheKeltnerChannel[idx];
			return CacheIndicator<KeltnerChannel>(new KeltnerChannel(){ OffsetMultiplier = offsetMultiplier, Period = period }, input, ref cacheKeltnerChannel);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
		{
			return indicator.KeltnerChannel(Input, offsetMultiplier, period);
		}

		public Indicators.KeltnerChannel KeltnerChannel(ISeries<double> input , double offsetMultiplier, int period)
		{
			return indicator.KeltnerChannel(input, offsetMultiplier, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
		{
			return indicator.KeltnerChannel(Input, offsetMultiplier, period);
		}

		public Indicators.KeltnerChannel KeltnerChannel(ISeries<double> input , double offsetMultiplier, int period)
		{
			return indicator.KeltnerChannel(input, offsetMultiplier, period);
		}
	}
}

#endregion
