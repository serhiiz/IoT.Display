namespace IoT.Display

module Graphics =

    type AddressingMode = 
        | RowMajor 
        | ColumnMajor
        | Page

    type Endianness = 
        | Little 
        | Big

    [<Literal>]
    let BitsInByte = 8   

    let private pxToBytes px = px / BitsInByte + (if px % BitsInByte <> 0 then 1 else 0)
    let private getBufferLength widthBytes heightBytes size = function
        | Page
        | ColumnMajor -> heightBytes * size.Width
        | RowMajor -> widthBytes * size.Height

    type Graphics private(mode, endianness, size, buffer, widthBytes, heightBytes) =
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
        member __.Size with get () = size
        member __.AddressingMode with get() = mode
        member __.Endianness with get () = endianness

        override __.ToString() =
            let lines = 
                List.init (size.Height / 2) id 
                |> List.map (fun j -> 
                    let chars = 
                        List.init size.Width id 
                        |> List.map (fun i -> 
                            let t = __.GetPixel i (j*2)
                            let b = __.GetPixel i ((j*2) + 1)
                            match (t, b) with 
                            | 1uy, 1uy -> '█'
                            | 0uy, 1uy -> '▄'
                            | 1uy, 0uy -> '▀'
                            | _ -> ' ')
                    System.String(('│' :: chars @ ['│']) |> List.toArray))
            let header = new System.String(('┌' :: List.replicate size.Width '─' @ ['┐']) |> List.toArray)
            let footer = new System.String(('└' :: List.replicate size.Width '─' @ ['┘']) |> List.toArray)
            System.String.Join(System.Environment.NewLine, header :: lines @ [footer])

    let copyTo targetRect (targetGraphics:Graphics) sourceRect (sourceGraphics:Graphics) = 
        let copySourceRect = Rect.getIntersection (Rect.fromSize sourceGraphics.Size) sourceRect
        let copyTargetRect = Rect.getIntersection (Rect.fromSize targetGraphics.Size) targetRect
        let copySize = Size.combine (fun a b -> System.Math.Min(a,b)) copySourceRect.Size copyTargetRect.Size

        for i = 0 to copySize.Width - 1 do   
            for j = 0 to copySize.Height - 1 do 
                if (sourceGraphics.GetPixel (i + copySourceRect.Point.X) (j + copySourceRect.Point.Y) = 1uy) then
                    targetGraphics.SetPixel (i + copyTargetRect.Point.X) (j + copyTargetRect.Point.Y)

    let clip rect (graphics:Graphics) = 
        let copyRect = Rect.getIntersection (Rect.fromSize graphics.Size) rect
        let newGraphics = Graphics(graphics.AddressingMode, graphics.Endianness, copyRect.Size)
        copyTo (Rect.fromSize copyRect.Size) newGraphics rect graphics
        newGraphics