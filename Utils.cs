using System;
using System.Net.NetworkInformation;

namespace GenericUpdater.SDK
{
    public class Utils
    {
        public static bool IsConnectedToInternet()
        {
            try
            {
                const string host = "www.google.com";
                var p = new Ping();
                var reply = p.Send(host, 3000);
                if (reply?.Status == IPStatus.Success)
                    return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return false;
        }

        // https://www.csharpens.com/c-sharp/convert-bytes-to-kb-mb-gb-and-tb-in-c-sharp-37/
        public static string ToKnownTransferUnit(double value)
        {
            string[] suffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};
            for (var i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value / Math.Pow(1024, i)) + " " + suffixes[i];
                }
            }

            return ThreeNonZeroDigits(value / Math.Pow(1024, suffixes.Length - 1)) +
                   " " + suffixes[suffixes.Length - 1];
        }

        // https://www.csharpens.com/c-sharp/convert-bytes-to-kb-mb-gb-and-tb-in-c-sharp-37/
        // Return the value formatted to include at most three
        // non-zero digits and at most two digits after the
        // decimal point. Examples:
        //         1
        //       123
        //        12.3
        //         1.23
        //         0.12
        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
                return value.ToString("0,0");
            if (value >= 10)
                return value.ToString("0.0");
            else
                return value.ToString("0.00");
        }
    }
}