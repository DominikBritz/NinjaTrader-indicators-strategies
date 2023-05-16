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

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class IFTOnRSI : Indicator
	{
		
		private Series<double> v12;
		private Series<double> v22;
		private Series<double> INV2;
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Inverse Fisher Transform on RSI";
				Name										= "IFTOnRSI";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				SmoothingLength					= 9;
				RSILength					= 5;
				
				FilterHigh					= 0.5;
				FilterLow					= -0.5;
				BullBrush					= Brushes.Lime;
				BearBrush					= Brushes.Red;
				NeutralBrush				= Brushes.Gray;
				
				AddPlot(NeutralBrush, "IFTOnRSI");
				AddLine(BullBrush, FilterHigh, "Higher");
				AddLine(BearBrush, FilterLow, "Lower");
			}
			else if (State == State.Configure)
			{
				v12 = new Series<double>(this);
				v22 = new Series<double>(this);
				INV2 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < SmoothingLength +2)
				return;
			
			// RSI
		    v12[0] = 0.1 * (RSI(Close, RSILength, 3)[0] - 50);
			v22[0] = WMA(v12, SmoothingLength)[0];
			INV2[0] = (Math.Exp(2 * v22[0]) - 1) / (Math.Exp(2 * v22[0]) + 1);
			
			PlotBrushes[0][0] = INV2[0] >= FilterHigh ? BullBrush : INV2[0] <= FilterLow ? BearBrush : NeutralBrush;
			
			Value[0] = INV2[0];
		}

		#region Properties
		[NinjaScriptProperty]
		[Display(Name="SmoothingLength", Order=1, GroupName="Parameters")]
		public int SmoothingLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="RSILength", Order=2, GroupName="Parameters")]
		public int RSILength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="FilterHigh", Order=3, GroupName="Parameters")]
		public double FilterHigh
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="FilterLow", Order=4, GroupName="Parameters")]
		public double FilterLow
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "Bull Color", GroupName="Colors", Order=1)]
		public Brush BullBrush
		{ get; set; }

		[Browsable(false)]
		public string BullBrushSerialize
		{
			get { return Serialize.BrushToString(BullBrush); }
   			set { BullBrush = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore()]
		[Display(Name = "Bear Color", GroupName="Colors", Order=2)]
		public Brush BearBrush
		{ get; set; }

		[Browsable(false)]
		public string BearBrushSerialize
		{
			get { return Serialize.BrushToString(BearBrush); }
   			set { BearBrush = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore()]
		[Display(Name = "Neutral Color", GroupName="Colors", Order=3)]
		public Brush NeutralBrush
		{ get; set; }

		[Browsable(false)]
		public string NeutralBrushSerialize
		{
			get { return Serialize.BrushToString(NeutralBrush); }
   			set { NeutralBrush = Serialize.StringToBrush(value); }
		}
		
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> pIFTOnRSI
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private IFTOnRSI[] cacheIFTOnRSI;
		public IFTOnRSI IFTOnRSI(int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			return IFTOnRSI(Input, smoothingLength, rSILength, filterHigh, filterLow);
		}

		public IFTOnRSI IFTOnRSI(ISeries<double> input, int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			if (cacheIFTOnRSI != null)
				for (int idx = 0; idx < cacheIFTOnRSI.Length; idx++)
					if (cacheIFTOnRSI[idx] != null && cacheIFTOnRSI[idx].SmoothingLength == smoothingLength && cacheIFTOnRSI[idx].RSILength == rSILength && cacheIFTOnRSI[idx].FilterHigh == filterHigh && cacheIFTOnRSI[idx].FilterLow == filterLow && cacheIFTOnRSI[idx].EqualsInput(input))
						return cacheIFTOnRSI[idx];
			return CacheIndicator<IFTOnRSI>(new IFTOnRSI(){ SmoothingLength = smoothingLength, RSILength = rSILength, FilterHigh = filterHigh, FilterLow = filterLow }, input, ref cacheIFTOnRSI);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.IFTOnRSI IFTOnRSI(int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			return indicator.IFTOnRSI(Input, smoothingLength, rSILength, filterHigh, filterLow);
		}

		public Indicators.IFTOnRSI IFTOnRSI(ISeries<double> input , int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			return indicator.IFTOnRSI(input, smoothingLength, rSILength, filterHigh, filterLow);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.IFTOnRSI IFTOnRSI(int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			return indicator.IFTOnRSI(Input, smoothingLength, rSILength, filterHigh, filterLow);
		}

		public Indicators.IFTOnRSI IFTOnRSI(ISeries<double> input , int smoothingLength, int rSILength, double filterHigh, double filterLow)
		{
			return indicator.IFTOnRSI(input, smoothingLength, rSILength, filterHigh, filterLow);
		}
	}
}

#endregion
