using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiTenancyFramework
{
    public static class StringExtensions
    {
        public static string[] SplitString(this string original, params char[] separators)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(separators)
                        let trimmed = piece.Trim()
                        where !string.IsNullOrWhiteSpace(trimmed)
                        select trimmed;
            return split.ToArray();
        }

        public static string TrimAndRemoveUnwantedCharacters(this string str, char[] unwantedCharacters)
        {
            str = str.Trim();
            foreach (var xter in unwantedCharacters)
            {
                str = str.Replace(xter.ToString(), "");
            }
            return str;
        }

        /// <summary>
        /// E.g. "bad coder" becomes "BadCoder"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str.Trim()))
                throw new Exception("Check to ensure that \"" + str.Trim() + "\" is NOT null");

            //remove these characters
            var s = str.TrimAndRemoveUnwantedCharacters().ToLower();

            if (s.Length < 2) return s.ToUpper();
            //split based on the white spaces
            var arr = s.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            //capitalise first letter in each case
            var stringToReturn = string.Empty;
            foreach (var oneString in arr)
            {
                stringToReturn += oneString.Substring(0, 1).ToUpper() + oneString.Substring(1);
            }

            return stringToReturn.Trim();
        }

        /// <summary>
        /// E.g. "Bad coder" becomes "BAD_CODER"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
		public static string ToConstantFormat(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(str.Trim()))
            {
                throw new Exception("Check to ensure that value is NOT null or empty or white-spaces");
            }

            //remove these characters; re-format string
            var s = str.Trim().Replace(".", "").Replace(",", "").Replace("-", "").Replace("_", "");
            return s.ToUpper().Replace(" ", "_"); ;
        }

        /// <summary>
        /// E.g. "bad coder" becomes "Bad Coder"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CapitalizeEachWord(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(str.Trim()))
            {
                throw new Exception("Check to ensure that value is NOT null or empty or white-spaces");
            }

            //split based on the white spaces
            var arr = str.Trim().Split(' ');

            //capitalise first letter in each case
            var stringToReturn = String.Empty;
            foreach (var oneString in arr)
            {
                if (string.IsNullOrWhiteSpace(oneString))
                    continue;
                var firstLetter = oneString.Substring(0, 1);
                var others = oneString.Length > 1 ? oneString.Substring(1) : string.Empty;
                stringToReturn += firstLetter.ToUpper() + others + " ";
            }

            return stringToReturn.Trim();
        }

        public static decimal ToDecimalAndRemoveBracketIfNegative(this string amount)
        {
            amount = amount.Replace(",", "");  //Remove any commas.

            if (amount.Trim().StartsWith("("))
            {
                return -1 * Convert.ToDecimal(amount.Trim().TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' }));
            }

            return Convert.ToDecimal(amount);
        }

        public static string ToPlural(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var pluralString = text;
            var lastCharacter = pluralString.Substring(pluralString.Length - 1).ToLower();

            // y’s become ies (such as Category to Categories)
            if (string.Equals(lastCharacter, "y", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("ay", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("ey", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("iy", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("oy", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("uy", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                pluralString = pluralString.Remove(pluralString.Length - 1);
                pluralString += "ie";
            }

            if (string.Equals(lastCharacter, "s", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("as", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("es", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("is", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("os", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("us", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("ms", StringComparison.InvariantCultureIgnoreCase)
                && !pluralString.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                pluralString = pluralString.Remove(pluralString.Length - 1);
            }
            // ch’s become ches (such as Pirch to Pirches)
            if (string.Equals(pluralString.Substring(pluralString.Length - 2), "ch",
                              StringComparison.InvariantCultureIgnoreCase))
            {
                pluralString += "e";
            }

            switch (lastCharacter)
            {
                case "s":
                    return pluralString + "es";

                default:
                    return pluralString + "s";

            }

        }

        /// <summary>
        /// Useful when you want to convert a string in PascalCase to human-meaningful form. In essence, splitting at capital letters.
        /// It guarantees that the returned string must start with upper-case letter. Acronyms and numbers are also rendered properly.
        /// </summary>
        /// <param name="stringToSplit"></param>
        /// <returns></returns>
        public static string AsSplitPascalCasedString(this string stringToSplit)
        {
            string finalString = Regex.Replace(stringToSplit, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

            if (finalString.Length == 0) return finalString;

            if (char.IsLower(finalString[0]))
            {
                finalString = string.Format("{0}{1}", finalString.Substring(0, 1).ToUpper(), finalString.Substring(1, finalString.Length - 1));
            }

            //This part is responsible for joining the ONE-LETTER strings.
            string[] moreCheck = finalString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (moreCheck.Length == 1)
            {
                return finalString;
            }

            StringBuilder result = new StringBuilder(moreCheck[0]);
            bool addSpace = moreCheck[0].Length > 1;

            for (int i = 1; i < moreCheck.Length; i++)
            {
                if (moreCheck[i].Length == 1)
                {
                    result.AppendFormat("{0}{1}", addSpace == true && i == 1 ? " " : "", moreCheck[i]);
                }
                else
                {
                    //Sometimes we may have numbers within the mix. Eg: "GL11Version" should give "GL 11 Version"
                    var subs = moreCheck[i].Substring(1);
                    int intg;
                    if (int.TryParse(subs, out intg))
                    {
                        result.AppendFormat("{0} {1}", moreCheck[i][0], subs);
                    }
                    else
                    {
                        result.AppendFormat(" {0}", moreCheck[i]);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(this string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            byte[] compressedData;
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
            }
            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(this string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        private static string TrimAndRemoveUnwantedCharacters(this string str)
        {
            str = str.Trim();
            return str.Replace(".", "").Replace(",", "").Replace("-", " ")
                                .Replace("_", "").Replace("(", " ").Replace(")", " ")
                                .Replace("\"", "").Replace("'", "").Replace("=", "")
                                .Replace("%", "Percent").Replace("&", "And").Replace(";", "")
                                .Replace("!", "").Replace("/", " ").Replace("@", "At")
                                .Replace("#", "").Replace("~", "").Replace("$", "USD")
                                .Replace("^", "").Replace("|", "").Replace("`", "")
                                .Replace(":", "").Replace("?", "").Replace("+", "")
                                .Replace("*", "");
        }

    }
}
