using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Manages knowledge of portals in the game.
	/// <para>Uses event handlers to keep informed about added/removed portals</para>
	/// </summary>
	class PortalManager
	{
		private static object _syncRoot = new object();
		public static PortalManager Instance { get; private set; }

		public GameObject PortalPrefab { get; set; }
		public int PortalPrefabHashCode { get; set; }
		public string UntaggedPortalPinName { get; private set; }
		private List<Portal> _portals = new List<Portal>();
		private List<Portal> _removedPortals = new List<Portal>();
		public static void Init(string untaggedPortalPinName)
		{
			Instance = new PortalManager(untaggedPortalPinName);
		}

		public PortalManager(string untaggedPortalPinName)
		{
			UntaggedPortalPinName = untaggedPortalPinName;
			ZNetPatcher.PortalFound += ZNetPatcher_PortalFound;
			ZNetPatcher.PortalInformation += ZNetPatcher_PortalInformation;
			ZDOManPatcher.PortalCreated += ZDOManPatcher_PortalCreated;
			ZDOManPatcher.PortalDestroyed += ZDOManPatcher_PortalDestroyed;
		}
		/// <summary>
		/// Returns a list of removed portals and empties the internal list
		/// </summary>
		/// <returns></returns>
		public List<Portal> EmptyRemovedPortalsList()
		{
			lock (_syncRoot)
			{
				List<Portal> res = _removedPortals;
				_removedPortals = new List<Portal>();
				return res;
			}
		}
		/// <summary>
		/// Retrieves a list of existing portals that this manager is aware of.
		/// </summary>
		/// <returns></returns>
		public List<Portal> GetPortalList()
		{
			lock (_syncRoot)
			{
				return new List<Portal>(_portals);
			}
		}

		private void TryAddPortal(Portal p)
		{
			if (!_portals.Any(x => x.Id == p.Id))
			{
				_portals.Add(p);
			}
		}

		private void ZDOManPatcher_PortalDestroyed(object sender, ZDOID e)
		{
			Portal p = _portals.FirstOrDefault(x => x.Id == e);
			if (p != null)
			{
				_portals.Remove(p);
				_removedPortals.Add(p);
			}
		}

		private void ZDOManPatcher_PortalCreated(object sender, Portal e)
		{
			TryAddPortal(e);
		}

		private void ZNetPatcher_PortalInformation(object sender, PortalInformationEventArgs e)
		{
			PortalPrefab = e.PortalPrefab;
			PortalPrefabHashCode = e.PrefabHashCode;
		}

		private void ZNetPatcher_PortalFound(object sender, Portal e)
		{
			TryAddPortal(e);
		}
	}
}
