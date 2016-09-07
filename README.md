[![Issue Stats](http://issuestats.com/github/WilliamBerryiii/Azure.IotHub.Examples.FSharp/badge/issue)](http://issuestats.com/github//WilliamBerryiii/Azure.IotHub.Examples.FSharp)
[![Issue Stats](http://issuestats.com/github/WilliamBerryiii/Azure.IotHub.Examples.FSharp/badge/pr)](http://issuestats.com/github/WilliamBerryiii/Azure.IotHub.Examples.FSharp)

# FSharp.AspNetCore.Examples

This project has FSharp examples for the Azure IoT Hub. 

## Getting Started

1. Open the solution with Visual Studio
2. In the solution's `src\config` folder open the `config.yaml` file and set the following values:
    1. `IotHubUri`<- Azure IoT Hub Hostname
    2. `ConnectionString` <- Azure IoT Hub -> Shared Access Policies -> iothubowner -> Connection String
    3. `EndPoint` <- "messages/events"
    4. `DeviceId` <- unique name for your local test DeviceId
    5. `DeviceKey` <- Set CreateDeviceIdentity project as start-up project. Console will return a key value to use for this field.
3. Set the `ReadDeviceToCloudMessages` and `SimulatedDevice` project as startup projects by right clicking the Soluton -> Properties

## Build Status

.NET
----
[![.NET Build Status](https://img.shields.io/appveyor/ci/WilliamBerryiii/azure-iothub-examples-fsharp/master.svg)](https://ci.appveyor.com/project/WilliamBerryiii/azure-iothub-examples-fsharp)

## Maintainer(s)

- [@WilliamBerryiii](https://github.com/WilliamBerryiii)

