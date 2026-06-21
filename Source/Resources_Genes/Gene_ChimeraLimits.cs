using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class Gene_ChimeraLimits : Gene_ChimeraDependant
	//{

	//	public virtual int CpxLimitOffset => 0;
	//	public virtual int ArcLimitOffset => 0;

	//}

	/// <summary>
	/// Dormant hivemind gene. Dormant hivemind gene - do not cause recache and synchronization, but are still considered hivemind genes.
	/// </summary>
	public class Gene_HiveMind_ChimeraLimit : Gene_Hivemind_Drone, IGeneChimeraLimit, IGeneNonSync
	{

		public float CpxLimitOffset => HivemindUtility.HivemindPawns.Count;
		public float CpxLimitFactor => 1;

		public float ArcLimitOffset => 0;
		public float ArcLimitFactor => 1;

		//public override void Local_AddOrRemoveHediff()
		//{
		//	if (!HivemindUtility.InHivemind(pawn))
		//	{
		//		Local_RemoveHediff();
		//		return;
		//	}
		//	base.Local_AddOrRemoveHediff();
		//}

	}

	public class Gene_ChimeraBandwidth : Gene_RemoteController, IGeneOverriddenBy, IGeneMechanitorUI, IGeneChimeraLimit
	{

		public override string RemoteActionName => "WVC_HideShow".Translate();
		public override TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlHideBandwitdhDesc".Translate();

		public override void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			shouldHideMechanitorUI = !shouldHideMechanitorUI;
			RecacheCollection();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RecacheCollection();
		}

		// =======================================

		public float CpxLimitOffset
		{
			get
			{
				if (pawn.mechanitor == null)
				{
					return 0f;
				}
				return pawn.mechanitor.TotalBandwidth - pawn.mechanitor.UsedBandwidth;
			}
		}

		public float CpxLimitFactor => 1;

		public float ArcLimitOffset => 0;
		public float ArcLimitFactor => 1;

		// =======================================

		public override void PostAdd()
		{
			base.PostAdd();
			RecacheCollection();
			InitPatch();
		}

		private bool shouldHideMechanitorUI;

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			RecacheCollection();
		}

		public void Notify_Override()
		{
			RecacheCollection();
		}

		public static void RecacheCollection()
		{
			MechanoidsUtility.ResetCollection();
		}

		private void InitPatch()
		{
			MechanoidsUtility.HarmonyPatch();
		}

		public bool HideMechanitorGUI => shouldHideMechanitorUI;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref shouldHideMechanitorUI, "shouldHideMechanitorUI", defaultValue: false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				InitPatch();
			}
		}

	}

}
