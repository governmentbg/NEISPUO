using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MON.Shared.Extensions.Utils
{
    public class Helper
    {
        public static bool ValidEGN(String sEgn)
        {
            int dd, mm, gg = 0;
            Boolean Result;

            if (sEgn == null)
            {
                return false;
            }

            Result = false;
            if (sEgn.Length != 10)
            {
                return false;
            }

            try
            {
                gg = Convert.ToInt32(sEgn.Substring(0, 2));
                mm = Convert.ToInt32(sEgn.Substring(2, 2));
                dd = Convert.ToInt32(sEgn.Substring(4, 2));
            }
            catch (System.Exception) { return false; }
            if ((gg == -1) || (mm == -1) || (dd == -1))
            {
                return false;
            }
            if (!DateIsValid(dd, mm, gg))
            {
                return false;
            }
            if (!CheckEGNChecksum(sEgn))
            {
                return false;
            }
            Result = true;
            return Result;
        }

        public static bool ValidEnch(string sEnch)
        {
            int j, checksum;
            int[] array;
            array = new int[10];

            if (sEnch.Length != 10)
            {
                return false;
            }
            for (int i = 0; i < 10; i++)
            {
                array[i] = Convert.ToInt32(sEnch[i]) - 48;
                if (array[i] == 10)
                {
                    return false;
                }
            }
            checksum = 0;
            j = 21;
            for (int i = 0; i < 9; i++)
            {
                checksum = checksum + array[i] * j;
                j = j - 2;
                if ((j % 5) == 0)
                {
                    j = j - 2;
                }
            }
            return ((checksum % 10) == array[9]);
        }


        public static bool CheckEGNChecksum(String egn)
        {
            try
            {
                int i, j, koef, checksum;
                Boolean Result;
                koef = 1;
                checksum = 0;
                for (i = 0; i <= 8; i++)
                {
                    koef = ((2 * koef) % 11);
                    j = Convert.ToInt32(egn.Substring(i, 1));
                    if (j == -1)
                    {
                        Result = false;
                        return false;
                    }
                    checksum = checksum + koef * j;
                };

                j = Convert.ToInt32(egn.Substring(9, 1));
                if (j == -1)
                {
                    Result = false;
                    return false;
                };

                Result = ((checksum % 11) % 10) == j;
                return Result;
            }
            catch
            {
                return false;
            }
        }

        public static bool DateIsValid(int dd, int mm, int gg)
        {
            if ((mm >= 21) && (mm <= 32)) { mm = mm - 20; gg = gg + 1800; }
            if ((mm >= 41) && (mm <= 52)) { mm = mm - 40; gg = gg + 2000; }
            else
                gg = 1900 + gg;

            try
            {
                DateTime dt = new DateTime(gg, mm, dd);
            }
            catch (System.Exception) { return false; }
            return true;

        }

        public static DateTime EgnToBirthDate(String egn)
        {
            const int month_offset_19th_century = 20;
            const int month_offset_20th_century = 0;
            const int month_offset_21st_century = 40;
            int century, month_offset;
            int year, month, day, i;

            if (egn.Length < 6) return DateTime.MinValue;

            for (i = 0; i <= 5; i++)
                if (!(egn.Substring(i, 1) == "0") && (egn.Substring(i, 1) == "9") && (egn.Substring(i, 1) == "1") && (egn.Substring(i, 1) == "2") && (egn.Substring(i, 1) == "3")
                && (egn.Substring(i, 1) == "4") && (egn.Substring(i, 1) == "5") && (egn.Substring(i, 1) == "6") && (egn.Substring(i, 1) == "7") && (egn.Substring(i, 1) == "8"))
                {
                    return DateTime.MinValue;
                }

            year = Convert.ToInt32(egn.Substring(0, 2));
            month = Convert.ToInt32(egn.Substring(2, 2));
            day = Convert.ToInt32(egn.Substring(4, 2));

            if ((month >= month_offset_20th_century + 1) && (month <= month_offset_20th_century + 12))
            {
                century = 20;
                month_offset = month_offset_20th_century;
            }
            else if ((month >= month_offset_19th_century + 1) && (month <= month_offset_19th_century + 12))
            {
                century = 19;
                month_offset = month_offset_19th_century;
            }
            else if ((month >= month_offset_21st_century + 1) && (month <= month_offset_21st_century + 12))
            {
                century = 21;
                month_offset = month_offset_21st_century;
            }
            else
            {
                return DateTime.MinValue;
            }

            month = month - month_offset;
            year = (century - 1) * 100 + year;

            try
            {
                DateTime dt = new DateTime(year, month, day);
                return dt;
            }
            catch (System.Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static int EgnToSexType(string egn)
        {
            if ((egn.Length < 9) || ((egn.Substring(8, 1) != "0") && (egn.Substring(8, 1) != "9") && (egn.Substring(8, 1) != "1") && (egn.Substring(8, 1) != "2") && (egn.Substring(8, 1) != "3")
                && (egn.Substring(8, 1) != "4") && (egn.Substring(8, 1) != "5") && (egn.Substring(8, 1) != "6") && (egn.Substring(8, 1) != "7") && (egn.Substring(8, 1) != "8")))
                return 0;
            else if (Convert.ToInt32(egn.Substring(8, 1)) % 2 == 0) return 1;
            else return 2;
        }

        public static int EgnToAgeInYears(string egn, DateTime currentDate)
        {
            DateTime birthDate = EgnToBirthDate(egn);

            TimeSpan toSpan = currentDate - new DateTime(currentDate.Year, 1, 1);
            TimeSpan fromSpan = birthDate - new DateTime(birthDate.Year, 1, 1);

            if (toSpan >= fromSpan)
            {
                return currentDate.Year - birthDate.Year;
            }
            else
            {
                return currentDate.Year - birthDate.Year - 1;
            }
        }

        public static int BirthDateToAgeInYears(DateTime birthDate, DateTime currentDate)
        {
            try
            {
                TimeSpan ts = currentDate.Subtract(birthDate);
                int ageInYears = Convert.ToInt32(ts.Days / 365.25);
                return ageInYears;
            }
            catch
            {
                return -1;
            }
        }

        public static string EndsWithSlash(string aPath)
        {
            if (aPath.Length == 0) return "\\";
            else if ((aPath.Substring(aPath.Length - 1, 1) != "\\") && (aPath.Substring(aPath.Length - 1, 1) != "/"))
            {
                return aPath + "\\";
            }
            else return aPath;
        }

        public static bool ValidBULSTAT(string aBulstat)
        {
            return (((aBulstat.Length == 9) && (checkBulstat9(aBulstat))) || ((aBulstat.Length == 13) && (checkBulstat13(aBulstat))));
        }

        public static bool checkBulstat9(string bul)
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
            checksum = checksum % 11;
            if (checksum == 10)
            {
                checksum = 0;
                for (i = 0; i < 8; i++)
                {
                    checksum += (i + 3) * ((int)bul[i] - 48);
                }
                checksum = ((checksum % 11) % 10);
            }
            j = (int)bul[8] - 48;
            if (j == 10)
            {
                return false;
            }
            return j == checksum;
        }


        public static bool checkBulstat13(string bul)
        {
            int i, checksum = 0;
            int[] arr = new int[5];

            if (!checkBulstat9(bul.Substring(0, 9)))
            {
                return false;
            }
            for (i = 8; i < 13; i++)
            {
                arr[i - 8] = Convert.ToInt32(bul[i]) - 48;
            }

            checksum = 2 * arr[0] + 7 * arr[1] + 3 * arr[2] + 5 * arr[3];
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

        ////  Sign the message by the using the private key of the signer.
        ////  Note that signer's public key certificate is input here 
        ////  because it is used to locate the corresponding private key.
        //public static byte[] SignMsg(
        //   Byte[] msg,
        //   X509Certificate2 signerCert)
        //{
        //    //  Place message in a ContentInfo object.
        //    //  This is required to build a SignedCms object.
        //    ContentInfo contentInfo = new ContentInfo(msg);

        //    //  Instantiate SignedCms object with the ContentInfo above.
        //    //  Has default SubjectIdentifierType IssuerAndSerialNumber.
        //    //  Has default Detached property value false, so message is
        //    //  included in the encoded SignedCms.
        //    SignedCms signedCms = new SignedCms(contentInfo, true);

        //    //  Formulate a CmsSigner object, which has all the needed
        //    //  characteristics of the signer.
        //    CmsSigner cmsSigner = new CmsSigner(signerCert);

        //    //  Sign the PKCS #7 message.
        //    Console.Write("Computing signature with signer subject " +
        //        "name {0} ... ", signerCert.SubjectName.Name);
        //    signedCms.ComputeSignature(cmsSigner, false);
        //    Console.WriteLine("Done.");

        //    //  Encode the PKCS #7 message.
        //    return signedCms.Encode();
        //}

    }
}
