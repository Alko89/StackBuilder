#region Using directives
using System;
using System.Collections.Generic;

using Sharp3D.Math.Core;
using Sharp3DBinPacking;
using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Engine
{
    public class HSolver : IHSolver
    {
        public List<HSolution> BuildSolutions(HAnalysis analysis)
        {
            List<ContentItem> contentItems = new List<ContentItem>(analysis.Content);
            // *** Sharp3DBinPacking : begin
            // create cuboid list
            List<Cuboid> listCuboids = new List<Cuboid>();
            bool bAllowAllOrientations = true;
            foreach (ContentItem ci in contentItems)
            {
                for (int i = 0; i < ci.Number; ++i)
                {
                    if (ci.Pack is BoxProperties b)
                        listCuboids.Add(
                            new Cuboid((decimal)b.Length, (decimal)b.Width, (decimal)b.Height)
                            {
                                Tag = b,
                                AllowOrientX = ci.AllowOrientX,
                                AllowOrientY = ci.AllowOrientY,
                                AllowOrientZ = ci.AllowOrientZ
                            }
                        );
                    if (!ci.AllowOrientX || !ci.AllowOrientY || !ci.AllowOrientZ)
                        bAllowAllOrientations = false;
                }
            }
            // dim container + offset
            Vector3D dimContainer = analysis.DimContainer(0), offset = analysis.Offset(0);

            // Create a bin packer instance
            // The default bin packer will test all algorithms and try to find the best result
            // BinPackerVerifyOption is used to avoid bugs, it will check whether the result is correct
            var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly, bAllowAllOrientations);
            // The result contains bins which contains packed cuboids whith their coordinates
            var parameter = new BinPackParameter(
                (decimal)dimContainer.X, (decimal)dimContainer.Y, (decimal)dimContainer.Z,
                listCuboids.ToArray())
            {
            };

            var binPackResult = binPacker.Pack(parameter);

            List<HSolution> solutions = new List<HSolution>();
            //foreach (var result in binPackResult.BestResult)
            //{
            HSolution sol = new HSolution("") { Analysis = analysis };
            foreach (var bins in binPackResult.BestResult)
            {
                HSolItem hSolItem = sol.CreateSolItem();
                foreach (var cuboid in bins)
                {
                    CuboidToSolItem(contentItems, offset, cuboid, out int index, out BoxPosition pos);
                    hSolItem.InsertContainedElt(index, pos);
                }
            }
            solutions.Add(sol);
            //}
            // *** Sharp3DBinPacking : end

            return solutions;
        }

        private bool CuboidToSolItem(List<ContentItem> contentItems, Vector3D offset, Cuboid cuboid, out int index, out BoxPosition pos)
        {
            index = 0;
            pos = BoxPosition.Zero;

            if (cuboid.Tag is BoxProperties bProperties)
            {
                try
                {
                    index = contentItems.FindIndex(ci => ci.MatchDimensions((double)cuboid.Width, (double)cuboid.Depth, (double)cuboid.Height));
                    pos = BoxPosition.FromPositionDimension(
                        new Vector3D((double)cuboid.X, (double)cuboid.Y, (double)cuboid.Z) + offset,
                        new Vector3D((double)cuboid.Width, (double)cuboid.Height, (double)cuboid.Depth),
                        new Vector3D(bProperties.Length, bProperties.Width, bProperties.Height));

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            return false;
        }

        private static ILog _log = LogManager.GetLogger(typeof(HSolver));
    }
}
