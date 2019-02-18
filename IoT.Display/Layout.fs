namespace IoT.Display

module Layout =

    open System
    open IoT.Display.Graphics

    type HorizontalAlignment = 
        | Left
        | Right
        | Center
        | Stretch

    type VerticalAlignment =
        | Top
        | Bottom
        | Center
        | Stretch

    type Dock = 
        | Left
        | Top
        | Right
        | Bottom
        | Fill

    type Attribute =
        | Width of int
        | Height of int
        | Margin of Thickness
        | Padding of Thickness
        | HorizontalAlignment of HorizontalAlignment
        | VerticalAlignment of VerticalAlignment
        | Dock of Dock

    type StackPanelOrientation = 
        | Horizontal
        | Vertical

    type Properties = { Width: int option; Height: int option; Margin: Thickness; Padding: Thickness; HorizontalAlignment: HorizontalAlignment; VerticalAlignment: VerticalAlignment; Dock: Dock}

    type LayoutElement =
        | StackPanel of (StackPanelOrientation*Properties*LayoutElement list)
        | DockPanel of (Properties *LayoutElement list)
        | Text of (Properties*string)
        | Image of (Properties*Graphics)

    let createProps attributes =
        let emptyProps = {Width=None; Height=None; Margin = emptyThickness; Padding = emptyThickness; HorizontalAlignment=HorizontalAlignment.Stretch; VerticalAlignment=Stretch; Dock = Left}
        let acc (s:Properties) (i:Attribute) =
            match i with 
            | Width w -> {s with Width = Some w}
            | Height h -> {s with Height = Some h}
            | Margin m -> {s with Margin = m }
            | Padding p -> {s with Padding = p}
            | HorizontalAlignment a -> {s with HorizontalAlignment = a}
            | VerticalAlignment a -> {s with VerticalAlignment = a}
            | Dock d -> {s with Dock = d}

        attributes
        |> List.fold acc emptyProps

    let inline stack orientation (attributes:Attribute list) children =
        StackPanel (orientation, attributes |> createProps, children)

    let inline dock (attributes:Attribute list) children =
        DockPanel (attributes |> createProps, children)

    let inline text (attributes:Attribute list) str =
        Text (attributes |> createProps, str)

    let inline image (attributes:Attribute list) buffer =
        Image (attributes |> createProps, buffer)

    let private measureChar c = 
        let data = FontClass.getCharData c 
        let len = data |> Array.length 
        len / 2

    let private measureString (str:string) = 
        str
        |> Seq.sumBy measureChar
        |> (fun p -> {Width = p + Math.Max(str.Length - 1, 0); Height = FontClass.fontHeight})

    let private getProps = function
        | Text (props, _) -> props
        | StackPanel (_, props, _) -> props
        | Image (props, _) -> props
        | DockPanel (props, _) -> props

    let rec measure (maxSize:Size) (element:LayoutElement) : Size = 
        let measureCore = function
            | Text (_, text) -> measureString text
            | Image (_, b) -> b.Size
            | StackPanel (orientation, _, childs) ->
                let measureChild (size : Size, cons) child = 
                    let m = measure cons child
                    match orientation with
                    | Horizontal -> 
                        let size' = {Width = size.Width + m.Width; Height = Math.Max(size.Height, m.Height)}
                        let cons' = {Width = Math.Max(cons.Width - m.Width, 0); Height = cons.Height}
                        (size', cons')
                    | Vertical -> 
                        let size' = {Width = Math.Max(size.Width, m.Width); Height = size.Height + m.Height}
                        let cons' = {Width = cons.Width; Height = Math.Max(cons.Height - m.Height, 0)}
                        (size', cons')

                childs |> List.fold measureChild (Size.empty, maxSize) |> fst

            | DockPanel (_, childs) ->
                let measureChild (size : Size, cons) child = 
                    let m = measure cons child
                    let childProps = child |> getProps
                    match childProps.Dock with
                    | Left
                    | Right -> 
                        let size' = {Width = size.Width + m.Width; Height = Math.Max(size.Height, m.Height)}
                        let cons' = {Width = Math.Max(cons.Width - m.Width, 0); Height = cons.Height}
                        (size', cons')
                    | Top
                    | Bottom -> 
                        let size' = {Width = Math.Max(size.Width, m.Width); Height = size.Height + m.Height}
                        let cons' = {Width = cons.Width; Height = Math.Max(cons.Height - m.Height, 0)}
                        (size', cons')
                    | Fill -> 
                        let size' = {Width = size.Width + cons.Width; Height = size.Height + cons.Height}
                        let cons' = {Width = 0; Height = 0}
                        (size', cons')

                childs |> List.fold measureChild (Size.empty, maxSize) |> fst
            
        
        let props = element |> getProps
        let coreSize = element |> measureCore |> (+) (props.Padding |> Thickness.toSize)

        {Width = props.Width |> Option.defaultValue coreSize.Width; Height = props.Height |> Option.defaultValue coreSize.Height} 
        |> (+) (props.Margin |> Thickness.toSize) 
        |> applyBounds maxSize
    
    let private renderGraphics writePixel rect origin (graphics:Graphics) =
        let size = Size.min rect.Size graphics.Size
        if isPointWithin rect origin then
            for iy = 0 to size.Height - 1 do
                for ix = 0 to size.Width - 1 do
                    if graphics.GetPixel ix iy = 1uy then
                        let p = {X = ix + origin.X; Y = iy + origin.Y}
                        if (isPointWithin rect p) then
                            writePixel p.X p.Y

    let private renderString writePixel (rect:Rect) str =
        let acc origin c =
            let g = FontClass.getCharGraphics c
            renderGraphics writePixel rect origin g
            {origin with X = origin.X + g.Size.Width + 1}
        
        str |> Seq.fold acc rect.Point |> ignore

    let rec private render writePixel (area:Rect) element =
        let getRenderRect (r:Rect) (desiredSize:Size) hAlignment vAlignment =
            let (x,w) = 
                match hAlignment with
                | HorizontalAlignment.Stretch -> (0, r.Size.Width)
                | HorizontalAlignment.Left -> (0, desiredSize.Width)
                | HorizontalAlignment.Right -> ((r.Size.Width - desiredSize.Width), desiredSize.Width)
                | HorizontalAlignment.Center -> ((r.Size.Width - desiredSize.Width)/2, desiredSize.Width)
            let (y,h) = 
                match vAlignment with
                | VerticalAlignment.Stretch -> (0, r.Size.Height)
                | VerticalAlignment.Top -> (0, desiredSize.Height)
                | VerticalAlignment.Bottom -> ((r.Size.Height - desiredSize.Height), desiredSize.Height)
                | VerticalAlignment.Center -> ((r.Size.Height - desiredSize.Height)/2, desiredSize.Height)
            
            {Point = {X = x; Y = y} + r.Point; Size = {Width = w; Height = h}}

        let props = element |> getProps
        let desiredSize = measure area.Size element 
        let marginArea = shrink area props.Margin
        
        let rect = getRenderRect marginArea desiredSize props.HorizontalAlignment props.VerticalAlignment
        
        match element with
            | Text (_, s) -> s |> renderString writePixel rect
            | StackPanel (o, _, childs) -> 
                let paddingArea = shrink rect props.Padding
                let folder remainingRect c = 
                    let m = measure remainingRect.Size c
                    let (renderRect, remaining) = 
                        match o with 
                        | Horizontal -> 
                            (shrink remainingRect (thickness 0 0 (remainingRect.Size.Width - m.Width) 0),
                                shrink remainingRect (thickness m.Width 0 0 0))
                        | Vertical -> 
                            (shrink remainingRect (thickness 0 0 0 (remainingRect.Size.Height - m.Height)),
                                shrink remainingRect (thickness 0 m.Height 0 0))
                    render writePixel renderRect c
                    remaining
                childs |> List.fold folder paddingArea |> ignore
            | Image (_, graphics) -> 
                renderGraphics writePixel rect rect.Point graphics
            | DockPanel (_, childs) ->
                let paddingArea = shrink rect props.Padding
                let folder remainingRect c = 
                    let m = measure remainingRect.Size c
                    let props = c |> getProps
                    let (renderRect, remaining) = 
                        match props.Dock with
                        | Left -> 
                            (shrink remainingRect (thickness 0 0 (remainingRect.Size.Width - m.Width) 0),
                                shrink remainingRect (thickness m.Width 0 0 0))
                        | Top ->
                            (shrink remainingRect (thickness 0 0 0 (remainingRect.Size.Height - m.Height)),
                                shrink remainingRect (thickness 0 m.Height 0 0))
                        | Right ->
                            (shrink remainingRect (thickness (remainingRect.Size.Width - m.Width) 0 0 0),
                                shrink remainingRect (thickness 0 0 m.Width 0))
                        | Bottom ->
                            (shrink remainingRect (thickness 0 (remainingRect.Size.Height - m.Height) 0 0),
                                shrink remainingRect (thickness 0 0 0 m.Height))
                        | Fill ->
                            (remainingRect, {remainingRect with Size = Size.empty})

                    render writePixel renderRect c
                    remaining
                childs |> List.fold folder paddingArea |> ignore

    let renderToGraphics (graphics:Graphics) element =
        render graphics.SetPixel (graphics.Size |> Rect.fromSize) element

    let renderToDisplay (display:IDisplay) element =
        let graphics = Graphics.createFromDisplay display
        renderToGraphics graphics element
        display.SendData (graphics.GetBuffer())