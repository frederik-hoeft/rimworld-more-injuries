using MoreInjuries.HealthConditions.Hemodilution;
using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.Tests.HealthConditions.Hemodilution;

[TestClass]
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Unit tests must be discoverable by the test runner.")]
public sealed class HemodilutionEvaluatorTests
{
    [TestMethod]
    public void CalculateMaximumSafeSalineTransfusionsTest()
    {
        Assert.AreEqual(0, HemodilutionEvaluator.CalculateMaximumSafeSalineTransfusions(
            hemodilution: 0.38f, 
            bloodLoss: 0.7f, 
            hemodilutionThreshold: 0.449f, 
            salineBagVolume: 0.15f));

        Assert.AreEqual(1, HemodilutionEvaluator.CalculateMaximumSafeSalineTransfusions(
            hemodilution: 0.22f,
            bloodLoss: 0.79f,
            hemodilutionThreshold: 0.449f,
            salineBagVolume: 0.15f));
    }
}