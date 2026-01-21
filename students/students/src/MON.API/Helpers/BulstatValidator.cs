using System;

namespace MON.API.Helpers
{
    public static class BulstatValidator
    {
        public static bool Validate(string aBulstat)
        {
            if (string.IsNullOrWhiteSpace(aBulstat)) return false;
            return (((aBulstat.Length == 9) && (CheckBulstat9(aBulstat))) || ((aBulstat.Length == 13) && (CheckBulstat13(aBulstat))));
        }

        public static bool CheckBulstat9(string bul)
        {
            int i, j, checksum;
            checksum = 0;
            for (i = 0; i < 8; i++)
            {
                j = bul[i] - 48;
                if (j == 10)
                {
                    return false;
                }
                checksum += ((i + 1) * j);
            }
            checksum %= 11;
            if (checksum == 10)
            {
                checksum = 0;
                for (i = 0; i < 8; i++)
                {
                    checksum += (i + 3) * (bul[i] - 48);
                }
                checksum = ((checksum % 11) % 10);
            }
            j = bul[8] - 48;
            if (j == 10)
            {
                return false;
            }
            return j == checksum;
        }

        public static bool CheckBulstat13(string bul)
        {
            int i;
            int[] arr = new int[5];

            if (!CheckBulstat9(bul.Substring(0, 9)))
            {
                return false;
            }
            for (i = 8; i < 13; i++)
            {
                arr[i - 8] = Convert.ToInt32(bul[i]) - 48;
            }

            int checksum = 2 * arr[0] + 7 * arr[1] + 3 * arr[2] + 5 * arr[3];
            if ((checksum % 11) == arr[4])
            {
                return true;
            }

            if ((checksum % 11) == 10)
            {
                checksum = 4 * arr[0] + 9 * arr[1] + 5 * arr[2] + 7 * arr[3];
                if (((checksum % 11) % 10) == arr[4])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
