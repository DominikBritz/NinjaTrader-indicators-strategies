# ProperSignalBar indicator

Highlights a "proper signal bar" as taught by price action trader **Financially Free Trading** on [YouTube](https://www.youtube.com/watch?v=_U493Pl-dGg). Shows the SL, scalp, and runner targets on the chart, too.

![screenshot](https://raw.githubusercontent.com/DominikBritz/NinjaTrader-indicators-strategies/main/Indicators/ProperSignalBar/screenshot.png)

The indicator is intended to be used on a 2000 tick chart for the ES futures market.

## Signal bar definition
A proper signal shows momentum and meets the following criteria:

- A bullish signal bar has to go at least 1 tick *below* the low of the previous bar. A bearish one has to go at least 1 tick *above* the previous bar.
- A bullish signal bar must finish within 2 ticks of its high. A bearish one within 2 ticks of its low.
- The candle body is minimum 4 ticks in size.
- A bullish signal bar must close above the 21 period EMA. A bearish one below.

## Installation
Download the `ProperSignalBar.zip` file and import it in NinjaTrader: `Tools -> Import -> NinjaScript Add-On...`

## Important to understand
As per **Financially Free Trading**, the two most important things for a good trade are the market context and a proper signal bar. The latter shows momentum in the market. The indicator can help you with  identifiying a good signal bar, but it **cannot** help you to analyze to correct market context. It also does **not** count if the signal bar is a first or second entry. 

## Indicator settings
The indicator comes with a lot of options to identify a good signal bar.

### BarMaxSizeInTicks
Max. size of the bar in ticks. The settings allows filtering out to large signal bars, as the stop loss may be too big.

Default: 25

### BarMinSizeInTicks
Min. size of the bar in ticks. One wants to avoid small bars as they don't show enough momentum. See also `BarMinBodyInTicks`.

Default: 4

### BarMinBodyInTicks
The minimum size of the candle's body in ticks. One want to avoid to small candle bodies as they don't show enough momentum. See also `BarMinBodyInTicks`.

### BarCloseToHighLowInTicks
Max. number of ticks the close can be away from the high or low.

Default: 2

### BarMinTickHigherLower
Min. number of ticks the current bar must go below or above the previous one, depending on a bullish or bearish bar.

Default: 1

### BarAwayFromEMAInTicks
Max. number of ticks the low of a bar can be away from the EMA if we are bullish.
Max. number of ticks the high of a bar can be away from the EMA if we are bearish.

Default: 6

### EMALength
Period of the EMA. 

Default: 21

### TargetScalpLessTicksThanSL
The indicator plots the scalp target on the chart with a small green line. SL in ticks minus `TargetScalpLessTicksThanSL` = scalp target.

SL calculation: Bar size + 2 ticks. 2 ticks because the SL goes one tick below the bar and you enter one tick above the bar.

Default: 4

### TargetRunnerTicks
The indicator plots the runner target on the chart with a big green line. 

Default: 24
