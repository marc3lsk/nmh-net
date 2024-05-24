using Abstraction.KeyValueStore;
using Abstraction.MessageBus;
using Core.Features.MagicCalculation.Contracts;
using Core.Features.MagicCalculation.Domain;
using MassTransit;
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
        IBus _bus;
        IMessagePublisher _messagePublisher;

        public RequestHandler(
            IClock clock,
            IKeyValueStore<int, CalculationValue> store,
            IBus bus,
            IMessagePublisher messagePublisher
        )
        {
            _clock = clock;
            _store = store;
            _bus = bus;
            _messagePublisher = messagePublisher;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var previousOutputValue = await _store.TryGetValueAsync(request.Key);

            if (previousOutputValue is null)
            {
                var defaultValue = CalculationValue.GetDefault(_clock);

                await _store.UpsertValueAsync(request.Key, CalculationValue.GetDefault(_clock));

                return await HandleResponse(
                    new Response(
                        computed_value: defaultValue.Value,
                        input_value: request.InputValue,
                        previous_value: previousOutputValue?.Value
                    ),
                    cancellationToken
                );
            }

            var nextValue = previousOutputValue.CalculateNextValue(_clock, request.InputValue);

            await _store.UpsertValueAsync(
                request.Key,
                previousOutputValue.CalculateNextValue(_clock, request.InputValue)
            );

            return await HandleResponse(
                new Response(
                    computed_value: nextValue.Value,
                    input_value: request.InputValue,
                    previous_value: previousOutputValue?.Value
                ),
                cancellationToken
            );
        }

        async Task<Response> HandleResponse(Response response, CancellationToken cancellationToken)
        {
            await _bus.Publish(
                new MagicValueCalculationResultMessage(
                    computed_value: response.computed_value,
                    input_value: response.input_value,
                    previous_value: response.previous_value
                ),
                cancellationToken
            );

            _messagePublisher.Publish("my-queue-worker", $"{response.computed_value}");

            return response;
        }
    }
}
