@UI
@allure.suite:CheckoutUIPlaywrightTests
Feature: SauceDemo Checkout

Background:
	Given the user is on the SauceDemo login page
	And the user logs in as a "standard" user
    And the user adds the first item to the cart
	And the user opens the cart
    And the user proceeds to the checkout

Scenario: Confirmation message is displayed after a successful checkout
    When the user enters the shipping details:
      | FirstName | LastName | PostalCode |
      | John      | Doe      |      12345 |
    And the user continues to the checkout overview
    And the user completes the checkout
    Then the checkout confirmation message should be displayed

Scenario Outline: Checkout fails with invalid shipping details
    When the user enters the shipping details:
        | FirstName   | LastName   | PostalCode   |
        | <firstName> | <lastName> | <postalCode> |
    And the user continues to the checkout overview
    Then the checkout error message should be displayed
    And the checkout error message should read "<errorMessage>"

    Examples:
        | firstName | lastName | postalCode | errorMessage                   |
        |           | Doe      |      12345 | Error: First Name is required  |
        | John      | Doe      |            | Error: Postal Code is required |
