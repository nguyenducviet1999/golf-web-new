using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooCartResponseDto
    {
        [JsonProperty("data")]
        public OdooOrderResponseDto Order { get; set; }
    }
    public class OdooCartResponse
    {
        public OdooOrderResponse Order { get; set; }
    }
    public class OdooOrderResponseDto
    {
        [JsonProperty("order_id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("date_order")]
        public string DateOrder { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("invoice_status")]
        public string InvoiceStatus { get; set; }
        [JsonProperty("amount_total")]
        public double AmountTotal { get; set; }
        [JsonProperty("order_lines")]
        public List<OdooCartProductResponseDto> OrderLines { get; set; }
    }
    public class OdooOrderResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DateOrder { get; set; }
        public string State { get; set; }
        public string InvoiceStatus { get; set; }
        public string AmountTotal { get; set; }
        public List<OdooCartProductResponse> OrderLines { get; set; }
    }

    public class OdooCartProductResponseDto
    {
        [JsonProperty("product_id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price_unit")]
        public double PriceUnit { get; set; }
        [JsonProperty("price_subtotal")]
        public double PriceSubtotal { get; set; }
        [JsonProperty("product_uom_qty")]
        public double Quantity { get; set; }
        [JsonProperty("discount")]
        public double Discount { get; set; }
        [JsonProperty("image_128")]
        public string Image { get; set; }
        [JsonProperty("variants")]
        public List<VariantDto> Variants { get; set; }
    }
    public class OdooCartProductResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double PriceUnit { get; set; }
        public double PriceSubtotal { get; set; }
        public double Quantity { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }
        public List<Variant> Variants { get; set; }
    }
    public class VariantDto
    {
        [JsonProperty("attribute_name")]
        public string AttributeName { get; set; }
        [JsonProperty("attribute_value")]
        public string AttributeValue { get; set; }
    }
    public class Variant
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }

}
