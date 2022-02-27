Feature: Update Customer Address
Event driven architecture for propagating data from one service to another.

@eda
Scenario: Update a customer address
	Given a customer has been created with 'customer.json'
	And orders have been created with 'orders.json'
	When I make a PUT request with 'updated-customer.json' to 'api/customer'
	Then the response status code should be '200'
	And the address for orders should equal 'updated-address.json'
