open System
open FSharp.Data;

let loginUrl          = "http://192.168.100.1/goform/login"
let connectionPageUrl = "http://192.168.100.1/MotoConnection.asp"
let loginUsername     = "admin"
let loginPassword     = "motorola"

type CellType = ValueCell | TitleCell | HeaderCell

type DownstreamChannelStatus =
  {
    Channel: string;
    ChannelId: string;
    LockStatus: string;
    PowerdBmV: string;
    SNRdB: string;
  }

type UpstreamChannelStatus =
  {
    Channel: string;
    ChannelId: string;
    FreqMhz: string;
    PowerdBmV: string;
  }

type ConnectionStatus =
  {
    Online: bool;
    Uptime: string;
    Downstream: DownstreamChannelStatus[];
    Upstream: UpstreamChannelStatus[];
  }

/// From StackOverflow - Seq.split implementation
let splitBy f input =
    let i = ref 0
    input
    |> Seq.groupBy (fun x ->
        if f x then incr i
        !i)
    |> Seq.map snd

/// Determines cell type of given node
let getCellType (cell: HtmlNode) =
    match cell with
    | x when x.HasClass("moto-content-value") -> Some ValueCell
    | x when x.HasClass("moto-param-title") -> Some TitleCell
    // | x when x.HasClass("moto-param-header-s") -> Some HeaderCell // Ignoring these for now
    | _ -> None

/// Fetch tuple types of status tables
let scrapeStatus (connectionPage: HtmlDocument) =
    connectionPage.Descendants ["td"]
    |> Seq.map (fun cell -> (getCellType cell, cell))
    |> Seq.filter (fun (cellType, _) -> cellType.IsSome)
    |> Seq.map (fun (cellType, cell) -> (cellType.Value, cell.InnerText().Trim()))
    |> splitBy (fun x -> ((=) TitleCell (fst x)))

let structureStatus (statusElements: seq<seq<CellType * string>>) =
    let status: ConnectionStatus
    statusElements
    |> Seq.fold status (fun )

[<EntryPoint>]
let main argv =
    try
        printfn "Logging in"
        Http.RequestString(loginUrl, body = FormValues ["loginUsername", loginUsername;
                                                        "loginPassword", loginPassword]) |> ignore
        let connectionPage = HtmlDocument.Load(connectionPageUrl)
        let statusElements = scrapeStatus connectionPage
        printfn "%A" (Seq.toList statusElements)
    with
        | :? System.Net.WebException -> printfn "Modem ded"
    0
