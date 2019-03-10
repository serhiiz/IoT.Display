namespace IoT.Display.Tests

open NUnit.Framework
open IoT.Display
open IoT.Display.Graphics
open Swensen.Unquote

module GraphicsTests = 

    //┌────────────────┐
    //│▀▄             ▀│
    //│  ▀▄            │
    //│    ▀▄          │
    //│      ▀▄        │
    //│        ▀▄      │
    //│          ▀▄    │
    //│            ▀▄  │
    //│              ▀▄│
    //└────────────────┘    
    let getAssymetricDiagonalGraphics mode endian = 
        let g = Graphics(mode, endian, {Size.Width = 16; Height = 16})
        for i = 0 to 15 do
            g.SetPixel i i
        g.SetPixel 15 0
        g

    [<Test>]
    let ``Diagonal graphics test ColumnMajor+Little`` () =
        let g = getAssymetricDiagonalGraphics ColumnMajor Little

        g.GetBuffer() =! [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                           0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                           0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                           0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

    [<Test>]
    let ``Diagonal graphics test ColumnMajor+Big`` () =
        let g = getAssymetricDiagonalGraphics ColumnMajor Big

        g.GetBuffer() =! [|0x80uy; 0x00uy; 0x40uy; 0x00uy; 0x20uy; 0x00uy; 0x10uy; 0x00uy; 
                           0x08uy; 0x00uy; 0x04uy; 0x00uy; 0x02uy; 0x00uy; 0x01uy; 0x00uy;
                           0x00uy; 0x80uy; 0x00uy; 0x40uy; 0x00uy; 0x20uy; 0x00uy; 0x10uy; 
                           0x00uy; 0x08uy; 0x00uy; 0x04uy; 0x00uy; 0x02uy; 0x80uy; 0x01uy|]

    [<Test>]
    let ``Diagonal graphics test Page+Little`` () =
        let g = getAssymetricDiagonalGraphics Page Little

        g.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy; 
                           0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x01uy; 
                           0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                           0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

    [<Test>]
    let ``Diagonal graphics test Page+Big`` () =
        let g = getAssymetricDiagonalGraphics Page Big

        g.GetBuffer() =! [|0x80uy; 0x40uy; 0x20uy; 0x10uy; 0x08uy; 0x04uy; 0x02uy; 0x01uy; 
                           0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x80uy; 
                           0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;                            
                           0x80uy; 0x40uy; 0x20uy; 0x10uy; 0x08uy; 0x04uy; 0x02uy; 0x01uy|]

    [<Test>]
    let ``Diagonal graphics test RowMajor+Little`` () =
        let g = getAssymetricDiagonalGraphics RowMajor Little

        g.GetBuffer() =! [|0x01uy; 0x80uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                           0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                           0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                           0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy|]

    [<Test>]
    let ``Diagonal graphics test RowMajor+Big`` () =
        let g = getAssymetricDiagonalGraphics RowMajor Big

        g.GetBuffer() =! [|0x80uy; 0x01uy; 0x40uy; 0x00uy; 0x20uy; 0x00uy; 0x10uy; 0x00uy; 
                           0x08uy; 0x00uy; 0x04uy; 0x00uy; 0x02uy; 0x00uy; 0x01uy; 0x00uy;
                           0x00uy; 0x80uy; 0x00uy; 0x40uy; 0x00uy; 0x20uy; 0x00uy; 0x10uy; 
                           0x00uy; 0x08uy; 0x00uy; 0x04uy; 0x00uy; 0x02uy; 0x00uy; 0x01uy|]
    
    [<Test>]
    [<TestCase(3, 3, 3)>]
    [<TestCase(3, 7, 3)>]
    [<TestCase(3, 9, 6)>]
    let ``Verify graphics buffer length Column Major`` width height expectedLength =
        let g = Graphics(ColumnMajor, Little, {Size.Width = width; Height = height})
        g.GetBuffer().Length =! expectedLength

    [<Test>]
    [<TestCase(3, 3, 3)>]
    [<TestCase(3, 7, 3)>]
    [<TestCase(3, 9, 6)>]
    let ``Verify graphics buffer length Page`` width height expectedLength =
        let g = Graphics(Page, Little, {Size.Width = width; Height = height})
        g.GetBuffer().Length =! expectedLength
    
    [<Test>]
    [<TestCase(3, 3, 3)>]
    [<TestCase(7, 3, 3)>]
    [<TestCase(9, 3, 6)>]
    let ``Verify graphics buffer length Row Major`` width height expectedLength =
        let g = Graphics(RowMajor, Little, {Size.Width = width; Height = height})
        g.GetBuffer().Length =! expectedLength

    [<Test>]
    let ``Clip test`` () =
        let buffer = [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                       0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                       0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                       0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16}, buffer)
        let result = clip {Point = {X = 0; Y = 0}; Size = {Width=8;Height=8}} g
        result.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

    [<Test>]
    let ``Clip outsize of bounds test`` () =
        let buffer = [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                       0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                       0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                       0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16}, buffer)
        let result = clip {Point = {X = 0; Y = 0}; Size = {Width=24;Height=8}} g
        result.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy;
                                0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x01uy|]

    [<Test>]
    let ``Clip 4 bits shifted vertically test`` () =
        let buffer = [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                       0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                       0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                       0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16}, buffer)
        let result = clip {Point = {X = 4; Y = 0}; Size = {Width=8;Height=8}} g
        result.GetBuffer() =! [|0x10uy; 0x20uy; 0x40uy; 0x80uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy|]

    [<Test>]
    let ``Clip 8 bits shifted test`` () =
        let buffer = [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                       0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                       0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                       0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16}, buffer)
        let result = clip {Point = {X = 8; Y = 8}; Size = {Width=8;Height=8}} g
        result.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

    [<Test>]
    let ``Clip returns original graphics when clip size is larger and starts from origin test`` () =
        let buffer = [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

        let g = Graphics(ColumnMajor, Little, {Size.Width = 8; Height = 8}, buffer)
        let result = clip {Point = {X = 0; Y = 0}; Size = {Width=16;Height=16}} g
        result.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

    [<Test>]
    let ``Copy to full size change addressing mode test`` () =
        let buffer = [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x01uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                       0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

        let sourceGraphics = Graphics(Page, Little, {Size.Width = 16; Height = 16}, buffer)
        let targetGraphics = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16})

        let rect = {Point = {X = 0; Y = 0}; Size = {Width=16;Height=16}}
        copyTo rect targetGraphics rect sourceGraphics
        
        targetGraphics.GetBuffer() =! [|0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 0x00uy; 
                                        0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x00uy; 0x80uy; 0x00uy;
                                        0x00uy; 0x01uy; 0x00uy; 0x02uy; 0x00uy; 0x04uy; 0x00uy; 0x08uy; 
                                        0x00uy; 0x10uy; 0x00uy; 0x20uy; 0x00uy; 0x40uy; 0x01uy; 0x80uy|]

    [<Test>]
    let ``Clip to first quarter using copy to test`` () =
        let buffer = [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x01uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                       0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

        let sourceGraphics = Graphics(Page, Little, {Size.Width = 16; Height = 16}, buffer)
        let targetGraphics = Graphics(Page, Little, {Size.Width = 8; Height = 8})

        let rect = {Point = {X = 0; Y = 0}; Size = {Width = 8; Height = 8}}
        copyTo rect targetGraphics rect sourceGraphics
        
        targetGraphics.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

    [<Test>]
    let ``Clip to third quarter using copy to test`` () =
        let buffer = [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x01uy; 
                       0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy; 0x00uy;
                       0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]

        let sourceGraphics = Graphics(Page, Little, {Size.Width = 16; Height = 16}, buffer)
        let targetGraphics = Graphics(Page, Little, {Size.Width = 8; Height = 8})

        let targetRect = {Point = {X = 0; Y = 0}; Size = {Width = 8; Height = 8}}
        let sourceRect = {Point = {X = 8; Y = 8}; Size = {Width = 8; Height = 8}}
        copyTo targetRect targetGraphics sourceRect sourceGraphics
        
        targetGraphics.GetBuffer() =! [|0x01uy; 0x02uy; 0x04uy; 0x08uy; 0x10uy; 0x20uy; 0x40uy; 0x80uy|]