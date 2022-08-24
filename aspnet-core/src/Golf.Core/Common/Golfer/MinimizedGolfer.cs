using System;

namespace Golf.Core.Common.Golfer
{
    public class MinimizedGolfer
    {
        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public double Handicap { get; set; }
        public double IDX { get; set; }
       
    }
    public class MiniGolfer
    {
        public MinimizedGolfer Golfer = new MinimizedGolfer();
    }
}