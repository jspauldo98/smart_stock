# Day Trading Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of "Smart Stock" trading specifically the day trading strategy. The following information in regards of trading within the timespan of a day or less. It should be noted for users to keep in mind the [PDT](https://www.investopedia.com/terms/p/patterndaytrader.asp) rule when day trading.

## Algorithm (Momentum Trading):

### Fundamentals
- N/A fundumentals should not matter in short term trading.

### Technicals
Buy Signal (must include all to excecute buy):
- 180 day SMA must show a positive trend (4hr chart).
- Average daily trading volumne must be greater than 20 million.
- Price action must close above 15 day EMA (15min, 5min, or 1min chart).
- MACD must show positive crossover (15min, 5min, or 1min chart).

Sell Signal (any can excecute sell):
- MACD negative crossover.
- Price action closes below 15day EMA (whiever time frame buy was initiated in).

## Author(s)
Jared Spaulding