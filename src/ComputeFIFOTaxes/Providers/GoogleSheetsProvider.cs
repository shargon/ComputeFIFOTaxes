using ComputeFIFOTaxes.Interfaces;
using ComputeFIFOTaxes.Types;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ComputeFIFOTaxes.Providers
{
    public class GoogleSheetsProvider : ITradeProvider
    {
        const string ApplicationName = "Trade Taxes";

        private SheetsService _service;

        /// <summary>
        /// Sheet
        /// </summary>
        public string SpreadsheetId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cfg">Config</param>
        public GoogleSheetsProvider(Config cfg)
        {
            SpreadsheetId = cfg.SpreadSheet.Id;

            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync
                    (
                    GoogleClientSecrets.Load(stream).Secrets,
                    // If modifying these scopes, delete your previously saved credentials
                    // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
                    new string[] { SheetsService.Scope.SpreadsheetsReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)
                    )
                    .Result;

                Console.WriteLine("Credential file saved to: token.json");
            }

            // Create Google Sheets API service.
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <returns>Return data</returns>
        public IEnumerable<TradeData> GetData()
        {
            // Define request parameters.

            var request = _service.Spreadsheets.Get(SpreadsheetId);

            // Prints the old values

            var response = request.Execute();
            var sheetList = response.Sheets.Select(u => u.Properties.Title).ToArray();

            foreach (var sheet in sheetList)
            {
                var requestData = _service.Spreadsheets.Values.Get(SpreadsheetId, sheet);
                var responseData = requestData.Execute();
                var data = responseData.Values.Select(u => u.ToArray()).ToList();

                yield return new TradeData()
                {
                    Title = sheet,
                    Data = data
                };
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Json string</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}