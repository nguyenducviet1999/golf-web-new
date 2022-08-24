using System;

using Golf.Domain.Shared.Golfer.Information;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    public class EditPersonalInformationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; } = null;
        public Gender Gender { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
    }

    public class EditContactInformationRequest
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class EditAddressInformationRequest
    {
        public int? CountryID { get; set; }
        public int? StateID { get; set; }
        public string Address { get; set; }
    }

    public class EditOccupationInformation
    {
        public string Workplace { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Workfield { get; set; }
        public string Website { get; set; }
    }

    public class EditClothingInformationRequest
    {
        public ClothesSizeType ClothesSizeType { get; set; }
        public ShoesSizeType ShoesSizeType { get; set; }
        public PreferredHand PreferredHand { get; set; }
        public int ShoesSize { get; set; }
        public int ShirtSize { get; set; }
        public int PantsSize { get; set; }
        //public int FootLength { get; set; }
    }

    public class EditQuoteRequest
    {
        public string Quote { get; set; }
    }

}
