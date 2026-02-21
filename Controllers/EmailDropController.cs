using System;
using System.Threading;
using System.Threading.Tasks;
using AdviceFirstApi.Domain.Models.Dto;
using AdviceFirstApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AdviceFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailDropController : ControllerBase
    {
        private readonly ILogger<EmailDropController> _logger;
        private readonly IEmailDropParser _emailDropParser;

        public EmailDropController(ILogger<EmailDropController> logger, IEmailDropParser emailDropParser)
        {
            _logger = logger;
            _emailDropParser = emailDropParser;
        }

        [HttpPost("parse")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [RequestSizeLimit(104857600)]
        [ProducesResponseType(typeof(EmailDropModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ParseEmail([FromForm] EmailDropRequestModel request, CancellationToken cancellationToken)
        {
            if (request?.EmailFile == null)
            {
                return BadRequest("Please upload an Outlook email file in the 'emailFile' form field.");
            }

            try
            {
                var parsedEmail = await _emailDropParser.ParseAsync(request.EmailFile, cancellationToken).ConfigureAwait(false);
                return Ok(parsedEmail);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to parse dropped email file: {FileName}", request.EmailFile.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to parse dropped email file.");
            }
        }
    }
}
