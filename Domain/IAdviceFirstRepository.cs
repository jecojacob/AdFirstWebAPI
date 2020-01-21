using System.Collections.Generic;
using System.Threading.Tasks;
using AdviceFirstApi.Domain.Models;
using AdviceFirstApi.Domain.Models.Dto;

namespace AdviceFirstApi.Domain
{
    public interface IAdviceFirstRepository
    {

        Task<AfUser>  AddUserAsync (AfUser afUser);
        AfUser AddUser (AfUser afUser);
        void UpdateUser (AfUser afUser);
        void DeleteUser(AfUser afUser);
        Task<IEnumerable<AfUserModelList>> GetUsersAsync();
        Task<IEnumerable<AfUser>> GetUsers();



         
    }
}