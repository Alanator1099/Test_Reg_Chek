using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test_Reg_Chek
{
    internal class GoogleSheetsReader
    {
        private readonly string _sheetId;
        private readonly SheetsService _service;
        public List<String> tags = new();

        public GoogleSheetsReader(string credentialsPath, string sheetId) 
        {
            _sheetId = sheetId;

            GoogleCredential credential;

            using (var stream = new System.IO.FileStream(credentialsPath, System.IO.FileMode.Open))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        public async Task GetListOfTages(string sheetName, string row) 
        {
            var range = $"{sheetName}!{row}:{row}";
            var request = _service.Spreadsheets.Values.Get(_sheetId, range);

            ValueRange response;

            try
            {
                response = await request.ExecuteAsync();
            }
            catch (System.Exception ex)
            { 
                Console.WriteLine(ex.Message);
                return;
            }

            //Parsing
            if (response != null && response.Values != null)
            {
                foreach(var cell in response.Values)
                {
                    
                    if (cell.Count > 0) 
                    {
                        tags.Add(cell[0].ToString()); 
                    }
                }

            }
        }
    }
}
