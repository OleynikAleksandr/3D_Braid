#region сборка Grasshopper, Version=6.35.21222.17000, Culture=neutral, PublicKeyToken=dda4f5ec2cd80803
// D:\Grasshopper Projects\repos\3D Braid\packages\Grasshopper.6.35.21222.17001\lib\net45\Grasshopper.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.HTML;
using Grasshopper.GUI.RemotePanel;
using Grasshopper.Kernel.Undo;
using Grasshopper.Kernel.Undo.Actions;
using Grasshopper.My.Resources;
using Microsoft.VisualBasic.CompilerServices;
using Rhino;

namespace Grasshopper.Kernel;

//
// Сводка:
//     Standard implementation of IGH_DocumentObject.
public abstract class GH_DocumentObject : GH_InstanceDescription, IGH_DocumentObject
{
    public delegate void ColourEventHandler(GH_ColourPicker sender, GH_ColourPickerEventArgs e);

    protected IGH_Attributes m_attributes;

    internal int m_icon_index;

    internal int m_icon_index_locked;

    internal Bitmap m_icon_override;

    internal GH_IconDisplayMode m_icon_mode;

    private GH_SettingsServer _valueTable;

    [SpecialName]
    private int _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag;

    [SpecialName]
    private StaticLocalInitFlag _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init;

    //
    // Сводка:
    //     Gets or sets the attributes that are associated with this object. Only set custom
    //     attributes if you know what you are doing.
    public IGH_Attributes Attributes
    {
        get
        {
            return m_attributes;
        }
        set
        {
            m_attributes = value;
        }
    }

