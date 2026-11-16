namespace EmployeeManagement.Exercises.LeetCode;

/// <summary>
/// LeetCode 242: Valid Anagram (Day 5).
/// Approach: different lengths can never match. Otherwise count letters of s up and letters of t down;
/// they are anagrams when every count ends at zero. Assumes lowercase English letters (the LeetCode constraint).
/// Edge cases: empty strings, different lengths, repeated letters. Time O(n), space O(1) (26 counters).
/// </summary>
public static class ValidAnagram
{
    public static bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length)
        {
            return false;
        }

        int[] counts = new int[26];

        for (int i = 0; i < s.Length; i++)
        {
            counts[s[i] - 'a']++;
            counts[t[i] - 'a']--;
        }

        foreach (int count in counts)
        {
            if (count != 0)
            {
                return false;
            }
        }

        return true;
    }
}
