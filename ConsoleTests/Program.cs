using MultiTenancyFramework;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTests
{
    public class TeacherSubjectClass : Entity
    {
        public virtual long TeacherId { get; set; }

        public virtual long ClassId { get; set; }

        public virtual long SubjectId { get; set; }

    }

    class Program
    {
        static void Init()
        {
            var baseContainer = new BaseContainer();
            MyServiceLocator.SetIoCContainer(baseContainer.Container);
        }
        
        public static async Task<byte[]> Index()
        {
            var roles = new List<UserRole>
            {
                new UserRole
                {
                    Name = "UserRole Web 3",
                    Description = "Go Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new UserRole
                {
                    Name = "UserRole Web 33",
                    Description = "Go Away 4 3",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                },
                new UserRole
                {
                    Name = "UserRole Web 13",
                    Description = "Go Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new UserRole
                {
                    Name = "UserRole Web 73",
                    Description = "Go gfe Away",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                },
            };
            UserRole x;
            return await roles.ToDataTable(new[] {
                new MyDataColumn<UserRole>(c => c.DateCreated),
                //new MyDataColumn(nameof(x.DateCreated)),
                //new MyDataColumn(nameof(x.IsDeleted)),
                //new MyDataColumn(nameof(x.IsDisabled)),
                new MyDataColumn<UserRole>(c => c.IsDeleted),
                new MyDataColumn<UserRole>(c => c.IsDisabled),
                new MyDataColumn(nameof(x.Name)),
                new MyDataColumn(nameof(x.Description)),
            })
            .ToExcel(new Dictionary<string, string> { { "Title", "Test Sheet" }, { "For", "FGC Okigwe" }, { "Date Done", DateTime.Now.ToString() } });
        }


        static void Main(string[] args)
        {
            Init();
            var list = new List<TeacherSubjectClass>();
            list.Add(new TeacherSubjectClass
            {
                TeacherId = 99,
                ClassId = 199,
                SubjectId = 299
            });
            list.Add(new TeacherSubjectClass
            {
                TeacherId = 99,
                ClassId = 199,
                SubjectId = 399
            });
            list.Add(new TeacherSubjectClass
            {
                TeacherId = 99,
                ClassId = 199,
                SubjectId = 499
            });
            //testSplitCamelCase();
            var dao = new MultiTenancyFramework.NHibernate.CoreDAO<TeacherSubjectClass>();
            dao.InstitutionCode = "ME2LQ";
            //dao.SqlBulkInsert(list);

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
