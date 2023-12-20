# EventDriven.ReferenceArchitecture

Reference architecture for using **Event Driven .NET** abstractions and libraries for [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) (DDD), [Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html) (CQRS) and [Event Driven Architecture](https://en.wikipedia.org/wiki/Event-driven_architecture) (EDA).

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (8.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://studio3t.com/download/):
  - Download Studio 3T only.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=dotnet-cli#install-net-aspire)
  ```
  dotnet workload update
  dotnet workload install aspire
  dotnet workload list
  ```
- [Specflow](https://specflow.org/) IDE Plugin  (recommended)
  - [Visual Studio](https://docs.specflow.org/projects/getting-started/en/latest/GettingStarted/Step1.html)
  - [JetBrains Rider](https://docs.specflow.org/projects/specflow/en/latest/Rider/rider-installation.html)

## Introduction

This project builds on the principles of [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) (DDD) to provide a set of abstractions and reference architecture for implementing the [Command Query Responsibility Segregation](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) (CQRS) pattern. By providing an event bus abstraction over [Dapr](https://dapr.io/) (Distributed Application Runtime), the reference architecture demonstrates how to apply principles of [Event Driven Architecture](https://en.wikipedia.org/wiki/Event-driven_architecture) (EDA). Because entities process commands by emitting domain events, adding [event sourcing](https://microservices.io/patterns/data/event-sourcing.html) at a later time will be relatively straightforward.

> **Note**: [EventDriven.CQRS.Abstractions](https://github.com/event-driven-dotnet/EventDriven.CQRS.Abstractions) version 2.0 or later uses [MediatR](https://github.com/jbogard/MediatR) to enable a handler per command pattern with behaviors for cross-cutting concerns.

The **Reference Architecture** projects demonstrate how to apply these concepts to two microservices: `CustomerService` and `OrderService`. In addition, each service has *separate controllers for read and write operations*, thus segregating command and query responsibilities, with different sets of models, or Data Transfer Objects (DTO's).
- **Command Controller**: Converts DTO's to domain entities using AutoMapper. Passes commands to a command broker, which selects the appropriate command handler for executing business logic.
- **Command Handlers**: Uses a domain entity to process commands which generate one or more domain events, then requests entity to apply the domain events in order to mutate entity state. Persists entity state using a repository abstraction and optionally publishes an integration event which is handled by another microservice.
- **Query Controller**: Passes queries to a query broker, which selects the appropriate query handler to retrieve entities. Converts entities to DTO's with AutoMapper.
- **Query Handlers**: Processes queries to retrieve entities using a repository abstraction.
- **Behaviors**: Used to implement cross-cutting concerns such as logging or validation.
- **Repository**: Used to persist or retrieve entities from a database.
- **Event Bus**: Used to publish integration events, as well as subscribe to events using an event handler. Dapr is used to abstract away the underlying pub/sub implementation. The default is Redis (for local development), but Dapr can be configured to use other components, such as AWS SNS+SQS.

> **Note**: This example illustrates a *simple* CQRS implementation with a **shared database** and **single service** for both read and write operations. A more sophisticated implementation might entail **separate services and databases** for read and write operations, using integration events to communicate between them. This simple example only uses integration events to communicate between the customer and order services.

<p align="center">
  <img width="600" src="images/event-driven-ref-arch.png">
</p>

## Running Services with Aspire

1. If using an IDE such as Visual Studio or Rider, Using an IDE such as Visual Studio or Rider, run the **http** profile of **ReferenceArchitecture.AppHost**.
2. Open the Aspire dashboard to inspect service endpoints and view logs.
3. Create some customers.
   - Open http://localhost:5656/swagger
   - Execute posts using contents of **customers.json**.
   - Copy post response, modify fields, then execute puts.
     - Make sure to copy `etag` value from last response, or you will get a concurrency error.
   - Copy `id` and `etag` values to execute deletes.
   - Execute gets to retrieve customers.
   - View customers database collections using Robo 3T.
4. Create some orders.
   - Open http://localhost:5757/swagger
   - Execute posts using contents of **orders.json**.
   - Copy post response, modify fields, then execute puts.
     - Make sure to copy `etag` value from last response, or you will get a concurrency error.
   - Copy `id` and `etag` values to execute deletes.
   - Execute gets to retrieve orders.
   - View orders database collections using Robo 3T.
5. Update the address of a customer who has order.
   - Note the address is also updated for the customer's orders.
   - Observe log messages in terminal when integration events are published and handled.

## Running Services with the Dapr CLI

1. Start the Customer Service using the Dapr CLI from a terminal at the project root.

    ```
    dapr run --app-id customer-service --app-port 5656 --resources-path ../dapr/components -- dotnet run
    ```

2. Start the Order Service using the Dapr CLI from a terminal at the project root.

    ```
    dapr run --app-id order-service --app-port 5757 --resources-path ../dapr/components -- dotnet run
    ```

3. Open the Dapr Dashboard at http://localhost:8080


    ```
    dapr dashboard
    ```

4. Execute steps 3-5 above to use services and pub-sub features.

## Running Tests

### Unit Tests

In the **test** folder you'll find unit tests for both **CustomerService** and **OrderService** projects.
- [xUnit](https://xunit.net/) is used as the unit testing framework.
- [Moq](https://github.com/moq/moq4) is used as the mocking framework.

> **Note**: Because database API's are notoriously [difficult to mock](https://jimmybogard.com/avoid-in-memory-databases-for-tests/), **repositories** are deliberately *excluded* from unit testing. Instead, repositories attain code coverage with **acceptance/integration tests**.

1. Run unit **CustomerService.Tests** and **OrderService.Tests** from the Test explorer in your IDE.
2. Alternatively, open a terminal at **CustomerService.Tests** and **OrderService.Tests**, then run `dotnet test`

### Acceptance (Integration) Tests

In the **tests** folder you'll find an **EventDriven.ReferenceArchitecture.Specs** project with automated acceptance / integration tests.
- [SpecFlow](https://specflow.org/) is used as the acceptance testing framework.
- Feature files use [Gherkin](https://specflow.org/learn/gherkin/) syntax to enable [Behavior Driven Development](https://en.wikipedia.org/wiki/Behavior-driven_development) with scenarios that match **acceptance criteria** in user stories.

1. Using an IDE such as Visual Studio or Rider, run the **specs** profile of **ReferenceArchitecture.AppHost**.
2. Run **EventDriven.ReferenceArchitecture.Specs** from the Test explorer of your IDE.
3. Alternatively, open a terminal at **EventDriven.ReferenceArchitecture.Specs**, then run `dotnet test`

## Development Guide

For step-by-step instructions on how to build microservices with [Event Driven .NET](https://github.com/event-driven-dotnet/Home) using this reference architecture, please see the **Event Driven .NET** [Development Guide](DevelopmentGuide.md).