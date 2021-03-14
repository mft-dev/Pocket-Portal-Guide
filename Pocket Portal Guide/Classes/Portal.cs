using UnityEngine;

namespace Pocket_Portal_Guide
{
	class PortalPair
	{
		public Portal One { get; set; } = null;
		public Portal Two { get; set; } = null;
		public string Tag { get; set; } = null;

		public void AddPortal(Portal p)
		{
			if (One == null) One = p;
			else Two = p;
		}

	}
	class Portal
	{
		public Color AssignedColor { get; set; } = Color.white;
		private ZDO _instance;
		public Minimap.PinData MapPin { get; set; }
		public bool MapPinRemoved { get; set; } = false;
		public ZDOID Id => _instance.m_uid;
		public Vector3 Position => _instance.GetPosition();
		public string Tag => _instance.GetString("tag", PortalManager.Instance.UntaggedPortalPinName);
		public ZDOID Target => _instance.GetZDOID("target");
		public Portal(ZDO portal)
		{
			_instance = portal;
		}

		public override string ToString()
		{
			return $"Portal at {Position} with tag=\"{Tag}\"";
		}
	}
}
