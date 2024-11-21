using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Rhino.Runtime;

namespace Rhino.Geometry
{
	/// <summary>
	/// Represents the three components of a vector in three-dimensional space,
	/// using <see cref="T:System.Double" />-precision floating point numbers.
	/// </summary>
	// Token: 0x0200052B RID: 1323
	[DebuggerDisplay("({m_x}, {m_y}, {m_z})")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
	public struct Vector3d : ISerializable, IEquatable<Vector3d>, IComparable<Vector3d>, IComparable, IEpsilonComparable<Vector3d>, ICloneable, IValidable, IFormattable
	{
		/// <summary>
		/// Initializes a new instance of a vector, using its three components.
		/// </summary>
		/// <param name="x">The X (first) component.</param>
		/// <param name="y">The Y (second) component.</param>
		/// <param name="z">The Z (third) component.</param>
		/// <since>5.0</since>
		// Token: 0x06007426 RID: 29734 RVA: 0x000CE585 File Offset: 0x000CC785
		public Vector3d(double x, double y, double z)
		{
			this.m_x = x;
			this.m_y = y;
			this.m_z = z;
		}

		/// <summary>
		/// Initializes a new instance of a vector, copying the three components from the three coordinates of a point.
		/// </summary>
		/// <param name="point">The point to copy from.</param>
		/// <since>5.0</since>
		// Token: 0x06007427 RID: 29735 RVA: 0x000CE59C File Offset: 0x000CC79C
		public Vector3d(Point3d point)
		{
			this.m_x = point.m_x;
			this.m_y = point.m_y;
			this.m_z = point.m_z;
		}

		/// <summary>
		/// Initializes a new instance of a vector, copying the three components from a single-precision vector.
		/// </summary>
		/// <param name="vector">A single-precision vector.</param>
		/// <since>5.0</since>
		// Token: 0x06007428 RID: 29736 RVA: 0x000CE5C2 File Offset: 0x000CC7C2
		public Vector3d(Vector3f vector)
		{
			this.m_x = (double)vector.m_x;
			this.m_y = (double)vector.m_y;
			this.m_z = (double)vector.m_z;
		}

		/// <summary>
		/// Initializes a new instance of a vector, copying the three components from a vector.
		/// </summary>
		/// <param name="vector">A double-precision vector.</param>
		/// <since>5.0</since>
		// Token: 0x06007429 RID: 29737 RVA: 0x000CE5EB File Offset: 0x000CC7EB
		public Vector3d(Vector3d vector)
		{
			this.m_x = vector.m_x;
			this.m_y = vector.m_y;
			this.m_z = vector.m_z;
		}

		/// <summary>
		/// Gets the value of the vector with components 0,0,0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F1 RID: 5361
		// (get) Token: 0x0600742A RID: 29738 RVA: 0x000CE614 File Offset: 0x000CC814
		public static Vector3d Zero
		{
			get
			{
				return default(Vector3d);
			}
		}

		/// <summary>
		/// Gets the value of the vector with components 1,0,0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F2 RID: 5362
		// (get) Token: 0x0600742B RID: 29739 RVA: 0x000CE62A File Offset: 0x000CC82A
		public static Vector3d XAxis
		{
			get
			{
				return new Vector3d(1.0, 0.0, 0.0);
			}
		}

		/// <summary>
		/// Gets the value of the vector with components 0,1,0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F3 RID: 5363
		// (get) Token: 0x0600742C RID: 29740 RVA: 0x000CE64C File Offset: 0x000CC84C
		public static Vector3d YAxis
		{
			get
			{
				return new Vector3d(0.0, 1.0, 0.0);
			}
		}

		/// <summary>
		/// Gets the value of the vector with components 0,0,1.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F4 RID: 5364
		// (get) Token: 0x0600742D RID: 29741 RVA: 0x000CE66E File Offset: 0x000CC86E
		public static Vector3d ZAxis
		{
			get
			{
				return new Vector3d(0.0, 0.0, 1.0);
			}
		}

		/// <summary>
		/// Gets the value of the vector with each component set to RhinoMath.UnsetValue.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F5 RID: 5365
		// (get) Token: 0x0600742E RID: 29742 RVA: 0x000CE690 File Offset: 0x000CC890
		public static Vector3d Unset
		{
			get
			{
				return new Vector3d(-1.23432101234321E+308, -1.23432101234321E+308, -1.23432101234321E+308);
			}
		}

		// Token: 0x0600742F RID: 29743 RVA: 0x000CE6B2 File Offset: 0x000CC8B2
		private Vector3d(SerializationInfo info, StreamingContext context)
		{
			this.m_x = info.GetDouble("X");
			this.m_y = info.GetDouble("Y");
			this.m_z = info.GetDouble("Z");
		}

		// Token: 0x06007430 RID: 29744 RVA: 0x000CE6E7 File Offset: 0x000CC8E7
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", this.m_x);
			info.AddValue("Y", this.m_y);
			info.AddValue("Z", this.m_z);
		}

		/// <summary>
		/// Multiplies a vector by a number, having the effect of scaling it.
		/// </summary>
		/// <param name="vector">A vector.</param>
		/// <param name="t">A number.</param>
		/// <returns>A new vector that is the original vector coordinate-wise multiplied by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007431 RID: 29745 RVA: 0x000CE71C File Offset: 0x000CC91C
		public static Vector3d operator *(Vector3d vector, double t)
		{
			return new Vector3d(vector.m_x * t, vector.m_y * t, vector.m_z * t);
		}

		/// <summary>
		/// Multiplies a vector by a number, having the effect of scaling it.
		/// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
		/// </summary>
		/// <param name="vector">A vector.</param>
		/// <param name="t">A number.</param>
		/// <returns>A new vector that is the original vector coordinate-wise multiplied by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007432 RID: 29746 RVA: 0x000CE73B File Offset: 0x000CC93B
		public static Vector3d Multiply(Vector3d vector, double t)
		{
			return new Vector3d(vector.m_x * t, vector.m_y * t, vector.m_z * t);
		}

		/// <summary>
		/// Multiplies a vector by a number, having the effect of scaling it.
		/// </summary>
		/// <param name="t">A number.</param>
		/// <param name="vector">A vector.</param>
		/// <returns>A new vector that is the original vector coordinate-wise multiplied by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007433 RID: 29747 RVA: 0x000CE75A File Offset: 0x000CC95A
		public static Vector3d operator *(double t, Vector3d vector)
		{
			return new Vector3d(vector.m_x * t, vector.m_y * t, vector.m_z * t);
		}

		/// <summary>
		/// Multiplies a vector by a number, having the effect of scaling it.
		/// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
		/// </summary>
		/// <param name="t">A number.</param>
		/// <param name="vector">A vector.</param>
		/// <returns>A new vector that is the original vector coordinate-wise multiplied by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007434 RID: 29748 RVA: 0x000CE779 File Offset: 0x000CC979
		public static Vector3d Multiply(double t, Vector3d vector)
		{
			return new Vector3d(vector.m_x * t, vector.m_y * t, vector.m_z * t);
		}

		/// <summary>
		/// Divides a <see cref="T:Rhino.Geometry.Vector3d" /> by a number, having the effect of shrinking it.
		/// </summary>
		/// <param name="vector">A vector.</param>
		/// <param name="t">A number.</param>
		/// <returns>A new vector that is component-wise divided by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007435 RID: 29749 RVA: 0x000CE798 File Offset: 0x000CC998
		public static Vector3d operator /(Vector3d vector, double t)
		{
			return new Vector3d(vector.m_x / t, vector.m_y / t, vector.m_z / t);
		}

		/// <summary>
		/// Divides a <see cref="T:Rhino.Geometry.Vector3d" /> by a number, having the effect of shrinking it.
		/// <para>(Provided for languages that do not support operator overloading. You can use the / operator otherwise)</para>
		/// </summary>
		/// <param name="vector">A vector.</param>
		/// <param name="t">A number.</param>
		/// <returns>A new vector that is component-wise divided by t.</returns>
		/// <since>5.0</since>
		// Token: 0x06007436 RID: 29750 RVA: 0x000CE7B7 File Offset: 0x000CC9B7
		public static Vector3d Divide(Vector3d vector, double t)
		{
			return new Vector3d(vector.m_x / t, vector.m_y / t, vector.m_z / t);
		}

		/// <summary>
		/// Sums up two vectors.
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>A new vector that results from the component-wise addition of the two vectors.</returns>
		// Token: 0x06007437 RID: 29751 RVA: 0x000CE7D6 File Offset: 0x000CC9D6
		public static Vector3d operator +(Vector3d vector1, Vector3d vector2)
		{
			return new Vector3d(vector1.m_x + vector2.m_x, vector1.m_y + vector2.m_y, vector1.m_z + vector2.m_z);
		}

		/// <summary>
		/// Sums up two vectors.
		/// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>A new vector that results from the component-wise addition of the two vectors.</returns>
		/// <since>5.0</since>
		// Token: 0x06007438 RID: 29752 RVA: 0x000CE804 File Offset: 0x000CCA04
		public static Vector3d Add(Vector3d vector1, Vector3d vector2)
		{
			return new Vector3d(vector1.m_x + vector2.m_x, vector1.m_y + vector2.m_y, vector1.m_z + vector2.m_z);
		}

		/// <summary>
		/// Subtracts the second vector from the first one.
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>A new vector that results from the component-wise difference of vector1 - vector2.</returns>
		/// <since>5.0</since>
		// Token: 0x06007439 RID: 29753 RVA: 0x000CE832 File Offset: 0x000CCA32
		public static Vector3d operator -(Vector3d vector1, Vector3d vector2)
		{
			return new Vector3d(vector1.m_x - vector2.m_x, vector1.m_y - vector2.m_y, vector1.m_z - vector2.m_z);
		}

		/// <summary>
		/// Subtracts the second vector from the first one.
		/// <para>(Provided for languages that do not support operator overloading. You can use the - operator otherwise)</para>
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>A new vector that results from the component-wise difference of vector1 - vector2.</returns>
		/// <since>5.0</since>
		// Token: 0x0600743A RID: 29754 RVA: 0x000CE860 File Offset: 0x000CCA60
		public static Vector3d Subtract(Vector3d vector1, Vector3d vector2)
		{
			return new Vector3d(vector1.m_x - vector2.m_x, vector1.m_y - vector2.m_y, vector1.m_z - vector2.m_z);
		}

		/// <summary>
		/// Multiplies two vectors together, returning the dot product (or inner product).
		/// This differs from the cross product.
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>
		/// A value that results from the evaluation of v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z.
		/// <para>This value equals v1.Length * v2.Length * cos(alpha), where alpha is the angle between vectors.</para>
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x0600743B RID: 29755 RVA: 0x000CE88E File Offset: 0x000CCA8E
		public static double operator *(Vector3d vector1, Vector3d vector2)
		{
			return vector1.m_x * vector2.m_x + vector1.m_y * vector2.m_y + vector1.m_z * vector2.m_z;
		}

		/// <summary>
		/// Multiplies two vectors together, returning the dot product (or inner product).
		/// This differs from the cross product.
		/// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
		/// </summary>
		/// <param name="vector1">A vector.</param>
		/// <param name="vector2">A second vector.</param>
		/// <returns>
		/// A value that results from the evaluation of v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z.
		/// <para>This value equals v1.Length * v2.Length * cos(alpha), where alpha is the angle between vectors.</para>
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x0600743C RID: 29756 RVA: 0x000CE8B9 File Offset: 0x000CCAB9
		public static double Multiply(Vector3d vector1, Vector3d vector2)
		{
			return vector1.m_x * vector2.m_x + vector1.m_y * vector2.m_y + vector1.m_z * vector2.m_z;
		}

		/// <summary>
		/// Computes the opposite vector.
		/// </summary>
		/// <param name="vector">A vector to negate.</param>
		/// <returns>A new vector where all components were multiplied by -1.</returns>
		/// <since>5.0</since>
		// Token: 0x0600743D RID: 29757 RVA: 0x000CE8E4 File Offset: 0x000CCAE4
		public static Vector3d operator -(Vector3d vector)
		{
			return new Vector3d(-vector.m_x, -vector.m_y, -vector.m_z);
		}

		/// <summary>
		/// Computes the reversed vector.
		/// <para>(Provided for languages that do not support operator overloading. You can use the - unary operator otherwise)</para>
		/// </summary>
		/// <remarks>Similar to <see cref="M:Rhino.Geometry.Vector3d.Reverse">Reverse()</see>, but static for CLR compliance.</remarks>
		/// <param name="vector">A vector to negate.</param>
		/// <returns>A new vector where all components were multiplied by -1.</returns>
		/// <since>5.0</since>
		// Token: 0x0600743E RID: 29758 RVA: 0x000CE900 File Offset: 0x000CCB00
		public static Vector3d Negate(Vector3d vector)
		{
			return new Vector3d(-vector.m_x, -vector.m_y, -vector.m_z);
		}

		/// <summary>
		/// Determines whether two vectors have the same value.
		/// </summary>
		/// <param name="a">A vector.</param>
		/// <param name="b">Another vector.</param>
		/// <returns>true if all coordinates are pairwise equal; false otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x0600743F RID: 29759 RVA: 0x000CE91C File Offset: 0x000CCB1C
		public static bool operator ==(Vector3d a, Vector3d b)
		{
			return a.m_x == b.m_x && a.m_y == b.m_y && a.m_z == b.m_z;
		}

		/// <summary>
		/// Determines whether two vectors have different values.
		/// </summary>
		/// <param name="a">A vector.</param>
		/// <param name="b">Another vector.</param>
		/// <returns>true if any coordinate pair is different; false otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x06007440 RID: 29760 RVA: 0x000CE94A File Offset: 0x000CCB4A
		public static bool operator !=(Vector3d a, Vector3d b)
		{
			return a.m_x != b.m_x || a.m_y != b.m_y || a.m_z != b.m_z;
		}

		/// <summary>
		/// Computes the cross product (or vector product, or exterior product) of two vectors.
		/// <para>This operation is not commutative.</para>
		/// </summary>
		/// <param name="a">First vector.</param>
		/// <param name="b">Second vector.</param>
		/// <returns>A new vector that is perpendicular to both a and b,
		/// <para>has Length == a.Length * b.Length * sin(theta) where theta is the angle between a and b.</para>
		/// <para>The resulting vector is oriented according to the right hand rule.</para>
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x06007441 RID: 29761 RVA: 0x000CE97C File Offset: 0x000CCB7C
		public static Vector3d CrossProduct(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.m_y * b.m_z - b.m_y * a.m_z, a.m_z * b.m_x - b.m_z * a.m_x, a.m_x * b.m_y - b.m_x * a.m_y);
		}

