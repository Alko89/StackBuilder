#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
#endregion

namespace Sharp3D.Boxologic
{
    public class Boxlogic
    {
        #region Private data members
        private DateTime _timeStart, _timeStop;
        private PalletInfo pallet;
        private List<Cuboid> boxList = new List<Cuboid>();
        private List<Layer> layers = new List<Layer>();
        private Scrappad scrapfirst, smallestz;
        private Scrappad trash;
        private Dictionary<KeyValuePair<int, int>, decimal> best_iterations = new Dictionary<KeyValuePair<int, int>, decimal>();

        private bool packing = true, layerdone = false, evened = false;
        private int best_variant;
        private decimal layerinlayer;
        private decimal prelayer;
        private decimal lilz;
        private int number_of_iterations;
        private decimal remainpy, preremainpy, remainpz;
        private decimal packedy, prepackedy;
        private decimal layerThickness;
        private int best_iteration;
        private int packednumbox;
        private int number_packed_boxes;
        private decimal packedvolume;
        private decimal best_solution_volume;
        private decimal total_box_volume;
        private double pallet_volume_used_percentage;
        private StreamWriter fso;
        #endregion
        #region Public properties
        public string OutputFilePath { get; set; }
        #endregion
        #region Run / Initialize / Execute_iterations
        public void Run(BoxItem[] listBoxItem, int palletLength, int palletWidth, int palletHeight, ref SolutionArray solArray)
        {
            packing = true;

            // boxes
            foreach (BoxItem bi in listBoxItem)
            {
                for (int i = 0; i < bi.N; ++i)
                    boxList.Add(new Cuboid() { ID= bi.ID, Dim1 = bi.Boxx, Dim2 = bi.Boxy, Dim3 = bi.Boxz, N = bi.N });
            }
            // pallet
            pallet = new PalletInfo(palletLength, palletWidth, palletHeight);
            // output file
            fso = new StreamWriter(OutputFilePath, true);

            _timeStart = DateTime.Now;
            Initialize();
            Execute_iterations();
            _timeStop = DateTime.Now;
            Report_results(ref solArray);
        }
        private void Initialize()
        {
            total_box_volume = 0;
            foreach (Cuboid bi in boxList)
            { total_box_volume += bi.Vol; }

            scrapfirst = new Scrappad();
            best_solution_volume = 0;
            number_of_iterations = 0;
        }
        public void Execute_iterations()
        {
            bool hundredpercent = false;
            for (int variant = 0; variant < 6; ++variant)
            {
                // initialize pallet
                pallet.Variant = variant;
                // LISTS ALL POSSIBLE LAYER HEIGHTS BY GIVING A WEIGHT VALUE TO EACH OF THEM.
                List_candidate_layers(false);


                int iLayerIndex = 0, itelayer = 0;
                foreach (Layer l in layers)
                {
                    ++number_of_iterations;
                    double elapsed_time = (DateTime.Now - _timeStart).TotalSeconds;
                    Console.WriteLine(
                        string.Format("VARIANT: {0,5:#####}; ITERATION (TOTAL): {1,5:#####}; BEST SO FAR: {2:0.000}; TIME: {3:0.00}"
                            , variant, number_of_iterations, pallet_volume_used_percentage, elapsed_time));
                    packedvolume = 0;
                    packedy = 0;
                    packing = true;
                    layerThickness = l.LayerDim;
                    itelayer = iLayerIndex++;
                    remainpy = pallet.Pallet_y;
                    remainpz = pallet.Pallet_z;
                    packednumbox = 0;
                    foreach (Cuboid bi in boxList)
                        bi.Is_packed = false;

                    // ### BEGIN DO-WHILE
                    do
                    {
                        layerinlayer = 0;
                        layerdone = false;
                        Pack_layer(false, ref hundredpercent);
                        packedy = packedy + layerThickness;
                        remainpy = pallet.Pallet_y - packedy;
                        if (0 != layerinlayer)
                        {
                            prepackedy = packedy;
                            preremainpy = remainpy;
                            remainpy = layerThickness - prelayer;
                            packedy = packedy - layerThickness + prelayer;
                            remainpz = lilz;
                            layerThickness = layerinlayer;
                            layerdone = false;
                            Pack_layer(false, ref hundredpercent);
                            packedy = prepackedy;
                            remainpy = preremainpy;
                            remainpz = pallet.Pallet_z;
                        }
                        //System.Diagnostics.Debug.Assert(remainpy >= 0);
                        Find_layer(remainpy, pallet);
                    }
                    while (packing);
                    // END DO-WHILE

                    if (packedvolume >= best_solution_volume)
                    {
                        best_solution_volume = packedvolume;
                        best_variant = variant;
                        best_iteration = itelayer;
                        number_packed_boxes = packednumbox;

                        best_iterations[new KeyValuePair<int, int>(variant, itelayer)] = packedvolume;
                    }
                    if (hundredpercent) break;
                    pallet_volume_used_percentage = (double)best_solution_volume * 100 / (double)pallet.Vol;
                }
                if (hundredpercent) break;
                if ((pallet.Dim1 == pallet.Dim2) && (pallet.Dim2 == pallet.Dim3)) variant = 5;
            }
        }
        #endregion
        #region List_candidate_layers (DONE)
        public void List_candidate_layers(bool show)
        {
            foreach (Cuboid bi1 in boxList)
            {
                for (int y = 1; y <= 3; ++y)
                {
                    decimal exdim = 0, dimen2 = 0, dimen3 = 0;
                    switch (y)
                    {
                        case 1:
                            exdim = bi1.Dim1;
                            dimen2 = bi1.Dim2;
                            dimen3 = bi1.Dim3;
                            break;
                        case 2:
                            exdim = bi1.Dim2;
                            dimen2 = bi1.Dim1;
                            dimen3 = bi1.Dim3;
                            break;
                        case 3:
                            exdim = bi1.Dim3;
                            dimen2 = bi1.Dim1;
                            dimen3 = bi1.Dim2;
                            break;
                        default:
                            break;
                    }

                    if (
                        (exdim > pallet.Pallet_y)
                        ||
                        (
                            ((dimen2 > pallet.Pallet_x) || (dimen3 > pallet.Pallet_z))
                            && ((dimen3 > pallet.Pallet_x) || (dimen2 > pallet.Pallet_z))
                        )
                    )
                        continue;

                    if (null != layers.Find(lay => lay.LayerDim == exdim))
                        continue;

                    decimal layereval = 0;
                    foreach (Cuboid bi2 in boxList)
                    {
                        if (bi1 != bi2)
                        {
                            decimal dimdif = Math.Abs(exdim - bi2.Dim1);
                            if (Math.Abs(exdim - bi2.Dim2) < dimdif)
                                dimdif = Math.Abs(exdim - bi2.Dim2);
                            if (Math.Abs(exdim - bi2.Dim3) < dimdif)
                                dimdif = Math.Abs(exdim - bi2.Dim3);
                            layereval = layereval + dimdif;
                        }
                    }
                    layers.Add(new Layer() { LayerEval = layereval, LayerDim = exdim });
                }
            }
            layers.Sort(new LayerComparer());

            if (show)
            {
                foreach (Layer l in layers)
                    Console.WriteLine(l.ToString());
            }
        }
        #endregion
        #region Find_smallest_z (DONE)
        //----------------------------------------------------------------------------
        // FINDS THE FIRST TO BE PACKED GAP IN THE LAYER EDGE
        //----------------------------------------------------------------------------
        private Scrappad Find_smallest_z()
        {
            Scrappad scrapmemb = scrapfirst;
            Scrappad sz = scrapfirst;
            while (null != scrapmemb.next)
            {
                if (scrapmemb.next.Cumz < sz.Cumz)
                    sz = scrapmemb.next;
                scrapmemb = scrapmemb.next;
            }
            return sz;
        }
        #endregion
        #region Pack_layer (DONE)
        ///----------------------------------------------------------------------------
        /// PACKS THE BOXES FOUND AND ARRANGES ALL VARIABLES AND RECORDS PROPERLY
        ///----------------------------------------------------------------------------
        private bool Pack_layer(bool packingbest, ref bool hundredpercent)
        {
            if (0 == layerThickness)
            {
                packing = false;
                return false;
            }

            scrapfirst = new Scrappad()
            {
                Cumx = pallet.Pallet_x,
                Cumz = 0
            };
            decimal cboxi = 0, cboxx = 0, cboxy = 0, cboxz = 0;
            while (true)
            {
                smallestz = Find_smallest_z();

                if (null == smallestz.prev && null == smallestz.next)
                {
                    //*** SITUATION-1: NO BOXES ON THE RIGHT AND LEFT SIDES ***
                    decimal lenx = smallestz.Cumx;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(lenx, layerThickness, remainpy, lpz, lpz,
                        ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    Cuboid bi = boxList[(int)cboxi];
                    boxList[(int)cboxi].Cox = 0;
                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx)
                    {
                        smallestz.Cumz = smallestz.Cumz + cboxz;
                    }
                    else
                    {
                        smallestz.next = new Scrappad
                        {
                            prev = smallestz,
                            Cumx = smallestz.Cumx,
                            Cumz = smallestz.Cumz
                        };
                        smallestz.Cumx = cboxx;
                        smallestz.Cumz = smallestz.Cumz + cboxz;
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (null == smallestz.prev)
                {
                    //*** SITUATION-2: NO BOXES ON THE LEFT SIDE ***
                    decimal lenx = smallestz.Cumx;
                    decimal lenz = smallestz.next.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    Cuboid bi = boxList[(int)cboxi];
                    bi.Coy = packedy;
                    bi.Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx)
                    {
                        bi.Cox = 0;
                        if (smallestz.Cumz + cboxz == smallestz.next.Cumz)
                        {
                            smallestz.Cumz = smallestz.next.Cumz;
                            smallestz.Cumx = smallestz.next.Cumx;
                            trash = smallestz.next;
                            smallestz.next = smallestz.next.next;
                            if (null != smallestz.next)
                            {
                                smallestz.next.prev = smallestz;
                            }
                        }
                        else
                        {
                            smallestz.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        bi = boxList[(int)cboxi];
                        bi.Cox = smallestz.Cumx - cboxx;
                        if (smallestz.Cumz + cboxz == smallestz.next.Cumz)
                        {
                            smallestz.Cumx = smallestz.Cumx - cboxx;
                        }
                        else
                        {
                            smallestz.next.prev = new Scrappad
                            {
                                next = smallestz.next,
                                prev = smallestz
                            };
                            smallestz.next = smallestz.next.prev;
                            smallestz.next.Cumx = smallestz.Cumx;
                            smallestz.Cumx = smallestz.Cumx - cboxx;
                            smallestz.next.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (null == smallestz.next)
                {
                    //*** SITUATION-3: NO BOXES ON THE RIGHT SIDE ***
                    decimal lenx = smallestz.Cumx - smallestz.prev.Cumx;
                    decimal lenz = smallestz.prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    Cuboid bi = boxList[(int)cboxi];
                    bi.Coy = packedy;
                    bi.Coz = smallestz.Cumz;
                    bi.Cox = smallestz.prev.Cumx;

                    if (cboxx == smallestz.Cumx - smallestz.prev.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.Cumx;
                            smallestz.prev.next = null;
                        }
                        else
                        {
                            smallestz.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.prev.Cumx + cboxx;
                        }
                        else
                        {
                            smallestz.prev.next = new Scrappad
                            {
                                prev = smallestz.prev,
                                next = smallestz
                            };
                            smallestz.prev = smallestz.prev.next;
                            smallestz.prev.Cumx = smallestz.prev.prev.Cumx + cboxx;
                            smallestz.prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (smallestz.prev.Cumz == smallestz.next.Cumz)
                {
                    //*** SITUATION-4: THERE ARE BOXES ON BOTH OF THE SIDES ***
                    //*** SUBSITUATION-4A: SIDES ARE EQUAL TO EACH OTHER ***
                    decimal lenx = smallestz.Cumx - smallestz.prev.Cumx;
                    decimal lenz = smallestz.prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx - smallestz.prev.Cumx)
                    {
                        boxList[(int)cboxi].Cox = smallestz.prev.Cumx;
                        if (smallestz.Cumz + cboxz == smallestz.next.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.next.Cumx;
                            if (null != smallestz.next.next)
                            {
                                smallestz.prev.next = smallestz.next.next;
                                smallestz.next.next.prev = smallestz.prev;
                            }
                            else
                            {
                                smallestz.prev.next = null;
                            }
                        }
                        else
                        {
                            smallestz.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else if (smallestz.prev.Cumx < pallet.Pallet_x - smallestz.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.Cumx = smallestz.Cumx - cboxx;
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                        }
                        else
                        {
                            boxList[(int)cboxi].Cox = smallestz.prev.Cumx;
                            smallestz.prev.next = new Scrappad
                            {
                                prev = smallestz.prev,
                                next = smallestz
                            };
                            smallestz.prev = smallestz.prev.next;
                            smallestz.prev.Cumx = smallestz.prev.prev.Cumx + cboxx;
                            smallestz.prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.prev.Cumx + cboxx;
                            boxList[(int)cboxi].Cox = smallestz.prev.Cumx;
                        }
                        else
                        {
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                            smallestz.next.prev = new Scrappad
                            {
                                next = smallestz.next,
                                prev = smallestz
                            };
                            smallestz.next = smallestz.next.prev;
                            smallestz.next.Cumx = smallestz.Cumx;
                            smallestz.next.Cumz = smallestz.Cumz + cboxz;
                            smallestz.Cumx = smallestz.Cumx - cboxx;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else
                {
                    //*** SUBSITUATION-4B: SIDES ARE NOT EQUAL TO EACH OTHER ***
                    //*** SUBSITUATION-4B: SIDES ARE NOT EQUAL TO EACH OTHER ***
                    decimal lenx = smallestz.Cumx - smallestz.prev.Cumx;
                    decimal lenz = smallestz.prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    boxList[(int)cboxi].Cox = smallestz.prev.Cumx;
                    if (cboxx == smallestz.Cumx - smallestz.prev.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.Cumx;
                            smallestz.prev.next = smallestz.next;
                            smallestz.next.prev = smallestz.prev;
                        }
                        else
                        {
                            smallestz.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.prev.Cumz)
                        {
                            smallestz.prev.Cumx = smallestz.prev.Cumx + cboxx;
                        }
                        else if (smallestz.Cumz + cboxz == smallestz.next.Cumz)
                        {
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                            smallestz.Cumx = smallestz.Cumx - cboxx;
                        }
                        else
                        {
                            smallestz.prev.next = new Scrappad
                            {
                                prev = smallestz.prev,
                                next = smallestz
                            };
                            smallestz.prev = smallestz.prev.next;
                            smallestz.prev.Cumx = smallestz.prev.prev.Cumx + cboxx;
                            smallestz.prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
            }
            return true;
        }
        #endregion
        #region Find_layer (DONE)
        ///----------------------------------------------------------------------------
        /// FINDS THE MOST PROPER LAYER HEIGHT BY LOOKING AT THE UNPACKED BOXES AND THE
        /// REMAINING EMPTY SPACE AVAILABLE
        ///----------------------------------------------------------------------------
        private int Find_layer(decimal thickness, PalletInfo p)
        {
            decimal exdim = 0, dimdif = 0, dimen2 = 0, dimen3 = 0;
            decimal layereval = 0, eval = decimal.MaxValue;
            layerThickness = 0;
            for (int x = 0; x < boxList.Count; x++)
            {
                Cuboid bi = boxList[x];
                if (bi.Is_packed)
                    continue;
                for (int y = 1; y <= 3; y++)
                {
                    switch (y)
                    {
                        case 1:
                            exdim = bi.Dim1;
                            dimen2 = bi.Dim2;
                            dimen3 = bi.Dim3;
                            break;
                        case 2:
                            exdim = bi.Dim2;
                            dimen2 = bi.Dim1;
                            dimen3 = bi.Dim3;
                            break;
                        case 3:
                            exdim = bi.Dim3;
                            dimen2 = bi.Dim1;
                            dimen3 = bi.Dim2;
                            break;
                        default:
                            break;
                    }
                    layereval = 0;
                    if ((exdim <= thickness)
                        && (((dimen2 <= p.Pallet_x) && (dimen3 <= p.Pallet_z))
                        || ((dimen3 <= p.Pallet_x) && (dimen2 <= p.Pallet_z))))
                    {
                        for (int z = 0; z < boxList.Count; z++)
                        {
                            if (!(x == z) && !(boxList[z].Is_packed))
                            {
                                dimdif = Math.Abs(exdim - boxList[z].Dim1);
                                if (Math.Abs(exdim - boxList[z].Dim2) < dimdif)
                                    dimdif = Math.Abs(exdim - boxList[z].Dim2);
                                if (Math.Abs(exdim - boxList[z].Dim3) < dimdif)
                                    dimdif = Math.Abs(exdim - boxList[z].Dim3);
                                layereval = layereval + dimdif;
                            }
                        }
                        if (layereval < eval)
                        {
                            eval = layereval;
                            layerThickness = exdim;
                        }
                    }
                }
            }
            if (layerThickness == 0 || layerThickness > remainpy)
                packing = false;
            return 0;
        }
        #endregion
        #region CheckFound (DONE)
        /// <summary>
        /// AFTER FINDING EACH BOX, THE CANDIDATE BOXES AND THE CONDITION OF THE LAYER
        /// ARE EXAMINED
        /// </summary>
        private void CheckFound(
            ref decimal cboxi, ref decimal cboxx, ref decimal cboxy, ref decimal cboxz
            , decimal boxi, decimal boxx, decimal boxy, decimal boxz
            , decimal bboxi, decimal bboxx, decimal bboxy, decimal bboxz)
        {
            evened = false;
            if (boxi >= 0 && 0 != boxx * boxy *boxz)
            {
                cboxi = boxi;
                cboxx = boxx;
                cboxy = boxy;
                cboxz = boxz;
            }
            else
            {
                if ((bboxi >= 0 && 0 != bboxx * bboxy * bboxz) && (layerinlayer > 0 || (null == smallestz.prev && null == smallestz.next)))
                {
                    if (layerinlayer == 0.0M)
                    {
                        prelayer = layerThickness;
                        lilz = smallestz.Cumz;
                    }
                    cboxi = bboxi;
                    cboxx = bboxx;
                    cboxy = bboxy;
                    cboxz = bboxz;
                    layerinlayer = layerinlayer + bboxy - layerThickness;
                    layerThickness = bboxy;
                }
                else
                {
                    if (null == smallestz.prev && null == smallestz.next)
                    {
                        layerdone = true;
                    }
                    else
                    {
                        evened = true;
                        if (null == smallestz.prev)
                        {
                            trash = smallestz.next;
                            smallestz.Cumx = smallestz.next.Cumx;
                            smallestz.Cumz = smallestz.next.Cumz;
                            smallestz.next = smallestz.next.next;
                            if (null != smallestz.next)
                            {
                                smallestz.next.prev = smallestz;
                            }
                            trash = null;
                        }
                        else if (null == smallestz.next)
                        {
                            smallestz.prev.next = null;
                            smallestz.prev.Cumx = smallestz.Cumx;
                            smallestz = null;
                        }
                        else
                        {
                            if (smallestz.prev.Cumz == smallestz.next.Cumz)
                            {
                                smallestz.prev.next = smallestz.next.next;
                                if (null != smallestz.next.next)
                                {
                                    smallestz.next.next.prev = smallestz.prev;
                                }
                                smallestz.prev.Cumx = smallestz.next.Cumx;
                                smallestz.next = null;
                                smallestz = null;
                            }
                            else
                            {
                                smallestz.prev.next = smallestz.next;
                                smallestz.next.prev = smallestz.prev;
                                if (smallestz.prev.Cumz < smallestz.next.Cumz)
                                {
                                    smallestz.prev.Cumx = smallestz.Cumx;
                                }
                                smallestz = null;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region VolumeCheck (DONE)
        /// <summary>
        /// AFTER PACKING OF EACH BOX, 100% PACKING CONDITION IS CHECKED
        /// </summary>
        private void VolumeCheck(int cboxi, decimal cboxx, decimal cboxy, decimal cboxz, bool packingbest
            , ref bool hundredpercent)
        {
            Cuboid bi = boxList[cboxi];
            bi.SetPacked(cboxx, cboxy, cboxz);
            packedvolume += bi.Vol;
            packednumbox++;
            if (packingbest)
            {
                bi.WriteToFile(fso, best_variant, 0);
            }
            else if (packedvolume == pallet.Vol || packedvolume == total_box_volume)
            {
                packing = false;
                hundredpercent = true;
            }
        }
        #endregion
        #region Find_box (DONE)
        /// <summary>
        /// FINDS THE MOST PROPER BOXES BY LOOKING AT ALL SIX POSSIBLE ORIENTATIONS,
        /// EMPTY SPACE GIVEN, ADJACENT BOXES, AND PALLET LIMITS 
        /// </summary>
        private void Find_box(decimal hmx, decimal hy, decimal hmy, decimal hz, decimal hmz,
            ref decimal cboxi, ref decimal cboxx, ref decimal cboxy, ref decimal cboxz)
        {
            decimal boxi = 0;
            decimal boxx = 0, boxy = 0, boxz = 0;
            decimal bboxi = 0;
            decimal bboxx = 0, bboxy = 0, bboxz = 0;
            decimal bfx = decimal.MaxValue, bfy = decimal.MaxValue, bfz = decimal.MaxValue;
            decimal bbfx = decimal.MaxValue, bbfy = decimal.MaxValue, bbfz = decimal.MaxValue;
            boxi = 0; bboxi = 0;

            for (int y = 0; y < boxList.Count; y += boxList[y].N)
            {
                int x = y;
                for (x = y; x < y + boxList[y].N - 1; x++)
                {
                    if (!boxList[x].Is_packed) break;
                }
                if (boxList[x].Is_packed) continue;
                if (x >= boxList.Count)
                    return;
                Cuboid bi = boxList[x];
                // 1 2 3
                Analyse_box(x, hmx, hy, hmy, hz, hmz,
                    bi.Dim1, bi.Dim2, bi.Dim3
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                if ((bi.Dim1 == bi.Dim3) && (bi.Dim3 == bi.Dim2))
                    continue;
                // 1 3 2
                Analyse_box(x, hmx, hy, hmy, hz, hmz
                    , bi.Dim1, bi.Dim3, bi.Dim2
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                // 2 1 3
                Analyse_box(x, hmx, hy, hmy, hz, hmz
                    , bi.Dim2, bi.Dim1, bi.Dim3
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                // 2 3 1
                Analyse_box(x, hmx, hy, hmy, hz, hmz
                    , bi.Dim2, bi.Dim3, bi.Dim1
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                // 3 1 2
                Analyse_box(x, hmx, hy, hmy, hz, hmz
                    , bi.Dim3, bi.Dim1, bi.Dim2
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                // 3 2 1
                Analyse_box(x, hmx, hy, hmy, hz, hmz
                    , bi.Dim3, bi.Dim2, bi.Dim1
                    , ref bfx, ref bfy, ref bfz
                    , ref bbfx, ref bbfy, ref bbfz
                    , ref boxi, ref boxx, ref boxy, ref boxz
                    , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
            }
            CheckFound(ref cboxi, ref cboxx, ref cboxy, ref cboxz
                , boxi, boxx, boxy, boxz
                , bboxi, bboxx, bboxy, bboxz);
        }
        #endregion
        #region Analyse_box
        /// <summary>
        /// ANALYZES EACH UNPACKED BOX TO FIND THE BEST FITTING ONE TO THE EMPTY SPACE
        /// GIVEN
        /// </summary>
        private void Analyse_box(decimal x, decimal hmx, decimal hy, decimal hmy, decimal hz, decimal hmz,
            decimal dim1, decimal dim2, decimal dim3
            , ref decimal bfx, ref decimal bfy, ref decimal bfz
            , ref decimal bbfx, ref decimal bbfy, ref decimal bbfz
            , ref decimal boxi, ref decimal boxx, ref decimal boxy, ref decimal boxz
            , ref decimal bboxi, ref decimal bboxx, ref decimal bboxy, ref decimal bboxz)
        {
            if (dim1 <= hmx && dim2 <= hmy && dim3 <= hmz)
            {
                if (dim2 <= hy)
                {
                    if (hy - dim2 < bfy)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                    else if (hy - dim2 == bfy && hmx - dim1 < bfx)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                    else if (hy - dim2 == bfy && hmx - dim1 == bfx && Math.Abs(hz - dim3) < bfz)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                }
                else
                {
                    if (dim2 - hy < bbfy)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                    else if (dim2 - hy == bbfy && hmx - dim1 < bbfx)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                    else if (dim2 - hy == bbfy && hmx - dim1 == bbfx && Math.Abs(hz - dim3) < bbfz)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                }
            }
        }
        #endregion
        #region Report_results
        public void Report_results(ref SolutionArray solArray)
        {
            pallet.Variant = best_variant;
            double packed_box_percentage = (double)best_solution_volume * 100 / (double)total_box_volume;
            pallet_volume_used_percentage = (double)best_solution_volume * 100 / (double)pallet.Vol;
            double elapsed_time = (_timeStop - _timeStart).TotalMilliseconds * 0.001;

            List_candidate_layers(false);
            packedvolume = 0;
            packedy = 0;
            packing = true;
            layerThickness = layers[best_iteration].LayerDim;
            remainpy = pallet.Pallet_y;
            remainpz = pallet.Pallet_z;

            foreach (Cuboid bi in boxList)
                bi.Is_packed = false;

            bool hundredpercent = false;
            do
            {
                layerinlayer = 0;
                layerdone = false;
                Pack_layer(true, ref hundredpercent);
                packedy += layerThickness;
                remainpy = pallet.Pallet_y - packedy;
                if (0 != layerinlayer)
                {
                    prepackedy = packedy;
                    preremainpy = remainpy;
                    remainpy = layerThickness - prelayer;
                    packedy = packedy - layerThickness + prelayer;
                    remainpz = lilz;
                    layerThickness = layerinlayer;
                    layerdone = false;
                    Pack_layer(true, ref hundredpercent);
                    packedy = prepackedy;
                    remainpy = preremainpy;
                    remainpz = pallet.Pallet_z;
                }
                System.Diagnostics.Debug.Assert(remainpy >= 0);
                Find_layer(remainpy, pallet);
            }
            while (packing);


            Solution sol = new Solution() { Variant = best_variant, Iteration = best_iteration };
            foreach (Cuboid bi in boxList)
            {
                if (bi.Is_packed)
                    sol.ItemsPacked.Add(bi.ToSolItem(best_variant));
                else
                    sol.ItemsUnpacked.Add(bi.ToSolItem(best_variant));
            }
            solArray.Solutions.Add(sol);
            /*
            foreach (BoxInfo bi in boxList)
            {
                if (bi.Is_packed)
                    bi.WriteToFile(fso, best_variant, best_iteration);
            }
            */
            Console.WriteLine("ELAPSED TIME                       : Almost {0:0.000} s", elapsed_time);
            Console.WriteLine("TOTAL NUMBER OF ITERATIONS DONE    : {0}", number_of_iterations);
            Console.WriteLine("BEST SOLUTION FOUND AT             : ITERATION: {0} OF VARIANT: {1}", best_iteration, best_variant);
            Console.WriteLine("TOTAL NUMBER OF BOXES              : {0}", boxList.Count);
            Console.WriteLine("PACKED NUMBER OF BOXES             : {0}", number_packed_boxes);
            Console.WriteLine("TOTAL VOLUME OF ALL BOXES          : {0}", total_box_volume);
            Console.WriteLine("PALLET VOLUME                      : {0}", pallet.Vol);
            Console.WriteLine("BEST SOLUTION'S VOLUME UTILIZATION : {0} OUT OF {1}", best_solution_volume, pallet.Vol);
            Console.WriteLine("PERCENTAGE OF PALLET VOLUME USED   : {0:0.000}", pallet_volume_used_percentage);
            Console.WriteLine("PERCENTAGE OF PACKEDBOXES (VOLUME) : {0:0.000}", packed_box_percentage);
            Console.WriteLine("WHILE PALLET ORIENTATION           : X={0}; Y={1}; Z= {2}", pallet.Pallet_x, pallet.Pallet_y, pallet.Pallet_z);
            Console.WriteLine();
            foreach (var vKey in best_iterations.Keys)
                Console.WriteLine("{0} {1} -> {2}", vKey.Key, vKey.Value, best_iterations[vKey]);
        }
        #endregion
    }
}
