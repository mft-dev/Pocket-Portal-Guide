using UnityEngine;

namespace Pocket_Portal_Guide
{
	class Portal
	{
		private ZDO _instance;
		public Minimap.PinData MapPin { get; set; }
		public bool MapPinRemoved { get; set; } = false;
		public ZDOID Id
		{
			get
			{
				return _instance.m_uid;
			}
		}
		public Vector3 Position
		{
			get
			{
				return _instance.GetPosition();
			}
		}
		public string Tag
		{
			get
			{
				return _instance.GetString("tag", PortalManager.Instance.UntaggedPortalPinName);
			}
		}
		public ZDOID Target
		{
			get
			{
				return _instance.GetZDOID("target");
			}
		}
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
