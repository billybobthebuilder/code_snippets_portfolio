/*/

You are playing a game with integers. You start with the integer 1 and you want to reach the integer target.

In one move, you can either:

Increment the current integer by one (i.e., x = x + 1).
Double the current integer (i.e., x = 2 * x).
You can use the increment operation any number of times, however, you can only use the double operation at most maxDoubles times.

Given the two integers target and maxDoubles, return the minimum number of moves needed to reach target starting with 1.

 


/*/



public class Solution {
    public int MinMoves(int target, int maxDoubles) {
        int numberOfMoves = 0;
        int numberOfDoublesMoves = 0;
        int targCopy = target;
        if (maxDoubles > 0)
        {
            while (targCopy > 1)
            {
                targCopy = ReturnDividedOfTwo(targCopy, numberOfMoves, out numberOfMoves, numberOfDoublesMoves, out numberOfDoublesMoves, maxDoubles);
                
            }
            return numberOfMoves;

        }
        return targCopy - 1;
    }


    public int ReturnDividedOfTwo(int number, int movesOrigin, out int moves, int doublesMovesOrigin, out int doublesMoves, int maximumDoubles)
    {
        moves = movesOrigin;
        doublesMoves = doublesMovesOrigin;
        moves++;
        if (number % 2 == 0 && doublesMoves < maximumDoubles)
        {
            doublesMoves++;
            return number / 2;
        }
        
        return number - 1;
    }
}
