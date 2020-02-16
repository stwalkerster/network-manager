namespace DnsWebApp.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class ReportResult
    {
        public string Title { get; }
        public List<string> Columns { get; }
        public List<string[]> Data { get; }
        public List<string[]> DataValues { get; }

        public ReportResult(string title, List<string> columns, List<string[]> data, List<string[]> dataValues = null)
        {
            this.Title = title;
            this.Columns = columns;
            this.Data = data;
            this.DataValues = dataValues ?? Enumerable.Repeat(new string[columns.Count], data.Count).ToList();
        }
    }
}