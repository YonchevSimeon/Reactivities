namespace API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IMediator mediator;
        
        protected IMediator Mediator 
        => this.mediator ?? (this.mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}