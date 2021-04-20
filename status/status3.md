# Status #3 - Mar. 31 2021

## Video Link
- https://youtu.be/yXgZmo8OUSo

## Last 3 Weeks Plan

- Achieve running a service in the background that will run for each user in parallel as per user trading preference.
- Log trades to our local database to show in UI ultimately improving the user experience.
- Determine which service will provide the best data/cost for getting stock market information (Alpaca or Polygon).
- Begin developing slightly more advanced trading algorithms, all based on short term trading. This will allow us to collect the most data on what is working and how successful the trade systems are with making short term profits. 

## Last 3 Weeks Completions (By Whom)
- Created the system that automatically and seamlessly iterates through the users preferences checking trade buy/sell statuses in the background: Stefan and Jared
- Log trades made by the Alpaca API into our database. The challenge was to separate each log based on trade accounts in a given users portfolio: Stefan
- Created the structure of how trading and analyzing will take place: Jared
- Wrote robust functions that get market data, buy, and sell: Jared
- Wrote the logic to retrieve user trade strategies and templated those strategies: Jared
- Wrote robust functions that retrieve indicator data for RSI, SMA, EMA, Volume, and MACD: Jared
- Created a standardized testing form approved by Jared and Stefan: Spencer

### Metrics

The following metrics are in terms of hours recorded on a shared spreadsheet by each member for project contributions.

- Bryce : 0
- Spencer : 2
- Stefan : 9.6
- Jared : 27.5
- Zach : 15

## Successes
- Smart Stock can now recognize when the stock market is open/closed and finally make rudimentary trades to any Alpaca paper trading account.
- Smart Stock trading algorithms are evolving to include specific EMA, SMA, RSI, and MACD functions that will become instrumental in our trade cycle tests in the following weeks.
- Smart Stock can log trades based on individual trade accounts inside a specific user’s portfolio.
- Potential introduction of Hangfire, a convenient multi-threading library that can help with asynchronous background tasks. 

## Roadblocks/Challenges
- Massive time constraints on all members. Midterms, job hunting, job responsibilities, and house hunting have all taken away from time that could be spent on Smart Stock.
- Some testing can only be done during market hours. For example when testing trade strategies none of them will trade outside of market hours.

## Changes/Plan Deviations
- Move away from Polygon.io, and solidify our position using Alpaca data client.
- May move toward Hangfire for background tasks.

## Next 3 Weeks Goals/Plans
- Develop the front end code needed to graph historical user trades.
- Refine background service creation and management.
- Create intelligent strategical algorithms based upon technical indications. (Priority algorithms include: Day trading, Scalp Trading, Swing Trading, and Blue Chip Trading).

## Completion Confidence (Per Member)
- Stefan: 5, I know the team and I will be able to gather some data on beta stage trading, but the overall product will very much be a “first launch” attempt at the end of the semester. We just do not have enough time/manpower to create the polished original vision. That being said, we’ll still be able to present something very unique and interesting.
- Spencer: 5. From what I've seen, everyone appears to be making some significant progress, and since a lot of trading strategies are being implemented, and I can finally start testing properly, I'm looking forward to things in the future.
- Bryce: 5, midterms were rough. getting back on track: outlook good.
- Zach: 5, It seems like eveything is going well. 
- Jared: 5, We achieved big milestones this iteration and Smart Stock is beginning to resemble what we had planned in the Fall.
