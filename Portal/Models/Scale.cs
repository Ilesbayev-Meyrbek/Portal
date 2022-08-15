﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
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
    }
}