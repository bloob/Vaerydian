using System;
using ECSFramework;
using Vaerydian.Components.Actions;
using Vaerydian.Utils;

namespace Vaerydian.Utils
{
	public struct ActionDef{
		public string Name;
		public short ID;
		public ActionType ActionType;
		public ImpactType ImpactType;
		public DamageDef DamageDef;
		public ModifyType ModifyType;
		public ModifyDuration ModifyDurtion;
		public CreateType CreateType;
		public DestoryType DestoryType;
	}
	
	public enum ActionType{
		DAMAGE = 0,
		MODIFY = 1,
		CREATE = 2,
		DESTROY = 3
	}
	
	public enum ImpactType{
		NONE = 0,
		DIRECT = 1,
		AREA = 2,
		CONE = 3,
		OVERTIME = 4	
	}
	
	public enum ModifyType{
		NONE = 0,
		SKILL = 1,
		ITEM = 2,
		MECHANIC = 3,
		ATTRIBUTE = 4,
		KNOWLEDGE = 5,
	}
	
	public enum ModifyDuration{
		NONE = 0,
		TEMPORARY = 1,
		PERMANENT = 2,
		LOCATION = 3
	}
	
	public enum CreateType{
		NONE = 0,
		OBJECT = 1,
		CHARACTER = 2,
		ITEM = 3,
		FEATURE = 4
	}
	
	public enum DestoryType{
		NONE = 0,
		OBJECT = 1,
		CHARACTER = 2,
		ITEM = 3,
		FEATURE = 4
	}

	struct ActionPackage{
		public Entity Owner;
		public Entity Target;
		public ActionDef ActionDef;
	}

	public static class ActionUtils
	{
		public static void doAction (Entity owner, Entity target, ActionDef aDef)
		{
			ActionPackage aPack;
			aPack.Owner = owner;
			aPack.Target = target;
			aPack.ActionDef = aDef;

			switch (aPack.ActionDef) {
			case ActionType.DAMAGE:
				doDamageAction(aPack);
				break;
			case ActionType.MODIFY:
				break;
			case ActionType.CREATE:
				break;
			case ActionType.DESTROY:
				break;
			default:
				break;
			}
		}

		public static void doDamageAction(ActionPackage aPack){
			switch (aPack.ActionDef.DamageDef.DamageBasis) {
			case DamageBasis.ATTRIBUTE:
				break;
			case DamageBasis.ITEM:
				break;
			case DamageBasis.KNOWLEDGE:
				break;
			case DamageBasis.NONE:
				break;
			case DamageBasis.SKILL:
				break;
			case DamageBasis.STATIC:

				break;
			default:
				return;
			}
		}

		private static void doModifyAction(ActionPackage aPack){
			switch (aPack.ActionDef.ModifyType) {
			case ModifyType.ATTRIBUTE:
				break;
			case ModifyType.ITEM:
				break;
			case ModifyType.KNOWLEDGE:
				break;
			case ModifyType.MECHANIC:
				break;
			case ModifyType.NONE:
				break;
			case ModifyType.SKILL:
				break;
			default:
				return;
			}
		}

		private static void doCreateAction(ActionPackage aPack){
		}

		private static void doDestroyAction(ActionPackage aPack){
		}

		public static float getStatProbability(int skillValue, int attributeValue, float knowledgeValue, int speedValue){
			return skillValue / 4f + attributeValue / 4f + knowledgeValue + speedValue;
		}

		public static float getHitProbability(float defender, float attacker, float max, float min){
			return (attacker/(defender+attacker))*max + (defender/(defender+attacker))*min;
		}

		public static float getDamage(float overhit, float attackSkill, float attackAttribute,
			                              float lethality, float mitigation, float absorbValue){
				return (overhit + 1f) * (attackSkill/5 + attackAttribute/4) * (lethality/mitigation) - absorbValue/10;
		}
		
	}
}

