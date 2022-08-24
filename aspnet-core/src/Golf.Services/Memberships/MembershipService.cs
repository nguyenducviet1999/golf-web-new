using AutoMapper;
using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Core.Dtos.Controllers.MembershipController.Requests;
using Golf.Core.Dtos.Controllers.MembershipController.Responses;
using Golf.Domain.Shared;
using Golf.Domain.Shared.OdooAPI;
using Golf.EntityFrameworkCore;
using Golf.Services.OdooAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Memberships
{
    public class MembershipService
    {
        private readonly DatabaseTransaction _databaseTransaction;
        public OdooAPIService _odooAPIService;
        private readonly IMapper _mapper;
        public MembershipService
        (
            DatabaseTransaction databaseTransaction,
            OdooAPIService odooAPIService,
            IMapper mapper
        )
        {
            _databaseTransaction = databaseTransaction;
            _odooAPIService = odooAPIService;
            _mapper = mapper;
        }
        public async Task<List<OdooMembershipResponse>> GetOdooMembershipResponses(int startIndex)
        {
            OdooMembershipRequestDto request = new OdooMembershipRequestDto(startIndex);
            var result = await _odooAPIService.CallAPI<OdooMembershipRequestDto, OdooResponse<OdooResult<List<OdooMembershipResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.Membership), request);
            return _mapper.Map<List<OdooMembershipResponse>>(result.Result.Data);
        }
        public async Task<OdooMembershipResponse> GetOdooMembershipResponse(int membershipID)
        {
            var result = await _odooAPIService.CallAPI<int, OdooResult<List<OdooMembershipResponseDto>>>(APIMethod.GET, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.Membership), membershipID);
            if (result.Data.First() == null)
                return new OdooMembershipResponse();
            else
            {
                return _mapper.Map<OdooMembershipResponse>(result.Data.First());
            }

        }
        public async Task<string> BuyOdooMembership(int productID)
        {
            BuyMembershipRequestDto request = new BuyMembershipRequestDto(productID);
            var result = await _odooAPIService.CallAPI<BuyMembershipRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.BuyMembership), request);
            return result.Result.Data.Message;
        }
        public async Task<List<OdooMyMembershipResponse>> GetMyOdooMembership(int startIndex)
        {
            var result = await _odooAPIService.CallAPI<string, OdooResult<List<OdooMyMembershipResponseDto>>>(APIMethod.GET, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.MyMembership) + "?" + "offset=" + startIndex + "&" + "limit=" + Const.PageSize, null);
            return _mapper.Map<List<OdooMyMembershipResponse>>(result.Data);
        }

    }
}
