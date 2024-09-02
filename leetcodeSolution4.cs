/*/

You are given an array of integers nums. You are also given an integer original which is the first number that needs to be searched for in nums.

You then do the following steps:

If original is found in nums, multiply it by two (i.e., set original = 2 * original).
Otherwise, stop the process.
Repeat this process with the new number as long as you keep finding the number.
Return the final value of original.


/*/



public class Solution {
    public int FindFinalValue(int[] nums, int original) {
        int current = original;
        
        restart:
        foreach (int num in nums)
        {
            if (num == current)
            {
                for (int numTwo = 0; numTwo < nums.Length; numTwo++)
                {
                    if (nums[numTwo] == current * 2)
                    {
                        current = nums[numTwo];
                        goto restart;
                    }
                    if (numTwo == nums.Length - 1)
                    {
                        return current * 2;
                    }

                }
            }
        }
        
        return current;

    }
}
