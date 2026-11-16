namespace EmployeeManagement.Exercises.LeetCode;

/// <summary>
/// LeetCode 1342: Number of Steps to Reduce a Number to Zero (Day 2).
/// Approach: while the number is above zero, halve it when even, otherwise subtract one.
/// Edge cases: 0 needs 0 steps, 1 needs 1 step. Time O(log n), space O(1).
/// </summary>
public static class NumberOfSteps
{
    public static int NumberOfStepsToZero(int num)
    {
        if (num < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(num), "num must not be negative.");
        }

        int steps = 0;

        while (num > 0)
        {
            num = num % 2 == 0 ? num / 2 : num - 1;
            steps++;
        }

        return steps;
    }
}
