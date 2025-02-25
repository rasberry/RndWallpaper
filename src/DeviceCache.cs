using System;
using System.Collections.Generic;

namespace RndWallpaper;
#pragma warning disable CA1416 // Validate platform compatibility

// needed a way to dispose of the devices so using this class to store
//  and manage the lifetime of the instances.
public sealed class DeviceCache : IDisposable
{
	public IDevice GetDevice(PickDevice device)
	{
		if (Cache.TryGetValue(device, out IDevice inst)) {
			return inst;
		}


		switch (device) {
		case PickDevice.WindowsDesktop: inst = new Windows.DeviceWinLegacy(); break;
		case PickDevice.VirtualWindows: inst = new Windows.DeviceWinVirtual(); break;
		}

		if (inst != null) {
			Cache.Add(device,inst);
		}

		return inst;
	}

	readonly Dictionary<PickDevice,IDevice> Cache = new();

	public void Dispose()
	{
		foreach(var kvp in Cache) {
			kvp.Value.Dispose();
		}
	}
}