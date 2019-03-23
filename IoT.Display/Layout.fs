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
    type ITextAttribute = 
        inherit IBaseAttribute
    type IAttribute = 
        inherit IStackPanelAttribute
        inherit IBorderAttribute
        inherit ITextAttribute

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
        | MinWidth of int
        | MaxWidth of int
        | MinHeight of int
        | MaxHeight of int
        interface IAttribute

    type StackPanelOrientation = 
        | Horizontal
        | Vertical

    type StackPanelAttribute = 
        | Orientation of StackPanelOrientation
        interface IStackPanelAttribute

    type TextWrapping = 
        | Word
        | Char
        | None

    type TextAttribute = 
        | TextWrapping of TextWrapping
        | TextAlignment of HorizontalAlignment
        interface ITextAttribute

    type BorderAttribute = 
        | Thickness of Thickness
        interface IBorderAttribute

    type LayoutElement =
        | StackPanel of (IStackPanelAttribute list * LayoutElement list)
        | DockPanel of (IAttribute list * LayoutElement list)
        | Border of (IBorderAttribute list * LayoutElement)
        | Text of (ITextAttribute list * string)
        | Image of (IAttribute list * IGraphics)
        | Canvas of (IAttribute list * Visual list)

    type private Properties = {
        Width: int option; 
        Height: int option; 
        Margin: Thickness; 
        Padding: Thickness; 
        HorizontalAlignment: HorizontalAlignment; 
        VerticalAlignment: VerticalAlignment; 
        Dock: Dock; 
        MinWidth : int option
        MaxWidth : int option
        MinHeight : int option
        MaxHeight : int option
    }

    type private TextProperties = {
        Wrapping: TextWrapping
        Alignment: HorizontalAlignment
    }

    let inline stack (attributes:IStackPanelAttribute list) children =
        StackPanel (attributes, children)

    let inline dock (attributes:IAttribute list) children =
        DockPanel (attributes, children)

    let inline border (attributes:IBorderAttribute list) child =
        Border (attributes, child)

    let inline text (attributes:ITextAttribute list) str =
        Text (attributes, str)

    let inline image (attributes:IAttribute list) buffer =
        Image (attributes, buffer)

    let inline canvas (attributes:IAttribute list) children =
        Canvas (attributes, children)

    let measureChar c = 
        let data = FontClass.getCharData c 
        let len = data |> Array.length 
        {Width = len / 2; Height = FontClass.fontHeight}

    let measureLine (str:string) = 
        str
        |> Seq.fold (fun m c -> let charSize = measureChar c in {Width = m.Width + charSize.Width; Height = Math.Max(m.Height, charSize.Height)}) Size.empty
        |> (fun m -> {m with Width = m.Width + if str.Length > 1 then (str.Length - 1) * FontClass.charSpacing else 0})

    let private getLineLengthCore<'T> spacingWidth spacingChars measureWidth maxLineWidth (getLength: 'T -> int) (items: 'T seq) =
        let acc (x, numberOfChars, resultLengths) i = 
            let charWidth = measureWidth i
            if (x + charWidth > maxLineWidth) then 
                // Line break
                (charWidth + spacingWidth, getLength i, numberOfChars :: resultLengths)
            else 
                (x + charWidth + spacingWidth, numberOfChars + getLength i + (if x = 0 then 0 else spacingChars), resultLengths)
        items
        |> Seq.fold acc (0, 0, [])
        |> (fun (_, l, ls) -> if l > 0 then l :: ls else ls)
        |> Seq.rev

    let private getTextLines textWrapping maxLineWidth (str:string) =
        let splitToWords inputString =
            let acc (hasChar, word, words) c = 
                match (hasChar, c) with
                | (false, ' ') -> (false, word + (c.ToString()), words)
                | (true, ' ') -> (false, "", word :: words)
                | _ -> (true, word + (c.ToString()), words)
            
            inputString
            |> Seq.fold acc (false, "", []) 
            |> (fun (hasChar, word, words) -> if hasChar && word.Length > 0 then word :: words else words) 
            |> Seq.toArray 
            |> Array.rev

        let splitByLength spacingChars (inputString:string) lens = 
            let acc (index, list) length =
                let s = inputString.Substring(index, length)
                (index + length + spacingChars, s :: list)
            lens
            |> Seq.fold acc (0, [])
            |> snd
            |> List.rev

        match textWrapping with 
        | TextWrapping.None -> [str]
        | TextWrapping.Char ->
            str
            |> getLineLengthCore FontClass.charSpacing 0 (measureChar >> (fun p -> p.Width)) maxLineWidth (fun _ -> 1)
            |> splitByLength 0 str
        | TextWrapping.Word ->
            str
            |> splitToWords
            |> getLineLengthCore FontClass.wordSpacing 1 (measureLine >> (fun p -> p.Width)) maxLineWidth (fun s -> s.Length)
            |> splitByLength 1 str

    let rec private measureString textWrapping (maxSize:Size) (str:string) : Size = 
        let acc (size:Size) string = 
            let len = measureLine string
            {Width = Math.Max(size.Width, len.Width); Height = size.Height + (if size = Size.empty then 0 else FontClass.lineSpacing) + len.Height}
        str
        |> getTextLines textWrapping maxSize.Width
        |> List.fold acc Size.empty
        |> applyBounds maxSize

    let private tryCastAttribute<'T when 'T :> IBaseAttribute> (attr:IBaseAttribute) =
        match attr with
        | :? 'T as a -> Some a
        | _ -> Option.None

    let private getGenericProperties element = 
        let createProps attributes =
            let emptyProps = {
                Width = Option.None 
                Height = Option.None 
                Margin = emptyThickness 
                Padding = emptyThickness 
                HorizontalAlignment = HorizontalAlignment.Stretch 
                VerticalAlignment = VerticalAlignment.Stretch 
                Dock = Left
                MinWidth = Option.None
                MaxWidth = Option.None
                MinHeight = Option.None
                MaxHeight = Option.None
            }
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
                    | MinWidth mw -> {s with MinWidth = Some mw}
                    | MaxWidth mw -> {s with MaxWidth = Some mw}
                    | MinHeight mh -> {s with MinHeight = Some mh}
                    | MaxHeight mh -> {s with MaxHeight = Some mh}
                | _ -> s
            attributes
            |> List.fold acc emptyProps

        match element with
        | Text (props, _) -> props |> List.choose tryCastAttribute<IAttribute> |> createProps
        | StackPanel (props, _) -> props |> List.choose tryCastAttribute<IAttribute> |> createProps
        | Image (props, _) -> props |> createProps
        | DockPanel (props, _) -> props |> createProps
        | Border (props, _) -> props |> List.choose tryCastAttribute<IAttribute> |> createProps
        | Canvas (props, _ ) -> props |> createProps

    let private getStackPanelOrientation attrs = 
        let acc orientation (attribute:IStackPanelAttribute) = 
            match attribute with 
            | :? StackPanelAttribute as spa ->
                match spa with 
                | Orientation o -> Some o
            | _ -> orientation

        attrs 
        |> List.fold acc Option.None
        |> Option.defaultValue StackPanelOrientation.Vertical

    let private getBorderThickness attrs = 
        let acc thickness (attribute:IBorderAttribute) = 
            match attribute with 
            | :? BorderAttribute as spa ->
                match spa with 
                | Thickness t -> Some t
            | _ -> thickness

        attrs 
        |> List.fold acc Option.None
        |> Option.defaultValue Thickness.emptyThickness

    let private getTextProperties attrs = 
        let empty = {
            Wrapping = TextWrapping.None
            Alignment = HorizontalAlignment.Left
        }
        let acc props (attribute:ITextAttribute) = 
            match attribute with 
            | :? TextAttribute as spa ->
                match spa with 
                | TextWrapping t -> {props with Wrapping = t}
                | TextAlignment a -> {props with Alignment = a}
            | _ -> props

        attrs 
        |> List.fold acc empty

    let rec measure (maxSize:Size) (element:LayoutElement) : Size = 
        let measureCore = function
            | Text (attrs, text) -> 
                let textProps = getTextProperties attrs
                measureString textProps.Wrapping maxSize text
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
            | Canvas _ -> Size.empty

        let props = element |> getGenericProperties
        let coreSize = lazy (element |> measureCore |> (+) (props.Padding |> Thickness.toSize))

        let applyMinMaxSize props (s:Size) =
            let applyOption o f v =
                o |> Option.map (fun ov -> f(v,ov)) |> Option.defaultValue v
            { 
                Width = s.Width |> applyOption props.MinWidth Math.Max |> applyOption props.MaxWidth Math.Min
                Height = s.Height |> applyOption props.MinHeight Math.Max |> applyOption props.MaxHeight Math.Min
            }
            
        {Width = props.Width |> Option.defaultWith (fun () -> coreSize.Value.Width); Height = props.Height |> Option.defaultWith (fun () -> coreSize.Value.Height)} 
        |> (+) (props.Margin |> Thickness.toSize) 
        |> applyMinMaxSize props
        |> applyBounds maxSize

    let private renderString textProperties (rect:Rect) graphics (str:string) = 
        let renderChar targetGraphics maxRext charSpacing cursor c = 
            let charGraphics = FontClass.getCharGraphics c
            let targetRect = Rect.getIntersection {Point = cursor; Size = charGraphics.Size} maxRext
            let charRect = Rect.fromSize charGraphics.Size
            copyTo targetRect targetGraphics charRect charGraphics
            {cursor with X = cursor.X + charGraphics.Size.Width + charSpacing}

        let renderLine lineStartCursor string = 
            let strLength = lazy measureLine string
            let (cursorX, spacing) = 
                match textProperties.Alignment with 
                | HorizontalAlignment.Right -> lineStartCursor.X + rect.Size.Width - strLength.Value.Width, FontClass.charSpacing
                | HorizontalAlignment.Center -> lineStartCursor.X + (rect.Size.Width - strLength.Value.Width) / 2, FontClass.charSpacing
                | HorizontalAlignment.Stretch when string.Length > 1 -> lineStartCursor.X, ((rect.Size.Width - strLength.Value.Width) / (string.Length - 1)) + FontClass.charSpacing
                | _ -> lineStartCursor.X, FontClass.charSpacing
            string
            |> Seq.fold (renderChar graphics rect spacing) {lineStartCursor with X = cursorX}
            |> ignore
            {lineStartCursor with Y = lineStartCursor.Y + FontClass.fontHeight + FontClass.lineSpacing}
        
        str
        |> getTextLines textProperties.Wrapping rect.Size.Width
        |> List.fold renderLine rect.Point
        |> ignore
        

    let rec private render (graphics:IGraphics) (area:Rect) element =
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
            | Text (attrs, s) -> 
                let textProps = getTextProperties attrs
                s |> renderString textProps rect graphics
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
                copyTo rect graphics (Rect.fromSize g.Size) g
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
            | Canvas (_, children) ->
                let g = GraphicsWindow(graphics, rect)
                children |> List.iter (renderVisualToGraphics g)

    let renderToGraphics (graphics:IGraphics) element =
        render graphics (graphics.Size |> Rect.fromSize) element