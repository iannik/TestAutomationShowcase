@allure.suite:NUnitAPITests
Feature: Restful Booker API

Background:
	Given the API client is initialized

Scenario: Get all bookings returns a non-empty list
	When the user requests all bookings
	Then the response should contain the list of bookings

Scenario: Get an existing booking returns valid booking data
	When the user requests all bookings
	And the user retrieves a random booking
	Then the booking should have a valid firstname
	And the booking should have a valid total price

Scenario: Get booking by invalid ID returns Not Found
	When the user requests a booking with ID 99999999
	Then the response status code should be 404
	And the response content should be "Not Found"

Scenario: Create a booking with valid details
	When the user creates a booking with the following details:
		| Field            | Value      |
		| First name       | John       |
		| Last name        | Doe        |
		| Total price      |        250 |
		| Deposit paid     | true       |
		| Check-in date    | 2026-08-01 |
		| Check-out date   | 2026-08-10 |
		| Additional needs | Breakfast  |
	Then the response status code should be 200
	And the created booking should contain the provided details

Scenario: Created booking can be retrieved
	When the user creates a booking with the following details:
		| Field            | Value      |
		| First name       | Alice      |
		| Last name        | Smith      |
		| Total price      |        500 |
		| Deposit paid     | false      |
		| Check-in date    | 2026-09-01 |
		| Check-out date   | 2026-09-05 |
		| Additional needs |            |
	When the user retrieves the created booking
	Then the response status code should be 200
	And the retrieved booking should contain the provided details

Scenario: Update an existing booking
	Given the user creates a booking
	When the user updates the booking with the following details:
		| Field            | Value         |
		| First name       | John_upd      |
		| Last name        | Doe_upd       |
		| Total price      |          1000 |
		| Deposit paid     | false         |
		| Check-in date    | 2032-08-01    |
		| Check-out date   | 2032-08-10    |
		| Additional needs | Breakfast_upd |
	Then the response status code should be 200
	And the updated booking should contain the provided details

Scenario: Updated booking can be retrieved
	Given the user creates a booking
	When the user updates the booking with the following details:
		| Field            | Value         |
		| First name       | John_upd      |
		| Last name        | Doe_upd       |
		| Total price      |          1000 |
		| Deposit paid     | false         |
		| Check-in date    | 2032-08-01    |
		| Check-out date   | 2032-08-10    |
		| Additional needs | Breakfast_upd |
	And the user retrieves the updated booking
	Then the response status code should be 200
	And the retrieved booking should contain the provided details

Scenario: Update a non-existing booking
	When the user updates booking with invalid ID 99999999
	Then the response status code should be 405

Scenario: Update booking without authentication
	Given the user creates a booking
	When the user updates an existing booking without authentication
	Then the response status code should be 403
	And the response content should be "Forbidden"

Scenario: Partially update an existing booking
	Given the user creates a booking
	When the user partially updates the booking with the following details:
		| Field            | Value         |
		| First name       | John_upd      |
		| Last name        | Doe_upd       |
		| Additional needs | Breakfast_upd |
	Then the response status code should be 200
	And the updated booking should contain the provided details, the remaining data should be unchanged

Scenario: Partially updated booking can be retrieved
	Given the user creates a booking
	When the user partially updates the booking with the following details:
		| Field          | Value      |
		| First name     | John_upd   |
		| Check-in date  | 2027-01-01 |
		| Check-out date | 2027-01-10 |
	And the user retrieves the updated booking
	Then the response status code should be 200
	And the retrieved booking should contain the provided details

Scenario: Partially update a non-existing booking
	When the user partially updates booking with invalid ID 99999999
	Then the response status code should be 405

Scenario: Partially update booking without authentication
	Given the user creates a booking
	When the user partially updates an existing booking without authentication
	Then the response status code should be 403
	And the response content should be "Forbidden"

Scenario: Delete an existing booking
	Given the user creates a booking
	When the user deletes the booking
	Then the response status code should be 201

Scenario: Deleted booking cannot be retrieved
	Given the user creates a booking
	When the user deletes the booking
	And the user retrieves the deleted booking
	Then the response status code should be 404

Scenario: Delete an existing booking without authentication
	Given the user creates a booking
	When the user deletes the booking without authentication
	Then the response status code should be 403
	And the response content should be "Forbidden"