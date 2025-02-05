using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMD_READER_LIB;

namespace SMD_REPLACE_BONE
{
    internal static class SmdReplaceBone
    {
        public static SMD Action(SMD eSMD, ReplaceBone replace) 
        {

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
                if (!oldIdNewId.ContainsKey(item.Value))
                {
                    if (replace.ReplaceDic.ContainsKey(item.Key))
                    {
                        var newId = replace.ReplaceDic[item.Key].BoneID;

                        oldIdNewId.Add(item.Value, newId);
                    }
                }
            }

            // new node
            List<Node> NewBones = new List<Node>();
            List<string> usedBoneName = new List<string>();

            foreach (var item in editedOrder)
            {
                if (!usedBoneName.Contains(item.Key) && replace.ReplaceDic.ContainsKey(item.Key))
                {
                    int newBoneID = oldIdNewId[item.Value];
                    int newParent = replace.ReplaceDic[item.Key].ParentID;
                    string newName = replace.ReplaceDic[item.Key].outName;

                    Console.WriteLine("OldName: \"" + (editedBoneName[item.Key] + "\"").PadRight(NameConsolePad + 1) +
                        "  NewName: \"" + (newName + "\"").PadRight(replace.OutNameConsolePad + 1) + 
                        "  BoneID: " +item.Value.ToString().PadLeft(3) + " => " + newBoneID.ToString().PadLeft(3) +
                        "  ParentID: " + editedParent[item.Key].ToString().PadLeft(3) + " => " + newParent.ToString().PadLeft(3));

                    NewBones.Add(new Node(newBoneID, newName, newParent));
                    usedBoneName.Add(item.Key);
                }
            }

            foreach (var item in replace.ReplaceDic)
            {
                if (!usedBoneName.Contains(item.Key))
                {
                    int newBoneID = replace.ReplaceDic[item.Key].BoneID;
                    int newParent = replace.ReplaceDic[item.Key].ParentID;
                    string newName = replace.ReplaceDic[item.Key].outName;

                    Console.WriteLine("NewName: \"" + (newName + "\"").PadRight(replace.OutNameConsolePad + 1) +
                        "  BoneID: " + newBoneID.ToString().PadLeft(3) +
                        "  ParentID: " + newParent.ToString().PadLeft(3));

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

                        Skeleton newSkeleton = new Skeleton(newBoneId);
                        newSkeleton.PosX = skeleton.PosX;
                        newSkeleton.PosY = skeleton.PosY;
                        newSkeleton.PosZ = skeleton.PosZ;
                        newSkeleton.RotX = skeleton.RotX;
                        newSkeleton.RotY = skeleton.RotY;
                        newSkeleton.RotZ = skeleton.RotZ;
                        newskeletons.Add(newSkeleton);
                    } 
                }

                Time newtime = new Time(time.ID);
                newtime.Skeletons = newskeletons;
                newTime.Add(newtime);
            }

            
            {
                var find = newTime.Where(x => x.ID == 0).FirstOrDefault();

                if (find != null)
                {
                    foreach (var item in replace.ReplaceDic)
                    {
                        var find2 = find.Skeletons.Where(x => x.BoneID == item.Value.BoneID).FirstOrDefault();

                        if (find2 == null)
                        {
                            Skeleton skeleton = new Skeleton(item.Value.BoneID);
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
                    nv.ParentBone = newBoneId;

                    nv.Links = new List<WeightMap>();

                    foreach (var weightMap in vertex.Links)
                    {
                        int newWeightBoneId = 0;
                        if (oldIdNewId.ContainsKey(weightMap.BoneID))
                        {
                            newWeightBoneId = oldIdNewId[weightMap.BoneID];
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
            return newSMD;
        }

    }
}
