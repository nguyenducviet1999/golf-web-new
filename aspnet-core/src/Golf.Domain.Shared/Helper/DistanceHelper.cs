using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Helper
{
    public static class DistanceHelper
    {
        public static double GetLongitudeUnitLength(double longitude)
        {
            if(0<=longitude&& longitude <= 15)
            {
                return 111.320;
            }
            if(15<=longitude&& longitude <= 30)
            {
                return 107.551;
            }
            if (30 <= longitude && longitude <= 45)
            {
                return 96.486;
            }
            if (45 <= longitude && longitude <= 60)
            {
                return 78.847;
            }
            if (60 <= longitude && longitude <= 75)
            {
                return 55.8;
            }
            if (75 <= longitude && longitude <= 90)
            {
                return 28.902;
            }
            return 0;
        }
    }
}
