using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools
{
	public static class iOSTools
	{
		public static int iOSCompareToSafe(this Guid lhs, Guid rhs)
		{
#if UNITY_IOS || UNITY_IPHONE
			return lhs.ToString().CompareTo(rhs.ToString());
#else
			return lhs.CompareTo(rhs);
#endif // IOS/IPHONE
		}

		public class iOSGuidComparer : IEqualityComparer<Guid>
		{
			#region IEqualityComparer implementation

			public bool Equals(Guid x, Guid y)
				=> x.iOSCompareToSafe(y) == 0;

			public int GetHashCode(Guid obj)
				=> obj.GetHashCode();

			#endregion
		}
	}
}
