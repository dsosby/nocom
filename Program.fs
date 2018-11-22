open System
open FSharp.Data;

let loginUrl          = "http://192.168.100.1/goform/login"
let connectionPageUrl = "http://192.168.100.1/MotoConnection.asp"
let loginUsername     = "admin"
let loginPassword     = "motorola"

type CellType = ValueCell | TitleCell | HeaderCell

/// Determines cell type of given node
let getCellType (cell: HtmlNode) =
    match cell with
    | x when x.HasClass("moto-content-value") -> Some ValueCell
    | x when x.HasClass("moto-param-title") -> Some TitleCell
    | x when x.HasClass("moto-param-header") -> Some HeaderCell
    | _ -> None


[<EntryPoint>]
let main argv =
    printfn "Logging in"
    try
        Http.RequestString(loginUrl, body = FormValues ["loginUsername", loginUsername;
                                                        "loginPassword", loginPassword]) |> ignore
        let connectionPage = HtmlDocument.Load(connectionPageUrl)
        let dataElems =
            connectionPage.Descendants ["td"]
            |> Seq.map (fun cell -> (getCellType cell, cell))
            |> Seq.filter (fun (cellType, _) -> cellType.IsSome)
            |> Seq.map (fun (cellType, cell) -> (cellType.Value, cell.InnerText()))
            |> Seq.toArray
        printfn "%A" dataElems
    with
        | :? System.Net.WebException -> printfn "Modem ded"
    0
