using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_AbilitySpawn : CompProperties_AbilityEffect
	{
		public ThingDef thingDef;

		public int stackCount = 1;

		public CompProperties_AbilitySpawn()
		{
			compClass = typeof(CompAbilityEffect_Spawn);
		}
	}

	public class CompAbilityEffect_Spawn : CompAbilityEffect
	{
		public new CompProperties_AbilitySpawn Props => (CompProperties_AbilitySpawn)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			// GenSpawn.Spawn(Props.thingDef, target.Cell, parent.pawn.Map);
			Thing thing = ThingMaker.MakeThing(Props.thingDef);
			thing.stackCount = Props.stackCount;
			GenPlace.TryPlaceThing(thing, parent.pawn.Position, parent.pawn.Map, ThingPlaceMode.Near, null, null, default);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			return true;
		}
	}

}
