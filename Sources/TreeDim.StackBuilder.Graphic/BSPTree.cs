#region Using directives
using System;
using System.Collections.Generic;

using log4net;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class BSPTree
    {
        public BSPTree()
        {
            Initialize();
        }
        public void Initialize()
        {
            Root = null;
        }
        public void Insert(Triangle f)
        {
            if (f.IsFlat) return;

            if (null == Root)
                Root = new BSPNodeTri(f);
            else
                Root.Insert(f);
        }
        public void InsertBox(Box b)
        {
            foreach (Triangle tr in b.Triangles)
                Insert(tr);
        }

        public void Draw(Graphics3D g)
        {
            if (null == Root)
                return;
            List<Triangle> triangles = new List<Triangle>();
            Draw(Root, g.CameraPosition, ref triangles);
            foreach (var tr in triangles)
                g.AddTriangle(tr);
        }

        private void Draw(BSPNodeTri node, Vector3D ptEye, ref List<Triangle> triangles)
        {
            if (null == node) return;
            double result = node.ClassifyPoint(ptEye);
            if (result < 0)
            {
                Draw(node.NodeLeft, ptEye, ref triangles);
                triangles.Add(node.Triangle);
                Draw(node.NodeRight, ptEye, ref triangles);
            }
            else if (result > 0)
            {
                Draw(node.NodeRight, ptEye, ref triangles);
                triangles.Add(node.Triangle);
                Draw(node.NodeLeft, ptEye, ref triangles);
            }
            else // result == 0
            {
                Draw(node.NodeRight, ptEye, ref triangles);
                Draw(node.NodeLeft, ptEye, ref triangles);
            }
        }

        private BSPNodeTri Root { get; set; }
        private static readonly ILog _log = LogManager.GetLogger(typeof(BSPTree));
    }
}
