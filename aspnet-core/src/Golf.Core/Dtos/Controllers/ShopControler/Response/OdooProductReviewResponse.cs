using Golf.Core.Common.Odoo.OdooResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooProductReviewResponsesDto
    {
        [JsonProperty("messages")]
        public List<OdooProductReviewResponseDto> Messages { get; set; } 
        [JsonProperty("options")]
        public OptionsResponseDto Options { get; set; }
        [JsonProperty("rating_stats")]
        public RatingStatsResponseDto RatingStats { get; set; }
    }
    public class OdooProductReviewResponses
    {
        public List<OdooProductReviewResponse> Messages { get; set; }
        public OptionsResponse Options { get; set; }
        public RatingStatsResponse RatingStats { get; set; }
    }
    public class OdooProductReviewResponseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("date")]
        public dynamic Date { get; set; }
        [JsonProperty("author_id")]
        public List<string> Author { get; set; }
        [JsonProperty("email_from")]
        public string EmailFrom { get; set; }
        [JsonProperty("message_type")]
        public string MessageType { get; set; }
        [JsonProperty("subtype_id")]
        public List<string> Subtype { get; set; }
        [JsonProperty("is_internal")]
        public bool IsInternal { get; set; }
        [JsonProperty("subject")]
        public dynamic Subject { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("res_id")]
        public int ResID { get; set; }
        [JsonProperty("record_name")]
        public string RecordName { get; set; } 
        [JsonProperty("rating_value")]
        public double RatingValue { get; set; } 
        [JsonProperty("notifications")]
        public List<string> Notifications { get; set; } 
        [JsonProperty("attachment_ids")]
        public List<AttachmentResponseDto> Attachments { get; set; }
        [JsonProperty("tracking_value_ids")]
        public List<string> TrackingValues { get; set; }
        [JsonProperty("rating")]
        public RatingResponseDto Rating { get; set; } 
    }
    public class OdooProductReviewResponse
    { 
        public int Id { get; set; }
        public string Body { get; set; }
        public dynamic Date { get; set; }
        public OdooObject Author { get; set; }
        public string EmailFrom { get; set; }
        public string MessageType { get; set; }
        public OdooObject Subtype { get; set; }
        public bool IsInternal { get; set; }
        public dynamic Subject { get; set; }
        public string Model { get; set; }
        public int ResID { get; set; }
        public string RecordName { get; set; }
        public double RatingValue { get; set; }
        public List<string> Notifications { get; set; }
        public List<AttachmentResponse> Attachments { get; set; }
        public List<string> TrackingValues { get; set; }
        public RatingResponse Rating { get; set; }
    } 
    public class AttachmentResponseDto
    {
        [JsonProperty("checksum")]
        public string CheckSum { get; set; } 
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("filename")]
        public string FileName { get; set; } 
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("mimetype")]
        public string MimeType { get; set; }
        [JsonProperty("is_main")]
        public bool IsMain { get; set; }
        [JsonProperty("res_id")]
        public int ResID { get; set; }
        [JsonProperty("res_model")]
        public string ResModel { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
    public class AttachmentResponse
    {
        public string CheckSum { get; set; } 
        public int ID { get; set; }
        public string FileName { get; set; } 
        public string Name { get; set; }
        public string MimeType { get; set; }
        public bool IsMain { get; set; }
        public int ResID { get; set; }
        public string ResModel { get; set; }
        public string AccessToken { get; set; }
    } 
    public class RatingResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("publisher_comment")]
        public dynamic PublisherComment { get; set; }
        [JsonProperty("publisher_id")]
        public dynamic PublisherID { get; set; }
        [JsonProperty("publisher_datetime")]
        public dynamic PublisherDatetime { get; set; }
        [JsonProperty("message_id")]
        public List<string> Message { get; set; }
    }
    public class RatingResponse
    {
        public int ID { get; set; }
        public dynamic PublisherComment { get; set; }
        public dynamic PublisherID { get; set; }
        public dynamic PublisherDatetime { get; set; }
        public OdooObject Message { get; set; }
    }
    public class OptionsResponseDto
    {
        [JsonProperty("message_count")]
        public int MessageCount { get; set; }
        [JsonProperty("is_user_public")]
        public bool IsUserPublic { get; set; }
        [JsonProperty("is_user_employee")]
        public bool IsUserEmployee { get; set; }
        [JsonProperty("is_user_publisher")]
        public bool IsUserPublisher { get; set; }
        [JsonProperty("display_composer")]
        public bool DisplayComposer { get; set; }
        [JsonProperty("partner_id")]
        public int PartnerID { get; set; }
    }
    public class OptionsResponse
    {
        public int MessageCount { get; set; }
        public bool IsUserPublic { get; set; }
        public bool IsUserEmployee { get; set; }
        public bool IsUserPublisher { get; set; }
        public bool DisplayComposer { get; set; }
        public int PartnerID { get; set; }
    }
    public class RatingStatsResponseDto
    {     
        [JsonProperty("avg")]
        public double avg { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }
        [JsonProperty("percent")]
        public PercentResponseDto Percent { get; set; }

    }
    public class RatingStatsResponse
    {     
        public double avg { get; set; }
        public int total { get; set; }
        public PercentResponse Percent { get; set; }

    }
    public class PercentResponseDto
    {
        [JsonProperty("1")]
        public double One { get; set; }
        [JsonProperty("2")]
        public double Two { get; set; }
        [JsonProperty("3")]
        public double Three { get; set; }
        [JsonProperty("4")]
        public double Four { get; set; }
        [JsonProperty("5")]
        public double Five { get; set; }

    }
    public class PercentResponse
    {
        [JsonProperty("1")]
        public int One { get; set; }
        [JsonProperty("2")]
        public int Two { get; set; }
        [JsonProperty("3")]
        public int Three { get; set; }
        [JsonProperty("4")]
        public int Four { get; set; }
        [JsonProperty("5")]
        public int Five { get; set; }

    }
}
