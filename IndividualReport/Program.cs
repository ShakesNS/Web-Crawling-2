using IndividualReport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ExcelDataReader;

namespace IndividualReport
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load("https://www.mfat.govt.nz/en/countries-and-regions/europe/ukraine/russian-invasion-of-ukraine/sanctions/russia-sanctions-register/");
                var filePath = @"C:\\Users\\SS\\source\\repos\\TaskExample\\TaskExample\\Files\\exmp.xlsx";
                var filePath2 = @"C:\\Users\\SS\\source\\repos\\TaskExample\\TaskExample\\Files\\exmp.json";

                var xmlFile = doc?.DocumentNode?.SelectSingleNode("/html/body/div/div[1]/section/div/main/article/div[2]/ul/li/a").GetAttributeValue("href", "");
                if (!xmlFile.StartsWith("http"))
                {
                    xmlFile = "https://www.mfat.govt.nz/" + xmlFile;
                }
                HttpClient client = new HttpClient();
                var res = client.GetAsync(xmlFile);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                if (res.Result.IsSuccessStatusCode)
                {
                    using (var stream = await res.Result.Content.ReadAsStreamAsync())
                    {

                        using (FileStream fileStream = File.Create(filePath))
                        {

                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                List<Individual> individuals = new List<Individual>();

                bool readActive = false;


                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelReader;
                    if (Path.GetExtension(filePath).ToUpper() == ".XLS")
                    {
                        //Reading from a binary Excel file ('97-2003 format; *.xls)

                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                    }
                    else
                    {
                        //Reading from a OpenXml Excel file (2007 format; *.xlsx)

                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    while (excelReader.Read())
                    {
                        if (readActive == true)
                        {
                            if (excelReader.GetValue(0).ToString().Equals("Individual") || excelReader.GetValue(0).ToString().Equals("individual"))
                            {
                                var indi = new Individual()
                                {
                                    FirstName = excelReader.GetValue(2)?.ToString(),
                                    MiddleName = excelReader.GetValue(3)?.ToString(),
                                    LastName = excelReader.GetValue(3)?.ToString(),
                                    UniqueIdentifier = excelReader.GetValue(1)?.ToString(),
                                    DateOfBirth = excelReader.GetValue(7)?.ToString(),
                                    PlaceOfBirth = excelReader.GetValue(11)?.ToString(),
                                    SanctionStatus = excelReader.GetValue(16)?.ToString()
                                };
                                individuals.Add(indi);
                            }
                        }
                        if (excelReader.GetValue(0) != null && excelReader.GetValue(0).ToString().Trim() == "Type")
                        {
                            readActive = true;
                            continue;
                        }

                    }
                    excelReader.Close();
                    string json = JsonSerializer.Serialize(individuals);
                    File.WriteAllText(filePath2, json);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
