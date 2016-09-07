// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
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
