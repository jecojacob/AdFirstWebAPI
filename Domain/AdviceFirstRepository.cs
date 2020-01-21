using System.Collections.Generic;
using System.Threading.Tasks;
using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Persistance;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AdviceFirstApi.Domain.Models.Dto;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AdviceFirstApi.Domain
{
    public class AdviceFirstRepository : IAdviceFirstRepository
    {
        private readonly AdviceFirstDbContext _context;
        private readonly IMapper _mapper;
        private ILogger _logger;
        public AdviceFirstRepository(AdviceFirstDbContext context , IMapper mapper ,  ILogger<AdviceFirstRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException (nameof(context));
            _mapper = mapper;
            _logger = logger;
        }

        public Task<AfUser> AddUserAsync(AfUser afUser)
        {
            throw new NotImplementedException();
        }

        public AfUser AddUser(AfUser afUser)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(AfUser afUser)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(AfUser afUser)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AfUser>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AfUserModelList>> GetUsersAsync()
        {
            try {
            var userList = await _context.AfUser
                        .OrderBy(c=> c.UserName).ToListAsync();
           var userListDto = _mapper.Map<IEnumerable<AfUserModelList>>(userList);        
             return userListDto;
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
        }
    }
}