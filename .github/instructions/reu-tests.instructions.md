---
description: "Use when writing unit tests, integration tests, or test fixtures in REU.Tests. Covers testing patterns and best practices."
applyTo: "tests/**/*.cs"
---

# REU.Tests Guidelines

REU.Tests contains unit and integration tests for all layers. Tests should be isolated, deterministic, and focus on core logic without IO.

## Core Principles

1. **Unit tests are fast and isolated**
   - One logical assertion per test
   - Mock external dependencies
   - No file I/O, database, or network calls
   - Run in milliseconds

2. **Test data is small and realistic**
   - Use csv samples in `data/samples/` or inline objects for small models
   - Avoid huge datasets that slow down tests
   - Mock `MarketContext` with realistic values
   - Avoid huge datasets in unit tests

3. **Deterministic and repeatable**
   - Same test run → same result every time
   - No randomness without explicit seeding
   - Seed any `Random` instances for reproducibility

4. **Clear test names**
   - Use pattern: `[SystemUnderTest]_[Scenario]_[ExpectedOutcome]`
   - Example: `MovingAverageStrategy_BuySignal_WhenFastAboveSlow`
   - Tests double as documentation

5. **Arrange-Act-Assert structure**
   - Arrange: Set up dependencies and input data
   - Act: Call the method under test
   - Assert: Verify output and state

6. **Separate tests for separate concerns**
  - public void GetResult_ReturnsCorrectCount() { /* ... */ }
  - public void GetResult_FirstItemHasPositiveValue() { /* ... */ }
  - public void GetResult_TimestampsArePresent() { /* ... */ }


## Project Structure

```
tests/
├── REU.Tests/
│   ├── Contracts/
│   │   └── MarketContextTests.cs
│   ├── Modules/
│   │   └── Strategies/
│   │       └── MovingAverageStrategyTests.cs
│   ├── Pipeline/
│   │   ├── Loaders/
│   │   │   └── CSVDataLoaderTests.cs
│   │   └── Fusers/
│   │       └── FuserTests.cs
│   ├── Simulator/
│   │   ├── SimulationEngineTests.cs
│   │   └── Broker/
│   │       └── SimpleBrokerTests.cs
│   └── Fixtures/
│       ├── MarketDataFixture.cs
│       └── MockBroker.cs
```

## Testing Strategy by Layer

| Layer         | Focus                           | Mocking                |
|---------------|---------------------------------|------------------------|
| **Contracts** | Serialization, shape validation | Minimal                |
| **Modules**   | Business logic determinism      | Mock `MarketContext`   |
| **Pipeline**  | Data alignment, transformation  | Use sample data files  |
| **Simulator** | Portfolio tracking, fills       | Mock broker/strategies |
| **App**       | Configuration, wiring           | Mock all layers        |

## Best Practices

1. **Use `[SetUp]` for common initialization**, `[TearDown]` for cleanup
2. **Parameterize tests** with `[TestCase]` for edge cases
3. **Test failures as clearly as unit successes**
4. **Sample data**: Keep realistic but small (`data/samples/`)
5. **Avoid test interdependence**: Each test should run standalone
6. **One logical assertion per test** (multiple related assertions ok)
