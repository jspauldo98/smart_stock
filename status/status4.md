# Status #4 - Apr. 21 2021

## Video Link
- 

## Last 3 Weeks Plan
- Develop day trading and swing trading buy/sell algorithms that utilize last weeks indicators.
- Update the values of a user's trade account and portfolio on a per trade basis.
- Add charts to the UI that represent the data from trading.
- Unit test and document the bounds of the user login system.
- Unit test and document the bounds of the user registration system.
- Unit test and document the bounds of the Portfolio page.
- Unit test and document the bounds of the Trade Account page.
- Unit test and document the bounds of trade account creation/edits.
- Unit test and document the bounds of the "money" transfers between a user's portfolio and trade accounts.

## Last 3 Weeks Completions (By Whom)
- Day trading buy and sell algorithm (Jared)

### Metrics

The following metrics are in terms of hours recorded on a shared spreadsheet by each member for project contributions.

- Bryce : 
- Spencer : 
- Stefan : 
- Jared : 
- Zach : 

## Successes
- A day trading buy/sell algorithm was written and picks decent short term assets to trade.
- When a trading algorithm reaches its maximum number of requests for an Alpaca account it waits to recover before continuing.

## Roadblocks/Challenges
- It has been difficult to test and develop portions of trading algorithms given that development and testing can only occur during market hours so for most members balancing other classes and senior design while the market is open has been difficult.
- Depending on the asset a trading algorithm is attempting to purchase the order may or may not be filled causing the algorithm to later try and sell something that was never purchased.
- With the end of the semester approaching it may be difficult to determine how well a particular trading algorithm does given only a couple weeks to trade. We are going to prioritize short term trading to hopefully acquire the most meaningful data in a short amount of time.

## Changes/Plan Deviations
- Moving to make the shorter term trading algorithms more of a priority since the end of the semester is nearing and we don't have years to collect data.

## Next 3 Weeks Goals/Plans
- The day trading algorithm loops through all assets tradable on Alpaca in a chronological order so scrambling the list or iterating in a pseudo random way will help the algorithm not favor some assets over others.
- Final touches and tweaks to the trading algorithms and UI.

## Completion Confidence (Per Member)
- Stefan:
- Spencer: 
- Bryce:
- Zach:
- Jared: 
