namespace IoT.Display.Tests

open NUnit.Framework
open IoT.Display
open IoT.Display.Graphics
open IoT.Display.Primitives

module RenderPrimitivesTests = 
    let width = 8
    let height = 8
    
    let createGraphics() = 
        Graphics(ColumnMajor, Little, {Size.Width = width; Height = height})
    
    [<Test>]
    let ``Dot in left top corner`` () =
        let g = createGraphics()

        Dot {X = 0; Y = 0}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▀       │
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Dot in right bottom corner test`` () =
        let g = createGraphics()

        Dot {X = 7; Y = 7}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│       ▄│
└────────┘"""

    [<Test>]
    let ``Dot out of bounds test`` () =
        let g = createGraphics()

        Dot {X = 8; Y = 8}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│        │
└────────┘"""
    
    [<Test>]
    [<TestCase(0,1,0,6)>]
    [<TestCase(0,6,0,1)>]
    let ``Vertical line test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▄       │
│█       │
│█       │
│▀       │
└────────┘"""

    [<Test>]
    [<TestCase(0,-1,0,16)>]
    [<TestCase(0,16,0,-1)>]
    let ``Vertical line crossing boudary test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█       │
│█       │
│█       │
│█       │
└────────┘"""

    [<Test>]
    [<TestCase(1,0,6,0)>]
    [<TestCase(6,0,1,0)>]
    let ``Horizontal line test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ▀▀▀▀▀▀ │
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    [<TestCase(-1,0,16,0)>]
    [<TestCase(16,0,-1,0)>]
    let ``Horizontal line crossing boudary test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▀▀▀▀▀▀▀▀│
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    [<TestCase(1,1,6,6)>]
    [<TestCase(6,6,1,1)>]
    let ``Major diagonal line test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ▄      │
│  ▀▄    │
│    ▀▄  │
│      ▀ │
└────────┘"""

    [<Test>]
    [<TestCase(1,6,6,1)>]
    [<TestCase(6,1,1,6)>]
    let ``Minor diagonal line test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Line ({X = x1; Y = y1}, {X = x2; Y = y2})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│      ▄ │
│    ▄▀  │
│  ▄▀    │
│ ▀      │
└────────┘"""

    [<Test>]
    let ``Rectangle test`` () =
        let g = createGraphics()

        Rectangle {Point = {X = 1; Y = 1}; Size = {Width = 6; Height = 6}}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ▄▄▄▄▄▄ │
│ ██████ │
│ ██████ │
│ ▀▀▀▀▀▀ │
└────────┘"""

    [<Test>]
    let ``Rectangle outside of boudaries test`` () =
        let g = createGraphics()

        Rectangle {Point = {X = 2; Y = -1}; Size = {Width = 4; Height = 10}}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│  ████  │
│  ████  │
│  ████  │
│  ████  │
└────────┘"""

    [<Test>]
    [<TestCase(0,1)>]
    [<TestCase(1,0)>]
    let ``Rectangle with zero width/height test`` widht height =
        let g = createGraphics()

        Rectangle {Point = {X = 0; Y = 0}; Size = {Width = widht; Height = height}}
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│        │
│        │
│        │
│        │
└────────┘"""

    
    [<Test>]
    let ``Polyline test`` () =
        let g = createGraphics()

        Polyline [{X = 1; Y = 1}; {X = 6; Y = 6}; {X = 6; Y = 0}]
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│ ▄    █ │
│  ▀▄  █ │
│    ▀▄█ │
│      ▀ │
└────────┘"""

    [<Test>]
    let ``Polyline border test`` () =
        let g = createGraphics()

        Polyline [{X = 0; Y = 0}; {X = 0; Y = 7}; {X = 7; Y = 7}; {X = 7; Y = 0}; {X = 0; Y = 0}]
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█▀▀▀▀▀▀█│
│█      █│
│█      █│
│█▄▄▄▄▄▄█│
└────────┘"""

    [<Test>]
    let ``Quadratic Bezier test`` () =
        let g = createGraphics()

        QuadraticBezier ({X = 0; Y = 0}, {X = 0; Y = 7}, {X = 7; Y = 7})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█       │
│▀▄      │
│ █▄     │
│   ▀▀▀▄▄│
└────────┘"""

    [<Test>]
    let ``Quadratic Bezier 16x16 test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 16})

        QuadraticBezier ({X = 0; Y = 0}, {X = 0; Y = 15}, {X = 15; Y = 15})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────────────┐
│█               │
│█               │
│█               │
│▀▄              │
│  █             │
│   ▀▄           │
│     ▀▀▄        │
│        ▀▀▀▀▀▄▄▄│
└────────────────┘"""

    [<Test>]
    let ``Vertical line light slope test`` () =
        let g = createGraphics()

        Line ({X = 0; Y = 0}, {X = 1; Y = 7})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│█       │
│█       │
│ █      │
│ █      │
└────────┘"""

    [<Test>]
    let ``Horizontal line light slope test`` () =
        let g = createGraphics()

        Line ({X = 0; Y = 0}, {X = 7; Y = 1})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▀▀▀▀▄▄▄▄│
│        │
│        │
│        │
└────────┘"""

    [<Test>]
    let ``Non-square graphics diaonal light slope test`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 16; Height = 8})

        Line ({X = 0; Y = 0}, {X = 15; Y = 9})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────────────┐
│▀▄▄             │
│   ▀▀▄          │
│      ▀▀▄▄      │
│          ▀▄▄   │
└────────────────┘"""

    [<Test>]
    let ``Non-square graphics diaonal light slope test 2`` () =
        let g = Graphics(ColumnMajor, Little, {Size.Width = 8; Height = 16})

        Line ({X = 0; Y = 0}, {X = 9; Y = 15})
        |> renderVisualToGraphics g

        let actual = g.ToString()
        assertRender actual """
┌────────┐
│▀▄      │
│ ▀▄     │
│  ▀▄    │
│    █   │
│     █  │
│      ▀▄│
│       ▀│
│        │
└────────┘"""