using System;
using System.Collections.Generic;
using AdviceFirstApi.Controllers;
using System.Threading.Tasks;
using AdviceFirstApi.Domain;
using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Domain.Models.Dto;
using AdviceFirstApi.Validators;
using AutoMapper;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;
using AdviceFirstApi.Mappings;
using AdviceFirstApi.Persistance;

namespace AdviceFirstApi.Test
{
    public class UserConrollerTests : IClassFixture<UserControllerClassFixture>
    {
        UserControllerClassFixture _fixture;
        
        ITestOutputHelper _itHelper;
        public UserConrollerTests(ITestOutputHelper itHelper, UserControllerClassFixture fixture)
        {
            _fixture = fixture;
            _itHelper = itHelper;
        }

        [Fact]
        public void UserNameIsNull()
        {
            var afuser = new AfUserModelCreate();
            var validator = new AfUserValidator();
            // var results = validator.Validate(afuser); // Validate(afuser, ruleSet: "all"); 
            validator.ShouldHaveValidationErrorFor(xx => xx.UserName, null as string);
        }
        [Fact]
        public void UserNameHasADomain()
        {
            var afuser = new AfUserModelCreate();
            var validator = new AfUserValidator();
            // Reload the data from the initial load
            _fixture.RefreshContext();
            // var results = validator.Validate(afuser); // Validate(afuser, ruleSet: "all"); 
            validator.ShouldHaveValidationErrorFor(xx => xx.UserName, "AdviceFirst\\jeco.jacob" as string);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async void UserControllerGetListUsers()
        {
            _itHelper.WriteLine("Start the test - UserControllerGetListUsers");
            //Arrange 
            var sut = new UserController(_fixture._loggerController, _fixture._repo, _fixture._mapper);
            //Act
            var listUsers = await sut.GetUsers();
            //var okObjectResult = listUsers as OkObjectResult;
            var okObjectResult = listUsers as OkObjectResult;
            //Assert

            Assert.NotNull(okObjectResult);
            var model = okObjectResult.Value as List<AfUserModelList>;
            Assert.NotNull(model);
            Assert.Equal(model[0].UserCode, "AA");
        }
    }
}
