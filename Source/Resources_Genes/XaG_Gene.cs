using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_Gene : Verse.Gene
	{

		// Cache?
		//public bool? cachedActivity;
		//public override bool Active
		//{
		//	get
		//	{
		//		if (cachedActivity == null)
		//		{
		//			cachedActivity = base.Active;
		//		}
		//		return cachedActivity.Value;
		//	}
		//}

	}

	// Basic remote controller
	public class Gene_RemoteController : XaG_Gene, IGeneRemoteControl
	{

		public virtual string RemoteActionName => "ERR";

		public virtual TaggedString RemoteActionDesc => "ERR";

		public virtual void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{

		}

		public virtual bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				yield return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
		}

	}

}
