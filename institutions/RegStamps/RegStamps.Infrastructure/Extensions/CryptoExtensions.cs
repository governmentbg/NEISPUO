using System.Security.Cryptography;
using System.Text;

namespace RegStamps.Infrastructure.Extensions
{
    public static class CryptoExtensions
    {
        private const string password = @"g3b5PIvf5hjhOYBWLvFQ5vMsZL5tmXuJ62RKguVqyHDNFNkVtv9rOXompjjd4yKwcXJF22h9MSiUEcR4kUEMfeiYjDh1iMzPAKnCXkQ4IZVkDay7qN5e7MVKZb48IXzk9t8BR
                                          rmu1Qt5B6Kl3JVs3GmG7052OT4pJuW8Cfo2IBZdgFEi2dqBxsFLVrtY19q1HCwt2N6J3QcMcUBMfQQAMOu37pfcs4ewzmfOvMdMRHFJ9AMA5STMa52axkytj9FWzPmQGY1lmh
                                          I53cifsqSeUhcQ9LBt6ONLXvqmGpWD6QhVXeQLDUmeIPd16bzpUuAXTk5GNiRJcWKJDrfanK0u2jddbfHlUFnEmf8rpQGbW47gLgsXLxAtBkDkZjipD158eld79bz7CjWaluK
                                          kAwhfKPUh9NcsvSutoz1Abr9M7tfZ0NaYcxA1qtW1uKa8jfGUR6E6fIh2Vv0Vu2MXhtR9zfZbCNlCCnYk9ec8hU5YuB85RKwsawAYaB1ldlVioKcKh8UfYxLNYXltbQYudCSK
                                          3VyEKKG55ktnbwl8p0Q2PVSnKPhk4xrY6pxQCANWgxqoTTN5RKFqOxrHhcoYj4wQAG3buqfoUPxSLd10FkAm0XR6lunHmIq9MVH7u9xh2WXmkELnmAvVwvg3OdLf2ifW55cnW
                                          LBusFBX2UBLZKxPderusI2UaUYbNqF2UN8smKWFBeWZwPRdupgTCDZO4LBHbDGdDETbXYILTnK9On3aFYuqQtY2rlaUho9j1DfRtLsWRgA187F1hvUwBSZTkoJ8XLTmDFCawm
                                          aSF9QyYEcPqcsgLmmAkBbQqVFVnE2LNBHjnxozzlaUT6PpUuMur3CemxYopdiGmBkj0ZIiFwueZXNdhBPavnNjWoxEeS2vYwPlGvJ86AxdDf3jtCoDSZwS0iPEhS6xa2n4uCn
                                          E43GBBwyirLb3wnEPc0s3I8CUAhwzwcbwDea7YC2Vo6AOco9fqt0vve0Eoo6c2i0njOMTPPFlTOpTVOdp9x5pdJNWw4Q2kBMsryn1PyPOPfRKtIm07VE6GT4cvGTFnhkXKtgx
                                          GZvRTLnG26E8O7McpfsYCSIGpDxo4bgosPtPHfepgeoNtqzCweODrP7NipnhE3Vqqpy6M4VqhLF85ryzq8HKQRLbGW8Js8cDr6ku8Dwu3RO1bIoBf3S39zOu3dGTZG30dkEAs
                                          ey6cKnNsiiJkWH3DY6wo8O9YlQDWIkBqP5NaTx9Utwlvv88FogduPhAJB4fxDqpMsT7rLDk4vSadRFCP2ukwGEANCbD9vWiT7c7JrTA97Sk4ayTlB85XnehmK4HOGYClJhqn3
                                          q2OLhX4tHwcHSpePN80njSi0t8ewX7HYdxQsdjzkcLPQYC8YT0m5k0ouHxHemqlUJiHtGgIuGb1rjX1ZaOqMdnNEldiS78VD7U4RiLAHNEO4qpa3S5lUEOdhdw04B9Q86VnwP
                                          Mc4X5btUxwftylskFsUNnFyStw69jZmWrLgqm1aWZwEvZy1Xa2hePXfWo2FgUiIROFcIU1PicfrwwfyUmY0KmEX91oVAuWA21oL8zCrQwv6gIhnhVWm7ViVkVYradaeRdbvt3";

        public static string ToEncrypt(this string text)
        {
            if (text == null || text.Length <= 0)
            {
                throw new ArgumentNullException("No data found");
            }

            StringBuilder sb = new StringBuilder();

            byte[] salt = GenerateRandomSalt();

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CFB;

            sb.Append(Convert.ToBase64String(salt.ToArray()));
            sb.Append("|");

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] textBytes = Encoding.UTF8.GetBytes(text);
                        cryptoStream.Write(textBytes, 0, textBytes.Length);
                    }

                    sb.Append(Convert.ToBase64String(ms.ToArray()));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                string innerExcMessageFirstLine = string.Empty;
                if (ex.InnerException != null)
                {
                    innerExcMessageFirstLine = ex.InnerException.InnerException.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).First();
                }

                string[] message = ex.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                throw new Exception(string.Format($"Грешка при обработка: {string.Join(" ", message)} {innerExcMessageFirstLine}"));
            }

        }

        public static string ToDecrypt(this string text)
        {
            StringBuilder sb = new StringBuilder();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            string[] tokens = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] saltBytes = Convert.FromBase64String(tokens[0]);
            byte[] textBytes = Convert.FromBase64String(tokens[1]);

            try
            {
                using (MemoryStream ms = new MemoryStream(saltBytes))
                {
                    ms.Read(salt, 0, salt.Length);
                }
            }
            catch (Exception ex)
            {
                string innerExcMessageFirstLine = string.Empty;
                if (ex.InnerException != null)
                {
                    innerExcMessageFirstLine = ex.InnerException.InnerException.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).First();
                }

                string[] message = ex.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                throw new Exception(string.Format($"Грешка при обработка: {string.Join(" ", message)} {innerExcMessageFirstLine}"));
            }


            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA1);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            try
            {
                using (MemoryStream ms = new MemoryStream(textBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            sb.Append(reader.ReadToEnd());
                        }
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                string innerExcMessageFirstLine = string.Empty;
                if (ex.InnerException != null)
                {
                    innerExcMessageFirstLine = ex.InnerException.InnerException.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).First();
                }

                string[] message = ex.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                throw new Exception(string.Format($"Грешка при обработка: {string.Join(" ", message)} {innerExcMessageFirstLine}"));
            }
        }

        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            return data;
        }
    }
}
