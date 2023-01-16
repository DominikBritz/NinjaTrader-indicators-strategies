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
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class TMABands : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.TDU.TDUTMABands TDUTMABands1;
		private bool tradeSwitch = false;
		
		// Define a Chart object to refer to the chart on which the indicator resides
		private Chart chartWindow;
		
		// Define a Button
	    private new System.Windows.Controls.Button btnTradeSwitch;
		
		private bool IsToolBarButtonAdded;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"";
				Name										= "TMABands";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.Infinite;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 100;
				
				RR					= 2;
				SLOffset			= 1;
				BandsDeviation      = 3;
				HalfLength          = 20;
				Contracts           = 1;
				TP                  = 5;
				
				Percent             = 5;
				ATRPeriod          = 100;
				
				TimeFilterStart = 10000;  // 93000 = 9:30
				TimeFilterEnd = 214500; // 200000 = 20:00
			}
			else if (State == State.Configure)
			{
				
			}
			else if (State == State.Realtime)
			{
				//Call the custom method in State.Historical or State.Realtime to ensure it is only done when applied to a chart not when loaded in the Indicators window				
				if (ChartControl != null && !IsToolBarButtonAdded)
				{
				    ChartControl.Dispatcher.InvokeAsync((Action)(() => // Use this.Dispatcher to ensure code is executed on the proper thread
				    {
						AddButtonToToolbar();
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
			else if (State == State.DataLoaded)
			{				
				TDUTMABands1				= TDUTMABands(Close, ATRPeriod, BandsDeviation, HalfLength, Percent, false);
				TDUTMABands1.Plots[0].Brush = Brushes.Pink;
				TDUTMABands1.Plots[1].Brush = Brushes.Gray;
				TDUTMABands1.Plots[2].Brush = Brushes.LightSkyBlue;
				TDUTMABands1.Plots[3].Brush = Brushes.Transparent;
				TDUTMABands1.Plots[4].Brush = Brushes.Transparent;
				TDUTMABands1.Plots[5].Brush = Brushes.Transparent;
				TDUTMABands1.Plots[6].Brush = Brushes.Transparent;
				AddChartIndicator(TDUTMABands1);
				SetProfitTarget("", CalculationMode.Ticks, TP);
				//SetStopLoss("", CalculationMode.Ticks, 25, false);
				if (tradeSwitch == false)
				{
					Draw.TextFixed(this,"infobox","Trading disabled",TextPosition.TopRight, Brushes.Red, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
					//RefreshChart();
				}
				else
				{
					Draw.TextFixed(this,"infobox","Trading enabled",TextPosition.TopRight, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
					//RefreshChart();
				}
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;
			
			if (CurrentBars[0] < BarsRequiredToTrade) return;
			
			if (Position.MarketPosition == MarketPosition.Flat && tradeSwitch == true && (ToTime(Time[0]) >= TimeFilterStart && ToTime(Time[0]) < TimeFilterEnd))
            {

				 // long
				if (TDUTMABands1.Signal[0] == 1 && Close[0] > Open[0])
				{
					var risk = (Close[0] - Low[0]) / TickSize + SLOffset;
               		//var target = RR * risk;
					EnterLong(Contracts, @"");
					SetStopLoss(CalculationMode.Ticks, risk);
               		//SetProfitTarget(CalculationMode.Ticks, target);
				}
				
				 // short
				if (TDUTMABands1.Signal[0] == -1 && Close[0] < Open[0])
				{
					var risk = (High[0] - Close[0]) / TickSize + SLOffset;
               		//var target = RR * risk;
					EnterShort(Contracts, "");
					SetStopLoss(CalculationMode.Ticks, risk);
               		//SetProfitTarget(CalculationMode.Ticks, target);
				}
			}
			
		}
		
		private void RefreshChart()
		{
			Draw.Dot(this, "dummy", false, 0, Low[0], Brushes.Red);
			RemoveDrawObject("dummy");
		}
		
		private void AddButtonToToolbar()
		{
			//Obtain the Chart on which the indicator is configured
			chartWindow = Window.GetWindow(this.ChartControl.Parent) as Chart;
	        if (chartWindow == null)
	        {
	            Print("chartWindow == null");
	            return;
	        }
			
			// Create a style to apply to the button
	        Style btnStyle = new Style();
	        btnStyle.TargetType = typeof(System.Windows.Controls.Button);
			
	        btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontSizeProperty, 11.0));
	        btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontFamilyProperty, new FontFamily("Arial")));
	        btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontWeightProperty, FontWeights.Bold));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.MarginProperty, new Thickness(2, 0, 2, 0)));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.PaddingProperty, new Thickness(4, 2, 4, 2)));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.ForegroundProperty, Brushes.WhiteSmoke));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.BackgroundProperty, Brushes.DimGray));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.IsEnabledProperty, true));
			btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.HorizontalAlignmentProperty, HorizontalAlignment.Center));
			
	        // Instantiate the buttons
	        btnTradeSwitch = new System.Windows.Controls.Button();
			
			// Set button names
			btnTradeSwitch.Content = "Toggle Trading";
							
	        // Set Button style            
	        btnTradeSwitch.Style = btnStyle;
			
			// Add the Buttons to the chart's toolbar
			chartWindow.MainMenu.Add(btnTradeSwitch);
			
			// Set button visibility
			btnTradeSwitch.Visibility = Visibility.Visible;
			
			// Subscribe to click events
			btnTradeSwitch.Click += btnTradeSwitchClick;

			// Set this value to true so it doesn't add the
			// toolbar multiple times if NS code is refreshed
	        IsToolBarButtonAdded = true;
		}
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
		}
		
		private void btnTradeSwitchClick(object sender, RoutedEventArgs e)
		{
			if (tradeSwitch == false)
			{
				tradeSwitch = true;
				Draw.TextFixed(this,"infobox","Trading enabled",TextPosition.TopRight, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
				//RefreshChart();
			}
			else
			{
				tradeSwitch = false;
				Draw.TextFixed(this,"infobox","Trading disabled",TextPosition.TopRight, Brushes.Red, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
				//RefreshChart();
			}
		}
		
		private void DisposeCleanUp()
		{
		    // remove toolbar item
            if (btnTradeSwitch != null) chartWindow.MainMenu.Remove(btnTradeSwitch);
				btnTradeSwitch.Click -= btnTradeSwitchClick;
		}
		
		
		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="RR", Order=1, GroupName="Position")]
		public int RR
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SL Offset", Order=2, GroupName="Position")]
		public int SLOffset
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Order=3, GroupName="Position")]
		public int Contracts
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TP", Order=4, GroupName="Position")]
		public int TP
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Bands deviation", Order=1, GroupName="TMA Bands")]
		public double BandsDeviation
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Half length", Order=2, GroupName="TMA Bands")]
		public int HalfLength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ATR period", Order=3, GroupName="TMA Bands")]
		public int ATRPeriod
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Percent", Order=4, GroupName="TMA Bands")]
		public int Percent
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="From", Order=1, GroupName="Misc.")]
		public int TimeFilterStart
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Until", Order=2, GroupName="Misc.")]
		public int TimeFilterEnd
		{ get; set; }
		
		#endregion
	}
}
