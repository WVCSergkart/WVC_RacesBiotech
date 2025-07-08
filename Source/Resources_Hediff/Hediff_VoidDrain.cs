using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_VoidDrain : Hediff
	{

		//private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => true;

		private static Color? cachedColor;
		public override Color LabelColor
		{
			get
			{
				if (!cachedColor.HasValue)
				{
					//cachedColor = new(0.77f, 0.38f, 0.38f, 1f);
					cachedColor = GenColor.FromHex("ff9c9b");
				}
				return cachedColor.Value;
			}
		}

		public override string LabelInBrackets => GetLabel();

		private Pawn master;

		[Unsaved(false)]
		private Gene_VoidHunger cachedGene;

		public Gene_VoidHunger Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = master?.genes?.GetFirstGeneOfType<Gene_VoidHunger>();
				}
				return cachedGene;
			}
		}

		public string GetLabel()
		{
			if (master != null)
			{
				return master.Name.ToStringShort;
			}
			return "";
		}


		private int nextTick = 125;
        public override void TickInterval(int delta)
        {
			if (!GeneResourceUtility.CanTick(ref nextTick, 56756, delta))
            {
				return;
            }
			if (Gene == null || Gene.Victim != pawn)
			{
				pawn.health.RemoveHediff(this);
			}
		}

        public void Notify_VictimChanged()
        {
			pawn.health.RemoveHediff(this);
		}

		public override void Notify_PawnKilled()
		{
			Gene?.ResetVictim();
		}

		public void SetOwner(Pawn master)
		{
			Gene?.ResetVictim(false);
			this.master = master;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref master, "master");
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (master?.Map != pawn.Map)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_VoidDrainSelectMaster".Translate(),
				defaultDesc = "WVC_XaG_VoidDrainSelectMaster_Desc".Translate(),
				icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_SelectVoidDrainMaster"),
				Order = -87f,
				action = delegate
				{
					Find.Selector.ClearSelection();
					Find.Selector.Select(master);
				}
			};
            yield return new Command_Action
            {
                defaultLabel = "WVC_XaG_VoidDrainRemove".Translate(),
                defaultDesc = "WVC_XaG_VoidDrainRemove_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_RemoveVoidDrainMaster"),
                Order = -87f,
                action = delegate
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_VoidDrainRemove_Warning".Translate(), delegate
                    {
                        if (Gene != null)
                        {
                            Gene.ResetVictim(true);
                        }
						else
						{
							pawn.health.RemoveHediff(this);
						}
                    });
					Find.WindowStack.Add(window);
				}
            };
        }

	}

}
