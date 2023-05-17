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
	public class TDFI : Indicator
	{
		
		private EMA mma;
		private EMA smma;
		
		private Series<double> tdf;
		private Series<double> result;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"";
				Name										= "TDFI";
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
				
				Lookback					= 13;
				MMALength					= 13;
				SmmaLength					= 13;
				NLength						= 3;
				FilterHigh					= 0.05;
				FilterLow					= -0.05;
				BullBrush					= Brushes.Lime;
				BearBrush					= Brushes.Red;
				NeutralBrush				= Brushes.Gray;
				
				ShowMarker					= true;
				MarkerBullish				= "⬆️";
				MarkerBearish				= "⬇️";
				MarkerOffset				= 22;
				MarkerSize					= 30;
				
				AlertFile					= @"Alert2.wav";
				
				AddPlot(NeutralBrush, "tdfi");
				AddLine(new Stroke(Brushes.Gainsboro, DashStyleHelper.Dash, 1), FilterHigh, "Filter High");
			    AddLine(new Stroke(Brushes.Gainsboro, DashStyleHelper.Dash, 1), FilterLow, "Filter Low");
			}
			else if (State == State.Configure)
			{
				tdf = new Series<double>(this);
				result = new Series<double>(this);
						
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < MMALength + 2)
            {
				return;
			}

	        mma = EMA(Close, MMALength);
			smma = EMA(mma, SmmaLength);
			
			double impetmma = mma[0] - mma[1];
			double impetsmma = smma[0] - smma[1];
			double divma = Math.Abs(mma[0] - smma[0]);
			double averimpet = (impetmma + impetsmma) / 2;
			double tdftemp = Math.Pow(divma, 1) * Math.Pow(averimpet, NLength);
			tdf[0] = Math.Abs(tdftemp);
			double highestTdf = MAX(tdf, Lookback * NLength)[0];
			
			result[0] = Math.Round(tdftemp / highestTdf, 2);
			
			if (result[0] >= FilterHigh && result[1] < FilterHigh)
			{
				if (ShowMarker == true)
				{
					Draw.Text(this, "Long "+ CurrentBar, false, MarkerBullish, 0, Low[0], - MarkerOffset, BullBrush, new NinjaTrader.Gui.Tools.SimpleFont("Arial", MarkerSize), TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 100);
				}
				
				if (string.IsNullOrEmpty(AlertFile) == false)
				{
					try
					{
						Alert("TDFI Long"+CurrentBar, Priority.High, "TDFI Long", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+AlertFile, 0, Brushes.Green, Brushes.White);  
					}
					catch (Exception e)
					{
						Print("Error playing alert file. Error message:\n" + e.Message);
					}
				}
			}
			
			if (result[0] <= FilterLow && result[1] > FilterLow)
			{
				if (ShowMarker == true)
				{
					Draw.Text(this, "Short "+ CurrentBar, false, MarkerBearish, 0, High[0], + MarkerOffset, BearBrush, new NinjaTrader.Gui.Tools.SimpleFont("Arial", MarkerSize), TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 100);
				}
				
				if (string.IsNullOrEmpty(AlertFile) == false)
				{
					try
					{
						Alert("TDFI Short"+CurrentBar, Priority.High, "TDFI Short", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+AlertFile, 0, Brushes.Red, Brushes.White);  
					}
					catch (Exception e)
					{
						Print("Error playing alert file. Error message:\n" + e.Message);
					}
				}
			}
			
			
			PlotBrushes[0][0] = result[0] >= FilterHigh ? BullBrush : result[0] <= FilterLow ? BearBrush : NeutralBrush;
            Value[0] = result[0];
			
		}
		


		#region Properties
		[NinjaScriptProperty]
		[Display(Name="Lookback", Order=1, GroupName="Parameters")]
		public int Lookback
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="MMALength", Order=2, GroupName="Parameters")]
		public int MMALength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SmmaLength", Order=3, GroupName="Parameters")]
		public int SmmaLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="NLength", Order=4, GroupName="Parameters")]
		public int NLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="FilterHigh", Order=5, GroupName="Parameters")]
		public double FilterHigh
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="FilterLow", Order=6, GroupName="Parameters")]
		public double FilterLow
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Alert sound", Order=8, Description="Clear the input if you don't want any sounds.", GroupName="Parameters")]
		public string AlertFile
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "Bull Color", GroupName="Optics", Order=1)]
		public Brush BullBrush
		{ get; set; }

		[Browsable(false)]
		public string BullBrushSerialize
		{
			get { return Serialize.BrushToString(BullBrush); }
   			set { BullBrush = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore()]
		[Display(Name = "Bear Color", GroupName="Optics", Order=2)]
		public Brush BearBrush
		{ get; set; }

		[Browsable(false)]
		public string BearBrushSerialize
		{
			get { return Serialize.BrushToString(BearBrush); }
   			set { BearBrush = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore()]
		[Display(Name = "Neutral Color", GroupName="Optics", Order=3)]
		public Brush NeutralBrush
		{ get; set; }

		[Browsable(false)]
		public string NeutralBrushSerialize
		{
			get { return Serialize.BrushToString(NeutralBrush); }
   			set { NeutralBrush = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore()]
		[Display(Name = "Show Marker", GroupName="Optics", Order=4)]
		public bool ShowMarker
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "  --> Marker Long", GroupName="Optics", Order=5)]
		public string MarkerBullish
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "  --> Marker Short", GroupName="Optics", Order=6)]
		public string MarkerBearish
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "  --> Bar Offset", GroupName="Optics", Order=7)]
		public int MarkerOffset
		{ get; set; }
		
		[XmlIgnore()]
		[Display(Name = "  --> Size", GroupName="Optics", Order=8)]
		public int MarkerSize
		{ get; set; }
		
		
		// strategy plots
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> pTDFI
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
		private TDFI[] cacheTDFI;
		public TDFI TDFI(int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			return TDFI(Input, lookback, mMALength, smmaLength, nLength, filterHigh, filterLow, alertFile);
		}

		public TDFI TDFI(ISeries<double> input, int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			if (cacheTDFI != null)
				for (int idx = 0; idx < cacheTDFI.Length; idx++)
					if (cacheTDFI[idx] != null && cacheTDFI[idx].Lookback == lookback && cacheTDFI[idx].MMALength == mMALength && cacheTDFI[idx].SmmaLength == smmaLength && cacheTDFI[idx].NLength == nLength && cacheTDFI[idx].FilterHigh == filterHigh && cacheTDFI[idx].FilterLow == filterLow && cacheTDFI[idx].AlertFile == alertFile && cacheTDFI[idx].EqualsInput(input))
						return cacheTDFI[idx];
			return CacheIndicator<TDFI>(new TDFI(){ Lookback = lookback, MMALength = mMALength, SmmaLength = smmaLength, NLength = nLength, FilterHigh = filterHigh, FilterLow = filterLow, AlertFile = alertFile }, input, ref cacheTDFI);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TDFI TDFI(int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			return indicator.TDFI(Input, lookback, mMALength, smmaLength, nLength, filterHigh, filterLow, alertFile);
		}

		public Indicators.TDFI TDFI(ISeries<double> input , int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			return indicator.TDFI(input, lookback, mMALength, smmaLength, nLength, filterHigh, filterLow, alertFile);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TDFI TDFI(int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			return indicator.TDFI(Input, lookback, mMALength, smmaLength, nLength, filterHigh, filterLow, alertFile);
		}

		public Indicators.TDFI TDFI(ISeries<double> input , int lookback, int mMALength, int smmaLength, int nLength, double filterHigh, double filterLow, string alertFile)
		{
			return indicator.TDFI(input, lookback, mMALength, smmaLength, nLength, filterHigh, filterLow, alertFile);
		}
	}
}

#endregion
