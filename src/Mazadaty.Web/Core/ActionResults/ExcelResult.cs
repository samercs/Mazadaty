using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Mazadaty.Web.Core.ActionResults
{
    public class ExcelResult : ActionResult
    {
        private readonly DataTable _dataTable;
        private readonly string _fileName;

        public ExcelResult(DataTable dataTable, string fileName)
        {
            _dataTable = dataTable;
            _fileName = string.Format("zeedli-{0}-{1:yyyy-MM-dd-HH-mm-ss}.xlsx", fileName, DateTime.UtcNow.AddHours(3)).ToLowerInvariant();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.HttpContext.Response.AppendHeader("content-disposition", "attachment; filename=" + _fileName);

            using (var ms = new MemoryStream())
            {
                using (var excel = new ExcelPackage())
                {
                    var ws = excel.Workbook.Worksheets.Add("Data");
                    ws.Cells["A1"].LoadFromDataTable(_dataTable, true);
                    ws.Cells[1, 1, 1, _dataTable.Columns.Count].Style.Font.Bold = true;

                    FormatDateColumns(ws);
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    excel.SaveAs(ms);
                    ms.Position = 0;
                }

                ms.WriteTo(context.HttpContext.Response.OutputStream);
            }
        }

        private void FormatDateColumns(ExcelWorksheet ws)
        {
            if (_dataTable.Rows.Count == 0)
            {
                return;
            }
            
            var dateColumns = from DataColumn d in _dataTable.Columns
                where d.DataType == typeof (DateTime)
                select d.Ordinal + 1;

            foreach (var dc in dateColumns)
            {
                ws.Cells[2, dc, _dataTable.Rows.Count + 1, dc].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
            }
        }
    }
}
