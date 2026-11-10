using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenogenesEditor_General : Gene_XenogenesEditor, IGeneWithEffects
	{

		protected List<GeneDef> cachedCollectedGenes;
		public override List<GeneDef> CollectedGenes
		{
			get
			{
				if (cachedCollectedGenes == null)
				{
					cachedCollectedGenes = ListsUtility.GeneticDatabase.Where(geneDef =>
					{
						bool category = Extension_Giver.geneCategoryDefs != null && Extension_Giver.geneCategoryDefs.Contains(geneDef.displayCategory);
						bool allowed = Extension_Giver.geneDefs != null && Extension_Giver.geneDefs.Contains(geneDef);
						return Extension_Giver.database || category || allowed;
					}).ToList();
				}
				return cachedCollectedGenes;
			}
		}

		public override List<GeneDef> AllGenes => CollectedGenes;
		public override List<GeneDef> GenelineGenes => CollectedGenes;

		public override int ArchiteLimit => Extension_Giver.arcLimit;
		public override int ComplexityLimit => Extension_Giver.cpxLimit;
		public override IntRange ReqMetRange => Extension_Giver.metLimit;

		public override bool ReqCooldown => Extension_Giver.reqCooldown;

		private Gizmo genesGizmo;
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SkipIf_SelectorDraftedActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (genesGizmo == null)
			{
				genesGizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return genesGizmo;
		}

		public override void AddGene_Editor(GeneDef geneDef)
		{
			if (!this.def.ConflictsWith(geneDef))
			{
				pawn.genes.AddGene(geneDef, true);
			}
		}

		public virtual void DoEffects(Pawn pawn)
		{
			DoEffects();
		}

		public virtual void DoEffects()
		{
			if (pawn.Map == null)
			{
				return;
			}
			EffectsUtility.DoShapeshiftEffects_OnPawn(pawn);
			Extension_Undead?.soundDefOnImplant?.PlayOneShot(SoundInfo.InMap(pawn));
		}

	}

}
