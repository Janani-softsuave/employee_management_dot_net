using EmployeeManagement.Exercises.LeetCode;

namespace EmployeeManagement.Tests.Week1;

public class LeetCodeTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 1 }, new[] { 1, 2, 1, 1, 2, 1 })]
    [InlineData(new[] { 1, 3, 2, 1 }, new[] { 1, 3, 2, 1, 1, 3, 2, 1 })]
    [InlineData(new[] { 7 }, new[] { 7, 7 })]
    [InlineData(new int[0], new int[0])]
    public void GetConcatenation_RepeatsTheArrayTwice(int[] input, int[] expected)
    {
        Assert.Equal(expected, ConcatenationOfArray.GetConcatenation(input));
    }

    [Theory]
    [InlineData(14, 6)]
    [InlineData(8, 4)]
    [InlineData(123, 12)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void NumberOfStepsToZero_CountsHalvingAndSubtracting(int num, int expected)
    {
        Assert.Equal(expected, NumberOfSteps.NumberOfStepsToZero(num));
    }

    [Fact]
    public void NumberOfStepsToZero_NegativeNumber_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NumberOfSteps.NumberOfStepsToZero(-1));
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 1 }, true)]
    [InlineData(new[] { 1, 2, 3, 4 }, false)]
    [InlineData(new[] { 1, 1, 1, 3, 3, 4, 3, 2, 4, 2 }, true)]
    [InlineData(new[] { 5 }, false)]
    [InlineData(new int[0], false)]
    public void ContainsDuplicate_DetectsRepeatedValues(int[] nums, bool expected)
    {
        Assert.Equal(expected, ContainsDuplicateSolution.ContainsDuplicate(nums));
    }

    [Theory]
    [InlineData(new[] { 7, 1, 5, 3, 6, 4 }, 5)]
    [InlineData(new[] { 7, 6, 4, 3, 1 }, 0)]
    [InlineData(new[] { 2, 4, 1 }, 2)]
    [InlineData(new[] { 3 }, 0)]
    [InlineData(new int[0], 0)]
    public void MaxProfit_ReturnsBestSingleTrade(int[] prices, int expected)
    {
        Assert.Equal(expected, BestTimeToBuyAndSellStock.MaxProfit(prices));
    }

    [Theory]
    [InlineData("anagram", "nagaram", true)]
    [InlineData("rat", "car", false)]
    [InlineData("a", "ab", false)]
    [InlineData("aab", "abb", false)]
    [InlineData("", "", true)]
    public void IsAnagram_ComparesLetterCounts(string s, string t, bool expected)
    {
        Assert.Equal(expected, ValidAnagram.IsAnagram(s, t));
    }
}
