// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

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
