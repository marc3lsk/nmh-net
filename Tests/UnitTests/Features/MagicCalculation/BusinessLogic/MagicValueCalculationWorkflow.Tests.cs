﻿using Abstraction.MessageBus;
using Core.Features.MagicCalculation.BusinessLogic;
using Core.Features.MagicCalculation.Domain;
using FluentAssertions;
using Infrastructure.KeyValueStore;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using NodaTime.Testing;

namespace UnitTests.Features.MagicCalculation.BusinessLogic;

public class MagicValueCalculationWorkflowTests
{
    [Fact]
    public async Task It_Works()
    {
        var clock = FakeClock.FromUtc(0, 01, 01, 00, 00, 00);
        var store = new KeyValueStoreInMemory<int, CalculationValue>();
        var bus = new Mock<IBus>();
        var messagePublisher = new Mock<IMessagePublisher>();
        var logger = new Mock<ILogger<MagicValueCalculationWorkflow>>();

        var workflow = new MagicValueCalculationWorkflow.RequestHandler(
            clock,
            store,
            bus.Object,
            messagePublisher.Object,
            logger.Object
        );

        await workflow.Handle(
            new MagicValueCalculationWorkflow.Request(Key: 1, InputValue: 1),
            default
        );

        var storedOutputValueWithDefaultOutputValue = await store.TryGetValueAsync(1);

        storedOutputValueWithDefaultOutputValue!.Value.Should().Be(2);

        await workflow.Handle(
            new MagicValueCalculationWorkflow.Request(Key: 1, InputValue: 1),
            default
        );

        var storedOutputValueAfterFirstCalculation = await store.TryGetValueAsync(1);

        var outputValueAfterFirstCalculation = MagicCalculator.CalculateOutputValue(
            1,
            storedOutputValueWithDefaultOutputValue!.Value
        );

        storedOutputValueAfterFirstCalculation!.Value.Should().Be(outputValueAfterFirstCalculation);

        clock.Advance(Duration.FromSeconds(16));

        await workflow.Handle(
            new MagicValueCalculationWorkflow.Request(Key: 1, InputValue: 1),
            default
        );

        var storedOutputValueAfterExpiration = await store.TryGetValueAsync(1);

        storedOutputValueAfterExpiration!.Value.Should().Be(2);
    }
}
