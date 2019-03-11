namespace IoT.Display.Tests

open NUnit.Framework
open Swensen.Unquote
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Layout

module MeasureLayoutTests = 
    let maxSize = {Size.Width=128; Height=64}
    let onePixelGraphics = Graphics(ColumnMajor, Little, {Width = 1; Height = 1})

    [<Test>]
    let ``Vertical empty stack test`` () =
        let layout = stack [ Orientation Vertical; ] []
        measure maxSize layout =! Size.empty

    [<Test>]
    let ``Horizontal empty stack test`` () =
        let layout = stack [ Orientation Horizontal; ] []
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
        let layout = stack [ Orientation Vertical; Padding (thickness 4 3 2 1)] []
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
        let layout = stack [ Orientation Horizontal; Width 10; Height 10] []
        measure {Width = 5; Height = 5} layout =! { Width = 5; Height = 5 }

    [<Test>]
    let ``Measure string test`` () =
        let layout = text [] "str8"
        measure maxSize layout =! { Width = 5+FontClass.charSpacing+3+FontClass.charSpacing+3+FontClass.charSpacing+5; Height = FontClass.fontHeight }

    [<Test>]
    let ``Measure char wrapping test`` () =
        let layout = text [TextWrapping Char] "aaa"
        measure {Width = 10; Height = 64} layout =! { Width = 7; Height = 3 * FontClass.fontHeight + 2 * FontClass.lineSpacing }

    [<Test>]
    let ``Measure char wrapping widest string test`` () =
        let layout = text [TextWrapping Char] "a!"
        measure {Width = 8; Height = 64} layout =! { Width = 7; Height = 2 * FontClass.fontHeight + FontClass.lineSpacing }

    [<Test>]
    let ``Measure too long string no wrapping test`` () =
        let layout = text [] "aaa"
        measure {Width = 8; Height = 64} layout =! { Width = 8; Height = FontClass.fontHeight }

    [<Test>]
    let ``Measure word wrapping first line test`` () =
        let layout = text [TextWrapping Word] "aaa aaa"
        measure {Width = 25; Height = 64} layout =! { Width = 7 * 3 + 2 * FontClass.charSpacing; Height = 2 * FontClass.fontHeight + FontClass.lineSpacing }

    [<Test>]
    let ``Measure word wrapping second line test`` () =
        let layout = text [TextWrapping Word] "a aaa aaa"
        measure {Width = 25; Height = 64} layout =! { Width = 7 * 3 + 2 * FontClass.charSpacing; Height = 3 * FontClass.fontHeight + 2 * FontClass.lineSpacing }

    [<Test>]
    let ``Measure too long string word wrapping test`` () =
        let layout = text [TextWrapping Word] "a aaa aaa"
        measure {Width = 25; Height = 25} layout =! { Width = 7 * 3 + 2 * FontClass.charSpacing; Height = 25 }

    [<Test>]
    let ``Measure too long string char wrapping test`` () =
        let layout = text [TextWrapping Word] "aaa aaa"
        measure {Width = 25; Height = 25} layout =! { Width = 7 * 3 + 2 * FontClass.charSpacing; Height = 25 }

    [<Test>]
    let ``Measure string with/without word wrapping is same test`` () =
        let layout1 = text [] "aaa aaa"
        let layout2 = text [TextWrapping Word] "aaa aaa"
        measure maxSize layout1 =! measure maxSize layout2

    [<Test>]
    let ``Measure string with/without char wrapping is same test`` () =
        let layout1 = text [] "aaa aaa"
        let layout2 = text [TextWrapping Char] "aaa aaa"
        measure maxSize layout1 =! measure maxSize layout2

    [<Test>]
    let ``Measure string traling line space doesn't count test`` () =
        let layout = text [TextWrapping Word] "a a "
        measure {Width = 13; Height = 64} layout =! { Width = 7; Height = 2 * FontClass.fontHeight + FontClass.lineSpacing }

    [<Test>]
    let ``Measure string leading space is honored test`` () =
        let layout = text [TextWrapping Word] "  a"
        measure maxSize layout =! { Width = 4 + 4 + 7 + 2 * FontClass.charSpacing; Height = FontClass.fontHeight}

    [<Test>]
    let ``Measure char test`` () =
        let layout = text [] "!"
        measure maxSize layout =! { Width = 1; Height = FontClass.fontHeight }

    [<Test>]
    let ``Measure stack test`` () =
        let layout = stack [ Orientation Vertical; ][
            text [] "1"
            text [] "2"
        ]
        measure maxSize layout =! { Width = 5; Height = FontClass.fontHeight * 2 }

    [<Test>]
    let ``Measure image test`` () =
        let layout = image [] onePixelGraphics
        measure maxSize layout =! { Width = 1; Height = 1 }

    [<Test>]
    let ``Measure image with margin test`` () =
        let layout = image [Margin (thickness 1 2 3 4)] onePixelGraphics
        measure maxSize layout =! { Width = 5; Height = 7 }

    [<Test>]
    let ``Measure image with width/heigh set test`` () =
        let layout = image [Width 10; Height 5] onePixelGraphics
        measure maxSize layout =! { Width = 10; Height = 5 }            

    [<Test>]
    let ``Measure border test`` () =
        let layout = border [] (image [] onePixelGraphics)
        measure maxSize layout =! { Width = 1; Height = 1 }

    [<Test>]
    let ``Measure border with border thickness test`` () =
        let layout = border [Thickness (thickness 1 2 3 4)] (image [] onePixelGraphics)
        measure maxSize layout =! { Width = 5; Height = 7 }
    
    [<Test>]
    let ``Measure border with margin test`` () =
        let layout = border [Margin (thickness 1 2 3 4);] (image [] onePixelGraphics)
        measure maxSize layout =! { Width = 5; Height = 7 }

    [<Test>]
    let ``Measure border with padding test`` () =
        let layout = border [Margin (thickness 4 3 2 1);] (image [] onePixelGraphics)
        measure maxSize layout =! { Width = 7; Height = 5 }

    [<Test>]
    let ``Measure border with width and height while child is larger test`` () =
        let layout = 
            border [Width 3; Height 3] (
                border [Width 10; Height 10] (
                    image [] onePixelGraphics
                )
            )
        measure maxSize layout =! { Width = 3; Height = 3 }