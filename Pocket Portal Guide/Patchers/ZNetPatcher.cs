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
		/// Raised when a Portal is found when the world first loads.
		/// </summary>
		public static event EventHandler<Portal> PortalFound;
		/// <summary>
		/// Postfix patch for Znet.Loadworld
		/// <para>Builds a list of ZDOs that are Portals and communicates this via <see cref="PortalFound"/></para>
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="___m_zdoMan"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZNet), "LoadWorld")]
		public static void ZNet_LoadWorld_Postfix(ref ZNet __instance, ref ZDOMan ___m_zdoMan)
		{
			List<ZDO> portals = new List<ZDO>();
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

			___m_zdoMan.GetAllZDOsWithPrefab(portalPrefab.name, portals);

			foreach (ZDO z1 in portals)
			{
				Portal kp = new Portal(z1);
				PortalFound?.Invoke(null, kp);
				//_portalsByZDOID.Add(kp.Id, kp);
				if (!kp.Target.IsNone())
				{
					ZDO z2 = ZDOMan.instance.GetZDO(kp.Target);
					LogManager.Instance.Log(BepInEx.Logging.LogLevel.Info, $"[LoadWorld] {kp} connected to portal at {z2.GetPosition()}");
				}
				else
				{
					LogManager.Instance.Log(BepInEx.Logging.LogLevel.Info, $"[LoadWorld] {kp} unconnected");
				}
			}
		}
	}
}
