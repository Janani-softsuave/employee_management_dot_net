namespace EmployeeManagement.Exercises.LeetCode;

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
