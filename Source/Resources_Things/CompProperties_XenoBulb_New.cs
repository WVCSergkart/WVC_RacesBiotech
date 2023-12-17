using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompXenoBulb : ThingComp
	{

		public CompProperties_XenoTree Props => (CompProperties_XenoTree)props;

		// public CompXenoTree mainXenoTree;

		public Thing mainXenoTree;

		private List<Thing> GetAllXenoTreesOnMap()
		{
			List<Thing> trees = new();
			List<Thing> allThings = parent.Map.listerThings.AllThings;
			foreach (Thing item in allThings)
			{
				if (item.def.plant != null && item.TryGetComp<CompXenoTree>() != null)
				{
					trees.Add(item);
				}
			}
			return trees;
		}

		private int rootsCooldown = 0;

		public override void PostDrawExtraSelectionOverlays()
		{
			if (mainXenoTree != null && mainXenoTree != null)
			{
				GenDraw.DrawLineBetween(parent.TrueCenter(), mainXenoTree.TrueCenter(), SimpleColor.Yellow);
			}
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Reset cooldown",
					action = delegate
					{
						rootsCooldown = 0;
					}
				};
			}
			if (Find.TickManager.TicksGame < rootsCooldown)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_XenoTreeChooseLabel".Translate(),
				defaultDesc = "WVC_XaG_XenoTreeChooseDesc".Translate(),
				icon = parent.def.uiIcon,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Thing> xenoTrees = GetAllXenoTreesOnMap();
					for (int i = 0; i < xenoTrees.Count; i++)
					{
						Thing xenoTree = xenoTrees[i];
						list.Add(new FloatMenuOption((i + 1).ToString() + " " + xenoTree.def.LabelCap, delegate
						{
							if (mainXenoTree != null)
							{
								mainXenoTree.TryGetComp<CompXenoTree>().connectedBulbs.Remove(parent);
							}
							mainXenoTree = xenoTree;
							xenoTree.TryGetComp<CompXenoTree>().connectedBulbs.Add(parent);
							rootsCooldown = Find.TickManager.TicksGame + Props.rootsConnectionCooldown;
							Messages.Message("WVC_XaG_XenoTreeChooseAssigned".Translate(parent.LabelCap, xenoTree.LabelCap), null, MessageTypeDefOf.NeutralEvent, historical: false);
						}));
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
				}
			};
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			if (mainXenoTree != null)
			{
				mainXenoTree.TryGetComp<CompXenoTree>().connectedBulbs.Remove(parent);
			}
		}

		public override string CompInspectStringExtra()
		{
			if (mainXenoTree == null)
			{
				return "WVC_XaG_XenoTreeXenoBulb_ReqRootConnection".Translate(parent.LabelCap).Colorize(ColorLibrary.RedReadable);
			}
			if (Find.TickManager.TicksGame < rootsCooldown)
			{
				return "WVC_XaG_XenoTreeXenoBulb_CooldownRootConnection".Translate(rootsCooldown.ToStringTicksToPeriod());
			}
			return null;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			// Scribe_Values.Look(ref tickCounter, "tickCounter_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref rootsCooldown, "rootsCooldown_" + Props.uniqueTag, 0);
			Scribe_References.Look(ref mainXenoTree, "mainXenoTree");
			// Scribe_Values.Look(ref mainXenoTree, "mainXenoTree");
		}
	}

}
