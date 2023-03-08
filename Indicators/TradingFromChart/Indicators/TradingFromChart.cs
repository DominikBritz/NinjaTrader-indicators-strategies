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
		public override string DisplayName
		{
			get
			{
				return "Trading From Chart";
			}
		}
		private Account myAccount;
		private ChartScale MyChartScale;
		private Order myOrder;
		private int myQuantity; 
		private string myATM;
		
		private bool buyButton = false;
		private bool sellButton = false;
		
		private DesiredKey buyKey = DesiredKey.LeftShift;
		private DesiredKey sellKey = DesiredKey.LeftAlt;
		
		private Key kbuyKey;
		private Key ksellKey;
		
		private StopOrderTypes stopOrderType = StopOrderTypes.StopLimit;
		private OrderType orderType;
		
		private bool myButtonClicked = false;
		private System.Windows.Controls.Button myButton;
		private System.Windows.Controls.Grid myGrid;
		private bool tradingFromChart = false;
		


		protected override void OnStateChange()
		{

			if (State == State.SetDefaults)
			{
				Description									= @"";
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
				
				
				myButtonRightSpacing = 50;
				myButtonTopSpacing = 10;
				
				AtmStrategy = String.Empty;

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
				
				if (AtmStrategy == String.Empty)
				{
					myATM = String.Empty;
				}
				else
				{
					myATM = AtmStrategy;
				}
				
				switch (buyKey)
				{
					case DesiredKey.LeftShift:
						kbuyKey = Key.LeftShift;
						break;
					
					case DesiredKey.LeftAlt:
						kbuyKey = Key.LeftAlt;
						break;
						
					case DesiredKey.RightAlt:
						kbuyKey = Key.RightAlt;
						break;
						
					case DesiredKey.RightShift:
						kbuyKey = Key.RightShift;
						break;
				}
				
				switch (sellKey)
				{
					case DesiredKey.LeftShift:
						ksellKey = Key.LeftShift;
						break;
					
					case DesiredKey.LeftAlt:
						ksellKey = Key.LeftAlt;
						break;
						
					case DesiredKey.RightAlt:
						ksellKey = Key.RightAlt;
						break;
						
					case DesiredKey.RightShift:
						ksellKey = Key.RightShift;
						break;
				}
				
				if (stopOrderType == StopOrderTypes.StopLimit)
				{
					orderType = OrderType.StopLimit;
				}
				else
				{
					orderType = OrderType.StopMarket;
				}
				
				if (UserControlCollection.Contains(myGrid))
					return;
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top
					};
					
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					
					myButton = new System.Windows.Controls.Button
					{
						Name = "myButton", Content = "Trading from chart off", Foreground = Brushes.White, Background = Brushes.Red, Margin = new Thickness(0, myButtonTopSpacing, myButtonRightSpacing, 0)
					};
					
					myButton.Click += OnButtonClick;

					System.Windows.Controls.Grid.SetColumn(myButton, 0);

					
					myGrid.Children.Add(myButton);
					
					UserControlCollection.Add(myGrid);
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
				
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid != null)
					{
						if (myButton != null)
						{
							myGrid.Children.Remove(myButton);
							myButton.Click -= OnButtonClick;
							myButton = null;
						}
					}
				}));
   			}
		}
		
		#region AtmStrategySelector converter
        [TypeConverter(typeof(FriendlyAtmConverter))] // Converts the bool to string values
        [PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
        [Display(Name = "Atm Strategy", Order = 1, GroupName = "AtmStrategy")]
        public string AtmStrategy
        { get; set; }		
		
		
		// Since this is only being applied to a specific property rather than the whole class,
		// we don't need to inherit from IndicatorBaseConverter and can just use a generic TypeConverter
		public class FriendlyAtmConverter : TypeConverter
		{
		    // Set the values to appear in the combo box
		    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		    {
		       List<string> values = new List<string>();
				string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(NinjaTrader.Core.Globals.UserDataDir, "templates", "AtmStrategy"), "*.xml");	
				
				foreach(string atm in files)
				{
					values.Add(System.IO.Path.GetFileNameWithoutExtension(atm));
					NinjaTrader.Code.Output.Process(System.IO.Path.GetFileNameWithoutExtension(atm), PrintTo.OutputTab1);
				}

		        return new StandardValuesCollection(values);
		    }

		    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		    {
		        return value.ToString();
		    }

		    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		    {
		        return value;
		    }

		    // required interface members needed to compile
		    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		    { return true; }

		    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		    { return true; }

		    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		    { return true; }

		    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		    { return true; }
		}
		#endregion

		
		private void OnButtonClick(object sender, RoutedEventArgs rea)
		{
			//System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			
			if (myButtonClicked == false)
			{
				myButton.Content = "Trading from chart on";
				myButton.Background = Brushes.Green;
				tradingFromChart = true;
				myButtonClicked = true;
			}
			else
			{
				myButton.Content = "Trading from chart off";
				myButton.Background = Brushes.Red;
				tradingFromChart = false;
				myButtonClicked = false;
			}
		}
		

		protected override void OnBarUpdate()
		{
			// nothing to do here
		}
		
		private void ChartPanel_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if ( Keyboard.IsKeyDown(kbuyKey))
			{
				buyButton = true;
			}
			
			if ( Keyboard.IsKeyDown(ksellKey))
			{
				sellButton = true;
			}
		}
		
		private void ChartPanel_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (Keyboard.IsKeyUp(kbuyKey))
			{
				buyButton = false;
			}
			
			if (Keyboard.IsKeyUp(ksellKey))
			{
				sellButton = false;
			}
			
		}
		
		protected void LeftMouseDown(object sender, MouseButtonEventArgs e)
		{
			if((buyButton == true || sellButton == true) && tradingFromChart == true)
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
					
					if (buyButton == true)
					{
						if (priceClicked > Close[0])
						{	
							myOrder = myAccount.CreateOrder(Instrument, OrderAction.Buy, orderType, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, priceClicked, "", "Entry", DateTime.MaxValue, null);
						}
						else if (priceClicked < Close[0])
						{
							myOrder = myAccount.CreateOrder(Instrument, OrderAction.Buy, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
						}
					}
					
					if (sellButton == true)
					{
						if (priceClicked < Close[0])
						{
							myOrder = myAccount.CreateOrder(Instrument, OrderAction.Sell, orderType, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, priceClicked, "", "Entry", DateTime.MaxValue, null);
						}
						else if (priceClicked > Close[0])
						{
							myOrder = myAccount.CreateOrder(Instrument, OrderAction.Sell, OrderType.Limit, OrderEntry.Manual, TimeInForce.Day, myQuantity, priceClicked, 0, "", "Entry", DateTime.MaxValue, null);
						}
					}
					
					if (string.IsNullOrEmpty(myATM) == false)
					{
						NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(myATM, myOrder);
					}
					
					myAccount.Submit(new[] { myOrder});
					
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
		
		
		
		
		
		#region Properties
		[Display(Name="Buy hotkey", Order=1,GroupName = "Hotkeys", Description="Choose a key binding for buys")]
		public DesiredKey BuyKey
		{
			get { return buyKey; }
			set { buyKey = value; }
		}
		
		[Display(Name="Sell hotkey", Order=2,GroupName = "Hotkeys", Description="Choose a key binding for sells")]
		public DesiredKey SellKey
		{
			get { return sellKey; }
			set { sellKey = value; }
		}
		
		[Display(Name="Stop order type", Order=1,GroupName = "Order management", Description="Choose the type of stop order")]
		public StopOrderTypes StopOrderType
		{
			get { return stopOrderType; }
			set { stopOrderType = value; }
		}
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Top spacing", Order=1, GroupName="Button layout")]
		public int myButtonTopSpacing
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Right spacing", Order=2, GroupName="Button layout")]
		public int myButtonRightSpacing
		{ get; set; }
		#endregion
	}
}

