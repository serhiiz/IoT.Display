namespace IoT.Display

open System
open IoT.Display.Graphics

module Primitives =

    type Visual =
    | Dot of Point
    | Line of Point*Point
    | Rectangle of Rect
    | Polyline of Point list
    
    let private writeLine (x1:int) (y1:int) (x2:int) (y2:int) (graphics:Graphics) =
        let dx = x2 - x1
        let dy = y2 - y1

        if (dx = 0) 
        then 
            let minY = Math.Max(Math.Min(y1, y2), 0)
            let maxY = Math.Min(Math.Max(y1, y2), graphics.Size.Height - 1)
            for y = minY to maxY do 
                graphics.SetPixel x1 y
        else
            let minX = Math.Max(Math.Min(x1, x2), 0)
            let maxX = Math.Min(Math.Max(x1, x2), graphics.Size.Width - 1)
            let k = (dy |> float)/(dx |> float)
            let b = (y1 |> float) - k * (x1 |> float)
            
            for x = minX to maxX do 
                let y = (k * (x |> float) + b) |> System.Math.Round |> int
                graphics.SetPixel x y
    
    let private writeRectangle (rect:Rect) (graphics:Graphics) =
        let x = Math.Max(rect.Point.X, 0)
        let width = Math.Min(rect.Size.Width, graphics.Size.Width)
        let y = Math.Max(rect.Point.Y, 0)
        let height = Math.Min(rect.Size.Height, graphics.Size.Height)

        for i = x to x + width - 1 do
            for j = y to y + height - 1 do
                graphics.SetPixel i j

    let renderVisualToGraphics (graphics:Graphics) visual =
        match visual with
        | Dot p -> 
            if (p.X >= 0 && p.X < graphics.Size.Width && p.Y >= 0 && p.Y < graphics.Size.Height) then 
                graphics.SetPixel p.X p.Y
        | Line (p1,p2) -> writeLine p1.X p1.Y p2.X p2.Y graphics
        | Rectangle rect -> writeRectangle rect graphics
        | Polyline points -> 
            points
            |> List.pairwise
            |> List.iter (fun (p1, p2) -> writeLine p1.X p1.Y p2.X p2.Y graphics)

    let renderVisualToDisplay (display:IDisplay) visual =
        let graphics = Graphics.createFromDisplay display
        renderVisualToGraphics graphics visual
        display.SendData (graphics.GetBuffer())