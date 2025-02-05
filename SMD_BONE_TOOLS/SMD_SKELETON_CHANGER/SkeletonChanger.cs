using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMD_READER_LIB;

namespace SMD_SKELETON_CHANGER
{
    internal static class SkeletonChanger
    {
        public static void Action(string originalFilePath, string editedFilePath) 
        {
            SMD oSMD = null;
            SMD eSMD = null;

            StreamReader oStream = null;
            StreamReader eStream = null;

            try
            {
                var oFile = new FileInfo(originalFilePath);
                var eFile = new FileInfo(editedFilePath);

                Console.WriteLine("OriginalFile: " + oFile.Name);
                Console.WriteLine("EditedFile: " + eFile.Name);

                oStream = oFile.OpenText();
                eStream = eFile.OpenText();
                oSMD = SmdReader.Reader(oStream);
                eSMD = SmdReader.Reader(eStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oStream != null)
                {
                    oStream.Close();
                }
                if (eStream != null)
                {
                    eStream.Close();
                }
            }

            int lastNextOriginalID = 0;
            int NameConsolePad = 0;

            //original
            Dictionary<string, int> originalOrder = new Dictionary<string, int>();
            Dictionary<string, int> originalParent = new Dictionary<string, int>();
            Dictionary<string, string> originalBoneName = new Dictionary<string, string>();

            foreach (var item in oSMD.Nodes)
            {
                string key = item.BoneName.ToLowerInvariant();
                if (! originalOrder.ContainsKey(key) )
                {
                    originalOrder.Add(key, item.ID);
                    originalBoneName.Add(key, item.BoneName);
                    originalParent.Add(key, item.ParentID);
                }

                if (lastNextOriginalID <= item.ID)
                {
                    lastNextOriginalID = item.ID + 1;
                }

                if (key.Length > NameConsolePad)
                {
                    NameConsolePad = key.Length;
                }
            }

            // edit
            Dictionary<string, int> editedOrder = new Dictionary<string, int>();
            Dictionary<string, int> editedParent = new Dictionary<string, int>();
            Dictionary<string, string> editedBoneName = new Dictionary<string, string>();

            foreach (var item in eSMD.Nodes)
            {
                string key = item.BoneName.ToLowerInvariant();
                if (!editedOrder.ContainsKey(key))
                {
                    editedOrder.Add(key, item.ID);
                    editedBoneName.Add(key, item.BoneName);
                    editedParent.Add(key, item.ParentID);
                }

                if (key.Length > NameConsolePad)
                {
                    NameConsolePad = key.Length;
                }
            }

            //new ids
            Dictionary<int, int> oldIdNewId = new Dictionary<int, int>();
            oldIdNewId.Add(-1, -1);

            foreach (var item in editedOrder)
            {
                if ( ! oldIdNewId.ContainsKey(item.Value))
                {
                    if (originalOrder.ContainsKey(item.Key))
                    {
                        oldIdNewId.Add(item.Value, originalOrder[item.Key]);
                    }
                    else
                    {
                        oldIdNewId.Add(item.Value, lastNextOriginalID);
                        lastNextOriginalID++;
                    }
                }
            }

            // new node
            List<Node> NewBones = new List<Node>();
            List<string> usedBoneName = new List<string>();

            foreach (var item in editedOrder)
            {
                if (!usedBoneName.Contains(item.Key))
                {
                    int newBoneID = oldIdNewId[item.Value];
                    int newParent = -1;
                    if (oldIdNewId.ContainsKey(editedParent[item.Key]))
                    {
                        newParent = oldIdNewId[editedParent[item.Key]];
                    }
                    string newName = editedBoneName[item.Key];
                    Console.WriteLine("Name: \"" + (newName + "\"").PadRight(NameConsolePad + 1) + "  BoneID: " + 
                        item.Value.ToString().PadLeft(3) + " => " + newBoneID.ToString().PadLeft(3) + 
                        "  ParentID: " + editedParent[item.Key].ToString().PadLeft(3) + " => " + newParent.ToString().PadLeft(3));

                    NewBones.Add(new Node(newBoneID, newName, newParent));
                    usedBoneName.Add(item.Key);
                }
            }

            foreach (var item in originalOrder)
            {
                if (!usedBoneName.Contains(item.Key)) 
                {
                    int newBoneID = item.Value;
                    int newParent = originalParent[item.Key];
                    string newName = originalBoneName[item.Key];
                    Console.WriteLine("Name: \"" + (newName + "\"").PadRight(NameConsolePad + 1) + "  BoneID: " +
                        item.Value.ToString().PadLeft(3) + " => " + newBoneID.ToString().PadLeft(3) +
                        "  ParentID: " + originalParent[item.Key].ToString().PadLeft(3) + " => " + newParent.ToString().PadLeft(3));

                    NewBones.Add(new Node(newBoneID, newName, newParent));
                    usedBoneName.Add(item.Key);
                }
            }

            NewBones = NewBones.OrderBy(x => x.ID).ToList();

            //new bone Positions
            List<Time> newTime = new List<Time>();

            foreach (var time in eSMD.Times)
            {
                List<Skeleton> newskeletons = new List<Skeleton>();

                foreach (var skeleton in time.Skeletons)
                {
                    int newBoneId = 0;
                    if (oldIdNewId.ContainsKey(skeleton.BoneID))
                    {
                        newBoneId = oldIdNewId[skeleton.BoneID];
                    }
                    else
                    {
                        newBoneId = lastNextOriginalID;
                        oldIdNewId.Add(skeleton.BoneID, lastNextOriginalID);
                        lastNextOriginalID++;

                    }

                    Skeleton newSkeleton = new Skeleton(newBoneId);
                    newSkeleton.PosX = skeleton.PosX;
                    newSkeleton.PosY = skeleton.PosY;
                    newSkeleton.PosZ = skeleton.PosZ;
                    newSkeleton.RotX = skeleton.RotX;
                    newSkeleton.RotY = skeleton.RotY;
                    newSkeleton.RotZ = skeleton.RotZ;
                    newskeletons.Add(newSkeleton);
                }

                Time newtime = new Time(time.ID);
                newtime.Skeletons = newskeletons;
                newTime.Add(newtime);
            }

            // new bone Positions falantes

            foreach (var time in oSMD.Times)
            {
                var find = newTime.Where(x => x.ID == time.ID).FirstOrDefault();

                if (find != null)
                {
                    foreach (var skeleton in time.Skeletons)
                    {
                        var find2 = find.Skeletons.Where(x => x.BoneID == skeleton.BoneID).FirstOrDefault();

                        if (find2 == null)
                        {
                            find.Skeletons.Add(skeleton);
                        }
                    }
                }
            }

            newTime = newTime.OrderBy(x => x.ID).ToList();
            foreach (var item in newTime)
            {
                item.Skeletons = item.Skeletons.OrderBy(x => x.BoneID).ToList();
            }

            // vertex fix
            List<Triangle> newTriangles = new List<Triangle>();

            foreach (var tri in eSMD.Triangles)
            {
                List<Vertex> newVertexs = new List<Vertex>();
                foreach (var vertex in tri.Vertexs)
                {
                    Vertex nv = new Vertex();
                    nv.VertexID = vertex.VertexID;
                    nv.PosX = vertex.PosX;
                    nv.PosY = vertex.PosY;
                    nv.PosZ = vertex.PosZ;
                    nv.NormX = vertex.NormX;
                    nv.NormY = vertex.NormY;
                    nv.NormZ = vertex.NormZ;
                    nv.U = vertex.U;
                    nv.V = vertex.V;

                    int newBoneId = 0;
                    if (oldIdNewId.ContainsKey(vertex.ParentBone))
                    {
                        newBoneId = oldIdNewId[vertex.ParentBone];
                    }
                    else
                    {
                        newBoneId = lastNextOriginalID;
                        oldIdNewId.Add(vertex.ParentBone, lastNextOriginalID);
                        lastNextOriginalID++;
                    }
                    nv.ParentBone = newBoneId;

                    nv.Links = new List<WeightMap>();

                    foreach (var weightMap in vertex.Links)
                    {
                        int newWeightBoneId = 0;
                        if (oldIdNewId.ContainsKey(weightMap.BoneID))
                        {
                            newWeightBoneId = oldIdNewId[weightMap.BoneID];
                        }
                        else
                        {
                            newWeightBoneId = lastNextOriginalID;
                            oldIdNewId.Add(weightMap.BoneID, lastNextOriginalID);
                            lastNextOriginalID++;
                        }

                        WeightMap newWeightMap = new WeightMap();
                        newWeightMap.BoneID = newWeightBoneId;
                        newWeightMap.Weight = weightMap.Weight;
                        nv.Links.Add(newWeightMap);

                    }

                    newVertexs.Add(nv);
                }

                Triangle newtriangle = new Triangle();
                newtriangle.ID = tri.ID;
                newtriangle.Material = tri.Material;
                newtriangle.Vertexs = newVertexs;
                newTriangles.Add(newtriangle);
            }

            SMD newSMD = new SMD();
            newSMD.Nodes = NewBones;
            newSMD.Times = newTime;
            newSMD.Triangles = newTriangles;
            newSMD.VertexAnimation = eSMD.VertexAnimation;

            var stream = new FileInfo(Path.ChangeExtension(editedFilePath, ".changed.smd")).CreateText();
            SMD_WRITER.SmdWriter.WriteSMD(newSMD, stream);
            stream.WriteLine("// SMD_SKELETON_CHANGER");
            stream.WriteLine("// By: JADERLINK");
            stream.WriteLine("// youtube.com/@JADERLINK");
            stream.Write("// github.com/JADERLINK");
            stream.Close();

        }

    }
}
