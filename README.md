# ComputeFIFOTaxes

Compute your crypto taxes

Thanks to coinpaprika.com for his amazing work!

Allowed exchanges:

- Kraken: Ledger and Trades
- Binance: Trades

You must create a Google spreadsheet with all the information, each file on different sheets.

Requite a config.json like this

```json
{
    "spreadsheet":
    {
        "id": "GOOGLE_SHEETS_ID"
    },
    "fiatProvider":
    {
        "fiatCoin": "EUR",
        "apiKey": ""
    }
}
```
