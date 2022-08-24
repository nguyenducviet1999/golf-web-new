using AutoMapper;
using Golf.Core.Dtos.Controllers.OdooResourcesController.Response;
using Golf.Services;
using Golf.Services.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly ResourceService _resourceService ;
        private readonly IMapper _mapper;

        public ResourcesController(ResourceService resourceService, IMapper mapper, ShopService shopService)
        {
            _mapper = mapper;
            _resourceService = resourceService;
        }
        /// <summary>
        /// Lấy danh sách các quôc gia
        /// </summary>
        /// <returns></returns>
        [HttpGet("Countries")]
        public async Task<List<OdooAddressResponse>> GetCountries()
        {
            return await _resourceService.GetOdooCountries();
        }
        /// <summary>
        /// lấy country theo định danh
        /// </summary>
        /// <param name="iD">Định danh quốc gia</param>
        /// <returns></returns>
        [HttpGet("Countries/{iD}")]
        public async Task<OdooAddressResponse> GetCountrie(int iD)
        {
            return await _resourceService.GetOdooCountrieByID(iD);
        }
        /// <summary>
        /// lấy danh sách tỉnh thành phố của quốc gia
        /// </summary>
        /// <param name="countryID">Định danh quốc gia</param>
        /// <returns></returns>
        [HttpGet("Countries/{countryID}/States")]
        public async Task<List<OdooAddressResponse>> GetStates(int countryID)
        {
            return await _resourceService.GetOdooStateByCountryID(countryID);
        }
        /// <summary>
        /// lấy thông tin tỉnh thành phố
        /// </summary>
        /// <param name="iD">đinh danh tỉnh, thành phố</param>
        /// <returns></returns>
        [HttpGet("States/{iD}")]
        public async Task<OdooAddressResponse> GetState(int iD)
        {
            return await _resourceService.GetOdooStateByID(iD);
        }
    }
}
