@notParallel
Feature: Order Service
    Order Service API

Scenario: View orders
    Given orders have been created with 'orders.json'
    When I make a GET request for 'Order' to 'api/order'
    Then the response status code should be '200'
    And the response orders-view should be 'orders-view.json'

Scenario: View orders by customer
    Given orders have been created with 'orders.json'
    When I make a GET request for 'Order' to 'api/order/customer/22eea083-6f0d-48f2-8c82-65ac850e5aad'
    Then the response status code should be '200'
    And the response orders-view should be 'orders-view.json'

Scenario: View order
    Given orders have been created with 'orders.json'
    When I make a GET request for 'Order' to 'api/order/3fa85f64-5717-4562-b3fc-2c963f66afa6'
    Then the response status code should be '200'
    And the response customer-view should be 'order-view.json'
    
Scenario: Create an order
    When I make a POST request for 'Order' with 'order.json' to 'api/order'
    Then the response status code should be '201'
    And the location header should be 'api/order/3fa85f64-5717-4562-b3fc-2c963f66afa6'
    And the response customer should be 'order.json'

Scenario: Update an order
    Given an order has been created with 'order.json'
    When I make a PUT request for 'Order' with 'updated-order.json' to 'api/order'
    Then the response status code should be '200'
    And the response customer should be 'updated-order.json'

Scenario: Ship an order
    Given an order has been created with 'order-to-ship.json'
    When I make a PUT request for 'Order' with 'order-to-ship.json' to 'api/order/ship'
    Then the response status code should be '200'
    And the response customer should be 'shipped-order.json'

Scenario: Cancel an order
    Given an order has been created with 'order-to-cancel.json'
    When I make a PUT request for 'Order' with 'order-to-cancel.json' to 'api/order/ship'
    Then the response status code should be '200'
    And the response customer should be 'cancelled-order.json'

Scenario: Remove an order
    Given an order has been created with 'order.json'
    When I make a DELETE request for 'Order' with id '22eea083-6f0d-48f2-8c82-65ac850e5aad' to 'api/order'
    Then the response status code should be '204'
