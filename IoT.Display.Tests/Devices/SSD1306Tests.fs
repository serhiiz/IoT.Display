namespace IoT.Display.Tests

open NUnit.Framework
open IoT.Display
open IoT.Display.Devices.SSD1306

module SSD1306Tests = 

    [<Test>]
    let ``Double horizontal lines test`` () =
        let g = Graphics.createDefault {Size.Width = 8; Height = 8}

        g.SetPixel 0 0
        g.SetPixel 1 1
        g.SetPixel 7 7
        g.SetPixel 7 0
        g.SetPixel 0 7

        let actual = (g |> doubleHorizontalLines)
        assertRender actual """
┌────────┐
│█      █│
│ █      │
│        │
│        │
│        │
│        │
│        │
│█      █│
└────────┘"""