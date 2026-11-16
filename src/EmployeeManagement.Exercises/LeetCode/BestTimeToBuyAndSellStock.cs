namespace EmployeeManagement.Exercises.LeetCode;

/// <summary>
/// LeetCode 121: Best Time to Buy and Sell Stock (Day 4).
/// Approach: one pass. Track the lowest price so far and the best profit if we sold today.
/// Edge cases: empty or one price (no trade possible), strictly falling prices (profit 0).
/// Time O(n), space O(1).
/// </summary>
public static class BestTimeToBuyAndSellStock
{
    public static int MaxProfit(int[] prices)
    {
        if (prices.Length < 2)
        {
            return 0;
        }

        int lowestPrice = prices[0];
        int bestProfit = 0;

        for (int i = 1; i < prices.Length; i++)
        {
            bestProfit = Math.Max(bestProfit, prices[i] - lowestPrice);
            lowestPrice = Math.Min(lowestPrice, prices[i]);
        }

        return bestProfit;
    }
}
