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

namespace NinjaTrader.NinjaScript.Indicators
{
	public class TradingFromChart : Indicator
	{
		private Account myAccount;
		private ChartScale MyChartScale;
		private bool buyButton = false;
		private bool sellButton = false;
		private Order limitOrder;
		private int myQuantity; 
		private string myATM;

		protected override void OnStateChange()
		{
			#region variables and State Changes
			if (State == State.SetDefaults)
			{
				Description									= @"Allows buying from chart with Shift + left mouse click and selling from chart with Alt + left mouse click.";
				Name										= "TradingFromChart";
				Calculate									= Calculate.OnPriceChange;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				IsSuspendedWhileInactive					= true;
				
			}
		 	else if (State == State.DataLoaded)
  			{
   				if (ChartControl != null)
    				ChartControl.MouseLeftButtonDown += LeftMouseDown;
				
				if (ChartControl != null)
					ChartPanel.MouseMove += ChartControl_MouseMove;
				
				
				Dispatcher.Invoke((() =>
				{
					ChartPanel.PreviewKeyDown += ChartPanel_PreviewKeyDown;
					ChartPanel.PreviewKeyUp += ChartPanel_PreviewKeyUp;
				}));

   			}
   			else if (State == State.Terminated)
   			{
	    		if (ChartControl != null)
	     			ChartControl.MouseLeftButtonDown -= LeftMouseDown;
				
				if (ChartControl != null)
					ChartPanel.MouseMove -= ChartControl_MouseMove;
				
				if (ChartPanel != null)
				{
					ChartPanel.PreviewKeyDown -= ChartPanel_PreviewKeyDown;
					ChartPanel.PreviewKeyUp -= ChartPanel_PreviewKeyUp;
				}
   			}
		}
		#endregion
		
	
		

		protected override void OnBarUpdate()
		{
			// nothing to do here
		}
		
		private void ChartPanel_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if ( Keyboard.IsKeyDown(Key.LeftShift))
			{
				buyButton = true;
			}
			
			if ( Keyboard.IsKeyDown(Key.LeftAlt))
			{
				sellButton = true;
			}
		}
		
		private void ChartPanel_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (Keyboard.IsKeyUp(Key.LeftShift))
			{
				buyButton = false;
			}
			
			if (Keyboard.IsKeyUp(Key.LeftAlt))
			{
				sellButton = false;
			}
			
		}
		
		protected void LeftMouseDown(object sender, MouseButtonEventArgs e)
		{
			if(buyButton == true || sellButton == true)
			{
				TriggerCustomEvent(o =>
				{
					int Y = ChartingExtensions.ConvertToVerticalPixels(e.GetPosition(ChartControl as IInputElement).Y, ChartControl.PresentationSource);
					
					double priceClicked = MyChartScale.GetValueByY(Y);
				
					// get account, quantity, and ATM of chart trader
					NinjaTrader.Gui.Tools.QuantityUpDown quantitySelector = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown);
					myQuantity = quantitySelector.Value;
					
					NinjaTrader.Gui.Tools.AccountSelector accountSelector = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlAccountSelector") as NinjaTrader.Gui.Tools.AccountSelector);
					myAccount = accountSelector.SelectedAccount;
					
					NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector atmSelector = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlATMStrategySelector") as NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector);
					myATM = atmSelector.SelectedAtmStrategy.DisplayName;
					
					if (buyButton == true)
					{
						if (priceClicked > Close[0])
						{
							limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Buy, OrderType.StopLimit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, priceClicked, "", "Entry", DateTime.MaxValue, null);
						}
						else if (priceClicked < Close[0])
						{
							limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Buy, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
						}
					}
					
					if (sellButton == true)
					{
						if (priceClicked < Close[0])
						{
							limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Sell, OrderType.StopLimit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, priceClicked, "", "Entry", DateTime.MaxValue, null);
						}
						else if (priceClicked > Close[0])
						{
							limitOrder = myAccount.CreateOrder(Instrument, OrderAction.Sell, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
						}
					}
					
					NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(myATM, limitOrder);
					myAccount.Submit(new[] { limitOrder});
					
				}, null);
				
				e.Handled = true;
			}
		}
		
		
		#region MouseMove And Render
		void ChartControl_MouseMove (object sender, System.Windows.Input.MouseEventArgs e)//DO NOT REMOVE
		{

		}
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			base.OnRender(chartControl, chartScale);
			
			MyChartScale = chartScale;
		}
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TradingFromChart[] cacheTradingFromChart;
		public TradingFromChart TradingFromChart()
		{
			return TradingFromChart(Input);
		}

		public TradingFromChart TradingFromChart(ISeries<double> input)
		{
			if (cacheTradingFromChart != null)
				for (int idx = 0; idx < cacheTradingFromChart.Length; idx++)
					if (cacheTradingFromChart[idx] != null &&  cacheTradingFromChart[idx].EqualsInput(input))
						return cacheTradingFromChart[idx];
			return CacheIndicator<TradingFromChart>(new TradingFromChart(), input, ref cacheTradingFromChart);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TradingFromChart TradingFromChart()
		{
			return indicator.TradingFromChart(Input);
		}

		public Indicators.TradingFromChart TradingFromChart(ISeries<double> input )
		{
			return indicator.TradingFromChart(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TradingFromChart TradingFromChart()
		{
			return indicator.TradingFromChart(Input);
		}

		public Indicators.TradingFromChart TradingFromChart(ISeries<double> input )
		{
			return indicator.TradingFromChart(input);
		}
	}
}

#endregion
