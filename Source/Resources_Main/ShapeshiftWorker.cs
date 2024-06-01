using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ShapeshiftModeDef : Def
	{

		[NoTranslate]
		public string iconPath;

		public Texture2D uiIcon;

		public int uiOrder;

		public List<HediffDef> hediffDefs;

		public List<GeneDef> reqGeneDefs;

		public Type workerClass = typeof(ShapeshifterModeWorker);

		[Unsaved(false)]
		private ShapeshifterModeWorker workerInt;

		public ShapeshifterModeWorker Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (ShapeshifterModeWorker)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

		public override void PostLoad()
		{
			if (!string.IsNullOrEmpty(iconPath))
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					uiIcon = ContentFinder<Texture2D>.Get(iconPath);
				});
			}
		}

	}

	public class ShapeshifterModeWorker
	{

		public virtual void Notify_ChangeHediffs()
		{
			
		}

	}

}
