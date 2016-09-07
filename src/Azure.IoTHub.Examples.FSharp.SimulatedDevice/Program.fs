// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
// Learn more about F# at http://fsharp.org

open System 
open System.Text
open System.Threading
open FSharp.Configuration
open Microsoft.Azure.Devices.Client
open Newtonsoft.Json

type Config = YamlConfig<FilePath="../config/config.yaml", ReadOnly=true>

type telemetryDataPoint = {
    deviceId : string 
    windSpeed : float 
    }

[<EntryPoint>]
let main argv = 
    let config = Config()
    let iotHubUri   = config.AzureIoTHub.IoTHubUri
    let deviceKey   = config.AzureIoTHub.DeviceKey
    let deviceId    = config.AzureIoTHub.DeviceId

    let deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey))

    let avgWindSpeed = 10.
    let rand = new Random()

    let windSpeedMessage = Seq.initInfinite (fun index -> 
        let telemetryReading = { deviceId = deviceId; windSpeed = (avgWindSpeed + rand.NextDouble() * 4. - 2.) }
        let json = JsonConvert.SerializeObject(telemetryReading)
        let bytes = Encoding.ASCII.GetBytes(json)
        index, new Message(bytes), json
        )

    let dataSendTask = 
        async {
            
            windSpeedMessage |> Seq.iter (fun (index, message, json) -> 
                deviceClient.SendEventAsync(message) |> Async.AwaitIAsyncResult |> Async.Ignore |> ignore
                printfn "%O > Sending message %i: %s" (DateTime.Now.ToString()) index json
                Thread.Sleep 1000
                )
        } |> Async.RunSynchronously

    printfn "%A" argv
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