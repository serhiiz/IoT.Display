namespace IoT.Display.Devices

open System

type IDevice = 
    inherit IDisposable
    abstract member Write: byte[] -> unit