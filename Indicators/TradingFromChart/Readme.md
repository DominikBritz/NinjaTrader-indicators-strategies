# TradingFromChart indicator
Allows placing orders with a keyboard + mouse combination on a NinjaTrader chart, without having to use NinjaTrader's fiddly right click menu.

![example_gif](TradingFromChartExample.gif)

# Requirements
- NinjaTrader 8

# Installation
1. Download the `TradingFromChart.zip` file 
2. Import it in NinjaTrader: `Tools -> Import -> NinjaScript Add-On...`
3. Add the indicator *TradingFromChart* to the chart

# Usage
## Buy
```
Left shift + left mouse
```
- Creates a buy limit order when below price
- Create a stop buy limit order when above price

## Sell
```
Left alt + left mouse
```
- Creates a sell limit order when above price
- Creates a stop sell limit order when below price

## Account and ATM
The indicator automatically uses the account and ATM strategy you have set in chart trader.

# Credits
Based on this NinjaTrader [support forum post by Mindset](https://ninjatrader.com/support/forum/forum/suggestions-and-feedback/suggestions-and-feedback-aa/1145221-chart-trading-from-charts-with-one-click?p=1206610#post1206610)
