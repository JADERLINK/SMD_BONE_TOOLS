using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMD_READER_LIB;

namespace SMD_FIX_BONE_ID
{
    internal static class FixSmdBoneId
    {
        public static void Action(string editedFilePath) 
        {
            SMD eSMD = null;
            StreamReader eStream = null;

            try
            {
                var eFile = new FileInfo(editedFilePath);
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
                if (eStream != null)
                {
                    eStream.Close();
                }
            }

            int lastNextOriginalID = 0;
            int NameConsolePad = 0;

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
                    var split = item.Key.Split('_');
                    if (item.Key.StartsWith("bone_") && split.Length == 2 && int.TryParse(split[1], out int newId))
                    {
                        oldIdNewId.Add(item.Value, newId);
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

            var stream = new FileInfo(Path.ChangeExtension(editedFilePath, ".fixed.smd")).CreateText();
            SMD_WRITER.SmdWriter.WriteSMD(newSMD, stream);
            stream.WriteLine("// SMD_FIX_BONE_ID");
            stream.WriteLine("// By: JADERLINK");
            stream.WriteLine("// youtube.com/@JADERLINK");
            stream.Write("// github.com/JADERLINK");
            stream.Close();
        }
    }
}
