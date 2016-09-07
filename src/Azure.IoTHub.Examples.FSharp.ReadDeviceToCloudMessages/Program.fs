// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
// Learn more about F# at http://fsharp.org

open System
open System.Text
open System.Threading
open System.Threading.Tasks
open FSharp.Configuration
open Microsoft.ServiceBus.Messaging 

type Config = YamlConfig<FilePath="../config/config.yaml", ReadOnly=true>

[<EntryPoint>]
let main argv = 
    let config = Config()
    let connectionString = config.AzureIoTHub.ConnectionString
    let iotHubD2cEndpoint = config.AzureIoTHub.EndPoint
    
    let eventHubClient connectionString iotHubD2cEndpoint = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint)
    let eventHubReceiver (eventHubClient : EventHubClient) (partition : string) = 
        eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow)
     
    let receiveMessagesFromDeviceAsync (eventHubReceiver:EventHubReceiver) = 
        async {
            while true do
                let! result = eventHubReceiver.ReceiveAsync() |> Async.AwaitTask
                match result with 
                | null -> ()
                | _ -> 
                    let message = Encoding.UTF8.GetString(result.GetBytes())
                    printfn "Message received. Partition: %A Data: '%A'" eventHubReceiver.PartitionId message
        }

    printfn "Receive messages. Ctrl-C to exit.\n"
    
    let hubClient = eventHubClient connectionString iotHubD2cEndpoint

    let runtimeData = hubClient.GetRuntimeInformation()
    let d2cPartitions = runtimeData.PartitionIds
        
    let cts = new CancellationTokenSource()

    let tasks = d2cPartitions 
                |> Array.map (fun partition -> 
                    partition
                    |> eventHubReceiver hubClient
                    |> receiveMessagesFromDeviceAsync
                    )
    
    Async.Start(tasks |> Async.Parallel |> Async.Ignore, cts.Token)

    System.Console.CancelKeyPress.Add(fun (arg) -> 
        arg.Cancel <- true
        cts.Cancel()
        printfn "Exiting...")

    Console.ReadLine() |> ignore

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