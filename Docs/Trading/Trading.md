# Trading Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of "Smart Stock" trading.

## Definitions and Acronyms

- EMA: Exponential Moving Average
- MACD: Moving Average Convergence/Divergence
- RSI: Relative Strength Index
- OBV: On Balance Volume

## Indicators Used
## [Volume](https://www.investopedia.com/articles/technical/02/010702.asp)
Can be useful for determining:

- Trend Confirmation
- Exhaustion moves
- Bullish signals 
- Price Reversals
- Breakouts and False breakouts

See indicators OBV, Chaikin Oscillator, and Klinger Oscillator

### [EMA](https://www.investopedia.com/terms/e/ema.asp)
Formula: EMAToday = ( ValueToday ( Smoothing / 1 + Days ) ) + EMA_yesterday ( 1 - ( Smoothing / 1 + Days ) )

Allow the user to set the arguments for calculating EMA if they desire. Default to Smoothing = 2, Days = 20.

Buy signal: When the price action of an underlying stock crosses above the EMA line.

Sell signal: When the price action of an underlying stock crosses below the EMA line.

### [MACD](https://www.investopedia.com/terms/m/macd.asp)
Formula: (X-day EMA) - (Y-day-EMA)

Allow the user to set the arguments for calculating MACD if they desire. Default to X = 12, Y = 26.

Buy signal: When the MACD line crosses above the signal line. This will be refereed as a bullish signal.

Sell signal: When the MACD line crosses below the signal line. This will be refereed as a bearish signal.

### [RSI](https://www.investopedia.com/terms/m/macd.asp)
Formula: 

RSI_one = 100 - [ 100 / ( 1 + ( average_gain / average_loss ) ) ]

RSI = 100 - [ 100 / ( 1 + ( ( ( prev_ave_gain * 13 ) + current_gain ) / ( ( - prev_ave_loss * 13 ) + current_loss ) ) ) ]

Above 50% means overbought. Below 50% means oversold.

Buy signal: A bullish divergence occurs when the RSI creates an oversold reading followed by a higher low that matches correspondingly lower lows in the price. This indicates rising bullish momentum, and a break above oversold territory could be used to trigger a new long position.

Sell signal: A bearish divergence occurs when the RSI creates an overbought reading followed by a lower high that matches corresponding higher highs on the price.

### [OBV](https://www.investopedia.com/terms/o/onbalancevolume.asp)
### [Chaikin Oscillator](https://www.investopedia.com/terms/c/chaikinoscillator.asp)
### [Klinger Oscillator](https://www.investopedia.com/terms/k/klingeroscillator.asp)

## Strategies
Strategies will consist of zero or more combinations of the indicators listed above to justify buying and selling conditions. Each indicator will be adjusted to account for the desired time span and risk level.

## Author(s)
Jared Spaulding 