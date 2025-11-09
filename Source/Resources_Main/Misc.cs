using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PatchOperationOptional : PatchOperation
	{
		public string settingName;
		public PatchOperation caseTrue;
		public PatchOperation caseFalse;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) != true && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}

	public class PatchOperationLegacyMode : PatchOperationOptional
	{

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Biotech.settings.EnableLegacyMode && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (!WVC_Biotech.settings.EnableLegacyMode && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}

	public class PatchOperationSafeAdd : PatchOperationPathed
	{

		public int safetyDepth = -1;

		public XmlContainer value;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			XmlNode node = value.node;
			foreach (XmlNode item in xml.SelectNodes(xpath))
			{
				// XmlNode xmlNode = item as XmlNode;
				foreach (XmlNode childNode in node.ChildNodes)
				{
					TryAddNode(item, childNode, 0);
				}
			}
			return true;
		}

		private void TryAddNode(XmlNode parent, XmlNode addNode, int depth)
		{
			XmlNode foundNode = null;
			if (!ContainsNode(parent, addNode, ref foundNode) || depth >= safetyDepth)
			{
				parent.AppendChild(parent.OwnerDocument.ImportNode(addNode, deep: true));
			}
			else
			{
				if (!addNode.HasChildNodes || !addNode.FirstChild.HasChildNodes)
				{
					return;
				}
				foreach (XmlNode childNode in addNode.ChildNodes)
				{
					TryAddNode(foundNode, childNode, depth + 1);
				}
			}
		}

		private bool ContainsNode(XmlNode parent, XmlNode node, ref XmlNode foundNode)
		{
			foreach (XmlNode childNode in parent.ChildNodes)
			{
				if (childNode.Name != node.Name && childNode.InnerText != node.InnerText)
				{
					continue;
				}
				foundNode = childNode;
				return true;
			}
			foundNode = null;
			return false;
		}

	}

	public class Command_GenesSettings : Command_Action
	{

		public bool visible = true;

		public override bool Visible => WVC_Biotech.settings.showGenesSettingsGizmo && visible;

	}

	public class Command_Ability_FloatMenu : Command_Ability
	{

		public Command_Ability_FloatMenu(Ability ability, Pawn pawn)
			: base(ability, pawn)
		{

		}

		private List<FloatMenuOption> cachedFloatMenuOptions;
		public List<FloatMenuOption> FloatMenuOptions
		{
			get
			{
				if (cachedFloatMenuOptions == null)
				{
					List<FloatMenuOption> allOptions = new();
					foreach (CompAbilityEffect compAbilityEffect in ability.EffectComps)
					{
						if (compAbilityEffect is IAbilityFloatMenu abilityFloatMenu)
						{
							allOptions.AddRange(abilityFloatMenu.FloatMenuOptions);
						}
					}
					cachedFloatMenuOptions = allOptions;
				}
				return cachedFloatMenuOptions;
			}
		}

		public void Recache()
		{
			cachedFloatMenuOptions = null;
		}

		public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
		{
			get
			{
				return FloatMenuOptions;
			}
		}

	}

	[Obsolete]
	public class Command_Ability_ThrallMaker : Command_Ability_FloatMenu
	{

		public Command_Ability_ThrallMaker(Ability ability, Pawn pawn)
			: base(ability, pawn)
		{

		}

		public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
		{
			get
			{
				List<FloatMenuOption> list = new();
				foreach (Gene gene in Pawn.genes.GenesListForReading)
				{
					if (gene is Gene_ThrallMaker maker)
					{
						list.Add(new FloatMenuOption("WVC_XaG_GeneThrallMaker_MenuLabel".Translate() + " (" + maker.LabelCap + ")", delegate
						{
							maker.ThrallMakerDialog();
						}));
					}
				}
				return list;
			}
		}

	}

	[Obsolete]
	public class Command_HiddenAbility : Command_Ability_ThrallMaker
	{

		public Command_HiddenAbility(Ability ability, Pawn pawn)
			: base(ability, pawn)
		{

		}

		//public override bool Visible => false;

	}

}
