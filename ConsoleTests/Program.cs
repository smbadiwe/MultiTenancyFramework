using MultiTenancyFramework;
using MultiTenancyFramework.Data;
using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.NHManager;
using MultiTenancyFramework.NHibernate.Queries;
using MultiTenancyFramework.SimpleInjector;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Specialized;
using System.Net;

namespace ConsoleTests
{
    public class Chukwuka : Entity
    {
        public virtual long TeacherId { get; set; }

        public virtual long ClassId { get; set; }

        public virtual long SubjectId { get; set; }
        public virtual bool WillCome { get { return true; } }

    }
    [ComplexType]
    public class NameAndVal
    {
        public string MyName { get; set; }
        public DateTime MyDate { get; set; }
        public decimal MyVal { get; set; }
    }
    public class Somadina : Entity
    {
        public virtual long TeacherId { get; set; }

        public virtual long ClassId { get; set; }
        public virtual Chukwuka Chukwuka { get; set; }
        public override string Name { get; set; }
        public virtual long SubjectId { get; set; }
        public virtual NameAndVal NameAndVal { get; set; }
        public virtual NameAndVal OtherNameAndVal { get; set; }
        public virtual bool WillCome { get { return true; } }

    }
    class Program
    {
        static void Init()
        {
            var baseContainer = new BaseContainer();
            MyServiceLocator.SetIoCContainer(baseContainer.Container);

            // Initialize MVC settings
            //AppStartInitializer.Initialize();

            NHSessionManager.AddEntityAssemblies(new[] { "ConsoleTests" });
            var fileRules = new Dictionary<string, Tuple<string, LoggingLevel>> {
                { "NHibernate.SQL", new Tuple<string, LoggingLevel>( "${shortdate}_nh.log", LoggingLevel.Debug) }
            };
            LoggerConfigurationManager.ConfigureNLog(true, fileRules);
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
            Task.Run(() =>
            {

                var postData = new NameValueCollection
                {
                    {"taskType", null},
                    {"instCode", null}
                };

                try
                {
                    var _scheduleTaskUrl = "http://localhost/schoolsoul/scheduledtaskapi/runtask";
                    using (var client = new WebClient())
                    {
                        client.UploadValues(_scheduleTaskUrl, postData);
                    }
                    Console.WriteLine("Call to {0} done well", _scheduleTaskUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
            //Console.WriteLine(typeof(string).IsPrimitiveType());
            //Console.WriteLine(typeof(DateTime).IsPrimitiveType());
            //Console.WriteLine(typeof(DateTime?).IsPrimitiveType());
            //Console.WriteLine(typeof(int?).IsPrimitiveType());
            //Console.WriteLine(typeof(int).IsPrimitiveType());
            //Dictionary<string, int> dictionary = new Dictionary<string, int>();

            //dictionary.Add("cat1", 1);
            //dictionary.Add("dog2", 2);
            //dictionary.Add("cat3", 3);
            //dictionary.Add("dog4", 4);
            //dictionary.Add("cat5", 5);
            //dictionary.Add("dog6", 6);
            //dictionary.Add("cat7", 7);
            //dictionary.Add("dog8", 8);
            //dictionary.Add("cat9", 9);
            //dictionary.Add("dog10", 10);
            //dictionary.Add("dog11", 11);

            //dictionary.Add("dog100", 100);

            //var firstHalf = new Dictionary<string, int>();
            //int i = 0, count = dictionary.Count;
            //foreach (var item in dictionary.Keys.ToList())
            //{
            //    firstHalf.Add(item, dictionary[item]);
            //    dictionary.Remove(item);
            //    if (++i == count / 2) break;
            //}
            Console.ReadKey();
            //var t = Type.ReflectionOnlyGetType("NHibernate.HibernateException, NHibernate", false, true);

            //var isIt = t != null && typeof(ApplicationException).IsAssignableFrom(t);
            //return;
            //Init();
            //var list = new List<Somadina>();
            //list.Add(new Somadina
            //{
            //    TeacherId = 99,
            //    ClassId = 199,
            //    SubjectId = 299
            //});
            //list.Add(new Somadina
            //{
            //    TeacherId = 99,
            //    ClassId = 199,
            //    SubjectId = 399
            //});
            //list.Add(new Somadina
            //{
            //    TeacherId = 99,
            //    ClassId = 199,
            //    SubjectId = 499
            //});
            ////testSplitCamelCase();
            //var handler = new GetAppUserByUsernameQueryHandler
            //{
            //    InstitutionCode = "mel2q"
            //};
            //var res = handler.Handle(new MultiTenancyFramework.Data.Queries.GetAppUserByUsernameQuery { Username = "Super User" });
            //try
            //{
            //    var dao = new MultiTenancyFramework.NHibernate.CoreDAO<UserRole>();
            //    dao.InstitutionCode = "mel2q";
            //    var ite = dao.RetrieveAll();
            //    var ite2 = dao.Retrieve(1);

            //}
            //catch (Exception ex)
            //{
            //    Utilities.Logger.Log(ex);
            //}
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
