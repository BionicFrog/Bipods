using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CombatExtended;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using UnityEngine;

namespace Mgaazines
{
	[StaticConstructorOnStartup]
	public class GizmoBipodSetupManual : Command
	{
		public GizmoBipodSetupManual(Biped2 BipodComp)
		{
			this.bipod = BipodComp;
			bool flag = this.bipod.BipodSetUp;
			if (flag)
			{
				this.defaultLabel = this.labelStart;
				this.defaultDesc = this.descriptionStart;
				this.icon = GizmoBipodSetupManual.startIcon;
			}
			else
			{
				
					this.defaultLabel = this.labelStop.Translate();
					this.defaultDesc = this.descriptionStop.Translate();
					this.icon = GizmoBipodSetupManual.stopIcon;
				
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			bool flag = this.bipod.ShouldSetUpBipodGizmoBool;
			if (flag)
			{
				this.bipod.ShouldSetUpBipodGizmoBool = false;
			}
			else
			{
				this.bipod.ShouldSetUpBipodGizmoBool = true;
				this.bipod.shoe = false;
				this.bipod.GizmoStuff();
			}
		}

		
		public static Texture2D stopIcon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);

		
		public static Texture2D startIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true);

		
		

		public Biped2 bipod;

		
		public string labelStart = "Set up bipod";

		
		public string descriptionStart = "Placeholder";

		
		public string labelStop = "obama";

	
		public string descriptionStop = "obama";
	}
	public class GizmoSetUpManually : Verb
	{
		protected override bool TryCastShot()
		{
			SetUpManually(this.EquipmentSource.TryGetComp<Biped2>());
			return true;
		}
		public static void SetUpManually(Biped2 BipodComp)
		{
			if (BipodComp == null)
			{
				Log.Error("Missing Bipod Comp");
				return;
			}

		}
	}
}
	

