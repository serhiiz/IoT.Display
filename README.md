# Layout engine for monochrome OLED dispalys in IoT applications written in F#
The library is supposed to be used in applications where UI is rendered every time there's a change. The layout is defined declaratively using elements like dock or stack panels similarly to WPF. Once defined it can be rendered into a buffer and then sent to the display driver. This library does not include platform specific communication code for SSD1306. It requires an implementation of a tiny interface or use one of the avalialbe ones:
- [IoT.Display.UWP](https://github.com/serhiiz/IoT.Display.UWP) UWP adapter and demo app;
- [IoT.Display.RaspberrySharp](https://github.com/serhiiz/IoT.Display.RaspberrySharp) .Net/Mono adapter for Raspberry Pi and demo app (based on [RaspberrySharp](https://github.com/JTrotta/RaspberrySharp)).

[![NuGet version (IoT.Display)](https://img.shields.io/nuget/v/IoT.Display.svg?style=flat-square)](https://www.nuget.org/packages/IoT.Display/)

## Features:
- Dock panel
- Stack panel
- Border
- Text (wrapping: char, word)
- Image
- Canvas
- Horizontal/Vertical alignment
- Marging/Padding
- Min/Max width/height

## Display Drivers Support:
- [SMD1306](https://cdn-shop.adafruit.com/datasheets/SSD1306.pdf)

## Layout examples:
### Stack panel
```F#
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Layout

let paramStyle = [HorizontalAlignment HorizontalAlignment.Right; Margin (thickness 0 2 1 2)]
let valueStyle = [HorizontalAlignment HorizontalAlignment.Left; Margin (thickness 1 2 0 2)]

let g = Graphics(AddressingMode.ColumnMajor, Endian.Little, {Size.Width = 128; Height = 64})

stack StackPanelOrientation.Horizontal [HorizontalAlignment HorizontalAlignment.Center; Padding (thicknessSame 1)] [
    stack StackPanelOrientation.Vertical [Width 64] [
        text paramStyle "Param1:"
        text paramStyle "Param2:"
        text paramStyle "Param3:"
    ]
    stack StackPanelOrientation.Vertical [Width 64] [
        text valueStyle "Value1"
        text valueStyle "Value2"
        text valueStyle "Value3"
    ]
]
|> renderToGraphics g
```
![Stack example rendered to 128x64 buffer](https://raw.githubusercontent.com/serhiiz/IoT.Display/master/Docs/Images/stack.bmp)

### Dock panel
```F#
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Layout

let g = Graphics(AddressingMode.ColumnMajor, Endian.Little, {Size.Width = 128; Height = 64})

dock [][
    text [Dock Dock.Bottom; Margin (thicknessSame 1)] "Bottom line"
    stack StackPanelOrientation.Vertical [Dock Dock.Left; Margin (thicknessSame 1)] [
        text [] "1"
        text [] "2"
        text [] "3"
    ]
    text [Dock Dock.Fill; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] "Filled"
]
|> renderToGraphics g
```
![Dock example rendered to 128x64 buffer](https://raw.githubusercontent.com/serhiiz/IoT.Display/master/Docs/Images/dock.bmp)

### Visuals examples
```F#
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Primitives

let g = Graphics(AddressingMode.ColumnMajor, Endian.Little, {Size.Width = 128; Height = 64})

let r = Random()
List.init 21 (fun _ -> r.Next(30) + 2)
|> List.mapi(fun i p -> Rectangle ({X = i * 6; Y = 63 - p}, {X = i * 6 + 4; Y = 63}) )
|> List.append [
        Visual.Line ({X = 0; Y = 0},{X = 127; Y = 31})
        Visual.Line ({X = 0; Y = 31},{X = 127; Y = 0})
    ]
|> List.iter (renderVisualToGraphics g)
```
![Visuals example rendered to 128x64 buffer](https://raw.githubusercontent.com/serhiiz/IoT.Display/master/Docs/Images/visuals.bmp)

## Possible enhancements
- ~Add Border~
- Add Text Direction
- Add Font Size
- ~Add Clip to Graphics~
- ~Add text wrapping~
- ~Support on mono/.net core~
- Support for SH1106

## License
IoT.Display is licensed under the [MIT license](LICENSE).
