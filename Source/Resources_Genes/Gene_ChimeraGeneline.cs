using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ChimeraGeneline : Gene_ChimeraDependant
	{

		//public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		[Unsaved(false)]
		private List<GeneDef> cachedGenelineGenes;
		public virtual List<GeneDef> GenelineGenes
		{
			get
			{
				if (cachedGenelineGenes == null)
				{
					List<GeneDef> geneDefs = new();
					if (Extension_Giver.geneDefs != null)
					{
						foreach (GeneDef geneDef in Extension_Giver.geneDefs)
						{
							AddGene(geneDefs, geneDef);
						}
					}
					if (Extension_Giver.geneCategoryDefs != null)
					{
						foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading.Where((gene) => Extension_Giver.geneCategoryDefs.Contains(gene.displayCategory)))
						{
							AddGene(geneDefs, geneDef);
						}
					}
					if (Extension_Giver.excludedGeneDefs != null)
					{
						foreach (GeneDef geneDef in Extension_Giver.excludedGeneDefs)
						{
							if (geneDefs.Contains(geneDef))
							{
								geneDefs.Remove(geneDef);
							}
						}
					}
					cachedGenelineGenes = geneDefs;
				}
				return cachedGenelineGenes;

				static void AddGene(List<GeneDef> geneDefs, GeneDef geneDef)
				{
					if (!geneDefs.Contains(geneDef))
					{
						geneDefs.Add(geneDef);
					}
				}
			}
		}

		//public override bool EnableCooldown => true;
		//public override bool DisableSubActions => true;
		//public override IntRange? ReqMetRange => new(0, 0);

	}

	public class Gene_Chimera_GeneDatabase : Gene_ChimeraGeneline
	{


		private static List<GeneDef> cachedGenelineGenes;
		public override List<GeneDef> GenelineGenes
		{
			get
			{
				if (cachedGenelineGenes == null)
				{
					cachedGenelineGenes = Database.ToList();
				}
				return cachedGenelineGenes;
			}
		}

		public static IEnumerable<GeneDef> Database => DefDatabase<GeneDef>.AllDefsListForReading.Where((def) => !def.IsAndroid());

	}

	/// <summary>
	/// Dormant hivemind gene. Dormant hivemind gene - do not cause recache and synchronization, but are still considered hivemind genes.
	/// </summary>
	public class Gene_Chimera_HiveGeneline : Gene_ChimeraGeneline, IGeneHivemind, IGeneNonSync
	{

		public static List<GeneDef> cachedGenelineGenes;
		public override List<GeneDef> GenelineGenes
		{
			get
			{
				if (cachedGenelineGenes == null)
				{
					List<GeneDef> geneDefs = new();
					SetHiveMindGeneline(geneDefs);
					cachedGenelineGenes = geneDefs;
				}
				return cachedGenelineGenes;
			}
		}

		private static void SetHiveMindGeneline(List<GeneDef> geneDefs)
		{
			if (HivemindUtility.HivemindPawns.NullOrEmpty())
			{
				return;
			}
			foreach (Pawn hivePawn in HivemindUtility.HivemindPawns)
			{
				foreach (Gene gene in hivePawn.genes.GenesListForReading)
				{
					if (gene is not Gene_Chimera chimera || !chimera.Active)
					{
						continue;
					}
					foreach (GeneDef geneDef in chimera.CollectedGenes)
					{
						if (geneDefs.Contains(geneDef))
						{
							continue;
						}
						geneDefs.Add(geneDef);
					}
				}
			}
		}

		//public override void PostAdd()
		//{
		//    base.PostAdd();
		//    ResetCollection();
		//}

		//public void ResetCollection()
		//{
		//    if (!HivemindUtility.SuitableForHivemind(pawn))
		//    {
		//        return;
		//    }
		//    HivemindUtility.ResetCollection();
		//}

		//public void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//    ResetCollection();
		//}

		//public void Notify_Override()
		//{
		//    ResetCollection();
		//}

		//public override void PostRemove()
		//{
		//    base.PostRemove();
		//    ResetCollection();
		//}

	}

}
