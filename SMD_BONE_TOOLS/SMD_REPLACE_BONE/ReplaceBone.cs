using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_REPLACE_BONE
{
    internal class ReplaceBone
    {
        public Dictionary<string, (string inName, string outName, int BoneID, int ParentID)> ReplaceDic { get; }
        public int OutNameConsolePad = 0; 

        public ReplaceBone()
        {
            ReplaceDic = new Dictionary<string, (string inName, string outName, int BoneID, int ParentID)>();
        }

        public static ReplaceBone GetReplace(FileInfo configFileInfo)
        {
            int error = 0;

            ReplaceBone rb = new ReplaceBone();

            var idx = configFileInfo.OpenText();
            while (!idx.EndOfStream)
            {
                string line = idx.ReadLine().Trim();
                if ((line.Length == 0
                        || line.StartsWith("#")
                        || line.StartsWith("\\")
                        || line.StartsWith("/")
                        || line.StartsWith(":")
                        ))
                {
                    continue;
                }

                var split = line.Split('|');
                if (split.Length >= 4)
                {
                    string inName = split[0].Trim();
                    string outName = split[2].Trim();
                    if (inName.Length == 0)
                    {
                        inName = "ERROR" + error;
                        error++;
                    }
                    if (outName.Length == 0)
                    {
                        outName = "ERROR" + error;
                        error++;
                    }

                    int.TryParse(split[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int BoneID);
                    int.TryParse(split[3].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int ParentID);

                    string key = inName.ToLowerInvariant();
                    if (! rb.ReplaceDic.ContainsKey(key))
                    {
                        rb.ReplaceDic.Add(key,(inName, outName, BoneID, ParentID));

                        if (rb.OutNameConsolePad < outName.Length)
                        {
                            rb.OutNameConsolePad = outName.Length;
                        }
                    }

                   
                }
                
            }
            idx.Close();

            return rb;
        }

    }
}
