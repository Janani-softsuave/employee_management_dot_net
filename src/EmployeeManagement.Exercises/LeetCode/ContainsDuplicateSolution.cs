namespace EmployeeManagement.Exercises.LeetCode;

/// <summary>
/// LeetCode 217: Contains Duplicate (Day 3).
/// Approach: walk the array once, remembering seen values in a HashSet. HashSet.Add returns false
/// when the value is already present, which means a duplicate.
/// Edge cases: empty array, single element, all equal. Time O(n), space O(n).
/// </summary>
public static class ContainsDuplicateSolution
{
    public static bool ContainsDuplicate(int[] nums)
    {
        var seen = new HashSet<int>();

        foreach (int value in nums)
        {
            if (!seen.Add(value))
            {
                return true;
            }
        }

        return false;
    }
}
