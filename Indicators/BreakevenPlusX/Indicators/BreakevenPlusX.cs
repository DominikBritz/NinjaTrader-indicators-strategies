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
	public class BreakevenPlusX : Indicator
	{
		private System.Windows.Controls.RowDefinition	addedRow1, addedRow2;
		private Gui.Chart.ChartTab						chartTab;
		private Gui.Chart.Chart							chartWindow;
		private System.Windows.Controls.Grid			chartTraderGrid, chartTraderButtonsGrid, lowerButtonsGrid, upperButtonsGrid;
		private System.Windows.Controls.Button[]		buttonsArray;
		private bool									panelActive;
		private System.Windows.Controls.TabItem			tabItem;
		private NinjaTrader.Gui.Tools.AccountSelector   xAlselector;
		private NinjaTrader.Gui.Tools.InstrumentSelector xInSelector;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= @"Adds buttons to the chart trader to set the SL to breakeven plus a configurable number of ticks";
				Name						= "Breakeven Plus x Ticks";
				Calculate					= Calculate.OnBarClose;
				IsOverlay					= true;
				DisplayInDataBox			= false;
				PaintPriceMarkers			= false;
				Button1						= true;
				Button1Color				= Brushes.DimGray;
				Button1Ticks				= 1;
				Button2						= false;
				Button2Color				= Brushes.DimGray;
				Button2Ticks				= 1;
				Button3						= false;
				Button3Color				= Brushes.DimGray;
				Button3Ticks				= 1;
				Button4						= false;
				Button4Color				= Brushes.DimGray;
				Button4Ticks				= 1;
			}
			else if (State == State.Historical)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						CreateWPFControls();
					});
				}
			}
			else if (State == State.Terminated)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						DisposeWPFControls();
					});
				}
			}
		}
		
		#region Properties
		[NinjaScriptProperty]
		[Display(Name="Button1", Order=1, GroupName="Parameters")]
		public bool Button1
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Button1Color", Order=2, GroupName="Parameters")]
		public Brush Button1Color
		{ get; set; }

		[Browsable(false)]
		public string Button1ColorSerializable
		{
			get { return Serialize.BrushToString(Button1Color); }
			set { Button1Color = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Button1Ticks", Order=3, GroupName="Parameters")]
		public int Button1Ticks
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Button2", Order=4, GroupName="Parameters")]
		public bool Button2
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Button2Color", Order=5, GroupName="Parameters")]
		public Brush Button2Color
		{ get; set; }

		[Browsable(false)]
		public string Button2ColorSerializable
		{
			get { return Serialize.BrushToString(Button2Color); }
			set { Button2Color = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Button2Ticks", Order=6, GroupName="Parameters")]
		public int Button2Ticks
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Button3", Order=7, GroupName="Parameters")]
		public bool Button3
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Button3Color", Order=8, GroupName="Parameters")]
		public Brush Button3Color
		{ get; set; }

		[Browsable(false)]
		public string Button3ColorSerializable
		{
			get { return Serialize.BrushToString(Button3Color); }
			set { Button3Color = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Button3Ticks", Order=9, GroupName="Parameters")]
		public int Button3Ticks
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Button4", Order=10, GroupName="Parameters")]
		public bool Button4
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Button4Color", Order=11, GroupName="Parameters")]
		public Brush Button4Color
		{ get; set; }

		[Browsable(false)]
		public string Button4ColorSerializable
		{
			get { return Serialize.BrushToString(Button4Color); }
			set { Button4Color = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Button4Ticks", Order=12, GroupName="Parameters")]
		public int Button4Ticks
		{ get; set; }
		#endregion
		
		private void StopsToBreakeven (int Ticks)
		{
			xAlselector = Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlAccountSelector") as NinjaTrader.Gui.Tools.AccountSelector;
			string currentAccount = xAlselector.SelectedAccount.ToString();
			
			Account Acct = Account.All.FirstOrDefault(x => currentAccount.Contains(x.Name));
			
			xInSelector = Window.GetWindow(ChartControl.OwnerChart).FindFirst("ChartWindowInstrumentSelector") as NinjaTrader.Gui.Tools.InstrumentSelector;
			string currentInstrument =	xInSelector.Instrument.ToString();
			
			Position thisPosition=Acct.Positions.FirstOrDefault(x => currentInstrument.Contains(x.Instrument.FullName)); 
			foreach (Order order in Acct.Orders) 
			{
				if (thisPosition.Account == order.Account && thisPosition.Instrument == order.Instrument)
				{
					if(order.OrderType==OrderType.StopMarket || order.OrderType==OrderType.StopLimit) 
					{                
						if(order.OrderState != OrderState.Cancelled & order.OrderState != OrderState.Filled)
						{         
							Order stopOrder=order;
							if (thisPosition.MarketPosition.ToString() == "Short")
							{
                                stopOrder.StopPriceChanged = thisPosition.AveragePrice - Ticks * order.Instrument.MasterInstrument.TickSize;

                                Acct.Change(new[] { stopOrder });
							}
							if (thisPosition.MarketPosition.ToString() == "Long")
							{
                                stopOrder.StopPriceChanged = thisPosition.AveragePrice + Ticks * order.Instrument.MasterInstrument.TickSize;
                                Acct.Change(new[] { stopOrder });
							}
						} 
					}        
				}
			}        
		}

		protected void Button1Click(object sender, RoutedEventArgs e)
		{
			StopsToBreakeven (Button1Ticks);
		}

		protected void Button2Click(object sender, RoutedEventArgs e)
		{
			StopsToBreakeven (Button2Ticks);
		}

		protected void Button3Click(object sender, RoutedEventArgs e)
		{
			StopsToBreakeven (Button3Ticks);
		}

		protected void Button4Click(object sender, RoutedEventArgs e)
		{
			StopsToBreakeven (Button4Ticks);
		}

		protected void CreateWPFControls()
		{
			chartWindow				= Window.GetWindow(ChartControl.Parent) as Gui.Chart.Chart;

			// if not added to a chart, do nothing
			if (chartWindow == null)
				return;

			// this is the entire chart trader area grid
			chartTraderGrid			= (chartWindow.FindFirst("ChartWindowChartTraderControl") as Gui.Chart.ChartTrader).Content as System.Windows.Controls.Grid;

			// this grid contains the existing chart trader buttons
			chartTraderButtonsGrid	= chartTraderGrid.Children[0] as System.Windows.Controls.Grid;

			// this grid is a grid i'm adding to a new row (at the bottom) in the grid that contains bid and ask prices and order controls (chartTraderButtonsGrid)
			upperButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(upperButtonsGrid, 3);

			upperButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
			upperButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength((double)Application.Current.FindResource("MarginBase")) }); // separator column
			upperButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());

			// this grid is to organize stuff below
			lowerButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(lowerButtonsGrid, 4);

			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength((double)Application.Current.FindResource("MarginBase")) });
			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());

			// these rows will be added later, but we can create them now so they only get created once
			addedRow1	= new System.Windows.Controls.RowDefinition() { Height = new GridLength(31) };
			addedRow2	= new System.Windows.Controls.RowDefinition() { Height = new GridLength(40) };

			// this style (provided by NinjaTrader_MichaelM) gives the correct default minwidth (and colors) to make buttons appear like chart trader buttons
			Style basicButtonStyle	= Application.Current.FindResource("BasicEntryButton") as Style;

			// all of the buttons are basically the same so to save lines of code I decided to use a loop over an array
			buttonsArray = new System.Windows.Controls.Button[4];

			for (int i = 0; i < 4; ++i)
			{
				System.Windows.Media.Brush ButtonBackground;
				string ButtonTicks;
				
				switch(i) 
				{
				  case 0:
				    ButtonBackground	= Button1Color;
					ButtonTicks         = Button1Ticks.ToString(); 
				    break;
				  case 1:
				    ButtonBackground	= Button2Color;
					ButtonTicks         = Button2Ticks.ToString();
				    break;
				  case 2:
				    ButtonBackground	= Button3Color;
					ButtonTicks         = Button3Ticks.ToString();
				    break;
				  case 3:
				    ButtonBackground	= Button4Color;
					ButtonTicks         = Button4Ticks.ToString();
				    break;	
				  default:
				    ButtonBackground	= Brushes.DimGray;
                    ButtonTicks         = "1";						
				    break;
				}	
				
				buttonsArray[i]	= new System.Windows.Controls.Button()	
				{
					Content			= string.Format("BE + {0}",ButtonTicks),
					Height			= 30,
					Margin			= new Thickness(0,0,0,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};

				buttonsArray[i].BorderBrush	= Brushes.DimGray;
				buttonsArray[i].Background = ButtonBackground;
				
			}

			buttonsArray[0].Click += Button1Click;
			buttonsArray[1].Click += Button2Click;
			buttonsArray[2].Click += Button3Click;
			buttonsArray[3].Click += Button4Click;

			
			System.Windows.Controls.Grid.SetColumn(buttonsArray[1], 2);
			System.Windows.Controls.Grid.SetColumn(buttonsArray[2], 0);
			System.Windows.Controls.Grid.SetColumn(buttonsArray[3], 2);
			
			if (Button1 == true)
				upperButtonsGrid.Children.Add(buttonsArray[0]);
            
			if (Button2 == true)
                upperButtonsGrid.Children.Add(buttonsArray[1]);
			
			if (Button3 == true)
                lowerButtonsGrid.Children.Add(buttonsArray[2]);

            if (Button4 == true)
                lowerButtonsGrid.Children.Add(buttonsArray[3]);

            if (TabSelected())
				InsertWPFControls();

			chartWindow.MainTabControl.SelectionChanged += TabChangedHandler;
		}

		public void DisposeWPFControls()
		{
			if (chartWindow != null)
				chartWindow.MainTabControl.SelectionChanged -= TabChangedHandler;

			if (buttonsArray[0] != null)
				buttonsArray[0].Click -= Button1Click;
			if (buttonsArray[0] != null)
				buttonsArray[1].Click -= Button2Click;
			if (buttonsArray[0] != null)
				buttonsArray[2].Click -= Button3Click;
			if (buttonsArray[0] != null)
				buttonsArray[3].Click -= Button4Click;

			RemoveWPFControls();
		}
		
		public void InsertWPFControls()
		{
			if (panelActive)
				return;

			// add a new row (addedRow1) for upperButtonsGrid to the existing buttons grid
			chartTraderButtonsGrid.RowDefinitions.Add(addedRow1);
			// set our upper grid to that new panel
			System.Windows.Controls.Grid.SetRow(upperButtonsGrid, (chartTraderButtonsGrid.RowDefinitions.Count - 1));
			// and add it to the buttons grid
			chartTraderButtonsGrid.Children.Add(upperButtonsGrid);
			
			// add a new row (addedRow2) for our lowerButtonsGrid below the ask and bid prices and pnl display			
			chartTraderGrid.RowDefinitions.Add(addedRow2);
			System.Windows.Controls.Grid.SetRow(lowerButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(lowerButtonsGrid);

			panelActive = true;
		}

		protected override void OnBarUpdate() { }

		protected void RemoveWPFControls()
		{
			if (!panelActive)
				return;

			if (chartTraderButtonsGrid != null || upperButtonsGrid != null)
			{
				chartTraderButtonsGrid.Children.Remove(upperButtonsGrid);
				chartTraderButtonsGrid.RowDefinitions.Remove(addedRow1);
			}
			
			if (chartTraderButtonsGrid != null || lowerButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(lowerButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(addedRow2);
			}

			panelActive = false;
		}

		private bool TabSelected()
		{
			bool tabSelected = false;

			// loop through each tab and see if the tab this indicator is added to is the selected item
			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as Gui.Chart.ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					tabSelected = true;

			return tabSelected;
		}

		private void TabChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as Gui.Chart.ChartTab;
			if (chartTab == null)
				return;

			if (TabSelected())
				InsertWPFControls();
			else
				RemoveWPFControls();
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private BreakevenPlusX[] cacheBreakevenPlusX;
		public BreakevenPlusX BreakevenPlusX(bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			return BreakevenPlusX(Input, button1, button1Color, button1Ticks, button2, button2Color, button2Ticks, button3, button3Color, button3Ticks, button4, button4Color, button4Ticks);
		}

		public BreakevenPlusX BreakevenPlusX(ISeries<double> input, bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			if (cacheBreakevenPlusX != null)
				for (int idx = 0; idx < cacheBreakevenPlusX.Length; idx++)
					if (cacheBreakevenPlusX[idx] != null && cacheBreakevenPlusX[idx].Button1 == button1 && cacheBreakevenPlusX[idx].Button1Color == button1Color && cacheBreakevenPlusX[idx].Button1Ticks == button1Ticks && cacheBreakevenPlusX[idx].Button2 == button2 && cacheBreakevenPlusX[idx].Button2Color == button2Color && cacheBreakevenPlusX[idx].Button2Ticks == button2Ticks && cacheBreakevenPlusX[idx].Button3 == button3 && cacheBreakevenPlusX[idx].Button3Color == button3Color && cacheBreakevenPlusX[idx].Button3Ticks == button3Ticks && cacheBreakevenPlusX[idx].Button4 == button4 && cacheBreakevenPlusX[idx].Button4Color == button4Color && cacheBreakevenPlusX[idx].Button4Ticks == button4Ticks && cacheBreakevenPlusX[idx].EqualsInput(input))
						return cacheBreakevenPlusX[idx];
			return CacheIndicator<BreakevenPlusX>(new BreakevenPlusX(){ Button1 = button1, Button1Color = button1Color, Button1Ticks = button1Ticks, Button2 = button2, Button2Color = button2Color, Button2Ticks = button2Ticks, Button3 = button3, Button3Color = button3Color, Button3Ticks = button3Ticks, Button4 = button4, Button4Color = button4Color, Button4Ticks = button4Ticks }, input, ref cacheBreakevenPlusX);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.BreakevenPlusX BreakevenPlusX(bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			return indicator.BreakevenPlusX(Input, button1, button1Color, button1Ticks, button2, button2Color, button2Ticks, button3, button3Color, button3Ticks, button4, button4Color, button4Ticks);
		}

		public Indicators.BreakevenPlusX BreakevenPlusX(ISeries<double> input , bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			return indicator.BreakevenPlusX(input, button1, button1Color, button1Ticks, button2, button2Color, button2Ticks, button3, button3Color, button3Ticks, button4, button4Color, button4Ticks);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.BreakevenPlusX BreakevenPlusX(bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			return indicator.BreakevenPlusX(Input, button1, button1Color, button1Ticks, button2, button2Color, button2Ticks, button3, button3Color, button3Ticks, button4, button4Color, button4Ticks);
		}

		public Indicators.BreakevenPlusX BreakevenPlusX(ISeries<double> input , bool button1, Brush button1Color, int button1Ticks, bool button2, Brush button2Color, int button2Ticks, bool button3, Brush button3Color, int button3Ticks, bool button4, Brush button4Color, int button4Ticks)
		{
			return indicator.BreakevenPlusX(input, button1, button1Color, button1Ticks, button2, button2Color, button2Ticks, button3, button3Color, button3Ticks, button4, button4Color, button4Ticks);
		}
	}
}

#endregion
