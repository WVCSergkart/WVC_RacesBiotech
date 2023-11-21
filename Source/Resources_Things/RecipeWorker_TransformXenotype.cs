using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class RecipeWorker_TransformXenotype : RecipeWorker
	{
		// public RecipeDef recipe;

		public virtual void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
		{
			foreach (ThingDef thingDef in recipe.products.thingDef)
			{
				thingDef.
			}
		}

	}

}
