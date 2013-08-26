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

type Either<'a, 'b> =
    | Success of 'a
    | Fail of 'b

type either<'a, 'b> = Either<'a, 'b>

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
    printfn "%A" successResult
    printfn "%A" failResult

let playEither () = 
    let devide x = function
        | y when y = 0 -> Fail(ArgumentException "Divide by zoro!")
        | y -> Success(x/y)

    let success = devide 10 2
    let fail = devide 20 0
    printfn "%A" success
    printfn "%A" fail

        

[<EntryPoint>]
let main argv = 
    playMaybe()        
    playEither()
    Console.ReadLine() |> ignore  
    0// return an integer exit code
