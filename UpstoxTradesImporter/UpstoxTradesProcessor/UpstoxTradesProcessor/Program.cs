using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dapper;
using OfficeOpenXml;

namespace ExcelToSqlServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string excelFilePath = "C:\\Users\\Soft\\Downloads\\trade_2223_AC9180 (1) (1).xlsx";
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Trading;Integrated Security=True;";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

            try
            {
                var records = ReadExcel<TradeRecord>(excelFilePath);

                if (records.Count > 0)
                {
                    PushToSqlServer(records, connectionString);
                    Console.WriteLine("Data pushed to SQL Server successfully.");
                }
                else
                {
                    Console.WriteLine("No records found in Excel file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            Console.ReadLine();
        }

        static List<T> ReadExcel<T>(string filePath) where T : class, new()
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                List<T> data = new List<T>();

                for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    var obj = new T();

                    if (worksheet.Cells[rowNum, 1].Value == null)
                    {
                        break;
                    }

                    foreach (var property in typeof(T).GetProperties())
                    {
                        var cellValue = worksheet.Cells[rowNum, GetColumnIndex(worksheet, property.Name)].Text;
                        // If the property type is numeric, remove the currency symbol before parsing
                        if (IsNumericType(property.PropertyType))
                        {
                            cellValue = cellValue.Replace("₹", ""); // Replace the currency symbol with empty string
                        }
                        try
                        {
                            if(property.Name == "Expiry" && cellValue == "")
                            {
                                cellValue = SqlDateTime.MinValue.ToString();
                            }
                            property.SetValue(obj, Convert.ChangeType(cellValue, property.PropertyType));
                        } 
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }

                    data.Add(obj);
                }

                return data;
            }
        }

        static bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        static int GetColumnIndex(ExcelWorksheet worksheet, string columnName)
        {
            for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
            {
                string columnHeader = worksheet.Cells[1, i].Text.Trim().Replace(" ", ""); // Remove spaces from the column header
                if (columnHeader.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            throw new ArgumentException($"Column '{columnName}' not found in the Excel file.");
        }

        static void PushToSqlServer<T>(List<T> data, string connectionString)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string tableName = typeof(T).Name;
                        string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
                        string values = "@" + string.Join(", @", typeof(T).GetProperties().Select(p => p.Name));

                        string insertQuery = $"INSERT INTO dbo.{tableName}s ({columns}) VALUES ({values})";

                        db.Execute(insertQuery, data, transaction: transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }

    public class TradeRecord
    {
        public DateTime Date { get; set; }
        public string Company { get; set; }
        public decimal Amount { get; set; }
        public string Exchange { get; set; }
        public string Segment { get; set; }
        public string ScripCode { get; set; }
        public string InstrumentType { get; set; }
        public decimal StrikePrice { get; set; }
        public DateTime Expiry { get; set; }
        public int TradeNum { get; set; }
        public string TradeTime { get; set; }
        public string Side { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
