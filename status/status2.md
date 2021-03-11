# Status #2 - Mar. 10 2021

## Video Link
- https://youtu.be/ej5UsSdOcvY

## Last 3 Weeks Plan

- Homepage refinements including but not limited to, navbar/toolbar functionality and summary services surrounding those features.
- Basic construction of trading scripts with activation based on initial user preference submission.
- Portfolio data retrieval and view generation.
- Dotnet Core Alpaca SDK files are writing to DB.

## Last 3 Weeks Completions (By Whom)
- Frontend and backend development supporting new features such as: Portfolio content, trade-account list, trade-account view, trade-account creation, trade-account edits, trade-account history list, and money transfers. (Jared)
- Alpaca Dotnet Core SDK interfacing with our project. This begins with getting our users signed up with alpaca, and setting themselves up with API keys, which are then given to us. From here, classes, interfaces, controller functions, and background threading is initiated to begin constant trading logic. First step has been getting a simple mean reversion paper trading file running, which is working, beyond some live data client fetching issues. (Stefan)

### Metrics

The following metrics are in terms of hours recorded on a shared spreadsheet by each member for project contributions.

- Bryce : 15
- Spencer : 2.2
- Stefan : 27.9
- Jared : 64.4
- Zach : 36.2

## Successes
- Completing the shell of the UI, and preparing it for live data from alpaca trading files.
- Finding an easier way to integrate alpaca into our project, beyond using python.
- Setting up the stage and first attempt for background workers handling constant trade logic after certain events (like registration) have been completed. Makes it so even when our app is closed, trades still happen.

## Roadblocks/Challenges
- Examples for Dotnet Core trading logic in alpaca are dated, they have undergone several updates that we need to account for.
- Background multithreading is dangerous, we have needed to test and create carefully in order to save our machines from deadlock.
- Heavy class/work load on all members means that time is limited.

## Changes/Plan Deviations
- Move away from Python scrips, focus more of native dotnet core trading files.

## Next 3 Weeks Goals/Plans
- Fine tune trading logic/create other trading files.
- Get up to speed with the numerous alpaca updates that lack documentation.
- Get preferences more involved in which files are chosen to run.
- Get lots of paper trade data stored in our DB and get ready to JSON-ify that data for the client.

## Completion Confidence (Per Member)
- Stefan: 5, Extremely confident with any changes that need to be made for the UI, very confident and happy with the progress made on the API, would like to see several files working in unison after debugging alpaca data client troubles. 
- Spencer:
- Bryce:
- Zach: 5 
- Jared: 4, Awesome progress with the UI and API. Great progress getting the Alpaca SDK to interface with dotnet, however we did not meet our goal of getting a simple paper trade working. Granted there has been some issues with trade implementation. The main reason I give a four for completion confidence and not a five because a significantly larger amount of hours were logged this status update than last update without a whole lot to show for the time spent.
