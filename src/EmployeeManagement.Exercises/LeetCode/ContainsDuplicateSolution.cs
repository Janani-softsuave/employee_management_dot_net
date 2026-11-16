namespace EmployeeManagement.Exercises.LeetCode;

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
