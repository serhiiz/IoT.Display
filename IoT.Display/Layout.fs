namespace IoT.Display

module Layout =

    open System
    open IoT.Display.Graphics
    open Primitives

    type IBaseAttribute = interface end
    type IStackPanelAttribute =
        inherit IBaseAttribute
    type IBorderAttribute =
        inherit IBaseAttribute
    type IAttribute = 
        inherit IStackPanelAttribute
        inherit IBorderAttribute

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
        interface IAttribute

    type StackPanelOrientation = 
        | Horizontal
        | Vertical

    type StackPanelAttribute = 
        | Orientation of StackPanelOrientation
        interface IStackPanelAttribute

    type BorderAttribute = 
        | Thickness of Thickness
        interface IBorderAttribute

    type LayoutElement =
        | StackPanel of (IStackPanelAttribute list * LayoutElement list)
        | DockPanel of (IAttribute list * LayoutElement list)
        | Border of (IBorderAttribute list * LayoutElement)
        | Text of (IAttribute list * string)
        | Image of (IAttribute list * Graphics)

    type private Properties = { Width: int option; Height: int option; Margin: Thickness; Padding: Thickness; HorizontalAlignment: HorizontalAlignment; VerticalAlignment: VerticalAlignment; Dock: Dock}

    let inline stack (attributes:IStackPanelAttribute list) children =
        StackPanel (attributes, children)

    let inline dock (attributes:IAttribute list) children =
        DockPanel (attributes, children)

    let inline border (attributes:IBorderAttribute list) child =
        Border (attributes, child)

    let inline text (attributes:IAttribute list) str =
        Text (attributes, str)

    let inline image (attributes:IAttribute list) buffer =
        Image (attributes, buffer)

    let private measureChar c = 
        let data = FontClass.getCharData c 
        let len = data |> Array.length 
        len / 2

    let private measureString (str:string) = 
        str
        |> Seq.sumBy measureChar
        |> (fun p -> {Width = p + Math.Max(str.Length - 1, 0); Height = FontClass.fontHeight})

    let private tryCastAttribute<'T when 'T :> IBaseAttribute> (attr:IBaseAttribute) =
        match attr with
        | :? 'T as a -> Some a
        | _ -> None

    let private getGenericProperties element = 
        let createProps attributes =
            let emptyProps = {Width=None; Height=None; Margin = emptyThickness; Padding = emptyThickness; HorizontalAlignment=HorizontalAlignment.Stretch; VerticalAlignment=Stretch; Dock = Left}
            let acc (s:Properties) (i:IAttribute) =
                match i with
                | :? Attribute as a -> 
                    match a with
                    | Width w -> {s with Width = Some w}
                    | Height h -> {s with Height = Some h}
                    | Margin m -> {s with Margin = m }
                    | Padding p -> {s with Padding = p}
                    | HorizontalAlignment a -> {s with HorizontalAlignment = a}
                    | VerticalAlignment a -> {s with VerticalAlignment = a}
                    | Dock d -> {s with Dock = d}
                | _ -> s
            attributes
            |> List.fold acc emptyProps

        match element with
        | Text (props, _) -> props |> createProps
        | StackPanel (props, _) -> props |> List.choose tryCastAttribute<IAttribute> |> createProps
        | Image (props, _) -> props |> createProps
        | DockPanel (props, _) -> props |> createProps
        | Border (props, _) -> props |> List.choose tryCastAttribute<IAttribute> |> createProps

    let private getStackPanelOrientation attrs = 
        let acc orientation (attribute:IStackPanelAttribute) = 
            match attribute with 
            | :? StackPanelAttribute as spa ->
                match spa with 
                | Orientation o -> Some o
            | _ -> orientation

        attrs 
        |> List.fold acc None
        |> Option.defaultValue StackPanelOrientation.Vertical

    let private getBorderThickness attrs = 
        let acc thickness (attribute:IBorderAttribute) = 
            match attribute with 
            | :? BorderAttribute as spa ->
                match spa with 
                | Thickness t -> Some t
            | _ -> thickness

        attrs 
        |> List.fold acc None
        |> Option.defaultValue Thickness.emptyThickness

    let rec measure (maxSize:Size) (element:LayoutElement) : Size = 
        let measureCore = function
            | Text (_, text) -> measureString text
            | Image (_, b) -> b.Size
            | StackPanel (attrs, childs) ->
                let measureChild (size : Size, cons) child = 
                    let m = measure cons child
                    let orientation = getStackPanelOrientation attrs
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
                    let childProps = child |> getGenericProperties
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
            | Border (attrs, child) ->
                let thickness = getBorderThickness attrs
                measure maxSize child |> (+) (thickness |> Thickness.toSize)

        let props = element |> getGenericProperties
        let coreSize = lazy (element |> measureCore |> (+) (props.Padding |> Thickness.toSize))

        {Width = props.Width |> Option.defaultWith (fun () -> coreSize.Value.Width); Height = props.Height |> Option.defaultWith (fun () -> coreSize.Value.Height)} 
        |> (+) (props.Margin |> Thickness.toSize) 
        |> applyBounds maxSize
    
    let private renderGraphics (targetGraphics:Graphics) rect origin (graphics:Graphics) =
        let size = Size.min rect.Size graphics.Size
        if isPointWithin rect origin then
            for iy = 0 to size.Height - 1 do
                for ix = 0 to size.Width - 1 do
                    if graphics.GetPixel ix iy = 1uy then
                        let p = {X = ix + origin.X; Y = iy + origin.Y}
                        if (isPointWithin rect p) then
                            targetGraphics.SetPixel p.X p.Y

    let private renderString graphics (rect:Rect) str =
        let acc origin c =
            let g = FontClass.getCharGraphics c
            renderGraphics graphics rect origin g
            {origin with X = origin.X + g.Size.Width + 1}
        
        str |> Seq.fold acc rect.Point |> ignore

    let rec private render (graphics:Graphics) (area:Rect) element =
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

        let props = element |> getGenericProperties
        let desiredSize = measure area.Size element 
        let marginArea = shrink area props.Margin
        
        let rect = getRenderRect marginArea desiredSize props.HorizontalAlignment props.VerticalAlignment
        
        match element with
            | Text (_, s) -> s |> renderString graphics rect
            | StackPanel (attrs, childs) -> 
                let paddingArea = shrink rect props.Padding
                let orientation = getStackPanelOrientation attrs
                let folder remainingRect c = 
                    let m = measure remainingRect.Size c
                    let (renderRect, remaining) = 
                        match orientation with 
                        | Horizontal -> 
                            (shrink remainingRect (thickness 0 0 (remainingRect.Size.Width - m.Width) 0),
                                shrink remainingRect (thickness m.Width 0 0 0))
                        | Vertical -> 
                            (shrink remainingRect (thickness 0 0 0 (remainingRect.Size.Height - m.Height)),
                                shrink remainingRect (thickness 0 m.Height 0 0))
                    render graphics renderRect c
                    remaining
                childs |> List.fold folder paddingArea |> ignore
            | Image (_, g) -> 
                renderGraphics graphics rect rect.Point g
            | DockPanel (_, childs) ->
                let paddingArea = shrink rect props.Padding
                let folder remainingRect c = 
                    let m = measure remainingRect.Size c
                    let props = c |> getGenericProperties
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

                    render graphics renderRect c
                    remaining
                childs |> List.fold folder paddingArea |> ignore
            | Border (attrs, child) -> 
                let bt = getBorderThickness attrs
                [ 
                    Rectangle (shrink rect (thickness 0 0 (rect.Size.Width - bt.Left) 0)) 
                    Rectangle (shrink rect (thickness 0 0 0 (rect.Size.Height - bt.Top))) 
                    Rectangle (shrink rect (thickness (rect.Size.Width - bt.Right) 0 0 0))
                    Rectangle (shrink rect (thickness 0 (rect.Size.Height - bt.Bottom) 0 0))
                ]
                |> List.iter (renderVisualToGraphics graphics)
                let childRect = shrink rect (bt + props.Padding)
                render graphics childRect child

    let renderToGraphics (graphics:Graphics) element =
        render graphics (graphics.Size |> Rect.fromSize) element

    let renderToDisplay (display:IDisplay) element =
        let graphics = Graphics.createFromDisplay display
        renderToGraphics graphics element
        display.SendData (graphics.GetBuffer())