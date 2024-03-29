using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AddOrRemoveHediff : Gene
	{

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			Local_AddOrRemoveHediff();
		}

		public void Local_AddOrRemoveHediff()
		{
			AddOrRemoveHediff(Props.hediffDefName, pawn, this, Props.bodyparts);
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(67200))
			{
				return;
			}
			Local_AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff(Props.hediffDefName, pawn);
		}

		// Static
		public static void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene, List<BodyPartDef> bodyparts = null)
		{
			if (hediffDef == null)
			{
				return;
			}
			if (gene.Active)
			{
				if (!pawn.health.hediffSet.HasHediff(hediffDef))
				{
					if (!bodyparts.NullOrEmpty())
					{
						Gene_PermanentHediff.BodyPartsGiver(bodyparts, pawn, hediffDef, gene);
						return;
					}
					// pawn.health.AddHediff(hediffDef);
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (hediff_GeneCheck != null)
					{
						hediff_GeneCheck.geneDef = gene.def;
					}
					pawn.health.AddHediff(hediff);
				}
			}
			else
			{
				RemoveHediff(hediffDef, pawn);
			}
		}

		public static void RemoveHediff(HediffDef hediffDef, Pawn pawn)
		{
			if (hediffDef == null)
			{
				return;
			}
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}
		// Static

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						if (Active)
						{
							Local_AddOrRemoveHediff();
						}
					}
				};
			}
		}

	}

	// public class Gene_HediffGiver : Gene
	// {

		// public List<HediffDef> HediffDefs => def.GetModExtension<GeneExtension_Giver>().hediffDefs;
		// public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// foreach (HediffDef hediffDef in HediffDefs)
			// {
				// if (!pawn.health.hediffSet.HasHediff(hediffDef))
				// {
					// Gene_PermanentHediff.BodyPartsGiver(Bodyparts, pawn, hediffDef);
				// }
			// }
		// }

	// }

	public class Gene_PermanentHediff : Gene
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public override void PostAdd()
		{
			base.PostAdd();
			if (!pawn.health.hediffSet.HasHediff(Props.hediffDefName))
			{
				BodyPartsGiver(Props.bodyparts, pawn, Props.hediffDefName, this);
			}
		}

		public static void BodyPartsGiver(List<BodyPartDef> bodyparts, Pawn pawn, HediffDef hediffDef, Gene gene)
		{
			int num = 0;
			foreach (BodyPartDef bodypart in bodyparts)
			{
				if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
				{
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (hediff_GeneCheck != null)
					{
						hediff_GeneCheck.geneDef = gene.def;
					}
					pawn.health.AddHediff(hediff, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
					num++;
				}
			}
		}

	}

	public class Gene_GenerateHediffWithRandomSeverity : Gene
	{

		public HediffDef HediffDef => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			// Hediff hediff = HediffMaker.MakeHediff(HediffDef, pawn);
			// FloatRange floatRange = new(HediffDef.minSeverity, HediffDef.maxSeverity);
			// hediff.Severity = floatRange.RandomInRange;
			// pawn.health.AddHediff(hediff);
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(67200))
			{
				return;
			}
			AddOrRemoveHediff(HediffDef, pawn, this);
		}

		public static void AddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene)
		{
			if (gene.Active)
			{
				if (!pawn.health.hediffSet.HasHediff(hediffDef))
				{
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (hediff_GeneCheck != null)
					{
						hediff_GeneCheck.geneDef = gene.def;
					}
					FloatRange floatRange = new(hediffDef.minSeverity, hediffDef.maxSeverity);
					hediff.Severity = floatRange.RandomInRange;
					pawn.health.AddHediff(hediff);
					// pawn.health.AddHediff(hediffDef);
				}
			}
			else
			{
				Gene_AddOrRemoveHediff.RemoveHediff(hediffDef, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Gene_AddOrRemoveHediff.RemoveHediff(HediffDef, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						AddOrRemoveHediff(HediffDef, pawn, this);
					}
				};
			}
		}

	}

	public class Gene_ResurgentHediff : Gene_ResurgentDependent
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(67200))
			{
				return;
			}
			AddOrRemoveHediff();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			RemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			if (Active)
			{
				Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (gene_Resurgent != null)
				{
					if (gene_Resurgent.Value >= def.resourceLossPerDay)
					{
						if (!pawn.health.hediffSet.HasHediff(HediffDefName))
						{
							// pawn.health.AddHediff(HediffDefName);
							Hediff hediff = HediffMaker.MakeHediff(HediffDefName, pawn);
							HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
							if (hediff_GeneCheck != null)
							{
								hediff_GeneCheck.geneDef = this.def;
							}
						}
					}
					else
					{
						RemoveHediff();
					}
				}
			}
			else
			{
				RemoveHediff();
			}
		}

		public void RemoveHediff()
		{
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefName);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						AddOrRemoveHediff();
					}
				};
			}
		}

	}

}
