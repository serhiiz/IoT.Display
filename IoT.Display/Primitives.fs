namespace IoT.Display

open System
open IoT.Display.Graphics

module Primitives =

    type Visual =
    | Dot of Point
    | Line of Point*Point
    | Rectangle of Rect
    | Polyline of Point list
    | QuadraticBezier of Point * Point * Point
    
    module Bezier = 

        type PointF = 
            {X:float; Y:float}
            static member ( * ) (left: PointF, a: float) = { X = left.X * a; Y = left.Y * a }
            static member ( + ) (left: PointF, right: PointF) = { X = left.X + right.X; Y = left.Y + right.Y }

        let writeQuadraticBezier p0 p1 p2 graphics writePolyline =
            let toPointf (p:Point) = {X = p.X |> float; Y = p.Y |> float}
            let f0 = toPointf p0
            let f1 = toPointf p1
            let f2 = toPointf p2

            let quadraticBezier (p0: PointF) (p1: PointF) (p2: PointF) t = (p0 * (1. - t) + p1 * t) * (1. - t) + (p1 * (1. - t) + p2 * t) * t

            let f = quadraticBezier f0 f1 f2
            let steps = 
                [p0.X - p1.X; p1.X - p2.X; p0.Y - p1.Y; p1.Y - p2.Y]
                |> List.map Math.Abs
                |> List.max
                |> (fun p -> p / 3)
        
            [0..steps]
            |> List.map (fun p -> (p |> float) / (steps |> float))
            |> List.map f
            |> List.map (fun p -> {Point.X = p.X |> int; Point.Y = p.Y |> int})
            |> writePolyline graphics

    let private writeLine (x1:int) (y1:int) (x2:int) (y2:int) (graphics:Graphics) =
        let dx = x2 - x1
        let dy = y2 - y1

        if (dx = 0 || (Math.Abs(dx) < Math.Abs(dy))) 
        then 
            let minY = Math.Max(Math.Min(y1, y2), 0)
            let maxY = Math.Min(Math.Max(y1, y2), graphics.Size.Height - 1)
            let k = lazy ((dy |> float)/(dx |> float))
            let b = lazy ((y1 |> float) - k.Value * (x1 |> float))

            for y = minY to maxY do 
                let x = if (dx = 0) then x1 else ((y |> float) - b.Value) / k.Value |> System.Math.Round |> int
                if x >= 0 && x < graphics.Size.Width then
                    graphics.SetPixel x y

        else
            let minX = Math.Max(Math.Min(x1, x2), 0)
            let maxX = Math.Min(Math.Max(x1, x2), graphics.Size.Width - 1)
            let k = (dy |> float)/(dx |> float)
            let b = (y1 |> float) - k * (x1 |> float)
            
            for x = minX to maxX do 
                let y = (k * (x |> float) + b) |> System.Math.Round |> int
                if y >= 0 && y < graphics.Size.Height then
                    graphics.SetPixel x y
    
    let private writeRectangle (rect:Rect) (graphics:Graphics) =
        let r' = Rect.getIntersection rect (Rect.fromSize graphics.Size)

        for i = r'.Point.X to r'.Point.X + r'.Size.Width - 1 do
            for j = r'.Point.Y to r'.Point.Y + r'.Size.Height - 1 do
                graphics.SetPixel i j

    let writePolyline graphics points = 
        points
            |> List.pairwise
            |> List.iter (fun (p1, p2) -> writeLine p1.X p1.Y p2.X p2.Y graphics)

    let renderVisualToGraphics (graphics:Graphics) visual =
        match visual with
        | Dot p -> 
            if (p.X >= 0 && p.X < graphics.Size.Width && p.Y >= 0 && p.Y < graphics.Size.Height) then 
                graphics.SetPixel p.X p.Y
        | Line (p1,p2) -> writeLine p1.X p1.Y p2.X p2.Y graphics
        | Rectangle rect -> writeRectangle rect graphics
        | Polyline points -> writePolyline graphics points 
        | QuadraticBezier (p0, p1, p2) -> Bezier.writeQuadraticBezier p0 p1 p2 graphics writePolyline