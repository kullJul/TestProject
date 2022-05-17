using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.CQRS.Commands.RegisterAccount;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IMediator mediator;
        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Register(RegisterAccountCommand command)
        {
            await mediator.Send(command);
        }
    }
}
