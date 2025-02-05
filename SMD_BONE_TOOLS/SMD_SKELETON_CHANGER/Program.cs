using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_SKELETON_CHANGER
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("# SMD_SKELETON_CHANGER");
            Console.WriteLine("# By JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# github.com/JADERLINK");
            Console.WriteLine("# VERSION 1.0 (2025-02-05)");
            Console.WriteLine("");

            if (args.Length >= 2 
                && File.Exists(args[0]) 
                && File.Exists(args[1])
                && Path.GetExtension(args[0].ToUpperInvariant()) == ".SMD"
                && Path.GetExtension(args[1].ToUpperInvariant()) == ".SMD"
                )
            {
                try
                {
                    SkeletonChanger.Action(args[0], args[1]);
                    Console.WriteLine("Finished!!!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid Arguments!");
            }

            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }

    }
}
