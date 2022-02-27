# Reference Architecture: User Acceptance Tests

SpecFlow tests for EventDriven.ReferenceArchitecture.

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (6.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://robomongo.org/download)
  - Download Robo 3T only.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)
- [Microsoft Tye](https://github.com/dotnet/tye/blob/main/docs/getting_started.md) (recommended)
- [Specflow](https://specflow.org/) IDE Plugin  (recommended)
  - [Visual Studio](https://docs.specflow.org/projects/getting-started/en/latest/GettingStarted/Step1.html)
  - [JetBrains Rider](https://docs.specflow.org/projects/specflow/en/latest/Rider/rider-installation.html)

## Usage

### Option 1: Run Tye independently (recommended)

1. Open **appsettings.json** and set `StartTyeProcess` to `false`.
2. Run Tye from a terminal at the SpecFlow project root.
    ```
    tye run
    ```
3. Alternatively, run Tye in debug mode.
    ```
    tye run --debug *
    ```
    - Set breakpoints in **OrderService**, **CustomerService**.
    - Attach the IDE debugger to **OrderService.dll**, **CustomerService.dll**
4. Run the SpecFlow tests using the IDE test runner.
   - You should hit breakpoints in each service.

### Option 2: Run Tye with SpecFlow

1. Open **appsettings.json** and set `StartTyeProcess` to `true`.
2. Run the SpecFlow tests using the IDE test runner.
3. Alternatively, run tests from a terminal at the SpecFlow project root.
    ```
    dotnet test
    ```

