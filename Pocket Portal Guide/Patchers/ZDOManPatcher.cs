using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Harmony Patcher for <see cref="ZDOMan"/>
	/// </summary>
	[HarmonyPatch]
	class ZDOManPatcher
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZDOMan), "Update")]
		public static void ZDOMan_Update_Postfix(ref ZDOMan __instance)
		{
			if (PortalManager.Instance.PortalPrefab == null) return;

			List<ZDO> portals = new List<ZDO>();
			__instance.GetAllZDOsWithPrefab(PortalManager.Instance.PortalPrefab.name, portals);

			PortalManager.Instance.UpdatePortals(portals);
		}
	}
}
