namespace Sharp3D.Boxologic
{
    public class St_instance
    {
        public decimal Dim1 { get; set; }
        public decimal Dim2 { get; set; }
        public decimal Dim3 { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }
        public decimal Vol => Dim1 * Dim2 * Dim3;
    }
}