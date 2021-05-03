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
The original plan for Smart Stock was to have several trading algorithms built to satisfy a diverse array of investment interests, with a comprehensive UI surrounding the decisions that these algorithms were making. However, the trading decsions is where our biggest deviation occured. As we shifted focus away from the UI, and into the trading files, we quickly found that building decision paths for the trading files was quite complex, and ended up losing a substantial amount of money in the early testing stages. We then realized that the limitaiton of time was going to be a roadblock for our team to complete the original goalset. Since we only had a short amount of time left to collect data on our automated trading decisions, we decided to focus on short term trading algorithms only. Hence the heavy focus on Day Trading and Swing Trading. Since we had such a heavy focus on homing in on these specific strategies, attention was taken away from the UI, which left something to be desired with the final product. As a whole, we are branding our current progress in terms of the overall plan as a "beta" release, with only a few "teaser" features being ready to demonstrate and interact with.
### Design
As was mentioned in the deviations section, the "final" design of this project is focused on short term trading data. This meant that all UI components beyond the initial registration and home components were focused on displaying information related to the decisions that our Swing Trading and Day Trading algorithms were making. The database retrieval efficiency and the multithreading services surrounding these decision files was also a significant design focus, but this can only be seen from a developers point view, hence the "beta" release title.
### Limitations
Our main limitations included time constraints and manpower. The UI and database itself did not, and will not take that much time to build and improve upon. However, the automated trading workflow and the services/math that support it took a substantial amount of time to develop. On top of this, we could only test our workflow iterations during market hours (7:30 AM - 2:00 PM). Only a few team members felt comfortable taking big strides in the development of this workflow, and as a result, progress was severly bottlenecked towards the end of the semester.
### Future Direction
We now know that the careful development of our trading algorithms could take months, or even years. This will be the main development focus for future iterations of this project. In the grand scheme of the project, the UI is a quick fix to clean up and perfect, and is of least concern to us. A few team members intend to carry this project beyond the scope of this class as time permits, as even the early algorithms show profit potential through leveraging traditional buy low, sell high methodologies. We have yet to build out the capacity to short specific stocks, which would help in the bear market we have been experiencing recently. 
### Statements of Work
In addition to the following statements of work, our team kept track of work and metrics by utilization a google spreadsheet that can be viewed [in google sheets](https://docs.google.com/spreadsheets/d/1mLvM_nBxB_Ml8ZfsETkVwG1neVZg_guDkcrJSNe4y0U/edit?usp=sharing) or [in Github](https://github.com/jspauldo98/smart_stock/blob/master/Docs/SmartStock_Deliverables.csv).
#### Bryce
#### Spencer
#### Stefan
Overall hours spent: 95.2  
Admittedly, I could've spent more time on this project. However, work/life/student balance gets in the way, and I intend to continue working on this project beyond Senior Design, so my guilt is limited. 
#### Jared
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