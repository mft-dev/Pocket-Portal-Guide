using System.Collections.Generic;
using System.Linq;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Manages map pins for portals on the game's Minimap
	/// </summary>
	class MinimapManager
	{
		public static MinimapManager Instance { get; private set; }

		private List<Minimap.PinData> _addedPins = new List<Minimap.PinData>();

		private bool _showMapPins = false;
		public bool ShowMapPins
		{
			get
			{
				return _showMapPins;
			}
			set
			{
				_showMapPins = value;
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Info, $"[MinimapManager] ShowMapPins toggled: {_showMapPins}");
			}
		}
		public static void Init(bool showPins)
		{
			Instance = new MinimapManager(showPins);
		}

		public MinimapManager(bool showPins)
		{
			_showMapPins = showPins;
			MinimapPatcher.PortalPinAdded += MinimapPatcher_PinAdded;
			MinimapPatcher.PortalPinRemoved += MinimapPatcher_PinRemoved;
		}

		private void MinimapPatcher_PinRemoved(object sender, Minimap.PinData e)
		{
			Minimap.PinData pin = _addedPins.FirstOrDefault(p => p.m_pos == e.m_pos);
			if (pin != null)
			{
				_addedPins.Remove(pin);
			}
		}

		private void MinimapPatcher_PinAdded(object sender, Minimap.PinData e)
		{
			if (!_addedPins.Any(pin => pin.m_pos == e.m_pos))
			{
				_addedPins.Add(e);
			}
		}
		/// <summary>
		/// Returns a list of removed Portals since the last time this method was called.
		/// <para>NOTE - DESTRUCTIVE. This method will empty the internal list of removed portals</para>
		/// </summary>
		/// <returns></returns>
		public List<Portal> GetPinsToRemove()
		{
			List<Portal> removed = PortalManager.Instance.EmptyRemovedPortalsList();
			List<Portal> pins = new List<Portal>();
			pins.AddRange(removed.Where(p => p.MapPin != null));
			if (!ShowMapPins)
			{
				List<Portal> active = PortalManager.Instance.GetPortalList();
				pins.AddRange(active.Where(p => p.MapPin != null));
			}
			return pins;
		}
		/// <summary>
		/// Returns a list of portals that should have their pin showing on the game's Minimap
		/// </summary>
		/// <param name="ignoreShowPinToggle"></param>
		/// <returns></returns>
		public List<Portal> GetPinsToShow(bool ignoreShowPinToggle = false)
		{
			if (ShowMapPins || ignoreShowPinToggle)
			{
				return PortalManager.Instance.GetPortalList();
			}
			return new List<Portal>();
		}
		/// <summary>
		/// Retireves a list of all pins this instance has knowledge of.
		/// </summary>
		/// <returns></returns>
		public List<Minimap.PinData> GetAllPins()
		{
			return _addedPins;
		}
	}
}
