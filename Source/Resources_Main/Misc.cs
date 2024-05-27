using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
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

	// public class PatchOperationRemove_XenoGenes : PatchOperationPathed
	// {

		// protected override bool ApplyWorker(XmlDocument xml)
		// {
			// bool result = false;
			// XmlNode[] array = xml.SelectNodes(xpath).Cast<XmlNode>().ToArray();
			// foreach (XmlNode xmlNode in array)
			// {
				// if (xmlNode)
				// {
					// result = true;
					// xmlNode.ParentNode.RemoveChild(xmlNode);
				// }
			// }
			// return result;
		// }

	// }

	// public class PawnRenderNode_FurIsSkin : PawnRenderNode
	// {

		// protected override Shader DefaultShader => ShaderDatabase.CutoutSkinOverlay;

		// public PawnRenderNode_FurIsSkin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			// : base(pawn, props, tree)
		// {
		// }

		// public override Graphic GraphicFor(Pawn pawn)
		// {
			// string bodyPath = TexPathFor(pawn);
			// if (bodyPath == null)
			// {
				// return null;
			// }
			// if (gene is not Gene_Exoskin gene_Exoskin)
			// {
				// return DefaultGraphic(pawn, bodyPath);
			// }
			// GeneExtension_Graphic modExtension = gene_Exoskin.Graphic;
			// if (modExtension == null)
			// {
				// return DefaultGraphic(pawn, bodyPath);
			// }
			// if (modExtension.furIsSkinWithHair)
			// {
				// return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
			// }
			// return DefaultGraphic(pawn, bodyPath);
		// }

		// protected override string TexPathFor(Pawn pawn)
		// {
			// return pawn?.story?.furDef?.GetFurBodyGraphicPath(pawn);
		// }

		// public override Color ColorFor(Pawn pawn)
		// {
			// return pawn.story.SkinColor;
		// }

		// public Graphic DefaultGraphic(Pawn pawn, string bodyPath)
		// {
			// return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderFor(pawn), Vector2.one, ColorFor(pawn));
		// }

	// }


}
