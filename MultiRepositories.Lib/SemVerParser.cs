
using SemVer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public class SemVerParser
    {
        public static SemVersion Parse(String version)
        {
            SemVersion result = null;
            if (!SemVersion.TryParse(version, out result))
            {
                string realVer = null;
                var exp = version.Split('.');
                for (int i = 0; i < exp.Length; i++)
                {
                    int test = 0;
                    if (int.TryParse(exp[i], out test))
                    {
                        if (realVer == null)
                        {
                            realVer = exp[i];
                        }
                        else
                        {
                            if (i < 3)
                            {
                                realVer += "." + exp[i];
                            }
                            else if (i == 3 && int.TryParse(exp[i],out test))
                            {
                                //realVer += "+" + exp[i];
                            }
                            else
                            {
                                realVer += "." + exp[i];
                            }
                        }
                    }
                }
                return SemVersion.Parse(realVer);
            }
            return result;
        }
    }
}
