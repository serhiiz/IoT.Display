namespace IoT.Display.Tests

open NUnit.Framework
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Primitives
open IoT.Display.Layout

module RenderLayoutTests = 
    let point2x2 = Graphics(ColumnMajor, Little, {Size.Width = 2; Height = 2})
    for i = 0 to 3 do point2x2.SetPixel (i % 2) (i / 2)
    let width = 8
    let height = 8
    let rect = {Point={X = 0; Y = 0}; Size={Width=width; Height=height}}

    let createGraphics() = 
        Graphics(ColumnMajor, Little, {Size.Width = width; Height = height})
    

    [<Test>]
    let ``Dock right align center test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Right; VerticalAlignment VerticalAlignment.Center] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│      ▄▄│
│      ▀▀│
│        │
└────────┘"""

    [<Test>]
    let ``Dock right align top test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Right; VerticalAlignment VerticalAlignment.Top] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│      ██│
│        │
│        │
│        │
└────────┘"""
    
    [<Test>]
    let ``Dock right align bottom test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Right; VerticalAlignment VerticalAlignment.Bottom] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│      ██│
└────────┘"""

    [<Test>]
    let ``Dock left align center test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Left; VerticalAlignment VerticalAlignment.Center] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│▄▄      │
│▀▀      │
│        │
└────────┘"""

    [<Test>]
    let ``Dock left align top test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Left; VerticalAlignment VerticalAlignment.Top] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│██      │
│        │
│        │
│        │
└────────┘"""
    
    [<Test>]
    let ``Dock left align bottom test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Left; VerticalAlignment VerticalAlignment.Bottom] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│██      │
└────────┘"""

    [<Test>]
    let ``Dock top align center test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Top; HorizontalAlignment HorizontalAlignment.Center] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│   ██   │
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Dock top align left test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Top; HorizontalAlignment HorizontalAlignment.Left] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│██      │
│        │
│        │
│        │
└────────┘"""
    
    [<Test>]
    let ``Dock top align tigh test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Top; HorizontalAlignment HorizontalAlignment.Right] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│      ██│
│        │
│        │
│        │
└────────┘"""
    
    [<Test>]
    let ``Dock bottom align center test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Bottom; HorizontalAlignment HorizontalAlignment.Center] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│   ██   │
