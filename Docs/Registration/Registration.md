# Registration Documentation

## Summary
The purpose of this document is to lay out a development plan detailing the specifics of the User registration aspect of "Smart Stock".

## Objective
Users will register by clicking on a link "Create Account" located on the login page. Registration will require the user to input their first name, last name, date of birth, and email. Optionally the user can input their phone number for a form of two factor authentication. Next the user will be required to set a password.

## Security Concerns
Passwords will be hashed and salted before storing them into the database.

## Author(s)
Jared Spaulding 