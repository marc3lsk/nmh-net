using Core.Features.MagicCalculation.BusinessLogic;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        IMediator _mediator;

        public CalculationController(ILogger<WeatherForecastController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public Task<MagicValueCalculationWorkflow.Response> Post()
        {
            return _mediator.Send(new MagicValueCalculationWorkflow.Request(Key: 1, InputValue: 1));
        }
    }
}