└────────┘"""

    [<Test>]
    let ``Dock bottom align left test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Bottom; HorizontalAlignment HorizontalAlignment.Left] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│██      │
└────────┘"""
    
    [<Test>]
    let ``Dock bottom align tigh test`` () =
        let g = createGraphics()

        dock [] [
          image [Dock Bottom; HorizontalAlignment HorizontalAlignment.Right] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│      ██│
└────────┘"""

    [<Test>]
    let ``Horizontal stack test items aligned top`` () =
        let g = createGraphics()

        stack [ Orientation Horizontal; ] [
          image [VerticalAlignment VerticalAlignment.Top] point2x2
          image [VerticalAlignment VerticalAlignment.Top] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│████    │
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Horizontal stack aligned right items aligned bottom test`` () =
        let g = createGraphics()

        stack [ Orientation Horizontal; HorizontalAlignment HorizontalAlignment.Right] [
          image [VerticalAlignment VerticalAlignment.Bottom] point2x2
          image [VerticalAlignment VerticalAlignment.Bottom] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│    ████│
└────────┘"""

    [<Test>]
    let ``Horizontal stack items aligned three ways test`` () =
        let g = createGraphics()

        stack [ Orientation Horizontal; ] [
          image [VerticalAlignment VerticalAlignment.Bottom] point2x2
          image [VerticalAlignment VerticalAlignment.Center] point2x2
          image [VerticalAlignment VerticalAlignment.Top] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│    ██  │
│  ▄▄    │
│  ▀▀    │
│██      │
└────────┘"""

    [<Test>]
    let ``Vertical stack test items aligned left`` () =
        let g = createGraphics()

        stack [Orientation Vertical] [
          image [HorizontalAlignment HorizontalAlignment.Left] point2x2
          image [HorizontalAlignment HorizontalAlignment.Left] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│██      │
│██      │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Vertial stack aligned right items aligned right test`` () =
        let g = createGraphics()

        stack [Orientation Vertical; VerticalAlignment VerticalAlignment.Bottom] [
          image [HorizontalAlignment HorizontalAlignment.Right] point2x2
          image [HorizontalAlignment HorizontalAlignment.Right] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│      ██│
│      ██│
└────────┘"""

    [<Test>]
    let ``Vertical stack items aligned three ways test`` () =
        let g = createGraphics()

        stack [Orientation Vertical] [
          image [HorizontalAlignment HorizontalAlignment.Right] point2x2
          image [HorizontalAlignment HorizontalAlignment.Center] point2x2
          image [HorizontalAlignment HorizontalAlignment.Left] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│      ██│
│   ██   │
│██      │
│        │
└────────┘"""

    [<Test>]
    let ``Horizontal stack items' horizontal alignment should be ignored test`` () =
        let g = createGraphics()

        stack [ Orientation Horizontal; ] [
          image [HorizontalAlignment HorizontalAlignment.Right] point2x2
          image [HorizontalAlignment HorizontalAlignment.Right] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│████    │
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Two horizontal stacks test`` () =
        let g = createGraphics()

        dock [] [
            stack [ Orientation Horizontal; Dock Top] [
              image [HorizontalAlignment HorizontalAlignment.Right] point2x2
              image [HorizontalAlignment HorizontalAlignment.Right] point2x2
            ]
            stack [ Orientation Horizontal; Dock Bottom] [
              image [HorizontalAlignment HorizontalAlignment.Right] point2x2
              image [HorizontalAlignment HorizontalAlignment.Right] point2x2
            ]
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│████    │
│        │
│        │
│████    │
└────────┘"""

    [<Test>]
    let ``Stack panel margin padding test `` () =
        let g = createGraphics()

        stack [ Orientation Horizontal; Margin (thickness 1 1 0 1); Padding (thickness 1 1 0 1)] [
            image [] point2x2
            image [Margin (thicknessSame 1)] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│  ██ ▄▄ │
│     ▀▀ │
│        │
└────────┘"""

    [<Test>]
    let ``Dock panel with central fill item test `` () =
        let g = createGraphics()

        dock [] [
            image [Dock Left] point2x2
            image [Dock Right] point2x2
            image [Dock Fill; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Top;] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│██ ██ ██│
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Dock padding item margin test `` () =
        let g = createGraphics()

        dock [Padding (thicknessSame 1)] [
            image [Margin (thickness 1 3 0 0)] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│  ██    │
│        │
└────────┘"""

    [<Test>]
    let ``Dock margin and padding test `` () =
        let g = createGraphics()

        dock [Margin (thickness 1 3 0 0); Padding (thicknessSame 1)] [
            image [] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│  ██    │
│        │
└────────┘"""

    [<Test>]
    let ``Vertical stack item margin test`` () =
        let g = createGraphics()

        stack [Orientation Vertical] [
            image [Margin (thickness 1 0 0 0)] point2x2
            image [Margin (thickness 2 1 0 0)] point2x2
            image [Margin (thickness 3 1 0 0)] point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ██     │
│  ▄▄    │
│  ▀▀    │
│   ██   │
└────────┘"""

    [<Test>]
    let ``Render 'A' char test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 9; Height = 14})

        dock [] [
            text [] "A"
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌─────────┐
│    ▄    │
│   ▄▀▄   │
│  ▄▀ ▀▄  │
│ ▄█▄▄▄█▄ │
│▄▀     ▀▄│
│▀       ▀│
│         │
└─────────┘"""

    [<Test>]
    let ``Render 'a' char test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 7; Height = 14})

        dock [] [
            text [] "a"
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌───────┐
│       │
│       │
│▄▀▀▀▀▄ │
│▄▀▀▀▀█ │
│█    █ │
│ ▀▀▀▀ ▀│
│       │
└───────┘"""

    [<Test>]
    let ``Render "123" string test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 15; Height = 14})

        dock [] [
            text [] "123"
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌───────────────┐
│  ▄  ▄▄▄   ▄▄▄ │
│▀▀█ █   █ ▀   █│
│  █    ▄▀   ▄▄▀│
│  █  ▄▀       █│
│  █ █     ▄   █│
│  ▀ ▀▀▀▀▀  ▀▀▀ │
│               │
└───────────────┘"""

    [<Test>]
    let ``Render text wrap word test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 22; Height = 30})

        text [TextWrapping Word] "111 111"
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌──────────────────────┐
│  ▄   ▄   ▄           │
│▀▀█ ▀▀█ ▀▀█           │
│  █   █   █           │
│  █   █   █           │
│  █   █   █           │
│  ▀   ▀   ▀           │
│                      │
│                      │
│  ▄   ▄   ▄           │
│▀▀█ ▀▀█ ▀▀█           │
│  █   █   █           │
│  █   █   █           │
│  █   █   █           │
│  ▀   ▀   ▀           │
│                      │
└──────────────────────┘"""

    [<Test>]
    let ``Render text wrap char test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 12; Height = 30})

        text [TextWrapping Char] "123"
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────────┐
│  ▄  ▄▄▄    │
│▀▀█ █   █   │
│  █    ▄▀   │
│  █  ▄▀     │
│  █ █       │
│  ▀ ▀▀▀▀▀   │
│            │
│            │
│ ▄▄▄        │
│▀   █       │
│  ▄▄▀       │
│    █       │
│▄   █       │
│ ▀▀▀        │
│            │
└────────────┘"""

    [<Test>]
    let ``Render text wrap char outside of bounds test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 12; Height = 22})

        text [TextWrapping Char] "123"
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────────┐
│  ▄  ▄▄▄    │
│▀▀█ █   █   │
│  █    ▄▀   │
│  █  ▄▀     │
│  █ █       │
│  ▀ ▀▀▀▀▀   │
│            │
│            │
│ ▄▄▄        │
│▀   █       │
│  ▄▄▀       │
└────────────┘"""

    [<Test>]
    let ``Render "123" string out of bounds test`` () =
        let g = createGraphics()

        dock [] [
            text [] "123"
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│  ▄  ▄▄▄│
│▀▀█ █   │
│  █    ▄│
│  █  ▄▀ │
└────────┘"""

    [<Test>]
    let ``Dock panel image out of bounds`` () =
        let g = createGraphics()

        let margin:IAttribute list = [Margin (thickness 1 0 0 0)]
        dock [] [
            image margin point2x2
            image margin point2x2
            image margin point2x2
            image margin point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ██ ██ █│
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Stack panel image out of bounds`` () =
        let g = createGraphics()

        let margin:IAttribute list = [Margin (thickness 0 1 0 0 )]
        stack [Orientation Vertical] [
            image margin point2x2
            image margin point2x2
            image margin point2x2
            image margin point2x2
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▄▄      │
│▀▀      │
│██      │
│▄▄      │
└────────┘"""

    [<Test>]
    let ``Border thickness test`` () =
        let g = createGraphics()
        let emptyGraphics = Graphics(AddressingMode.ColumnMajor, Little, Size.empty)

        border [Thickness (thickness 1 2 3 1); Margin (thicknessSame 1)] (
            image [] emptyGraphics
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ▄▄▄▄▄▄ │
│ █▀▀███ │
│ █  ███ │
│ ▀▀▀▀▀▀ │
└────────┘"""

    [<Test>]
    let ``Border with child test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); Padding (thicknessSame 1)] (
            image [] point2x2
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█▀▀▀▀▀▀█│
│█ ██   █│
│█      █│
│█▄▄▄▄▄▄█│
└────────┘"""

    [<Test>]
    let ``Border with child aligned center test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); Padding (thicknessSame 1)] (
            image [HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] point2x2
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█▀▀▀▀▀▀█│
│█  ▄▄  █│
│█  ▀▀  █│
│█▄▄▄▄▄▄█│
└────────┘"""

    [<Test>]
    let ``Canvas draw centered line test`` () =
        let g = createGraphics()

        canvas [HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center; Width 3; Height 1] [
            Visual.Line (Point.zero, {X = 3; Y = 0})
        ]
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│  ▄▄▄   │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Border with min width test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); MinWidth 4; Height 6; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] (
            canvas [Width 0; Height 0] []
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│  ▄▄▄▄  │
│  █  █  │
│  █  █  │
│  ▀▀▀▀  │
└────────┘"""

    [<Test>]
    let ``Border with min height test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); MinHeight 4; Width 6; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] (
            canvas [Width 0; Height 0] []
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│ █▀▀▀▀█ │
│ █▄▄▄▄█ │
│        │
└────────┘"""

    [<Test>]
    let ``Border with max width test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); MaxWidth 4; Height 6; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] (
            canvas [Width 10; Height 10] []
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│  ▄▄▄▄  │
│  █  █  │
│  █  █  │
│  ▀▀▀▀  │
└────────┘"""

    [<Test>]
    let ``Border with max height test`` () =
        let g = createGraphics()

        border [Thickness (thicknessSame 1); MaxHeight 4; Width 6; HorizontalAlignment HorizontalAlignment.Center; VerticalAlignment VerticalAlignment.Center] (
            canvas [Width 10; Height 10] []
        )
        |> renderToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│ █▀▀▀▀█ │
│ █▄▄▄▄█ │
│        │
└────────┘"""    