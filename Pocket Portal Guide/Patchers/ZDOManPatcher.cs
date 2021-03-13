using HarmonyLib;
using System;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Harmony Patcher for <see cref="ZDOMan"/>
	/// </summary>
	[HarmonyPatch]
	class ZDOManPatcher
	{
		/// <summary>
		/// Raised when a Portal is placed in the world
		/// </summary>
		public static event EventHandler<Portal> PortalCreated;
		/// <summary>
		/// Raised when a Portal is destroyed
		/// </summary>
		public static event EventHandler<ZDOID> PortalDestroyed;

		/// <summary>
		/// Postfix patch for ZDOMan.CreateNewZDO
		/// <para>Detects whether the created ZDO is a Portal, and raises <see cref="PortalCreated"/> if it is</para>
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="position"></param>
		/// <param name="__result"></param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZDOMan), "CreateNewZDO", new Type[] { typeof(ZDOID), typeof(Vector3) })]
		public static void ZDOMan_CreateNewZDO_Postfix(ZDOID uid, Vector3 position, ref ZDO __result)
		{
			if (__result == null || __result.GetPrefab() != PortalManager.Instance.PortalPrefabHashCode)
			{
				return;
			}
			Portal kp = new Portal(__result);
			PortalCreated?.Invoke(null, kp);
			LogManager.Instance.Log(BepInEx.Logging.LogLevel.Info, $"[CreateNewZDO] {kp}");
		}
		/// <summary>
		/// Prefix patch for ZDOMan.DestroyZDO
		/// <para>Detects whether the destroyed ZDO is a Portal, and raises <see cref="PortalDestroyed"/> if it is</para>
		/// </summary>
		/// <param name="zdo"></param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(ZDOMan), "DestroyZDO")]
		public static void ZDOMan_DestroyZDO_Prefix(ZDO zdo)
		{
			if (zdo.GetPrefab() == PortalManager.Instance.PortalPrefabHashCode)
			{
				PortalDestroyed?.Invoke(null, zdo.m_uid);
			}
		}
	}
}
