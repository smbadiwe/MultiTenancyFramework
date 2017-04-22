using MultiTenancyFramework;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.Logic;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTests
{
    class Program
    {
        static void Init()
        {
            var baseContainer = new BaseContainer();
            MyServiceLocator.SetIoCContainer(baseContainer.Container);
        }

        static void Main(string[] args)
        {
            testSplitCamelCase();
            var dao = new MultiTenancyFramework.NHibernate.CoreDAO<Institution>();
            var school = dao.RetrieveAll();
            Console.ReadKey();
        }

        private static void testSplitCamelCase()
        {
            assertEquals("lowercase", splitCamelCase("lowercase"));
            assertEquals("Class", splitCamelCase("Class"));
            assertEquals("My Class", splitCamelCase("MyClass"));
            assertEquals("HTML", splitCamelCase("HTML"));
            assertEquals("PDF Loader", splitCamelCase("PDFLoader"));
            assertEquals("A String", splitCamelCase("AString"));
            assertEquals("Simple XML Parser", splitCamelCase("SimpleXMLParser"));
            assertEquals("GL 11 Version", splitCamelCase("GL11Version"));
        }

        private static string splitCamelCase(string v)
        {
            return AsSplitPascalCasedString(v);
        }
        
        private static string AsSplitPascalCasedString(string stringToSplit)
        {
            string finalString = Regex.Replace(stringToSplit, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

            if (finalString.Length == 0) return finalString;

            if (char.IsLower(finalString[0]))
            {
                finalString = string.Format("{0}{1}", finalString.Substring(0, 1).ToUpper(), finalString.Substring(1, finalString.Length - 1));
            }

            //This part is responsible for joining the ONE-LETTER strings.
            string[] moreCheck = finalString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                    //Sometimes we may have numbers within the mix. Eg: 
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
        
        private static void assertEquals(string v, string p)
        {
            var str = string.Format("Are equal: {0}.", v == p);
            if (v != p)
            {
                str += "\tOP: " + p;
            }
            Console.WriteLine(str);
        }
    }
}
