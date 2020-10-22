# 1. Introduction

## 1.1 Purpose
This document lays out a project plan for the development of "Smart Stock" by Bryce Michaud, Stefan Emmons, Spencer Romberg, Jared Spaulding, and Zack Strong. Mentored by Ruben Gamboa.

## 1.2 Intentions
The indented readers of this document are current and future developers working on "Smart Stock" and the mentor(s) of the project.

The intended use of this document will be to outline every detail and component of "Smart Stock" before commencing development of anything. This will be like pseudocode, but more modularized and separated from our actual code. It will serve as the blueprints for our work to come. 

## 1.3 Scope
This planning document will include, but is not limited to:

- An overview of the system functionality.
- The scope of the project from the perspective of the "Smart Stock" Team.
- Scheduling and delivery estimates of deliverables.
- The development responsibilities for each team member.
- Project risks and how those risks will be mitigated.
- The process by which we will develop the project.

## 1.4 Definitions and Acronyms

- Trade Account: A sub account within the user's registered "Smart Stock" account. The Trade Account contains all the information related to a users trading strategy. The user can create unlimited Trade Accounts.
- Paper Trading: Simulated market trading that allows an investor to practice buying and selling without risking real money.

# 2. Overview
The stock market has been widely traded for decades by many different types of people, but consistently among each user group there has always existed the variability of emotions. All market traders face this inevitability at one point or another as they are too attached to their investment to make an emotionless decision that will impact their future holdings. Despite the explosion of commission-free trading platforms in the past decade, the lack of assistance and built-in education is astonishing. It is incredibly easy to lose money in these types of applications. Training groups have begun to grow in popularity in an attempt to mitigate the emotional attachment to the monetary value of a trade by encouraging accountability of specific trading strategies. This could and does work for extremely experienced traders who can trade in an exceptionally disciplined manor, but most people interested in trading the stock market or any market don’t have the time to become experienced traders. Since gambling has become a favorite pastime for Americans during the Coronavirus pandemic, we wanted to take advantage of this new trend to launch an interactive, and attractive tool for novice investors. This is where "Smart Stock" comes into play. 

"Smart Stock" will be developed by a team consisting of five University of Wyoming computer science students with the intention of building an attractive, easy to use, and convenient stock trading web application that removes the emotions from manual trading practices. We want to make a tool that helps individuals enjoy investing, while being able to profit with limited tending. 

## 2.1 Users
Anyone who is interested in trading the stock market and of State or regional age to participate in such practices.

## 2.2 Platform
"Smart Stock" will be launched as a Web-based application.

## 2.3 Dependencies

 - [Alpaca Trading API](https://alpaca.markets)

# 3. Goals and Scopes

- Users should be able to register an account with "Smart Stock". (See Registration Documentation)
- Users should be able to create and manage their Trade Accounts. (See Trade-Accounts Documentation)
- "Smart Stock" should use an intuitive and effective navigation system. (See Navigation Documentation)
- Each web-page should display pertinent information related to that specific page. (See Pages Documentation)
- The system should automatically make and log trades provided a Trading Account's trading parameters. (See Trading Documentation)
- The system should display graphical representations of trades and Trading Account developments. (See Graphics Documentation)

# 4. Deliverables Scheduling and Estimates

| Deadline | Modal | Description | Date Completed |
| :---------- | :---: | :---------- | -------------: |
| 10-04-2020 | Project Topic Summary Declaration | Summary of problem we are solving, scope, goals,...etc | 10-04-2020 |
| ??? | Peer Feedback on Project Description | N/A | ... |
| ??? | Midterm Status Update Document & Presentation | N/A | ... |
| ??? | Peer Feedback on Status Update Presentation | N/A | ... |
| ??? | Poster Product - Actual Poster | N/A | ... |
| ??? | Poster Presentation | N/A | ... |
| ??? | Peer Evaluation on Poster Design Draft & Practice | N/A | ... |
| ??? | Self and Team Evaluation | N/A | ... |
| 05-??-2020 | Launch "Smart Stock" | Finalize project and launch | ... |

# 5. Development Responsibility
This section will contain different aspects of the "Smart Stock" and who will be responsible for development of said aspects.

# 6. Risk Management
## 6.1 Risk Identification

- Other automatic trading platforms/brokerages already exist. What is the motivation for a user to join "Smart Stock".
- Liability for loss of capital. This is taken care of in lieu that we will be Paper Trading. 

## 6.2 Risk Mitigation
Even though the majority of users already use some sort of automatic or manual trading platform/brokerage, our platform would still offer users additional features that may not be includes within other platforms.

- Sophisticated recording and modeling of data.
- Flexibility to define specifics in trading strategies.

Thus, we believe that there is considerable differences that would attract many users.

# 7. Technical Process
We will be using an Angular client facing framework that the user will be able to interact with, and consume information from. This front end framework will communicate with a C# API backend that processes data via controllers, and writes to a database schema of our choice using repository classes. These classes will be using the ORM Dapper, or another convenient ORM like Entity Framework. We will be using a cloud based relational database management system such as MySQL or PostGres. This will likely be hosted on an AWS cloud database or similar infrastructure.

The information written to our database will be used to “activate” one of several algorithmic trading services that will make buy/sell decisions for the user, based on the investment/trading preferences a user has provided. We will be creating custom decision-making algorithms with our Python (FLASK, or Django restful service) that will leverage the Alpaca API to execute buy/sell commands. The history and performance of these executions will be recorded, and reported back to this user using a graphical interface within the UI.

A very exciting feature of this process is that a user will be able to trade with real, or fake (Paper Trading) money. We will be testing extensively with Paper Trading, before committing to true stock market trading (if that ever becomes a viable option). 