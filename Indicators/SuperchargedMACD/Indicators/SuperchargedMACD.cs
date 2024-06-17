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
	public class SuperchargedMACD : Indicator
	{
		
		#region Variables
		private KeltnerChannel myKeltner;
		private Bollinger myBollinger;
		private Vortex myVortex;
		#endregion
		
		#region Display Name
		public override string DisplayName
			{
			get { return Name ;}
			}
		#endregion
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Macd BB with squeeze";
				Name										= "Supercharged MACD";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= false;
				DrawVerticalGridLines						= false;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				MaximumBarsLookBack 						= MaximumBarsLookBack.Infinite;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				
				Fast										= 12;
				Slow										= 26;
				Smoothing									= 5;
				StDev										= 1;
				Period										= 10;
				BandFillColor								= Brushes.LightSlateGray;
				BandFillOpacity								= 50;
				UpColor										= Brushes.Lime;
				DwColor										= Brushes.Red;
				ColorBarsInBB 								= true;
				BarColor									= Brushes.White;
				
				ShowTransparentPlotsInDataBox = true; 
				AddPlot(new Stroke(Brushes.Blue, 3), PlotStyle.Dot, "Macd");
				AddPlot(Brushes.Silver, "BBUpper");
				AddPlot(Brushes.Silver, "BBMid");
				AddPlot(Brushes.Silver, "BBLower");
				AddPlot(Brushes.Transparent, "Signal");
				AddPlot(new Stroke(Brushes.White, 1), PlotStyle.Line, "Momentum");
				
			}
			else if (State == State.Configure)
			{
				myKeltner = KeltnerChannel(1.5, 20); // Reduce the multiplier to get less squeeze
				myBollinger = Bollinger(2,20);
				myVortex = Vortex(20);
			}
		}

		protected override void OnBarUpdate()
		{
			
			if(CurrentBar < Slow) return;
			
			Draw.Region(this, "Fill_BB", CurrentBar, 0,BBLower, BBUpper, null, BandFillColor, BandFillOpacity,0);
			Macd[0] 			= MACD(Input, Fast, Slow, Smoothing)[0];
			double Avg 			= EMA(Macd,Period)[0];
			double SDev 		= StdDev(Macd,Period)[0];	
			double upperBand	= Avg+(StDev*SDev);
			double lowerBand	= Avg-(StDev*SDev);
			BBUpper[0] 			= upperBand;
			BBLower[0] 			= lowerBand;
			BBMid[0]			= Avg;
			
			
			
			if (myBollinger.Upper[0] < myKeltner.Upper[0] && myBollinger.Lower[0] > myKeltner.Lower[0]) // Bollinger Bands inside Keltner Channel indicates a squeeze
		    {
				Draw.Dot(this, "Squeeze" + CurrentBar, true, 0, 0, Brushes.Yellow, false);
		    }
		    else if (Macd[0] < BBLower[0] && Macd[1] >= BBLower[1]) // MACD crosses below the lower BB
		    {
				Draw.ArrowDown(this, "ArrowDown" + CurrentBar, true, 0 , High[0] + TickSize * 2, Brushes.Red, true);
		        Values[4][0] = -1; // Set the signal plot to -1
		    }
			else if (Macd[0] > BBUpper[0] && Macd[1] <= BBUpper[1]) // MACD crosses below the lower BB
		    {
		        Draw.ArrowUp(this, "ArrowUp" + CurrentBar, true, 0, Low[0] - TickSize * 2, Brushes.Lime, true);
		        Values[4][0] = 1; // Set the signal plot to 1
		    }
			
			
			
			if (Macd[0] > BBLower[0] && Macd[0] < BBUpper[0])
			{
				PlotBrushes[0][0] = Brushes.White;
			}
			else if(IsRising(Macd))
			{
				PlotBrushes[0][0] = UpColor;
			}
			else if(IsFalling(Macd))
			{
				PlotBrushes[0][0] = DwColor;
			}
			
			double viPositive = myVortex.VIPlus[0];
    		double viNegative = myVortex.VIMinus[0];
			
			Values[5][0] = 0;
			if (viPositive > viNegative)
		    {
				PlotBrushes[5][0] = UpColor; 
		    }
		    else
		    {
		        PlotBrushes[5][0] = DwColor;
		    }
			
			if (ColorBarsInBB && Macd[0] >= BBLower[0] && Macd[0] <= BBUpper[0])
		    {
		        BarBrush = BarColor;
		    }

			
			
			
		}

		#region Properties
		

		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Fast", Description="Number of bars for fast EMA", Order=1, GroupName="1. MACD")]
		public int Fast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Slow", Description="Number of bars for slow EMA", Order=2, GroupName="1. MACD")]
		public int Slow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Smoothing", Description="Number of bars for smoothing", Order=3, GroupName="1. MACD")]
		public int Smoothing
		{ get; set; }

		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Rising Colour", Description="Default Colour for Rising MACD", Order=8, GroupName="1. MACD")]
		public Brush UpColor
		{ get; set; }

		[Browsable(false)]
		public string UpColorSerializable
		{
			get { return Serialize.BrushToString(UpColor); }
			set { UpColor = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Falling Colour", Description="Default Colour for Falling MACD", Order=9, GroupName="1. MACD")]
		public Brush DwColor
		{ get; set; }

		[Browsable(false)]
		public string DwColorSerializable
		{
			get { return Serialize.BrushToString(DwColor); }
			set { DwColor = Serialize.StringToBrush(value); }
		}			
		
		
		
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StDev Factor", Description="Standard Deviation Factor", Order=1, GroupName="2. BB")]
		public int StDev
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StDev Period", Description="Period for StDev", Order=2, GroupName="2. BB")]
		public int Period
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Fill Opacity", Description="Fill Color Opacity.", Order=4, GroupName="2. BB")]
		public int BandFillOpacity
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Band Fill Color", Description="Default Colour to Fill Bollinger Bands", Order=3, GroupName="2. BB")]
		public Brush BandFillColor
		{ get; set; }

		[Browsable(false)]
		public string BandFillColorSerializable
		{
			get { return Serialize.BrushToString(BandFillColor); }
			set { BandFillColor = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[Display(Name="Color Bars in BB", Description="Color bars if they're inside the Bollinger Bands", Order=5, GroupName="2. BB")]
		public bool ColorBarsInBB
		{ get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Bar Color", Description="Bar color", Order=6, GroupName="2. BB")]
		public Brush BarColor
		{ get; set; }
		
		[Browsable(false)]
		public string BarColorSerializable
		{
			get { return Serialize.BrushToString(BarColor); }
			set { BarColor = Serialize.StringToBrush(value); }
		}	
		
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Macd
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BBUpper
		{
			get { return Values[1]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BBMid
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BBLower
		{
			get { return Values[3]; }
		}


		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Signal
		{
		    get { return Values[4]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Momentum
		{
		    get { return Values[5]; } 
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private SuperchargedMACD[] cacheSuperchargedMACD;
		public SuperchargedMACD SuperchargedMACD(int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			return SuperchargedMACD(Input, fast, slow, smoothing, upColor, dwColor, stDev, period, bandFillOpacity, bandFillColor, colorBarsInBB, barColor);
		}

		public SuperchargedMACD SuperchargedMACD(ISeries<double> input, int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			if (cacheSuperchargedMACD != null)
				for (int idx = 0; idx < cacheSuperchargedMACD.Length; idx++)
					if (cacheSuperchargedMACD[idx] != null && cacheSuperchargedMACD[idx].Fast == fast && cacheSuperchargedMACD[idx].Slow == slow && cacheSuperchargedMACD[idx].Smoothing == smoothing && cacheSuperchargedMACD[idx].UpColor == upColor && cacheSuperchargedMACD[idx].DwColor == dwColor && cacheSuperchargedMACD[idx].StDev == stDev && cacheSuperchargedMACD[idx].Period == period && cacheSuperchargedMACD[idx].BandFillOpacity == bandFillOpacity && cacheSuperchargedMACD[idx].BandFillColor == bandFillColor && cacheSuperchargedMACD[idx].ColorBarsInBB == colorBarsInBB && cacheSuperchargedMACD[idx].BarColor == barColor && cacheSuperchargedMACD[idx].EqualsInput(input))
						return cacheSuperchargedMACD[idx];
			return CacheIndicator<SuperchargedMACD>(new SuperchargedMACD(){ Fast = fast, Slow = slow, Smoothing = smoothing, UpColor = upColor, DwColor = dwColor, StDev = stDev, Period = period, BandFillOpacity = bandFillOpacity, BandFillColor = bandFillColor, ColorBarsInBB = colorBarsInBB, BarColor = barColor }, input, ref cacheSuperchargedMACD);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.SuperchargedMACD SuperchargedMACD(int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			return indicator.SuperchargedMACD(Input, fast, slow, smoothing, upColor, dwColor, stDev, period, bandFillOpacity, bandFillColor, colorBarsInBB, barColor);
		}

		public Indicators.SuperchargedMACD SuperchargedMACD(ISeries<double> input , int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			return indicator.SuperchargedMACD(input, fast, slow, smoothing, upColor, dwColor, stDev, period, bandFillOpacity, bandFillColor, colorBarsInBB, barColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.SuperchargedMACD SuperchargedMACD(int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			return indicator.SuperchargedMACD(Input, fast, slow, smoothing, upColor, dwColor, stDev, period, bandFillOpacity, bandFillColor, colorBarsInBB, barColor);
		}

		public Indicators.SuperchargedMACD SuperchargedMACD(ISeries<double> input , int fast, int slow, int smoothing, Brush upColor, Brush dwColor, int stDev, int period, int bandFillOpacity, Brush bandFillColor, bool colorBarsInBB, Brush barColor)
		{
			return indicator.SuperchargedMACD(input, fast, slow, smoothing, upColor, dwColor, stDev, period, bandFillOpacity, bandFillColor, colorBarsInBB, barColor);
		}
	}
}

#endregion
