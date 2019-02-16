namespace IoT.Display.Tests

open NUnit.Framework
open Swensen.Unquote
open IoT.Display
open IoT.Display.Layout

module MeasureLayoutTests = 
    let maxSize = {Size.Width=128; Height=64}

    [<Test>]
    let ``Vertical empty stack test`` () =
        let layout = stack Vertical [] []
        measure maxSize layout =! Size.empty

    [<Test>]
    let ``Horizontal empty stack test`` () =
        let layout = stack Horizontal [] []
        measure maxSize layout =! Size.empty

    [<Test>]
    let ``Dock honors size test`` () =
        let layout = dock [Width 10; Height 10] []
        measure maxSize layout =! { Width = 10; Height = 10 }
       
    [<Test>]
    let ``Dock honors margin test`` () =
        let layout = dock [Width 10; Height 10; Margin (thickness 1 2 3 4)] []
        measure maxSize layout =! { Width = 14; Height = 16 }

    [<Test>]
    let ``Dock honors padding test`` () =
        let layout = dock [Padding (thickness 4 3 2 1)] []
        measure maxSize layout =! { Width = 6; Height = 4 }

    [<Test>]
    let ``Stack honors padding test`` () =
        let layout = stack Vertical [Padding (thickness 4 3 2 1)] []
        measure maxSize layout =! { Width = 6; Height = 4 }

    [<Test>]
    let ``Measure string with width test`` () =
        let layout = text [Width 10] "aaaaaaaaaaaaaa"
        measure maxSize layout =! { Width = 10; Height = FontClass.fontHeight }

    [<Test>]
    let ``Max size is honored while measuring string`` () =
        let layout = text [Width 10; Height 10] ""
        measure {Width = 5; Height = 5} layout =! { Width = 5; Height = 5 }

    [<Test>]
    let ``Max size is honored while measuring dock`` () =
        let layout = dock [Width 10; Height 10] []
        measure {Width = 5; Height = 5} layout =! { Width = 5; Height = 5 }

    [<Test>]
    let ``Max size is honored while measuring stack`` () =
        let layout = stack Horizontal [Width 10; Height 10] []
        measure {Width = 5; Height = 5} layout =! { Width = 5; Height = 5 }

    [<Test>]
    let ``Measure string test`` () =
        let layout = text [] "str8"
        measure maxSize layout =! { Width = 5+1+3+1+3+1+5; Height = FontClass.fontHeight }

    [<Test>]
    let ``Measure char test`` () =
        let layout = text [] "!"
        measure maxSize layout =! { Width = 1; Height = FontClass.fontHeight }

    [<Test>]
    let ``Measure stack test`` () =
        let layout = stack Vertical [][
            text [] "1"
            text [] "2"
        ]
        measure maxSize layout =! { Width = 5; Height = FontClass.fontHeight * 2 }