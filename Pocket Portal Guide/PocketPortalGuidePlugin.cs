using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Main plugin loader.
	/// </summary>
	[BepInPlugin("dk.mft_dev.pocket_portal_guide", "Pocket Portal Guide", "1.0.0.0")]
    [BepInProcess("valheim.exe")]
    public class PocketPortalGuidePlugin : BaseUnityPlugin
    {
		private Harmony _h;

        void Awake()
		{
			LogManager.Init((l, s) => Logger.Log(l, s));

			bool showPins = Config.Bind("Minimap", "Show Pins", false, "Value indicating whether to show map pins for portals by default").Value;
			string untaggedPinName = Config.Bind("Minimap", "Untagged Portal Label", "-untagged-", "The name to give untagged portal pins").Value;
			KeyCode key = Config.Bind("Toggle Show Pins", "Key", KeyCode.F8, "Key to press to toggle showing map pins. Combine with the Modifier key to create CTRL+P combinations.").Value;
			KeyCode mod = Config.Bind("Toggle Show Pins", "Modifier", KeyCode.None, "The key to hold while pressing the Key to toggle. If you only wish to use a single key, set this to None").Value;

			PortalManager.Init(untaggedPinName);
			MinimapManager.Init(showPins);

			InputManager.Init(mod, key);
			InputManager.Instance.MapPinToggled += (s, e) => { MinimapManager.Instance.ShowMapPins = !MinimapManager.Instance.ShowMapPins; };

			LogManager.Instance.Log($"[Toggle] Modifier={InputManager.Instance.Modifier}, Key={InputManager.Instance.Key}");
		
			_h = new Harmony("dk.mft_dev.pocket_portal_guide");
            _h.PatchAll();
			LogManager.Instance.Log("[Harmony] PatchAll() done.");
		}

		void Update()
		{
			InputManager.Instance.Update();
		}

		void OnDestroy()
		{
			_h.UnpatchSelf();
		}
	}
}
