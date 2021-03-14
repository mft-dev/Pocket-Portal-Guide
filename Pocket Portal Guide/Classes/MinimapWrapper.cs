using System.Collections.Generic;
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

		private static MethodInfo reflected_HaveSimilarPin = typeof(Minimap).GetMethod("HaveSimilarPin", BindingFlags.NonPublic | BindingFlags.Instance);
		public bool HaveSimilarPin(Vector3 pos, Minimap.PinType type, string name, bool save)
		{
			if (_mm == null) return false;
			return (bool)reflected_HaveSimilarPin.Invoke(_mm, new object[] { pos, type, name, save });
		}
	}
}
