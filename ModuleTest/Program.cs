using AgorastorePSModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var module = new BuildSolutions();

            module.BuildCmdlet();


        }
    }
}
