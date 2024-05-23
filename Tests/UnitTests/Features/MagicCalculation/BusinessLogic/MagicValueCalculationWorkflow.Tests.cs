﻿using Core.Features.MagicCalculation.BusinessLogic;
using Core.Features.MagicCalculation.Domain;
using FluentAssertions;
using Infrastructure.KeyValueStore;
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
        var workflow = new MagicValueCalculationWorkflow(clock, store);

        await workflow.ExecuteMagicValueCalculationAsync(1, 1);

        var storedOutputValueWithDefaultOutputValue = await store.TryGetValueAsync(1);

        storedOutputValueWithDefaultOutputValue!.Value.Should().Be(2);

        await workflow.ExecuteMagicValueCalculationAsync(1, 1);

        var storedOutputValueAfterFirstCalculation = await store.TryGetValueAsync(1);

        var outputValueAfterFirstCalculation = MagicCalculator.CalculateOutputValue(
            1,
            storedOutputValueWithDefaultOutputValue!.Value
        );

        storedOutputValueAfterFirstCalculation!.Value.Should().Be(outputValueAfterFirstCalculation);

        clock.Advance(Duration.FromSeconds(16));

        await workflow.ExecuteMagicValueCalculationAsync(1, 1);

        var storedOutputValueAfterExpiration = await store.TryGetValueAsync(1);

        storedOutputValueAfterExpiration!.Value.Should().Be(2);
    }
}