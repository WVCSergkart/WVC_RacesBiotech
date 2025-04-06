using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MechsSummon : Gene, IGeneRemoteControl
	{
		public string RemoteActionName
		{
			get
			{
				if (nextTick > 0)
				{
					return nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return "WVC_XaG_Gene_DustMechlink".Translate();
			}
		}

		public string RemoteActionDesc => "WVC_XaG_RemoteControlMechsSummonAbilityDesc".Translate();

		public void RemoteControl()
		{
			if (!Active || nextTick > 0 || pawn.mechanitor == null)
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return;
			}
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_RemoteControlMechsSummonAbilityDesc".Translate(), delegate
			{
				int countSpawn = Spawner.summonRange.RandomInRange;
				//for (int i = 0; i < countSpawn; i++)
				//{
				//	MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
				//}
				if (Gene_Mechlink.CanDoOrbitalSummon(pawn) && MechanoidsUtility.TrySummonMechanoids(pawn, countSpawn, Spawner.allowedMechWeightClasses, out List<Thing> summonList))
				{
					Messages.Message("WVC_RB_Gene_Summoner".Translate(), new LookTargets(summonList), MessageTypeDefOf.PositiveEvent);
					nextTick = Spawner.spawnIntervalRange.RandomInRange;
				}
				else
                {
					SoundDefOf.ClickReject.PlayOneShotOnCamera();
				}
				//Messages.Message("WVC_RB_Gene_Summoner".Translate(pawn.LabelIndefinite().CapitalizeFirst()), pawn, MessageTypeDefOf.PositiveEvent);
			});
			Find.WindowStack.Add(window);
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.ResetAllRemoteControllers(ref cachedRemoteControlGenes);
		}

		public void RecacheGenes()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref cachedRemoteControlGenes, ref enabled);
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		private List<IGeneRemoteControl> cachedRemoteControlGenes;

		//===========

		public override void PostAdd()
		{
			base.PostAdd();
			if (Current.ProgramState == ProgramState.Playing)
			{
				nextTick = Spawner.spawnIntervalRange.RandomInRange;
			}
		}

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public int nextTick;

		public override void Tick()
		{
			if (nextTick > 0)
			{
				nextTick--;
				if (nextTick == 0)
				{
					Find.LetterStack.ReceiveLetter("AbilityReadyLabel".Translate(def.LabelCap), "AbilityReadyText".Translate(pawn, def.label), LetterDefOf.NeutralEvent, new LookTargets(pawn));
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes);
			}
			return null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

}
