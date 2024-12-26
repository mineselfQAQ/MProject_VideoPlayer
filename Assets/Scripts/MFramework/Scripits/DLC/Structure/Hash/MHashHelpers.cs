using System;

namespace MFramework.DLC
{
    public static class MHashHelpers
    {
        public static readonly int[] primes = new int[72]
        {
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
        89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
        631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
        4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
        25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
        156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
        968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
        5999471, 7199369
        };

        public static bool IsPrime(int candidate)
        {
            if (((uint)candidate & (true ? 1u : 0u)) != 0)
            {
                int num = (int)Math.Sqrt(candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if (candidate % i == 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return candidate == 2;
        }

        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new Exception();
            }

            //获取比min大的质数
            for (int i = 0; i < primes.Length; i++)
            {
                int num = primes[i];
                if (num >= min)
                {
                    return num;
                }
            }

            //当min过大(超过7199369)时，计算质数的方法
            //min|1的含义---如果为奇数不变，如果为偶数+1，这是因为偶数永远不是质数
            for (int j = min | 1; j < int.MaxValue; j += 2)
            {
                if (IsPrime(j) && (j - 1) % 101 != 0)
                {
                    return j;
                }
            }

            return min;
        }

        public static int ExpandPrime(int oldSize)
        {
            //扩容方法---翻倍，再取"最小质数"
            int num = 2 * oldSize;
            //数比int最大值大(转uint可以存放，但是int就会overflow)
            if ((uint)num > 2146435069u && 2146435069 > oldSize)
            {
                return 2146435069;
            }

            return GetPrime(num);
        }
    }
}
