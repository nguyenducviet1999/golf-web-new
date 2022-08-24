using Golf.Core.Dtos.Controllers.AdminController.Report.Request;
using Golf.Domain.GolferData;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.Reports;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        ReportService _reportService;
        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }
        /// <summary>
        /// Tạo báo cáo sai bảng điểm
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <returns></returns>
        // POST api/<ReportsController>
        [HttpPost]
        public ActionResult<bool> Post([FromForm] ReportRequest reportRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_reportService.Add(currentGolfer.Id, reportRequest).Result);
        }

    }
}
