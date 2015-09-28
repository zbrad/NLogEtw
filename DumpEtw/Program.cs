using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using ZBrad.NLogEtw;


namespace DumpEtw
{
    class Program
    {
        static void Main(string[] args)
        {
            var man = EtwTarget.GetManifest();
            Console.WriteLine(man);
        }
    }
}
