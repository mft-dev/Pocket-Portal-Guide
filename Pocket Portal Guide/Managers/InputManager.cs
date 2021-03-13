using System;
using UnityEngine;

namespace Pocket_Portal_Guide
{
	/// <summary>
	/// Manages input detecting for the toggle switch keyboard combination set in Config
	/// </summary>
	class InputManager
	{
		public event EventHandler MapPinToggled;
		public KeyCode Modifier { get; private set; } = KeyCode.None;
		public KeyCode Key { get; private set; } = KeyCode.None;
		public static InputManager Instance { get; private set; }
		public static void Init(KeyCode mod, KeyCode key)
		{
			Instance = new InputManager(mod, key);
		}

		private InputManager(KeyCode mod, KeyCode key)
		{
			Modifier = mod;
			Key = key;
		}
		/// <summary>
		/// Update. Should be called once per UnityEngine.Update() run. Will raise <see cref="MapPinToggled"/> if the configured key combination is detected
		/// </summary>
		public void Update()
		{
			bool modifier = true;
			if (Modifier != KeyCode.None)
			{
				modifier = Input.GetKey(Modifier);
			}
			if (modifier)
			{
				if (Input.GetKeyUp(Key))
				{
					MapPinToggled?.Invoke(this, EventArgs.Empty);
				}
			}
		}
	}
}
