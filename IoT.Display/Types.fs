namespace IoT.Display
open System

type Thickness = 
    {Left:int; Top:int; Right: int; Bottom:int}
    static member ( + ) (left: Thickness, right: Thickness) = 
        {Left = left.Left + right.Left; Top = left.Top + right.Top; Right = left.Right + right.Right; Bottom = left.Bottom + right.Bottom}

type Point = 
    { X:int; Y:int }
    static member ( + ) (left: Point, right: Point) = { X = left.X + right.X; Y = left.Y + right.Y }
    static member ( - ) (left: Point, right: Point) = { X = left.X - right.X; Y = left.Y - right.Y }

type Size = 
    { Width:int; Height:int }
    static member ( + ) (left: Size, right: Size) = { Width = left.Width + right.Width; Height = left.Height + right.Height }
    static member ( - ) (left: Size, right: Size) = { Width = left.Width - right.Width; Height = left.Height - right.Height }

type Rect = 
    {Point:Point; Size:Size}

type AddressingMode = 
    | RowMajor 
    | ColumnMajor
    | Page

type Endian = 
    | Little 
    | Big

[<AutoOpen>]
module Point =
    let zero = {X = 0; Y = 0}

[<AutoOpen>]
module Thickness =
    let thickness l t r b = {Left = l; Top = t; Right = r; Bottom = b}
    let thicknessSimm lr tb = thickness lr tb lr tb
    let thicknessSame ltrb = thickness ltrb ltrb ltrb ltrb
    let emptyThickness = thickness 0 0 0 0
    let toSize t =
        {Width =t.Left + t.Right; Height = t.Top + t.Bottom}

[<AutoOpen>]
module Size =
    let empty = {Size.Width = 0; Height = 0}

    let toPoint s = {X = s.Width; Y = s.Height}

    let combine f s1 s2 = 
        { Width = f s1.Width s2.Width; Height = f s1.Height s2.Height }

    let min = combine (fun x y -> Math.Min(x, y))
    
    let applyBounds = combine (fun bounds s -> if s > bounds then bounds else s )

[<AutoOpen>]
module Rect =
    let fromSize s = 
        {Point = zero; Size = s}

    let isPointWithin r p =
        p.X >= r.Point.X && p.Y >= r.Point.Y && p.X < (r.Point.X + r.Size.Width) && p.Y < (r.Point.Y + r.Size.Height)
    
    let shrink r thickness =
        let newSize = r.Size - (toSize thickness)
        {Point = r.Point + {X = thickness.Left; Y = thickness.Top}; Size = { Width = Math.Max(newSize.Width, 0); Height = Math.Max(newSize.Height, 0) }}

    let intersects rect1 rect2 = 
        rect1.Size <> Size.empty
        && rect2.Size <> Size.empty
        && rect1.Point.X <= rect2.Point.X + rect2.Size.Width
        && rect1.Point.X + rect1.Size.Width >= rect2.Point.X
        && rect1.Point.Y <= rect2.Point.Y + rect2.Size.Height
        && rect1.Point.Y + rect1.Size.Height >= rect2.Point.Y

    let getIntersection rect1 rect2 = 
        if intersects rect1 rect2 then
            let x = Math.Max (rect1.Point.X, rect2.Point.X)
            let y = Math.Max(rect1.Point.Y, rect2.Point.Y)

            let width =  Math.Max(Math.Min(rect1.Point.X + rect1.Size.Width, rect2.Point.X + rect2.Size.Width) - x, 0)
            let height = Math.Max(Math.Min(rect1.Point.Y + rect1.Size.Height, rect2.Point.Y + rect2.Size.Height) - y, 0)
            
            {Point = {X = x; Y = y}; Size = {Width = width; Height = height}}
        else
            {rect1 with Size = Size.empty}

