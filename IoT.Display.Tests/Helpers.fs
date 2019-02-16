namespace IoT.Display.Tests

open System
open Swensen.Unquote

[<AutoOpen>]
module Assert = 
    let assertRender (actual:String) (expected:String) =
        let fail () = 
            let message = 
                "Expected: " 
                + Environment.NewLine 
                + expected
                + Environment.NewLine 
                + "Actual: " 
                + Environment.NewLine 
                + actual 
            raise (AssertionFailedException(message))

        let splitFilter (s:String) = 
            s.Split([|'\n'|])
            |> Array.filter (fun l -> l.StartsWith("│"))
            |> Array.map (fun l -> l.Substring(0, l.IndexOf('│', 1) + 1))
        
        let actualLines = actual |> splitFilter
        let expectedLines = expected |> splitFilter

        if (actualLines.Length <> expectedLines.Length) then
            fail()

        let result = 
            Array.zip actualLines expectedLines
            |> Array.forall (fun (a, b) -> a = b)

        if (result |> not) then
            fail()