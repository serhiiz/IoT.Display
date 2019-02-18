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
    [<TestCase(1,6,6,1)>]
    [<TestCase(6,1,1,6)>]
    let ``Rectangle test`` x1 y1 x2 y2 =
        let g = createGraphics()

        Rectangle ({X = x1; Y = y1}, {X = x2; Y = y2})
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