		/// <summary>
		/// Compute the angle between two vectors.
		/// <para>This operation is commutative.</para>
		/// </summary>
		/// <param name="a">First vector for angle.</param>
		/// <param name="b">Second vector for angle.</param>
		/// <returns>If the input is valid, the angle (in radians) between a and b; RhinoMath.UnsetValue otherwise.</returns>
		/// <since>5.0</since>
		// Token: 0x06007442 RID: 29762 RVA: 0x000CE9E0 File Offset: 0x000CCBE0
		public static double VectorAngle(Vector3d a, Vector3d b)
		{
			if (!a.Unitize() || !b.Unitize())
			{
				return -1.23432101234321E+308;
			}
			double num = a.m_x * b.m_x + a.m_y * b.m_y + a.m_z * b.m_z;
			if (num > 1.0)
			{
				num = 1.0;
			}
			if (num < -1.0)
			{
				num = -1.0;
			}
			return Math.Acos(num);
		}

		/// <summary>
		/// Computes the angle on a plane between two vectors.
		/// </summary>
		/// <param name="a">First vector.</param>
		/// <param name="b">Second vector.</param>
		/// <param name="plane">Two-dimensional plane on which to perform the angle measurement.</param>
		/// <returns>On success, the angle (in radians) between a and b as projected onto the plane; RhinoMath.UnsetValue on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x06007443 RID: 29763 RVA: 0x000CEA68 File Offset: 0x000CCC68
		public static double VectorAngle(Vector3d a, Vector3d b, Plane plane)
		{
			Point3d point3d = plane.Origin + a;
			Point3d point3d2 = plane.Origin + b;
			point3d = plane.ClosestPoint(point3d);
			point3d2 = plane.ClosestPoint(point3d2);
			a = point3d - plane.Origin;
			b = point3d2 - plane.Origin;
			if (!a.Unitize())
			{
				return -1.23432101234321E+308;
			}
			if (!b.Unitize())
			{
				return -1.23432101234321E+308;
			}
			double num = a * b;
			if (num >= 1.0)
			{
				num = 1.0;
			}
			else if (num < -1.0)
			{
				num = -1.0;
			}
			double num2 = Math.Acos(num);
			if (Math.Abs(num2) < 1E-64)
			{
				return 0.0;
			}
			if (Math.Abs(num2 - 3.141592653589793) < 1E-64)
			{
				return 3.141592653589793;
			}
			Vector3d other = Vector3d.CrossProduct(a, b);
			if (plane.ZAxis.IsParallelTo(other) == 1)
			{
				return num2;
			}
			return 6.283185307179586 - num2;
		}

