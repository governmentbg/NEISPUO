namespace MON.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IExcelService
    {
        public byte[] DumpExcel<T>(IEnumerable<T> tbl, string title = "Report", int StartRow = 1);
    }
}
