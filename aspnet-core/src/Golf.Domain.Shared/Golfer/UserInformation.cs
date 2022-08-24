using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Golfer.Information
{
    public enum ClothesSizeType
    {
        US,
        UK,
        EU,
        International,
        VietNam,
    }

    public enum ShoesSizeType
    {
        US,
        UK,
        VietNam,
        LengthOfFoot ,
    }

    public enum PreferredHand
    {
        Right,
        Left,
    }

    public enum Gender
    {
        Male,
        Female,
    }

    public enum MaritalStatus
    {
        Single,
        Married,
    }

    public enum Workfield
    {
        None,
        Bussiness_Finance,
        Information_News_Entertainment,
        RealEstate,
        IndustrialProduction,
        Agroforestry_Fisher,
        Transport,
        RetailAndDistribution,
        Other,
    }
}
