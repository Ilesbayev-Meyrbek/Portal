using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Portal.DTO
{
    public class LogoViewModel
    {
        public string MarketID { get; set; }
        public IFormFile BMP { get; set; }
        public int DateBegin { get; set; }
        public int DateEnd { get; set; }
        public string Note { get; set; }
        public long IsSavedToPOS { get; set; }
        public bool IsSaved { get; set; }
        public DateTime DateS { get; set; }
        public DateTime DateE { get; set; }

        //public string Name { get; set; }
        //public IFormFile Avatar { get; set; }
    }
}
