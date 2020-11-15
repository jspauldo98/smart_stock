# Trading Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of "Smart Stock" trading. Each Trading Account will be assigned a "strategy" in which to base trades off of. Strategies be be built from a multitude of variables including but not limited to time, fundamental analysis, and technical analysis. Final underlying stocks will be separated and chosen given a quantifiable ranking system.

## Definitions and Acronyms

- Fundamental Analysis: Measuring a companies "worthiness" by the overall state of the company.
- Technical Analysis: Speculation based off of trends and patterns.
- EMA: Exponential Moving Average
- MACD: Moving Average Convergence/Divergence
- RSI: Relative Strength Index
- OBV: On Balance Volume
- Volatility: The standard deviation or variance between returns from that same security or market index

## Fundamental Analysis
Will take place for strategies with a longer time frame where the value of the underlying company to be traded matters more. Some areas "Smart Stock" should take a look at include but is not limited to:

- Revenues
- Earnings
- Future Growth Estimation
- Return on Equity
- Profit Margins
- P/E Ratio

## Technical Analysis (Indicators)
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

### [RSI](https://www.investopedia.com/terms/r/rsi.asp)
Formula: 

RSI_one = 100 - [ 100 / ( 1 + ( average_gain / average_loss ) ) ]

RSI = 100 - [ 100 / ( 1 + ( ( ( prev_ave_gain * 13 ) + current_gain ) / ( ( - prev_ave_loss * 13 ) + current_loss ) ) ) ]

Above 50% means overbought. Below 50% means oversold.

Buy signal: A bullish divergence occurs when the RSI creates an oversold reading followed by a higher low that matches correspondingly lower lows in the price. This indicates rising bullish momentum, and a break above oversold territory could be used to trigger a new long position.

Sell signal: A bearish divergence occurs when the RSI creates an overbought reading followed by a lower high that matches corresponding higher highs on the price.  
  
### [Volatility](https://www.investopedia.com/terms/v/volatility.asp)  
How to calculate volatility:  
(This will be done by a machine as it will take this data from tickers that a user prefers)   
1. Find the mean of your data set. Say you have ten stock closing prices for this month. Add those prices up, and divide by how many you have.  
2. Calculate the difference between each data value, and the mean. This is known as the deviation.  
3. Square all of the deviations.  
4. Add all of the squared deviations together.  
5. Divide the sum of the squared deviations by the number of data values.  


### [OBV](https://www.investopedia.com/terms/o/onbalancevolume.asp)
### [Chaikin Oscillator](https://www.investopedia.com/terms/c/chaikinoscillator.asp)
### [Klinger Oscillator](https://www.investopedia.com/terms/k/klingeroscillator.asp)

## Strategies
Strategies will consist of zero or more combinations of the indicators listed above to justify buying and selling conditions. Each indicator will be adjusted to account for the desired time span and risk level. Some of these strategies will include, but are not limited to:  

- [Swing Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/SwingTrading.md)
- [Day Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/DayTrading.md)  
- [Long Term Investing / Blue Chip Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/LongTermTrading.md)
- [Trend Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/TrendTrading.md)
- [Scalp Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/ScalpTrading.md)
- [Sector-Specific Trading](https://github.com/jspauldo98/smart_stock/blob/master/Docs/Trading/SectorTrading.md)

## Final Execution

We will undoubtedly come across many situations where multiple tickers in the NYSE and NASDAQ BX exchanges that are within a user's selection have triggered buy/sell actions based on real-time data in their desired trading algorithm(s). For this reason, we have devised a plan to make a selection of a few tickers among that pool of options. This selection process will take a final look at the following historical attributes, before making a decision on five, or less tickers, depending on the circumstances. Quantifiable scoring will be associated with each ticker corresponding to weighted criteria. This scoring will be the deciding factor for choosing a final stock ticker. For the criteria listed below each stock will be assigned a score.

| Criteria | Weight | 
| :--- | ---: |
| The current price of the stock / the highest price of the stock traded in the last 52 weeks. | 0.2 |
| The lowest price of the stock traded in the last 52 weeks / the current price of the stock. | 0.2 |
| Current volume / average volume in the last 52 weeks. | 0.2 |
| Current price / average opening price. | 0.2 |
| Current price / average closing price. | 0.2 |
  
Depending on the results to the above calculations/questions, a selection will be made that will profit the user the most. For example, if the current price of the stock is much higher,than what the user (machine) bought it at, the volume is trailing down from it's usual average, and it meets all the criteria laid out by the trading algorithm(s), that particular ticker will be chosen. This process will be repeated for all tickers in the selection pool, and once five or less have been selected/executed, the rest will be released from the pool.
  
The amount, or quantity of shares that will be bought or sold will depend on how aggressive or passive the users preferences are. More will be bought or sold at once if the user wants to pursue an aggressive/risky account strategy, and less will be bought or sold at once if the user wishes to pursue a passive/safe account strategy.  


## Author(s)
Jared Spaulding,  

Stefan Emmons  