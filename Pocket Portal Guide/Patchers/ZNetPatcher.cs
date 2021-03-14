using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Harmony Patcher for <see cref="ZNet"/>
	/// </summary>
	[HarmonyPatch]
	class ZNetPatcher
	{
		/// <summary>
		/// Raised when a world is loaded. Contains information on Portal prefab and hash code
		/// </summary>
		public static event EventHandler<PortalInformationEventArgs> PortalInformation;
		/// <summary>
		/// Postfix patch for Znet.Loadworld
		/// <para>Used only to get the portal prefab hash code</para>
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="___m_zdoMan"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZNet), "LoadWorld")]
		public static void ZNet_LoadWorld_Postfix(ref ZNet __instance, ref ZDOMan ___m_zdoMan)
		{
			if (Game.instance == null)
			{
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Error, $"[LoadWorld] Game.instance == null");
				return;
			}
			if (Game.instance.m_portalPrefab == null)
			{
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Error, $"[LoadWorld] Game.instance.m_portalPrefab == null");
				return;
			}
			if (___m_zdoMan == null)
			{
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Error, $"[LoadWorld] this.m_zdoMan == null");
				return;
			}
			GameObject portalPrefab = Game.instance.m_portalPrefab;
			int portalPrefabHashcode = portalPrefab.name.GetStableHashCode();

			PortalInformation?.Invoke(null, new PortalInformationEventArgs(portalPrefab, portalPrefabHashcode));

		}
	}
}
