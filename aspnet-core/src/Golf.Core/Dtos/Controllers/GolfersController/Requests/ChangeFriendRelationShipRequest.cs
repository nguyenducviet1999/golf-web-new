using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Golf.Core.Dtos.Controllers.GolfersController.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChangeRelationshipRequest
    {
        SendRequest = 10,
        CancelRequest = 20,
        AcceptRequest = 21,
        RejectRequest = 22,
        UnfriendRequest = 30
    }
}