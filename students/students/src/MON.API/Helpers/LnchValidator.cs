using System;

namespace MON.API.Helpers
{
    public static class LnchValidator
    {
        public static bool Validate(string lnch)
        {
            if (string.IsNullOrWhiteSpace(lnch))
            {
                return false;
            }
            int j, checksum;
            int[] array;
            array = new int[10];

            if (lnch.Length != 10)
            {
                return false;
            }
            for (int i = 0; i < 10; i++)
            {
                array[i] = Convert.ToInt32(lnch[i]) - 48;
                if (array[i] == 10)
                {
                    return false;
                }
            }
            checksum = 0;
            j = 21;
            for (int i = 0; i < 9; i++)
            {
                checksum += array[i] * j;
                j -= 2;
                if ((j % 5) == 0)
                {
                    j -= 2;
                }
            }

            var result = (checksum % 10) == array[9];

            return result;
        }

    }
}
