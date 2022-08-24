using System;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    /// <summary>
    /// is Validated 
    /// </summary>
    public class AddProfileRequest
    {
        public Guid GolferID { get; set; }
    }
}
