using AutoMapper;
using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Core.Dtos.Controllers.OdooResourcesController.Requests;
using Golf.Core.Dtos.Controllers.OdooResourcesController.Response;
using Golf.Domain.Shared.OdooAPI;
using Golf.EntityFrameworkCore;
using Golf.Services.OdooAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Resource
{
    public class ResourceService
    {
        private readonly DatabaseTransaction _databaseTransaction;
        public OdooAPIService _odooAPIService;
        private readonly IMapper _mapper;
        public ResourceService(DatabaseTransaction databaseTransaction, OdooAPIService odooAPIService, IMapper mapper)
        {
            _databaseTransaction = databaseTransaction;
            _mapper = mapper;
            _odooAPIService = odooAPIService;
        }
        public async Task<List<OdooAddressResponse>> GetOdooCountries()
        {
            OdooCountryRequestDto request = new OdooCountryRequestDto();
            var result = await _odooAPIService.CallAPI<OdooCountryRequestDto, OdooResponse<OdooResult<List<OdooAddressResponse>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ListCountry), request);
            return result.Result.Data.OrderBy(c=>c.Code).ToList();
        }  
        public async Task<OdooAddressResponse> GetOdooCountrieByID(int iD)
        {
            OdooCountryRequestDto request = new OdooCountryRequestDto(iD);
            var result = await _odooAPIService.CallAPI<OdooCountryRequestDto, OdooResponse<OdooResult<List<OdooAddressResponse>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ListCountry), request);
            return result.Result.Data.FirstOrDefault();
        }
        public async Task<OdooAddressResponse> GetOdooStateByID(int iD)
        {
            OdooStateRequest request = new OdooStateRequest(iD);
            var result = await _odooAPIService.CallAPI<OdooStateRequestDto, OdooResponse<OdooResult<List<OdooAddressResponse>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ListState), request.GetStateByIDRequest());
            return result.Result.Data.FirstOrDefault();
        }
        public async Task<List<OdooAddressResponse>> GetOdooStateByCountryID(int iD)
        {
            OdooStateRequest request = new OdooStateRequest(iD);
            var result = await _odooAPIService.CallAPI<OdooStateRequestDto, OdooResponse<OdooResult<List<OdooAddressResponse>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ListState), request.GetListStateByCountryIDRequest());
            return result.Result.Data.OrderBy(c => c.Code).ToList();
        }
    }
}
