using System;
using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Domain.Models.Dto;
using AdviceFirstApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AdviceFirstApi.Test
{
    public class GetContextWithData : IDisposable
    {

        public AdviceFirstDbContext _context;
        public   GetContextWithData ()
        {
            var options = new DbContextOptionsBuilder<AdviceFirstDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
             _context = new AdviceFirstDbContext(options);
            GetLoadData();
        }

        public void Dispose()
        {
           _context.Dispose();
        }

        public  void  GetLoadData()
        {
            // Add Users
            _context.AfUser.Add(new AfUser { Id = 1, UserCode = "AA", UserName = "AA User", UserCodeName = "AA - AA User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _context.AfUser.Add(new AfUser { Id = 2, UserCode = "BB", UserName = "BB User", UserCodeName = "BB - BB User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _context.AfUser.Add(new AfUser { Id = 3, UserCode = "CC", UserName = "CC User", UserCodeName = "CC - CC User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _context.AfUser.Add(new AfUser { Id = 4, UserCode = "DD", UserName = "DD User", UserCodeName = "DD - DD User", UserRole = "User", UserStatus = AfUserStatus.Active });
            _context.SaveChanges();
        }
    }
}