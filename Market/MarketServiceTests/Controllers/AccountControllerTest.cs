using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarketService.Controllers;
using System.Web.Http.Results;
using Contracts.Models;
using FluentAssertions;

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

        [TestMethod]
        public void GetUserTest()
        {
            var userId = Guid.NewGuid();
            var controller = GetAccountController(userId.ToString());

            controller.CreateUser("testUser");
            var response = controller.GetUser("");

            ((OkNegotiatedContentResult<User>)response).Content
                .Should().BeEquivalentTo(new User
                {
                    UserId = userId,
                    Username = "testUser"
                });
        }

        [TestMethod]
        public void AddFunds_InvalidUser()
        {
            var controller = GetAccountController(Guid.NewGuid().ToString());
            controller.CreateUser("testUser");

            controller.Request.Headers.Clear();
            controller.Request.Headers.Add("UserId", Guid.NewGuid().ToString());
            var response =  controller.AddFunds(100m);

            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void GetFunds_InvalidUser()
        {
            var controller = GetAccountController(Guid.NewGuid().ToString());
            controller.CreateUser("testUser");

            controller.AddFunds(100m);
            controller.Request.Headers.Clear();
            controller.Request.Headers.Add("UserId", Guid.NewGuid().ToString());
            var response = controller.GetFunds("");

            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void AddFunds()
        {
            var controller = GetAccountController(Guid.NewGuid().ToString());
            controller.CreateUser("testUser");

            var response = controller.AddFunds(100m);

            Assert.IsInstanceOfType(response, typeof(OkResult));
        }

        [TestMethod]
        public void GetFunds()
        {
            var controller = GetAccountController(Guid.NewGuid().ToString());
            controller.CreateUser("testUser");
            controller.AddFunds(100m);

            var response = controller.GetFunds("");

            Assert.AreEqual(100m, ((OkNegotiatedContentResult<decimal>)response).Content);
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
