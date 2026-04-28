using RimWorld;

namespace WVC_XenotypesAndGenes
{

	public interface IHediffCustomPregnancy
	{

		GeneSet GeneSet { get; }

	}

	public interface IHediffFleshmassOvergrow
	{

		int CurrentLevel { get; }
		void LevelUp();
		string LabelCap { get; }

	}

}
