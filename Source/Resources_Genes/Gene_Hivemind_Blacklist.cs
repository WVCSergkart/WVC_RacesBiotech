using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// In Dev
	public class Gene_Hivemind_Blacklist : Gene_Hivemind_Drone, IGeneRemoteControl
	{

		public string RemoteActionName
		{
			get
			{
				return "WVC_Edit".Translate();
			}
		}

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControl_HivemindBlacklistDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			Find.WindowStack.Add(new Dialog_HivemindBlacklist(pawn));
		}

		public bool RemoteControl_Hide => !Active || !pawn.InHivemind();

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
			savedBlacklistedPawns = null;
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

		// ===================

		public override void PostAdd()
		{
			base.PostAdd();
			if (savedBlacklistedPawns != null)
			{
				savedBlacklistedPawns.RemoveAll((Pawn x) => x == null);
			}
		}

		private static List<Pawn> savedBlacklistedPawns;
		public static List<Pawn> BlacklistedPawns
		{
			get
			{
				if (savedBlacklistedPawns == null)
				{
					savedBlacklistedPawns = new();
				}
				return savedBlacklistedPawns;
			}
		}

		public static void UpdateBlacklist(Pawn hiver)
		{
			if (savedBlacklistedPawns == null)
			{
				savedBlacklistedPawns = new();
			}
			if (!savedBlacklistedPawns.Contains(hiver))
			{
				savedBlacklistedPawns.Add(hiver);
			}
			else
			{
				savedBlacklistedPawns.Remove(hiver);
			}
			//HivemindUtility.ResetCollection();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			if (savedBlacklistedPawns != null)
			{
				savedBlacklistedPawns.RemoveAll((Pawn x) => x == null);
			}
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "WVC_XaG_HivemindBlackPawns_Label".Translate(), BlacklistedPawns.Count.ToString(), "WVC_XaG_HivemindBlackPawns_Desc".Translate(), 100, hyperlinks: BlacklistedPawns.Select((pawn) => new Dialog_InfoCard.Hyperlink(pawn)));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref savedBlacklistedPawns, "savedBlacklistedPawns", LookMode.Reference);
			if (savedBlacklistedPawns != null && savedBlacklistedPawns.RemoveAll((Pawn x) => x == null) > 0)
			{
				Log.Warning("Removed null pawn(s) from gene: " + def.defName);
			}
		}

	}

}
