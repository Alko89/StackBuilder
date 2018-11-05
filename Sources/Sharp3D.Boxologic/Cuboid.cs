using System;
using System.IO;

namespace Sharp3D.Boxologic
{
    public class Cuboid : St_instance
    {
        public void SetPacked(decimal cboxx, decimal cboxy, decimal cboxz)
        {
            Is_packed = true;
            X = cboxx;
            Y = cboxy;
            Z = cboxz;
        }
        public bool Is_packed { get; set; }
        public decimal Cox { get; set; }
        public decimal Coy { get; set; }
        public decimal Coz { get; set; }
        public int N { get; set; }
        public uint ID { get; set; }
        public void Write(int index)
        {
            if (Is_packed)
                Console.WriteLine(string.Format("{0,5:D} {1:0.#} {2:0.#} {3:0.#} {4:0.#} {5:0.#} {6:0.#}", index, Cox, Coy, Coz, X, Y, Z));
            else
                Console.WriteLine(string.Format("{0,5:D} {1:0.#} {2:0.#} {3:0.#}", index, Cox, Coy, Coz));
        }
        public void WriteToFile(StreamWriter file, int variant, int index)
        {
            decimal x = 0.0M, y = 0.0M, z = 0.0M;
            decimal bx = 0.0M, by = 0.0M, bz = 0.0M;
            switch (variant)
            {
                case 1:
                    x = Cox;
                    y = Coy;
                    z = Coz;
                    bx = X;
                    by = Y;
                    bz = Z;
                    break;
                case 2:
                    x = Coz;
                    y = Coy;
                    z = Cox;
                    bx = Z;
                    by = Y;
                    bz = X;
                    break;
                case 3:
                    x = Coy;
                    y = Coz;
                    z = Cox;
                    bx = Y;
                    by = Z;
                    bz = X;
                    break;
                case 4:
                    x = Coy;
                    y = Cox;
                    z = Coz;
                    bx = Y;
                    by = X;
                    bz = Z;
                    break;
                case 5:
                    x = Cox;
                    y = Coz;
                    z = Coy;
                    bx = X;
                    by = Z;
                    bz = Y;
                    break;
                case 6:
                    x = Coz;
                    y = Cox;
                    z = Coy;
                    bx = Z;
                    by = X;
                    bz = Y;
                    break;
                default:
                    break;
            }
            file.WriteLine(
                string.Format(
                "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                index,
                Is_packed ? 1 : 0,
                Dim1, Dim2, Dim3,
                x, y, z,
                bx, by, bz
                )
                );
            Cox = x;
            Coy = y;
            Coz = z;
            X = bx;
            Y = by;
            Z = bz;
        }

        public SolItem ToSolItem(int variant)
        {
            decimal x = 0.0M, y = 0.0M, z = 0.0M;
            decimal bx = 0.0M, by = 0.0M, bz = 0.0M;
            switch (variant)
            {
                case 1:
                    x = Cox;
                    y = Coy;
                    z = Coz;
                    bx = X;
                    by = Y;
                    bz = Z;
                    break;
                case 2:
                    x = Coz;
                    y = Coy;
                    z = Cox;
                    bx = Z;
                    by = Y;
                    bz = X;
                    break;
                case 3:
                    x = Coy;
                    y = Coz;
                    z = Cox;
                    bx = Y;
                    by = Z;
                    bz = X;
                    break;
                case 4:
                    x = Coy;
                    y = Cox;
                    z = Coz;
                    bx = Y;
                    by = X;
                    bz = Z;
                    break;
                case 5:
                    x = Cox;
                    y = Coz;
                    z = Coy;
                    bx = X;
                    by = Z;
                    bz = Y;
                    break;
                case 6:
                    x = Coz;
                    y = Cox;
                    z = Coy;
                    bx = Z;
                    by = X;
                    bz = Y;
                    break;
                default:
                    break;
            }

            return new SolItem()
            {
                Id = ID
                ,
                X = x,
                Y = y,
                Z = z
                ,
                BX = bx,
                BY = by,
                BZ = bz
                ,
                DimX = Dim1,
                DimY = Dim2,
                DimZ = Dim3
            };
        }
    }
}