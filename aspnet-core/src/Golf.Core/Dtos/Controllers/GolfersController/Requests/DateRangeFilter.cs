using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Golf.Core.Dtos.Controllers.GolfersController.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]

    public enum DateRangeFilter
    {
        All = 0,
        FiveRounds = 1,
        TenRounds = 2,
        TwentyRounds = 3,
        ThisWeek = 4,
        ThisMonth = 5,
        ThisQuarter = 6,
        HalfYear = 7,
        ThisYear = 8,
        Custom =9
    }

    public class AchievementsByDateFilterRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}