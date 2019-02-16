namespace IoT.Display

open IoT.Display.Graphics

module Primitives =

    type Visual =
    | Dot of Point
    | Line of Point*Point
    | Rectangle of Point*Point
    
    let private writeLine (x1:int) (y1:int) (x2:int) (y2:int) writeDot =
        let dx = x2 - x1
        let dy = y2 - y1

        if (dy = 0) 
        then 
            for x = (x1|>int) to (x2 |> int) do 
                writeDot (x*1) y1
        else
            let k = (dy |> float)/(dx |> float)
            let b = (y1 |> float) - k * (x1 |> float)
            
            for x = (x1|>int) to (x2 |> int) do 
                let y = k * (x |> float) + b |> System.Math.Round |> int
                writeDot (x*1) (y*1)
    
    let private writeRectangle (x1:int) (y1:int) (x2:int) (y2:int) writeDot =
        for i = x1 |> int to x2 |> int do
            for j = y1 |> int to y2 |> int do
                writeDot (i*1) (j*1)


    let renderVisual writeDot visual =
        match visual with
        | Dot p -> writeDot p.X p.Y
        | Line (p1,p2) -> writeLine p1.X p1.Y p2.X p2.Y writeDot
        | Rectangle (p1, p2) -> writeRectangle p1.X p1.Y p2.X p2.Y writeDot

    let renderVisualToGraphics (graphics:Graphics) visual =
        renderVisual graphics.SetPixel visual

    let renderVisualToDisplay (display:IDisplay) visual =
        let graphics = Graphics.createFromDisplay display
        renderVisualToGraphics graphics visual
        display.SendData (graphics.GetBuffer())