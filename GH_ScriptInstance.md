using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rhino.Geometry;

namespace Grasshopper.Kernel
{
	/// <exclude />
	// Token: 0x020002FF RID: 767
	public abstract class GH_ScriptInstance : IGH_ScriptInstance
	{
		/// <summary>
		/// Override this method when you draw custom geometry.
		/// </summary>
		// Token: 0x17000DE6 RID: 3558
		// (get) Token: 0x06002AB9 RID: 10937 RVA: 0x000D6500 File Offset: 0x000D4700
		public virtual BoundingBox ClippingBox
		{
			get
			{
				return BoundingBox.Empty;
			}
		}

		/// <summary>
		/// Override this method to draw meshes in the Rhino viewports.
		/// </summary>
		// Token: 0x06002ABA RID: 10938 RVA: 0x000D6507 File Offset: 0x000D4707
		public virtual void DrawViewportMeshes(IGH_PreviewArgs args)
		{
		}

		/// <summary>
		/// Override this method to draw points and curves in the Rhino viewports.
		/// </summary>
		// Token: 0x06002ABB RID: 10939 RVA: 0x000D6509 File Offset: 0x000D4709
		public virtual void DrawViewportWires(IGH_PreviewArgs args)
		{
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000D650B File Offset: 0x000D470B
		public virtual void BeforeRunScript()
		{
		}

		// Token: 0x06002ABD RID: 10941 RVA: 0x000D650D File Offset: 0x000D470D
		public virtual void AfterRunScript()
		{
		}

		// Token: 0x17000DE7 RID: 3559
		// (get) Token: 0x06002ABE RID: 10942 RVA: 0x000D650F File Offset: 0x000D470F
		// (set) Token: 0x06002ABF RID: 10943 RVA: 0x000D6512 File Offset: 0x000D4712
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Hidden
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x06002AC0 RID: 10944 RVA: 0x000D6514 File Offset: 0x000D4714
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsPreviewCapable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002AC1 RID: 10945
		public abstract void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA);
	}
}
