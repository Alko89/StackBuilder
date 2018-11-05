#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    #region AnalysisTreeView
    /// <summary>
    /// AnalysisTreeView : left frame treeview control
    /// </summary>
    public partial class AnalysisTreeView
        : TreeView, IDocumentListener
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AnalysisTreeView()
        {
            try
            {
                // build image list for tree
                ImageList = new ImageList();
                ImageList.Images.Add(AnalysisTreeView.CLSDFOLD);                    // 0
                ImageList.Images.Add(AnalysisTreeView.OPENFOLD);                    // 1
                ImageList.Images.Add(AnalysisTreeView.DOC);                         // 2
                ImageList.Images.Add(AnalysisTreeView.Box);                         // 3
                ImageList.Images.Add(AnalysisTreeView.Case);                        // 4
                ImageList.Images.Add(AnalysisTreeView.Bundle);                      // 5
                ImageList.Images.Add(AnalysisTreeView.Cylinder);                    // 6
                ImageList.Images.Add(AnalysisTreeView.Pallet);                      // 7
                ImageList.Images.Add(AnalysisTreeView.Interlayer);                  // 8
                ImageList.Images.Add(AnalysisTreeView.Truck);                       // 9
                ImageList.Images.Add(AnalysisTreeView.PalletCorners);               // 10
                ImageList.Images.Add(AnalysisTreeView.PalletCap);                   // 11
                ImageList.Images.Add(AnalysisTreeView.PalletFilm);                  // 12
                ImageList.Images.Add(AnalysisTreeView.Pack);                        // 13
                ImageList.Images.Add(AnalysisTreeView.AnalysisCasePallet);          // 14
                ImageList.Images.Add(AnalysisTreeView.AnalysisBundlePallet);        // 15
                ImageList.Images.Add(AnalysisTreeView.AnalysisTruck);               // 16
                ImageList.Images.Add(AnalysisTreeView.AnalysisCase);                // 17
                ImageList.Images.Add(AnalysisTreeView.AnalysisStackingStrength);    // 18
                ImageList.Images.Add(AnalysisTreeView.AnalysisCylinderPallet);      // 19
                ImageList.Images.Add(AnalysisTreeView.AnalysisHCylinderPallet);     // 20
                ImageList.Images.Add(AnalysisTreeView.AnalysisPackPallet);          // 21
               // instantiate context menu
                this.ContextMenuStrip = new ContextMenuStrip();
                // attach event handlers
                this.NodeMouseClick += new TreeNodeMouseClickEventHandler(AnalysisTreeView_NodeMouseClick);
                this.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(OnNodeLeftDoubleClick);
                this.ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
                this.DrawMode = TreeViewDrawMode.OwnerDrawText;
                this.DrawNode += new DrawTreeNodeEventHandler(AnalysisTreeView_DrawNode);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private int ToIconIndex(ItemBase item)
        {
            if (item is AnalysisCasePallet)
            {
                AnalysisCasePallet analysisCasePallet = item as AnalysisCasePallet;
                Packable content = analysisCasePallet.Content;
                if (content is BoxProperties) return 14;
                else if (content is BundleProperties) return 15;
                else if (content is PackProperties) return 21;
                else return 0;
            }
            else if (item is AnalysisBoxCase) return 17;
            else if (item is AnalysisPalletTruck) return 16;
            else if (item is AnalysisCaseTruck) return 16;
            else if (item is AnalysisCylinderPallet) return 19;
            else if (item is AnalysisCylinderCase) return 17;
            else if (item is HAnalysisPallet) return 14;
            else if (item is HAnalysisCase) return 17;
            else
            {
                _log.Error("Unexpected analysis type");
                return 0;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AnalysisTreeView
            // 
            this.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.ResumeLayout(false);

        }
        #endregion

        #region Context menu strip
        /// <summary>
        /// Handler for ContextMenu.Popup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // retrieve node which was clicked
                TreeNode node = GetNodeAt(PointToClient(Cursor.Position));
                if (node == null) return; // user might right click no valid node
                SelectedNode = node;
                // clear previous items
                this.ContextMenuStrip.Items.Clear();
                // let the provider do his work
                if (node.Tag is NodeTag nodeTag)
                    QueryContextMenuItems(nodeTag, this.ContextMenuStrip);
                // set Cancel to false. 
                // it is optimized to true based on empty entry.
                e.Cancel = !(this.ContextMenuStrip.Items.Count > 0);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void QueryContextMenuItems(NodeTag nodeTag, ContextMenuStrip contextMenuStrip)
        {
            if (nodeTag.Type == NodeTag.NodeType.NT_DOCUMENT)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOX, AnalysisTreeView.Box         , new EventHandler(OnCreateNewBox)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCASE, AnalysisTreeView.Case, new EventHandler(OnCreateNewCase)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDER, AnalysisTreeView.Cylinder, new EventHandler(OnCreateNewCylinder)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLET, AnalysisTreeView.Pallet      , new EventHandler(OnCreateNewPallet)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWINTERLAYER, AnalysisTreeView.Interlayer, new EventHandler(OnCreateNewInterlayer)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBUNDLE, AnalysisTreeView.Bundle      , new EventHandler(OnCreateNewBundle)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCORNERS, AnalysisTreeView.PalletCorners, new EventHandler(OnCreateNewPalletCorners)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCAP, AnalysisTreeView.PalletCap, new EventHandler(OnCreateNewPalletCap)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETFILM, AnalysisTreeView.PalletFilm, new EventHandler(OnCreateNewPalletFilm)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCK, AnalysisTreeView.Truck       , new EventHandler(OnCreateNewTruck)));

                if (((DocumentSB)nodeTag.Document).CanCreateAnalysisCasePallet || ((DocumentSB)nodeTag.Document).CanCreateOptiCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripSeparator());
                if (((DocumentSB)nodeTag.Document).CanCreateAnalysisCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWANALYSIS, AnalysisTreeView.AnalysisCasePallet, new EventHandler(OnCreateNewAnalysisCasePallet)));
                /*
                if (((DocumentSB)nodeTag.Document).CanCreateOptiCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCASEANALYSIS, AnalysisTreeView.AnalysisCase, new EventHandler(onCreateNewAnalysisCase)));
                */
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_CLOSE, null, new EventHandler(OnDocumentClose)));
            }
            if (nodeTag.Type == NodeTag.NodeType.NT_BOX
                || nodeTag.Type == NodeTag.NodeType.NT_CASE
                || nodeTag.Type == NodeTag.NodeType.NT_PACK
                || nodeTag.Type == NodeTag.NodeType.NT_CASEOFBOXES
                || nodeTag.Type == NodeTag.NodeType.NT_CYLINDER
                || nodeTag.Type == NodeTag.NodeType.NT_PALLET
                || nodeTag.Type == NodeTag.NodeType.NT_BUNDLE
                || nodeTag.Type == NodeTag.NodeType.NT_INTERLAYER
                || nodeTag.Type == NodeTag.NodeType.NT_TRUCK
                || nodeTag.Type == NodeTag.NodeType.NT_PALLETCORNERS
                || nodeTag.Type == NodeTag.NodeType.NT_PALLETCAP
                || nodeTag.Type == NodeTag.NodeType.NT_PALLETFILM
                )
            {
                string message = string.Format(Resources.ID_DELETEITEM, nodeTag.ItemProperties.Name);
                contextMenuStrip.Items.Add(new ToolStripMenuItem(message, AnalysisTreeView.DELETE, new EventHandler(OnDeleteBaseItem)));
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTBOX)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOX, AnalysisTreeView.Box, new EventHandler(OnCreateNewBox)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTCASE)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCASE, AnalysisTreeView.Case, new EventHandler(OnCreateNewCase)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTCYLINDER)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDER, AnalysisTreeView.Cylinder, new EventHandler(OnCreateNewCylinder)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLET)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLET, AnalysisTreeView.Pallet, new EventHandler(OnCreateNewPallet)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTINTERLAYER)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWINTERLAYER, AnalysisTreeView.Interlayer, new EventHandler(OnCreateNewInterlayer)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTBUNDLE)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBUNDLE, AnalysisTreeView.Bundle, new EventHandler(OnCreateNewBundle)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTTRUCK)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCK, AnalysisTreeView.Truck, new EventHandler(OnCreateNewTruck)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLETCORNERS)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCORNERS, AnalysisTreeView.PalletCorners, new EventHandler(OnCreateNewPalletCorners)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLETCAP)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCAP, AnalysisTreeView.PalletCap, new EventHandler(OnCreateNewPalletCap)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLETFILM)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETFILM, AnalysisTreeView.PalletFilm, new EventHandler(OnCreateNewPalletFilm)));
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTANALYSIS)
            {
                if (nodeTag.Document.CanCreateAnalysisCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWANALYSIS, AnalysisTreeView.AnalysisCasePallet, new EventHandler(OnCreateNewAnalysisCasePallet)));
                if (nodeTag.Document.CanCreateAnalysisCylinderPallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDERANALYSIS, AnalysisTreeView.AnalysisCylinderPallet, new EventHandler(OnCreateNewAnalysisCylinderPallet)));
                if (nodeTag.Document.CanCreateAnalysisPalletTruck)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCKANALYSIS, AnalysisTreeView.AnalysisTruck, new EventHandler(OnCreateNewAnalysisPalletTruck)));
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_ANALYSIS)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_EDIT, nodeTag.Analysis.Name), null
                    , new EventHandler(OnEditAnalysis)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_DELETEITEM, nodeTag.Analysis.Name), AnalysisTreeView.DELETE
                    , new EventHandler(OnDeleteBaseItem)));
                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_GENERATEREPORT, nodeTag.Analysis.Name), AnalysisTreeView.WORD
                    , new EventHandler(OnAnalysisReport)));
            }
        }
        #endregion

        #region Handling context menus
        private void OnDeleteBaseItem(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;                
                tag.Document.RemoveItem(tag.ItemProperties);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnAnalysisReport(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                SolutionReportClicked(this, new AnalysisTreeViewEventArgs(tag));
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnAnalysisExportCollada(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                SolutionColladaExportClicked(this, new AnalysisTreeViewEventArgs(tag));
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBox(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBoxUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewCase(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewCaseUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPack(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPackUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewCylinder(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewCylinderUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewInterlayer(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewInterlayerUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBundle(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBundleUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewTruck(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewTruckUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletCorners(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletCornersUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletCap(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletCapUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletFilm(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletFilmUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }

        private void OnCreateNewAnalysisCasePallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisCasePalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewAnalysisCylinderPallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisCylinderPalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewAnalysisPalletTruck(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisPalletTruckUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnDocumentClose(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                CancelEventArgs cea = new CancelEventArgs();
                FormMain.GetInstance().CloseDocument((DocumentSB)tag.Document, cea);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnEditAnalysis(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                DocumentSB doc = tag.Document as DocumentSB;
                doc.EditAnalysis(tag.Analysis);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        #endregion

        #region Event handlers
        void OnNodeLeftDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                SelectedNode = e.Node;
                // handle only left mouse button click
                if (e.Button != MouseButtons.Left) return;
                NodeTag tag = CurrentTag;
                NodeTag.NodeType tagType = tag.Type;
                if (null != AnalysisNodeClicked &&
                    (tag.Type == NodeTag.NodeType.NT_ANALYSIS)
                    || (tag.Type == NodeTag.NodeType.NT_ECTANALYSIS)
                    || (tag.Type == NodeTag.NodeType.NT_BOX)
                    || (tag.Type == NodeTag.NodeType.NT_CASE)
                    || (tag.Type == NodeTag.NodeType.NT_PACK)
                    || (tag.Type == NodeTag.NodeType.NT_BUNDLE)
                    || (tag.Type == NodeTag.NodeType.NT_CYLINDER)
                    || (tag.Type == NodeTag.NodeType.NT_CASEOFBOXES)
                    || (tag.Type == NodeTag.NodeType.NT_PALLET)
                    || (tag.Type == NodeTag.NodeType.NT_INTERLAYER)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETCORNERS)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETCAP)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETFILM)
                    || (tag.Type == NodeTag.NodeType.NT_TRUCK)
                    )
                {
                    AnalysisNodeClicked(this, new AnalysisTreeViewEventArgs(tag));
                    e.Node.Expand();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        void AnalysisTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }
        void AnalysisTreeView_DrawNode(object sender, System.Windows.Forms.DrawTreeNodeEventArgs e)
        {
            try
            {
                // get NodeTag
                NodeTag tag = e.Node.Tag as NodeTag;
                if (null == tag)
                    throw new Exception(string.Format("Node {0} has null tag", e.Node.Text));
                Rectangle nodeBounds = e.Node.Bounds;
                if (null != tag.ItemProperties)
                    TextRenderer.DrawText(e.Graphics, tag.ItemProperties.Name, Font, nodeBounds, System.Drawing.Color.Black, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.NoClipping);
                else
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, Font, nodeBounds, System.Drawing.Color.Black, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.NoClipping);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Helpers
        internal NodeTag CurrentTag
        {
            get
            {
                TreeNode currentNode = this.SelectedNode;
                if (null == currentNode)
                    throw new Exception("No node selected");
                return currentNode.Tag as NodeTag;
            }
        }
        internal TreeNode FindNode(TreeNode node, NodeTag nodeTag)
        {
            // check with node itself
            if (null != node)
            {
                NodeTag tag = node.Tag as NodeTag;
                if (null == tag)
                {
                    _log.Error(string.Format("Node {0} has no valid NodeTag", node.Text));
                    return null;
                }
                if (tag.Equals(nodeTag))
                    return node;
            }
            // check with child nodes
            TreeNodeCollection tnCollection = null == node ? Nodes : node.Nodes;
            foreach (TreeNode tn in tnCollection)
            {
                TreeNode tnResult = FindNode(tn, nodeTag);
                if (null != tnResult)
                    return tnResult;
            }
            return null;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// is a prototype for event handlers of AnalysisNodeClicked / SolutionReportNodeClicked
        /// </summary>
        /// <param name="sender">sending object (tree)</param>
        /// <param name="eventArg">contains NodeTag to identify clicked TreeNode</param>
        public delegate void AnalysisNodeClickHandler(object sender, AnalysisTreeViewEventArgs eventArg);
        #endregion

        #region Events
        public event AnalysisNodeClickHandler AnalysisNodeClicked;
        public event AnalysisNodeClickHandler SolutionReportClicked;
        public event AnalysisNodeClickHandler SolutionColladaExportClicked;
        #endregion

        #region IDocumentListener implementation
        /// <summary>
        /// handles new document creation
        /// </summary>
        /// <param name="doc"></param>
        public void OnNewDocument(Document doc)
        {
            doc.DocumentClosed += OnDocumentClosed;

            // add document node
            TreeNode nodeDoc = new TreeNode(doc.Name, 2, 2)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_DOCUMENT, doc)
            };
            this.Nodes.Add(nodeDoc);
            // add box list node
            TreeNode nodeBoxes = new TreeNode(Resources.ID_NODE_BOXES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTBOX, doc)
            };
            nodeDoc.Nodes.Add(nodeBoxes);
            // add case list node
            TreeNode nodeCases = new TreeNode(Resources.ID_NODE_CASES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTCASE, doc)
            };
            nodeDoc.Nodes.Add(nodeCases);
            // add pack list node
            TreeNode nodePacks = new TreeNode(Resources.ID_NODE_PACKS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPACK, doc)
            };
            nodeDoc.Nodes.Add(nodePacks);
            // add bundle list node
            TreeNode nodeBundles = new TreeNode(Resources.ID_NODE_BUNDLES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTBUNDLE, doc)
            };
            nodeDoc.Nodes.Add(nodeBundles);
            // add cylinder list node
            TreeNode nodeCylinders = new TreeNode(Resources.ID_NODE_CYLINDERS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTCYLINDER, doc)
            };
            nodeDoc.Nodes.Add(nodeCylinders);
            // add pallet list node
            TreeNode nodeInterlayers = new TreeNode(Resources.ID_NODE_INTERLAYERS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTINTERLAYER, doc)
            };
            nodeDoc.Nodes.Add(nodeInterlayers);
            // add pallet list node
            TreeNode nodePallets = new TreeNode(Resources.ID_NODE_PALLETS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLET, doc)
            };
            nodeDoc.Nodes.Add(nodePallets);
            // add pallet corners list node
            TreeNode nodePalletCorners = new TreeNode(Resources.ID_NODE_PALLETCORNERS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLETCORNERS, doc)
            };
            nodeDoc.Nodes.Add(nodePalletCorners);
            // add pallet cap node
            TreeNode nodePalletCaps = new TreeNode(Resources.ID_NODE_PALLETCAPS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLETCAP, doc)
            };
            nodeDoc.Nodes.Add(nodePalletCaps);
            // add pallet film node
            TreeNode nodePalletFilms = new TreeNode(Resources.ID_NODE_PALLETFILMS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLETFILM, doc)
            };
            nodeDoc.Nodes.Add(nodePalletFilms);
            // add truck list node
            TreeNode nodeTrucks = new TreeNode(Resources.ID_NODE_TRUCKS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTTRUCK, doc)
            };
            nodeDoc.Nodes.Add(nodeTrucks);
            // add analysis list node
            TreeNode nodeAnalyses = new TreeNode(Resources.ID_NODE_ANALYSES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc)
            };
            nodeDoc.Nodes.Add(nodeAnalyses);
            nodeDoc.Expand();
        }
        /// <summary>
        /// handles new type creation
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="itemProperties"></param>
        public void OnNewTypeCreated(Document doc, ItemBase itemProperties)
        {
            int iconIndex = 0;
            NodeTag.NodeType nodeType = NodeTag.NodeType.NT_BOX;
            NodeTag.NodeType parentNodeType = NodeTag.NodeType.NT_LISTBOX;
            if (itemProperties.GetType() == typeof(CaseOfBoxesProperties))
            {
                iconIndex = 17;
                nodeType = NodeTag.NodeType.NT_CASEOFBOXES;
                parentNodeType = NodeTag.NodeType.NT_LISTCASE;
            }
            else if (itemProperties.GetType() == typeof(BoxProperties))
            {
                BoxProperties boxProperties = itemProperties as BoxProperties;
                if (boxProperties.HasInsideDimensions)
                {
                    iconIndex = 4;
                    nodeType = NodeTag.NodeType.NT_CASE;
                    parentNodeType = NodeTag.NodeType.NT_LISTCASE;
                }
                else
                {
                    iconIndex = 3;
                    nodeType = NodeTag.NodeType.NT_BOX;
                    parentNodeType = NodeTag.NodeType.NT_LISTBOX;
                }
            }
            else if (itemProperties.GetType() == typeof(BundleProperties))
            {
                iconIndex = 5;
                nodeType = NodeTag.NodeType.NT_BUNDLE;
                parentNodeType = NodeTag.NodeType.NT_LISTBUNDLE;
            }
            else if (itemProperties.GetType() == typeof(CylinderProperties))
            {
                iconIndex = 6;
                nodeType = NodeTag.NodeType.NT_CYLINDER;
                parentNodeType = NodeTag.NodeType.NT_LISTCYLINDER;
            }
            else if (itemProperties.GetType() == typeof(PalletProperties))
            {
                iconIndex = 7;
                nodeType = NodeTag.NodeType.NT_PALLET;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLET;
            }
            else if (itemProperties.GetType() == typeof(InterlayerProperties))
            {
                iconIndex = 8;
                nodeType = NodeTag.NodeType.NT_INTERLAYER;
                parentNodeType = NodeTag.NodeType.NT_LISTINTERLAYER;
            }
            else if (itemProperties.GetType() == typeof(TruckProperties))
            {
                iconIndex = 9;
                nodeType = NodeTag.NodeType.NT_TRUCK;
                parentNodeType = NodeTag.NodeType.NT_LISTTRUCK;
            }
            else if (itemProperties.GetType() == typeof(PalletCornerProperties))
            {
                iconIndex = 10;
                nodeType = NodeTag.NodeType.NT_PALLETCORNERS;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETCORNERS;
            }
            else if (itemProperties.GetType() == typeof(PalletCapProperties))
            {
                iconIndex = 11;
                nodeType = NodeTag.NodeType.NT_PALLETCAP;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETCAP;
            }
            else if (itemProperties.GetType() == typeof(PalletFilmProperties))
            {
                iconIndex = 12;
                nodeType = NodeTag.NodeType.NT_PALLETFILM;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETFILM;
            }
            else if (itemProperties.GetType() == typeof(PackProperties))
            {
                iconIndex = 13;
                nodeType = NodeTag.NodeType.NT_PACK;
                parentNodeType = NodeTag.NodeType.NT_LISTPACK;
            }
            else
            {
                Debug.Assert(false);
                _log.Error("AnalysisTreeView.OnNewTypeCreated() -> unknown type!");
                return;
            }
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(parentNodeType, doc));
            if (null == parentNode)
            { 
                _log.Error(string.Format("Failed to load parentNode for {0}", itemProperties.Name));
                return;
            }
            // instantiate node
            TreeNode nodeItem = new TreeNode(itemProperties.Name, iconIndex, iconIndex)
            {
                // set node tag
                Tag = new NodeTag(nodeType, doc, itemProperties)
            };
            // insert
            parentNode.Nodes.Add(nodeItem);
            parentNode.Expand();
            // if item is CaseOfBoxesProperties
            if (itemProperties is CaseOfBoxesProperties)
            {
                // insert sub node
                CaseOfBoxesProperties caseOfBoxesProperties = itemProperties as CaseOfBoxesProperties;
                TreeNode subNode = new TreeNode(caseOfBoxesProperties.InsideBoxProperties.Name, 3, 3)
                {
                    Tag = new NodeTag(NodeTag.NodeType.NT_BOX, doc, caseOfBoxesProperties.InsideBoxProperties)
                };
                nodeItem.Nodes.Add(subNode);
            }
        }
        public void OnNewAnalysisCreated(Document doc, Analysis analysis)
        {
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc));
            // instantiate analysis node
            TreeNode nodeAnalysis = new TreeNode(analysis.Name, ToIconIndex(analysis), ToIconIndex(analysis))
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis)
            };
            // insert context menu
            parentNode.Nodes.Add( nodeAnalysis );
            parentNode.Expand();
        }
        public void OnAnalysisUpdated(Document doc, Analysis analysis)
        {
            // get parent node
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            if (null != analysisNode)
                analysisNode.Name = analysis.Name;
        }
        public void OnNewAnalysisCreated(Document doc, HAnalysis analysis)
        {
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc));
            // instantiate analysis node
            TreeNode nodeAnalysis = new TreeNode(analysis.Name, ToIconIndex(analysis), ToIconIndex(analysis))
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis)
            };
            // insert context menu
            parentNode.Nodes.Add( nodeAnalysis );
            parentNode.Expand();
        }
        public void OnAnalysisUpdated(Document doc, HAnalysis analysis)
        {
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            if (null != analysisNode)
                analysisNode.Name = analysis.Name;
        }
        #endregion

        #region Remove functions
        /// <summary>
        /// handles new type removed
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="itemBase"></param>
        public void OnTypeRemoved(Document doc, ItemBase itemBase)
        {
            NodeTag.NodeType nodeType = NodeTag.NodeType.NT_UNKNOWN;
            if (itemBase.GetType() == typeof(BoxProperties))
            {
                BoxProperties box = itemBase as BoxProperties;
                if (box.HasInsideDimensions)
                    nodeType = NodeTag.NodeType.NT_CASE;
                else
                    nodeType = NodeTag.NodeType.NT_BOX;
            }
            else if (itemBase.GetType() == typeof(BundleProperties))
                nodeType = NodeTag.NodeType.NT_BUNDLE;
            else if (itemBase.GetType() == typeof(PackProperties))
                nodeType = NodeTag.NodeType.NT_PACK;
            else if (itemBase.GetType() == typeof(CaseOfBoxesProperties))
                nodeType = NodeTag.NodeType.NT_CASEOFBOXES;
            else if (itemBase.GetType() == typeof(InterlayerProperties))
                nodeType = NodeTag.NodeType.NT_INTERLAYER;
            else if (itemBase.GetType() == typeof(PalletCornerProperties))
                nodeType = NodeTag.NodeType.NT_PALLETCORNERS;
            else if (itemBase.GetType() == typeof(PalletCapProperties))
                nodeType = NodeTag.NodeType.NT_PALLETCAP;
            else if (itemBase.GetType() == typeof(PalletFilmProperties))
                nodeType = NodeTag.NodeType.NT_PALLETFILM;
            else if (itemBase.GetType() == typeof(PalletProperties))
                nodeType = NodeTag.NodeType.NT_PALLET;
            else if (itemBase.GetType() == typeof(TruckProperties))
                nodeType = NodeTag.NodeType.NT_TRUCK;
            else if (itemBase.GetType() == typeof(CylinderProperties))
                nodeType = NodeTag.NodeType.NT_CYLINDER;
            Debug.Assert(nodeType != NodeTag.NodeType.NT_UNKNOWN);
            if (nodeType == NodeTag.NodeType.NT_UNKNOWN)
                return; // ->not found exit
            // get node
            TreeNode typeNode = FindNode(null, new NodeTag(nodeType, doc, itemBase));
            // remove node
            if (null != typeNode)
                Nodes.Remove(typeNode);
        }

        public void OnAnalysisRemoved(Document doc, ItemBase analysis)
        {
            // get node
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            // test
            if (null == analysisNode)
            {
                _log.Error(string.Format("Failed to find a valid tree node for analysis {0}", analysis.Name));
                return;
            }
            // remove node
            Nodes.Remove(analysisNode);
        }
        /// <summary>
        /// handles document closing event by removing the corresponding document node in TreeView
        /// </summary>
        public void OnDocumentClosed(Document doc)
        {
            NodeTag.NodeType nodeType = NodeTag.NodeType.NT_DOCUMENT;
            // get node
            TreeNode docNode = FindNode(null, new NodeTag(nodeType, doc));
            // remove node
            Nodes.Remove(docNode);

            doc.DocumentClosed -= OnDocumentClosed;
        }
        #endregion

        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(AnalysisTreeView));
        #endregion
    }
    #endregion

    #region NodeTag class
    /// <summary>
    /// NodeTag will be used for each TreeNode.Tag
    /// </summary>
    public class NodeTag
    {
        #region Enums
        /// <summary>
        /// AnalysisTreeView node types
        /// </summary>
        public enum NodeType
        {
            /// <summary>
            /// document
            /// </summary>
            NT_DOCUMENT,
            /// <summary>
            /// list of boxes
            /// </summary>
            NT_LISTBOX,
            /// <summary>
            /// list of cases
            /// </summary>
            NT_LISTCASE,
            /// <summary>
            /// list of pack
            /// </summary>
            NT_LISTPACK,
            /// <summary>
            /// list of cylinders
            /// </summary>
            NT_LISTCYLINDER,
            /// <summary>
            /// list of bundles
            /// </summary>
            NT_LISTBUNDLE,
            /// <summary>
            /// list of palets
            /// </summary>
            NT_LISTPALLET,
            /// <summary>
            /// list of interlayers
            /// </summary>
            NT_LISTINTERLAYER,
            /// <summary>
            /// list of trucks
            /// </summary>
            NT_LISTTRUCK,
            /// <summary>
            /// list of analyses
            /// </summary>
            NT_LISTANALYSIS,
            /// <summary>
            /// list of pallet corners
            /// </summary>
            NT_LISTPALLETCORNERS,
            /// <summary>
            /// list of pallet cap
            /// </summary>
            NT_LISTPALLETCAP,
            /// <summary>
            /// list of pallet film
            /// </summary>
            NT_LISTPALLETFILM,
            /// <summary>
            /// box
            /// </summary>
            NT_BOX,
            /// <summary>
            /// case
            /// </summary>
            NT_CASE,
            /// <summary>
            /// pack
            /// </summary>
            NT_PACK,
            /// <summary>
            /// case of boxes
            /// </summary>
            NT_CASEOFBOXES,
            /// <summary>
            /// bundle
            /// </summary>
            NT_BUNDLE,
            /// <summary>
            /// cylinder
            /// </summary>
            NT_CYLINDER,
            /// <summary>
            /// palet
            /// </summary>
            NT_PALLET,
            /// <summary>
            /// interlayer
            /// </summary>
            NT_INTERLAYER,
            /// <summary>
            /// truck
            /// </summary>
            NT_TRUCK,
            /// <summary>
            /// pallet corners
            /// </summary>
            NT_PALLETCORNERS,
            /// <summary>
            /// pallet cap
            /// </summary>
            NT_PALLETCAP,
            /// <summary>
            /// pallet film
            /// </summary>
            NT_PALLETFILM,
            /// <summary>
            /// analysis
            /// </summary>
            NT_ANALYSIS,
            /// <summary>
            /// analysis box
            /// </summary>
            NT_ANALYSISBOX,
            /// <summary>
            /// analysis pack
            /// </summary>
            NT_ANALYSISPACK,
            /// <summary>
            /// analysis pallet
            /// </summary>
            NT_ANALYSISPALLET,
            /// <summary>
            /// analysis interlayer
            /// </summary>
            NT_ANALYSISINTERLAYER,
            /// <summary>
            /// analysis pallet corners
            /// </summary>
            NT_ANALYSISPALLETCORNERS,
            /// <summary>
            /// analysis pallet cap
            /// </summary>
            NT_ANALYSISPALLETCAP,
            /// <summary>
            /// analysis pallet film
            /// </summary>
            NT_ANALYSISPALLETFILM,
            /// <summary>
            /// analysis solution
            /// </summary>
            NT_CASEPALLETANALYSISSOLUTION,
            /// <summary>
            /// pack/pallet analysis solution
            /// </summary>
            NT_PACKPALLETANALYSISSOLUTION,
            /// <summary>
            /// cylinder pallet analysis solution
            /// </summary>
            NT_CYLINDERPALLETANALYSISSOLUTION,
            /// <summary>
            /// hcylinder pallet analysis solution
            /// </summary>
            NT_HCYLINDERPALLETANALYSISSOLUTION,
            /// <summary>
            /// analysis report
            /// </summary>
            NT_ANALYSISSOLREPORT,
            /// <summary>
            /// truck analysis
            /// </summary>
            NT_TRUCKANALYSIS,
            /// <summary>
            /// truck analysis solution
            /// </summary>
            NT_TRUCKANALYSISSOL,
            /// <summary>
            /// case analysis
            /// </summary>
            NT_BOXCASEPALLETANALYSIS,
            /// <summary>
            /// case analysis solution
            /// </summary>
            NT_CASESOLUTION,
            /// <summary>
            /// ECT analysis (Edge Crush Test)
            /// </summary>
            NT_ECTANALYSIS,
            /// <summary>
            /// box/case analysis
            /// </summary>
            NT_ANALYSISBOXCASE,
            /// <summary>
            /// box/case analysis case
            /// </summary>
            NT_BOXCASEANALYSISCASE,
            /// <summary>
            /// box/case analysis box
            /// </summary>
            NT_BOXCASEANALYSISBOX,
            /// <summary>
            /// box/case analysis solution
            /// </summary>
            NT_BOXCASEANALYSISSOLUTION,
            /// <summary>
            /// unknown
            /// </summary>
            NT_UNKNOWN
        }
        #endregion

        #region Data members
        private NodeType _type;
        private Document _document;
        private ItemBase _itemBase;
        #endregion

        #region Constructor
        public NodeTag(NodeType type, Document document)
        {
            _type = type;
            _document = document;       
        }
        public NodeTag(NodeType type, Document document, ItemBase itemBase)
        {
            _type = type;
            _document = document;
            _itemBase = itemBase;
        }
        #endregion

        #region Object method overrides
        public override bool Equals(object obj)
        {
            NodeTag nodeTag = obj as NodeTag;
            if (null == nodeTag) return false;
            return _type == nodeTag._type
                && _document == nodeTag._document
                && _itemBase == nodeTag._itemBase;
        }
        public override int GetHashCode()
        {
            return _type.GetHashCode()
                ^ _document.GetHashCode()
                ^ _itemBase.GetHashCode();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// returns node type
        /// </summary>
        public NodeType Type { get { return _type; } }
        /// <summary>
        /// returns document adressed 
        /// </summary>
        public Document Document { get { return _document; } }
        /// <summary>
        /// returns itempProperties (box/palet/interlayer)
        /// </summary>
        public ItemBase ItemProperties { get { return _itemBase; } }
        /// <summary>
        /// returns analysis if any
        /// </summary>
        public Analysis Analysis => _itemBase as Analysis;
        public HAnalysis HAnalysis => _itemBase as HAnalysis;
        #endregion
    }
    #endregion

    #region AnalysisTreeViewEventArgs class
    /// <summary>
    /// EventArg inherited class used as AnalysisNodeClickHandler delegate argument
    /// Encapsulates a reference to a NodeTag
    /// </summary>
    public class AnalysisTreeViewEventArgs : EventArgs
    {
        #region Constructor
        public AnalysisTreeViewEventArgs(NodeTag nodeTag)  { NodeTag = nodeTag; }
        #endregion
        #region Public properties
        public Document Document => NodeTag.Document;
        public Analysis Analysis => NodeTag.Analysis;
        public HAnalysis HAnalysis => NodeTag.HAnalysis;
        public ItemBase ItemBase => NodeTag.ItemProperties;
        #endregion
        #region Private properties
        private NodeTag NodeTag { get; set; }
        #endregion
    }
    #endregion
}
