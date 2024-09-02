/*/
There are n rings and each ring is either red, green, or blue. The rings are distributed across ten rods labeled from 0 to 9.

You are given a string rings of length 2n that describes the n rings that are placed onto the rods. Every two characters in rings forms a color-position pair that is used to describe each ring where:

The first character of the ith pair denotes the ith ring's color ('R', 'G', 'B').
The second character of the ith pair denotes the rod that the ith ring is placed on ('0' to '9').
For example, "R3G2B1" describes n == 3 rings: a red ring placed onto the rod labeled 3, a green ring placed onto the rod labeled 2, and a blue ring placed onto the rod labeled 1.

Return the number of rods that have all three colors of rings on them.

/*/


public class Solution 
{
    public int CountPoints(string rings) 
    {
        HashSet<string> hash = new HashSet<string>();

        for (int i = 0; i < rings.Length; i += 2)
        {
            
            string pair = rings.Substring(i, Math.Min(2, rings.Length - i));
            hash.Add(pair);
            
        }
        
        int[] rod = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        foreach (string s in hash)
        {
            if (s[0] == 'B')
            {
                rod[s[1] - '0'] *= 2;
                continue;
            }
            else if (s[0] == 'R')
            {
                rod[s[1] - '0'] *= 3;
                continue;
            }
            else
            {
                rod[s[1] - '0'] *= 5;
            }





        }
        int count = 0;
        foreach (int v in rod)
        {
            if (v % 30 == 0)
            {
                count++;
            }
        }
        return count;
    }

    HashSet<string> TransformStringToHashSet(string input)
    {
        HashSet<string> hashSet = new HashSet<string>();

        for (int i = 0; i < input.Length; i += 2)
        {
            
            string pair = input.Substring(i, Math.Min(2, input.Length - i));
            hashSet.Add(pair);
            
        }
        

        return hashSet;
    }

}
