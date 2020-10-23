# Pages Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of each web page of "Smart Stock". Since we are using Angular as our client-facing framework, each section of this markdown document will reference specific <b>components</b> of our client facing application. These components will be in their own directory, often consisting of a TS, HTML, and CSS file.  

## Sign-In Or Create Account
- Sign-In Component: Simple page with email and password material forms. Additional link with navigation to account creation page will be supplied.  
- Create Account Component: Another simple page with material forms to take email, name, DOB(You must be 18), password, and confirm password. Redirects to preferences page when user has completed. 
## Preferences
- Preferences Component: This will be a relatively complex page with various material forms and input options that will take number, string, and other data types associated with the investment preferences of the user. This page may be separated into several tabs, or will be one continuous entity. Regardless, the component code behind this page will be responsible for collecting, validating, and transferring this data back to our API. 
## Home
## Dashboard
## Trade Account

## Author(s)
Jared Spaulding
Stefan Emmons 