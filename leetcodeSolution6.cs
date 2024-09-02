/*/

Given an integer array nums, return true if any value appears at least twice in the array, and return false if every element is distinct.

/*/



public class Solution {
    public bool ContainsDuplicate(int[] nums) {
        long[] numsLong = Array.ConvertAll(nums, x => (long)x);
        List<long> convertedList = numsLong.ToList();
        const long replacementValue = 9223372036854775807;
        long savedValue = 0;

        for (int i = 0; i < numsLong.Length; i++)
        {
            savedValue = convertedList[i];
            convertedList[i] = replacementValue;
            if (convertedList.Contains(numsLong[i]))
            {
                return true;
            }
            convertedList[i] = savedValue;
        }


        return false;
    }
}
