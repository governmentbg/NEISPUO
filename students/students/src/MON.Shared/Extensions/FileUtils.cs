namespace MON.Shared.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class FileUtils
    {
        public static string GetTempFileName(string extension)
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extension);
        }
    }
}