    //
    // Сводка:
    //     Gets whether this object is obsolete. Default implementation returns true if
    //     the class name contains the string "OBSOLETE" or if this class has been decorated
    //     with the ObsoleteAttribute. You are free to override this if you want, but I
    //     suggest adding the ObsoleteAttribute instead.
    public virtual bool Obsolete
    {
        get
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0019: Expected O, but got Unknown
            //IL_0059: Unknown result type (might be due to invalid IL or missing references)
            if (_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init == null)
            {
                Interlocked.CompareExchange(ref _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init, new StaticLocalInitFlag(), null);
            }

            bool lockTaken = false;
            try
            {
                Monitor.Enter(_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init, ref lockTaken);
                if (_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init.State == 0)
                {
                    _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init.State = 2;
                    _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag = -1;
                }
                else if (_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init.State = 1;
                if (lockTaken)
                {
                    Monitor.Exit(_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag_0024Init);
                }
            }

            if (_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag == 0)
            {
                return false;
            }

            if (_0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag > 0)
            {
                return true;
            }

            _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag = 0;
            Type type = GetType();
            if (type.Name.ToUpperInvariant().Contains("OBSOLETE"))
            {
                _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag = 1;
                return true;
            }

            object[] customAttributes = type.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: true);
            if (customAttributes == null)
            {
                return false;
            }

            if (customAttributes.Length == 0)
            {
                return false;
            }

            object[] array = customAttributes;
            for (int i = 0; i < array.Length; i = checked(i + 1))
            {
                if (RuntimeHelpers.GetObjectValue(array[i]) is ObsoleteAttribute)
                {
                    _0024STATIC_0024get_Obsolete_00242002_0024obsoleteFlag = 1;
                    return true;
                }
            }

            return false;
        }
    }

    //
    // Сводка:
    //     Gets the exposure of this object in the Graphical User Interface. The default
    //     is to expose everywhere.
    public virtual GH_Exposure Exposure => GH_Exposure.primary;

    //
    // Сводка:
    //     The icon associated with this object.
    public Bitmap Icon_24x24
    {
        get
        {
            if (m_icon_index < 0)
            {
                if (m_icon_override == null)
                {
                    Bitmap bitmap = Icon;
                    if (bitmap == null)
                    {
                        bitmap = Internal_Icon_24x24;
                    }

                    if (bitmap != null)
                    {
                        switch (bitmap.PixelFormat)
                        {
                            default:
                                bitmap = GH_IconTable.To32BppArgb(bitmap);
                                break;
                            case PixelFormat.Format24bppRgb:
                            case PixelFormat.Format32bppRgb:
                            case PixelFormat.Format32bppPArgb:
                            case PixelFormat.Format32bppArgb:
                                break;
                        }
                    }

                    m_icon_index = GH_IconTable.RegisterIcon(bitmap);
                }
                else
                {
                    m_icon_index = GH_IconTable.RegisterIcon(m_icon_override);
                }
            }

            return GH_IconTable.Icon(m_icon_index);
        }
    }

    //
    // Сводка:
    //     The greyscale icon of this object.
    public Bitmap Icon_24x24_Locked
    {
        get
        {
            if (m_icon_index_locked < 0)
            {
                Bitmap bitmap = Icon_24x24;
                if (bitmap != null)
                {
                    bitmap = (Bitmap)Icon_24x24.Clone();
                    GH_MemoryBitmap gH_MemoryBitmap = new GH_MemoryBitmap(bitmap);
                    gH_MemoryBitmap.Filter_GreyScale();
                    gH_MemoryBitmap.Filter_Dullify();
                    gH_MemoryBitmap.Filter_Blur(3, 1);
                    gH_MemoryBitmap.Release(includeChanges: true);
                }

                m_icon_index_locked = GH_IconTable.RegisterIcon(bitmap);
            }

            return GH_IconTable.Icon(m_icon_index_locked);
        }
    }

    //
    // Сводка:
    //     Override this function to supply a custom icon. The result of this property is
    //     cached, so don't worry if icon retrieval is not very fast.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    protected virtual Bitmap Internal_Icon_24x24 => m_icon_override;

    //
    // Сводка:
    //     Override this function to supply a custom icon (24x24 pixels). The result of
    //     this property is cached, so don't worry if icon retrieval is not very fast.
    protected virtual Bitmap Icon => m_icon_override;

    //
    // Сводка:
    //     Gets the current display mode of the object.
    public GH_IconDisplayMode IconDisplayMode
    {
        get
        {
            return m_icon_mode;
        }
        set
        {
            if (m_icon_mode != value)
            {
                m_icon_mode = value;
                OnObjectChanged(GH_ObjectEventType.IconDisplayMode);
            }
        }
    }

    //
    // Сводка:
    //     Returns a consistent ID for this object type. Every object must supply a unique
    //     and unchanging ID that is used to identify objects of the same type.
    public abstract Guid ComponentGuid { get; }

    //
    // Сводка:
    //     Gets the value table of this object. If the value table doesn't exist yet, null
    //     will be returned.
    internal GH_SettingsServer ValueTable => _valueTable;

    public event IGH_DocumentObject.ObjectChangedEventHandler ObjectChanged;

    //
    // Сводка:
    //     Raised whenever the number or kind of attributes changes. This event is handled
    //     by GH_Documents who subsequently wipe their attribute caches.
    //
    // Параметры:
    //   sender:
    //     The object that raised the event.
    //
    //   e:
    //     Details of event
    public event IGH_DocumentObject.AttributesChangedEventHandler AttributesChanged;

    //
    // Сводка:
    //     Raised whenever the solution of a certain object becomes invalid.
    //
    // Параметры:
    //   sender:
    //     The object that raised the event.
    //
    //   e:
    //     Details of event.
    public event IGH_DocumentObject.SolutionExpiredEventHandler SolutionExpired;

    //
    // Сводка:
    //     Raised whenever the display (on the Canvas) of a certain object becomes invalid.
    //
    //
    // Параметры:
    //   sender:
    //     The object that raised the event.
    //
    //   e:
    //     Details of event.
    public event IGH_DocumentObject.DisplayExpiredEventHandler DisplayExpired;

    //
    // Сводка:
    //     Raised whenever the display (in the Rhino viewports) of a certain object becomes
    //     invalid.
    //
    // Параметры:
    //   sender:
    //     The object that raised the event.
    //
    //   e:
    //     Details of event.
    public event IGH_DocumentObject.PreviewExpiredEventHandler PreviewExpired;

    //
    // Сводка:
    //     Raised whenever an object needs to know which GH_Document it belongs to.
    //
    // Параметры:
    //   sender:
    //     Object that sends the ping.
    //
    //   e:
    //     Ping arguments, documents which handle this event should, assign themselves to
    //     the Document field in this argument.
    public event IGH_DocumentObject.PingDocumentEventHandler PingDocument;

    protected GH_DocumentObject(IGH_InstanceDescription tag)
        : base(tag)
    {
        m_icon_index = -1;
        m_icon_index_locked = -1;
        m_icon_mode = GH_IconDisplayMode.application;
        AssertVarParamConflict();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected GH_DocumentObject(string name, string nickname, string description, string category)
        : this(new GH_InstanceDescription(name, nickname, description, category))
    {
        AssertVarParamConflict();
    }

    protected GH_DocumentObject(string name, string nickName, string description, string category, string subCategory)
        : this(new GH_InstanceDescription(name, nickName, description, category, subCategory))
    {
        AssertVarParamConflict();
    }

    private void AssertVarParamConflict()
    {
        if (this is IGH_VarParamComponent && this is IGH_VariableParameterComponent)
        {
            Tracing.Assert(new Guid("{EA7AC890-D80C-42b7-8350-4C223A78BC27}"), "A component cannot implement both IGH_VarParamComponent and IGH_VariableParameterComponent. Please stick to only one of these interfaces, preferably IGH_VariableParameterComponent.");
        }
    }

    //
    // Сводка:
    //     This function creates the stand-alone attributes for this object. If you wish
    //     to supply your own Attributes, you must override this function.
    public abstract void CreateAttributes();

    void IGH_DocumentObject.CreateAttributes()
    {
        //ILSpy generated this explicit interface implementation from .override directive in CreateAttributes
        this.CreateAttributes();
    }

    //
    // Сводка:
    //     Destroy all connections to other objects.
    public virtual void IsolateObject()
    {
    }

    void IGH_DocumentObject.IsolateObject()
    {
        //ILSpy generated this explicit interface implementation from .override directive in IsolateObject
        this.IsolateObject();
    }

    public void OnObjectChanged(GH_ObjectEventType eventType)
    {
        OnObjectChanged(new GH_ObjectChangedEventArgs(this, eventType));
    }

    void IGH_DocumentObject.OnObjectChanged(GH_ObjectEventType eventType)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnObjectChanged
        this.OnObjectChanged(eventType);
    }

    public void OnObjectChanged(string customEvent)
    {
        OnObjectChanged(new GH_ObjectChangedEventArgs(this, customEvent));
    }

    void IGH_DocumentObject.OnObjectChanged(string customEvent)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnObjectChanged
        this.OnObjectChanged(customEvent);
    }

    public void OnObjectChanged(GH_ObjectEventType eventType, object tag)
    {
        OnObjectChanged(new GH_ObjectChangedEventArgs(this, eventType, RuntimeHelpers.GetObjectValue(tag)));
    }

    void IGH_DocumentObject.OnObjectChanged(GH_ObjectEventType eventType, object tag)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnObjectChanged
        this.OnObjectChanged(eventType, tag);
    }

    public void OnObjectChanged(string customEvent, object tag)
    {
        OnObjectChanged(new GH_ObjectChangedEventArgs(this, customEvent, RuntimeHelpers.GetObjectValue(tag)));
    }

    void IGH_DocumentObject.OnObjectChanged(string customEvent, object tag)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnObjectChanged
        this.OnObjectChanged(customEvent, tag);
    }

    public void OnObjectChanged(GH_ObjectChangedEventArgs e)
    {
        if (e == null)
        {
            throw new ArgumentNullException("e");
        }

        try
        {
            ObjectChanged?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            RhinoApp.WriteLine("Exception in the Grasshopper ObjectChanged event:");
            RhinoApp.WriteLine(Environment.StackTrace);
            ProjectData.ClearProjectError();
        }
    }

    void IGH_DocumentObject.OnObjectChanged(GH_ObjectChangedEventArgs e)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnObjectChanged
        this.OnObjectChanged(e);
    }

    //
    // Сводка:
    //     Raises the AttributesChanged event on the toplevel object.
    public void OnAttributesChanged()
    {
        IGH_Attributes attributes = Attributes;
        if (attributes == null)
        {
            AttributesChanged?.Invoke(this, new GH_AttributesChangedEventArgs());
        }
        else if (attributes.IsTopLevel)
        {
            AttributesChanged?.Invoke(this, new GH_AttributesChangedEventArgs());
        }
        else
        {
            attributes.GetTopLevel.DocObject.OnAttributesChanged();
        }
    }

    void IGH_DocumentObject.OnAttributesChanged()
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnAttributesChanged
        this.OnAttributesChanged();
    }

    //
    // Сводка:
    //     Raises the SolutionExpired event on the toplevel object. You probably want to
    //     call ExpireSolution() instead of this method directly.
    //
    // Параметры:
    //   recompute:
    //     If True, the solution will be immediately recalculated.
    public void OnSolutionExpired(bool recompute)
    {
        IGH_Attributes attributes = Attributes;
        if (attributes == null)
        {
            try
            {
                SolutionExpired?.Invoke(this, new GH_SolutionExpiredEventArgs(recompute));
                return;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                RhinoApp.WriteLine("Exception in the Grasshopper SolutionExpired event:");
                RhinoApp.WriteLine(Environment.StackTrace);
                ProjectData.ClearProjectError();
                return;
            }
        }

        if (attributes.IsTopLevel)
        {
            attributes.ExpireLayout();
            try
            {
                SolutionExpired?.Invoke(this, new GH_SolutionExpiredEventArgs(recompute));
                return;
            }
            catch (Exception ex3)
            {
                ProjectData.SetProjectError(ex3);
                Exception ex4 = ex3;
                RhinoApp.WriteLine("Exception in the Grasshopper SolutionExpired event:");
                RhinoApp.WriteLine(Environment.StackTrace);
                ProjectData.ClearProjectError();
                return;
            }
        }

        attributes = attributes.GetTopLevel;
        attributes.DocObject.OnSolutionExpired(recompute);
    }

    void IGH_DocumentObject.OnSolutionExpired(bool recompute)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnSolutionExpired
        this.OnSolutionExpired(recompute);
    }

    //
    // Сводка:
    //     Call this function whenever you do something which expires the current solution.
    //     This will make sure all caches are erased, all downstream objects are expired
    //     and that the event is raised. The default implementation merely places a call
    //     to OnSolutionExpired(), override this function in derived classes to make sure
    //     you clear local data caches and expire downstream objects.
    //
    // Параметры:
    //   recompute:
    //     If True, the solution will be recomputed straight away.
    public virtual void ExpireSolution(bool recompute)
    {
        OnSolutionExpired(recompute);
    }

    void IGH_DocumentObject.ExpireSolution(bool recompute)
    {
        //ILSpy generated this explicit interface implementation from .override directive in ExpireSolution
        this.ExpireSolution(recompute);
    }

    //
    // Сводка:
    //     Raises the DisplayExpired event on the toplevel object.
    //
    // Параметры:
    //   redraw:
    //     If True, the canvas will be immediately redrawn.
    public void OnDisplayExpired(bool redraw)
    {
        IGH_Attributes attributes = Attributes;
        if (attributes == null)
        {
            DisplayExpired?.Invoke(this, new GH_DisplayExpiredEventArgs(redraw));
        }
        else if (attributes.IsTopLevel)
        {
            DisplayExpired?.Invoke(this, new GH_DisplayExpiredEventArgs(redraw));
        }
        else
        {
            attributes.GetTopLevel.DocObject.OnDisplayExpired(redraw);
        }
    }

    void IGH_DocumentObject.OnDisplayExpired(bool redraw)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnDisplayExpired
        this.OnDisplayExpired(redraw);
    }

    //
    // Сводка:
    //     Raises the PreviewExpired event on the toplevel object.
    //
    // Параметры:
    //   redraw:
    //     If True, the viewports will be immediately redrawn.
    public void OnPreviewExpired(bool redraw)
    {
        IGH_Attributes attributes = Attributes;
        if (attributes == null)
        {
            PreviewExpired?.Invoke(this, new GH_PreviewExpiredEventArgs(redraw));
        }
        else if (attributes.IsTopLevel)
        {
            PreviewExpired?.Invoke(this, new GH_PreviewExpiredEventArgs(redraw));
        }
        else
        {
            attributes.GetTopLevel.DocObject.OnPreviewExpired(redraw);
        }
    }

    void IGH_DocumentObject.OnPreviewExpired(bool redraw)
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnPreviewExpired
        this.OnPreviewExpired(redraw);
    }

    //
    // Сводка:
    //     Call this function when you suspect that the preview has expired for this object.
    //     This will cause the display cache to be eradicated.
    public virtual void ExpirePreview(bool redraw)
    {
        OnPreviewExpired(redraw);
    }

    void IGH_DocumentObject.ExpirePreview(bool redraw)
    {
        //ILSpy generated this explicit interface implementation from .override directive in ExpirePreview
        this.ExpirePreview(redraw);
    }

    //
    // Сводка:
    //     Raise the PingDocument Event on the toplevel object and try to find the document
    //     which owns this object.
    //
    // Возврат:
    //     The document which owns this object if successful, or nothing if no document
    //     owns this object.
    public GH_Document OnPingDocument()
    {
        IGH_Attributes attributes = Attributes;
        if (attributes == null)
        {
            GH_PingDocumentEventArgs gH_PingDocumentEventArgs = new GH_PingDocumentEventArgs();
            PingDocument?.Invoke(this, gH_PingDocumentEventArgs);
            return gH_PingDocumentEventArgs.Document;
        }

        if (attributes.IsTopLevel)
        {
            GH_PingDocumentEventArgs gH_PingDocumentEventArgs2 = new GH_PingDocumentEventArgs();
            PingDocument?.Invoke(this, gH_PingDocumentEventArgs2);
            return gH_PingDocumentEventArgs2.Document;
        }

        return attributes.GetTopLevel.DocObject.OnPingDocument();
    }

    GH_Document IGH_DocumentObject.OnPingDocument()
    {
        //ILSpy generated this explicit interface implementation from .override directive in OnPingDocument
        return this.OnPingDocument();
    }

    //
    // Сводка:
    //     This method will be called when an object is added to a document. Override this
    //     method if you want to handle such events.
    //
    // Параметры:
    //   document:
    //     Document that now owns this object.
    public virtual void AddedToDocument(GH_Document document)
    {
    }

    void IGH_DocumentObject.AddedToDocument(GH_Document document)
    {
        //ILSpy generated this explicit interface implementation from .override directive in AddedToDocument
        this.AddedToDocument(document);
    }

    //
    // Сводка:
    //     This method will be called when an object is removed from a document. Override
    //     this method if you want to handle such events.
    //
    // Параметры:
    //   document:
    //     Document that now no longer owns this object.
    public virtual void RemovedFromDocument(GH_Document document)
    {
    }

    void IGH_DocumentObject.RemovedFromDocument(GH_Document document)
    {
        //ILSpy generated this explicit interface implementation from .override directive in RemovedFromDocument
        this.RemovedFromDocument(document);
    }

    //
    // Сводка:
    //     This method will be called when an object is moved from one document to another.
    //     Override this method if you want to handle such events.
    //
    // Параметры:
    //   oldDocument:
    //     Document that used to own this object.
    //
    //   newDocument:
    //     Document that now owns ths object.
    public virtual void MovedBetweenDocuments(GH_Document oldDocument, GH_Document newDocument)
    {
    }

    void IGH_DocumentObject.MovedBetweenDocuments(GH_Document oldDocument, GH_Document newDocument)
    {
        //ILSpy generated this explicit interface implementation from .override directive in MovedBetweenDocuments
        this.MovedBetweenDocuments(oldDocument, newDocument);
    }

    //
    // Сводка:
    //     This method will be called when the document that owns this object moves into
    //     a different context.
    //
    // Параметры:
    //   document:
    //     Document that owns this object.
    //
    //   context:
    //     The reason for this event.
    public virtual void DocumentContextChanged(GH_Document document, GH_DocumentContext context)
    {
    }

    void IGH_DocumentObject.DocumentContextChanged(GH_Document document, GH_DocumentContext context)
    {
        //ILSpy generated this explicit interface implementation from .override directive in DocumentContextChanged
        this.DocumentContextChanged(document, context);
    }

    //
    // Сводка:
    //     Triggers the AutoSave function on the owner document with the object_changed
    //     flag.
    public void TriggerAutoSave()
    {
        OnPingDocument()?.AutoSave(GH_AutoSaveTrigger.object_generic);
    }

    void IGH_DocumentObject.TriggerAutoSave()
    {
        //ILSpy generated this explicit interface implementation from .override directive in TriggerAutoSave
        this.TriggerAutoSave();
    }

    //
    // Сводка:
    //     Triggers the AutoSave function on the owner document with a custom flag.
    //
    // Параметры:
    //   trigger:
    //     Reason for the autosave operation. It is possible that a user has decided to
    //     avoid autosave events for specific types, so if you can, try and provide a correct
    //     trigger flag.
    public void TriggerAutoSave(GH_AutoSaveTrigger trigger)
    {
        OnPingDocument()?.AutoSave(trigger);
    }

    void IGH_DocumentObject.TriggerAutoSave(GH_AutoSaveTrigger trigger)
    {
        //ILSpy generated this explicit interface implementation from .override directive in TriggerAutoSave
        this.TriggerAutoSave(trigger);
    }

    //
    // Сводка:
    //     Triggers the AutoSave function on the owner document with the object_changed
    //     flag.
    //
    // Параметры:
    //   id:
    //     ID of autosave event. Consecutive autosave requests with the same ID will be
    //     ignored.
    public void TriggerAutoSave(Guid id)
    {
        OnPingDocument()?.AutoSave(GH_AutoSaveTrigger.object_generic, id);
    }

    void IGH_DocumentObject.TriggerAutoSave(Guid id)
    {
        //ILSpy generated this explicit interface implementation from .override directive in TriggerAutoSave
        this.TriggerAutoSave(id);
    }

    //
    // Сводка:
    //     Triggers the AutoSave function on the owner document with a custom flag.
    //
    // Параметры:
    //   trigger:
    //     Reason for the autosave operation. It is possible that a user has decided to
    //     avoid autosave events for specific types, so if you can, try and provide a correct
    //     trigger flag.
    //
    //   id:
    //     ID of autosave event. Consecutive autosave requests with the same ID will be
    //     ignored.
    public void TriggerAutoSave(GH_AutoSaveTrigger trigger, Guid id)
    {
        OnPingDocument()?.AutoSave(trigger, id);
    }

    void IGH_DocumentObject.TriggerAutoSave(GH_AutoSaveTrigger trigger, Guid id)
    {
        //ILSpy generated this explicit interface implementation from .override directive in TriggerAutoSave
        this.TriggerAutoSave(trigger, id);
    }

    //
    // Сводка:
    //     Record a generic object change undo event.
    //
    // Параметры:
    //   undoName:
    //     Name of undo record.
    //
    // Возврат:
    //     The ID of the newly added record or Guid.Empty on failure.
    public Guid RecordUndoEvent(string undoName)
    {
        if (undoName == null)
        {
            undoName = string.Empty;
        }

        GH_Document gH_Document = OnPingDocument();
        if (gH_Document != null)
        {
            try
            {
                return gH_Document.UndoUtil.RecordGenericObjectEvent(undoName, this);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                Tracing.Assert(new Guid("{E76497B5-3E6E-4a5e-B393-4D3BCEC03D1A}"), "Undo recording threw an exception: " + ex2.Message);
                ProjectData.ClearProjectError();
            }
        }

        return Guid.Empty;
    }

    Guid IGH_DocumentObject.RecordUndoEvent(string undoName)
    {
        //ILSpy generated this explicit interface implementation from .override directive in RecordUndoEvent
        return this.RecordUndoEvent(undoName);
    }

    //
    // Сводка:
    //     Record a specific object change undo event.
    //
    // Параметры:
    //   undoName:
    //     Name of undo record.
    //
    //   action:
    //     Undo action to record.
    //
    // Возврат:
    //     The ID of the newly added record or Guid.Empty on failure.
    public Guid RecordUndoEvent(string undoName, IGH_UndoAction action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        if (undoName == null)
        {
            undoName = string.Empty;
        }

        return OnPingDocument()?.UndoUtil.RecordEvent(undoName, action) ?? Guid.Empty;
    }

    Guid IGH_DocumentObject.RecordUndoEvent(string undoName, IGH_UndoAction action)
    {
        //ILSpy generated this explicit interface implementation from .override directive in RecordUndoEvent
        return this.RecordUndoEvent(undoName, action);
    }

    //
    // Сводка:
    //     Record an entire undo record.
    //
    // Параметры:
    //   record:
    //     Record to push.
    public void RecordUndoEvent(GH_UndoRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException("record");
        }

        if (record.Name == null)
        {
            record.Name = string.Empty;
        }

        GH_Document gH_Document = OnPingDocument();
        if (gH_Document != null)
        {
            try
            {
                gH_Document.UndoServer.PushUndoRecord(record);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                Tracing.Assert(new Guid("{E76497B5-3E6E-4a5e-B393-4D3BCEC03D1A}"), "Undo recording threw an exception: " + ex2.Message);
                ProjectData.ClearProjectError();
            }
        }
    }

    void IGH_DocumentObject.RecordUndoEvent(GH_UndoRecord record)
    {
        //ILSpy generated this explicit interface implementation from .override directive in RecordUndoEvent
        this.RecordUndoEvent(record);
    }

    //
    // Сводка:
    //     This function is called when a context menu is about to be displayed. Override
    //     it to set custom items.
    //
    // Параметры:
    //   menu:
    //     Menu object to populate.
    //
    // Возврат:
    //     If true, the menu will be displayed, if false the menu will be supressed.
    public virtual bool AppendMenuItems(ToolStripDropDown menu)
    {
        return false;
    }

    bool IGH_DocumentObject.AppendMenuItems(ToolStripDropDown menu)
    {
        //ILSpy generated this explicit interface implementation from .override directive in AppendMenuItems
        return this.AppendMenuItems(menu);
    }

    //
    // Сводка:
    //     Utility function for appending separators to a menu dropdown. If the dropdown
    //     is empty or if it already has a separator at the bottom, nothing will happen.
    //
    //
    // Параметры:
    //   menu:
    //     Menu to append separator to.
    //
    // Возврат:
    //     The appended separator or null if no separator was appended.
    public static ToolStripSeparator Menu_AppendSeparator(ToolStrip menu)
    {
        if (menu.Items.Count == 0)
        {
            return null;
        }

        if (menu.Items[menu.Items.Count - 1] is ToolStripSeparator)
        {
            return null;
        }

        ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
        menu.Items.Add(toolStripSeparator);
        return toolStripSeparator;
    }

    //
    // Сводка:
    //     This method is obsolete.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method is obsolete")]
    public static ToolStripMenuItem Menu_AppendGenericMenuItem(ToolStrip menu, string text, EventHandler on_click = null, Image image = null, object tag = null, bool enabled = true, bool @checked = false)
    {
        ToolStripMenuItem toolStripMenuItem = Menu_AppendItem(menu, text, on_click, image, enabled, @checked);
        toolStripMenuItem.Tag = RuntimeHelpers.GetObjectValue(tag);
        return toolStripMenuItem;
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text)
    {
        return Menu_AppendItem(menu, text, null, null, enabled: true, @checked: false);
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events. If nothing, the click event will not be handled.
    //
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click)
    {
        return Menu_AppendItem(menu, text, click, null, enabled: true, @checked: false);
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events. If nothing, the click event will not be handled.
    //
    //
    //   enabled:
    //     If true, the item will be enabled.
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click, bool enabled)
    {
        return Menu_AppendItem(menu, text, click, null, enabled, @checked: false);
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events. If nothing, the click event will not be handled.
    //
    //
    //   enabled:
    //     If true, the item will be enabled.
    //
    //   checked:
    //     If true, the item will be checked.
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click, bool enabled, bool @checked)
    {
        return Menu_AppendItem(menu, text, click, null, enabled, @checked);
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events. If nothing, the click event will not be handled.
    //
    //
    //   icon:
    //     Item icon, or null for no icon.
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click, Image icon)
    {
        return Menu_AppendItem(menu, text, click, icon, enabled: true, @checked: false);
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events. If nothing, the click event will not be handled.
    //
    //
    //   icon:
    //     Item icon, or null for no icon.
    //
    //   tag:
    //     Tag to assign to menu item.
    //
    // Возврат:
    //     The appended item
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click, Image icon, object tag)
    {
        ToolStripMenuItem toolStripMenuItem = Menu_AppendItem(menu, text, click, icon, enabled: true, @checked: false);
        toolStripMenuItem.Tag = RuntimeHelpers.GetObjectValue(tag);
        return toolStripMenuItem;
    }

    //
    // Сводка:
    //     Utility function for appending generic menu items.
    //
    // Параметры:
    //   menu:
    //     Menu to append item to.
    //
    //   text:
    //     Item text.
    //
    //   click:
    //     Delegate that handles click events.
    //
    //   icon:
    //     Item icon.
    //
    //   enabled:
    //     If true, the item will be enabled.
    //
    //   checked:
    //     If true, the item will be checked.
    //
    // Возврат:
    //     The appended item.
    public static ToolStripMenuItem Menu_AppendItem(ToolStrip menu, string text, EventHandler click, Image icon, bool enabled, bool @checked)
    {
        text = text.Replace("...", "…");
        ToolStripMenuItem toolStripMenuItem = ((click != null) ? new ToolStripMenuItem(text, icon, click) : new ToolStripMenuItem(text, icon));
        toolStripMenuItem.Enabled = enabled;
        toolStripMenuItem.Checked = @checked;
        toolStripMenuItem.ImageScaling = ToolStripItemImageScaling.SizeToFit;
        menu.Items.Add(toolStripMenuItem);
        return toolStripMenuItem;
    }

    //
    // Сводка:
    //     Utility function for moving menu items.
    //
    // Параметры:
    //   item:
    //     Item to move, it must already be part of a dropdown menu.
    //
    //   precedingItems:
    //     Names of preceding items, the item will be inserted below the first name, if
    //     that fails the second, etc. etc.
    //
    // Возврат:
    //     True on success, false on failure.
    public static bool Menu_MoveItem(ToolStripItem item, params string[] precedingItems)
    {
        return Menu_MoveItem(item, insertBelow: true, precedingItems);
    }

    //
    // Сводка:
    //     Utility function for moving menu items.
    //
    // Параметры:
    //   item:
    //     Item to move, it must already be part of the dropdown menu.
    //
    //   insertBelow:
    //     If true, the item will be inserted below the match, if false, above the match.
    //
    //
    //   targets:
    //     Names of move to targets, the item will be inserted next to the first name, if
    //     that fails the second, etc. etc.
    //
    // Возврат:
    //     True on success, false on failure.
    public static bool Menu_MoveItem(ToolStripItem item, bool insertBelow, params string[] targets)
    {
        if (item == null)
        {
            return false;
        }

        if (targets == null)
        {
            return false;
        }

        if (targets.Length == 0)
        {
            return false;
        }

        ToolStrip owner = item.Owner;
        if (owner == null)
        {
            return false;
        }

        int num = owner.Items.IndexOf(item);
        if (num < 0)
        {
            return false;
        }

        owner.Items.Remove(item);
        foreach (string value in targets)
        {
            int num2 = owner.Items.Count - 1;
            for (int j = 0; j <= num2; j++)
            {
                ToolStripItem toolStripItem = owner.Items[j];
                if (toolStripItem != item && toolStripItem.Text.Equals(value, StringComparison.Ordinal))
                {
                    if (insertBelow)
                    {
                        owner.Items.Insert(j + 1, item);
                    }
                    else
                    {
                        owner.Items.Insert(j, item);
                    }

                    return true;
                }
            }
        }

        owner.Items.Insert(num, item);
        return false;
    }

    //
    // Сводка:
    //     Utility function for inserting text boxes into menus.
    //
    // Параметры:
    //   menu:
    //     Menu to add a textbox to.
    //
    //   text:
    //     Content of textbox.
    //
    //   keydown:
    //     Delegate for the KeyDown event.
    //
    //   textchanged:
    //     Delegate for the TextChanged event.
    //
    //   lockOnFocus:
    //     If true, then a GotFocus event will lock the other menu items.
    //
    // Возврат:
    //     The appended text box item.
    public static ToolStripTextBox Menu_AppendTextItem(ToolStripDropDown menu, string text, GH_MenuTextBox.KeyDownEventHandler keydown, GH_MenuTextBox.TextChangedEventHandler textchanged, bool lockOnFocus)
    {
        return Menu_AppendTextItem(menu, text, keydown, textchanged, enabled: true, -1, lockOnFocus);
    }

    //
    // Сводка:
    //     Utility function for inserting text boxes into menus.
    //
    // Параметры:
    //   menu:
    //     Menu to add a textbox to.
    //
    //   text:
    //     Content of textbox.
    //
    //   keydown:
    //     Delegate for the KeyDown event.
    //
    //   textchanged:
    //     Delegate for the TextChanged event.
    //
    //   enabled:
    //     If true, the textbox will be enabled.
    //
    //   width:
    //     If larger than zero, we'll do our very best to grow the menu to the given width.
    //     No guarantees.
    //
    //   lockOnFocus:
    //     If true, then a GotFocus event will lock the other menu items.
    //
    // Возврат:
    //     The appended text box item.
    public static ToolStripTextBox Menu_AppendTextItem(ToolStripDropDown menu, string text, GH_MenuTextBox.KeyDownEventHandler keydown, GH_MenuTextBox.TextChangedEventHandler textchanged, bool enabled, int width, bool lockOnFocus)
    {
        GH_MenuTextBox gH_MenuTextBox = new GH_MenuTextBox(menu, text, lockOnFocus);
        if (width > 0)
        {
            gH_MenuTextBox.Width = width;
        }
        else
        {
            gH_MenuTextBox.Width = Global_Proc.UiAdjust(200);
        }

        gH_MenuTextBox.TextBoxItem.Enabled = enabled;
        if (keydown != null)
        {
            gH_MenuTextBox.KeyDown += keydown;
        }

        if (textchanged != null)
        {
            gH_MenuTextBox.TextChanged += textchanged;
        }

        return gH_MenuTextBox.TextBoxItem;
    }

    //
    // Сводка:
    //     Utility function for inserting a digit scroller into menus.
    //
    // Параметры:
    //   menu:
    //
    // Возврат:
    //     The digit scroller object that was inserted into the menu.
    public static GH_DigitScrollerBase Menu_AppendDigitScrollerItem(ToolStripDropDown menu, decimal min, decimal max, decimal value, int decimals)
    {
        return new GH_DigitScroller
        {
            MinimumValue = min,
            MaximumValue = max,
            DecimalPlaces = decimals,
            Value = value
        }.DigitScroller;
    }

    //
    // Сводка:
    //     Utility function for inserting exotic controls into dropdown menus.
    //
    // Параметры:
    //   menu:
    //     Dropdown to add a control to.
    //
    //   control:
    //     Control to insert.
    public static bool Menu_AppendCustomItem(ToolStripDropDown menu, Control control)
    {
        return Menu_AppendCustomItem(menu, control, null, enabled: true, control.Width, lockOnFocus: false);
    }

    //
    // Сводка:
    //     Utility function for inserting exotic controls into dropdown menus.
    //
    // Параметры:
    //   menu:
    //     Dropdown to add a control to.
    //
    //   control:
    //     Control to insert.
    //
    //   keydown:
    //     Delegate for key-presses.
    public static bool Menu_AppendCustomItem(ToolStripDropDown menu, Control control, GH_MenuCustomControl.KeyDownEventHandler keydown)
    {
        return Menu_AppendCustomItem(menu, control, keydown, enabled: true, control.Width, keydown != null);
    }

    //
    // Сводка:
    //     Utility function for inserting exotic controls into dropdown menus.
    //
    // Параметры:
    //   menu:
    //     Dropdown to add a control to.
    //
    //   control:
    //     Control to insert.
    //
    //   keydown:
    //     Delegate for key-presses.
    //
    //   enabled:
    //     If true, the control will be enabled.
    //
    //   width:
    //     Width hint for the control.
    //
    //   lockOnFocus:
    //     If true and the control gets focus, the rest of the menu will be locked.
    public static bool Menu_AppendCustomItem(ToolStripDropDown menu, Control control, GH_MenuCustomControl.KeyDownEventHandler keydown, bool enabled, int width, bool lockOnFocus)
    {
        if (menu == null)
        {
            return false;
        }

        if (control == null)
        {
            return false;
        }

        if (keydown == null)
        {
            GH_MenuCustomControl gH_MenuCustomControl = new GH_MenuCustomControl(menu, control, lockOnFocus, width);
            gH_MenuCustomControl.Control.Enabled = enabled;
            gH_MenuCustomControl.CommitItem.Visible = false;
            gH_MenuCustomControl.CancelItem.Visible = false;
            gH_MenuCustomControl.CommitItem.Enabled = false;
            gH_MenuCustomControl.CancelItem.Enabled = false;
            return true;
        }

        GH_MenuCustomControl gH_MenuCustomControl2 = new GH_MenuCustomControl(menu, control, lockOnFocus, width);
        gH_MenuCustomControl2.Control.Enabled = enabled;
        gH_MenuCustomControl2.CommitItem.Visible = true;
        gH_MenuCustomControl2.CancelItem.Visible = true;
        gH_MenuCustomControl2.CommitItem.Enabled = true;
        gH_MenuCustomControl2.CancelItem.Enabled = true;
        gH_MenuCustomControl2.KeyDown += keydown;
        return true;
    }

    //
    // Сводка:
    //     Add a colour picker to a menu.
    //
    // Параметры:
    //   menu:
    //     Menu to add to.
    //
    //   colour:
    //     Default colour.
    //
    //   colourChanged:
    //     Handler for Colour picker changed events.
    //
    // Возврат:
    //     Menu item representing the colour picker.
    public static GH_ColourPicker Menu_AppendColourPicker(ToolStripDropDown menu, Color colour, ColourEventHandler colourChanged)
    {
        GH_ColourPicker gH_ColourPicker = new GH_ColourPicker();
        gH_ColourPicker.BackColor = SystemColors.Window;
        gH_ColourPicker.Padding = new Padding(Global_Proc.UiAdjust(10));
        gH_ColourPicker.Font = menu.Font;
        gH_ColourPicker.Width = Global_Proc.UiAdjust(200);
        gH_ColourPicker.Height = gH_ColourPicker.DesiredHeight;
        gH_ColourPicker.Colour = colour;
        gH_ColourPicker.Tag = colourChanged;
        gH_ColourPicker.ColourChanged += Menu_ColourPickerEventHandler;
        Menu_AppendCustomItem(menu, gH_ColourPicker, null, enabled: true, Global_Proc.UiAdjust(300), lockOnFocus: false);
        return gH_ColourPicker;
    }

    private static void Menu_ColourPickerEventHandler(object sender, GH_ColourPickerEventArgs e)
    {
        GH_ColourPicker gH_ColourPicker = (GH_ColourPicker)sender;
        if (gH_ColourPicker.Tag != null && gH_ColourPicker.Tag is ColourEventHandler)
        {
            ((ColourEventHandler)gH_ColourPicker.Tag)(gH_ColourPicker, e);
        }
    }

    //
    // Сводка:
    //     Appends the default object name + display mode menu item.
    protected void Menu_AppendObjectNameEx(ToolStripDropDown menu)
    {
        GH_NickNameTextBox gH_NickNameTextBox = new GH_NickNameTextBox();
        gH_NickNameTextBox.OldNickName = NickName;
        gH_NickNameTextBox.OldIconMode = m_icon_mode;
        if (this is IGH_ActiveObject iGH_ActiveObject)
        {
            gH_NickNameTextBox.Enabled = iGH_ActiveObject.MutableNickName;
        }

        gH_NickNameTextBox.MinimumSize = new Size(200, 20);
        gH_NickNameTextBox.BackColor = Color.White;
        gH_NickNameTextBox.Dock = DockStyle.Fill;
        gH_NickNameTextBox.NickNameChanged += Menu_NickNameChanged;
        gH_NickNameTextBox.IconModeChanged += Menu_IconModeChanged;
        gH_NickNameTextBox.NickNameChangeAccepted += Menu_NickNameChangeAccepted;
        gH_NickNameTextBox.NickNameChangeCancelled += Menu_NickNameChangeCancelled;
        ToolStripControlHost toolStripControlHost = new ToolStripControlHost(gH_NickNameTextBox);
        toolStripControlHost.BackColor = Color.White;
        toolStripControlHost.AutoSize = false;
        toolStripControlHost.Height += 3;
        menu.Items.Add(toolStripControlHost);
    }

    private void Menu_NickNameChanged(object sender, string newName)
    {
        OnPingDocument()?.UndoUtil.RecordNickNameEvent("Name change", this);
        NickName = newName;
        Attributes.GetTopLevel.ExpireLayout();
        OnObjectChanged(GH_ObjectEventType.NickName, newName);
        Instances.RedrawCanvas();
    }

    private void Menu_IconModeChanged(object sender, GH_IconDisplayMode newMode)
    {
        RecordUndoEvent("Icon mode", new GH_IconDisplayAction(this));
        m_icon_mode = newMode;
        Attributes.GetTopLevel.ExpireLayout();
        OnObjectChanged(GH_ObjectEventType.Layout);
        Instances.RedrawCanvas();
    }

    private void Menu_NickNameChangeAccepted(GH_NickNameTextBox sender)
    {
        Control parent = sender.Parent;
        if (parent != null && parent is ContextMenuStrip && parent is ContextMenuStrip contextMenuStrip)
        {
            contextMenuStrip.Dispose();
        }
    }

    private void Menu_NickNameChangeCancelled(GH_NickNameTextBox sender)
    {
        Control parent = sender.Parent;
        if (parent != null && parent is ContextMenuStrip)
        {
            if (parent is ContextMenuStrip contextMenuStrip)
            {
                contextMenuStrip.Dispose();
            }

            NickName = sender.OldNickName;
            m_icon_mode = sender.OldIconMode;
            Attributes.ExpireLayout();
            OnObjectChanged(GH_ObjectEventType.NickName, NickName);
            Instances.RedrawCanvas();
        }
    }

    //
    // Сводка:
    //     Appends the old-fashioned object name menu item. If you also want the Display
    //     mode toggle then use Menu_AppendObjectNameEx()
    protected void Menu_AppendObjectName(ToolStripDropDown menu)
    {
        ToolStripTextBox toolStripTextBox = new ToolStripTextBox(NickName);
        toolStripTextBox.TextBox.Width = Global_Proc.UiAdjust(100);
        toolStripTextBox.Text = NickName;
        toolStripTextBox.Tag = NickName;
        if (this is IGH_ActiveObject iGH_ActiveObject)
        {
            toolStripTextBox.Enabled = iGH_ActiveObject.MutableNickName;
        }

        toolStripTextBox.TextChanged += Menu_NameItemTextChanged;
        toolStripTextBox.KeyDown += Menu_NameItemKeyDown;
        menu.Items.Add(toolStripTextBox);
    }

    private void Menu_NameItemTextChanged(object sender, EventArgs e)
    {
        try
        {
            ToolStripTextBox toolStripTextBox = (ToolStripTextBox)sender;
            OnPingDocument()?.UndoUtil.RecordNickNameEvent("Name change", this);
            NickName = toolStripTextBox.Text;
            Attributes.GetTopLevel.ExpireLayout();
            OnObjectChanged(GH_ObjectEventType.NickName, NickName);
            Instances.RedrawCanvas();
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }
    }

    private void Menu_NameItemKeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            ToolStripTextBox toolStripTextBox = (ToolStripTextBox)sender;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    NickName = $"{RuntimeHelpers.GetObjectValue(toolStripTextBox.Tag)}";
                    break;
                case Keys.Return:
                    OnObjectChanged(GH_ObjectEventType.NickNameAccepted);
                    break;
                default:
                    return;
            }

            toolStripTextBox.Owner.Dispose();
            Attributes.GetTopLevel.ExpireLayout();
            Instances.RedrawCanvas();
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }
    }

    //
    // Сводка:
    //     Appends the default item for publishing to RCP. This menu will only appear if
    //     the current class implement IRcpAwareObject
    protected void Menu_AppendPublish(ToolStripDropDown menu)
    {
        if (this is IRcpAwareObject)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Publish To Remote Panel");
            toolStripMenuItem.ToolTipText = "Publish this item to the remote control panel.";
            toolStripMenuItem.Click += Menu_PublishObjectClick;
            menu.Items.Add(toolStripMenuItem);
        }
    }

    private void Menu_PublishObjectClick(object sender, EventArgs e)
    {
        GH_Document gH_Document = OnPingDocument();
        if (gH_Document != null && this is IRcpAwareObject rcpAwareObject)
        {
            IRcpItem rcpItem = rcpAwareObject.PublishRcpItem();
            if (rcpItem != null)
            {
                RcpGroup rcpGroup = gH_Document.RemotePanelLayout.EnsureGroup();
                rcpGroup.Expand();
                rcpGroup.AddItem(rcpItem);
                gH_Document.RemotePanelLayout.OnLayoutChanged();
            }
        }
    }

    //
    // Сводка:
    //     Return a String which contains HTML formatted source for the help topic. If you
    //     want to pass a URL that points to a remote page, then prefix the URL with a GOTO:
    //     tag, like so: GOTO:http://www.YourWebAddressHere.com
    //
    // Примечания:
    //     You can use the GUI.GH_HtmlFormatter class to generate an HTML page which conforms
    //     to Grasshopper Help standards
    protected internal virtual string HtmlHelp_Source()
    {
        GH_HtmlFormatter gH_HtmlFormatter = new GH_HtmlFormatter(this, Name, Description);
        gH_HtmlFormatter.AddRemark("This is the autogenerated help topic for this object. " + Environment.NewLine + "Developers: override the HtmlHelp_Source() function in the base class to provide custom help.", GH_HtmlFormatterPalette.Gray);
        return gH_HtmlFormatter.HtmlFormat();
    }

    //
    // Сводка:
    //     Appends the default object Help menu item.
    protected void Menu_AppendObjectHelp(ToolStripDropDown menu)
    {
        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Help…", Res_ContextMenu.Help_16x16);
        toolStripMenuItem.Click += Menu_ObjectHelpClick;
        menu.Items.Add(toolStripMenuItem);
    }

    private void Menu_ObjectHelpClick(object sender, EventArgs e)
    {
        GH_HtmlHelpPopup gH_HtmlHelpPopup = new GH_HtmlHelpPopup();
        if (gH_HtmlHelpPopup.LoadObject(this))
        {
            gH_HtmlHelpPopup.SetLocation(Cursor.Position);
            gH_HtmlHelpPopup.Show(Instances.DocumentEditor);
        }
    }

    //
    // Сводка:
    //     Call this method to erase the existing icon cache. You must call this if you
    //     want to change the display icon of an object.
    protected void DestroyIconCache()
    {
        m_icon_index = -1;
        m_icon_index_locked = -1;
    }

    //
    // Сводка:
    //     Set a new custom icon override for this object.
    //
    // Параметры:
    //   customIcon:
    //     Icon override. Should be a 24x24 image.
    public void SetIconOverride(Bitmap customIcon)
    {
        m_icon_override = customIcon;
        DestroyIconCache();
        OnObjectChanged(GH_ObjectEventType.Icon);
    }

    //
    // Сводка:
    //     Ensures the existence of the value table.
    internal void EnsureValueTable()
    {
        if (_valueTable == null)
        {
            _valueTable = new GH_SettingsServer(base.InstanceGuid.ToString(), loadSettings: false);
        }
    }

    //
    // Сводка:
    //     Override this method if you want to respond to changes to the value table. The
    //     base implementation is empty, so you don't have to call it.
    protected virtual void ValuesChanged()
    {
    }

    //
    // Сводка:
    //     Get a boolean value from the component value table.
    //
    // Параметры:
    //   valueName:
    //     Name of boolean to retrieve.
    //
    //   default:
    //     Default value to return in case of missing named value.
    //
    // Возврат:
    //     The boolean value with the given name of the default value.
    protected bool GetValue(string valueName, bool @default)
    {
        if (_valueTable == null)
        {
            return @default;
        }

        if (_valueTable.ConstainsEntry(valueName))
        {
            return _valueTable.GetValue(valueName, @default);
        }

        return @default;
    }

    //
    // Сводка:
    //     Get an integer value from the component value table.
    //
    // Параметры:
    //   valueName:
    //     Name of integer to retrieve.
    //
    //   default:
    //     Default value to return in case of missing named value.
    //
    // Возврат:
    //     The integer value with the given name of the default value.
    protected int GetValue(string valueName, int @default)
    {
        if (_valueTable == null)
        {
            return @default;
        }

        if (_valueTable.ConstainsEntry(valueName))
        {
            return _valueTable.GetValue(valueName, @default);
        }

        return @default;
    }

    //
    // Сводка:
    //     Get a double value from the component value table.
    //
    // Параметры:
    //   valueName:
    //     Name of double to retrieve.
    //
    //   default:
    //     Default value to return in case of missing named value.
    //
    // Возврат:
    //     The double value with the given name of the default value.
    protected double GetValue(string valueName, double @default)
    {
        if (_valueTable == null)
        {
            return @default;
        }

        if (_valueTable.ConstainsEntry(valueName))
        {
            return _valueTable.GetValue(valueName, @default);
        }

        return @default;
    }

    //
    // Сводка:
    //     Get a string value from the component value table.
    //
    // Параметры:
    //   valueName:
    //     Name of string to retrieve.
    //
    //   default:
    //     Default value to return in case of missing named value.
    //
    // Возврат:
    //     The string value with the given name of the default value.
    protected string GetValue(string valueName, string @default)
    {
        if (_valueTable == null)
        {
            return @default;
        }

        if (_valueTable.ConstainsEntry(valueName))
        {
            return _valueTable.GetValue(valueName, @default);
        }

        return @default;
    }

    //
    // Сводка:
    //     Get a color value from the component value table.
    //
    // Параметры:
    //   valueName:
    //     Name of color to retrieve.
    //
    //   default:
    //     Default value to return in case of missing named value.
    //
    // Возврат:
    //     The color value with the given name of the default value.
    protected Color GetValue(string valueName, Color @default)
    {
        if (_valueTable == null)
        {
            return @default;
        }

        if (_valueTable.ConstainsEntry(valueName))
        {
            return _valueTable.GetValue(valueName, @default);
        }

        return @default;
    }

    //
    // Сводка:
    //     Set a named value. This value will be serialized with the component.
    //
    // Параметры:
    //   valueName:
    //     Name of value.
    //
    //   value:
    //     Value itself.
    protected void SetValue(string valueName, bool value)
    {
        EnsureValueTable();
        if (_valueTable.ConstainsEntry(valueName))
        {
            GH_SettingsServer.GH_SingleSetting instance = _valueTable.GetInstance(valueName);
            if (instance.m_typ == GH_SettingsType._boolean && instance._Boolean == value)
            {
                return;
            }
        }

        _valueTable.SetValue(valueName, value);
        ValuesChanged();
    }

    //
    // Сводка:
    //     Set a named value. This value will be serialized with the component.
    //
    // Параметры:
    //   valueName:
    //     Name of value.
    //
    //   value:
    //     Value itself.
    protected void SetValue(string valueName, int value)
    {
        EnsureValueTable();
        if (_valueTable.ConstainsEntry(valueName))
        {
            GH_SettingsServer.GH_SingleSetting instance = _valueTable.GetInstance(valueName);
            if (instance.m_typ == GH_SettingsType._integer && instance._Integer == value)
            {
                return;
            }
        }

        _valueTable.SetValue(valueName, value);
        ValuesChanged();
    }

    //
    // Сводка:
    //     Set a named value. This value will be serialized with the component.
    //
    // Параметры:
    //   valueName:
    //     Name of value.
    //
    //   value:
    //     Value itself.
    protected void SetValue(string valueName, double value)
    {
        EnsureValueTable();
        if (_valueTable.ConstainsEntry(valueName))
        {
            GH_SettingsServer.GH_SingleSetting instance = _valueTable.GetInstance(valueName);
            if (instance.m_typ == GH_SettingsType._double && instance._Double.Equals(value))
            {
                return;
            }
        }

        _valueTable.SetValue(valueName, value);
        ValuesChanged();
    }

    //
    // Сводка:
    //     Set a named value. This value will be serialized with the component.
    //
    // Параметры:
    //   valueName:
    //     Name of value.
    //
    //   value:
    //     Value itself.
    protected void SetValue(string valueName, string value)
    {
        EnsureValueTable();
        if (_valueTable.ConstainsEntry(valueName))
        {
            GH_SettingsServer.GH_SingleSetting instance = _valueTable.GetInstance(valueName);
            if (instance.m_typ == GH_SettingsType._string && Operators.CompareString(instance._String, value, false) == 0)
            {
                return;
            }
        }

        _valueTable.SetValue(valueName, value);
        ValuesChanged();
    }

    //
    // Сводка:
    //     Set a named value. This value will be serialized with the component.
    //
    // Параметры:
    //   valueName:
    //     Name of value.
    //
    //   value:
    //     Value itself.
    protected void SetValue(string valueName, Color value)
    {
        EnsureValueTable();
        if (_valueTable.ConstainsEntry(valueName))
        {
            GH_SettingsServer.GH_SingleSetting instance = _valueTable.GetInstance(valueName);
            if (instance.m_typ == GH_SettingsType._color && instance._Color == value)
            {
                return;
            }
        }

        _valueTable.SetValue(valueName, value);
        ValuesChanged();
    }

    public override bool Write(GH_IWriter writer)
    {
        base.Write(writer);
        switch (m_icon_mode)
        {
            case GH_IconDisplayMode.name:
                writer.SetInt32("IconDisplay", 1);
                break;
            case GH_IconDisplayMode.icon:
                writer.SetInt32("IconDisplay", 2);
                break;
        }

        if (m_icon_override != null)
        {
            writer.SetDrawingBitmap("IconOverride", m_icon_override);
        }

        if (_valueTable != null && _valueTable.Count > 0)
        {
            GH_IWriter writer2 = writer.CreateChunk("ValueTable");
            _valueTable.Write(writer2);
        }

        GH_IWriter writer3 = writer.CreateChunk("Attributes");
        return Attributes.Write(writer3);
    }

    public override bool Read(GH_IReader reader)
    {
        base.Read(reader);
        int value = 0;
        reader.TryGetInt32("IconDisplay", ref value);
        switch (value)
        {
            case 1:
                m_icon_mode = GH_IconDisplayMode.name;
                break;
            case 2:
                m_icon_mode = GH_IconDisplayMode.icon;
                break;
            default:
                m_icon_mode = GH_IconDisplayMode.application;
                break;
        }

        if (reader.ItemExists("IconOverride"))
        {
            SetIconOverride(reader.GetDrawingBitmap("IconOverride"));
        }

        _valueTable = null;
        GH_IReader gH_IReader = reader.FindChunk("ValueTable");
        if (gH_IReader != null)
        {
            EnsureValueTable();
            _valueTable.Read(gH_IReader);
        }

        ValuesChanged();
        if (m_attributes != null)
        {
            GH_IReader gH_IReader2 = reader.FindChunk("Attributes");
            if (gH_IReader2 != null)
            {
                return Attributes.Read(gH_IReader2);
            }

            reader.AddMessage("Attributes chunk is missing. Could be a hint something's wrong.", GH_Message_Type.info);
        }

        return true;
    }
}
