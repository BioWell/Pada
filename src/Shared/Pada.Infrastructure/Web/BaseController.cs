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
        
        private IMediator _mediator;
        
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        
        private IMapper _mapper;
        
        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
    }
}