		/// <summary>
		/// Computes the angle of v1, v2 with a normal vector.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <param name="vNormal">Normal vector.</param>
		/// <returns>On success, the angle (in radians) between a and b with respect of normal vector; RhinoMath.UnsetValue on failure.</returns>
		/// <since>6.0</since>
		// Token: 0x06007444 RID: 29764 RVA: 0x000CEB94 File Offset: 0x000CCD94
		public static double VectorAngle(Vector3d v1, Vector3d v2, Vector3d vNormal)
		{
			if (Math.Abs(v1.X - v2.X) < 1E-64 && Math.Abs(v1.Y - v2.Y) < 1E-64 && Math.Abs(v1.Z - v2.Z) < 1E-64)
			{
				return 0.0;
			}
			double num = v1 * v2;
			double num2 = v1.Length * v2.Length;
			Vector3d vector = Vector3d.CrossProduct(v1, v2);
			vector.Unitize();
			if (Math.Abs(vector.X - 0.0) < 1E-64 && Math.Abs(vector.Y - 0.0) < 1E-64 && Math.Abs(vector.Z - 0.0) < 1E-64)
			{
				if (Math.Abs(num - 1.0) < 1E-64)
				{
					return 0.0;
				}
				if (Math.Abs(num + 1.0) < 1E-64)
				{
					return 3.141592653589793;
				}
			}
			double num3 = num / num2;
			if (num3 > 1.0)
			{
				num3 = 1.0;
			}
			else if (num3 < -1.0)
			{
				num3 = -1.0;
			}
			if (Math.Abs(num3 + 1.0) < 1E-64)
			{
				return 3.141592653589793;
			}
			double num4 = Math.Acos(num3);
			vNormal.Unitize();
			if (Math.Abs(vector * vNormal + 1.0) < 1E-64)
			{
				num4 = 6.283185307179586 - num4;
			}
			return num4;
		}

