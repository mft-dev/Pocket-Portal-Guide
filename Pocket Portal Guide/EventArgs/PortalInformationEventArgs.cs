using UnityEngine;

namespace Pocket_Portal_Guide
{
	public class PortalInformationEventArgs
	{
		public GameObject PortalPrefab { get; set; }
		public int PrefabHashCode { get; set; }

		public PortalInformationEventArgs(GameObject go, int hashCode)
		{
			PortalPrefab = go;
			PrefabHashCode = hashCode;
		}
	}
}
