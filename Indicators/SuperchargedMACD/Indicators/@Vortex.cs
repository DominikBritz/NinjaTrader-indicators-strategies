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
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The Vortex indicator is an oscillator used to identify trends. A bullish signal triggers when the VIPlus line crosses above the VIMinus line. A bearish signal triggers when the VIMinus line crosses above the VIPlus line.
	/// </summary>
	public class Vortex : Indicator
	{
		private Series<double> vmPlus;
		private Series<double> vmMinus;
		private Series<double> trueRange;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionVortex;
				Name						= NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameVortex;
				IsOverlay					= false;
				IsSuspendedWhileInactive	= true;
				Period						= 14;

				AddPlot(Brushes.RoyalBlue,	NinjaTrader.Custom.Resource.NinjaScriptIndicatorVIPlus);
				AddPlot(Brushes.Red,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorVIMinus);
			}
			else if (State == State.DataLoaded)
			{				
				vmPlus		= new Series<double>(this);
				vmMinus		= new Series<double>(this);
				trueRange	= new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1)
				return;
			
			vmPlus[0] 		= Math.Abs(High[0] - Low[1]);
			vmMinus[0]		= Math.Abs(Low[0] - High[1]);
			trueRange[0]	= Math.Max(Math.Max(Math.Abs(High[0] - Close[1]), Math.Abs(Low[0] - Close[1])), High[0]-Low[0]);
            VIPlus[0]		= SUM(vmPlus, Period)[0] / SUM(trueRange, Period)[0];
            VIMinus[0]		= SUM(vmMinus, Period)[0] / SUM(trueRange, Period)[0];
		}

		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> VIPlus
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> VIMinus
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
		private Vortex[] cacheVortex;
		public Vortex Vortex(int period)
		{
			return Vortex(Input, period);
		}

		public Vortex Vortex(ISeries<double> input, int period)
		{
			if (cacheVortex != null)
				for (int idx = 0; idx < cacheVortex.Length; idx++)
					if (cacheVortex[idx] != null && cacheVortex[idx].Period == period && cacheVortex[idx].EqualsInput(input))
						return cacheVortex[idx];
			return CacheIndicator<Vortex>(new Vortex(){ Period = period }, input, ref cacheVortex);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Vortex Vortex(int period)
		{
			return indicator.Vortex(Input, period);
		}

		public Indicators.Vortex Vortex(ISeries<double> input , int period)
		{
			return indicator.Vortex(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Vortex Vortex(int period)
		{
			return indicator.Vortex(Input, period);
		}

		public Indicators.Vortex Vortex(ISeries<double> input , int period)
		{
			return indicator.Vortex(input, period);
		}
	}
}

#endregion
