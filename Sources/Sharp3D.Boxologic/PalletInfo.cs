namespace Sharp3D.Boxologic
{
    public class PalletInfo : St_instance
    {
        public PalletInfo(int dim1, int dim2, int dim3)
        { Dim1 = dim1; Dim2 = dim2; Dim3 = dim3; }
        public int Variant { get; set; }
        public decimal Pallet_x
        {
            get
            {
                switch (Variant)
                {
                    case 1: return Dim1;
                    case 2: return Dim3;
                    case 3: return Dim3;
                    case 4: return Dim2;
                    case 5: return Dim1;
                    case 6: return Dim2;
                    default: return 0;
                }
            }
        }
        public decimal Pallet_y
        {
            get
            {
                switch (Variant)
                {
                    case 1: return Dim2;
                    case 2: return Dim2;
                    case 3: return Dim1;
                    case 4: return Dim1;
                    case 5: return Dim3;
                    case 6: return Dim3;
                    default: return 0;
                }
            }
        }
        public decimal Pallet_z
        {
            get
            {
                switch (Variant)
                {
                    case 1: return Dim3;
                    case 2: return Dim1;
                    case 3: return Dim2;
                    case 4: return Dim3;
                    case 5: return Dim2;
                    case 6: return Dim1;
                    default: return 0;
                }
            }
        }
    }
}
