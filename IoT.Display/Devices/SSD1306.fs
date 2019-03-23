namespace IoT.Display.Devices.SSD1306

open IoT.Display
open IoT.Display.Devices
open IoT.Display.Graphics
open System

type OnOff = 
    | On
    | Off

type MemoryAddressingMode = 
    | Horizontal
    | Vertical
    
type SegmentRemap = 
    | COL0_SEG0
    | COL127_SEG0

type COMOutputScanDirection = 
    | Normal
    | Remapped

type COMPinConfiguration = 
    | Sequential
    | Alternative

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

type VCOMDeselectLevel = 
    | VCC_065
    | VCC_077
    | VCC_083

type Command =
    | SetDisplay of OnOff
    | SetChargePump of OnOff
    | SetMemoryAddressingMode of MemoryAddressingMode
    | SetSegmentRemap of SegmentRemap
    | SetCOMOutputScanDirection of COMOutputScanDirection
    | SetupScroll of HorizontalScrollConfig * VerticalScrollConfig option
    | ActivateScroll
    | DeactivateScroll
    | SetContrastControl of byte
    | EntireDisplayOn of bool
    | InverseDisplay of bool
    | SetColumnAddress of startAddress:byte * endAddress:byte
    | SetPageAddress of startPage:Page * endPage:Page
    | SetDisplayStartLine of byte
    | SetMultiplexRatio of byte
    | SetDisplayOffset of byte
    | SetCOMPinsHardwareConfiguration of COMPinConfiguration * enableCOMLeftRightRemap:bool
    | SetDisplayClockDivideRatioOscilatorFrequency of divideRatio:byte * oscillatorFrequency:byte
    | SetPreChargePeriod of phase1:byte * phase2:byte
    | SetVCOMHDeselectLevel of VCOMDeselectLevel
    | NOP

type ISSD1306 = 
    inherit IDisplay
    inherit IDisposable
    abstract member SendCommand: Command -> unit

