using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class PawnContainerHolder : PawnGeneSetHolder, IThingHolder, ISuspendableThingHolder
	{

		public override TaggedString Label
		{
			get
			{
				if (cachedLabel == null)
				{
					cachedLabel = name;
				}
				return cachedLabel;
			}
		}

		public Pawn owner = null;
		public Pawn holded = null;

		public bool IsNullContainer => owner == null || holded == null;

		public ThingOwner innerContainer;

		public int lastTimeSeenByPlayer = -1;

		public override int AllGenesCount
		{
			get
			{
				if (!cachedGenesCount.HasValue)
				{
					cachedGenesCount = holded.genes.GenesListForReading.Count;
				}
				return cachedGenesCount.Value;
			}
		}

		public bool TrySetContainer(Pawn owner, Pawn toHold)
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
			if (Accepts(toHold))
			{
				this.owner = owner;
				this.holded = toHold;
				name = toHold.Name.ToStringShort;
				xenotypeDef = toHold.genes.Xenotype;
				lastTimeSeenByPlayer = Find.TickManager.TicksGame;
				//endogeneDefs.AddRange(XaG_GeneUtility.ConvertGenesInGeneDefs(toHold.genes.Endogenes));
				//xenogeneDefs.AddRange(XaG_GeneUtility.ConvertGenesInGeneDefs(toHold.genes.Xenogenes));
				if (toHold.genes.UniqueXenotype)
				{
					iconDef = toHold.genes.iconDef;
				}
				toHold.DeSpawnOrDeselect();
				if (innerContainer.TryAdd(toHold))
				{
					return true;
				}
			}
			return false;
		}

		public IThingHolder ParentHolder => owner?.ParentHolder;

		public bool IsContentsSuspended => true;

		public bool Accepts(Thing thing)
		{
			return innerContainer.CanAcceptAnyOf(thing, canMergeWithExistingStacks: false);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public override void ExposeData()
		{
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_References.Look(ref owner, "owner", saveDestroyedThings: true);
			Scribe_References.Look(ref holded, "holded", saveDestroyedThings: true);
			base.ExposeData();
			Scribe_Values.Look(ref lastTimeSeenByPlayer, "lastTimeSeenByPlayer", -1);
		}
	}

}
