# Reference Architecture: User Acceptance Tests

SpecFlow tests for EventDriven.ReferenceArchitecture.

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (6.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://robomongo.org/)
  - Download Studio 3T.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=dotnet-cli#install-net-aspire)
- [Specflow](https://specflow.org/) IDE Plugin  (recommended)
  - [Visual Studio](https://docs.specflow.org/projects/getting-started/en/latest/GettingStarted/Step1.html)
  - [JetBrains Rider](https://docs.specflow.org/projects/specflow/en/latest/Rider/rider-installation.html)

## Usage

1. Using an IDE such as Visual Studio or Rider, run the **specs** profile of **ReferenceArchitecture.AppHost**.
2. Run **EventDriven.ReferenceArchitecture.Specs** from the Test explorer of your IDE.
3. Alternatively, open a terminal at **EventDriven.ReferenceArchitecture.Specs**, then run `dotnet test`

