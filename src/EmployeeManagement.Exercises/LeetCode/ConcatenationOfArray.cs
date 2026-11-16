namespace EmployeeManagement.Exercises.LeetCode;

/// <summary>
/// LeetCode 1929: Concatenation of Array (Day 1).
/// Approach: allocate 2n slots and copy each element to index i and i + n.
/// Edge cases: single element, repeated values. Time O(n), space O(n) for the result.
/// </summary>
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
