using System.Collections;

public static class Utiliy
{
    /// <summary>
    /// 셔플 알고리즘 'the fisher-Yates'
    /// </summary>
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        // -1 하는 이유는 'the fisher-Yates' 알고리즘에 마지막 인덱스는 생략해도 되기 때문
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
