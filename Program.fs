open System
open FSharp.Data;

let loginUrl          = "http://192.168.100.1/goform/login"
let connectionPageUrl = "http://192.168.100.1/MotoConnection.asp"
let loginUsername     = "admin"
let loginPassword     = "motorola"

[<EntryPoint>]
let main argv =
    printfn "Logging in"
    try
        Http.RequestString(loginUrl, body = FormValues ["loginUsername", loginUsername;
                                                        "loginPassword", loginPassword]) |> ignore
        let connectionPage = HtmlDocument.Load(connectionPageUrl)
        let dataElems =
            connectionPage.Descendants ["td"]
            |> Seq.filter(fun cell -> cell.HasClass("moto-content-value") || cell.HasClass("moto-param-title"))
            |> Seq.toArray
        printfn "%A" dataElems
    with
        | :? System.Net.WebException -> printfn "Modem ded"
    0
