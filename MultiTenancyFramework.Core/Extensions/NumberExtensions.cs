using System;
using System.Globalization;

namespace MultiTenancyFramework
{
    public static class NumberExtensions
    {
        /// <summary>
        /// This adds ordinal to a number: 1 -> 1st etc
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string AddOrdinal(this int num)
        {
            return AddOrdinal((long)num);
        }

        /// <summary>
        /// This adds ordinal to a number: 1 -> 1st etc
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string AddOrdinal(this long num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        /// <summary>
        /// Reformats a given amount to money format. NB: This does not multiply by 100.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToMoney(this long amount)
        {
            return Convert.ToDecimal(amount).ToMoney();
        }

        /// <summary>
        /// Reformats a given amount to money format. NB: This does not multiply by 100.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToMoney(this int amount)
        {
            return ((long)amount).ToMoney();
        }

        /// <summary>
        /// Reformats a given amount to money format. NB: This does not multiply by 100.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToMoney(this decimal amount)
        {
            //if (amount < 0)
            //{
            //    return (Math.Floor(amount)).ToString("N2");
            //}
            //return (Math.Ceiling(amount)).ToString("N2");
            return amount.ToString("N2");
        }

        /// <summary>
        /// Reformats a given amount to money format. NB: This does not multiply by 100.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string ToMoneyWithBracketIfNegative(this decimal amount)
        {
            if (amount < 0)
            {
                return string.Format("({0})", Math.Abs(amount).ToMoney());
            }
            return amount.ToMoney();
        }

        // Lookup table.
        static string[] _cache =
            {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
                "21", "22", "23", "24", "25",
                "26",
                "27",
                "28",
                "29",
                "30",
                "31",
                "32",
                "33",
                "34",
                "35",
                "36",
                "37",
                "38",
                "39",
                "40",
                "41",
                "42",
                "43",
                "44",
                "45",
                "46",
                "47",
                "48",
                "49",
                "50",
                "51",
                "52",
                "53",
                "54",
                "55",
                "56",
                "57",
                "58",
                "59",
                "60",
                "61",
                "62",
                "63",
                "64",
                "65",
                "66",
                "67",
                "68",
                "69",
                "70",
                "71",
                "72",
                "73",
                "74",
                "75",
                "76",
                "77",
                "78",
                "79",
                "80",
                "81",
                "82",
                "83",
                "84",
                "85",
                "86",
                "87",
                "88",
                "89",
                "90",
                "91",
                "92",
                "93",
                "94",
                "95",
                "96",
                "97",
                "98",
                "99",
                "100",
                "101",
                "102",
                "103",
                "104",
                "105",
                "106",
                "107",
                "108",
                "109",
                "110",
                "111",
                "112",
                "113",
                "114",
                "115",
                "116",
                "117",
                "118",
                "119",
                "120",
                "121",
                "122",
                "123",
                "124",
                "125",
                "126",
                "127",
                "128",
                "129",
                "130",
                "131",
                "132",
                "133",
                "134",
                "135",
                "136",
                "137",
                "138",
                "139",
                "140",
                "141",
                "142",
                "143",
                "144",
                "145",
                "146",
                "147",
                "148",
                "149",
                "150",
                "151",
                "152",
                "153",
                "154",
                "155",
                "156",
                "157",
                "158",
                "159",
                "160",
                "161",
                "162",
                "163",
                "164",
                "165",
                "166",
                "167",
                "168",
                "169",
                "170",
                "171",
                "172",
                "173",
                "174",
                "175",
                "176",
                "177",
                "178",
                "179",
                "180",
                "181",
                "182",
                "183",
                "184",
                "185",
                "186",
                "187",
                "188",
                "189",
                "190",
                "191",
                "192",
                "193",
                "194",
                "195",
                "196",
                "197",
                "198",
                "199"
            };

        // Lookup table last index.
        const int _top = 199;

        public static string ToStringLookup(this int value)
        {
            return ((long)value).ToStringLookup();
        }

        public static string ToStringLookup(this long value)
        {
            // See if the integer is in range of the lookup table.
            // ... If it is present, return the string literal element.
            if (value >= 0 && value <= _top)
            {
                return _cache[value];
            }
            else if (value < 0 && Math.Abs(value) <= _top)
            {
                return "-" + _cache[value];
            }
            // Fall back to ToString method.
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
