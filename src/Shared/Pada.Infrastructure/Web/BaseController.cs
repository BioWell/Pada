using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Infrastructure.Web
{
    [Route(BaseApiPath + "/[controller]")]
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected const string BaseApiPath = "api/v{version:apiVersion}";
        
        private IMediator _mediatorInstance;
        
        protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
        
        private IMapper _mapperInstance;
        
        protected IMapper Mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();
    }
}