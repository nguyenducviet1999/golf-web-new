using System;

namespace Golf.Services.Helpers
{
    public class DistanceConvert
    {
        public static int ConvertYardToMeter(int distanceYard)
        {
            return (int)(distanceYard / 1.0936);
        }
    }
}