		/// <summary>
		/// Converts a single-precision (float) vector in a double-precision vector, without needing casting.
		/// </summary>
		/// <param name="vector">A single-precision vector.</param>
		/// <returns>The same vector, expressed using double-precision values.</returns>
		// Token: 0x06007445 RID: 29765 RVA: 0x000CED78 File Offset: 0x000CCF78
		public static implicit operator Vector3d(Vector3f vector)
		{
			return new Vector3d(vector);
		}

		/// <summary>
		/// Determines whether the first specified vector comes before (has inferior sorting value than) the second vector.
		/// <para>Components evaluation priority is first X, then Y, then Z.</para>
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>true if a.X is smaller than b.X,
		/// or a.X == b.X and a.Y is smaller than b.Y,
		/// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
		/// otherwise, false.</returns>
		/// <since>5.0</since>
		// Token: 0x06007446 RID: 29766 RVA: 0x000CED80 File Offset: 0x000CCF80
		public static bool operator <(Vector3d a, Vector3d b)
		{
			if (a.X < b.X)
			{
				return true;
			}
			if (a.X == b.X)
			{
				if (a.Y < b.Y)
				{
					return true;
				}
				if (a.Y == b.Y && a.Z < b.Z)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether the first specified vector comes before
		/// (has inferior sorting value than) the second vector, or it is equal to it.
		/// <para>Components evaluation priority is first X, then Y, then Z.</para>
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>true if a.X is smaller than b.X,
		/// or a.X == b.X and a.Y is smaller than b.Y,
		/// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
		/// otherwise, false.</returns>
		/// <since>5.0</since>
		// Token: 0x06007447 RID: 29767 RVA: 0x000CEDE4 File Offset: 0x000CCFE4
		public static bool operator <=(Vector3d a, Vector3d b)
		{
			return a.CompareTo(b) <= 0;
		}

		/// <summary>
		/// Determines whether the first specified vector comes after (has superior sorting value than)
		/// the second vector.
		/// <para>Components evaluation priority is first X, then Y, then Z.</para>
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>true if a.X is larger than b.X,
		/// or a.X == b.X and a.Y is larger than b.Y,
		/// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
		/// otherwise, false.</returns>
		/// <since>5.0</since>
		// Token: 0x06007448 RID: 29768 RVA: 0x000CEDF4 File Offset: 0x000CCFF4
		public static bool operator >(Vector3d a, Vector3d b)
		{
			if (a.X > b.X)
			{
				return true;
			}
			if (a.X == b.X)
			{
				if (a.Y > b.Y)
				{
					return true;
				}
				if (a.Y == b.Y && a.Z > b.Z)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether the first specified vector comes after (has superior sorting value than)
		/// the second vector, or it is equal to it.
		/// <para>Components evaluation priority is first X, then Y, then Z.</para>
		/// </summary>
		/// <param name="a">The first vector.</param>
		/// <param name="b">The second vector.</param>
		/// <returns>true if a.X is larger than b.X,
		/// or a.X == b.X and a.Y is larger than b.Y,
		/// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
		/// otherwise, false.</returns>
		/// <since>5.0</since>
		// Token: 0x06007449 RID: 29769 RVA: 0x000CEE58 File Offset: 0x000CD058
		public static bool operator >=(Vector3d a, Vector3d b)
		{
			return a.CompareTo(b) >= 0;
		}

		/// <summary>
		/// Test whether three vectors describe an orthogonal axis system.
		/// All vectors must be mutually perpendicular this to be the case.
		/// </summary>
		/// <param name="x">X axis vector.</param>
		/// <param name="y">Y axis vector.</param>
		/// <param name="z">Z axis vector.</param>
		/// <returns>True if all vectors are non-zero and mutually perpendicular.</returns>
		/// <since>6.7</since>
		// Token: 0x0600744A RID: 29770 RVA: 0x000CEE68 File Offset: 0x000CD068
		public static bool AreOrthogonal(Vector3d x, Vector3d y, Vector3d z)
		{
			return UnsafeNativeMethods.ON_Plane_IsOrthogonalFrame(x, y, z);
		}

		/// <summary>
		/// Test whether three vectors describe an orthogonal, unit axis system.
		/// All vectors must be mutually perpendicular and have unit length for this to be the case.
		/// </summary>
		/// <param name="x">X axis vector.</param>
		/// <param name="y">Y axis vector.</param>
		/// <param name="z">Z axis vector.</param>
		/// <returns>True if all vectors are non-zero and mutually perpendicular.</returns>
		/// <since>6.7</since>
		// Token: 0x0600744B RID: 29771 RVA: 0x000CEE72 File Offset: 0x000CD072
		public static bool AreOrthonormal(Vector3d x, Vector3d y, Vector3d z)
		{
			return UnsafeNativeMethods.ON_Plane_IsOrthonormalFrame(x, y, z);
		}

		/// <summary>
		/// Test whether three vectors describe a right-handed, orthogonal, unit axis system.
		/// The vectors must be orthonormal and follow the right-hand ordering; index-finger=x,
		/// middle-finger=y, thumb=z.
		/// </summary>
		/// <param name="x">X axis vector.</param>
		/// <param name="y">Y axis vector.</param>
		/// <param name="z">Z axis vector.</param>
		/// <returns>True if all vectors are non-zero and mutually perpendicular.</returns>
		/// <since>6.7</since>
		// Token: 0x0600744C RID: 29772 RVA: 0x000CEE7C File Offset: 0x000CD07C
		public static bool AreRighthanded(Vector3d x, Vector3d y, Vector3d z)
		{
			return UnsafeNativeMethods.ON_Plane_IsRightHandFrame(x, y, z);
		}

		/// <summary>
		/// Gets or sets the X (first) component of the vector.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F6 RID: 5366
		// (get) Token: 0x0600744D RID: 29773 RVA: 0x000CEE86 File Offset: 0x000CD086
		// (set) Token: 0x0600744E RID: 29774 RVA: 0x000CEE8E File Offset: 0x000CD08E
		public double X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y (second) component of the vector.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F7 RID: 5367
		// (get) Token: 0x0600744F RID: 29775 RVA: 0x000CEE97 File Offset: 0x000CD097
		// (set) Token: 0x06007450 RID: 29776 RVA: 0x000CEE9F File Offset: 0x000CD09F
		public double Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = value;
			}
		}

		/// <summary>
		/// Gets or sets the Z (third) component of the vector.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014F8 RID: 5368
		// (get) Token: 0x06007451 RID: 29777 RVA: 0x000CEEA8 File Offset: 0x000CD0A8
		// (set) Token: 0x06007452 RID: 29778 RVA: 0x000CEEB0 File Offset: 0x000CD0B0
		public double Z
		{
			get
			{
				return this.m_z;
			}
			set
			{
				this.m_z = value;
			}
		}

		/// <summary>
		/// Gets or sets a vector component at the given index.
		/// </summary>
		/// <param name="index">Index of vector component. Valid values are: 
		/// <para>0 = X-component</para>
		/// <para>1 = Y-component</para>
		/// <para>2 = Z-component</para>
		/// .</param>
		// Token: 0x170014F9 RID: 5369
		public double this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.m_x;
				}
				if (1 == index)
				{
					return this.m_y;
				}
				if (2 == index)
				{
					return this.m_z;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				if (index == 0)
				{
					this.m_x = value;
					return;
				}
				if (1 == index)
				{
					this.m_y = value;
					return;
				}
				if (2 == index)
				{
					this.m_z = value;
					return;
				}
				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this vector is valid. 
		/// A valid vector must be formed of valid component values for x, y and z.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FA RID: 5370
		// (get) Token: 0x06007455 RID: 29781 RVA: 0x000CEF0A File Offset: 0x000CD10A
		public bool IsValid
		{
			get
			{
				return RhinoMath.IsValidDouble(this.m_x) && RhinoMath.IsValidDouble(this.m_y) && RhinoMath.IsValidDouble(this.m_z);
			}
		}

		/// <summary>
		/// Gets the smallest (both positive and negative) component value in this vector.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FB RID: 5371
		// (get) Token: 0x06007456 RID: 29782 RVA: 0x000CEF34 File Offset: 0x000CD134
		public double MinimumCoordinate
		{
			get
			{
				Point3d point3d = new Point3d(this);
				return point3d.MinimumCoordinate;
			}
		}

		/// <summary>
		/// Gets the largest (both positive and negative) component value in this vector,  as an absolute value.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FC RID: 5372
		// (get) Token: 0x06007457 RID: 29783 RVA: 0x000CEF58 File Offset: 0x000CD158
		public double MaximumCoordinate
		{
			get
			{
				Point3d point3d = new Point3d(this);
				return point3d.MaximumCoordinate;
			}
		}

		/// <summary>
		/// Computes the length (or magnitude, or size) of this vector.
		/// This is an application of Pythagoras' theorem.
		/// If this vector is invalid, its length is considered 0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FD RID: 5373
		// (get) Token: 0x06007458 RID: 29784 RVA: 0x000CEF79 File Offset: 0x000CD179
		public double Length
		{
			get
			{
				return Vector3d.GetLengthHelper(this.m_x, this.m_y, this.m_z);
			}
		}

		/// <summary>
		/// Computes the squared length (or magnitude, or size) of this vector.
		/// This is an application of Pythagoras' theorem.
		/// While the Length property checks for input validity,
		/// this property does not. You should check validity in advance,
		/// if this vector can be invalid.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FE RID: 5374
		// (get) Token: 0x06007459 RID: 29785 RVA: 0x000CEF92 File Offset: 0x000CD192
		public double SquareLength
		{
			get
			{
				return this.m_x * this.m_x + this.m_y * this.m_y + this.m_z * this.m_z;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not this is a unit vector. 
		/// A unit vector has length 1.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x170014FF RID: 5375
		// (get) Token: 0x0600745A RID: 29786 RVA: 0x000CEFBD File Offset: 0x000CD1BD
		public bool IsUnitVector
		{
			get
			{
				return Math.Abs(Vector3d.GetLengthHelper(this.m_x, this.m_y, this.m_z) - 1.0) <= 1.490116119385E-08;
			}
		}

		/// <summary>
		/// Determines whether a vector is very short.
		/// </summary>
		/// <param name="tolerance">
		/// A nonzero value used as the coordinate zero tolerance.
		/// .</param>
		/// <returns>(Math.Abs(X) &lt;= tiny_tol) AND (Math.Abs(Y) &lt;= tiny_tol) AND (Math.Abs(Z) &lt;= tiny_tol).</returns>
		/// <example>
		/// <code source="examples\vbnet\ex_addline.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_addline.cs" lang="cs" />
		/// <code source="examples\py\ex_addline.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x0600745B RID: 29787 RVA: 0x000CEFF3 File Offset: 0x000CD1F3
		public bool IsTiny(double tolerance)
		{
			return UnsafeNativeMethods.ON_3dVector_IsTiny(this, tolerance);
		}

		/// <summary>
		/// Uses RhinoMath.ZeroTolerance for IsTiny calculation.
		/// </summary>
		/// <returns>true if vector is very small, otherwise false.</returns>
		/// <since>5.0</since>
		// Token: 0x0600745C RID: 29788 RVA: 0x000CF001 File Offset: 0x000CD201
		public bool IsTiny()
		{
			return this.IsTiny(2.3283064365386963E-10);
		}

		/// <summary>
		/// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
		/// </summary>
		/// <since>5.0</since>
		// Token: 0x17001500 RID: 5376
		// (get) Token: 0x0600745D RID: 29789 RVA: 0x000CF012 File Offset: 0x000CD212
		public bool IsZero
		{
			get
			{
				return this.m_x == 0.0 && this.m_y == 0.0 && this.m_z == 0.0;
			}
		}

		/// <summary>
		/// Determines whether the specified System.Object is a Vector3d and has the same values as the present vector.
		/// </summary>
		/// <param name="obj">The specified object.</param>
		/// <returns>true if obj is a Vector3d and has the same coordinates as this; otherwise false.</returns>
		// Token: 0x0600745E RID: 29790 RVA: 0x000CF049 File Offset: 0x000CD249
		[ConstOperation]
		public override bool Equals(object obj)
		{
			return obj is Vector3d && this == (Vector3d)obj;
		}

		/// <summary>
		/// Determines whether the specified vector has the same value as the present vector.
		/// </summary>
		/// <param name="vector">The specified vector.</param>
		/// <returns>true if vector has the same coordinates as this; otherwise false.</returns>
		/// <since>5.0</since>
		// Token: 0x0600745F RID: 29791 RVA: 0x000CF066 File Offset: 0x000CD266
		[ConstOperation]
		public bool Equals(Vector3d vector)
		{
			return this == vector;
		}

		/// <summary>
		/// Check that all values in other are within epsilon of the values in this
		/// </summary>
		/// <param name="other"></param>
		/// <param name="epsilon"></param>
		/// <returns></returns>
		/// <since>5.4</since>
		// Token: 0x06007460 RID: 29792 RVA: 0x000CF074 File Offset: 0x000CD274
		[ConstOperation]
		public bool EpsilonEquals(Vector3d other, double epsilon)
		{
			return RhinoMath.EpsilonEquals(this.m_x, other.m_x, epsilon) && RhinoMath.EpsilonEquals(this.m_y, other.m_y, epsilon) && RhinoMath.EpsilonEquals(this.m_z, other.m_z, epsilon);
		}

		/// <summary>
		/// Compares this <see cref="T:Rhino.Geometry.Vector3d" /> with another <see cref="T:Rhino.Geometry.Vector3d" />.
		/// <para>Component evaluation priority is first X, then Y, then Z.</para>
		/// </summary>
		/// <param name="other">The other <see cref="T:Rhino.Geometry.Vector3d" /> to use in comparison.</param>
		/// <returns>
		/// <para> 0: if this is identical to other</para>
		/// <para>-1: if this.X &lt; other.X</para>
		/// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
		/// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
		/// <para>+1: otherwise.</para>
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x06007461 RID: 29793 RVA: 0x000CF0B4 File Offset: 0x000CD2B4
		[ConstOperation]
		public int CompareTo(Vector3d other)
		{
			if (this.m_x < other.m_x)
			{
				return -1;
			}
			if (this.m_x > other.m_x)
			{
				return 1;
			}
			if (this.m_y < other.m_y)
			{
				return -1;
			}
			if (this.m_y > other.m_y)
			{
				return 1;
			}
			if (this.m_z < other.m_z)
			{
				return -1;
			}
			if (this.m_z > other.m_z)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06007462 RID: 29794 RVA: 0x000CF122 File Offset: 0x000CD322
		[ConstOperation]
		int IComparable.CompareTo(object obj)
		{
			if (obj is Vector3d)
			{
				return this.CompareTo((Vector3d)obj);
			}
			throw new ArgumentException("Input must be of type Vector3d", "obj");
		}

		/// <summary>
		/// Computes the hash code for the current vector.
		/// </summary>
		/// <returns>A non-unique number that represents the components of this vector.</returns>
		// Token: 0x06007463 RID: 29795 RVA: 0x000CF148 File Offset: 0x000CD348
		[ConstOperation]
		public override int GetHashCode()
		{
			return this.m_x.GetHashCode() ^ this.m_y.GetHashCode() ^ this.m_z.GetHashCode();
		}

		/// <summary>
		/// Returns the string representation of the current vector, in the form X,Y,Z.
		/// </summary>
		/// <returns>A string with the current location of the point.</returns>
		// Token: 0x06007464 RID: 29796 RVA: 0x000CF170 File Offset: 0x000CD370
		[ConstOperation]
		public override string ToString()
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			return string.Format("{0},{1},{2}", this.m_x.ToString(invariantCulture), this.m_y.ToString(invariantCulture), this.m_z.ToString(invariantCulture));
		}

		/// <inheritdoc />
		/// <since>7.0</since>
		// Token: 0x06007465 RID: 29797 RVA: 0x000CF1B1 File Offset: 0x000CD3B1
		[ConstOperation]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return Point3d.FormatCoordinates(format, formatProvider, new double[]
			{
				this.m_x,
				this.m_y,
				this.m_z
			});
		}

		/// <summary>
		/// Unitizes the vector in place. A unit vector has length 1 unit. 
		/// <para>An invalid or zero length vector cannot be unitized.</para>
		/// </summary>
		/// <returns>true on success or false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x06007466 RID: 29798 RVA: 0x000CF1DB File Offset: 0x000CD3DB
		public bool Unitize()
		{
			return this.IsValid && UnsafeNativeMethods.ON_3dVector_Unitize(ref this);
		}

		/// <summary>
		/// Transforms the vector in place.
		/// <para>The transformation matrix acts on the left of the vector; i.e.,</para>
		/// <para>result = transformation*vector</para>
		/// </summary>
		/// <param name="transformation">Transformation matrix to apply.</param>
		/// <since>5.0</since>
		// Token: 0x06007467 RID: 29799 RVA: 0x000CF1F0 File Offset: 0x000CD3F0
		public void Transform(Transform transformation)
		{
			double x = transformation.m_00 * this.m_x + transformation.m_01 * this.m_y + transformation.m_02 * this.m_z;
			double y = transformation.m_10 * this.m_x + transformation.m_11 * this.m_y + transformation.m_12 * this.m_z;
			double z = transformation.m_20 * this.m_x + transformation.m_21 * this.m_y + transformation.m_22 * this.m_z;
			this.m_x = x;
			this.m_y = y;
			this.m_z = z;
		}

		/// <summary>
		/// Rotates this vector around a given axis.
		/// </summary>
		/// <param name="angleRadians">Angle of rotation (in radians).</param>
		/// <param name="rotationAxis">Axis of rotation.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <since>5.0</since>
		// Token: 0x06007468 RID: 29800 RVA: 0x000CF290 File Offset: 0x000CD490
		public bool Rotate(double angleRadians, Vector3d rotationAxis)
		{
			if (RhinoMath.IsValidDouble(angleRadians) && rotationAxis.IsValid)
			{
				UnsafeNativeMethods.ON_3dVector_Rotate(ref this, angleRadians, rotationAxis);
				return true;
			}
			return false;
		}

		/// <summary>
		///  Reverses this vector in place (reverses the direction).
		///  <para>If this vector is Invalid, no changes will occur and false will be returned.</para>
		/// </summary>
		/// <remarks>Similar to <see cref="M:Rhino.Geometry.Vector3d.Negate(Rhino.Geometry.Vector3d)">Negate</see>, that is only provided for CLR language compliance.</remarks>
		/// <returns>true on success or false if the vector is invalid.</returns>
		///  <since>5.0</since>
		// Token: 0x06007469 RID: 29801 RVA: 0x000CF2AE File Offset: 0x000CD4AE
		public bool Reverse()
		{
			if (!this.IsValid)
			{
				return false;
			}
			this.m_x = -this.m_x;
			this.m_y = -this.m_y;
			this.m_z = -this.m_z;
			return true;
		}

		/// <summary>
		/// Determines whether this vector is parallel to another vector, within one degree (within Pi / 180). 
		/// </summary>
		/// <param name="other">Vector to use for comparison.</param>
		/// <returns>
		/// Parallel indicator:
		/// <para>+1 = both vectors are parallel</para>
		/// <para> 0 = vectors are not parallel, or at least one of the vectors is zero</para>
		/// <para>-1 = vectors are anti-parallel.</para>
		/// </returns>
		/// <example>
		/// <code source="examples\vbnet\ex_intersectlines.vb" lang="vbnet" />
		/// <code source="examples\cs\ex_intersectlines.cs" lang="cs" />
		/// <code source="examples\py\ex_intersectlines.py" lang="py" />
		/// </example>
		/// <since>5.0</since>
		// Token: 0x0600746A RID: 29802 RVA: 0x000CF2E2 File Offset: 0x000CD4E2
		[ConstOperation]
		public int IsParallelTo(Vector3d other)
		{
			return this.IsParallelTo(other, 0.017453292519943295);
		}

		/// <summary>
		/// Determines whether this vector is parallel to another vector, within a provided tolerance. 
		/// </summary>
		/// <param name="other">Vector to use for comparison.</param>
		/// <param name="angleTolerance">Angle tolerance (in radians).</param>
		/// <returns>
		/// Parallel indicator:
		/// <para>+1 = both vectors are parallel.</para>
		/// <para>0 = vectors are not parallel or at least one of the vectors is zero.</para>
		/// <para>-1 = vectors are anti-parallel.</para>
		/// </returns>
		/// <since>5.0</since>
		// Token: 0x0600746B RID: 29803 RVA: 0x000CF2F4 File Offset: 0x000CD4F4
		[ConstOperation]
		public int IsParallelTo(Vector3d other, double angleTolerance)
		{
			return UnsafeNativeMethods.ON_3dVector_IsParallelTo(this, other, angleTolerance);
		}

		/// <summary>
		///  Test to see whether this vector is perpendicular to within one degree of another one. 
		/// </summary>
		///  <param name="other">Vector to compare to.</param>
		/// <returns>true if both vectors are perpendicular, false if otherwise.</returns>
		///  <since>5.0</since>
		// Token: 0x0600746C RID: 29804 RVA: 0x000CF303 File Offset: 0x000CD503
		[ConstOperation]
		public bool IsPerpendicularTo(Vector3d other)
		{
			return this.IsPerpendicularTo(other, 0.017453292519943295);
		}

		/// <summary>
		///  Determines whether this vector is perpendicular to another vector, within a provided angle tolerance. 
		/// </summary>
		///  <param name="other">Vector to use for comparison.</param>
		///  <param name="angleTolerance">Angle tolerance (in radians).</param>
		/// <returns>true if vectors form Pi-radians (90-degree) angles with each other; otherwise false.</returns>
		///  <since>5.0</since>
		// Token: 0x0600746D RID: 29805 RVA: 0x000CF318 File Offset: 0x000CD518
		[ConstOperation]
		public bool IsPerpendicularTo(Vector3d other, double angleTolerance)
		{
			bool result = false;
			double num = this.Length * other.Length;
			if (num > 0.0 && Math.Abs((this.m_x * other.m_x + this.m_y * other.m_y + this.m_z * other.m_z) / num) <= Math.Sin(angleTolerance))
			{
				result = true;
			}
			return result;
		}

		/// <summary>
		///  Sets this vector to be perpendicular to another vector. 
		///  Result is not unitized.
		/// </summary>
		///  <param name="other">Vector to use as guide.</param>
		/// <returns>true on success, false if input vector is zero or invalid.</returns>
		///  <since>5.0</since>
		// Token: 0x0600746E RID: 29806 RVA: 0x000CF37D File Offset: 0x000CD57D
		public bool PerpendicularTo(Vector3d other)
		{
			return UnsafeNativeMethods.ON_3dVector_PerpendicularTo(ref this, other);
		}

		/// <summary>
		/// Set this vector to be perpendicular to a plane defined by 3 points.
		/// </summary>
		/// <param name="point0">The first point.</param>
		/// <param name="point1">The second point.</param>
		/// <param name="point2">The third point.</param>
		/// <returns></returns>
		/// <since>8.0</since>
		// Token: 0x0600746F RID: 29807 RVA: 0x000CF386 File Offset: 0x000CD586
		public bool PerpendicularTo(Point3d point0, Point3d point1, Point3d point2)
		{
			return UnsafeNativeMethods.ON_3dVector_PerpendicularTo2(ref this, point0, point1, point2);
		}

		// Token: 0x06007470 RID: 29808 RVA: 0x000CF394 File Offset: 0x000CD594
		internal static double GetLengthHelper(double dx, double dy, double dz)
		{
			if (!RhinoMath.IsValidDouble(dx) || !RhinoMath.IsValidDouble(dy) || !RhinoMath.IsValidDouble(dz))
			{
				return 0.0;
			}
			double num = Math.Abs(dx);
			double num2 = Math.Abs(dy);
			double num3 = Math.Abs(dz);
			double num4;
			if (num2 >= num && num2 >= num3)
			{
				num4 = num;
				num = num2;
				num2 = num4;
			}
			else if (num3 >= num && num3 >= num2)
			{
				num4 = num;
				num = num3;
				num3 = num4;
			}
			if (num > 2.2250738585072014E-308)
			{
				num4 = 1.0 / num;
				num2 *= num4;
				num3 *= num4;
				num4 = num * Math.Sqrt(1.0 + num2 * num2 + num3 * num3);
			}
			else if (num > 0.0 && RhinoMath.IsValidDouble(num))
			{
				num4 = num;
			}
			else
			{
				num4 = 0.0;
			}
			return num4;
		}

		// Token: 0x06007471 RID: 29809 RVA: 0x000CF454 File Offset: 0x000CD654
		object ICloneable.Clone()
		{
			return this;
		}

		// Token: 0x04001969 RID: 6505
		internal double m_x;

		// Token: 0x0400196A RID: 6506
		internal double m_y;

		// Token: 0x0400196B RID: 6507
		internal double m_z;
	}
}
