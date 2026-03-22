using RimWorld;

namespace WVC_XenotypesAndGenes
{
	public class Thought_HumanCentricMemory : Thought_Memory
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
			if (RimWorld.ThoughtUtility.ThoughtNullified(pawn, def) || StaticCollectionsClass.cachedNonHumansCount <= 0)
			{
				return 0f;
			}
			return CurStage.baseMoodEffect * (1 + (StaticCollectionsClass.cachedNonHumansCount * 0.1f) - 0.1f);
		}

		public override float MoodOffset()
		{
			return BaseMoodOffset;
		}

	}

}
