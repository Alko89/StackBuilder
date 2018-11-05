using System.Collections.Generic;

namespace Sharp3D.Boxologic
{
    public class BoxItem
    {
        public decimal Boxx { get; set; }
        public decimal Boxy { get; set; }
        public decimal Boxz { get; set; }
        public int N { get; set; }
        public uint ID { get; set; }
    }
    public class Layer
    {
        public Layer() { }
        public Layer(int leval, int ldim) { LayerEval = leval; LayerDim = ldim; }
        public decimal LayerEval { get; set; }
        public decimal LayerDim { get; set; }
        public override string ToString()
        {
            return string.Format("{0,5:D}, {1,5:D}", LayerDim, LayerEval);
        }
    }
    public class LayerComparer : IComparer<Layer>
    {
        public int Compare(Layer layer0, Layer layer1)
        {
            if (layer0.LayerEval > layer1.LayerEval) return 1;
            else if (layer0.LayerEval < layer1.LayerEval) return -1;
            else return 0;
        }
    }
    public class Scrappad
    {
        public Scrappad() { }
        public Scrappad prev, next;
        public decimal Cumx { get; set; }
        public decimal Cumz { get; set; }
    }
}
