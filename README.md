# ComputeFIFOTaxes

<p align="center">
  <a href="https://coveralls.io/github/shargon/ComputeFIFOTaxes?branch=master">
    <img src="https://coveralls.io/repos/github/shargon/ComputeFIFOTaxes/badge.svg?branch=master" alt="Current Coverage Status." />
  </a>
  <a href="https://travis-ci.org/shargon/ComputeFIFOTaxes">
    <img src="https://travis-ci.org/shargon/ComputeFIFOTaxes.svg?branch=master" alt="Current TravisCI build status.">
  </a>
  <a href="https://github.com/shargon/ComputeFIFOTaxes/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License.">
  </a>
  <a href="https://github.com/shargon/ComputeFIFOTaxes/releases">
    <img src="https://badge.fury.io/gh/shargon%2FComputeFIFOTaxes.svg" alt="Current version.">
  </a>
</p>

Compute your crypto taxes and pay your taxes!

*Thanks to coinpaprika.com for his amazing work!*

Allowed exchanges:

- Kraken: Ledger and Trades
- Binance: Trades
- Bittrex: fullOrders

You must create a Google spreadsheet with all the information, each file on different sheets.

For `credentials.json` look at https://developers.google.com/sheets/api/quickstart/dotnet

Require a `config.json` like this

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
