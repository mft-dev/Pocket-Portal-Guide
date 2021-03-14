using System;
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
		public static PortalManager Instance { get; private set; }
		public GameObject PortalPrefab { get; set; }
		public int PortalPrefabHashCode { get; set; }
		public string UntaggedPortalPinName { get; private set; }

		private static object _syncRoot = new object();
		private List<Portal> _portals = new List<Portal>();
		private List<Portal> _removedPortals = new List<Portal>();
		private Dictionary<string, PortalPair> _pairs = new Dictionary<string, PortalPair>();
		public static void Init(string untaggedPortalPinName)
		{
			Instance = new PortalManager(untaggedPortalPinName);
		}

		public PortalManager(string untaggedPortalPinName)
		{
			UntaggedPortalPinName = untaggedPortalPinName;
			ZNetPatcher.PortalInformation += ZNetPatcher_PortalInformation;
		}

		public Portal GetPortalByZDOID(ZDOID id)
		{
			return _portals.FirstOrDefault(p => p.Id == id);
		}

		/// <summary>
		/// Updates the internal list of known Portals. Any Portals destroyed are placed in the removed portals list
		/// </summary>
		/// <param name="zdos"></param>
		public void UpdatePortals(List<ZDO> zdos)
		{
			List<Portal> removed = new List<Portal>();
			foreach(Portal p in _portals)
			{
				ZDO obj = zdos.FirstOrDefault(z => z.m_uid == p.Id);
				if (obj == null)
				{
					removed.Add(p);
				}
			}
			foreach(Portal p in removed)
			{
				_portals.Remove(p);
			}
			foreach(ZDO z in zdos)
			{
				Portal existing = GetPortalByZDOID(z.m_uid);
				if (existing == null)
				{
					_portals.Add(new Portal(z));
				}
			}
			UpdateConnectedPortals();
		}
		/// <summary>
		/// Updates connected portal pairs and assigns the found pairs a color if they have none
		/// </summary>
		public void UpdateConnectedPortals()
		{
			_pairs.Clear();
			foreach(Portal p in _portals)
			{
				if (!p.Target.IsNone())
				{
					if (!_pairs.ContainsKey(p.Tag))
					{
						_pairs[p.Tag] = new PortalPair();
						_pairs[p.Tag].Tag = p.Tag;
					}
					PortalPair pair = _pairs[p.Tag];
					if (pair.One == null)
					{
						pair.One = p;
					}
					else if (pair.Two == null)
					{
						pair.Two = p;
					}
					else
					{
						LogManager.Instance.Log(BepInEx.Logging.LogLevel.Warning, $"[UpdateConnectedPortals] {p} is connected to two other portals: {pair.One}, {pair.Two}");
					}
					if (pair.One != null && pair.Two != null)
					{
						Portal one = pair.One;
						Portal two = pair.Two;
						Color oneColor = one.AssignedColor;
						Color twoColor = two.AssignedColor;
						if (oneColor == Color.white && twoColor == Color.white)
						{
							Color r = MinimapManager.Instance.GetRandomColor();
							one.AssignedColor = r;
							two.AssignedColor = r;
						}
						else if (oneColor != Color.white)
						{
							two.AssignedColor = one.AssignedColor;
						}
						else
						{
							one.AssignedColor = two.AssignedColor;
						}
					}
				}
				else
				{
					p.AssignedColor = Color.white;
				}
			}
			var unconnected = _pairs.Values.Where(pair => pair.One == null || pair.Two == null);
			foreach(PortalPair pair in unconnected)
			{
				_pairs.Remove(pair.Tag);
			}
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

		private void ZNetPatcher_PortalInformation(object sender, PortalInformationEventArgs e)
		{
			PortalPrefab = e.PortalPrefab;
			PortalPrefabHashCode = e.PrefabHashCode;
		}
	}
}
