using System;
using AdviceFirstApi.Controllers;
using AdviceFirstApi.Domain;
using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Domain.Models.Dto;
using AdviceFirstApi.Mappings;
using AdviceFirstApi.Persistance;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace AdviceFirstApi.Test
{
    public class UserControllerClassFixture : IDisposable
    {

        public ILogger<AdviceFirstRepository> _loggerRepo;
        public ILogger<UserController> _loggerController;
        public IMapper _mapper;
        public AdviceFirstDbContext _dataContext;
        public AdviceFirstRepository _repo;

        public UserControllerClassFixture()
        {
            // testing Console.log
            //create mapper for mapping all the entities
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
            // Or var mock = new Mock<ILogger<UserController>>(); logger = mock.Object;
            //Get all the context
             var options = new DbContextOptionsBuilder<AdviceFirstDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            RefreshContext();
            _loggerRepo = Mock.Of<ILogger<AdviceFirstRepository>>();
            _loggerController = Mock.Of<ILogger<UserController>>();
            _repo = new AdviceFirstRepository(_dataContext, _mapper, _loggerRepo);
            
        }

        public void Dispose()
        {
            _mapper = null;
            _dataContext.Dispose();
           _loggerRepo = null;
           _loggerController = null;
            _repo = null;
        }

        public void RefreshContext()
        {
            var options = new DbContextOptionsBuilder<AdviceFirstDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
             _dataContext = new AdviceFirstDbContext(options);
            // Add Users
            _dataContext.AfUser.Add(new AfUser { Id = 1, UserCode = "AA", UserName = "AA User", UserCodeName = "AA - AA User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _dataContext.AfUser.Add(new AfUser { Id = 2, UserCode = "BB", UserName = "BB User", UserCodeName = "BB - BB User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _dataContext.AfUser.Add(new AfUser { Id = 3, UserCode = "CC", UserName = "CC User", UserCodeName = "CC - CC User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _dataContext.AfUser.Add(new AfUser { Id = 4, UserCode = "DD", UserName = "DD User", UserCodeName = "DD - DD User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _dataContext.SaveChanges();
        }

    }
}