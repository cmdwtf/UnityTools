using System;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class MathExtensions
	{
		public static sbyte Positive(this sbyte b) => Math.MakePositive(b);
		public static short Positive(this short s) => Math.MakePositive(s);
		public static int Positive(this int i) => Math.MakePositive(i);
		public static long Positive(this long l) => Math.MakePositive(l);
		public static float Positive(this float f) => Math.MakePositive(f);
		public static double Positive(this double d) => Math.MakePositive(d);
		public static decimal Positive(this decimal m) => Math.MakePositive(m);

		public static sbyte Negative(this sbyte b) => Math.MakeNegative(b);
		public static short Negative(this short s) => Math.MakeNegative(s);
		public static int Negative(this int i) => Math.MakeNegative(i);
		public static long Negative(this long l) => Math.MakeNegative(l);
		public static float Negative(this float f) => Math.MakeNegative(f);
		public static double Negative(this double d) => Math.MakeNegative(d);
		public static decimal Negative(this decimal m) => Math.MakeNegative(m);

		public static int ClosestMultipleOf(this int x, int y) => Math.ClosestMultiple(x, y);
		public static float ClosestMultipleOf(this float x, float y) => Math.ClosestMultiple(x, y);
		public static double ClosestMultipleOf(this double x, double y) => Math.ClosestMultiple(x, y);

		public static bool IsNegative(this sbyte b) => b < 0;
		public static bool IsNegative(this byte _) => false;
		public static bool IsNegative(this short s) => s < 0;
		public static bool IsNegative(this ushort _) => false;
		public static bool IsNegative(this int i) => i < 0;
		public static bool IsNegative(this uint _) => false;
		public static bool IsNegative(this long l) => l < 0;
		public static bool IsNegative(this ulong _) => false;
		public static bool IsNegative(this float f) => f < 0;
		public static bool IsNegative(this double d) => d < 0;
		public static bool IsNegative(this decimal m) => m < 0;

		public static sbyte Clamp01(this sbyte sb) => sb.Clamp(0, 1);
		public static byte Clamp01(this byte b) => b.Clamp(0, 1);
		public static short Clamp01(this short s) => s.Clamp(0, 1);
		public static ushort Clamp01(this ushort us) => us.Clamp(0, 1);
		public static int Clamp01(this int i) => i.Clamp(0, 1);
		public static uint Clamp01(this uint ui) => ui.Clamp(0, 1);
		public static long Clamp01(this long l) => l.Clamp(0, 1);
		public static ulong Clamp01(this ulong ul) => ul.Clamp(0, 1);
		public static float Clamp01(this float f) => f.Clamp(0, 1);
		public static double Clamp01(this double d) => d.Clamp(0, 1);
		public static decimal Clamp01(this decimal m) => m.Clamp(0, 1);
		public static Vector2 Clamp01(this Vector2 v) => v.Clamp(Vector2.zero, Vector2.one);
		public static Vector3 Clamp01(this Vector3 v) => v.Clamp(Vector3.zero, Vector3.one);
		public static Vector4 Clamp01(this Vector4 v) => v.Clamp(Vector4.zero, Vector4.one);

		public static sbyte Clamp(this sbyte sb, sbyte min, sbyte max)
			=> (sb > max ? max : (sb < min ? min : sb));
		public static byte Clamp(this byte b, byte min, byte max)
			=> (b > max ? max : (b < min ? min : b));
		public static short Clamp(this short s, short min, short max)
			=> (s > max ? max : (s < min ? min : s));
		public static ushort Clamp(this ushort us, ushort min, ushort max)
			=> (us > max ? max : (us < min ? min : us));
		public static int Clamp(this int i, int min, int max)
			=> (i > max ? max : (i < min ? min : i));
		public static uint Clamp(this uint ui, uint min, uint max)
			=> (ui > max ? max : (ui < min ? min : ui));
		public static long Clamp(this long l, long min, long max)
			=> (l > max ? max : (l < min ? min : l));
		public static ulong Clamp(this ulong ul, ulong min, ulong max)
			=> (ul > max ? max : (ul < min ? min : ul));
		public static float Clamp(this float f, float min, float max)
			=> (f > max ? max : (f < min ? min : f));
		public static double Clamp(this double d, double min, double max)
			=> (d > max ? max : (d < min ? min : d));
		public static decimal Clamp(this decimal m, decimal min, decimal max)
			=> (m > max ? max : (m < min ? min : m));
		public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
			=> new Vector2(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y)
			);
		public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
			=> new Vector3(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y),
				v.z.Clamp(min.z, max.z)
			);
		public static Vector4 Clamp(this Vector4 v, Vector4 min, Vector4 max)
			=> new Vector4(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y),
				v.z.Clamp(min.z, max.z),
				v.w.Clamp(min.w, max.w)
			);

		public static short NthTriangular(this short n) => (short)((n * (n + 1)) / 2);
		public static int NthTriangular(this int n) => (n * (n + 1)) / 2;
		public static long NthTriangular(this long n) => (n * (n + 1)) / 2;
		public static float NthTriangular(this float n) => (n * (n + 1)) / 2.0f;
		public static double NthTriangular(this double n) => (n * (n + 1)) / 2.0d;
		public static decimal NthTriangular(this decimal n) => (n * (n + 1)) / 2.0m;
	}
}
