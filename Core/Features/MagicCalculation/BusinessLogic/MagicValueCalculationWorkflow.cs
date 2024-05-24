using Abstraction.KeyValueStore;
using Core.Features.MagicCalculation.Domain;
using MediatR;
using NodaTime;

namespace Core.Features.MagicCalculation.BusinessLogic;

public class MagicValueCalculationWorkflow
{
    public record Request(int Key, decimal InputValue) : IRequest<Response>;

    public record Response(decimal computed_value, decimal input_value, decimal? previous_value);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        IClock _clock;
        IKeyValueStore<int, CalculationValue> _store;

        public RequestHandler(IClock clock, IKeyValueStore<int, CalculationValue> store)
        {
            _clock = clock;
            _store = store;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var previousOutputValue = await _store.TryGetValueAsync(request.Key);

            if (previousOutputValue is null)
            {
                var defaultValue = CalculationValue.GetDefault(_clock);

                await _store.UpsertValueAsync(request.Key, CalculationValue.GetDefault(_clock));

                return new Response(
                    computed_value: defaultValue.Value,
                    input_value: request.InputValue,
                    previous_value: previousOutputValue?.Value
                );
            }

            var nextValue = previousOutputValue.CalculateNextValue(_clock, request.InputValue);

            await _store.UpsertValueAsync(
                request.Key,
                previousOutputValue.CalculateNextValue(_clock, request.InputValue)
            );

            return new Response(
                computed_value: nextValue.Value,
                input_value: request.InputValue,
                previous_value: previousOutputValue?.Value
            );
        }
    }
}
