@notParallel
Feature: Publish Subscribe
	Data propagation between services over an event bus abstraction layer.

Scenario: Publish customer address updated event
	Given a customer has been created with 'customer-pubsub.json'
	And orders have been created with 'orders-pubsub.json'
	When I make a PUT request for 'Customer' with 'updated-customer-pubsub.json' to 'api/customer'
	Then the response status code should be '200'
	And the address for orders should equal 'updated-address-pubsub.json'
