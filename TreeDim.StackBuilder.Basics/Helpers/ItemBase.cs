﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region IItemListener
    public interface IItemListener
    {
        void Update(ItemBase itemFrom);
        void Kill(ItemBase itemFrom);
    }
    #endregion

    #region Descriptor
    public class GlobID
    {
        public GlobID()
        { IGuid = Guid.NewGuid(); Name = string.Empty; Description = string.Empty; }
        public GlobID(Guid guid, string name, string description)
        { IGuid = guid; Name = name; Description = description; }
        public GlobID(string name, string description)
        { IGuid = Guid.NewGuid(); Name = name; Description = description; }
        public Guid IGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public void SetNameDesc(string name, string description)
        { Name = name; Description = description; }
        public override string ToString()
        { return string.Format("Guid = {0}\nName = {1}\nDescription = {2}", IGuid, Name, Description); }
    }
    #endregion

    #region ItemBase
    /// <summary>
    /// This class holds Name / description properties for Box / Pallet / Interlayer / Analysis
    /// it also handle dependancy problems
    /// </summary>
    public abstract class ItemBase : IDisposable
    {
        #region Data members
        // parent document
        private Document _parentDocument;
        // dependancies
        private List<ItemBase> _dependancies = new List<ItemBase>();
        // Track whether Dispose has been called.
        private bool disposed = false;
        // listeners
        List<IItemListener> _listeners = new List<IItemListener>();
        // logger
        static readonly ILog _log = LogManager.GetLogger(typeof(ItemBase));
        #endregion

        #region Constructors
        public ItemBase(Document document)
        {
            _parentDocument = document;
        }
        #endregion

        #region Abstract properties
        public abstract GlobID ID { get; }
        #endregion

        #region Public properties
        public Document ParentDocument
        {
            get { return _parentDocument; }
        }
        public string Name { get { return ID.Name; } }
        public string Description { get { return ID.Description; } }
        #endregion

        #region Dependancies
        public void AddDependancy(ItemBase dependancie)
        {
            if (_dependancies.Contains(dependancie))
            {
                _log.Warn(string.Format("Tried to add {0} as a dependancy of {1} a second time!", dependancie.ID.Name, this.ID.Name));
                return;
            }
            _dependancies.Add(dependancie);    
        }
        public bool HasDependingAnalyses
        { get { return _dependancies.Count > 0; } }
        public void RemoveDependancy(ItemBase dependancie)
        {
            _dependancies.Remove(dependancie);  
        }
        protected void Modify()
        {
            // update dependancies
            foreach (ItemBase item in _dependancies)
                item.OnAttributeModified(this);
            // update listeners
            UpdateListeners();
            // update parent document
            if (null != _parentDocument)
                _parentDocument.Modify();
        }
        public void EndUpdate()
        {
            // update dependancies
            foreach (ItemBase item in _dependancies)
                item.OnEndUpdate(this);
        }
        public virtual void OnAttributeModified(ItemBase modifiedAttribute) {}
        public virtual void OnEndUpdate(ItemBase updatedAttribute) {}
        protected virtual void RemoveItselfFromDependancies() {}
        protected virtual void OnDispose() {}
        #endregion

        #region IDisposable implementation
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this); 
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                OnDispose();
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources
                    int iCount = _dependancies.Count;
                    while (_dependancies.Count > 0)
                    {
                        _parentDocument.RemoveItem(_dependancies[0]);
                        if (_dependancies.Count == iCount)
                        {
                            _log.Warn(string.Format("Failed to remove correctly dependancy {0} ", _dependancies[0].ID.Name));
                            _dependancies.Remove(_dependancies[0]);
                            break;
                        }
                    }
                }
                RemoveItselfFromDependancies();
                KillListeners();
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false, only the following code is executed.
 
                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return string.Format("Name:{0} \nDescription: {1}\n", ID.Name, ID.Description);
        }
        #endregion

        #region Listeners
        public void AddListener(IItemListener listener)
        {
            _listeners.Add(listener);
        }
        public void RemoveListener(IItemListener listener)
        {
            _listeners.Remove(listener);
        }
        protected void UpdateListeners()
        {
            foreach (IItemListener listener in _listeners)
                listener.Update(this);
        }
        protected void KillListeners()
        {
            while (_listeners.Count > 0)
                _listeners[0].Kill(this);
        }
        #endregion
    }
    #endregion

    #region ItemBaseNamed
    public abstract class ItemBaseNamed : ItemBase
    {
        #region Data members
        private GlobID _id = new GlobID();
        #endregion

        #region Constructors
        public ItemBaseNamed(Document doc)
            : base(doc)
        { 
        }
        public ItemBaseNamed(Document doc, string name, string description)
            : base(doc)
        {
            ID.Name = name; ID.Description = description;
        }
        #endregion

        #region Override ItemBase
        public override GlobID ID { get { return _id; } }
        #endregion
    }
    #endregion

    #region IPackContainer
    public interface IPackContainer
    {
        bool HasInsideDimensions { get; }
        double InsideLength     { get; }
        double InsideWidth      { get; }
        double InsideHeight     { get; }
        Vector3D InsideDimensions { get; }
        double[] InsideDimensionsArray { get; }
    }
    #endregion
}
