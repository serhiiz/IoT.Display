open System.Drawing

open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Primitives
open IoT.Display.Layout
open System.Xml.Linq

let renderToFile fileName size layout = 
    
    use b = new Bitmap(size.Width+2, size.Height+2)
    
    let g = Graphics.createFromSize AddressingMode.Page Endianness.Little {Width = size.Width + 2; Height = size.Height + 2}
    border [Thickness (thicknessSame 1)] (layout) |> renderToGraphics g
    for i = 0 to g.Size.Width - 1 do
        for j = 0 to g.Size.Height - 1 do
            b.SetPixel(i, j, (if (g.GetPixel i j = 1uy) then Color.Black else Color.White))

    b.Save fileName

let size = {Width = 128; Height = 64}

let showDockOrder () =
    dock [] [
        dock [Width 64; Padding (thicknessSame 2)] [
            border [Dock Dock.Left; Width 20; Thickness (thicknessSame 1)] ( canvas [] [] )
            border [Dock Dock.Bottom; Height 20; Thickness (thicknessSame 1)] ( canvas [] [] )
        ]
        dock [Width 64; Padding (thicknessSame 2)] [
            border [Dock Dock.Bottom; Height 20; Thickness (thicknessSame 1)] ( canvas [] [] )
            border [Dock Dock.Left; Width 20; Thickness (thicknessSame 1)] ( canvas [] [] )
        ]
    ]
    |> renderToFile "showDockOrder.bmp" size

let showWordWrapping () =
    text [TextWrapping Word] "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the"
    |> renderToFile "textWrapping.bmp" size

let showTextWrapping () =
    dock [] [
        dock [Width 64; Padding (thicknessSame 2)] [
            border [Dock Dock.Left; Width 20; Thickness (thicknessSame 1)] ( canvas [] [] )
            border [Dock Dock.Bottom; Height 20; Thickness (thicknessSame 1)] ( canvas [] [] )
        ]
        dock [Width 64; Padding (thicknessSame 2)] [
            border [Dock Dock.Bottom; Height 20; Thickness (thicknessSame 1)] ( canvas [] [] )
            border [Dock Dock.Left; Width 20; Thickness (thicknessSame 1)] ( canvas [] [] )
        ]
    ]
    |> renderToFile "dockOrder.bmp" size

let showCanvas () =
    dock [] [
        canvas [Dock Dock.Fill; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center; Width 21; Height 21] [
            Polyline [{X = 5; Y = 9}; {X = 9; Y = 16}; {X = 15; Y = 5}]
            QuadraticBezier ({X = 0; Y = 10}, {X = 0; Y = 20}, {X = 10; Y = 20})
            QuadraticBezier ({X = 0; Y = 10}, {X = 0; Y = 0}, {X = 10; Y = 0})
            QuadraticBezier ({X = 20; Y = 10}, {X = 20; Y = 20}, {X = 10; Y = 20})
            QuadraticBezier ({X = 20; Y = 10}, {X = 20; Y = 0}, {X = 10; Y = 0})
        ]
    ]
    |> renderToFile "canvas.bmp" size

let showStackPanel () = 
    let paramStyle:ITextAttribute list = [HorizontalAlignment HorizontalAlignment.Right; Margin (thickness 0 2 1 2)]
    let valueStyle:ITextAttribute list = [HorizontalAlignment HorizontalAlignment.Left; Margin (thickness 1 2 0 2)]

    stack [Orientation StackPanelOrientation.Horizontal; HorizontalAlignment HorizontalAlignment.Center; Padding (thicknessSame 1)] [
        stack [Orientation StackPanelOrientation.Vertical; Width 64] [
            text paramStyle "Param1:"
            text paramStyle "Param2:"
            text paramStyle "Param3:"
        ]
        stack [Orientation StackPanelOrientation.Vertical; Width 64] [
            text valueStyle "Value1"
            text valueStyle "Value2"
            text valueStyle "Value3"
        ]
    ]
    |> renderToFile "stack.bmp" size


let showDockPanel () = 
    let paramStyle:ITextAttribute list = [HorizontalAlignment HorizontalAlignment.Right; Margin (thickness 0 2 1 2)]
    let valueStyle:ITextAttribute list = [HorizontalAlignment HorizontalAlignment.Left; Margin (thickness 1 2 0 2)]

    stack [Orientation StackPanelOrientation.Horizontal; HorizontalAlignment HorizontalAlignment.Center; Padding (thicknessSame 1)] [
        stack [Orientation StackPanelOrientation.Vertical; Width 64] [
            text paramStyle "Param1:"
            text paramStyle "Param2:"
            text paramStyle "Param3:"
        ]
        stack [Orientation StackPanelOrientation.Vertical; Width 64] [
            text valueStyle "Value1"
            text valueStyle "Value2"
            text valueStyle "Value3"
        ]
    ]
    |> renderToFile "dock.bmp" size

let showVisuals () = 
    let r = System.Random()
    canvas [Width 128; Height 64] (
        List.init 21 (fun _ -> r.Next(30) + 2)
        |> List.mapi(fun i p -> Visual.Rectangle {Point = {X = i * 6; Y = 63 - p}; Size = {Width = 4; Height = p }})
        |> List.append [
            Visual.Line ({X = 0; Y = 0},{X = 127; Y = 31})
            Visual.Line ({X = 0; Y = 31},{X = 127; Y = 0})
        ]
    )
    |> renderToFile "visuals.bmp" size

[<EntryPoint>]
let main _ =
    showDockOrder()
    showWordWrapping()
    showCanvas()
    showStackPanel()
    showDockPanel()
    showVisuals()
    0
