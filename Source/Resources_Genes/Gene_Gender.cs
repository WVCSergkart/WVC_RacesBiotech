using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Gender : Gene, IGeneOverridden
	{

		private GeneExtension_Giver cachedGeneExtensionGiver;
		public GeneExtension_Giver Props
		{
			get
			{
				if (cachedGeneExtensionGiver == null)
				{
					cachedGeneExtensionGiver = def?.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtensionGiver;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ChangeGender();
		}

		public virtual void ChangeGender()
		{
			if (!Active)
			{
				return;
			}
			if (pawn.gender == Props.gender)
			{
				return;
			}
			if (Props.gender != Gender.None)
			{
				SetGender(pawn, Props.gender);
			}
		}

		public static void SetGender(Pawn pawn, Gender gender)
        {
            try
            {
                pawn.gender = gender;
                if (gender == Gender.Female && pawn.story?.bodyType == BodyTypeDefOf.Male)
                {
                    pawn.story.bodyType = BodyTypeDefOf.Female;
                    GetRandomHeadFromSet(pawn);
                }
                else if (gender == Gender.Male && pawn.story?.bodyType == BodyTypeDefOf.Female)
                {
                    pawn.story.bodyType = BodyTypeDefOf.Male;
                    GetRandomHeadFromSet(pawn);
                }
                if (MiscUtility.GameNotStarted() && pawn.Name is NameTriple nameTriple)
                {
                    pawn.Name = new NameTriple(nameTriple.First, nameTriple.First, nameTriple.Last);
                }
                if (!pawn.style.CanWantBeard && pawn.style.beardDef != BeardDefOf.NoBeard)
                {
                    pawn.style.beardDef = BeardDefOf.NoBeard;
                }
            }
            catch (Exception arg)
            {
				Log.Error("Failed update pawn gender. For pawn: " + pawn.Name + ". Reason: " + arg.Message);
            }
            pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public static void GetRandomHeadFromSet(Pawn pawn)
        {
            try
            {
                pawn.story.TryGetRandomHeadFromSet((pawn.genes.GenesListForReading?.First((gene) => gene.def.forcedHeadTypes != null)?.def?.forcedHeadTypes ?? DefDatabase<HeadTypeDef>.AllDefs.Where((HeadTypeDef x) => x.randomChosen)));
            }
            catch (Exception arg)
            {
				Log.Error("Failed fix pawn head during gender update. Reason: "+ arg.Message);
            }
        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {

        }

        public void Notify_Override()
		{
			ChangeGender();
		}

    }

	public class Gene_Feminine : Gene_GauntSkin
	{

		public override void ChangeBodyType()
		{
			if (!Active)
			{
				return;
			}
			if (pawn?.DevelopmentalStage != DevelopmentalStage.Adult)
			{
				return;
			}
			if (pawn?.story?.bodyType != null && pawn.story.bodyType != BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Female;
			}
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		public override void Notify_OverriddenBy(Gene overriddenBy)
		{
			Remove();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Remove();
		}

		private void Remove()
		{
			if (pawn?.DevelopmentalStage != DevelopmentalStage.Adult)
			{
				return;
			}
			if (pawn.gender == Gender.Male && pawn.story?.bodyType == BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Male;
			}
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
		}

		// public override void ExposeData()
		// {
		// base.ExposeData();
		// ChangeBodyType();
		// }

	}

	public class Gene_GenderMorph : Gene, IGeneRemoteControl
	{

		public string RemoteActionName => choosenGender.ToStringHuman().CapitalizeFirst();

		public TaggedString RemoteActionDesc => "WVC_XaG_PostImplanterSubGeneDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			List<FloatMenuOption> list = new();
			list.Add(new FloatMenuOption("Female".Translate().CapitalizeFirst(), delegate
			{
				choosenGender = Gender.Female;
			}));
			list.Add(new FloatMenuOption("Male".Translate().CapitalizeFirst(), delegate
			{
				choosenGender = Gender.Male;
			}));
			list.Add(new FloatMenuOption("WVC_None".Translate(), delegate
			{
				choosenGender = Gender.None;
			}));
			Find.WindowStack.Add(new FloatMenu(list));
		}

		public bool RemoteControl_Hide => !Active;

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

		//===========

		public Gender choosenGender = Gender.None;

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59993, delta))
			{
				return;
			}
			GenderMorph();
		}

		public void GenderMorph()
		{
			if (pawn.gender == choosenGender || choosenGender == Gender.None)
			{
				return;
			}
			Gene_Gender.SetGender(pawn, choosenGender);
			//MiscUtility.DoShapeshiftEffects_OnPawn(pawn);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref choosenGender, "choosenGender", Gender.None);
		}

	}

}
