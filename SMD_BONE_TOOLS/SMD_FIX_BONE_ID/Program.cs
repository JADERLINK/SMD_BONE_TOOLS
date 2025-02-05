using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_FIX_BONE_ID
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("# FIX_SMD_BONE_ID");
            Console.WriteLine("# By JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# github.com/JADERLINK");
            Console.WriteLine("# VERSION 1.0 (2025-02-05)");
            Console.WriteLine("");

            for (int i = 0; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    string ext = Path.GetExtension(args[i].ToUpperInvariant());
                    if (ext == ".SMD")
                    {
                        try
                        {
                            FixSmdBoneId.Action(args[i]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error:");
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid File!");
                    }

                }
                else
                {
                    Console.WriteLine("File specified does not exist: " + args[i]);
                }
            }

            Console.WriteLine("Finished!!!");
            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }
    }
}
