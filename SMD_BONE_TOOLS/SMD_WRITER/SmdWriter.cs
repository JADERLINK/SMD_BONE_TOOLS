/*
 MIT License

Copyright (c) 2025 JADERLINK

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using SMD_READER_LIB;
using System.IO;

namespace SMD_WRITER_LIB
{
    // Codigo escrito por JADERLINK
    //https://github.com/JADERLINK
    // Referencia do formato SMD
    //https://developer.valvesoftware.com/wiki/SMD
    // Versão do codigo: 1.1.2

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

            if (smd.Triangles != null && smd.Triangles.Count > 0)
            {
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
            }

    

            if (smd.VertexAnimation != null && smd.VertexAnimation.Count != 0)
            {
                text.WriteLine("vertexanimation");

                foreach (var time in smd.VertexAnimation)
                {
                    text.WriteLine("time " + time.ID.ToIntString());

                    foreach (var vertex in time.Vextexs)
                    {
                        text.WriteLine(
                             vertex.VertexID.ToIntString() + "  " +
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

        private static string ToFloatString(this float value)
        {
            string raw = value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            if (raw.IndexOf('E') >= 0 || raw.IndexOf('e') >= 0)
            {
                string s = value.ToString("F9", System.Globalization.CultureInfo.InvariantCulture);
                int end = s.Length - 1;
                while (end > 0 && s[end] == '0' && s[end - 1] != '.')
                {
                    end--;
                }
                if (s[end] == '.')
                {
                    return s.Substring(0, end + 2);
                }
                return s.Substring(0, end + 1);
            }
            if (raw.IndexOf('.') < 0)
            {
                return raw + ".0";
            }
            return raw;
        }

        private static string ToIntString(this int value)
        {
            return value.ToString("D1", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
