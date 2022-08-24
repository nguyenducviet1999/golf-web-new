namespace Golf.Domain.Common
{
    public class Quarter
    {
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }

        public Quarter(int startMonth, int endMonth)
        {
            StartMonth = startMonth;
            EndMonth = endMonth;
        }
        public Quarter(int month)
        {
            if (month <= 3)
            {
                StartMonth = 1;
                EndMonth = 3;
            }
            else if (month <= 6)
            {
                StartMonth = 4;
                EndMonth = 6;
            }
            else if (month <= 9)
            {
                StartMonth = 7;
                EndMonth = 9;
            }
            else
            {
                StartMonth = 10;
                EndMonth = 12;
            }

        }
    }
}