using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Harmony Patcher for <see cref="Minimap"/>
	/// </summary>
	[HarmonyPatch]
	class MinimapPatcher
	{
		/// <summary>
		/// Raised when a map pin for a Portal has been added
		/// </summary>
		public static event EventHandler<Minimap.PinData> PortalPinAdded;
		/// <summary>
		/// Raised when a map pin for a Portal has been removed
		/// </summary>
		public static event EventHandler<Minimap.PinData> PortalPinRemoved;
		public static Minimap.PinType PinType { get; set; } = Minimap.PinType.Icon4;
		private static MinimapWrapper _miniMap = null;

		/// <summary>
		/// Prefix patch for Minimap.UpdatePins.
		/// <para>First removes all pins of Portals that have been removed since the last UpdatePins call</para>
		/// <para>Then adds map pins for all Portals with missing pins</para>
		/// <para>Lastly it updates the name field of all pins to reflect the portal's tag</para>
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Minimap), "UpdatePins")]
		public static void Minimap_UpdatePins_Prefix(ref Minimap __instance)
		{
			foreach (Portal portal in MinimapManager.Instance.GetPinsToRemove())
			{
				_miniMap.RemovePin(portal.Position);
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Warning, $"[UpdateMapPins] Removed pin for {portal}");
				PortalPinRemoved?.Invoke(null, portal.MapPin);
				portal.MapPin = null;
			}
			AddPortalPins(__instance);
		}

		private static void AddPortalPins(Minimap __instance)
		{
			foreach (Portal portal in MinimapManager.Instance.GetPinsToShow())
			{
				if (portal.MapPin != null && !_miniMap.HaveSimilarPin(portal.MapPin.m_pos, portal.MapPin.m_type, portal.MapPin.m_name, portal.MapPin.m_save))
				{
					_miniMap.RemovePin(portal.Position);
					portal.MapPin = null;
				}
				if (portal.MapPin == null)
				{
					portal.MapPin = __instance.AddPin(portal.Position, PinType, portal.Tag, true, false);
					PortalPinAdded?.Invoke(null, portal.MapPin);
				}
				if (portal.MapPin != null)
				{
					portal.MapPin.m_name = portal.Tag;
				}
				if (portal.MapPin.m_uiElement != null && MinimapManager.Instance.UseColorCoding)
				{
					Image icon = portal.MapPin.m_uiElement.GetComponent<Image>();
					icon.color = portal.AssignedColor;
				}
			}
		}

		/// <summary>
		/// Prefix patch for Minimap.SaveMapData
		/// <para>Removes all Portal pins from the map to avoid they end up in the saved game data</para>
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Minimap), "SaveMapData")]
		public static void Minimap_SaveMapData_Prefix(ref Minimap __instance)
		{
			LogManager.Instance.Log($"[SaveMapData] Removing pins");
			foreach (Minimap.PinData pin in MinimapManager.Instance.GetAllPins())
			{
				_miniMap.RemovePin(pin.m_pos);
			}
		}
		/// <summary>
		/// Postfix patch for Minimap.SaveMapData
		/// <para>Re-adds all pins removed in the prefix</para>
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Minimap), "SaveMapData")]
		public static void Minimap_SaveMapData_Postfix(ref Minimap __instance)
		{
			LogManager.Instance.Log($"[SaveMapData] Adding pins back");
			AddPortalPins(__instance);
		}
		/// <summary>
		/// Prefix patch for Minimap.AddPin
		/// <para>For debugging purposes. Not really needed</para>
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="pos"></param>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="save"></param>
		/// <param name="isChecked"></param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Minimap), "AddPin")]
		public static void Minimap_AddPin_Prefix(ref Minimap __instance, Vector3 pos, Minimap.PinType type, string name, bool save, bool isChecked)
		{
			LogManager.Instance.Log(BepInEx.Logging.LogLevel.Info, $"[AddPin] {pos} {type} {name} {save} {isChecked}");
		}
		/// <summary>
		/// Postfix patch for Minimap.Awake
		/// <para>Instantiates the MinimapWrapper that exposes needed functionality from Minimap</para>
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Minimap), "Awake")]
		public static void Minimap_Awake_Postfix(ref Minimap __instance)
		{
			_miniMap = new MinimapWrapper(__instance);
		}
	}
}
