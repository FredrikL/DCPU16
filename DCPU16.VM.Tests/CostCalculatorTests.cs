using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class CostCalculatorTests
    {
        private CostCalculator costCalculator;

        [SetUp]
        public void Setup()
        {
            this.costCalculator = new CostCalculator();
        }

        [Test]
        public void SimpleSetShouldCost1()
        {
            Instruction ins = new Instruction(){a = 0, b=0, instruction = 0x01};

            var cost = this.costCalculator.CalculateCost(ins);

            Assert.That(cost, Is.EqualTo(1));
        }
    }
}