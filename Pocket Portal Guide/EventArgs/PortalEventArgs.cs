using System;

namespace Pocket_Portal_Guide
{
	class PortalEventArgs: EventArgs
	{
		public Portal Portal { get; set; }

		public PortalEventArgs(Portal p)
		{
			Portal = p;
		}

		public static PortalEventArgs Create(Portal p)
		{
			return new PortalEventArgs(p);
		}
	}
}