type SSD1306(device:IDevice) as this = 
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
        | SetContrastControl c -> [| 0x00uy; 0x81uy; c |]
        | EntireDisplayOn false -> [| 0x00uy; 0xA4uy |]
        | EntireDisplayOn true -> [| 0x00uy; 0xA5uy |]
        | InverseDisplay false -> [| 0x00uy; 0xA6uy |]
        | InverseDisplay true -> [| 0x00uy; 0xA7uy |]
        | SetDisplay On -> [|0x00uy; 0xAFuy|]
        | SetDisplay Off -> [|0x00uy; 0xAEuy|]
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
        | DeactivateScroll -> [|0x00uy; 0x2Euy|]
        | ActivateScroll -> [|0x00uy; 0x2Fuy|]
        | SetMemoryAddressingMode Horizontal -> [|0x00uy; 0x20uy; 0x00uy|]
        | SetMemoryAddressingMode Vertical -> [|0x00uy; 0x20uy; 0x01uy|]
        | SetColumnAddress (startAddress, endAddress) -> [|0x00uy; 0x21uy; startAddress &&& 0x7Fuy; endAddress &&& 0x7Fuy|]
        | SetPageAddress (startPage, endPage) -> [|0x00uy; 0x22uy; getPageCode startPage; getPageCode endPage|]
        | SetDisplayStartLine startLine -> [|0x00uy; (startLine &&& 0x3Fuy) ||| 0x40uy |]
        | SetSegmentRemap COL0_SEG0 -> [|0x00uy; 0xA0uy|]
        | SetSegmentRemap COL127_SEG0 -> [|0x00uy; 0xA1uy|]
        | SetMultiplexRatio n -> [|0x00uy; 0xA8uy; n &&& 0x3Fuy|]
        | SetCOMOutputScanDirection Normal -> [|0x00uy; 0xC0uy|]
        | SetCOMOutputScanDirection Remapped -> [|0x00uy; 0xC8uy|]
        | SetDisplayOffset verticalShift -> [|0x00uy; verticalShift &&& 0x3Fuy|]
        | SetCOMPinsHardwareConfiguration (COMPinConfiguration.Sequential, false) -> [|0x00uy; 0xDAuy; 0x02uy|]
        | SetCOMPinsHardwareConfiguration (COMPinConfiguration.Alternative, false) -> [|0x00uy; 0xDAuy; 0x12uy|]
        | SetCOMPinsHardwareConfiguration (COMPinConfiguration.Sequential, true) -> [|0x00uy; 0xDAuy; 0x22uy|]
        | SetCOMPinsHardwareConfiguration (COMPinConfiguration.Alternative, true) -> [|0x00uy; 0xDAuy; 0x32uy|]
        | SetDisplayClockDivideRatioOscilatorFrequency (divideRatio, oscillatorFrequency) -> [| 0x00uy; 0xD5uy; (divideRatio &&& 0x0Fuy) ||| ((oscillatorFrequency &&& 0x0Fuy) <<< 4)|]
        | SetPreChargePeriod (phase1, phase2) -> [|0x00uy; 0xD9uy; (phase1 &&& 0x0Fuy) ||| ((phase2 &&& 0x0Fuy) <<< 4)|]
        | SetVCOMHDeselectLevel VCC_065 -> [|0x00uy; 0xDBuy; 0x00uy|]
        | SetVCOMHDeselectLevel VCC_077 -> [|0x00uy; 0xDBuy; 0x20uy|]
        | SetVCOMHDeselectLevel VCC_083 -> [|0x00uy; 0xDBuy; 0x30uy|]
        | NOP -> [|0x00uy; 0xE3uy|]
        | SetChargePump On -> [|0x00uy; 0x8duy; 0x14uy|]
        | SetChargePump Off -> [|0x00uy; 0x8duy; 0x10uy|]
        
    let sendData data =
        data
        |> Array.append [|0x40uy|]
        |> device.Write

    let mutable displayRect = { Point = Point.zero; Size = {Width = 128; Height = 64} }
    let endianness = Little
    let mutable addressingMode = AddressingMode.Page
    let mutable preprocessor = id

    let sendCommand command =
        match command with 
        | SetMemoryAddressingMode mode -> 
            addressingMode <- match mode with 
                              | Vertical -> ColumnMajor
                              | Horizontal -> Page
        | SetColumnAddress (startAddress, endAddress) ->
            displayRect <- {Point = { X = startAddress |> int; Y = displayRect.Point.Y}; Size = {Width = endAddress - startAddress |> int |> (+) 1; Height = displayRect.Size.Height}}
            preprocessor <- id
        | SetPageAddress (startPage, endPage) -> 
            let minY = getPageCode startPage |> int |> (*) BitsInByte
            let maxY = (getPageCode endPage |> int |> (+) 1 |> (*) BitsInByte) - 1
            displayRect <- {Point = { X = displayRect.Point.X; Y = minY }; Size = {Width = displayRect.Size.Width; Height = maxY - minY + 1}}
            preprocessor <- id
        | _ -> ()

        getCommandBytes command
        |> device.Write

    let ensureModeAndEndianness (g:IGraphicDispalyMemory) =
        if g.AddressingMode = addressingMode && g.Endianness = endianness && g.Size.Width = displayRect.Size.Width && g.Size.Height = displayRect.Size.Height then
            g
        else
            let rect = Rect.fromSize displayRect.Size
            let newGraphics = Graphics.createFromDisplay this
            Graphics.copyTo rect newGraphics rect g
            newGraphics

    let display (g:IGraphicDispalyMemory) = 
        g 
        |> ensureModeAndEndianness 
        |> (fun p -> p.GetBuffer()) 
        |> sendData

    do SetMemoryAddressingMode Horizontal |> sendCommand

    interface ISSD1306 with
        member __.Size with get () = displayRect.Size
        member __.AddressingMode with get () = addressingMode
        member __.Endianness = endianness
        member __.Display(graphics) = display graphics
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

    let setup128x64 = 
        [
            SetColumnAddress (0uy, 127uy)
            SetPageAddress (Page0, Page7)
        ]

    let setup64x48 = 
        [
            SetColumnAddress (32uy, 95uy)
            SetPageAddress (Page2, Page7)
        ]

    let doubleHorizontalLines (g:IGraphicDispalyMemory) =
        let newGraphics = Graphics.createFromSize g.AddressingMode g.Endianness {g.Size with Height = g.Size.Height * 2}
        for x = 0 to g.Size.Width - 1 do
            for y = 0 to g.Size.Height - 1 do
                if (g.GetPixel x y = 1uy) then
                    newGraphics.SetPixel x (y * 2)
                    newGraphics.SetPixel x (y * 2 + 1)
        newGraphics