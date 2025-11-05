using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MorpherXenotypeTargeter : Gene_MorpherDependant
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public XenotypeHolder choosenXenotype = null;

		public virtual XenotypeHolder TargetedXenotype
		{
			get
			{
				if (choosenXenotype != null)
				{
					return choosenXenotype;
				}
				//Log.Error("1");
				return new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(Giver?.morpherXenotypeChances, null));
			}
		}


		public List<XenotypeDef> cachedXenotypeDefs;
		public virtual List<XenotypeDef> PossibleXenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					List<XenotypeDef> list = new();
					foreach (XenotypeChance chance in Giver?.morpherXenotypeChances)
					{
						list.Add(chance.xenotype);
					}
					cachedXenotypeDefs = list;
				}
				return cachedXenotypeDefs;
			}
		}

	}

	public class Gene_MorpherXenotypeTargeter_WithMatch : Gene_MorpherXenotypeTargeter, IGeneOverridden
	{

		public override XenotypeHolder TargetedXenotype
		{
			get
			{
				return new(PossibleXenotypeDefs.RandomElement());
			}
		}

		public override List<XenotypeDef> PossibleXenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					List<XenotypeDef> list = new();
					foreach (XenotypeDef def in XaG_GeneUtility.GetAllMatchedXenotypes(pawn, DefDatabase<XenotypeDef>.AllDefsListForReading, percent: Giver.match))
					{
						list.Add(def);
					}
					cachedXenotypeDefs = list;
				}
				return cachedXenotypeDefs;
			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			cachedXenotypeDefs = null;
		}

		public void Notify_Override()
		{
			cachedXenotypeDefs = null;
		}

	}

	public class Gene_MorpherXenotypeTargeter_Hivemind : Gene_MorpherXenotypeTargeter, IGeneOverridden
	{

		public override XenotypeHolder TargetedXenotype
		{
			get
			{
				return new(PossibleXenotypeDefs.RandomElement());
			}
		}

		public new static List<XenotypeDef> cachedXenotypeDefs;
		public override List<XenotypeDef> PossibleXenotypeDefs
		{
			get
			{
				if (cachedXenotypeDefs == null)
				{
					List<XenotypeDef> list = new();
					List<XenotypeDef> xenotypeDefs = ListsUtility.GetAllXenotypesExceptAndroids();
					foreach (Pawn hiver in HivemindUtility.HivemindPawns)
					{
						list.AddRangeSafe(XaG_GeneUtility.GetAllMatchedXenotypes(hiver, xenotypeDefs, 1f));
					}
					cachedXenotypeDefs = list;
				}
				return cachedXenotypeDefs;
			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			cachedXenotypeDefs = null;
		}

		public void Notify_Override()
		{
			cachedXenotypeDefs = null;
		}

	}

}
