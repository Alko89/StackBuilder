#region Using directives
using System;
using System.IO;
using System.Reflection;
#endregion

namespace Sharp3D.Boxologic.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BoxItem[] boxItem = {
                    new BoxItem() { ID=1, Boxx = 190, Boxy = 200, Boxz = 420, N = 2 },
                    new BoxItem() { ID=2, Boxx = 250, Boxy = 200, Boxz = 300, N = 1 },
                    new BoxItem() { ID=3, Boxx = 250, Boxy = 200, Boxz = 250, N = 1 },
                    new BoxItem() { ID=4, Boxx = 250, Boxy = 200, Boxz = 290, N = 1 },
                    new BoxItem() { ID=5, Boxx = 80, Boxy = 200, Boxz = 210, N = 4 },
                    new BoxItem() { ID=6, Boxx = 360, Boxy = 460, Boxz = 840, N = 1 },
                    new BoxItem() { ID=7, Boxx = 160, Boxy = 460, Boxz = 100, N = 2 },
                    new BoxItem() { ID=8, Boxx = 160, Boxy = 460, Boxz = 320, N = 2 },
                    new BoxItem() { ID=9, Boxx = 200, Boxy = 300, Boxz = 150, N = 1 },
                    new BoxItem() { ID=10, Boxx = 200, Boxy = 300, Boxz = 690, N = 1 },
                    new BoxItem() { ID=11, Boxx = 200, Boxy = 300, Boxz = 210, N = 4 },
                    new BoxItem() { ID=12, Boxx = 120, Boxy = 300, Boxz = 70, N = 12 },
                    new BoxItem() { ID=13, Boxx = 520, Boxy = 600, Boxz = 420, N = 2 },
                    new BoxItem() { ID=14, Boxx = 260, Boxy = 360, Boxz = 210, N = 4 },
                    new BoxItem() { ID=15, Boxx = 260, Boxy = 360, Boxz = 840, N = 1 }
                };

                Boxlogic bl = new Boxlogic
                {
                    OutputFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.txt")
                };
                SolutionArray solArray = new SolutionArray();
                bl.Run(boxItem, 1040, 960, 840, ref solArray);

                Console.Write(solArray.ToString());
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
