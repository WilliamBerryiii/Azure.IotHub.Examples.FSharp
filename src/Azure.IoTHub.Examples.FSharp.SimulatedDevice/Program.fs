// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

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
