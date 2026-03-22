using RimWorld;

namespace WVC_XenotypesAndGenes
{
	public class Thought_RecluseMemory : Thought_Memory
	{

		public override bool ShouldDiscard => !permanent;

		public override bool VisibleInNeedsTab => BaseMoodOffset != 0;

		public static float? baseMoodOffset;
		protected override float BaseMoodOffset
		{
			get
			{
				if (baseMoodOffset == null)
				{
					baseMoodOffset = GetOffset();
				}
				return baseMoodOffset.Value;
			}
		}

		private float GetOffset()
		{
			if (RimWorld.ThoughtUtility.ThoughtNullified(pawn, def) || StaticCollectionsClass.cachedNonDeathrestingColonistsCount <= 1)
			{
				return 0f;
			}
			return CurStage.baseMoodEffect * (1 + (StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.2f) - 0.4f);
		}

		public override float MoodOffset()
		{
			return BaseMoodOffset;
		}

	}

}
