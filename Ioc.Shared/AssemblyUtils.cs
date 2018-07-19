using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ioc
{
    public class AssemblyUtils : IAssemblyUtils
    {
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
