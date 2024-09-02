/*/


Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.

You may assume that each input would have exactly one solution, and you may not use the same element twice.

You can return the answer in any order.




/*/



public class Solution {
    public int[] TwoSum(int[] nums, int target) {
        
        int[] copyNums = nums;
        int i;
        int o;
        
        for (i = 0; i < nums.Length; i++)
        {
            for (o = 0; o < copyNums.Length; o++)
            {

                if (o != i)
                {
                    if (nums[i] + copyNums[o] == target)
                    {
                        return new int[] {i, o};
                    }
                    
                }

            }

        }
        throw new ArgumentException("null");

    }
}
