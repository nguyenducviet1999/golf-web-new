using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Golf.Core.Dtos.Controllers.GolfersController.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]

    public enum AchievementFilter
    {
        All,
        HoleInOne,
        Condor,
        Albatross,
        Eagle,
        Birdie,
        Par
    }
}