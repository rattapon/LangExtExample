// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System

type Maybe<'a> = 
        | Just of 'a
        | Nothing

type MaybeBuilder() = 
     member this.Bind (mx, f) =
        match mx with
        | Just mx when mx >= 0 && mx <= 100 -> f mx
        | _ -> Nothing
     member this.Delay (f) = f()
     member this.Return (x: 'a) : Maybe<'a> = Just x

let playMaybe () =
    let maybe = new MaybeBuilder()
    let add a b = 
        maybe {
           let  x = 0
           let! y = Just a
           let! z = Just b
           return x + y + z
        }

    let successResult = add 20 30
    let failResult = add 100 -10
    Console.WriteLine("{0}", successResult)
    Console.WriteLine("{0}", failResult)
        

[<EntryPoint>]
let main argv = 
    playMaybe()        
    Console.ReadLine() |> ignore  
    0// return an integer exit code
