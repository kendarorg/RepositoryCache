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
        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
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

            using (Stream resFilestream = asm.GetManifestResourceStream(rs))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                var result= Encoding.UTF8.GetString(ba);
                if (result.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
                {
                    result = result.Remove(0, _byteOrderMarkUtf8.Length);
                }
                return result;
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
