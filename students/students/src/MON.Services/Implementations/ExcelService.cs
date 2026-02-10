namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Options;
    using MON.Models.Configuration;
    using MON.Services.Interfaces;
    using OfficeOpenXml.Style;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using System.IO;
    using MON.Shared.Attributes;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using DocumentFormat.OpenXml.Spreadsheet;
    using MON.Models;

    public class ExcelService : BaseService<ExcelService>, IExcelService
    {
        public ExcelService(DbServiceDependencies<ExcelService> dependencies)
            : base(dependencies)
        {
        }

        public static DataTable CollectionToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others  will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {

                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public byte[] DumpExcel(DataTable tbl)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(tbl, true);
                var columnIndex = 0;
                foreach (DataColumn column in tbl.Columns)
                {
                    columnIndex++;
                    if (column.DataType == typeof(DateTime))
                    {
                        ws.Column(columnIndex).Style.Numberformat.Format = "yyyy-mm-dd";
                    }
                }
                return pck.GetAsByteArray();
            }
        }


        public byte[] DumpExcel<T>(IEnumerable<T> tbl, string title = "Report", int StartRow = 1)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(title);

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                LoadFromCollectionWithHeaders(ws.Cells["A" + StartRow.ToString()], tbl, StartRow);

                return pck.GetAsByteArray();
            }
        }

        private PropertyInfo GetProp(Type baseType, string propertyName)
        {
            string[] parts = propertyName.Split('.');

            return (parts.Length > 1)
                ? GetProp(baseType.GetProperty(parts[0]).PropertyType, parts.Skip(1).Aggregate((a, i) => a + "." + i))
                : baseType.GetProperty(propertyName);
        }

        public ExcelRangeBase LoadFromCollectionWithHeaders<T>(ExcelRange excelRange, IEnumerable<T> list, int Row = 1)
        {
            MemberInfo[] membersToInclude = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => 
                    !(Attribute.IsDefined(p, typeof(ExportIgnoreAttribute)))
                    )
                .ToArray();

            excelRange.LoadFromCollection<T>(list, true, OfficeOpenXml.Table.TableStyles.None,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);

            int ColumnsCount = 0;

            if (list.Count() > 0)
            {
                ColumnsCount = excelRange.Worksheet.Cells.Count() / list.Count();
            }
            else
            {
                ColumnsCount = excelRange.Worksheet.Cells.Count();
            }
            for (int Column = 1; Column <= ColumnsCount; Column++)
            {
                string IncorrectHeader = (((OfficeOpenXml.ExcelRangeBase)(excelRange.Worksheet.Cells[Row, Column]))).Text;

                PropertyInfo[] Properties = typeof(T).GetProperties();
                var Property = Properties.Where(i => i.Name.Replace("_", " ") == IncorrectHeader).FirstOrDefault();
                if (Property != null)
                {
                    object[] DisplayAttributes = Property.GetCustomAttributes(typeof(DisplayAttribute), true);

                    if (DisplayAttributes.Length == 1)
                    {
                        var attribute = (DisplayAttribute)(DisplayAttributes[0]);
                        (((OfficeOpenXml.ExcelRangeBase)(excelRange.Worksheet.Cells[Row, Column]))).Value = attribute.GetName();
                    }
                    (excelRange.Worksheet.Cells[Row, Column]).Style.Font.Bold = true;

                    if (Property.PropertyType == typeof(DateTime) || Property.PropertyType == typeof(DateTime?))
                    {
                        excelRange.Worksheet.Column(Column).Style.Numberformat.Format = "dd.MM.yyyy";
                    }

                    if (Property.PropertyType == typeof(bool) || Property.PropertyType == typeof(bool?))
                    {
                        for(int rowIndex = 1; rowIndex <= list.Count(); rowIndex++)
                        {
                            var cell = excelRange.Worksheet.Cells[Row + rowIndex, Column];
                            bool? value = (bool?)cell.Value;
                            if (value.HasValue)
                            {
                                cell.Value = value.Value ? "Да" : "Не";
                            }
                        }
                    }
                }
            }
            excelRange.Worksheet.Cells.AutoFitColumns();

            return excelRange;
        }

        public T ReadFromExcel<T>(string path, bool hasHeader = true)
        {
            using (var excelPack = new ExcelPackage())
            {
                //Load excel stream
                using (var stream = File.OpenRead(path))
                {
                    excelPack.Load(stream);
                }

                //Lets Deal with first worksheet.(You may iterate here if dealing with multiple sheets)
                var ws = excelPack.Workbook.Worksheets[0];

                //Get all details as DataTable -because Datatable make life easy :)
                DataTable excelasTable = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    //Get colummn details
                    if (!string.IsNullOrEmpty(firstRowCell.Text))
                    {
                        string firstColumn = string.Format("Column {0}", firstRowCell.Start.Column);
                        excelasTable.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                    }
                }
                var startRow = hasHeader ? 2 : 1;
                //Get row details
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, excelasTable.Columns.Count];
                    DataRow row = excelasTable.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                //Get everything as generics and let end user decides on casting to required type
                var generatedType = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(excelasTable));
                return (T)Convert.ChangeType(generatedType, typeof(T));
            }
        }

    }
}
