using Core.Features.MagicCalculation.BusinessLogic;
using Core.Features.MagicCalculation.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class CalculationController : ControllerBase
    {
        private readonly ILogger<CalculationController> _logger;
        IMediator _mediator;

        public CalculationController(ILogger<CalculationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("calculation/{key:int}")]
        public async Task<MagicValueCalculationEndpointResponse> Post(
            int key,
            [FromBody] MagicValueCalculationEndpointRequest data
        )
        {
            var response = await _mediator.Send(
                new MagicValueCalculationWorkflow.Request(Key: key, InputValue: data.input)
            );

            return new MagicValueCalculationEndpointResponse(
                computed_value: response.ComputedValue,
                input_value: response.InputValue,
                previous_value: response.PreviousValue
            );
        }
    }
}
