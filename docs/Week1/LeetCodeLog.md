# LeetCode log (Week 1)

Solutions: `src/EmployeeManagement.Exercises/LeetCode/` — tests: `tests/EmployeeManagement.Tests/Week1/LeetCodeTests.cs`.

Rules followed: write the approach and edge cases first, state complexity, test at least two edge cases, refactor for readability.

| Day | Problem | Approach | Edge cases tested | Time | Space | Lesson learned |
|-----|---------|----------|-------------------|------|-------|----------------|
| 1 | 1929 Concatenation of Array | Allocate `2n` slots; copy `nums[i]` to `i` and `i + n` | single element, empty array | O(n) | O(n) | Size the result array up front instead of growing a `List`. |
| 2 | 1342 Number of Steps to Reduce a Number to Zero | Loop: halve if even, else subtract 1, count steps | `0` (0 steps), `1` (1 step), negative input throws | O(log n) | O(1) | Halving dominates, so steps grow with the number of bits, not the value. |
| 3 | 217 Contains Duplicate | One pass with a `HashSet`; `Add` returns false when the value was already seen | empty array, one element, duplicates far apart | O(n) | O(n) | Sorting first is O(n log n) and mutates the input; a `HashSet` trades memory for speed. |
| 4 | 121 Best Time to Buy and Sell Stock | Track lowest price so far and best profit if selling today | falling prices (profit 0), one price, empty | O(n) | O(1) | Update the profit before updating the minimum so you never sell before you buy. |
| 5 | 242 Valid Anagram | Different lengths fail fast; otherwise `+1` for `s` letters and `-1` for `t` letters in `int[26]`, all must be 0 | different lengths, empty strings, same letters with different counts (`aab`/`abb`) | O(n) | O(1) | A fixed 26-slot array beats a `Dictionary`, but it assumes lowercase letters. Unicode input would need a `Dictionary<char,int>`. |

## Approach notes written before coding

1. **Concatenation of Array** — result is the input twice. Edge cases: empty input should give an empty array.
2. **Number of Steps** — even: divide by 2; odd: subtract 1. Edge cases: 0 and 1.
3. **Contains Duplicate** — need a fast "have I seen this?" check, so a set. Edge cases: empty, single element.
4. **Best Time to Buy and Sell Stock** — buy day must precede sell day. Edge cases: always falling prices means do not trade (0).
5. **Valid Anagram** — same multiset of letters. Edge cases: unequal lengths, empty strings.
