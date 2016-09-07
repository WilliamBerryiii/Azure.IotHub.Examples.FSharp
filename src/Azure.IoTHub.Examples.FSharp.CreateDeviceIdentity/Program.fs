// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
// Learn more about F# at http://fsharp.org

open System
open FSharp.Configuration
open Microsoft.Azure.Devices
open Microsoft.Azure.Devices.Common.Exceptions

type Config = YamlConfig<FilePath="../config/config.yaml", ReadOnly=true>

[<EntryPoint>]
let main argv = 
    let config = Config()
    let connectionString = config.AzureIoTHub.ConnectionString
    let deviceId = config.AzureIoTHub.DeviceId

    let printDeviceKey (device: Device) = printfn "Generated device key: %A" device.Authentication.SymmetricKey.PrimaryKey

    let registryManager = RegistryManager.CreateFromConnectionString(connectionString)

    let addDevice deviceId = 
        registryManager.AddDeviceAsync(new Device(deviceId))

    let getDevice deviceId =  
        registryManager.GetDeviceAsync(deviceId)
 
    try 
        addDevice deviceId |> Async.AwaitTask |> Async.RunSynchronously |> printDeviceKey
    with 
    | :? System.AggregateException as e ->
        e.InnerExceptions 
        |> Seq.iter (fun ex -> 
            if ex :? DeviceAlreadyExistsException then 
                getDevice deviceId |> Async.AwaitTask |> Async.RunSynchronously |> printDeviceKey
            )


    let console = Console.ReadLine()

    0 // return an integer exit code


//*********************************************************
//
//Azure.IotHub.Examples.FSharp, https://github.com/WilliamBerryiii/Azure.IotHub.Examples.FSharp
//
//Copyright (c) Microsoft Corporation
//All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:




// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.




// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************