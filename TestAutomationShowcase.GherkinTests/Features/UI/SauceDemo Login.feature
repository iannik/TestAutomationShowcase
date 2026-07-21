@UI
@allure.suite:LoginUIPlaywrightTests
Feature: SauceDemo Login

Background:
    Given the user is on the SauceDemo login page


Scenario: Successful login redirects to Products page
    When the user logs in as a "standard" user
    Then they should be redirected to the Products page

Scenario Outline: Login fails with invalid credentials
    When the user logs in as a "<persona>" user
    Then the login error message should be displayed
    And the login error message should read "<message>"

    Examples:
        | persona          | message                                                                   |
        | locked out       | Epic sadface: Sorry, this user has been locked out.                       |
        | empty credential | Epic sadface: Username is required                                        |
        | wrong password   | Epic sadface: Username and password do not match any user in this service |