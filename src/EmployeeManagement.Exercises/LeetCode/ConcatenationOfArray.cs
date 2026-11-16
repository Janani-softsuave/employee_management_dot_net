namespace EmployeeManagement.Exercises.LeetCode;

public static class ConcatenationOfArray
{
    public static int[] GetConcatenation(int[] nums)
    {
        int n = nums.Length;
        int[] result = new int[2 * n];

        for (int i = 0; i < n; i++)
        {
            result[i] = nums[i];
            result[i + n] = nums[i];
        }

        return result;
    }
}
