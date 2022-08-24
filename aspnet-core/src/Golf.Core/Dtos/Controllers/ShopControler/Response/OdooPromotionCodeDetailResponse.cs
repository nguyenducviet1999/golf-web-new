using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooPromotionCodeDetailResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("promo_code")]
        public string PromoCode { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("rule_date_from")]
        public string DateFrom { get; set; }
        [JsonProperty("rule_date_to")]
        public string DateTo { get; set; }
        [JsonProperty("rule_min_quantity")]
        public int MinQuantity { get; set; }
        [JsonProperty("rule_minimum_amount")]
        public double MinimumAmount { get; set; }
        [JsonProperty("rule_minimum_amount_tax_inclusion")]
        public string MinimumAmountTaxInclusion { get; set; }
        [JsonProperty("discount_percentage")]
        public double DiscountPercentage { get; set; }
        [JsonProperty("discount_fixed_amount")]
        public double DiscountFixedAmount { get; set; }  
        [JsonProperty("discount_line_product_id")]
        public List<string> DiscountLineProduct { get; set; }
    }
    public class OdooPromotionCodeDetailResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PromoCode { get; set; }
        public string DisplayName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int MinQuantity { get; set; }
        public int MinimumAmount { get; set; }
        public string MinimumAmountTaxInclusion { get; set; }
        public double DiscountPercentage { get; set; }
        public double DiscountFixedAmount { get; set; }  
        public List<string> DiscountLineProduct { get; set; }
    }
    public class OdooPromotionCodeResponseDto
    {
        [JsonProperty("reward_id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("promo_code")]
        public string PromoCode { get; set; }
        public string DisplayName => Name;
        [JsonProperty("date_from")]
        public string DateFrom { get; set; }
        [JsonProperty("date_to")]
        public string DateTo { get; set; }
        [JsonProperty("rule_min_quantity")]
        public int MinQuantity { get; set; }
        [JsonProperty("rule_minimum_amount")]
        public double MinimumAmount { get; set; }
        [JsonProperty("discount_percentage")]
        public double DiscountPercentage { get; set; }
        [JsonProperty("discount_fixed_amount")]
        public double DiscountFixedAmount { get; set; }
      
    }
  
}
