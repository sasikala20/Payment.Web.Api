using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.Web.Api.Utilities
{
    public static class StringExtensions
    {
              
        public static string Mask(this string source, int start, int end, char maskCharacter)
        {
            int maskLength = source.Length - (start+ end);
            if (start > source.Length - 1)
            {
                throw new ArgumentException("Start position is greater than string length");
            }

            if (start + end > source.Length)
            {
                throw new ArgumentException("Start position and mask length imply more characters than are present");
            }

            string mask = new string(maskCharacter, maskLength);
            string unMaskStart = source.Substring(0, start);
            string unMaskEnd = source.Substring(start + maskLength, end);

            return unMaskStart + mask + unMaskEnd;
        }
    }
}
 