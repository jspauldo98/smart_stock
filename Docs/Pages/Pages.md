# Pages Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of each web page of "Smart Stock". Since we are using Angular as our client-facing framework, each section of this markdown document will reference specific <b>components</b> of our client facing application. These components will be in their own directory, often consisting of a TS, HTML, and CSS file.  

## Sign-In Or Create Account
- Sign-In Component: Simple page with email and password material forms. Additional link with navigation to account creation page will be supplied.  
- Create Account Component: Another simple page with material forms to take email, name, DOB(You must be 18), password, and confirm password. Redirects to preferences page when user has completed. 

## Preferences
- Preferences Component: This will be a relatively complex page with various material forms and input options that will take number, string, and other data types associated with the investment preferences of the user. This page may be separated into several tabs, or will be one continuous entity. Regardless, the component code behind this page will be responsible for collecting, validating, and transferring this data back to our API. 

## Home
- Big title with project name.
- Some attractive graphics.
- Explanation of the project to a non technical audience.
- Explanation of inner workings to a non technical audience.
- Some results and metrics. With graphical representations.
- Get started button.
- Disclaimer at the bottom.

## Dashboard
This page should contain a broad view of the user's total investing experience.

- Lots of graphical representations but simple previews. Leave the intense data representations for the Analytics page.
- Users total value overtime.
- Total value.
- Percentage total gain/loss.
- Total value gain/loss.
- Some recent trades?
- Diversity Pie?

## Portfolio
This page should contain an overview of all the user's Trading Accounts.

- Attractive list of the user's Trading Accounts. Each Trading Account object in list should have:
  - Name of Trading Account.
  - Value of Trading Account.
  - Percentage gain/loss.
  - Value of gain/loss.
  - Options/settings button.
- Link/button to add a new Trading Account.
- User should be able to select a Trading Account and be redirected to that Trading Account page.
- User should be able to select a Trading Account and see its configurations.
- User should be able to select a Trading Account have the option to edit and delete that Trading Account.
  - Make sure to add extra verification to make sure the User wants to delete a Trading Account.
- User should be able to allocate how much money goes into each Trading Account.
- User should be able to navigate to the Analytics page for a certain Trading Account in the list.

## Trade Account
This page should contain specifics about a Trading Account.

- Total value of the Trading Account timeline.
- Total value of the Trading Account.
- Total gain/loss percentage of the Trading Account.
- Total value gain/loss of the Trading Account.
- History of the Trading Account.
- User should be able to edit the configurations of the Trading Account.
- User should be able to delete the Trading Account.
  - Make sure to verify the users actions here.
- User should be able to navigate to the Analytics page from here.

## Analytics
This page should contain as much data representation as we can create on a specific Trading Account.

## Author(s)
Jared Spaulding

Stefan Emmons 