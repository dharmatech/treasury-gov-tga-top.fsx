open System.Net.Http
open System.Net.Http.Json

let baseUri = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/dts"

let date = "2023-05-18"

let client = new HttpClient()

type TGARecordData = {
    record_date           : string
    transaction_type      : string
    transaction_catg      : string
    transaction_catg_desc : string
    transaction_today_amt : int 
}

type TGARecordLinks = {
    self  : string
    first : string
    prev  : string
    next  : string
    last  : string
}

type TGARecord = {
        data  : TGARecordData[]
        links : TGARecordLinks 
}

let str = (sprintf "%s/dts_table_2?filter=record_date:eq:%s&page[number]=1&page[size]=300" baseUri date)

let result = client.GetFromJsonAsync<TGARecord>(str).Result

let display_record item =
    printfn "%s %-15s %-50s %-20s %20d" 
        item.record_date 
        item.transaction_type 
        item.transaction_catg 
        item.transaction_catg_desc 
        item.transaction_today_amt

let display data transaction_type =

    let filtered = 
        data |> 
        Array.filter           (fun item -> item.transaction_type = transaction_type) |>
        Array.sortByDescending (fun item -> item.transaction_today_amt) |>
        Array.take 15
    
    for item in filtered do
        display_record item
    
display result.data "Deposits"
printfn ""
display result.data "Withdrawals"
