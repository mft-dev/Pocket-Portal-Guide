using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Exposes private methods and fields on Minimap
	/// </summary>
	class MinimapWrapper
	{
		private Minimap _mm;

		public MinimapWrapper(Minimap instance)
		{
			_mm = instance;
			if (_mm == null)
			{
				LogManager.Instance.Log(BepInEx.Logging.LogLevel.Warning, $"MinimapWrapper instantiated with null argument");
			}
		}

		private static FieldInfo reflectedField_m_largeZoom = typeof(Minimap).GetField("m_largeZoom", BindingFlags.NonPublic | BindingFlags.Instance);
		public bool RemovePin(Vector3 worldPos)
		{
			if (_mm == null) return false;
			float largeZoom = (float)reflectedField_m_largeZoom.GetValue(_mm);
			return _mm.RemovePin(worldPos, _mm.m_removeRadius * (largeZoom * 2f));
		}

		private static MethodInfo reflected_WorldToMapPoint = typeof(Minimap).GetMethod("WorldToMapPoint", BindingFlags.NonPublic | BindingFlags.Instance);
		public void WorldToMapPoint(Vector3 p, out float mx, out float my)
		{
			if (_mm == null)
			{
				mx = 0;
				my = 0;
				return;
			}
			object[] coords = new object[] { p, 0f, 0f };
			reflected_WorldToMapPoint.Invoke(_mm, coords);
			mx = (float)coords[1];
			my = (float)coords[2];
		}

		public Vector3 WorldToMapPoint(Vector3 p)
		{
			float mx;
			float my;
			WorldToMapPoint(p, out mx, out my);
			return new Vector3(mx, 0, my);
		}

		private static MethodInfo reflected_HaveSimilarPin = typeof(Minimap).GetMethod("HaveSimilarPin", BindingFlags.NonPublic | BindingFlags.Instance);
		public bool HaveSimilarPin(Vector3 pos, Minimap.PinType type, string name, bool save)
		{
			if (_mm == null) return false;
			return (bool)reflected_HaveSimilarPin.Invoke(_mm, new object[] { pos, type, name, save });
		}

		private static MethodInfo reflected_IsPointVisible = typeof(Minimap).GetMethod("IsPointVisible", BindingFlags.NonPublic | BindingFlags.Instance);
		public bool IsPointVisible(Vector3 p, RawImage map)
		{
			if (_mm == null) return false;
			return (bool)reflected_IsPointVisible.Invoke(_mm, new object[] { p, map });
		}

		private static FieldInfo reflectedField_m_mode = typeof(Minimap).GetField("m_mode", BindingFlags.NonPublic | BindingFlags.Instance);
		public MinimapWrapperMode GetMapMode()
		{
			return (MinimapWrapperMode)reflectedField_m_mode.GetValue(_mm);
		}

		public RawImage GetMapImageForCurrentMode()
		{
			MinimapWrapperMode mode = GetMapMode();
			switch (mode)
			{
				case MinimapWrapperMode.Large: return _mm.m_mapImageLarge;
				case MinimapWrapperMode.Small:
				default: return _mm.m_mapImageSmall;
			}
		}

		public enum MinimapWrapperMode
		{
			None,
			Small,
			Large,
		}
	}
}
