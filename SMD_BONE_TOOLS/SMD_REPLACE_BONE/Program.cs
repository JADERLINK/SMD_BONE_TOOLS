using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_REPLACE_BONE
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("# SMD_REPLACE_BONE");
            Console.WriteLine("# by: JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# github.com/JADERLINK");
            Console.WriteLine("# Version 1.0 (2025-02-05)");
            Console.WriteLine("");

            try
            {
                Continue(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            Console.WriteLine("Finished!!!");
            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }

        public static void Continue(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You must pass as parameters a SMD file and a REPLACEBONE file");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("The first file does not exist.");
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine("The second file does not exist.");
                return;
            }

            FileInfo SmdFileInfo = null;
            FileInfo renameFileInfo = null;

            if (Path.GetExtension(args[0].ToLowerInvariant()).Contains("smd"))
            {
                SmdFileInfo = new FileInfo(args[0]);
            }
            else if (Path.GetExtension(args[1].ToLowerInvariant()).Contains("smd"))
            {
                SmdFileInfo = new FileInfo(args[1]);
            }

            if (Path.GetExtension(args[0].ToLowerInvariant()).Contains("replacebone"))
            {
                renameFileInfo = new FileInfo(args[0]);
            }
            else if (Path.GetExtension(args[1].ToLowerInvariant()).Contains("replacebone"))
            {
                renameFileInfo = new FileInfo(args[1]);
            }

            if (SmdFileInfo == null)
            {
                Console.WriteLine("The SMD path not found.");
                return;
            }

            if (renameFileInfo == null)
            {
                Console.WriteLine("The REPLACEBONE path not found.");
                return;
            }

            SMD_READER_LIB.SMD smd = null;
            ReplaceBone replace;

            StreamReader streamSmd = null;
            try
            {
                streamSmd = SmdFileInfo.OpenText();
                smd = SMD_READER_LIB.SmdReader.Reader(streamSmd);
            }
            catch (Exception)
            {
                Console.WriteLine("Error loading SMD file.");
                return;
            }
            finally 
            {
                if (streamSmd != null)
                {
                    streamSmd.Close();
                }
            }

            try
            {
                replace = ReplaceBone.GetReplace(renameFileInfo);
            }
            catch (Exception)
            {
                Console.WriteLine("Error loading REPLACEBONE file.");
                return;
            }

            SMD_READER_LIB.SMD newSMD = SmdReplaceBone.Action(smd, replace);

            var stream = new FileInfo(Path.ChangeExtension(SmdFileInfo.FullName, ".replaced.smd")).CreateText();
            SMD_WRITER.SmdWriter.WriteSMD(newSMD, stream);
            stream.WriteLine("// SMD_REPLACE_BONE");
            stream.WriteLine("// By: JADERLINK");
            stream.WriteLine("// youtube.com/@JADERLINK");
            stream.WriteLine("// github.com/JADERLINK");
            stream.Write("// replacebone: " + renameFileInfo.Name);
            stream.Close();
        }

    }
}
