using CsvHelper.Configuration.Attributes;
using Microsoft.CodeAnalysis;

namespace Portal.DTO
{
    public class AccountantReport
    {
        [Name("№")]                                                  
        public int? Number { get; set; }                           // №
        [Name("Дата")]
        public string Date { get; set; }                             // Дата 
        [Name("Сумма(нал)")]
        public decimal AmountCash { get; set; }                      // Сумма(нал)
        [Name("Сумма(безнал)")]
        public decimal AmountNonCash { get; set; }                   // Сумма(безнал)
        [Name("Итого с НДС")] 
        public decimal TotalWithVAT { get; set; }                    // Итого с НДС
        [Name("НДС")]
        public decimal VAT { get; set; }                             // НДС
        [Name("Сумма скидки")]
        public decimal DiscountAmount { get; set; }                  // Сумма скидки
        [Name("Скидка по картам лояльности")]
        public decimal LoyaltyCardDiscount { get; set; }             // Скидка по картам лояльности
        [Name("Чеки продаж")]
        public int SalesChequeCount { get; set; }                    // Чеки продаж
        [Name("Сумма возврата с НДС")]
        public decimal VATRefundAmount { get; set; }                 // Сумма возврата с НДС
        [Name("НДС возврата")]
        public decimal VATRefund { get; set; }                       // НДС возврата
        [Name("НДС итоговый")]
        public decimal VATFinal { get; set; }                        // НДС итоговый
        [Name("Сумма скидки")]
        public decimal ReturnDiscountAmount { get; set; }            // Сумма скидки возврата
        [Name("Скидка по картам лояльности")]
        public decimal ReturnLoyaltyCardDiscount { get; set; }       // Скидка по картам лояльности возврата
        [Name("Чеки возврата")]
        public int ReturnChequeCount { get; set; }                   // Чеки возврата
    }
}
