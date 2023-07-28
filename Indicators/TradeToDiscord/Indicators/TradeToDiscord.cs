#region Using declarations
using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Windows.Media.Imaging;
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
	public class TradeToDiscord : Indicator
	{
		private Account account;
		NinjaTrader.Gui.Chart.Chart chart;
		BitmapFrame outputFrame;
		private string ScreenshotPath = "";
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"";
				Name										= "TradeToDiscord";
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
				
				DiscordWebhooks					= String.Empty;
			}
			else if (State == State.Configure)
			{
				Dispatcher.InvokeAsync(new Action(() =>
                {
                    chart = Window.GetWindow(ChartControl) as Chart;
                }));
			}
			
			else if (State == State.DataLoaded)
			{
				// Find our account
				lock (Account.All)
					account = Account.All.FirstOrDefault(a => a.Name == AccountName);

				// Subscribe to account item updates
				if (account != null)
					
					account.ExecutionUpdate += OnExecutionUpdate;
			}
			else if(State == State.Terminated)
			{
				// Make sure to unsubscribe to the account item subscription
        		if (account != null)
            		
					account.ExecutionUpdate -= OnExecutionUpdate;
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
		}

		
		private void OnExecutionUpdate(object sender, ExecutionEventArgs e)
	    {
			if (string.IsNullOrEmpty(DiscordWebhooks) == false)
			{
				try
				{
					if (e.Execution.Order.OrderState == OrderState.Filled || e.Execution.Order.OrderState == OrderState.PartFilled)
					{
						string[] aDiscordWebhooks = (DiscordWebhooks.Trim()).Split(',');
						string message = string.Format("Instrument: {0} Quantity: {1} Price: {2} Name: {3} Position: {4}",e.Execution.Instrument.FullName, e.Execution.Quantity, e.Execution.Price, e.Execution.Name, e.Execution.MarketPosition);
						
						foreach (string x in aDiscordWebhooks)
						{
							string Webhook = x.Trim();
							SendToDiscord(message, Webhook);
						}
						
					}
					
					
				}
				catch (Exception err)
				{
					Print("Error sending to Discord. Error message:\n" + err.Message);
				}
			}
			


	    }
		
		static void SendToDiscord (string message, string webhook)
		{
		    WebClient client = new WebClient();
		    client.Headers.Add("Content-Type", "application/json");
		    string payload = "{\"content\": \"" + message + "\"}";
		    client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
		}
		
		#region Properties
		
		[TypeConverter(typeof(NinjaTrader.NinjaScript.AccountNameConverter))]
		public string AccountName { get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Discord webhooks", Description="One or more Discord webhooks, separated by comma.", GroupName="Parameters")]
		public string DiscordWebhooks
		{ get; set; }

		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TradeToDiscord[] cacheTradeToDiscord;
		public TradeToDiscord TradeToDiscord(string discordWebhooks)
		{
			return TradeToDiscord(Input, discordWebhooks);
		}

		public TradeToDiscord TradeToDiscord(ISeries<double> input, string discordWebhooks)
		{
			if (cacheTradeToDiscord != null)
				for (int idx = 0; idx < cacheTradeToDiscord.Length; idx++)
					if (cacheTradeToDiscord[idx] != null && cacheTradeToDiscord[idx].DiscordWebhooks == discordWebhooks && cacheTradeToDiscord[idx].EqualsInput(input))
						return cacheTradeToDiscord[idx];
			return CacheIndicator<TradeToDiscord>(new TradeToDiscord(){ DiscordWebhooks = discordWebhooks }, input, ref cacheTradeToDiscord);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TradeToDiscord TradeToDiscord(string discordWebhooks)
		{
			return indicator.TradeToDiscord(Input, discordWebhooks);
		}

		public Indicators.TradeToDiscord TradeToDiscord(ISeries<double> input , string discordWebhooks)
		{
			return indicator.TradeToDiscord(input, discordWebhooks);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TradeToDiscord TradeToDiscord(string discordWebhooks)
		{
			return indicator.TradeToDiscord(Input, discordWebhooks);
		}

		public Indicators.TradeToDiscord TradeToDiscord(ISeries<double> input , string discordWebhooks)
		{
			return indicator.TradeToDiscord(input, discordWebhooks);
		}
	}
}

#endregion
