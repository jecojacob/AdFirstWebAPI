using System.Threading;
using System.Threading.Tasks;
using AdviceFirstApi.Domain.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace AdviceFirstApi.Services
{
    public interface IEmailDropParser
    {
        Task<EmailDropModel> ParseAsync(IFormFile emailFile, CancellationToken cancellationToken = default);
    }
}
