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
	public class ProperSignalBar : Indicator
	{
		private EMA myEMA;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"https://github.com/DominikBritz/NinjaTrader-indicators-strategies/tree/main/Indicators/ProperSignalBar";
				Name										= "Proper Signal Bar";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				BarsRequiredToPlot							= 60;


				// Inputs misc
				BarMaxSizeInTicks						= 25;
				BarMinSizeInTicks						= 4;
				BarMinBodyInTicks					    = 4;
				BarCloseToHighLowInTicks			    = 2;
				BarMinTickHigherLower					= 1;
				BarAwayFromEMAInTicks					= 6;
				EMALength								= 21;
				AlertFile								= @"Alert1.wav";
				
				// Inputs target
				TargetScalpLessTicksThanSL				= 4;
				TargetRunnerTicks						= 30;
				
				// Plots
				ShowTransparentPlotsInDataBox = true;  
				AddPlot(Brushes.Transparent, "IsBullishBar");
				AddPlot(Brushes.Transparent, "IsBearishBar");
			
				AddPlot(Brushes.Transparent, "IsCloseAboveEMA");
				AddPlot(Brushes.Transparent, "IsCloseBelowEMA");
				
				AddPlot(Brushes.Transparent, "IsBarLowerThanPrevious");
				AddPlot(Brushes.Transparent, "IsBarHigherThanPrevious");
				
				AddPlot(Brushes.Transparent, "IsLowNearEMA");
				AddPlot(Brushes.Transparent, "IsHighNearEMA");
				
				AddPlot(Brushes.Transparent, "IsCloseNearHigh");
				AddPlot(Brushes.Transparent, "IsCloseNearLow");
				
				AddPlot(Brushes.Transparent, "IsBullishBarNotTooLarge");
				AddPlot(Brushes.Transparent, "IsBearishBarNotTooLarge");
				
				AddPlot(Brushes.Transparent, "IsBullishBarNotTooSmall");
				AddPlot(Brushes.Transparent, "IsBearishBarNotTooSmall");
				
				AddPlot(Brushes.Transparent, "IsNotBullishDojiBar");
				AddPlot(Brushes.Transparent, "IsNotBearishDojiBar");
				
				AddPlot(Brushes.Transparent, "IsProperSignalBar");
				
				
			}
			else if (State == State.Configure)
			{
			}
			
			else if (State == State.DataLoaded)
			{
				myEMA = EMA(EMALength);
			}
		}

		protected override void OnBarUpdate()
		{
			
			if (CurrentBars[0] < BarsRequiredToPlot)
			return;

			int IsBullishBar = 0;
			int IsBearishBar = 0;
			
			int IsCloseAboveEMA = 0;
			int IsCloseBelowEMA = 0;
			
			int IsBarLowerThanPrevious = 0;
			int IsBarHigherThanPrevious = 0;
			
			int IsLowNearEMA = 0;
			int IsHighNearEMA = 0;
			
			int IsCloseNearHigh = 0;
			int IsCloseNearLow = 0;
			
			int IsBullishBarNotTooLarge = 0;
			int IsBearishBarNotTooLarge = 0;
			
			int IsBullishBarNotTooSmall = 0;
			int IsBearishBarNotTooSmall = 0;
			
			int IsNotBullishDojiBar = 0;
			int IsNotBearishDojiBar = 0;
			
			int IsProperSignalBar = 0;
			
			// General vars
			
			double SignalBarSize = (High[0] - Low[0]) / TickSize;
			
			//
			// Bullish
			//
			
			// Bullish bar
			if (Close[0] > Open[0])
			{
				IsBullishBar = 1;
			}
			
			// Close above EMA
			if (Close[0] > myEMA[0])
			{
				IsCloseAboveEMA = 1;
			}
			
			// Bar lower than the previous to trap shorts
			if (Low[0] < Low[1])
			{
				IsBarLowerThanPrevious = 1;
			}
				
			// Bar low not too far away from EMA
			int BarLowTicksAway = Convert.ToInt32((Low[0] - myEMA[0]) / TickSize);
			if (BarLowTicksAway < BarAwayFromEMAInTicks)
			{
				IsLowNearEMA = 1;
			}
						
			// Bar closes near its high
			if (High[0] / TickSize - Close[0] / TickSize <= BarCloseToHighLowInTicks)
			{
				IsCloseNearHigh = 1;
			}
			
			// Bar not too large
			if (SignalBarSize <= BarMaxSizeInTicks)
			{
				IsBullishBarNotTooLarge = 1;
			}
			
			// Bar not too small
			if (SignalBarSize >= BarMinSizeInTicks)
			{
				IsBullishBarNotTooSmall = 1;
			}
			
			// No Dojo bar
			if (Close[0] / TickSize - Open[0] / TickSize > BarMinBodyInTicks)
			{
				IsNotBullishDojiBar = 1;
			}
							
			if (IsBullishBar == 1 && IsCloseAboveEMA == 1 && IsBarLowerThanPrevious == 1 && IsLowNearEMA == 1 && IsCloseNearHigh == 1 && IsBullishBarNotTooLarge == 1 && IsBullishBarNotTooSmall == 1 && IsNotBullishDojiBar ==1)
			{
				IsProperSignalBar = 1;
				
				if (string.IsNullOrEmpty(AlertFile) == false)
				{
					try
					{
						Alert("Proper signal bar"+CurrentBar, Priority.High, "Proper signal bar long", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+AlertFile, 0, Brushes.Green, Brushes.White);  
					}
					catch (Exception e)
					{
						Print("Error playing alert file. Error message:\n" + e.Message);
					}
				}

				Draw.TriangleUp(this, "BullishSignalBar"+CurrentBar, true, 0, Low[0] - 2 * TickSize, Brushes.Green);
						
				
				double SL = SignalBarSize + 2;
				Draw.Text(this, "SignalBarSize"+CurrentBar, "SL: "+Convert.ToString(SL), 0, Low[0] - 6 * TickSize, Brushes.Yellow);
				Draw.Line(this, "Entry"+CurrentBar, false, 0, High[0] + 1 * TickSize, -1, High[0] + 1 * TickSize, Brushes.HotPink, DashStyleHelper.Solid, 1);
				Draw.Line(this, "Scalp"+CurrentBar, false, 0, High[0] + ((SL-TargetScalpLessTicksThanSL + 1) * TickSize), -1, High[0] + ((SL-TargetScalpLessTicksThanSL + 1) * TickSize), Brushes.LimeGreen, DashStyleHelper.Solid, 2);
				Draw.Line(this, "Runner"+CurrentBar, false, 0, High[0] + 1 * TickSize + (TargetRunnerTicks * TickSize), -1, High[0] + 1 * TickSize + (TargetRunnerTicks * TickSize), Brushes.LimeGreen, DashStyleHelper.Solid, 5);
			}
							
	
			
			
			//
			// Bearish
			//
			
			// Bearish bar
			if (Close[0] < Open[0])
			{
				IsBearishBar = 1;
			}
			
			// Close below EMA
			if (Close[0] < myEMA[0])
			{
				IsCloseBelowEMA = 1;
			}
			
			// Bar higher than the previous to trap longs
			if (High[0] > High[1])
			{
				IsBarHigherThanPrevious = 1;
			}
				
			// Bar high not too far away from EMA
			int BarHighTicksAway = Convert.ToInt32((myEMA[0] - High[0]) / TickSize);
			if (BarHighTicksAway < BarAwayFromEMAInTicks)
			{
				IsHighNearEMA = 1;
			}
						
			// Bar closes near its low
			if (Close[0] / TickSize - Low[0] / TickSize <= BarCloseToHighLowInTicks)
			{
				IsCloseNearLow = 1;
			}
			
			// Bar not too large
			if (SignalBarSize <= BarMaxSizeInTicks)
			{
				IsBearishBarNotTooLarge = 1;
			}
			
			// Bar not too small
			if (SignalBarSize >= BarMinSizeInTicks)
			{
				IsBearishBarNotTooSmall = 1;
			}
			
			// No Dojo bar
			if (Open[0] / TickSize - Close[0] / TickSize > BarMinBodyInTicks)
			{
				IsNotBearishDojiBar = 1;
			}
							
			if (IsBearishBar == 1 && IsCloseBelowEMA == 1 && IsBarHigherThanPrevious == 1 && IsHighNearEMA == 1 && IsCloseNearLow == 1 && IsBearishBarNotTooLarge == 1 && IsBearishBarNotTooSmall == 1 && IsNotBearishDojiBar == 1)
			{
				IsProperSignalBar = -1;

				if (string.IsNullOrEmpty(AlertFile) == false)
				{
					try
					{
						Alert("Proper signal bar"+CurrentBar, Priority.High, "Proper signal bar short", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+AlertFile, 0, Brushes.Red, Brushes.White);  
					}
					catch (Exception e)
					{
						Print("Error playing alert file. Error message:\n" + e.Message);
					}
				}
				
				Draw.TriangleDown(this, "BearishSignalBar"+CurrentBar, true, 0, High[0] + 2 * TickSize, Brushes.Red);
						
				
				double SL = SignalBarSize + 2;
				Draw.Text(this, "SignalBarSize"+CurrentBar, "SL: "+Convert.ToString(SL), 0, High[0] + 6 * TickSize, Brushes.Yellow);
				Draw.Line(this, "Entry"+CurrentBar, false, 0, Low[0] - 1 * TickSize, -1, Low[0] - 1 * TickSize, Brushes.HotPink, DashStyleHelper.Solid, 1);
				Draw.Line(this, "Scalp"+CurrentBar, false, 0, Low[0] - ((SL-TargetScalpLessTicksThanSL+1) * TickSize), -1, Low[0] - ((SL-TargetScalpLessTicksThanSL+1) * TickSize), Brushes.Red, DashStyleHelper.Solid, 2);
				Draw.Line(this, "Runner"+CurrentBar, false, 0, Low[0] - 1 * TickSize - (TargetRunnerTicks * TickSize), -1, Low[0] - 1 * TickSize - (TargetRunnerTicks * TickSize), Brushes.Red, DashStyleHelper.Solid, 5);
			}
			

			//
			// Plots
			//
			
			Values[0][0] = IsBullishBar;
			Values[1][0] = IsBearishBar;
			
			Values[2][0] = IsCloseAboveEMA;
			Values[3][0] = IsCloseBelowEMA;
			
			Values[4][0] = IsBarLowerThanPrevious;
			Values[5][0] = IsBarHigherThanPrevious;
			
			Values[6][0] = IsLowNearEMA;
			Values[7][0] = IsHighNearEMA;
			
			Values[8][0] = IsCloseNearHigh;
			Values[9][0] = IsCloseNearLow;
			
			Values[10][0] = IsBullishBarNotTooLarge;
			Values[11][0] = IsBearishBarNotTooLarge;
			
			Values[12][0] = IsBullishBarNotTooSmall;
			Values[13][0] = IsBearishBarNotTooSmall;
			
			Values[14][0] = IsNotBullishDojiBar;
			Values[15][0] = IsNotBearishDojiBar;
			
			Values[16][0] = IsProperSignalBar; 
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarMaxSizeInTicks", Order=1, GroupName="Parameters")]
		public int BarMaxSizeInTicks
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarMinSizeInTicks", Order=2, GroupName="Parameters")]
		public int BarMinSizeInTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarCloseToHighLowInTicks", Order=3, GroupName="Parameters")]
		public int BarCloseToHighLowInTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarMinTickHigherLower", Order=4, GroupName="Parameters")]
		public int BarMinTickHigherLower
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarAwayFromEMAInTicks", Order=5, GroupName="Parameters")]
		public int BarAwayFromEMAInTicks
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarMinBodyInTicks", Order=6, GroupName="Parameters")]
		public int BarMinBodyInTicks
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMALength", Order=7, GroupName="Parameters")]
		public int EMALength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="AlertFile", Order=8, GroupName="Parameters")]
		public string AlertFile
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TargetScalpLessTicksThanSL", Order=1, GroupName="Targets")]
		public int TargetScalpLessTicksThanSL
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TargetRunnerTicks", Order=2, GroupName="Targets")]
		public int TargetRunnerTicks
		{ get; set; }
		#endregion

		
		#region Plots
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBullishBar
		{ get { return Values[0]; } }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBearishBar
		{ get { return Values[1]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsCloseAboveEMA
		{ get { return Values[2]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsCloseBelowEMA
		{ get { return Values[3]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBarLowerThanPrevious
		{ get { return Values[4]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBarHigherThanPrevious
		{ get { return Values[5]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsLowNearEMA
		{ get { return Values[6]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsHighNearEMA
		{ get { return Values[7]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsCloseNearHigh
		{ get { return Values[8]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsCloseNearLow
		{ get { return Values[9]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBullishBarNotTooLarge
		{ get { return Values[10]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBearishBarNotTooLarge
		{ get { return Values[11]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBullishBarNotTooSmall
		{ get { return Values[12]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsBearishBarNotTooSmall
		{ get { return Values[13]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsNotBullishDojiBar
		{ get { return Values[14]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsNotBearishDojiBar
		{ get { return Values[15]; } }
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> IsProperSignalBar
		{ get { return Values[16]; } }
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ProperSignalBar[] cacheProperSignalBar;
		public ProperSignalBar ProperSignalBar(int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			return ProperSignalBar(Input, barMaxSizeInTicks, barMinSizeInTicks, barCloseToHighLowInTicks, barMinTickHigherLower, barAwayFromEMAInTicks, barMinBodyInTicks, eMALength, alertFile, targetScalpLessTicksThanSL, targetRunnerTicks);
		}

		public ProperSignalBar ProperSignalBar(ISeries<double> input, int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			if (cacheProperSignalBar != null)
				for (int idx = 0; idx < cacheProperSignalBar.Length; idx++)
					if (cacheProperSignalBar[idx] != null && cacheProperSignalBar[idx].BarMaxSizeInTicks == barMaxSizeInTicks && cacheProperSignalBar[idx].BarMinSizeInTicks == barMinSizeInTicks && cacheProperSignalBar[idx].BarCloseToHighLowInTicks == barCloseToHighLowInTicks && cacheProperSignalBar[idx].BarMinTickHigherLower == barMinTickHigherLower && cacheProperSignalBar[idx].BarAwayFromEMAInTicks == barAwayFromEMAInTicks && cacheProperSignalBar[idx].BarMinBodyInTicks == barMinBodyInTicks && cacheProperSignalBar[idx].EMALength == eMALength && cacheProperSignalBar[idx].AlertFile == alertFile && cacheProperSignalBar[idx].TargetScalpLessTicksThanSL == targetScalpLessTicksThanSL && cacheProperSignalBar[idx].TargetRunnerTicks == targetRunnerTicks && cacheProperSignalBar[idx].EqualsInput(input))
						return cacheProperSignalBar[idx];
			return CacheIndicator<ProperSignalBar>(new ProperSignalBar(){ BarMaxSizeInTicks = barMaxSizeInTicks, BarMinSizeInTicks = barMinSizeInTicks, BarCloseToHighLowInTicks = barCloseToHighLowInTicks, BarMinTickHigherLower = barMinTickHigherLower, BarAwayFromEMAInTicks = barAwayFromEMAInTicks, BarMinBodyInTicks = barMinBodyInTicks, EMALength = eMALength, AlertFile = alertFile, TargetScalpLessTicksThanSL = targetScalpLessTicksThanSL, TargetRunnerTicks = targetRunnerTicks }, input, ref cacheProperSignalBar);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ProperSignalBar ProperSignalBar(int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			return indicator.ProperSignalBar(Input, barMaxSizeInTicks, barMinSizeInTicks, barCloseToHighLowInTicks, barMinTickHigherLower, barAwayFromEMAInTicks, barMinBodyInTicks, eMALength, alertFile, targetScalpLessTicksThanSL, targetRunnerTicks);
		}

		public Indicators.ProperSignalBar ProperSignalBar(ISeries<double> input , int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			return indicator.ProperSignalBar(input, barMaxSizeInTicks, barMinSizeInTicks, barCloseToHighLowInTicks, barMinTickHigherLower, barAwayFromEMAInTicks, barMinBodyInTicks, eMALength, alertFile, targetScalpLessTicksThanSL, targetRunnerTicks);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ProperSignalBar ProperSignalBar(int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			return indicator.ProperSignalBar(Input, barMaxSizeInTicks, barMinSizeInTicks, barCloseToHighLowInTicks, barMinTickHigherLower, barAwayFromEMAInTicks, barMinBodyInTicks, eMALength, alertFile, targetScalpLessTicksThanSL, targetRunnerTicks);
		}

		public Indicators.ProperSignalBar ProperSignalBar(ISeries<double> input , int barMaxSizeInTicks, int barMinSizeInTicks, int barCloseToHighLowInTicks, int barMinTickHigherLower, int barAwayFromEMAInTicks, int barMinBodyInTicks, int eMALength, string alertFile, int targetScalpLessTicksThanSL, int targetRunnerTicks)
		{
			return indicator.ProperSignalBar(input, barMaxSizeInTicks, barMinSizeInTicks, barCloseToHighLowInTicks, barMinTickHigherLower, barAwayFromEMAInTicks, barMinBodyInTicks, eMALength, alertFile, targetScalpLessTicksThanSL, targetRunnerTicks);
		}
	}
}

#endregion
