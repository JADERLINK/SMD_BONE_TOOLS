using System;
using System.Collections.Generic;
using System.Text;
using SMD_READER_LIB;
using System.IO;

namespace SMD_WRITER
{
    public static class SmdWriter
    {
        public static void WriteSMD(SMD smd, TextWriter text) 
        {
            text.WriteLine("version 1");
            text.WriteLine("nodes");

            foreach (var node in smd.Nodes)
            {
                text.WriteLine(node.ID.ToIntString() + " \"" + node.BoneName + "\" " + node.ParentID.ToIntString());
            }

            text.WriteLine("end");

            text.WriteLine("skeleton");

            foreach (var time in smd.Times)
            {
                text.WriteLine("time " + time.ID.ToIntString());

                foreach (var skeleton in time.Skeletons)
                {
                    text.WriteLine(skeleton.BoneID.ToIntString() + "  " +
                                   skeleton.PosX.ToFloatString() + " " +
                                   skeleton.PosY.ToFloatString() + " " +
                                   skeleton.PosZ.ToFloatString() + "  " +
                                   skeleton.RotX.ToFloatString() + " " +
                                   skeleton.RotY.ToFloatString() + " " +
                                   skeleton.RotZ.ToFloatString());
                }
            }
          
            text.WriteLine("end");

            text.WriteLine("triangles");

            foreach (var tri in smd.Triangles)
            {
                text.WriteLine(tri.Material);

                foreach (var vertex in tri.Vertexs)
                {
                    text.Write(vertex.ParentBone.ToIntString() + "  " +
                               vertex.PosX.ToFloatString() + " " +
                               vertex.PosY.ToFloatString() + " " +
                               vertex.PosZ.ToFloatString() + "  " +
                               vertex.NormX.ToFloatString() + " " +
                               vertex.NormY.ToFloatString() + " " +
                               vertex.NormZ.ToFloatString() + "  " +
                               vertex.U.ToFloatString() + " " +
                               vertex.V.ToFloatString() + " "
                               );

                    if (vertex.Links.Count == 0)
                    {
                        text.WriteLine(" 0");
                    }
                    else
                    {
                        text.Write(" " + vertex.Links.Count.ToIntString());

                        foreach (var weightMap in vertex.Links)
                        {
                            text.Write(" " + weightMap.BoneID.ToIntString() + " " + weightMap.Weight.ToFloatString());
                        }
                        text.WriteLine();
                    }
                }
            }

            text.WriteLine("end");

            if (smd.VertexAnimation != null && smd.VertexAnimation.Count != 0)
            {
                text.WriteLine("vertexanimation");

                foreach (var time in smd.VertexAnimation)
                {
                    text.WriteLine("time " + time.ID.ToIntString());

                    foreach (var vertex in time.Vextexs)
                    {
                        text.Write(vertex.VertexID.ToIntString() + "  " +
                                   vertex.PosX.ToFloatString() + " " +
                                   vertex.PosY.ToFloatString() + " " +
                                   vertex.PosZ.ToFloatString() + "  " +
                                   vertex.NormX.ToFloatString() + " " +
                                   vertex.NormY.ToFloatString() + " " +
                                   vertex.NormZ.ToFloatString()
                                   );
                    }
                }

                text.WriteLine("end");
            }
         
        }

    }

    public static class NunExtensions
    {
        public static string ToFloatString(this float value)
        {
            string s = value.ToString("F9", System.Globalization.CultureInfo.InvariantCulture);
            s = s.TrimEnd('0');
            s = s.EndsWith(".") ? s + '0' : s;
            return s;
        }

        public static string ToIntString(this int value)
        {
            return value.ToString("D1", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

}
