using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MultiTenancyFramework.IoC
{
    public class IoCUtility
    {
        public static HashSet<Assembly> GetAssembliesForRegistration(string iocAssemblyName, string[] assembliesToScan = null)
        {
            if (string.IsNullOrWhiteSpace(iocAssemblyName)) throw new ArgumentNullException("iocAssemblyName");

            if (assembliesToScan == null) assembliesToScan = new string[0];
            HashSet<string> assemblyNamesSet = new HashSet<string>(assembliesToScan);

            string execFolder = AppDomain.CurrentDomain.BaseDirectory;
            var frameworkDlls = Directory.EnumerateFiles(execFolder, "MultiTenancyFramework*.dll", SearchOption.TopDirectoryOnly);
            if (frameworkDlls == null || !frameworkDlls.Any())
            {
                // Maybe it's Web and we're not looking into the bin folder. So...
                execFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                frameworkDlls = Directory.EnumerateFiles(execFolder, "MultiTenancyFramework*.dll", SearchOption.TopDirectoryOnly);
            }
            if (frameworkDlls != null)
            {
                foreach (var dll in frameworkDlls)
                {
                    assemblyNamesSet.Add(Path.GetFileNameWithoutExtension(dll));
                }
            }
            var assemblies = new HashSet<Assembly>();
            foreach (var assemblyName in assemblyNamesSet)
            {
                // Exclude self
                if (assemblyName == iocAssemblyName) continue;

                try
                {
                    assemblies.Add(Assembly.Load(assemblyName));
                }
                catch { }
            }
            return assemblies;
        }
    }
}
