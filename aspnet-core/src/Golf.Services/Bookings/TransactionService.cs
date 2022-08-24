using Golf.Domain.Shared;
using Golf.Core.Exceptions;
using Golf.Domain.Bookings;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Golf.Core.Dtos.Controllers.TransactionController.Requests;
using System.Linq;
using Golf.Services.Courses;
using Golf.Core.Dtos.Controllers.TransactionController.Respone;
using Golf.Domain.Shared.Transactions;
using Golf.Core.Dtos.Controllers.ShopControler.Request;

namespace Golf.Services.Bookings
{
    public class TransactionService
    {
        readonly private TransactionRepository _transactionRepository;
        readonly private ProductRepository _productRepository;
        readonly private CourseRepository _courseRepository;
        readonly private MemberShipService _courseMemberShipService;
        readonly private CourseExtensionService _courseExtensionService;
        readonly private ShopService _shopService;

        public TransactionService(ShopService shopService, CourseExtensionService courseExtensionService, MemberShipService courseMemberShipService, TransactionRepository transactionRepository, ProductRepository productRepository, CourseRepository courseRepository)
        {
            _shopService = shopService;
            _transactionRepository = transactionRepository;
            _productRepository = productRepository;
            _courseRepository = courseRepository;
            _courseMemberShipService = courseMemberShipService;
            _courseExtensionService = courseExtensionService;
        }

