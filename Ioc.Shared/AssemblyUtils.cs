using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ioc
{
    public class AssemblyUtils : IAssemblyUtils
    {
        public String ReadRes<T>(String name)
        {
            var asm = Assembly.GetAssembly(typeof(T));
            var rs = name.Replace("/", ".").Replace("\\", ".").ToUpperInvariant();

            foreach (var avail in asm.GetManifestResourceNames())
            {
                if (avail.ToUpperInvariant().EndsWith(rs))
                {
                    rs = avail;
                    break;
                }
            }
            using (Stream str = asm.GetManifestResourceStream(rs))
            {
                using (StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public IEnumerable<Type> LoadAlltypes(params Type[] interfaces)
        {
            var result = new List<Type>();
            // find all types
            foreach (var interfaceType in interfaces)
            {
                foreach (var currentAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var currentType in currentAsm.GetTypes())
                        {
                            if (interfaceType.IsAssignableFrom(currentType) && currentType.IsClass && !currentType.IsAbstract)
                            {
                                result.Add(currentType);
                            }
                        }
                    }
                    catch { }
                }
            }
            return result;
        }
    }
}
