using System.Collections.Generic;

namespace AdviceFirstApi.Domain.Response
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    }
}