using Portal.ScaleModels;
using System.Collections.Generic;
using UZ.STS.POS2K.DataAccess.Models;

namespace Portal.DTO
{
    public class ScaleView
    {
        public List<Scale> Scales { get; set; }
        public bool IsAdmin { get; set; }
        public Roles UserRole { get; set; }
        public string Market { get; set; }
        public List<MarketsName> Markets { get; set; }
    }
}