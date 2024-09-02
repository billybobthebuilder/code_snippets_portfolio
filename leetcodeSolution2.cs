/*/

Given an n x n binary matrix image, flip the image horizontally, then invert it, and return the resulting image.

To flip an image horizontally means that each row of the image is reversed.

For example, flipping [1,1,0] horizontally results in [0,1,1].
To invert an image means that each 0 is replaced by 1, and each 1 is replaced by 0.

For example, inverting [0,1,1] results in [1,0,0].

/*/


public class Solution {
    public int[][] FlipAndInvertImage(int[][] image) {
        int le = image.Length;
        for(int i = 0; i < le; i++)
        {   
            int lej = image[i].Length;
            Array.Reverse(image[i]);
            for (int j = 0; j < lej; j++)
            {
                

                if (image[i][j] == 1)
                {
                    image[i][j] = 0;
                    continue;
                }
                image[i][j] = 1;
            }
        }

        return image;


    }



    
}
