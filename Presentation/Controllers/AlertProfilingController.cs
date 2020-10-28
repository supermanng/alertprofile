using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Applications.Handlers.Commands;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertProfilingController : BaseController
    {

        [ProducesResponseType(typeof(BaseMessageResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseMessageResponse), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        [HttpPost, Route("onboarduser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var res = await Mediator.Send(command);

            if (res.IsSuccessResponse)
            {
                return Ok(res);
            }
            else
            {
                return new BadRequestObjectResult((BaseMessageResponse)res);

            }
        }

        [ProducesResponseType(typeof(BaseMessageResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseMessageResponse), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        [HttpPost, Route("invoketransaction")]
        public async Task<IActionResult> InvokeTransaction([FromBody] InvokeTransactionCommand command)
        {
            var res = await Mediator.Send(command);

            if (res.IsSuccessResponse)
            {
                return Ok(res);
            }
            else
            {
                return new BadRequestObjectResult((BaseMessageResponse)res);

            }
        }

    }
}