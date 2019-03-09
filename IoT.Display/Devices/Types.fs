namespace IoT.Display.Devices

open System
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Layout

type IDevice = 
    inherit IDisposable
    abstract member Write: byte[] -> unit

type IDisplay = 
    abstract member Size: Size
    abstract member AddressingMode: AddressingMode
    abstract member Endian: Endian
    abstract member Display: Graphics -> unit

[<AutoOpen>]
module Graphics =
    let createFromDisplayCustomSize (display:IDisplay) size = 
        Graphics(display.AddressingMode, display.Endian, size)

    let createFromDisplay (display:IDisplay) = 
        createFromDisplayCustomSize display display.Size

[<AutoOpen>]
module Primitives =
    let renderVisualToDisplay (display:IDisplay) visual =
        let graphics = Graphics.createFromDisplay display
        Primitives.renderVisualToGraphics graphics visual
        display.Display graphics

[<AutoOpen>]
module Layout = 
    let renderToDisplay (display:IDisplay) element =
        let graphics = Graphics.createFromDisplay display
        renderToGraphics graphics element
        display.Display graphics