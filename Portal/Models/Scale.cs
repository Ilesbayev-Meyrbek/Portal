using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Portal.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table("Scales")]
    public class Scale
    {
        public Int64 ID { get; set; }
        public string MarketID { get; set; }
        public string ScaleType { get; set; }
        public string ScaleName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool Tiger { get; set; }
        public bool BPlus { get; set; }
        public bool FreshBase { get; set; }

        public int DefaultCategoryIndex { get; set; }
        public DateTime DataUpdateTime { get; set; }
        public DateTime? LastSyncTime { get; set; }
    }
}