namespace IoT.Display

module Graphics =

    type IGraphics = 
        abstract member Size : Size with get
        abstract member SetPixel : x:int -> y:int -> unit
        abstract member GetPixel : x:int -> y:int -> byte

    type AddressingMode = 
        | RowMajor 
        | ColumnMajor
        | Page

    type Endianness = 
        | Little 
        | Big

    type IGraphicDispalyMemory = 
        inherit IGraphics
        abstract member GetBuffer : unit -> byte[]
        abstract member AddressingMode : AddressingMode with get
        abstract member Endianness : Endianness with get

    [<Literal>]
    let BitsInByte = 8   

    let private pxToBytes px = px / BitsInByte + (if px % BitsInByte <> 0 then 1 else 0)
    let private getBufferLength widthBytes heightBytes size = function
        | Page
        | ColumnMajor -> heightBytes * size.Width
        | RowMajor -> widthBytes * size.Height

    type private Graphics private(mode, endianness, size, buffer, widthBytes, heightBytes) =
        let getByteIndex x y = function
            | Page -> x + (y / BitsInByte) * size.Width
            | ColumnMajor -> x * heightBytes + y / BitsInByte
            | RowMajor -> y * widthBytes + x / BitsInByte

        let getBitIndex x y mode endianness = 
            match (mode, endianness) with
            | (Page, Little)
            | (ColumnMajor, Little) -> y % BitsInByte
            | (RowMajor, Little) -> x % BitsInByte
            | (Page, Big)
            | (ColumnMajor, Big) -> BitsInByte - y % BitsInByte - 1
            | (RowMajor, Big) -> BitsInByte - x % BitsInByte - 1
        
        new(mode, endianness, size) = 
            let widthBytes = size.Width |> pxToBytes
            let heightBytes = size.Height |> pxToBytes
            let bufferLength = getBufferLength widthBytes heightBytes size mode
            let buffer = Array.zeroCreate (bufferLength)
            Graphics(mode, endianness, size, buffer, widthBytes, heightBytes)
        
        new(mode, endianness, size, buffer:byte[]) = 
            let widthBytes = size.Width |> pxToBytes
            let heightBytes = size.Height |> pxToBytes
            let expectedLength = getBufferLength widthBytes heightBytes size mode
            if (buffer.Length <> expectedLength)
                then invalidArg "buffer" (sprintf "The length of the array is invalid. Expected %i, but got %i." expectedLength buffer.Length)
            Graphics(mode, endianness, size, buffer, widthBytes, heightBytes)
   
        interface IGraphicDispalyMemory with
            member __.Size with get () = size

            member __.SetPixel x y =
                let index = getByteIndex x y mode  
                let pos = getBitIndex x y mode endianness
                let value = 1uy <<< pos
                buffer.[index] <- buffer.[index] ||| value

            member __.GetPixel x y = 
                let index = getByteIndex x y mode  
                let pos = getBitIndex x y mode endianness
                let value = 1uy <<< pos
                (buffer.[index] &&& value) >>> pos

            member __.GetBuffer() = buffer        
            member __.AddressingMode with get() = mode
            member __.Endianness with get () = endianness

    type GraphicsWindow(graphics:IGraphics, rect:Rect) =
        let actualRect = Rect.getIntersection (Rect.fromSize graphics.Size) rect
        interface IGraphics with
            member __.Size with get () = actualRect.Size

            member __.SetPixel x y =
                graphics.SetPixel (actualRect.Point.X + x) (actualRect.Point.Y + y)

            member __.GetPixel x y = 
                graphics.GetPixel (actualRect.Point.X + x) (actualRect.Point.Y + y)

    let createFromSize mode endianness size = 
        Graphics (mode, endianness, size) :> IGraphicDispalyMemory

    let createFromBuffer mode endianness size buffer = 
        Graphics (mode, endianness, size, buffer) :> IGraphicDispalyMemory

    let createDefault size = 
        createFromSize Page Little size

    let createWindow graphics rect = 
        GraphicsWindow (graphics, rect) :> IGraphics

    let renderToString (g:IGraphics) =
        let lines = 
            List.init (g.Size.Height / 2) id 
            |> List.map (fun j -> 
                let chars = 
                    List.init g.Size.Width id 
                    |> List.map (fun i -> 
                        let t = g.GetPixel i (j*2)
                        let b = g.GetPixel i ((j*2) + 1)
                        match (t, b) with 
                        | 1uy, 1uy -> '█'
                        | 0uy, 1uy -> '▄'
                        | 1uy, 0uy -> '▀'
                        | _ -> ' ')
                System.String(('│' :: chars @ ['│']) |> List.toArray))
        let header = new System.String(('┌' :: List.replicate g.Size.Width '─' @ ['┐']) |> List.toArray)
        let footer = new System.String(('└' :: List.replicate g.Size.Width '─' @ ['┘']) |> List.toArray)
        System.String.Join(System.Environment.NewLine, header :: lines @ [footer])

    let copyTo targetRect (targetGraphics:IGraphics) sourceRect (sourceGraphics:IGraphics) = 
        let copySourceRect = Rect.getIntersection (Rect.fromSize sourceGraphics.Size) sourceRect
        let copyTargetRect = Rect.getIntersection (Rect.fromSize targetGraphics.Size) targetRect
        let copySize = Size.combine (fun a b -> System.Math.Min(a,b)) copySourceRect.Size copyTargetRect.Size

        for i = 0 to copySize.Width - 1 do   
            for j = 0 to copySize.Height - 1 do 
                if (sourceGraphics.GetPixel (i + copySourceRect.Point.X) (j + copySourceRect.Point.Y) = 1uy) then
                    targetGraphics.SetPixel (i + copyTargetRect.Point.X) (j + copyTargetRect.Point.Y)

    let clip rect (graphics:IGraphicDispalyMemory) = 
        let copyRect = Rect.getIntersection (Rect.fromSize graphics.Size) rect
        let newGraphics = createFromSize graphics.AddressingMode graphics.Endianness copyRect.Size
        copyTo (Rect.fromSize copyRect.Size) newGraphics rect graphics
        newGraphics