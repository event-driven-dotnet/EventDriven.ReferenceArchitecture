@notParallel
Feature: Customer Service
	Customer Service API

Scenario: View customers
	Given customers have been created with 'customers.json'
	When I make a GET request for 'Customer' to 'api/customer'
	Then the response status code should be '200'
	And the response customers-view should be 'customers-view.json'

Scenario: View customer
	Given a customer has been created with 'customer.json'
	When I make a GET request for 'Customer' to 'api/customer/22eea083-6f0d-48f2-8c82-65ac850e5aad'
	Then the response status code should be '200'
	And the response customer-view should be 'customer-view.json'

Scenario: Create a customer
	When I make a POST request for 'Customer' with 'customer.json' to 'api/customer'
	Then the response status code should be '201'
	And the location header should be 'api/customer/22eea083-6f0d-48f2-8c82-65ac850e5aad'
	And the response customer should be 'customer.json'

Scenario: Update a customer
	Given a customer has been created with 'customer.json'
	When I make a PUT request for 'Customer' with 'updated-customer.json' to 'api/customer'
	Then the response status code should be '200'
	And the response customer should be 'updated-customer.json'

Scenario: Remove a customer
	Given a customer has been created with 'customer.json'
	When I make a DELETE request for 'Customer' with id '3fa85f64-5717-4562-b3fc-2c963f66afa6' to 'api/customer'
	Then the response status code should be '204'
