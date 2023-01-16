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
	public class ZoomWithMouse : Indicator
	{
		private Chart chartWindow;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Zooms the chart by holding Ctrl und using the mouse wheel";
				Name										= "Zoom With Mouse";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= false;
				DrawVerticalGridLines						= false;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.Realtime)
			{
				//Call the custom method in State.Historical or State.Realtime to ensure it is only done when applied to a chart not when loaded in the Indicators window				
				if (ChartControl != null)
				{
				    ChartControl.Dispatcher.InvokeAsync((Action)(() => // Use this.Dispatcher to ensure code is executed on the proper thread
				    {
						AddIndicator();
					}));
				}
			}
			else if (State == State.Terminated)
			{
				if (chartWindow != null)
				{
			        ChartControl.Dispatcher.InvokeAsync((Action)(() => //Dispatcher used to Assure Executed on UI Thread
			        {	
						DisposeCleanUp();
					}));
				}
			}
		}
		
		private void AddIndicator()
		{
			//Obtain the Chart on which the indicator is configured
			chartWindow = Window.GetWindow(this.ChartControl.Parent) as Chart;
	        if (chartWindow == null)
	        {
	            Print("chartWindow == null");
	            return;
	        }
			
			// subscribe chartwindow to keypress events
			if (chartWindow != null)
			{			
				chartWindow.PreviewMouseWheel += OnMouseWheel;
			}
		}

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
		    if (chartWindow.ActiveChartControl != null && ChartBars != null)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					if (e.Delta < 0)
					{
						chartWindow.ActiveChartControl.Properties.BarDistance = (float)(chartWindow.ActiveChartControl.Properties.BarDistance * 0.9);
						chartWindow.ActiveChartControl.BarWidth = chartWindow.ActiveChartControl.BarWidth * 0.9;
					}
					else if (e.Delta > 0)
					{
						chartWindow.ActiveChartControl.Properties.BarDistance = (float)(chartWindow.ActiveChartControl.Properties.BarDistance / 0.9);
						chartWindow.ActiveChartControl.BarWidth = chartWindow.ActiveChartControl.BarWidth / 0.9;
					}			
					e.Handled = true;
					chartWindow.ActiveChartControl.InvalidateVisual();
					ForceRefresh();
				}
			}
		}
		
		private void DisposeCleanUp()
		{
			chartWindow.PreviewMouseWheel -= OnMouseWheel;
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ZoomWithMouse[] cacheZoomWithMouse;
		public ZoomWithMouse ZoomWithMouse()
		{
			return ZoomWithMouse(Input);
		}

		public ZoomWithMouse ZoomWithMouse(ISeries<double> input)
		{
			if (cacheZoomWithMouse != null)
				for (int idx = 0; idx < cacheZoomWithMouse.Length; idx++)
					if (cacheZoomWithMouse[idx] != null &&  cacheZoomWithMouse[idx].EqualsInput(input))
						return cacheZoomWithMouse[idx];
			return CacheIndicator<ZoomWithMouse>(new ZoomWithMouse(), input, ref cacheZoomWithMouse);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ZoomWithMouse ZoomWithMouse()
		{
			return indicator.ZoomWithMouse(Input);
		}

		public Indicators.ZoomWithMouse ZoomWithMouse(ISeries<double> input )
		{
			return indicator.ZoomWithMouse(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ZoomWithMouse ZoomWithMouse()
		{
			return indicator.ZoomWithMouse(Input);
		}

		public Indicators.ZoomWithMouse ZoomWithMouse(ISeries<double> input )
		{
			return indicator.ZoomWithMouse(input);
		}
	}
}

#endregion
