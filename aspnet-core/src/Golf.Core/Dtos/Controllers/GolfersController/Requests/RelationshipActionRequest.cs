using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Golf.Core.Dtos.Controllers.GolfersController.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RelationshipRequestStatus
    {
        Sent,
        Received,
        InRelationship
    }
}