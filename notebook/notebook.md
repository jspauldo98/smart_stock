# Notebook

## Synopsis of Project Goal(s)
The intended goal at the beginning of the project was to develop an attractive, easy to use, and convenient stock trading web application that removes the emotions from manual trading practices and incorporates a level of automation. We wanted to build a tool that would help individuals enjoy investing, while being able to profit with limited lending.

It should be noted that throughout the project's life cycle the goals shifted a bit. This is discussed below in greater detail.

## Status Updates
[Status #1 Document](https://github.com/jspauldo98/smart_stock/blob/master/status/status1.md)
[Status #2 Document](https://github.com/jspauldo98/smart_stock/blob/master/status/status2.md)
[Status #3 Document](https://github.com/jspauldo98/smart_stock/blob/master/status/status3.md)
[Status #4 Document](https://github.com/jspauldo98/smart_stock/blob/master/status/status4.md)

## Video Links
[Status #1 Video](https://youtu.be/RBhET0mhtCU)
[Status #2 Video](https://youtu.be/ej5UsSdOcvY)
[Status #3 Video](https://youtu.be/yXgZmo8OUSo)
[Status #4 Video](https://youtu.be/hAaw08ZxywU)
[Final Video](tba)

## Planning Documents
Please take the time to read through the Smart Stock [SRS](https://github.com/jspauldo98/smart_stock/blob/master/SRS.md) (Specification Requirement Sheet) and any referenced documentation within it to satisfy design requirements, specifications, and work plans.

## Summary of Final Implementation
### Deviations From Original Goal/Plan
The original plan for Smart Stock was to have several different trading algorithms the user could experiment with to satisfy a diverse array of investment interests, with a comprehensive UI surrounding the decisions that these algorithms were making. Because algorithmic trading of the stock market presents a magnitude of variables we realized time would not allow us to complete our original goal in its entirety. Due to the complexity of writing trading algorithms we decided, for the short time enrolled in senior design at the University of Wyoming, we will shift our focus away from the UI and prioritize shorter term trading strategies only. Hence the heavy focus on Day Trading and Swing Trading. Since we had such a heavy focus on homing in on these specific strategies, attention was taken away from the UI, which left something to be desired with the final product. As a whole, we are branding our current progress in terms of the overall plan as a "beta" release, with only a few "teaser" features being ready to demonstrate and interact with. In hindsight we should’ve spent the entire year honing trading algorithms and focusing less on the UI.

### Design
####User Interface
The user is able to register with Smart Stock, providing name, date of birth, email, keys for trading, and preferences for trading. When it comes to preferences the user has the option to select a risk tolerance (low, moderate, high, and aggressive), select how much equity is used per trade, and select which sectors would be traded. Given these preferences Smart Stock will generate a Trade Account (TA) that represents their preferences. Once logged in the user is able to create as many additional TAs with differing preferences for experimentation as desired. It should be noted that the user is able to edit the preferences for every TA in their Portfolio.

The Portfolio page of a user’s Smart Stock account displays an array of statistics and a graph of the user’s Portfolio’s equity that can be displayed minutely, hourly, daily, weekly, monthly, and yearly. Following is a list of cards that represent each TA with a preview of statistics and small graph preview. The user is able to select a card to review a TA in more detail. The TA’s detail page displays an array of statistics, a graph of the TA’s equity that can be displayed minutely, hourly, daily, weekly, monthly, and yearly, and a table of the TA’s trading history.

####Multithreaded Background Service
In order for an automatic trading service to work, there must be some background process running the trading algorithms. Smart Stock’s background service is multithreaded allowing for asynchronous parallel processing of all user’s preferences. 

####Trading Algorithms
As of Smart Stock beta version the only trading algorithms for user use is Day Trading, and Swing Trading. These algorithms are extremely complex and have a magnitude of variables that are subject to slight modification to increase the profitability of an algorithm. Additionally these algorithms can only trade long positions, but it is our future goal to incorporate shorting positions. 

### Limitations
Our main limitations included time constraints and manpower. The UI and database itself did not, and will not take that much time to build and improve upon. However, the automated trading workflow and the services/math that support it took a substantial amount of time to develop. On top of this, we could only test our workflow iterations during market hours (7:30 AM - 2:00 PM). Only a few team members felt comfortable taking big strides in the development of this workflow, and as a result, progress was severely bottlenecked towards the end of the semester.

### Future Direction
We now know that the careful development of our trading algorithms could take months, or even years. This will be the main development focus for future iterations of this project. In the grand scheme of the project, the UI is a quick fix to clean up and perfect, and is of least concern to us. A few team members intend to carry this project beyond the scope of this class as time permits, as even the early algorithms show profit potential through leveraging traditional buy low, sell high methodologies. We have yet to build out the capacity to short specific stocks, which would help in the bear market we have been experiencing recently. 

### Statements of Work
In addition to the following statements of work, our team kept track of work and metrics by utilization a google spreadsheet that can be viewed [in google sheets](https://docs.google.com/spreadsheets/d/1mLvM_nBxB_Ml8ZfsETkVwG1neVZg_guDkcrJSNe4y0U/edit?usp=sharing) or [in Github](https://github.com/jspauldo98/smart_stock/blob/master/Docs/SmartStock_Deliverables.csv).
#### Bryce
Overall hours spent: 49.25  
I could've put some more time and effort into this project, but balancing a multitude of responsibilities rendered it difficult to overcome the learning curve that this web-app required. All-in-all I'm satisfied with the progress that the project has seen since the start of the school year.
#### Spencer
#### Stefan
Overall hours spent: 95.2  
Admittedly, I could've spent more time on this project. However, work/life/student balance gets in the way, and I intend to continue working on this project beyond Senior Design, so my guilt is limited. 
#### Jared
Spending approximately 145.9 hours just in development of Smart Stock I accomplished the following:

- Backend C# model development/modifications needed for: logging a user in, registering a user, portfolio content, trade account content, trade account creation, trade account retrieval, log creation, and log retrieval.

- Backend C# interface and class development/modifications for communication between api and db for: user login, user registration, retrieval of portfolio content, retrieval of trade account content, creation of trade account content, trade account content edits, retrieval of logs, and tracking assets.

- Frontend Angular component/service development/modification needed for: home screen content, trade account content, portfolio content, trade account edit content, trade account create content, money transfer, and charting.

- Database development/modification.

- Background trading algorithm development/modification for: day trading.

#### Zack

## Reflection of Team
### Ability to Design, Implement, and Evaluate a Solution
As a whole, the teams ability to design and implement a solution, given our constraints, was high. We consistently found new and unique solutions to complex problems that weren't widely documented because of the secretive nature of succesful stock trading. In terms of evaluating a solution, it is hard to judge. I don't think we will ever get the chance to evaluate a true solution to our problems, as we don't really have a "solution" yet, we only have the beginning stage of a potentially succesful trading application.
### Lessons Learned
Web application development as a whole is really fun. Creating an application from scratch is very fulfilling, and tying it together with complex background processes is even more satisfying. With this being said, trading is hard. We knew this from day one. As it turns out, giving directions to a machine for trading without incorporating a "luck" factor is also very difficult. Everything took more time than we had originally planned for, and so we learned that a team should always determine what the "hard part" will be first, and tackle that to begin with. 
### "If we had to do it all over again"
Start with the trading first! This was the most complex and time-consuming part of the project, and we did not initially acnowledged this. We also should've started actually coding the first semester of Senior Design instead of planning. Every little contribution would've helped, even if it was for the UI. You can plan all you want, but this won't help account for the numerous stumbling points that a team will come across when designing a complex application such as this. 
### Advice For Future Teams
Establish team member strengths first and foremost, plan for a month or so, and then get coding! You can only foresee so much before you start "getting your hands dirty". Learning new services and frameworks takes time, and so does coding, so use what time you have to your advantage, especially if you're building a project with many moving parts!
