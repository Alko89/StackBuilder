#region Using directives
using System.Linq;
using System.Collections.Generic;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Heterogeneous solution
    /// </summary>
    public class HSolution
    {
        public HSolution(string algo) { Algorithm = algo; }
        public IEnumerable<HSolItem> SolItems { get => hSolItems; }
        public IEnumerable<HUnloadedElt> UnloadedElts { get => hUnloadedElts; }
        public HAnalysis Analysis { get; set; }
        public HSolItem CreateSolItem() { hSolItems.Add(new HSolItem()); return hSolItems[hSolItems.Count - 1]; }
        public string Algorithm { get; private set; } = string.Empty;
        public int LoadedCasesCount
        {
            get
            {
                int iCount = 0;
                foreach (HSolItem solItem in SolItems)
                    iCount += solItem.Count;
                return iCount;
            }
        }
        public string LoadedCasesCountString
        {
            get
            {
                string sRes = string.Empty;
                int iCount = 0;
                foreach (HSolItem solItem in SolItems)
                {
                    if (0 != iCount) sRes += "+";
                    sRes += string.Format("{0}", solItem.Count);
                    iCount += solItem.Count;
                }
                if (SolItemCount > 1)
                    sRes += string.Format(" = {0}", iCount);
                return sRes;
            }
        }
        public int UnloadedCasesCount => UnloadedElts.ToList().Count;
        public double LoadedVolumePercentage
        {
            get
            {
                return 100.0;
            }
        }
        public bool HasSolItems => hSolItems.Count > 0;
        public int SolItemCount => hSolItems.Count;
        public HSolItem SolItem(int itemIndex) => hSolItems[itemIndex];
        public virtual BBox3D BBoxGlobal(int solItemIndex)
        {
            BBox3D bbox = new BBox3D();
            bbox.Extend(Analysis.AdditionalBoudingBox(solItemIndex));
            bbox.Extend( BBoxLoad(solItemIndex));
            return bbox;
        }
        public virtual BBox3D BBoxLoad(int solItemIndex)
        {
            return SolItem(solItemIndex).BBox(this);
        }
        private readonly List<HSolItem> hSolItems = new List<HSolItem>();
        private readonly List<HUnloadedElt> hUnloadedElts = new List<HUnloadedElt>();
    }
    /// <summary>
    /// Heterogeneous solution item
    /// </summary>
    public class HSolItem
    {
        public int ContainerType { get; set; }
        public IEnumerable<HSolElement> ContainedElements { get => hSolElt; }

        public void InsertContainedElt(int contentType, BoxPosition bPosition)
        {
            hSolElt.Add(new HSolElement() { ContentType = contentType, Position = bPosition });
        }
        public int Count => hSolElt.Count;
        public Vector3D LoadDimensions { get; set; }
        public Vector3D TotalDimensions { get; set; }
        public BBox3D BBox(HSolution sol)
        {
            HAnalysis analysis = sol.Analysis;
            BBox3D bbox = new BBox3D();
            foreach (var solElt in ContainedElements)
                bbox.Extend(solElt.Position.BBox(analysis.ContentTypeByIndex(solElt.ContentType).OuterDimensions));
            return bbox;
        }
        private readonly List<HSolElement> hSolElt = new List<HSolElement>();
    }
    /// <summary>
    /// Solution element
    /// </summary>
    public class HSolElement
    {
        public int ContentType { get; set; }
        public BoxPosition Position { get; set; }
    }
    /// <summary>
    /// Unloaded element
    /// </summary>
    public class HUnloadedElt
    {
        public int ContentType { get; set; }
        public int Number { get; set; }
    }
}
