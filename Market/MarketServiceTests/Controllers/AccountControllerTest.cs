using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarketService.Controllers;
using System.Web.Http.Results;

namespace MarketServiceTests.Controllers
{
    [TestClass]
    public class AccountControllerTest : TestHarness
    {
        [TestMethod]
        public void GetUserTest_NoUserHeader()
        {
            var controller = GetAccountController();

            var response = controller.GetUser(null);

            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void GetUserTest_InvalidUserId()
        {
            var controller = GetAccountController(userId: "InvalidGuid");

            var response = controller.GetUser(null);

            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void GetUserTest_NonExtantUser()
        {
            var userId = Guid.NewGuid();
            var controller = GetAccountController(userId.ToString());

            var response = controller.GetUser(userId);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateUserTest_NoUserHeader()
        {
            var controller = GetAccountController();

            var response = controller.CreateUser("testUser");

            Assert.IsInstanceOfType(response, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var controller = GetAccountController(Guid.NewGuid().ToString());

            var response = controller.CreateUser("testUser");

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<Guid?>));
        }

        private AccountController GetAccountController(string userId = null)
        {
            AccountController.Request = new HttpRequestMessage();
            if (userId != null)
                AccountController.Request.Headers.Add("UserId", userId);

            return AccountController;
        }
    }
}