        /// <summary>
        /// Lấy giao dịch theo id của giảo dịch
        /// </summary>
        /// <param name="uId">Id người dùng hiện tại</param>
        /// <param name="id">ID giao dịch</param>
        /// <returns></returns>
        public TransactionResponse Get(Guid uId, Guid id)
        {
            var result = _transactionRepository.Get(id);
            var product = _productRepository.Get(result.ProductID);
            if (uId == result.CreatedBy || uId == product.CreatedBy)
            {
                if (result != null)
                {
                    return this.GetTransactionResponse(result);
                }
                else
                {
                    throw new BadRequestException("Not found Transaction !");
                }
            }
            else
            {
                throw new BadRequestException("Can't get Transaction !");
            }
        }
        /// <summary>
        /// Lấy các giao dcjh đang ở trạng thái yêu cầu
        /// </summary>
        /// <param name="uId">Id người dùng</param>
        /// <returns></returns>
        public List<TransactionResponse> GetTransactionRequest(Guid uId, int startIndex)
        {
            var result = _transactionRepository.FindWithProduct(t => t.CreatedBy == uId && t.Status == TransactionStatus.Request);
            if (result != null)
            {
                return this.GetTransactionResponses(result.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }
        /// <summary>
        /// Lấy các giao dcjh đang ở trạng thái hủy
        /// </summary>
        /// <param name="uId">id người dùng hiện tại</param>
        /// <returns></returns>
        public List<TransactionResponse> GetTransactionCancelled(Guid uId, int startIndex)
        {
            var results = _transactionRepository.FindWithProduct(t => t.CreatedBy == uId && t.Status == TransactionStatus.Cancelled);
            if (results != null)
            {
                return this.GetTransactionResponses(results.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }
        /// <summary>
        /// Lấy các giao dcjh đang ở trạng thái hoàn  thành
        /// </summary>
        /// <param name="uId">id người dùng hiện tại</param>
        /// <returns></returns>
        public List<TransactionResponse> GetTransactionCompleted(Guid uId, int startIndex)
        {
            var results = _transactionRepository.FindWithProduct(t => t.CreatedBy == uId && t.Status == TransactionStatus.Completed);
            if (results != null)
            {
                return this.GetTransactionResponses(results.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }
        public List<TransactionResponse> GetTransactionRejected(Guid uId, int startIndex)
        {
            var results = _transactionRepository.FindWithProduct(t => t.CreatedBy == uId && t.Status == TransactionStatus.Rejected);
            if (results != null)
            {
                return this.GetTransactionResponses(results.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }

        /// <summary>
        /// Lấy các giao dcjh đang ở trạng thái đã xác nhận
        /// </summary>
        /// <param name="uId">id người dùng hiện tại</param>
        /// <returns></returns>
        public List<TransactionResponse> GetTransactionConfirm(Guid uId, int startIndex)
        {
            var results = _transactionRepository.FindWithProduct(t => t.CreatedBy == uId && t.Status == TransactionStatus.Confirm);
            if (results != null)
            {
                return this.GetTransactionResponses(results.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }
        public List<TransactionResponse> GetTransactionByCourse(Guid uId,Guid courseID, TransactionStatus transactionStatus,int startIndex)
        {
            var course = _courseRepository.Get(courseID);
            if(course==null)
            {
                throw new BadRequestException("Not found course!");
            }
            if(course.OwnerID!=uId)
            {
                throw new ForbiddenException("Not allow!");
            }
            var results = _transactionRepository.FindWithProduct(t => t.Product.CourseID == courseID && t.Status == transactionStatus);
            if (results != null)
            {
                return this.GetTransactionResponses(results.Skip(startIndex).Take(Const.PageSize).ToList());
            }
            else
            {
                throw new BadRequestException("Not found Transaction !");
            }
        }
        /// <summary>
        /// Tạo giao dịch mới
        /// </summary>
        /// <param name="transactionRequest"></param>
        /// <returns></returns>
        public bool AddTransactionRequest(TransactionRequest transactionRequest)
        {
            var tmp = _transactionRepository.Find(t => t.GolferID == transactionRequest.GolferID && t.ProductID == transactionRequest.ProductID && t.Status == TransactionStatus.Request);
            if (tmp.Count() > 0)
                throw new BadRequestException("Transaction is exit");
            var product = _productRepository.Get(transactionRequest.ProductID);
            if (DateTime.Compare(product.Date.Date + product.TeeTime, DateTime.Now) < 0)
            {
                throw new ForbiddenException("Can not booking");
            }
            if (product.FullBooking == true)
            {
                throw new BadRequestException("Tee time full slots");
            }
            var membership = _courseMemberShipService.IsMemberShip(transactionRequest.GolferID);// false;//đang phaasat triển
            var price = 0.0;
            if (membership == true)
                price = product.Price * (1 - product.MembershipPromotion - product.Promotion);
            else
                price = product.Price * (1 - product.Promotion);
            Transaction transaction = new Transaction()
            {
                GolferID = transactionRequest.GolferID,
                ProductID = transactionRequest.ProductID,
                ContactInfo = transactionRequest.ContactInfo,
                Status = TransactionStatus.Request,
                NumberOfGolfer = transactionRequest.NumberOfGolfer,
                PromotionCode = transactionRequest.PromotionCode,
                Amount = Math.Round(price*transactionRequest.NumberOfGolfer, 0)
            };
            _transactionRepository.Add(transaction);

            if (_transactionRepository.Find(t => t.ProductID == transactionRequest.ProductID).Sum(t => t.NumberOfGolfer) >= 4)
            {
                product.FullBooking = true;
                _productRepository.UpdateEntity(product);
            }
            return true;
        }

        /// <summary>
        /// Thêm giao dịch mới 
        /// </summary>
        /// <param name="uId">ID người dùng hiện tại(người thực hiện giao dịch)</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Add(Guid uId, Transaction transaction)
        {
            transaction.CreatedBy = uId;
            _transactionRepository.Add(transaction);
            return true;
        }

        /// <summary>
        /// Hủy giao dịch trước teetime
        /// </summary>
        /// <param name="uId">ID ngươi dùng</param>
        /// <param name="tId">Id giao dịch</param>
        /// <returns></returns>
        public bool Cancel(Guid uId, Guid tId)
        {
            var trans = _transactionRepository.Get(tId);
            var product = _productRepository.Get(trans.ProductID);
            if (uId == (Guid)trans.CreatedBy)
            {
                if (trans.Status != TransactionStatus.Request)
                {
                    throw new ForbiddenException("Can not cancel");
                }
                trans.Status = TransactionStatus.Cancelled;
                _transactionRepository.UpdateEntity(trans);
                return true;
            }
            else
            {
                throw new BadRequestException("this golfer can't cancel");
            }
        }

        /// <summary>
        /// đặt lại các giao dịch đã hủy
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="tID"></param>
        /// <returns></returns>
        public bool ReBooking(Guid currentID, Guid tID, RebookingRequest rebookingRequest)
        {
            var trans = _transactionRepository.Get(tID); 
            if (trans == null)
                throw new NotFoundException("Transaction not found");
            var product = _productRepository.Get(rebookingRequest.ProductID);
            if (DateTime.Compare(product.Date.Date + product.TeeTime, DateTime.Now) < 0)
            {
                throw new ForbiddenException("Can not rebooking");
            }
            if (product.FullBooking == true)
            {
                throw new BadRequestException("Tee time full slots");
            }
            var membership = _courseMemberShipService.IsMemberShip(trans.GolferID);// false;//đang phaasat triển
            var price = 0.0;
            if (membership == true)
                price = product.Price * (1 - product.MembershipPromotion - product.Promotion);
            else
                price = product.Price * (1 - product.Promotion);
            if (currentID == (Guid)trans.CreatedBy)
            {
                if (trans.Status != TransactionStatus.Cancelled)
                {
                    throw new ForbiddenException("Can not rebooking");
                }
                trans.Status = TransactionStatus.Request;
                trans.MoreRequests = rebookingRequest.MoreRequests;
                trans.NumberOfGolfer = rebookingRequest.NumberOfGolfer;
                trans.PromotionCode = rebookingRequest.PromotionCode;
                trans.ContactInfo = rebookingRequest.ContactInfo;
                trans.Amount = Math.Round(price * trans.NumberOfGolfer, 0);
                _transactionRepository.UpdateEntity(trans);
                return true;
            }
            else
            {
                throw new BadRequestException("this golfer can't rebooking");
            }
        }
        public bool UpdateTransaction(Guid currentID, Guid transactionID, TransactionStatusRequest transactionStatusRequest)
        {
            var trans = _transactionRepository.Get(transactionID);
            var product = _productRepository.Get(trans.ProductID);
            var course = _courseRepository.Get(product.CourseID);
            if (course == null)
            {
                throw new NotFoundException("Course don't exit");
            }
            var courseAdminID = course.OwnerID;
            if (currentID != courseAdminID)
            {
                throw new BadRequestException("This golfer can't confirm");
            }
            switch (transactionStatusRequest)
            {
                case TransactionStatusRequest.Confirm:
                    {
                        if (trans.Status == TransactionStatus.Request)
                        {
                            OdooCourseBookingRequest request = new OdooCourseBookingRequest();
                            request.Amount = trans.Amount;
                            if(trans.SalesmanID!=null)
                            {
                                request.GSAID = trans.Salesman.OdooPartnerID;
                            }    
                            var tmp= _shopService.CourseBooking(request).Result;
                            if(tmp==true)
                            {
                                trans.Status = TransactionStatus.Confirm;
                                _transactionRepository.UpdateEntity(trans);
                            }    
                            return true;
                        }
                        return false;
                    }
                case TransactionStatusRequest.Rejected:
                    {
                        if (currentID == course.OwnerID && trans.Status == TransactionStatus.Request)
                        {
                            trans.Status = TransactionStatus.Rejected;
                            _transactionRepository.UpdateEntity(trans);
                            return true;
                        }
                        return false;
                    }
                case TransactionStatusRequest.Completed:
                    {
                        if (currentID == course.OwnerID && trans.Status == TransactionStatus.Confirm)
                        {
                            trans.Status = TransactionStatus.Completed;
                            _transactionRepository.UpdateEntity(trans);
                            return true;
                        }
                        else
                        {
                            throw new BadRequestException("Can not complete");
                        }
                    }
                case TransactionStatusRequest.Deleted:
                    {
                        if ((trans.CreatedBy == currentID || product.CreatedBy == currentID) && trans.Status == TransactionStatus.Request)
                        {
                            _transactionRepository.RemoveEntity(trans);
                            return true;
                        }
                        else
                        {
                            throw new BadRequestException("Can not delete Transaction !");
                        }
                    }
                default:
                    {
                        throw new BadRequestException("TransactionStatus invalid!");
                    }
            }

        }

        /// <summary>
        /// Xác nhận giao dịch 
        /// </summary>
        /// <param name="uId">ID ngươi dùng hiện thời</param>
        /// <param name="tId">ID giao dịch</param>
        /// <returns></returns>
        public bool ConFirm(Guid uId, Guid tId)
        {
            var trans = _transactionRepository.Get(tId);
            var product = _productRepository.Get(trans.ProductID);
            var course = _courseRepository.Get(product.CourseID);
            if (course == null)
            {
                throw new NotFoundException("Course don't exit");
            }
            var courseAdminID = course.OwnerID;
            if (uId != courseAdminID)
            {
                throw new BadRequestException("This golfer Can't confirm");
            }
            if (trans.Status == TransactionStatus.Request)
            {
                trans.Status = TransactionStatus.Confirm;
                _transactionRepository.UpdateEntity(trans);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hoàn thành giao dịch
        /// </summary>
        /// <param name="uId">ID ngươi dùng hiện thời</param>
        /// <param name="tId">ID giao dịch</param>
        /// <returns></returns>
        public bool Complete(Guid uId, Guid tId)
        {
            var trans = _transactionRepository.Get(tId);
            var product = _productRepository.Get(trans.ProductID);
            var course = _courseRepository.Get(product.CourseID);
            // var course=_courseRepository.Get(product.)
            if (uId == course.OwnerID && trans.Status == TransactionStatus.Confirm)
            {
                trans.Status = TransactionStatus.Completed;
                _transactionRepository.UpdateEntity(trans);
                return true;
            }
            else
            {
                throw new BadRequestException("Can not complete");
            }
        }

        /// <summary>
        /// Từ chối giao dịch
        /// </summary>
        /// <param name="uId">ĐỊnh danh người dùng hiện thời</param>
        /// <param name="tId">Định danh giao dịch</param>
        /// <returns></returns>
        public bool Rejected(Guid uId, Guid tId)
        {
            var trans = _transactionRepository.Get(tId);
            var product = _productRepository.Get(trans.ProductID);
            var course = _courseRepository.Get(product.CourseID);
            if (uId == course.OwnerID && trans.Status == TransactionStatus.Rejected)
            {
                trans.Status = TransactionStatus.Rejected;
                _transactionRepository.UpdateEntity(trans);
                return true;
            }
            else
            {
                throw new BadRequestException("Can not Rejected");
            }
        }

        /// <summary>
        /// Xóa giao dịch
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(Guid uId, Guid id)
        {
            var result = _transactionRepository.Get(id);
            var product = _productRepository.Get(result.ProductID);
            if (result != null)
            {
                if ((result.CreatedBy == uId || product.CreatedBy == uId) && result.Status == TransactionStatus.Request)
                {
                    _transactionRepository.RemoveEntity(result);
                    return true;
                }
                else
                {
                    throw new BadRequestException("Can not delete Transaction !");
                }
            }
            else
            {
                throw new BadRequestException("This Transaction dosen't exit !");
            }
        }

        public TransactionResponse GetTransactionResponse(Transaction t)
        {
            TransactionResponse response = new TransactionResponse();
            response.Date = t.Product.Date;
            response.CourseImage = t.Product.Course.Cover;
            response.CourseName = t.Product.Course.Name;
            response.CourseID = t.Product.Course.ID;
            response.CourseAddress = t.Product.Course.Location.Address;
            response.Price = t.Amount;//_ t.Product.Course.LocationID;
            response.ID = t.ID;
            response.TotalPlayer = t.NumberOfGolfer;
            response.Date = t.Product.Date;
            response.TeeTime = t.Product.TeeTime;
            response.LisExtension = _courseExtensionService.GetExtensionByListID(t.Product.LisExtensionID);
            response.PromotionCode = t.PromotionCode;
            response.MoreRequests = t.MoreRequests;
            response.Description = t.Description;
            response.ContactInfo = t.ContactInfo;
            return response;
        }

        public List<TransactionResponse> GetTransactionResponses(List<Transaction> transactions)
        {
            List<TransactionResponse> transactionResponses = new List<TransactionResponse>();
            foreach (var i in transactions)
            {
                transactionResponses.Add(this.GetTransactionResponse(i));
            }
            return transactionResponses;
        }
    }
}
