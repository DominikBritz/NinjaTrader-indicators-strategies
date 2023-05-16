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
	/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
	/// </summary>
	public class RSI : Indicator
	{
		private Series<double>		avgDown;
		private Series<double>		avgUp;
		private double				constant1;
		private double				constant2;
		private double				constant3;
		private Series<double>		down;
		private SMA					smaDown;
		private	SMA					smaUp;
		private Series<double>		up;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionRSI;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameRSI;
				IsSuspendedWhileInactive	= true;
				BarsRequiredToPlot			= 20;
				Period						= 14;
				Smooth						= 3;

				AddPlot(Brushes.DodgerBlue,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameRSI);
				AddPlot(Brushes.Goldenrod,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorAvg);

				AddLine(Brushes.DarkCyan,	30,	NinjaTrader.Custom.Resource.NinjaScriptIndicatorLower);
				AddLine(Brushes.DarkCyan,	70,	NinjaTrader.Custom.Resource.NinjaScriptIndicatorUpper);
			}
			else if (State == State.Configure)
			{
				constant1 = 2.0 / (1 + Smooth);
				constant2 = (1 - (2.0 / (1 + Smooth)));
				constant3 = (Period - 1);
			}
			else if (State == State.DataLoaded)
			{
				avgUp	= new Series<double>(this);
				avgDown = new Series<double>(this);
				down	= new Series<double>(this);
				up		= new Series<double>(this);
				smaDown = SMA(down, Period);
				smaUp	= SMA(up, Period);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				down[0]		= 0;
				up[0]		= 0;

				if (Period < 3)
					Avg[0] = 50;

				return;
			}

			double input0	= Input[0];
			double input1	= Input[1];
			down[0]			= Math.Max(input1 - input0, 0);
			up[0]			= Math.Max(input0 - input1, 0);

			if (CurrentBar + 1 < Period)
			{
				if (CurrentBar + 1 == Period - 1)
					Avg[0] = 50;
				return;
			}

			if ((CurrentBar + 1) == Period)
			{
				// First averages
				avgDown[0]	= smaDown[0];
				avgUp[0]	= smaUp[0];
			}
			else
			{
				// Rest of averages are smoothed
				avgDown[0]	= (avgDown[1] * constant3 + down[0]) / Period;
				avgUp[0]	= (avgUp[1] * constant3 + up[0]) / Period;
			}

			double avgDown0	= avgDown[0];
			double value0	= avgDown0 == 0 ? 100 : 100 - 100 / (1 + avgUp[0] / avgDown0);
			Default[0]		= value0;
			Avg[0]			= constant1 * value0 + constant2 * Avg[1];
		}

		#region Properties
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Avg
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Default
		{
			get { return Values[0]; }
		}

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Smooth", GroupName = "NinjaScriptParameters", Order = 1)]
		public int Smooth
		{ get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private RSI[] cacheRSI;
		public RSI RSI(int period, int smooth)
		{
			return RSI(Input, period, smooth);
		}

		public RSI RSI(ISeries<double> input, int period, int smooth)
		{
			if (cacheRSI != null)
				for (int idx = 0; idx < cacheRSI.Length; idx++)
					if (cacheRSI[idx] != null && cacheRSI[idx].Period == period && cacheRSI[idx].Smooth == smooth && cacheRSI[idx].EqualsInput(input))
						return cacheRSI[idx];
			return CacheIndicator<RSI>(new RSI(){ Period = period, Smooth = smooth }, input, ref cacheRSI);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.RSI RSI(int period, int smooth)
		{
			return indicator.RSI(Input, period, smooth);
		}

		public Indicators.RSI RSI(ISeries<double> input , int period, int smooth)
		{
			return indicator.RSI(input, period, smooth);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.RSI RSI(int period, int smooth)
		{
			return indicator.RSI(Input, period, smooth);
		}

		public Indicators.RSI RSI(ISeries<double> input , int period, int smooth)
		{
			return indicator.RSI(input, period, smooth);
		}
	}
}

#endregion
