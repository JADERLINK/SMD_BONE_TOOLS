using SMD_READER_LIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SMD_NEW_BONE_NAME
{
    internal static class SmdNewBoneName
    {
        public static void Action(string smdFilePath) 
        {
            string baseBoneName = Path.GetFileNameWithoutExtension(smdFilePath).ToUpperInvariant().Replace(" ", "_").Replace("-", "_");
            
            SMD eSMD = null;
            StreamReader eStream = null;

            try
            {
                var eFile = new FileInfo(smdFilePath);
                Console.WriteLine("File: " + eFile.Name);
                eStream = eFile.OpenText();
                eSMD = SmdReader.Reader(eStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                eStream?.Close();
            }

            int NameConsolePad = 0;

            Dictionary<string, (int BoneID, string Name, int Parent)> BoneList = new Dictionary<string, (int BoneID, string Name, int Parent)>();

            foreach (var item in eSMD.Nodes)
            {
                string key = item.BoneName.ToLowerInvariant();
                if (!BoneList.ContainsKey(key))
                {
                    BoneList.Add(key, (item.ID, item.BoneName, item.ParentID));
                }

                if (key.Length > NameConsolePad)
                {
                    NameConsolePad = key.Length;
                }
            }

            Regex regex = new Regex("^.*b(?:one|0ne)_[0-9]{1,3}$");

            // new node
            List<Node> NewBones = new List<Node>();

            int NewNameConsolePad = NameConsolePad > baseBoneName.Length + 9 ? NameConsolePad : baseBoneName.Length + 9;

            foreach (var item in BoneList)
            {
                string newName = item.Value.Name;

                var split = item.Key.Split('_');
                if (regex.IsMatch(item.Key) && split.Length > 1 && int.TryParse(split.LastOrDefault(), out int newId))
                {
                    newName = baseBoneName + "_B0NE_" + newId.ToString("D3");
                }

                Console.WriteLine( "OldName: \"" + (item.Value.Name + "\"").PadRight(NameConsolePad + 1) + 
                    "  NewName: \"" + (newName + "\"").PadRight(NewNameConsolePad + 1) +
                    "  BoneID: " + item.Value.BoneID.ToString().PadLeft(3) +
                    "  ParentID: " + item.Value.Parent.ToString().PadLeft(3));

                NewBones.Add(new Node(item.Value.BoneID, newName, item.Value.Parent));
            }

            NewBones = NewBones.OrderBy(x => x.ID).ToList();

            SMD newSMD = new SMD();
            newSMD.Nodes = NewBones;
            newSMD.Times = eSMD.Times;
            newSMD.Triangles = eSMD.Triangles;
            newSMD.VertexAnimation = eSMD.VertexAnimation;

            var stream = new FileInfo(smdFilePath).CreateText();
            SMD_WRITER_LIB.SmdWriter.WriteSMD(newSMD, stream);
            stream.WriteLine("// SMD_NEW_BONE_NAME");
            stream.WriteLine("// By: JADERLINK");
            stream.WriteLine("// youtube.com/@JADERLINK");
            stream.Write("// github.com/JADERLINK");
            stream.Close();
        }

    }
}
