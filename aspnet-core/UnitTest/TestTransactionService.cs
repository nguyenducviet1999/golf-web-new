using Golf;
using Golf.Core.Dtos.Controllers.TransactionController.Requests;
using Golf.Domain.Bookings;
using Golf.Domain.Shared;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Bookings;
using Golf.Services.Courses;
using Golf.Services.Locations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    class TestTransactionService
    {
        private TransactionService _transactionService;
        private TransactionRepository _transactionRepository;
        [SetUp]
        public void Setup()
        {
            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();
            var serviceScope = webHost.Services.CreateScope();
            _transactionService = serviceScope.ServiceProvider.GetService<TransactionService>();
            _transactionRepository = serviceScope.ServiceProvider.GetService<TransactionRepository>();
        }

        //[Test]
        //public void TestGetCancelTransaction()
        //{
        //    var reusult = _transactionService.GetTransactionCancelled(new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"), 0);
        //  //  Assert.IsTrue(reusult.IsCompletedSuccessfully);
        //}
        //[Test]
        //public void TestGetRequestTransasction()
        //{
        //    var reusult = _transactionService.GetTransactionRequest(new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),0);
        //       // Assert.IsTrue(reusult.IsCompletedSuccessfully);
        //}
        //public void TestGetCompletedTransasction()
        //{
        //    var reusult = _transactionService.GetTransactionCompleted(new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),0);
        //        //Assert.IsTrue(reusult.IsCompletedSuccessfully);
        //}
        //public void TestGetConfirmTransasction()
        //{
        //    var reusult = _transactionService.GetTransactionConfirm(new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),0);
        //        //Assert.IsTrue(reusult.IsCompletedSuccessfully);
        //}
    }
}
