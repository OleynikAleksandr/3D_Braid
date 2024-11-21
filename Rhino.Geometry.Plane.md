using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Rhino.Collections;
using Rhino.Runtime;

namespace Rhino.Geometry
{
	/// <summary>
	/// Represents the value of a center point and two axes in a plane in three dimensions.
	/// </summary>
	// Token: 0x02000519 RID: 1305
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 128)]
	public struct Plane : IEquatable<Plane>, IEpsilonComparable<Plane>, ICloneable, IValidable, IFormattable
	{
		/// <summary>
		/// Gets or sets the origin point of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001453 RID: 5203
		// (get) Token: 0x06007164 RID: 29028 RVA: 0x000C63A0 File Offset: 0x000C45A0
		// (set) Token: 0x06007165 RID: 29029 RVA: 0x000C63A8 File Offset: 0x000C45A8
		public Point3d Origin
		{
			get
			{
				return this.m_origin;
			}
			set
			{
				this.m_origin = value;
			}
		}

		/// <summary>
		/// Gets or sets the X coordinate of the origin of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001454 RID: 5204
		// (get) Token: 0x06007166 RID: 29030 RVA: 0x000C63B1 File Offset: 0x000C45B1
		// (set) Token: 0x06007167 RID: 29031 RVA: 0x000C63BE File Offset: 0x000C45BE
		public double OriginX
		{
			get
			{
				return this.m_origin.X;
			}
			set
			{
				this.m_origin.X = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y coordinate of the origin of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001455 RID: 5205
		// (get) Token: 0x06007168 RID: 29032 RVA: 0x000C63CC File Offset: 0x000C45CC
		// (set) Token: 0x06007169 RID: 29033 RVA: 0x000C63D9 File Offset: 0x000C45D9
		public double OriginY
		{
			get
			{
				return this.m_origin.Y;
			}
			set
			{
				this.m_origin.Y = value;
			}
		}

		/// <summary>
		/// Gets or sets the Z coordinate of the origin of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001456 RID: 5206
		// (get) Token: 0x0600716A RID: 29034 RVA: 0x000C63E7 File Offset: 0x000C45E7
		// (set) Token: 0x0600716B RID: 29035 RVA: 0x000C63F4 File Offset: 0x000C45F4
		public double OriginZ
		{
			get
			{
				return this.m_origin.Z;
			}
			set
			{
				this.m_origin.Z = value;
			}
		}

		/// <summary>
		/// Gets or sets the X axis vector of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001457 RID: 5207
		// (get) Token: 0x0600716C RID: 29036 RVA: 0x000C6402 File Offset: 0x000C4602
		// (set) Token: 0x0600716D RID: 29037 RVA: 0x000C640A File Offset: 0x000C460A
		public Vector3d XAxis
		{
			get
			{
				return this.m_xaxis;
			}
			set
			{
				this.m_xaxis = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y axis vector of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001458 RID: 5208
		// (get) Token: 0x0600716E RID: 29038 RVA: 0x000C6413 File Offset: 0x000C4613
		// (set) Token: 0x0600716F RID: 29039 RVA: 0x000C641B File Offset: 0x000C461B
		public Vector3d YAxis
		{
			get
			{
				return this.m_yaxis;
			}
			set
			{
				this.m_yaxis = value;
			}
		}

		/// <summary>
		/// Gets or sets the Z axis vector of this plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001459 RID: 5209
		// (get) Token: 0x06007170 RID: 29040 RVA: 0x000C6424 File Offset: 0x000C4624
		// (set) Token: 0x06007171 RID: 29041 RVA: 0x000C642C File Offset: 0x000C462C
		public Vector3d ZAxis
		{
			get
			{
				return this.m_zaxis;
			}
			set
			{
				this.m_zaxis = value;
			}
		}

		/// <summary>
		/// plane coincident with the World XY plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145A RID: 5210
		// (get) Token: 0x06007172 RID: 29042 RVA: 0x000C6438 File Offset: 0x000C4638
		public static Plane WorldXY
		{
			get
			{
				return new Plane
				{
					XAxis = new Vector3d(1.0, 0.0, 0.0),
					YAxis = new Vector3d(0.0, 1.0, 0.0),
					ZAxis = new Vector3d(0.0, 0.0, 1.0)
				};
			}
		}

		/// <summary>
		/// plane coincident with the World YZ plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145B RID: 5211
		// (get) Token: 0x06007173 RID: 29043 RVA: 0x000C64C4 File Offset: 0x000C46C4
		public static Plane WorldYZ
		{
			get
			{
				return new Plane
				{
					XAxis = new Vector3d(0.0, 1.0, 0.0),
					YAxis = new Vector3d(0.0, 0.0, 1.0),
					ZAxis = new Vector3d(1.0, 0.0, 0.0)
				};
			}
		}

		/// <summary>
		/// plane coincident with the World ZX plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145C RID: 5212
		// (get) Token: 0x06007174 RID: 29044 RVA: 0x000C6550 File Offset: 0x000C4750
		public static Plane WorldZX
		{
			get
			{
				return new Plane
				{
					XAxis = new Vector3d(0.0, 0.0, 1.0),
					YAxis = new Vector3d(1.0, 0.0, 0.0),
					ZAxis = new Vector3d(0.0, 1.0, 0.0)
				};
			}
		}

		/// <summary>
		/// Gets a plane that contains Unset origin and axis vectors.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145D RID: 5213
		// (get) Token: 0x06007175 RID: 29045 RVA: 0x000C65DC File Offset: 0x000C47DC
		public static Plane Unset
		{
			get
			{
				return new Plane
				{
					Origin = Point3d.Unset,
					XAxis = Vector3d.Unset,
					YAxis = Vector3d.Unset,
					ZAxis = Vector3d.Unset
				};
			}
		}

		/// <summary>Copy constructor.
		/// </summary>
		/// <param name="other">The source plane value.</param>
		/// <since>5.0</since>
		// Token: 0x06007176 RID: 29046 RVA: 0x000C6622 File Offset: 0x000C4822
		public Plane(Plane other)
		{
			this = other;
		}

		/// <summary>
		/// Constructs a plane from a point and a normal vector.
		/// </summary>
		/// <param name="origin">Origin point of the plane.</param>
		/// <param name="normal">Non-zero normal to the plane.</param>
		/// <seealso>CreateFromNormal</seealso>
		/// <example>
		/// <code source="examples\vbnet\ex_addcylinder.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_addcylinder.cs" lang="cs" />
		/// <code source="examples\py\ex_addcylinder.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x06007177 RID: 29047 RVA: 0x000C662B File Offset: 0x000C482B
		public Plane(Point3d origin, Vector3d normal)
		{
			this = default(Plane);
			UnsafeNativeMethods.ON_Plane_CreateFromNormal(ref this, origin, normal);
		}

		/// <summary>
		/// Constructs a plane from a point and two vectors in the plane.
		/// </summary>
		/// <param name="origin">Origin point of the plane.</param>
		/// <param name="xDirection">
		/// Non-zero vector in the plane that determines the x-axis direction.
		/// </param>
		/// <param name="yDirection">
		/// Non-zero vector not parallel to x_dir that is used to determine the
		/// y-axis direction. y_dir does not need to be perpendicular to x_dir.
		/// </param>
		/// <since>5.0</since>
		// Token: 0x06007178 RID: 29048 RVA: 0x000C663D File Offset: 0x000C483D
		public Plane(Point3d origin, Vector3d xDirection, Vector3d yDirection)
		{
			this = default(Plane);
			UnsafeNativeMethods.ON_Plane_CreateFromFrame(ref this, origin, xDirection, yDirection);
		}

		/// <summary>
		/// Initializes a plane from three non-collinear points.
		/// </summary>
		/// <param name="origin">Origin point of the plane.</param>
		/// <param name="xPoint">
		/// Second point in the plane. The x-axis will be parallel to x_point-origin.
		/// </param>
		/// <param name="yPoint">
		/// Third point on the plane that is not collinear with the first two points.
		/// taxis*(y_point-origin) will be &gt; 0.
		/// </param>
		/// <example>
		/// <code source="examples\vbnet\ex_addclippingplane.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_addclippingplane.cs" lang="cs" />
		/// <code source="examples\py\ex_addclippingplane.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x06007179 RID: 29049 RVA: 0x000C6650 File Offset: 0x000C4850
		public Plane(Point3d origin, Point3d xPoint, Point3d yPoint)
		{
			this = default(Plane);
			UnsafeNativeMethods.ON_Plane_CreateFromPoints(ref this, origin, xPoint, yPoint);
		}

		/// <summary>
		/// Constructs a plane from an equation
		/// Ax+By+Cz+D=0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x0600717A RID: 29050 RVA: 0x000C6663 File Offset: 0x000C4863
		public Plane(double a, double b, double c, double d)
		{
			this = default(Plane);
			UnsafeNativeMethods.ON_Plane_CreateFromEquation(ref this, a, b, c, d);
			this.m_zaxis.Unitize();
		}

		/// <summary>
		/// Constructs a plane from a point, and two vectors in the plane.
		/// </summary>
		/// <param name="origin">Point on the plane.</param>
		/// <param name="xDirection">Non-zero vector in the plane that determines the XAxis direction.</param>
		/// <param name="yDirection">
		/// Non-zero vector not parallel to xDirection that is used to determine the YAxis direction. 
		/// Note, yDirection does not have to be perpendicular to xDirection.
		/// </param>
		/// <returns>A valid plane if successful, or Plane.Unset on failure.</returns>
		/// <since>8.0</since>
		// Token: 0x0600717B RID: 29051 RVA: 0x000C6684 File Offset: 0x000C4884
		public static Plane CreateFromFrame(Point3d origin, Vector3d xDirection, Vector3d yDirection)
		{
			Plane result = default(Plane);
			if (!UnsafeNativeMethods.ON_Plane_CreateFromFrame(ref result, origin, xDirection, yDirection))
			{
				return Plane.Unset;
			}
			return result;
		}

		/// <summary>
		/// Constructs a plane from a point and normal vector.
		/// </summary>
		/// <param name="origin">Point on the plane.</param>
		/// <param name="normal">Non-zero normal to the plane.</param>
		/// <returns>A valid plane if successful, or Plane.Unset on failure.</returns>
		/// <since>8.0</since>
		// Token: 0x0600717C RID: 29052 RVA: 0x000C66AC File Offset: 0x000C48AC
		public static Plane CreateFromNormal(Point3d origin, Vector3d normal)
		{
			Plane result = default(Plane);
			if (!UnsafeNativeMethods.ON_Plane_CreateFromNormal(ref result, origin, normal))
			{
				return Plane.Unset;
			}
			return result;
		}

		/// <summary>
		/// Construct a plane from a point, a normal vector, and a vector that projects to the positive YAxis.
		/// </summary>
		/// <param name="origin">Point on the plane.</param>
		/// <param name="normal">Non-zero normal to the plane.</param>
		/// <param name="yDirection">Non-zero vector, linearly independent from normal, that projects to the positive YAxis of the plane.</param>
		/// <returns>A valid plane if successful, or Plane.Unset on failure.</returns>
		/// <since>8.0</since>
		// Token: 0x0600717D RID: 29053 RVA: 0x000C66D4 File Offset: 0x000C48D4
		public static Plane CreateFromNormalYup(Point3d origin, Vector3d normal, Vector3d yDirection)
		{
			Plane result = default(Plane);
			if (!UnsafeNativeMethods.ON_Plane_CreateFromNormalYup(ref result, origin, normal, yDirection))
			{
				return Plane.Unset;
			}
			return result;
		}

		/// <summary>
		/// Construct a plane from three non-collinear points.
		/// </summary>
		/// <param name="origin">Point on the plane.</param>
		/// <param name="xPoint">Second point in the plane. XAxis will be parallel to xPoint-origin.</param>
		/// <param name="yPoint">
		/// Third point on the plane that is not collinear with the first two points, where
		/// YAxis*(yPoint-origin) will be &gt; 0.</param>
		/// <returns>A valid plane if successful, or Plane.Unset on failure.</returns>
		/// <since>8.0</since>
		// Token: 0x0600717E RID: 29054 RVA: 0x000C66FC File Offset: 0x000C48FC
		public static Plane CreateFromPoints(Point3d origin, Point3d xPoint, Point3d yPoint)
		{
			Plane result = default(Plane);
			if (!UnsafeNativeMethods.ON_Plane_CreateFromPoints(ref result, origin, xPoint, yPoint))
			{
				return Plane.Unset;
			}
			return result;
		}

		/// <summary>Fit a plane through a collection of points.</summary>
		/// <param name="points">Points to fit to.</param>
		/// <param name="plane">Resulting plane.</param>
		/// <returns>A value indicating the result of the operation.</returns>
		/// <since>5.0</since>
		// Token: 0x0600717F RID: 29055 RVA: 0x000C6724 File Offset: 0x000C4924
		public static PlaneFitResult FitPlaneToPoints(IEnumerable<Point3d> points, out Plane plane)
		{
			double num;
			return Plane.FitPlaneToPoints(points, out plane, out num);
		}

		/// <summary>Fit a plane through a collection of points.</summary>
		/// <param name="points">Points to fit to.</param>
		/// <param name="plane">Resulting plane.</param>
		/// <param name="maximumDeviation">The distance from the furthest point to the plane.</param>
		/// <returns>A value indicating the result of the operation.</returns>
		/// <since>5.0</since>
		// Token: 0x06007180 RID: 29056 RVA: 0x000C673C File Offset: 0x000C493C
		public static PlaneFitResult FitPlaneToPoints(IEnumerable<Point3d> points, out Plane plane, out double maximumDeviation)
		{
			plane = default(Plane);
			maximumDeviation = 0.0;
			int num;
			Point3d[] constArray = RhinoListHelpers.GetConstArray<Point3d>(points, out num);
			if (constArray == null || num < 1)
			{
				return PlaneFitResult.Failure;
			}
			int num2 = UnsafeNativeMethods.RHC_FitPlaneToPoints(num, constArray, ref plane, ref maximumDeviation);
			if (num2 == -1)
			{
				return PlaneFitResult.Failure;
			}
			if (num2 == 0)
			{
				return PlaneFitResult.Success;
			}
			return PlaneFitResult.Inconclusive;
		}

		/// <summary>
		/// Determines if two planes are equal.
		/// </summary>
		/// <param name="a">A first plane.</param>
		/// <param name="b">A second plane.</param>
		/// <returns>true if the two planes have all equal components; false otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x06007181 RID: 29057 RVA: 0x000C6783 File Offset: 0x000C4983
		public static bool operator ==(Plane a, Plane b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Determines if two planes are different.
		/// </summary>
		/// <param name="a">A first plane.</param>
		/// <param name="b">A second plane.</param>
		/// <returns>true if the two planes have any different component components; false otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x06007182 RID: 29058 RVA: 0x000C6790 File Offset: 0x000C4990
		public static bool operator !=(Plane a, Plane b)
		{
			return a.m_origin != b.m_origin || a.m_xaxis != b.m_xaxis || a.m_yaxis != b.m_yaxis || a.m_zaxis != b.m_zaxis;
		}

		/// <summary>
		/// Determines if an object is a plane and has the same components as this plane.
		/// </summary>
		/// <param name="obj">An object.</param>
		/// <returns>true if obj is a plane and has the same components as this plane; false otherwise.</returns>
		// Token: 0x06007183 RID: 29059 RVA: 0x000C67E9 File Offset: 0x000C49E9
		public override bool Equals(object obj)
		{
			return obj is Plane && this == (Plane)obj;
		}

		/// <summary>
		/// Determines if another plane has the same components as this plane.
		/// </summary>
		/// <param name="plane">A plane.</param>
		/// <returns>true if plane has the same components as this plane; false otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x06007184 RID: 29060 RVA: 0x000C6808 File Offset: 0x000C4A08
		public bool Equals(Plane plane)
		{
			return this.m_origin == plane.m_origin && this.m_xaxis == plane.m_xaxis && this.m_yaxis == plane.m_yaxis && this.m_zaxis == plane.m_zaxis;
		}

		/// <summary>
		/// Gets a non-unique hashing code for this entity.
		/// </summary>
		/// <returns>A particular number for a specific instance of plane.</returns>
		// Token: 0x06007185 RID: 29061 RVA: 0x000C6864 File Offset: 0x000C4A64
		public override int GetHashCode()
		{
			return this.m_origin.GetHashCode() ^ this.m_xaxis.GetHashCode() ^ this.m_yaxis.GetHashCode() ^ this.m_zaxis.GetHashCode();
		}

		/// <summary>
		/// Constructs the string representation of this plane.
		/// </summary>
		/// <returns>Text.</returns>
		// Token: 0x06007186 RID: 29062 RVA: 0x000C68B8 File Offset: 0x000C4AB8
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Origin={0} XAxis={1}, YAxis={2}, ZAxis={3}", new object[]
			{
				this.Origin,
				this.XAxis,
				this.YAxis,
				this.ZAxis.ToString()
			});
		}

		/// <inheritdoc />
		/// <since>7.0</since>
		// Token: 0x06007187 RID: 29063 RVA: 0x000C691C File Offset: 0x000C4B1C
		public string ToString(string format, IFormatProvider formatProvider)
		{
			string text = this.Origin.ToString(format, formatProvider);
			string text2 = this.XAxis.ToString(format, formatProvider);
			string text3 = this.YAxis.ToString(format, formatProvider);
			string text4 = this.ZAxis.ToString(format, formatProvider);
			return string.Concat(new string[]
			{
				"Origin=",
				text,
				" XAxis=",
				text2,
				", YAxis=",
				text3,
				", ZAxis=",
				text4
			});
		}

		/// <summary>
		/// Gets the normal of this plane. This is essentially the ZAxis of the plane.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145E RID: 5214
		// (get) Token: 0x06007188 RID: 29064 RVA: 0x000C69AC File Offset: 0x000C4BAC
		public Vector3d Normal
		{
			get
			{
				return this.ZAxis;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not this is a valid plane. 
		/// A plane is considered to be valid when all fields contain reasonable 
		/// information and the equation jibes with point and z-axis.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x1700145F RID: 5215
		// (get) Token: 0x06007189 RID: 29065 RVA: 0x000C69B4 File Offset: 0x000C4BB4
		public bool IsValid
		{
			get
			{
				return UnsafeNativeMethods.ON_Plane_IsValid(ref this);
			}
		}

		/// <summary>
		/// Gets the plane equation for this plane in the format of Ax+By+Cz+D=0.
		/// </summary>
		/// <returns>
		/// Array of four values.
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x0600718A RID: 29066 RVA: 0x000C69BC File Offset: 0x000C4BBC
		[ConstOperation]
		public double[] GetPlaneEquation()
		{
			double[] array = new double[4];
			UnsafeNativeMethods.ON_Plane_GetEquation(ref this, array);
			return array;
		}

		/// <summary>
		/// Update Equations
		/// </summary>
		/// <returns>
		/// bool
		/// </returns>
		/// <since>6.0</since>
		// Token: 0x0600718B RID: 29067 RVA: 0x000C69D8 File Offset: 0x000C4BD8
		public bool UpdateEquation()
		{
			return UnsafeNativeMethods.ON_Plane_UpdateEquation(ref this);
		}

		/// <summary>
		/// Get the value of the plane equation at the point.
		/// </summary>
		/// <param name="p">evaluation point.</param>
		/// <returns>returns pe[0]*p.X + pe[1]*p.Y + pe[2]*p.Z + pe[3] where
		/// pe[0], pe[1], pe[2] and pe[3] are the coefficients of the plane equation.
		///
		/// </returns>
		/// <since>5.7</since>
		// Token: 0x0600718C RID: 29068 RVA: 0x000C69E0 File Offset: 0x000C4BE0
		[ConstOperation]
		public double ValueAt(Point3d p)
		{
			double[] planeEquation = this.GetPlaneEquation();
			return planeEquation[0] * p.X + planeEquation[1] * p.Y + planeEquation[2] * p.Z + planeEquation[3];
		}

		/// <summary>
		/// Evaluate a point on the plane.
		/// </summary>
		/// <param name="u">evaluation parameter.</param>
		/// <param name="v">evaluation parameter.</param>
		/// <returns>plane.origin + u*plane.xaxis + v*plane.yaxis.</returns>
		/// <since>5.0</since>
		// Token: 0x0600718D RID: 29069 RVA: 0x000C6A1B File Offset: 0x000C4C1B
		[ConstOperation]
		public Point3d PointAt(double u, double v)
		{
			return this.Origin + u * this.XAxis + v * this.YAxis;
		}

		/// <summary>
		/// Test if this plane is co-planar with a another plane.
		/// </summary>
		/// <param name="plane">The plane to test.</param>
		/// <returns>True if this plane is co-planar with the test plane, false otherwise.</returns>
		/// <since>8.0</since>
		// Token: 0x0600718E RID: 29070 RVA: 0x000C6A45 File Offset: 0x000C4C45
		public bool IsCoplanar(Plane plane)
		{
			return this.IsCoplanar(plane, 2.3283064365386963E-10);
		}

		/// <summary>
		/// Test if this plane is co-planar with a another plane.
		/// </summary>
		/// <param name="plane">The plane to test.</param>
		/// <param name="tolerance">Testing tolerance.</param>
		/// <returns>True if this plane is co-planar with the test plane, false otherwise.</returns>
		/// <since>8.0</since>
		// Token: 0x0600718F RID: 29071 RVA: 0x000C6A58 File Offset: 0x000C4C58
		public bool IsCoplanar(Plane plane, double tolerance)
		{
			if (!this.IsValid || !plane.IsValid)
			{
				return false;
			}
			if (tolerance < 2.3283064365386963E-10)
			{
				tolerance = 2.3283064365386963E-10;
			}
			double[] planeEquation = this.GetPlaneEquation();
			double[] planeEquation2 = plane.GetPlaneEquation();
			return Math.Abs(planeEquation[0] - planeEquation2[0]) < tolerance && Math.Abs(planeEquation[1] - planeEquation2[1]) < tolerance && Math.Abs(planeEquation[2] - planeEquation2[2]) < tolerance && Math.Abs(planeEquation[3] - planeEquation2[3]) < tolerance;
		}

		/// <summary>
		/// Evaluate a point on the plane.
		/// </summary>
		/// <param name="u">evaluation parameter.</param>
		/// <param name="v">evaluation parameter.</param>
		/// <param name="w">evaluation parameter.</param>
		/// <returns>plane.origin + u*plane.xaxis + v*plane.yaxis + z*plane.zaxis.</returns>
		/// <since>5.0</since>
		// Token: 0x06007190 RID: 29072 RVA: 0x000C6ADC File Offset: 0x000C4CDC
		[ConstOperation]
		public Point3d PointAt(double u, double v, double w)
		{
			return this.Origin + u * this.XAxis + v * this.YAxis + w * this.ZAxis;
		}

		/// <summary>
		/// Extends this plane through a bounding box. 
		/// </summary>
		/// <param name="box">A box to use as minimal extension boundary.</param>
		/// <param name="s">
		/// If this function returns true, 
		/// the s parameter returns the Interval on the plane along the X direction that will 
		/// encompass the Box.
		/// </param>
		/// <param name="t">
		/// If this function returns true, 
		/// the t parameter returns the Interval on the plane along the Y direction that will 
		/// encompass the Box.
		/// </param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x06007191 RID: 29073 RVA: 0x000C6B17 File Offset: 0x000C4D17
		public bool ExtendThroughBox(BoundingBox box, out Interval s, out Interval t)
		{
			s = Interval.Unset;
			t = Interval.Unset;
			return this.IsValid && box.IsValid && this.ExtendThroughPoints(box.GetCorners(), ref s, ref t);
		}

		/// <summary>
		/// Extend this plane through a Box. 
		/// </summary>
		/// <param name="box">A box to use for extension.</param>
		/// <param name="s">
		/// If this function returns true, 
		/// the s parameter returns the Interval on the plane along the X direction that will 
		/// encompass the Box.
		/// </param>
		/// <param name="t">
		/// If this function returns true, 
		/// the t parameter returns the Interval on the plane along the Y direction that will 
		/// encompass the Box.
		/// </param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x06007192 RID: 29074 RVA: 0x000C6B53 File Offset: 0x000C4D53
		public bool ExtendThroughBox(Box box, out Interval s, out Interval t)
		{
			s = Interval.Unset;
			t = Interval.Unset;
			return this.IsValid && box.IsValid && this.ExtendThroughPoints(box.GetCorners(), ref s, ref t);
		}

		// Token: 0x06007193 RID: 29075 RVA: 0x000C6B90 File Offset: 0x000C4D90
		internal bool ExtendThroughPoints(IEnumerable<Point3d> pts, ref Interval s, ref Interval t)
		{
			double num = double.MaxValue;
			double num2 = double.MinValue;
			double num3 = double.MaxValue;
			double num4 = double.MinValue;
			bool flag = false;
			foreach (Point3d testPoint in pts)
			{
				double val;
				double val2;
				if (this.ClosestParameter(testPoint, out val, out val2))
				{
					flag = true;
					num = Math.Min(num, val);
					num2 = Math.Max(num2, val);
					num3 = Math.Min(num3, val2);
					num4 = Math.Max(num4, val2);
				}
			}
			if (flag)
			{
				s = new Interval(num, num2);
				t = new Interval(num3, num4);
			}
			return flag;
		}

		/// <summary>
		/// Gets the parameters of the point on the plane closest to a test point.
		/// </summary>
		/// <param name="testPoint">Point to get close to.</param>
		/// <param name="s">Parameter along plane X-direction.</param>
		/// <param name="t">Parameter along plane Y-direction.</param>
		/// <returns>
		/// true if a parameter could be found, 
		/// false if the point could not be projected successfully.
		/// </returns>
		/// <example>
		/// <code source="examples\vbnet\ex_addlineardimension2.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_addlineardimension2.cs" lang="cs" />
		/// <code source="examples\py\ex_addlineardimension2.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x06007194 RID: 29076 RVA: 0x000C6C58 File Offset: 0x000C4E58
		[ConstOperation]
		public bool ClosestParameter(Point3d testPoint, out double s, out double t)
		{
			Vector3d vector = testPoint - this.Origin;
			s = vector * this.XAxis;
			t = vector * this.YAxis;
			return true;
		}

		/// <summary>
		/// Gets the point on the plane closest to a test point.
		/// </summary>
		/// <param name="testPoint">Point to get close to.</param>
		/// <returns>
		/// The point on the plane that is closest to testPoint, 
		/// or Point3d.Unset on failure.
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x06007195 RID: 29077 RVA: 0x000C6C90 File Offset: 0x000C4E90
		[ConstOperation]
		public Point3d ClosestPoint(Point3d testPoint)
		{
			double u;
			double v;
			if (this.ClosestParameter(testPoint, out u, out v))
			{
				return this.PointAt(u, v);
			}
			return Point3d.Unset;
		}

		/// <summary>
		/// Returns the signed distance from testPoint to its projection onto this plane. 
		/// If the point is below the plane, a negative distance is returned.
		/// </summary>
		/// <param name="testPoint">Point to test.</param>
		/// <returns>Signed distance from this plane to testPoint.</returns>
		/// <example>
		/// <code source="examples\vbnet\ex_issurfaceinplane.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_issurfaceinplane.cs" lang="cs" />
		/// <code source="examples\py\ex_issurfaceinplane.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x06007196 RID: 29078 RVA: 0x000C6CB8 File Offset: 0x000C4EB8
		[ConstOperation]
		public double DistanceTo(Point3d testPoint)
		{
			return UnsafeNativeMethods.ON_Plane_DistanceTo(ref this, testPoint);
		}

		/// <summary>
		/// Returns the signed minimum and maximum distances from bounding box to this plane.
		/// </summary>
		/// <param name="bbox">bounding box to get distances from</param>
		/// <param name="min">minimum signed distance from plane to box</param>
		/// <param name="max">maximum signed distance from plane to box</param>
		/// <returns>false if plane has zero length normal</returns>
		/// <since>6.0</since>
		// Token: 0x06007197 RID: 29079 RVA: 0x000C6CC1 File Offset: 0x000C4EC1
		[ConstOperation]
		public bool DistanceTo(BoundingBox bbox, out double min, out double max)
		{
			min = 0.0;
			max = 0.0;
			return UnsafeNativeMethods.ON_Plane_GetDistanceToBoundingBox(ref this, bbox.Min, bbox.Max, ref min, ref max);
		}

		/// <summary>
		/// Convert a point from World space coordinates into Plane space coordinates.
		/// </summary>
		/// <param name="ptSample">World point to remap.</param>
		/// <param name="ptPlane">Point in plane (s,t,d) coordinates.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <remarks>D stands for distance, not disease.</remarks>
		/// <since>5.0</since>
		// Token: 0x06007198 RID: 29080 RVA: 0x000C6CF0 File Offset: 0x000C4EF0
		[ConstOperation]
		public bool RemapToPlaneSpace(Point3d ptSample, out Point3d ptPlane)
		{
			double x;
			double y;
			if (!this.ClosestParameter(ptSample, out x, out y))
			{
				ptPlane = Point3d.Unset;
				return false;
			}
			double z = this.DistanceTo(ptSample);
			ptPlane = new Point3d(x, y, z);
			return true;
		}

		/// <summary>
		/// Flip this plane by swapping out the X and Y axes and inverting the Z axis.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x06007199 RID: 29081 RVA: 0x000C6D30 File Offset: 0x000C4F30
		public void Flip()
		{
			Vector3d xaxis = this.m_xaxis;
			this.m_xaxis = this.m_yaxis;
			this.m_yaxis = xaxis;
			this.m_zaxis = -this.m_zaxis;
		}

		/// <summary>
		/// Transform the plane with a Transformation matrix.
		/// </summary>
		/// <param name="xform">Transformation to apply to plane.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719A RID: 29082 RVA: 0x000C6D68 File Offset: 0x000C4F68
		public bool Transform(Transform xform)
		{
			return UnsafeNativeMethods.ON_Plane_Transform(ref this, ref xform);
		}

		/// <summary>
		/// Translate (move) the plane along a vector.
		/// </summary>
		/// <param name="delta">Translation (motion) vector.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719B RID: 29083 RVA: 0x000C6D72 File Offset: 0x000C4F72
		public bool Translate(Vector3d delta)
		{
			if (!delta.IsValid)
			{
				return false;
			}
			this.Origin += delta;
			return true;
		}

		/// <summary>
		/// Rotate the plane about its origin point.
		/// </summary>
		/// <param name="sinAngle">Sin(angle).</param>
		/// <param name="cosAngle">Cos(angle).</param>
		/// <param name="axis">Axis of rotation.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719C RID: 29084 RVA: 0x000C6D94 File Offset: 0x000C4F94
		public bool Rotate(double sinAngle, double cosAngle, Vector3d axis)
		{
			bool result = true;
			if (axis == this.ZAxis)
			{
				Vector3d xaxis = cosAngle * this.XAxis + sinAngle * this.YAxis;
				Vector3d yaxis = cosAngle * this.YAxis - sinAngle * this.XAxis;
				this.XAxis = xaxis;
				this.YAxis = yaxis;
			}
			else
			{
				Point3d origin = this.Origin;
				result = this.Rotate(sinAngle, cosAngle, axis, this.Origin);
				this.Origin = origin;
			}
			return result;
		}

		/// <summary>
		/// Rotate the plane about its origin point.
		/// </summary>
		/// <param name="angle">Angle in radians.</param>
		/// <param name="axis">Axis of rotation.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719D RID: 29085 RVA: 0x000C6E1C File Offset: 0x000C501C
		public bool Rotate(double angle, Vector3d axis)
		{
			return this.Rotate(Math.Sin(angle), Math.Cos(angle), axis);
		}

		/// <summary>
		/// Rotate the plane about a custom anchor point.
		/// </summary>
		/// <param name="angle">Angle in radians.</param>
		/// <param name="axis">Axis of rotation.</param>
		/// <param name="centerOfRotation">Center of rotation.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719E RID: 29086 RVA: 0x000C6E31 File Offset: 0x000C5031
		public bool Rotate(double angle, Vector3d axis, Point3d centerOfRotation)
		{
			return this.Rotate(Math.Sin(angle), Math.Cos(angle), axis, centerOfRotation);
		}

		/// <summary>Rotate the plane about a custom anchor point.</summary>
		/// <param name="sinAngle">Sin(angle)</param>
		/// <param name="cosAngle">Cos(angle)</param>
		/// <param name="axis">Axis of rotation.</param>
		/// <param name="centerOfRotation">Center of rotation.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x0600719F RID: 29087 RVA: 0x000C6E48 File Offset: 0x000C5048
		public bool Rotate(double sinAngle, double cosAngle, Vector3d axis, Point3d centerOfRotation)
		{
			if (centerOfRotation == this.Origin)
			{
				Transform m = Rhino.Geometry.Transform.Rotation(sinAngle, cosAngle, axis, Point3d.Origin);
				this.XAxis = m * this.XAxis;
				this.YAxis = m * this.YAxis;
				this.ZAxis = m * this.ZAxis;
				return true;
			}
			Transform xform = Rhino.Geometry.Transform.Rotation(sinAngle, cosAngle, axis, centerOfRotation);
			return this.Transform(xform);
		}

		/// <summary>
		/// Check that all values in other are within epsilon of the values in this
		/// </summary>
		/// <param name="other"></param>
		/// <param name="epsilon"></param>
		/// <returns></returns>
		/// <since>5.4</since>
		// Token: 0x060071A0 RID: 29088 RVA: 0x000C6EBC File Offset: 0x000C50BC
		[ConstOperation]
		public bool EpsilonEquals(Plane other, double epsilon)
		{
			return this.m_origin.EpsilonEquals(other.m_origin, epsilon) && this.m_xaxis.EpsilonEquals(other.m_xaxis, epsilon) && this.m_yaxis.EpsilonEquals(other.m_yaxis, epsilon) && this.m_zaxis.EpsilonEquals(other.m_zaxis, epsilon);
		}

		/// <since>6.0</since>
		// Token: 0x060071A1 RID: 29089 RVA: 0x000C6F19 File Offset: 0x000C5119
		object ICloneable.Clone()
		{
			return this;
		}

		/// <summary>
		/// Returns a deep copy of this instance.
		/// </summary>
		/// <returns>A plane with the same values as this item.</returns>
		/// <since>6.0</since>
		// Token: 0x060071A2 RID: 29090 RVA: 0x000C6F26 File Offset: 0x000C5126
		public Plane Clone()
		{
			return this;
		}

		// Token: 0x04001929 RID: 6441
		internal Point3d m_origin;

		// Token: 0x0400192A RID: 6442
		internal Vector3d m_xaxis;

		// Token: 0x0400192B RID: 6443
		internal Vector3d m_yaxis;

		// Token: 0x0400192C RID: 6444
		internal Vector3d m_zaxis;
	}
}
