using System;

using Golf.Domain.Shared.Golfer.Information;
namespace Golf.Core.Dtos.Controllers.ProfileController.Responses
{
    public class FullProfileResponse
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public PreferredHand PreferredHand { get; set; }
        public string Workplace { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public ClothesSizeType ClothesSizeType { get; set; }
        public ShoesSizeType ShoesSizeType { get; set; }
        public int ShoesSize { get; set; } 
        public int ShirtSize { get; set; }
        public int PantsSize { get; set; }
        //public int FootLength { get; set; }
        public string Quote { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Workfield { get; set; }
        public string Website { get; set; }
        public virtual Address MyAddress => new Address() {Country=Country == null ? "" : Country, Adress=Address == null ? "" : Address, State=State==null?"":State};
    }
    public class Address
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string Adress { get; set; }
    }
}