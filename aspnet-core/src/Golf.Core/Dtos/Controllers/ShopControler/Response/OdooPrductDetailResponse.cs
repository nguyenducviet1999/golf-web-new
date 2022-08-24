using Golf.Core.Common.Odoo.OdooResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
  public class OdooPrductDetailResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description_sale")]
        public string DescriptionSale { get; set; }
        [JsonProperty("image_128")]
        public string Image { get; set; }
        [JsonProperty("list_price")]
        public double Price { get; set; }
        [JsonProperty("currency_id")]
        public List<string> Currency { get; set; }
        [JsonProperty("virtual_available")]
        public double VirtualAvailable { get; set; } 
        [JsonProperty("product_variant_ids")]
        public List<ProductVariantResponseDto> ProductVariants { get; set; }
        [JsonProperty("product_template_image_ids")]
        public List<ProductTemplateImageResponseDto> ProductTemplateImages { get; set; }
        [JsonProperty("alternative_product_ids")]
        public List<AlternativeProductResponseDto> AlternativeProducts { get; set; }
    } 
  public class OdooPrductDetailResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DescriptionSale { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public OdooObject Currency { get; set; }
        public double VirtualAvailable { get; set; }
        public List<ProductVariantResponse> ProductVariants { get; set; }
        public List<ProductTemplateImageResponse> ProductTemplateImages { get; set; }
        public List<AlternativeProductResponse> AlternativeProducts { get; set; }
    }
    public class ProductVariantResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("display_name")]
        public string Name { get; set; }
        [JsonProperty("image_128")]
        public string Image { get; set; }
        [JsonProperty("lst_price")]
        public double Price { get; set; }
        [JsonProperty("virtual_available")]
        public double VirtualAvailable { get; set; }
        [JsonProperty("available_threshold")]
        public double AvailableThreshold { get; set; } 
        [JsonProperty("product_template_attribute_value_ids")]
        public List<ProductTemplateAttributeValueDto> ProductTemplateAttributeValues { get; set; }
    }
    public class ProductVariantResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public double VirtualAvailable { get; set; }
        public double AvailableThreshold { get; set; }
        public bool IsFavourite { get; set; }
        public List<ProductTemplateAttributeValue> ProductTemplateAttributeValues { get; set; }
    }
    public class ProductTemplateImageResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class ProductTemplateImageResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class AlternativeProductResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description_sale")]
        public string DescriptionSale { get; set; }
        [JsonProperty("list_price")]
        public double Price { get; set; }
        [JsonProperty("currency_id")]
        public List<string> Currency { get; set; }
        [JsonProperty("virtual_available")]
        public double VirtualAvailable { get; set; }
    }
    public class AlternativeProductResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DescriptionSale { get; set; }
        public double Price { get; set; }
        public OdooObject Currency { get; set; }
        public double VirtualAvailable { get; set; }
    }
    public class ProductTemplateAttributeValueDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("display_type")]
        public string DisplayType { get; set; }
        [JsonProperty("html_color")]
        public string Color { get; set; }
        [JsonProperty("attribute_id")]
        public OdooProductAttributeDto Attribute { get; set; }

    } 
    public class ProductTemplateAttributeValue
    {
        public string Name { get; set; }
        public string DisplayType { get; set; }
        public string Color { get; set; }
        public OdooProductAttribute Attribute { get; set; }

    }
    public class OdooProductAttributeDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    } 
    public class OdooProductAttribute
    {
        public string Name { get; set; }
    }
}
