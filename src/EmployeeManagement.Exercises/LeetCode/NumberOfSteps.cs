namespace EmployeeManagement.Exercises.LeetCode;

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
