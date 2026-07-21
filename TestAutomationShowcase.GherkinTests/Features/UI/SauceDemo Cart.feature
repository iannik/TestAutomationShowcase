@UI
@allure.suite:CartUIPlaywrightTests
Feature: SauceDemo Cart

Background:
	Given the user is on the SauceDemo login page
	And the user logs in as a "standard" user

Scenario: Adding an item to the cart updates the badge counter
	When the user adds the first item to the cart
	Then the cart badge should show 1 item

Scenario: Added item appears in the cart
	When the user adds the first item to the cart
	And the user opens the cart
	Then the cart should contain 1 item

Scenario: Removing an item leaves the cart empty
	When the user adds the first item to the cart
	And the user opens the cart
	And the user removes the first item from the cart
	Then the cart should be empty