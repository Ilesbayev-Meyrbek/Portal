using System;

namespace Portal.DTO
{
    public class ReportDto
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public int PosNum { get; set; }
        public string TerminalID { get; set; }
        public string MarketID { get; set; }
    }
}