public enum DesiredKey
{
	LeftAlt,
	LeftShift,
	RightAlt,
	RightShift
}

public enum StopOrderTypes
{
	StopLimit,
	StopMarket
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TradingFromChart[] cacheTradingFromChart;
		public TradingFromChart TradingFromChart(int myButtonTopSpacing, int myButtonRightSpacing)
		{
			return TradingFromChart(Input, myButtonTopSpacing, myButtonRightSpacing);
		}

		public TradingFromChart TradingFromChart(ISeries<double> input, int myButtonTopSpacing, int myButtonRightSpacing)
		{
			if (cacheTradingFromChart != null)
				for (int idx = 0; idx < cacheTradingFromChart.Length; idx++)
					if (cacheTradingFromChart[idx] != null && cacheTradingFromChart[idx].myButtonTopSpacing == myButtonTopSpacing && cacheTradingFromChart[idx].myButtonRightSpacing == myButtonRightSpacing && cacheTradingFromChart[idx].EqualsInput(input))
						return cacheTradingFromChart[idx];
			return CacheIndicator<TradingFromChart>(new TradingFromChart(){ myButtonTopSpacing = myButtonTopSpacing, myButtonRightSpacing = myButtonRightSpacing }, input, ref cacheTradingFromChart);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TradingFromChart TradingFromChart(int myButtonTopSpacing, int myButtonRightSpacing)
		{
			return indicator.TradingFromChart(Input, myButtonTopSpacing, myButtonRightSpacing);
		}

		public Indicators.TradingFromChart TradingFromChart(ISeries<double> input , int myButtonTopSpacing, int myButtonRightSpacing)
		{
			return indicator.TradingFromChart(input, myButtonTopSpacing, myButtonRightSpacing);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TradingFromChart TradingFromChart(int myButtonTopSpacing, int myButtonRightSpacing)
		{
			return indicator.TradingFromChart(Input, myButtonTopSpacing, myButtonRightSpacing);
		}

		public Indicators.TradingFromChart TradingFromChart(ISeries<double> input , int myButtonTopSpacing, int myButtonRightSpacing)
		{
			return indicator.TradingFromChart(input, myButtonTopSpacing, myButtonRightSpacing);
		}
	}
}

#endregion
