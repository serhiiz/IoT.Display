namespace IoT.Display.Tests

open NUnit.Framework
open IoT.Display
open Swensen.Unquote

module RectTests = 
    [<Test>]
    let ``Rects do not intersect test`` () =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 5; Height = 5}}
        let rect2 = {Point = {X = 10; Y = 10}; Size = {Width = 5; Height = 5}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection.Size =! Size.empty

    [<Test>]
    let ``Rects are same test`` () =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 5; Height = 5}}
        let rect2 = {Point = {X = 0; Y = 0}; Size = {Width = 5; Height = 5}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! rect1

    [<Test>]
    let ``First rect is inside second test`` () =
        let rect1 = {Point = {X = 3; Y = 3}; Size = {Width = 5; Height = 5}}
        let rect2 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 10}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! rect1

    [<Test>]
    let ``Second rect is inside first test`` () =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 10}}
        let rect2 = {Point = {X = 3; Y = 3}; Size = {Width = 5; Height = 5}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! rect2

    [<Test>]
    [<TestCase(-3, 0)>]
    [<TestCase(3, 3)>]
    let ``One rect is shifted horizontally test`` rectX resultX =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 10}}
        let rect2 = {Point = {X = rectX; Y = 0}; Size = {Width = 10; Height = 10}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! {Point = {X = resultX; Y = 0}; Size = {Width = 7; Height = 10}}

    [<Test>]
    [<TestCase(-4, 0)>]
    [<TestCase(4, 4)>]
    let ``One rect is shifted vertically test`` rectY resultY =
        let rect1 = {Point = {X = 0; Y = rectY}; Size = {Width = 10; Height = 10}}
        let rect2 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 10}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! {Point = {X = 0; Y = resultY}; Size = {Width = 10; Height = 6}}

    [<Test>]
    let ``One rect intersects another test`` () =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 9}}
        let rect2 = {Point = {X = 3; Y = 3}; Size = {Width = 15; Height = 15}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! {Point = {X = 3; Y = 3}; Size = {Width = 7; Height = 6}}

    [<Test>]
    let ``One rect intersects side of another test`` () =
        let rect1 = {Point = {X = 0; Y = 0}; Size = {Width = 10; Height = 10}}
        let rect2 = {Point = {X = 8; Y = 2}; Size = {Width = 10; Height = 6}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! {Point = {X = 8; Y = 2}; Size = {Width = 2; Height = 6}}

    [<Test>]
    let ``One rect make a cross with another`` () =
        let rect1 = {Point = {X = 5; Y = 0}; Size = {Width = 5; Height = 15}}
        let rect2 = {Point = {X = 0; Y = 5}; Size = {Width = 15; Height = 5}}
        let intersection = Rect.getIntersection rect1 rect2
        intersection =! {Point = {X = 5; Y = 5}; Size = {Width = 5; Height = 5}}