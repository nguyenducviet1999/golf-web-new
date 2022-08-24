using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Golfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GolfersController.Responses
{
    public class RankingResponse
    {
       public List<RankingGolfer> RankingGolfers = new List<RankingGolfer>();
        public RankingCurrentGolfer RankingCurrentGolfer = new RankingCurrentGolfer();

    }
    public class RankingGolfer
    {
        public MinimizedGolfer Golfer = new MinimizedGolfer();
        public string CourseName = "";
        public double BestNet = 0;
        public double BestGross = 0;
    }  
    public class RankingCurrentGolfer
    {
        public int Rank = 0;
        public RankingCurrentGolferType Type;
        public string CourseName = "";
        public double BestNet = 0;
        public double BestGross = 0;
    }

}
