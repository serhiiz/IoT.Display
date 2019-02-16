namespace IoT.Display.Devices.SSD1306

open IoT.Display
open IoT.Display.Devices
open System

type OnOff = 
    | On
    | Off

type MemoryAddressingMode = 
    | Horizontal
    | Vertical
    | Page
    
type SegmentRemap = 
    | COL0_SEG0
    | COL127_SEG0

type COMOutputScanDirection = 
    | Normal
    | Remapped

type Page = Page0 | Page1 | Page2 | Page3 | Page4 | Page5 | Page6 | Page7
type ScrollInterval = Frames256 | Frames128 | Frames64  | Frames25  | Frames5 | Frames3  | Frames2

type ScrollDirection = 
    | Left
    | Right
    
type HorizontalScrollConfig = {
    Direction: ScrollDirection
    StartPage: Page
    Interval: ScrollInterval
    EndPage: Page
}

type VerticalScrollConfig = {
    Offset: byte
    StartRow: byte
    Height: byte
}

type Command =
    | SetDisplay of OnOff
    | SetChargePump of OnOff
    | SetMemoryAddressingMode of MemoryAddressingMode
    | SetSegmentRemap of SegmentRemap
    | SetCOMOutputScanDirection of COMOutputScanDirection
    | SetupScroll of HorizontalScrollConfig * VerticalScrollConfig option
    | ActivateScroll
    | DeactivateScroll

type ISSD1306 = 
    inherit IDisplay
    inherit IDisposable
    abstract member SendCommand: Command -> unit

type SSD1306(device:IDevice) = 
    let memoryAddressingMode = MemoryAddressingMode.Vertical

    let getPageCode = function
        | Page0 -> 0uy
        | Page1 -> 1uy
        | Page2 -> 2uy
        | Page3 -> 3uy
        | Page4 -> 4uy
        | Page5 -> 5uy
        | Page6 -> 6uy
        | Page7 -> 7uy

    let getFrameCode = function
        | Frames256 -> 0b011uy
        | Frames128 -> 0b010uy
        | Frames64 -> 0b001uy
        | Frames25 -> 0b110uy
        | Frames5 -> 0b000uy
        | Frames3 -> 0b100uy
        | Frames2 -> 0b111uy

    let getCommandBytes = function
        | SetDisplay On -> [|0x00uy; 0xAFuy|]
        | SetDisplay Off -> [|0x00uy; 0xAEuy|]
        | SetChargePump On -> [|0x00uy; 0x8duy; 0x14uy |]
        | SetChargePump Off -> [|0x00uy; 0x8duy; 0x10uy |]
        | SetMemoryAddressingMode Horizontal -> [|0x00uy; 0x20uy; 0x00uy|]
        | SetMemoryAddressingMode Vertical -> [|0x00uy; 0x20uy; 0x01uy|]
        | SetMemoryAddressingMode Page -> [|0x00uy; 0x20uy; 0x02uy|]
        | SetSegmentRemap COL0_SEG0 -> [|0x00uy; 0xA0uy|]
        | SetSegmentRemap COL127_SEG0 -> [|0x00uy; 0xA1uy|]
        | SetCOMOutputScanDirection Normal -> [|0x00uy; 0xC0uy|]
        | SetCOMOutputScanDirection Remapped -> [|0x00uy; 0xC8uy|]
        | ActivateScroll -> [|0x00uy; 0x2Fuy|]
        | DeactivateScroll -> [|0x00uy; 0x2Euy|]
        | SetupScroll (hconfig, vconfigOption) -> 
            match vconfigOption with 
            | Some vconfig ->
                [|
                    0x00uy
                    (match hconfig.Direction with
                     | Left -> 0x2Auy
                     | Right -> 0x29uy)
                    0x00uy
                    getPageCode hconfig.StartPage
                    getFrameCode hconfig.Interval
                    getPageCode hconfig.EndPage
                    vconfig.Offset
                    0x00uy
                    0xA3uy
                    vconfig.StartRow
                    vconfig.Height
                |]
            | None ->
                [|
                    0x00uy
                    (match hconfig.Direction with
                     | Left -> 0x27uy
                     | Right -> 0x26uy)
                    0uy
                    getPageCode hconfig.StartPage
                    getFrameCode hconfig.Interval
                    getPageCode hconfig.EndPage
                    0uy
                    0xFFuy
                |]
    
    let sendCommand command =
        getCommandBytes command
        |> device.Write

    let sendData data =
        data
        |> Array.append [|0x40uy|]
        |> device.Write

    let addressingMode = 
        match memoryAddressingMode with 
                | Vertical -> ColumnMajor
                | Page
                | Horizontal -> RowMajor
    
    do SetMemoryAddressingMode memoryAddressingMode |> sendCommand 

    interface ISSD1306 with
        member __.Size = {Width = 128; Height = 64}
        member __.AddressingMode = addressingMode
            
        member __.Endian = Endian.Little
        member __.SendData(data) = sendData data
        member __.SendCommand(command) = sendCommand command
        member __.Dispose() = device.Dispose()

[<AutoOpen>]
module SSD1306 = 
    let flipVertically =
        SetCOMOutputScanDirection Remapped
    
    let flipHorizontally =
        SetSegmentRemap COL127_SEG0
    
    let setAddressingMode mode =
        SetMemoryAddressingMode mode

    let setChargePumpOn =
        SetChargePump On
    
    let turnOn = 
        SetDisplay On