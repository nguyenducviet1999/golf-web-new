using System;

namespace Golf.Core.Dtos.Request
{
    public abstract class IRequest
    {
        public abstract bool Validate();
        public static bool IsNumber(string strNum)
        {
            if (strNum == null||strNum.Length<10||strNum.Length>12)
            {
                return false;
            }
            try
            {
                UInt64 d = UInt64.Parse(strNum);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    public abstract class IMutationRequest : IRequest
    {
        public Guid GolferID { get; set; }
    }

    public abstract class IQueryRequest : IRequest
    {
        public Guid GolferID  { get; set; }
    }
}