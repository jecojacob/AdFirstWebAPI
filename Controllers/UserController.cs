namespace AdviceFirstApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AdviceFirstApi.Domain;
    using AdviceFirstApi.Domain.Models.Dto;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IAdviceFirstRepository _repo;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repo"></param>
        /// <param name="mapper"></param>
        public UserController(ILogger<UserController> logger, IAdviceFirstRepository repo, IMapper mapper)
        {
            _logger = logger;
            _repo = repo;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.Log(LogLevel.Information, "Getting the list of Users  --- ");
            var userList = await _repo.GetUsersAsync();
            if (userList == null) return  NotFound();
            var userListModel = _mapper.Map<IEnumerable<AfUserModelList>>(userList);
            return  Ok(userListModel);
        }
    }
}