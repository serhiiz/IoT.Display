namespace IoT.Display

open System
open IoT.Display.Graphics

module Primitives =

    type Visual =
    | Dot of Point
    | Line of Point*Point
    | Rectangle of Point*Point
    | Polyline of Point list
    
    let private writeLine (x1:int) (y1:int) (x2:int) (y2:int) writeDot =
        let dx = x2 - x1
        let dy = y2 - y1

        if (dx = 0) 
        then 
            let minY = Math.Min(y1, y2)
            let maxY = Math.Max(y1, y2)
            for y = minY to maxY do 
                writeDot x1 y
        else
            let minX = Math.Min(x1, x2)
            let maxX = Math.Max(x1, x2)
            let k = (dy |> float)/(dx |> float)
            let b = (y1 |> float) - k * (x1 |> float)
            
            for x = minX to maxX do 
                let y = (k * (x |> float) + b) |> System.Math.Round |> int
                writeDot x y
    
    let private writeRectangle (x1:int) (y1:int) (x2:int) (y2:int) writeDot =
        let minX = Math.Min(x1, x2)
        let maxX = Math.Max(x1, x2)
        let minY = Math.Min(y1, y2)
        let maxY = Math.Max(y1, y2)

        for i = minX |> int to maxX |> int do
            for j = minY |> int to maxY |> int do
                writeDot i j

    let renderVisual writeDot visual =
        match visual with
        | Dot p -> writeDot p.X p.Y
        | Line (p1,p2) -> writeLine p1.X p1.Y p2.X p2.Y writeDot
        | Rectangle (p1, p2) -> writeRectangle p1.X p1.Y p2.X p2.Y writeDot
        | Polyline points -> 
            points
            |> List.pairwise
            |> List.iter (fun (p1, p2) -> writeLine p1.X p1.Y p2.X p2.Y writeDot)

    let renderVisualToGraphics (graphics:Graphics) visual =
        renderVisual graphics.SetPixel visual

    let renderVisualToDisplay (display:IDisplay) visual =
        let graphics = Graphics.createFromDisplay display
        renderVisualToGraphics graphics visual
        display.SendData (graphics.GetBuffer())