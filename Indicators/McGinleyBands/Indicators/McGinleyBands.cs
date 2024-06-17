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
	public class McGinleyBands : Indicator
	{
		private double Num;
		
		private Series<double> upperBand;
        private Series<double> lowerBand;
		
		private ATR atr;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				
				Description									= @"McGinley Bands";
				Name										= "McGinley Bands";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				
				Period 										= 21;
				Smoothing                                   = 0.618;
				Exponent                                    = 4;
				BColor										= Brushes.Green;
				SColor										= Brushes.Red;	
				NColor										= Brushes.Yellow;
				
				ATRMulti									= 1.0;
				ATRPeriod									= 100;
				
				AddPlot(Brushes.ForestGreen, "McGinleyDynamic");
				AddPlot(Brushes.Cyan, "UpperBand");
                AddPlot(Brushes.Magenta, "LowerBand");
			}
			else if (State == State.Configure)
			{
				Num = Smoothing * Period;
				atr = ATR(ATRPeriod);
                upperBand = new Series<double>(this);
                lowerBand = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 0)
				return;
			
			if (CurrentBar == 0)
				Value[0] = Input[0];
			else
				Value[0] = Value[1] + (Input[0] - Value[1]) / (Num * Math.Pow(Input[0] / Value[1], Exponent));
			
		    if (Value[0] < Input[0])
			{
				PlotBrushes[0][0] = BColor;
				PlotBrushes[1][0] = BColor;
				PlotBrushes[2][0] = BColor;
			}
    		else
      		   	if (Value[0] > Input[0])
				{
			   		PlotBrushes[0][0] = SColor;
					PlotBrushes[1][0] = SColor;	
					PlotBrushes[2][0] = SColor;
				}
				else
				{
					PlotBrushes[0][0] = NColor;
					PlotBrushes[1][0] = NColor;	
					PlotBrushes[2][0] = NColor;
				}
				
			double atrValue = atr[0] * ATRMulti;
				
			// Calculate Upper and Lower Bands
            upperBand[0] = Value[0] + atrValue;
            lowerBand[0] = Value[0] - atrValue;

            // Plot Bands
            UpperBand[0] = upperBand[0];
            LowerBand[0] = lowerBand[0];
		}
		
#region Properties		
		[NinjaScriptProperty, Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "Parameters", Order = 0)]
		public int Period { get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name="Smoothing", GroupName="Parameters", Order = 1)]
		public double Smoothing
		{ get; set; }	
		
		[NinjaScriptProperty, Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Exponent", GroupName = "Parameters", Order = 2)]
		public int Exponent { get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Long Color", Description="Buy Color", Order=3, GroupName="Parameters")]
		public Brush BColor
		{ get; set; }

		[Browsable(false)]
		public string BColorSerializable
		{
			get { return Serialize.BrushToString(BColor); }
			set { BColor = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Short Color", Description="Sell Color", Order=4, GroupName="Parameters")]
		public Brush SColor
		{ get; set; }

		[Browsable(false)]
		public string SColorSerializable
		{
			get { return Serialize.BrushToString(SColor); }
			set { SColor = Serialize.StringToBrush(value); }
		}	
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Neutral Color", Description="Neutral Color", Order=5, GroupName="Parameters")]
		public Brush NColor
		{ get; set; }

		[Browsable(false)]
		public string NColorSerializable
		{
			get { return Serialize.BrushToString(NColor); }
			set { SColor = Serialize.StringToBrush(value); }
		}	
		
		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name="ATR Multiplier", GroupName="Parameters", Order = 6)]
		public double ATRMulti
		{ get; set; }
		
		[NinjaScriptProperty, Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Period", GroupName = "Parameters", Order = 7)]
		public int ATRPeriod { get; set; }
		
		[Browsable(false)]
		public Series<double> McGinley
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)]
        [XmlIgnore()]
        public Series<double> UpperBand
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public Series<double> LowerBand
        {
            get { return Values[2]; }
        }
#endregion		
		
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private McGinleyBands[] cacheMcGinleyBands;
		public McGinleyBands McGinleyBands(int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			return McGinleyBands(Input, period, smoothing, exponent, bColor, sColor, nColor, aTRMulti, aTRPeriod);
		}

		public McGinleyBands McGinleyBands(ISeries<double> input, int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			if (cacheMcGinleyBands != null)
				for (int idx = 0; idx < cacheMcGinleyBands.Length; idx++)
					if (cacheMcGinleyBands[idx] != null && cacheMcGinleyBands[idx].Period == period && cacheMcGinleyBands[idx].Smoothing == smoothing && cacheMcGinleyBands[idx].Exponent == exponent && cacheMcGinleyBands[idx].BColor == bColor && cacheMcGinleyBands[idx].SColor == sColor && cacheMcGinleyBands[idx].NColor == nColor && cacheMcGinleyBands[idx].ATRMulti == aTRMulti && cacheMcGinleyBands[idx].ATRPeriod == aTRPeriod && cacheMcGinleyBands[idx].EqualsInput(input))
						return cacheMcGinleyBands[idx];
			return CacheIndicator<McGinleyBands>(new McGinleyBands(){ Period = period, Smoothing = smoothing, Exponent = exponent, BColor = bColor, SColor = sColor, NColor = nColor, ATRMulti = aTRMulti, ATRPeriod = aTRPeriod }, input, ref cacheMcGinleyBands);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.McGinleyBands McGinleyBands(int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			return indicator.McGinleyBands(Input, period, smoothing, exponent, bColor, sColor, nColor, aTRMulti, aTRPeriod);
		}

		public Indicators.McGinleyBands McGinleyBands(ISeries<double> input , int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			return indicator.McGinleyBands(input, period, smoothing, exponent, bColor, sColor, nColor, aTRMulti, aTRPeriod);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.McGinleyBands McGinleyBands(int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			return indicator.McGinleyBands(Input, period, smoothing, exponent, bColor, sColor, nColor, aTRMulti, aTRPeriod);
		}

		public Indicators.McGinleyBands McGinleyBands(ISeries<double> input , int period, double smoothing, int exponent, Brush bColor, Brush sColor, Brush nColor, double aTRMulti, int aTRPeriod)
		{
			return indicator.McGinleyBands(input, period, smoothing, exponent, bColor, sColor, nColor, aTRMulti, aTRPeriod);
		}
	}
}

#endregion
