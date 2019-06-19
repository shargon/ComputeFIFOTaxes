# ComputeFIFOTaxes

Compute your crypto taxes and pay your taxes!

*Thanks to coinpaprika.com for his amazing work!*

Allowed exchanges:

- Kraken: Ledger and Trades
- Binance: Trades
- Bittrex: fullOrders

You must create a Google spreadsheet with all the information, each file on different sheets.

Require a config.json like this

```json
{
    "spreadsheet":
    {
        "id": "GOOGLE_SHEETS_ID"
    },
    "fiatProvider":
    {
        "fiatCoin": "EUR",
        "UsdPerCoin" : 0.88,
        "apiKey": ""
    }
}
```
