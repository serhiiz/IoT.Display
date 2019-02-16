namespace IoT.Display
open System

type Thickness = {Left:int; Top:int; Right: int; Bottom:int}
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

type Endian = 
    | Little 
    | Big
  
type IDisplay = 
    abstract member Size: Size
    abstract member AddressingMode: AddressingMode
    abstract member Endian: Endian
    abstract member SendData: byte[] -> unit

[<AutoOpen>]
module Thickness =
    let thickness l t r b = {Left = l; Top = t; Right = r; Bottom = b}
    let thicknessSimm lr tb = thickness lr tb lr tb
    let thicknessSame ltrb = thickness ltrb ltrb ltrb ltrb
    let emptyThickness = thickness 0 0 0 0
    let toSize t =
        {Width =t.Left + t.Right; Height = t.Top + t.Bottom}
    
    let shrink r thickness =
        {Point = r.Point + {X = thickness.Left; Y = thickness.Top}; Size = r.Size - (toSize thickness)}

    let expand r thickness =
        {Point = r.Point - {X = thickness.Left; Y = thickness.Top}; Size = r.Size + (toSize thickness)}

[<AutoOpen>]
module Rect =
    let fromSize s = 
        {Point = {X = 0; Y = 0;}; Size = s}

[<AutoOpen>]
module Size =
    let empty = {Size.Width = 0; Height = 0}

    let toPoint s = {X = s.Width; Y = s.Height}

    let combine f s1 s2 = 
        { Width = f s1.Width s2.Width; Height = f s1.Height s2.Height }

    let min = combine (fun x y -> Math.Min(x, y))
    
    let applyBounds = combine (fun bounds s -> if s > bounds then bounds else s )