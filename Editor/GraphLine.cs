using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal class GraphLine
	{
		public Vector3[] UIPoints;
		public Color Color;
		public bool Focused;
		public bool Visible;
		public object UserData;
		public float[] OriginalSamples;
		public float SampleMinimum;
		public float SampleMaximum;
	}
}
