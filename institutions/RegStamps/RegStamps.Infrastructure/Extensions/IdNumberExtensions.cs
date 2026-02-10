namespace RegStamps.Infrastructure.Extensions
{
    public static class IdNumberExtensions
    {
        public static bool IsValidPersonalIdNumber(this string personalIdNumber)
        {
            if (personalIdNumber.Length != 10)
            {
                return false;
            }

            foreach (char digit in personalIdNumber)
            {
                if (!char.IsDigit(digit))
                {
                    return false;
                }
            }

            int month = int.Parse(personalIdNumber.Substring(2, 2));
            int year = 0;

            if (month < 13)
            {
                year = int.Parse("19" + personalIdNumber.Substring(0, 2));
            }
            else if (month < 33)
            {
                month -= 20;
                year = int.Parse("18" + personalIdNumber.Substring(0, 2));
            }
            else
            {
                month -= 40;
                year = int.Parse("20" + personalIdNumber.Substring(0, 2));
            }

            int day = int.Parse(personalIdNumber.Substring(4, 2));
            DateTime dateOfBirth = new DateTime();
            if (!DateTime.TryParse(string.Format("{0}/{1}/{2}", day, month, year), out dateOfBirth))
            {
                return false;
            }

            int[] weights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int totalControlSum = 0;

            for (int i = 0; i < 9; i++)
            {
                totalControlSum += weights[i] * (personalIdNumber[i] - '0');
            }

            int controlDigit = 0;
            int reminder = totalControlSum % 11;

            if (reminder < 10)
            {
                controlDigit = reminder;
            }

            int lastDigitFromIDNumber = int.Parse(personalIdNumber.Substring(9));

            if (lastDigitFromIDNumber != controlDigit)
            {
                return false;
            }

            return true;
        }

        public static bool IsValidForeignIdNumber(this string personalIdNumber)
        {
            if (personalIdNumber.Length != 10)
            {
                return false;
            }
            foreach (char digit in personalIdNumber)
            {
                if (!Char.IsDigit(digit))
                {
                    return false;
                }
            }

            int[] weights = new int[] { 21, 19, 17, 13, 11, 9, 7, 3, 1 };
            int totalControlSum = 0;

            for (int i = 0; i < 9; i++)
            {
                totalControlSum += weights[i] * (personalIdNumber[i] - '0');
            }

            int controlDigit = 0;
            int reminder = totalControlSum % 10;

            if (reminder < 10)
            {
                controlDigit = reminder;
            }

            int lastDigitFromIDNumber = int.Parse(personalIdNumber.Substring(9));
            if (lastDigitFromIDNumber != controlDigit)
            {
                return false;
            }

            return true;
        }
    }
}
