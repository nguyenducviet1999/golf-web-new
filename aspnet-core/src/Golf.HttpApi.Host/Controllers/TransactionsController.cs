using Golf.Domain.Shared;
using Golf.Domain.Bookings;
using Golf.Services.Bookings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Golf.Domain.GolferData;
using Golf.Services.Products;
using Golf.EntityFrameworkCore.Repositories;
using Golf.HttpApi.Host.Helpers;
using Golf.Core.Dtos.Controllers.TransactionController.Respone;
using Golf.Core.Dtos.Controllers.TransactionController.Requests;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        private readonly ProductRepository _productRepository;

        public TransactionsController(TransactionService transactionService, ProductRepository productRepository)
        {
            _transactionService = transactionService;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Lấy dữ liệu giao dịch
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpGet("{transactionId}")]
        public ActionResult<TransactionResponse> Get(Guid transactionId)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.Get(golfer.Id, transactionId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy các giao dịch đang gửi yêu cầu
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("request/{startIndex}")]
        public ActionResult<List<TransactionResponse>> GetRequestTransaction(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.GetTransactionRequest(golfer.Id, startIndex);
            return Ok(result);
        }

        [HttpGet("Rejected/{startIndex}")]
        public ActionResult<List<TransactionResponse>> GetRejectedTransaction(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.GetTransactionRejected(golfer.Id, startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Lấy các giao dịch đã xác nhận
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("confirm/{startIndex}")]
        public ActionResult<List<TransactionResponse>> GetConfirmTransaction(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.GetTransactionConfirm(golfer.Id, startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Lấy các giao dịch đã hoàn thành
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("complete/{startIndex}")]
        public ActionResult<List<TransactionResponse>> GetCompleteTransaction(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.GetTransactionCompleted(golfer.Id, startIndex);
            return Ok(result);

        }

        /// <summary>
        /// Lấy các giao dịch đã hủy
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Cancel/{startIndex}")]
        public ActionResult<List<TransactionResponse>> GetCancelTransaction(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.GetTransactionCancelled(golfer.Id, startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Tạo giao dịch mới
        /// </summary>
        /// <param name="transaction">Dữ liệu giao dịch</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> Post([FromBody] Transaction transaction)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var product = _productRepository.Get(transaction.ProductID);
            if (product == null)
            { return BadRequest("Product isn't exit !"); }
            else
            {
                //product.IsBooking = true;
                _productRepository.Update(product);
            }
            transaction.Status = TransactionStatus.Request;
            var result = _transactionService.Add(golfer.Id, transaction);
            return Ok(result);
        }

        // PUT api/<PostController>/5
        /// <summary>
        /// Hủy giao dịch 
        /// </summary>
        /// <param name="transactionId">Định danh giao dịch</param>
        /// <returns></returns>
        [HttpPut("{transactionId}/cancel")]
        public ActionResult<bool> Cancel(Guid transactionId)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.Cancel(golfer.Id, transactionId);
            return Ok(result);
        }
        /// <summary>
        /// Đặt lại sân
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="rebookingRequest"></param>
        /// <returns></returns>
        [HttpPut("{transactionId}/Rebooking")]
        public ActionResult<bool> Rebooking(Guid transactionId, RebookingRequest rebookingRequest )
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.ReBooking(golfer.Id, transactionId, rebookingRequest);
            return Ok(result);
        }

        /// <summary>
        /// Xác nhận giao dịch
        /// </summary>
        /// <param name="transactionId">Định danh giao dịch</param>
        /// <returns></returns>
        [HttpPut("{transactionId}/Confirm")]
        public ActionResult<bool> Confirm(Guid transactionId)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.ConFirm(golfer.Id, transactionId);
            return Ok(result);
        }

        
        // DELETE api/<PostController>/5
        /// <summary>
        /// Xóa giao dịch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{transactionId}")]
        public ActionResult<bool> Delete(Guid id)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _transactionService.Delete(golfer.Id, id);
            return Ok(result);
        }
    }
}
