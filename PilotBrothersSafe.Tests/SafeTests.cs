using PilotBrothersSafe.SafeLogic;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PilotBrothersSafe.Tests
{
    public class SafeTests
    {
        private readonly ISafeLogic _safeLogic;

        public SafeTests(ISafeLogic safeLogic)
        {
            _safeLogic = safeLogic;
        }

        public static IEnumerable<object[]> Dimensions =>
            new List<object[]>
            {
                new object[] { 3 },
                new object[] { 4 },
                new object[] { 5 },
                new object[] { 6 },
                new object[] { 10 }
            };

        [Theory]
        [MemberData(nameof(Dimensions))]
        public async Task EqualMatricesTest(int n)
        {
            var matrix = await _safeLogic.CreateSafe(n);
            var cloneMatrix = (bool[,])matrix.Clone();

            Assert.True(matrix.SequenceEquals(cloneMatrix));
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(11)]
        public async Task IsNotPossibleToSolveSafeTest(int n)
        {
            var matrix = await _safeLogic.CreateSafe(n);
            matrix[0, 0] = !matrix[0, 0];

            Assert.False(await _safeLogic.IsPossibleToSolveSafe(matrix));
        }

        [Theory]
        [MemberData(nameof(Dimensions))]
        public async Task IsPossibleToSolveSafeTest(int n)
        {
            var matrix = await _safeLogic.CreateSafe(n);
            var randomRotatingMatrix = await _safeLogic.RandomRotating(matrix);

            Assert.True(await _safeLogic.IsPossibleToSolveSafe(randomRotatingMatrix));
        }

        [Theory]
        [MemberData(nameof(Dimensions))]
        public async Task NotEqualMatricesTest(int n)
        {
            var matrix = await _safeLogic.CreateSafe(n);
            var cloneMatrix = (bool[,])matrix.Clone();
            cloneMatrix[0, 0] = !cloneMatrix[0, 0];

            Assert.False(matrix.SequenceEquals(cloneMatrix));
        }

        [Theory]
        [MemberData(nameof(Dimensions))]
        public async Task RandomRotatingTest(int n)
        {
            var matrix = await _safeLogic.CreateSafe(n);
            var randomRotatingMatrix = await _safeLogic.RandomRotating(matrix);

            Assert.False(matrix.SequenceEquals(randomRotatingMatrix));
        }
    }
}