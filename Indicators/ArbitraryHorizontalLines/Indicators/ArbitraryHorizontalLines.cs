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
	public class ArbitraryHorizontalLines : Indicator
	{
		
		private DashStyleHelper	myDashStyle1  = DashStyleHelper.Solid;
		private DashStyleHelper	myDashStyle2  = DashStyleHelper.Solid;
		private DashStyleHelper	myDashStyle3  = DashStyleHelper.Solid;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"";
				Name										= "ArbitraryHorizontalLines";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Prices1					= string.Empty;
				Color1					= Brushes.Goldenrod;
				Width1					= 6;
				Prices2					= string.Empty;
				Color2					= Brushes.Green;
				Width2					= 6;
				Prices3					= string.Empty;
				Color3					= Brushes.Red;
				Width3					= 6;
			}
			else if (State == State.Transition)
			{
				DrawLines();
			}
		}
		
		private void DrawLines()
		{
			if (string.IsNullOrEmpty(Prices1) == false)
			{
				string[] aPrices1 = (Prices1.Trim()).Split(';');
				foreach (string x in aPrices1)
				{
					string Price = x.Trim();
					double iPrice;
					iPrice = double.Parse(Price, System.Globalization.CultureInfo.InvariantCulture);
					HorizontalLine myHline = Draw.HorizontalLine(this, Price, iPrice, true, "");
					myHline.Stroke = new Stroke(Color1, MyDashStyle1, Width1);
				}
			}
			
			if (string.IsNullOrEmpty(Prices2) == false)
			{
				string[] aPrices2 = (Prices2.Trim()).Split(';');
				foreach (string x in aPrices2)
				{
					string Price = x.Trim();
					double iPrice;
					iPrice = double.Parse(Price, System.Globalization.CultureInfo.InvariantCulture);
					HorizontalLine myHline = Draw.HorizontalLine(this, Price, iPrice, true, "");
					myHline.Stroke = new Stroke(Color2, myDashStyle2, Width2);
				}
			}
			
			if (string.IsNullOrEmpty(Prices3) == false)
			{
				string[] aPrices3 = (Prices3.Trim()).Split(';');
				foreach (string x in aPrices3)
				{
					string Price = x.Trim();
					double iPrice;
					iPrice = double.Parse(Price, System.Globalization.CultureInfo.InvariantCulture);
					HorizontalLine myHline = Draw.HorizontalLine(this, Price, iPrice, true, "");
					myHline.Stroke = new Stroke(Color3, myDashStyle3, Width3);
				}
			}
		}

		protected override void OnBarUpdate()
		{
			
			// nothing to do here
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Display(Name="Prices", Order=1, GroupName="Group 1", Description="Enter prices comma separated without any spaces")]
		public string Prices1
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Color", Order=2, GroupName="Group 1")]
		public Brush Color1
		{ get; set; }

		[Browsable(false)]
		public string Color1Serializable
		{
			get { return Serialize.BrushToString(Color1); }
			set { Color1 = Serialize.StringToBrush(value); }
		}			
		
		[NinjaScriptProperty]
		[Display(Name="Width", Order=3, GroupName="Group 1")]
		public int Width1
		{ get; set; }
		
		[Display(ResourceType = typeof(Custom.Resource), Name = "Style", Description = "", GroupName = "Group 1", Order = 4)]
		public DashStyleHelper MyDashStyle1				        
		{
			get { return myDashStyle1; }
			set { myDashStyle1 = value; }
		}

		[NinjaScriptProperty]
		[Display(Name="Prices", Order=1, GroupName="Group 2", Description="Enter prices comma separated without any spaces")]
		public string Prices2
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Color", Order=2, GroupName="Group 2")]
		public Brush Color2
		{ get; set; }

		[Browsable(false)]
		public string Color2Serializable
		{
			get { return Serialize.BrushToString(Color2); }
			set { Color2 = Serialize.StringToBrush(value); }
		}		
		
		[NinjaScriptProperty]
		[Display(Name="Width", Order=3, GroupName="Group 2")]
		public int Width2
		{ get; set; }
		
		[Display(ResourceType = typeof(Custom.Resource), Name = "Style", Description = "", GroupName = "Group 2", Order = 4)]
		public DashStyleHelper MyDashStyle2				        
		{
			get { return myDashStyle2; }
			set { myDashStyle2 = value; }
		}

		[NinjaScriptProperty]
		[Display(Name="Prices", Order=1, GroupName="Group 3", Description="Enter prices comma separated without any spaces")]
		public string Prices3
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Color", Order=2, GroupName="Group 3")]
		public Brush Color3
		{ get; set; }

		[Browsable(false)]
		public string Color3Serializable
		{
			get { return Serialize.BrushToString(Color3); }
			set { Color3 = Serialize.StringToBrush(value); }
		}		
		
		[NinjaScriptProperty]
		[Display(Name="Width", Order=3, GroupName="Group 3")]
		public int Width3
		{ get; set; }
		
		[Display(ResourceType = typeof(Custom.Resource), Name = "Style", Description = "", GroupName = "Group 3", Order = 4)]
		public DashStyleHelper MyDashStyle3				        
		{
			get { return myDashStyle3; }
			set { myDashStyle3 = value; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ArbitraryHorizontalLines[] cacheArbitraryHorizontalLines;
		public ArbitraryHorizontalLines ArbitraryHorizontalLines(string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			return ArbitraryHorizontalLines(Input, prices1, color1, width1, prices2, color2, width2, prices3, color3, width3);
		}

		public ArbitraryHorizontalLines ArbitraryHorizontalLines(ISeries<double> input, string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			if (cacheArbitraryHorizontalLines != null)
				for (int idx = 0; idx < cacheArbitraryHorizontalLines.Length; idx++)
					if (cacheArbitraryHorizontalLines[idx] != null && cacheArbitraryHorizontalLines[idx].Prices1 == prices1 && cacheArbitraryHorizontalLines[idx].Color1 == color1 && cacheArbitraryHorizontalLines[idx].Width1 == width1 && cacheArbitraryHorizontalLines[idx].Prices2 == prices2 && cacheArbitraryHorizontalLines[idx].Color2 == color2 && cacheArbitraryHorizontalLines[idx].Width2 == width2 && cacheArbitraryHorizontalLines[idx].Prices3 == prices3 && cacheArbitraryHorizontalLines[idx].Color3 == color3 && cacheArbitraryHorizontalLines[idx].Width3 == width3 && cacheArbitraryHorizontalLines[idx].EqualsInput(input))
						return cacheArbitraryHorizontalLines[idx];
			return CacheIndicator<ArbitraryHorizontalLines>(new ArbitraryHorizontalLines(){ Prices1 = prices1, Color1 = color1, Width1 = width1, Prices2 = prices2, Color2 = color2, Width2 = width2, Prices3 = prices3, Color3 = color3, Width3 = width3 }, input, ref cacheArbitraryHorizontalLines);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ArbitraryHorizontalLines ArbitraryHorizontalLines(string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			return indicator.ArbitraryHorizontalLines(Input, prices1, color1, width1, prices2, color2, width2, prices3, color3, width3);
		}

		public Indicators.ArbitraryHorizontalLines ArbitraryHorizontalLines(ISeries<double> input , string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			return indicator.ArbitraryHorizontalLines(input, prices1, color1, width1, prices2, color2, width2, prices3, color3, width3);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ArbitraryHorizontalLines ArbitraryHorizontalLines(string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			return indicator.ArbitraryHorizontalLines(Input, prices1, color1, width1, prices2, color2, width2, prices3, color3, width3);
		}

		public Indicators.ArbitraryHorizontalLines ArbitraryHorizontalLines(ISeries<double> input , string prices1, Brush color1, int width1, string prices2, Brush color2, int width2, string prices3, Brush color3, int width3)
		{
			return indicator.ArbitraryHorizontalLines(input, prices1, color1, width1, prices2, color2, width2, prices3, color3, width3);
		}
	}
}

#endregion
