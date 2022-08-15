using System.ComponentModel.DataAnnotations.Schema;

namespace Portal.Models
{
    [Table("POS_SettingsKeyboard")]
    public class Keyboard
    {
        public int ID { get; set; }

        public string MarketID { get; set; }

        public int Pos_num { get; set; }

        public string? Key_1 { get; set; }
        public string? Key_2 { get; set; }
        public string? Key_3 { get; set; }
        public string? Key_4 { get; set; }
        public string? Key_5 { get; set; }
        public string? Key_6 { get; set; }
        public string? Key_7 { get; set; }
        public string? Key_8 { get; set; }
        public string? Key_9 { get; set; }
        public string? Key_10 { get; set; }
                     
        public string? Key_11 { get; set; }
        public string? Key_12 { get; set; }
        public string? Key_13 { get; set; }
        public string? Key_14 { get; set; }
        public string? Key_15 { get; set; }
        public string? Key_16 { get; set; }
        public string? Key_17 { get; set; }
        //public string Key_18 { get; set; }
        //public string Key_19 { get; set; }
        //public string Key_20 { get; set; }

        //public string Key_21 { get; set; }
        //public string Key_22 { get; set; }
        //public string Key_23 { get; set; }
        //public string Key_24 { get; set; }
        //public string Key_25 { get; set; }
        //public string Key_26 { get; set; }
        //public string Key_27 { get; set; }
        //public string Key_28 { get; set; }
        //public string Key_29 { get; set; }
        public string? Key_30 { get; set; }
                     
        public string? Key_31 { get; set; }
        public string? Key_32 { get; set; }
        public string? Key_33 { get; set; }
        public string? Key_34 { get; set; }
        public string? Key_35 { get; set; }
        public string? Key_36 { get; set; }
        public string? Key_37 { get; set; }
        public string? Key_38 { get; set; }
        public string? Key_39 { get; set; }
        public string? Key_40 { get; set; }
                     
        public string? Key_41 { get; set; }
        public string? Key_42 { get; set; }
        public string? Key_43 { get; set; }
        public string? Key_44 { get; set; }
        public string? Key_45 { get; set; }
        public string? Key_46 { get; set; }
        public string? Key_47 { get; set; }
        public string? Key_48 { get; set; }
        public string? Key_49 { get; set; }
        public string? Key_50 { get; set; }
                     
        public string? Key_51 { get; set; }
        public string? Key_52 { get; set; }
        public string? Key_53 { get; set; }
        public string? Key_54 { get; set; }
        public string? Key_55 { get; set; }
        public string? Key_56 { get; set; }
        public string? Key_57 { get; set; }
        public string? Key_58 { get; set; }
        public string? Key_59 { get; set; }
        public string? Key_60 { get; set; }
                     
        public string? Key_61 { get; set; }
        public string? Key_62 { get; set; }
        public string? Key_63 { get; set; }
        public string? Key_64 { get; set; }
        public string? Key_65 { get; set; }
        public string? Key_66 { get; set; }
        public string? Key_67 { get; set; }

        public Int64 IsSavedToPOS { get; set; }
        public bool IsSaved { get; set; }
    }
}