using Abstraction.KeyValueStore;
using Abstraction.MessageBus;
using Core.Features.MagicCalculation.Contracts;
using Core.Features.MagicCalculation.Domain;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Core.Features.MagicCalculation.RequestHandlers;

public class MagicValueCalculationRequestHandler
{
    public record Request(int Key, decimal InputValue) : IRequest<Response>;

    public record Response(decimal ComputedValue, decimal InputValue, decimal? PreviousValue);

    public class RequestHandler : IRequestHandler<Request, Response>
    {
        IClock _clock;
        IKeyValueStore<int, CalculationValue> _store;
        IBus _bus;
        IMessagePublisher _messagePublisher;
        ILogger<MagicValueCalculationRequestHandler> _logger;

        public RequestHandler(
            IClock clock,
            IKeyValueStore<int, CalculationValue> store,
            IBus bus,
            IMessagePublisher messagePublisher,
            ILogger<MagicValueCalculationRequestHandler> logger
        )
        {
            _clock = clock;
            _store = store;
            _bus = bus;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var previousCalculationValue = await _store.TryGetValueAsync(request.Key);

            if (request.InputValue == 0 || previousCalculationValue is null)
            {
                var defaultValue = CalculationValue.GetDefault(_clock);

                return await HandleResponse(
                    request,
                    defaultValue,
                    previousCalculationValue,
                    cancellationToken
                );
            }

            try
            {
                var nextCalculationValue = previousCalculationValue.CalculateNextValue(
                    _clock,
                    request.InputValue
                );

                return await HandleResponse(
                    request,
                    nextCalculationValue,
                    previousCalculationValue,
                    cancellationToken
                );
            }
            catch (OverflowException ex)
            {
                // TODO: how to handle this error?

                _logger.LogError(ex, "Returning defalt value");

                var defaultValue = CalculationValue.GetDefault(_clock);

                return await HandleResponse(
                    request,
                    defaultValue,
                    previousCalculationValue,
                    cancellationToken
                );
            }
        }

        async Task<Response> HandleResponse(
            Request request,
            CalculationValue nextCalculationValue,
            CalculationValue? previousCalculationValue,
            CancellationToken cancellationToken
        )
        {
            await _store.UpsertValueAsync(request.Key, nextCalculationValue);

            await _bus.Publish(
                new MagicValueCalculationResultMessage(
                    computed_value: nextCalculationValue.Value,
                    input_value: request.InputValue,
                    previous_value: previousCalculationValue?.Value
                ),
                cancellationToken
            );

            _messagePublisher.Publish("my-queue-worker", nextCalculationValue.Value);

            return new Response(
                ComputedValue: nextCalculationValue.Value,
                InputValue: request.InputValue,
                PreviousValue: previousCalculationValue?.Value
            );
        }
    }
}
