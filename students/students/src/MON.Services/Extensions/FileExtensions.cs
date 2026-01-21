using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Extensions
{
    public static class FileExtensions
    {
        public static byte[] ToByteArray(this IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            long length = file.Length;

            using Stream fileStream = file.OpenReadStream();
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, (int)file.Length);

            return bytes;
        }

        public static List<string> ReadAsList(this IFormFile file, Encoding encoding = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            List<string> result = new List<string>();
            using (StreamReader reader = encoding == null ? new StreamReader(file.OpenReadStream()) : new StreamReader(file.OpenReadStream(), encoding))
            {
                while (reader.Peek() >= 0)
                    result.Add(reader.ReadLine());
            }
            return result;
        }

        public static async Task<List<string>> ReadAsListAsync(this IFormFile file, Encoding encoding = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            List<string> result = new List<string>();
            using (StreamReader reader = encoding == null ? new StreamReader(file.OpenReadStream()) : new StreamReader(file.OpenReadStream(), encoding))
            {
                while (reader.Peek() >= 0)
                    result.Add(await reader.ReadLineAsync());
            }

            return result;
        }



        public static async Task<List<string>> ReadAsListAsync(this byte[] file, Encoding encoding = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            List<string> result = new List<string>();
            using Stream stream = new MemoryStream(file);
            using StreamReader reader = encoding == null ? new StreamReader(stream) : new StreamReader(stream, encoding);
            while (reader.Peek() >= 0)
            {
                result.Add(await reader.ReadLineAsync());
            }

            return result;
        }

        public static string ReadAsString(this IFormFile file, Encoding encoding = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            StringBuilder result = new StringBuilder();
            using (StreamReader reader = encoding == null ? new StreamReader(file.OpenReadStream()) : new StreamReader(file.OpenReadStream(), encoding))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }

            return result.ToString();
        }

        public static async Task<string> ReadAsStringAsync(this IFormFile file, Encoding encoding = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            StringBuilder result = new StringBuilder();
            using (StreamReader reader = encoding == null ? new StreamReader(file.OpenReadStream()) : new StreamReader(file.OpenReadStream(), encoding))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }

            return result.ToString();
        }
    }
}
