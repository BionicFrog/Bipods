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
	public class BipedProps : CompProperties
	{

		public JobDef setup;
		public int bipodBurst;
		public int bipodAuto;
		

		public BipedProps()
		{
			this.compClass = typeof(Biped2);
		}

		public BipedProps(Type compClass) : base(compClass)
		{
			this.compClass = compClass;
		}

	}
	public class Biped2 : CompRangedGizmoGiver
	{






		public JobDef yalla => Props.setup;

		public BipedProps Props => (BipedProps)this.props;
		public bool BipodSetUp;
		public bool shoe;
		public bool ShouldSetUpBipod;
		public bool ShouldSetUpBipodGizmoBool = false;
		public float DaTime = 2f;
		public Thing Myself
		{
			get
			{
				return this.parent;
			}
		}
		public Pawn daPawn
		{
			get
			{
				return this.CompEquippable.PrimaryVerb.CasterPawn;
			}
		}
		public override void CompTick()
		{
			if (pawnHolding != null)
			{
				if (pawnHolding.Faction.def != FactionDefOf.PlayerColony)
				{
					if (pawnHolding.Faction.def != FactionDefOf.PlayerTribe)
					{
						ShouldSetUpBipodGizmoBool = true;
					}
				}
				if (!shoe)
				{
					shoe = true;

				}
			}
			
		}
		public override void PostDeSpawn(Map map)
		{
			Log.Error("Omaba");
			ShouldSetUpBipodGizmoBool = false;
			shoe = false;
		}
		public CompEquippable CompEquippable
		{
			get
			{
				return this.parent.GetComp<CompEquippable>();
			}
		}
		public bool SetUp
		{
			get
			{
				return false;
			}
		}
		public float rangechange;
		public override void Notify_Equipped(Pawn pawn)
		{

			Biped bipedo = CompEquippable.PrimaryVerb as Biped;
			rangechange = bipedo.VerbPropsCE.range;
			shoe = false;
		}
		//public Biped daPawnR
		//{
		//get
		//{
		//return this.CompEquippableR.PrimaryVerb as Biped;
		//}
		//}
	
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return new GizmoBipodSetupManual(this)
			{
				
			};
			
			yield break;
		}
		public Pawn_InventoryTracker pwaninve;
		public Pawn pawnHolding;
		public void GizmoStuff()
		{
			pawnHolding = pwaninve.pawn;
			pwaninve = this.ParentHolder as Pawn_InventoryTracker;
			ThinkNode jobGiver = null;
			Pawn_JobTracker jobs = pawnHolding.jobs;
			Job job = this.TryMakeBipodJobComp();
			Job newJob = job;
			JobCondition lastJobEndCondition = JobCondition.InterruptForced;
			Job curJob = pawnHolding.CurJob;
			jobs.StartJob(newJob, lastJobEndCondition, jobGiver, ((curJob != null) ? curJob.def : null) != job.def, true, null, null, false, false);
		}
		public Job TryMakeBipodJobComp()
		{
			return new Job(Myself.TryGetComp<Biped2>().yalla, pawnHolding);
		}
	}
	public class Biped : Verb_LaunchProjectileCE
	{

		protected override int ShotsPerBurst
		{
			get
			{
				bool flag = base.CompFireModes != null;
				if (flag)
				{
					bool flag2 = base.CompFireModes.CurrentFireMode == FireMode.SingleFire;
					if (flag2)
					{
						return 1;
					}
					bool flag3 = base.CompFireModes.CurrentFireMode == FireMode.BurstFire && base.CompFireModes.Props.aimedBurstShotCount > 0;
					if (flag3)
					{
						if (BipodComp.BipodSetUp)
						{
							if (BipodComp.Props.bipodBurst != 0)
						
							{ return base.VerbPropsCE.burstShotCount + BipodComp.Props.bipodBurst; }
							else
							{
								return base.VerbPropsCE.burstShotCount;
							}

						}
						else
						{
							return base.CompFireModes.Props.aimedBurstShotCount;
						}
						
					}
				}
				if (BipodComp.BipodSetUp)
				{
					if (BipodComp.Props.bipodAuto != 0)
					{
						return BipodComp.Props.bipodAuto + base.VerbPropsCE.burstShotCount;
					}
					else
					{
						return base.VerbPropsCE.burstShotCount;
					}

				}
				else
				{
					return base.VerbPropsCE.burstShotCount;
				}
				
			}
		}
		public Biped2 BipodComp
		{
			get
			{
				return this.EquipmentSource.TryGetComp<Biped2>();
			}
			
		}
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000DA64 File Offset: 0x0000BC64
		private bool ShouldAim
		{
			get
			{
				bool flag = base.CompFireModes != null;
				bool result;
				if (flag)
				{
					bool flag2 = base.ShooterPawn != null;
					if (flag2)
					{
						bool flag3 = base.ShooterPawn.CurJob != null && base.ShooterPawn.CurJob.def == JobDefOf.Hunt;
						if (flag3)
						{
							return true;
						}
						bool isSuppressed = this.IsSuppressed;
						if (isSuppressed)
						{
							return false;
						}
						Pawn_PathFollower pather = base.ShooterPawn.pather;
						bool flag4 = pather != null && pather.Moving;
						if (flag4)
						{
							return false;
						}
					}
					result = (base.CompFireModes.CurrentAimMode == AimMode.AimedShot);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000DB0C File Offset: 0x0000BD0C
		protected override float SwayAmplitude
		{
			get
			{
				float swayAmplitude = base.SwayAmplitude;
				bool shouldAim = this.ShouldAim;
				float result;
				if (shouldAim)
				{
					result = swayAmplitude * Mathf.Max(0f, 1f - base.AimingAccuracy) / Mathf.Max(1f, base.SightsEfficiency);
				}
				else
				{
					bool isSuppressed = this.IsSuppressed;
					if (isSuppressed)
					{
						result = swayAmplitude * 1.5f;
					}
					else
					{
						result = swayAmplitude;
					}
				}
				return result;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600011E RID: 286 RVA: 0x0000DB74 File Offset: 0x0000BD74
		private bool IsSuppressed
		{
			get
			{
				Pawn shooterPawn = base.ShooterPawn;
				bool? flag;
				if (shooterPawn == null)
				{
					flag = null;
				}
				else
				{
					CompSuppressable compSuppressable = shooterPawn.TryGetComp<CompSuppressable>();
					flag = ((compSuppressable != null) ? new bool?(compSuppressable.isSuppressed) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000DBBC File Offset: 0x0000BDBC
		public override void WarmupComplete()
		{
			Log.Error(ShotsPerBurst.ToString());
			float lengthHorizontal = (this.currentTarget.Cell - this.caster.Position).LengthHorizontal;
			int num = (int)Mathf.Lerp(30f, 240f, lengthHorizontal / 100f);
			bool flag = this.ShouldAim && !this._isAiming;
			if (flag)
			{
				Building_TurretGunCE building_TurretGunCE = this.caster as Building_TurretGunCE;
				bool flag2 = building_TurretGunCE != null;
				if (flag2)
				{
					building_TurretGunCE.burstWarmupTicksLeft += num;
					this._isAiming = true;
					return;
				}
				bool flag3 = base.ShooterPawn != null;
				if (flag3)
				{
					base.ShooterPawn.stances.SetStance(new Stance_Warmup(num, this.currentTarget, this));
					this._isAiming = true;
					return;
				}
			}
			base.WarmupComplete();
			this._isAiming = false;
			Pawn shooterPawn = base.ShooterPawn;
			bool flag4 = ((shooterPawn != null) ? shooterPawn.skills : null) != null && this.currentTarget.Thing is Pawn;
			if (flag4)
			{
				float num2 = this.verbProps.AdjustedFullCycleTime(this, base.ShooterPawn);
				num2 += num.TicksToSeconds();
				float num3 = this.currentTarget.Thing.HostileTo(base.ShooterPawn) ? 170f : 20f;
				num3 *= num2;
				base.ShooterPawn.skills.Learn(SkillDefOf.Shooting, num3, false);
			}
		}

		public Biped2 bidopcomp
		{
			get
			{
				return Myself.TryGetComp<Biped2>();
			}
		}
		
	public override void VerbTickCE()
		{
			
			
			if (!Myself.TryGetComp<Biped2>().shoe)
			{
				if (Myself.ParentHolder != Myself.Map)
				{


					if (CasterPawn != null)
					{


						if (daPawn != null)
						{
							if (daPawn.pather.Moving)
							{
								Myself.TryGetComp<Biped2>().BipodSetUp = false;
							}

							if (daPawn.Drafted)
							{
								if (Myself.TryGetComp<Biped2>().ShouldSetUpBipodGizmoBool) 
								{
									if (!daPawn.pather.Moving)
									{
										if (!daPawn.pather.MovingNow)
										{
											ThinkNode jobGiver = null;
											Pawn_JobTracker jobs = this.CasterPawn.jobs;
											Job job = this.TryMakeBipodJob();
											Job newJob = job;
											JobCondition lastJobEndCondition = JobCondition.InterruptForced;
											Job curJob = this.CasterPawn.CurJob;
											if (jobs.curJob != job)
											{
												if (Myself.TryGetComp<Biped2>().BipodSetUp != true)
												{
													jobs.StartJob(newJob, lastJobEndCondition, jobGiver, ((curJob != null) ? curJob.def : null) != job.def, true, null, null, false, false);
												}

											}


										}


									}
								}
								

							}
						}

					}
				}
			}




			bool isAiming = this._isAiming;
			if (isAiming)
			{
				bool flag = !this.ShouldAim;
				if (flag)
				{
					this.WarmupComplete();
				}
				bool flag2;
				if (!(this.caster is Building_TurretGunCE))
				{
					Pawn shooterPawn = base.ShooterPawn;
					Type left;
					if (shooterPawn == null)
					{
						left = null;
					}
					else
					{
						Pawn_StanceTracker stances = shooterPawn.stances;
						if (stances == null)
						{
							left = null;
						}
						else
						{
							Stance curStance = stances.curStance;
							left = ((curStance != null) ? curStance.GetType() : null);
						}
					}
					flag2 = (left != typeof(Stance_Warmup));
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					this._isAiming = false;
				}
			}
		}

		
		public override void Notify_EquipmentLost()
		{
			base.Notify_EquipmentLost();
			bool flag = base.CompFireModes != null;
			if (flag)
			{
				base.CompFireModes.ResetModes();
			}
		}

		
		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			bool flag = base.ShooterPawn != null && !base.ShooterPawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight);
			return !flag && base.CanHitTargetFrom(root, targ);
		}
		public float NeededFloat;


		public int yes;
		protected override bool TryCastShot()
		{
			
			if (yes != 1)
			{
				NeededFloat = this.VerbPropsCE.warmupTime;
				yes = 1;
			}
			if (Myself.TryGetComp<Biped2>() != null)
			{
				if (Myself.TryGetComp<Biped2>().BipodSetUp)
				{
					Log.Error(NeededFloat.ToString());
					this.VerbPropsCE.warmupTime = NeededFloat / 2;
					Log.Error(this.VerbPropsCE.warmupTime.ToString());
				}
				else
				{
					this.VerbPropsCE.warmupTime = NeededFloat;
					Log.Error(this.VerbPropsCE.warmupTime.ToString());
				}
			}
			else
			{
				Log.Error("missing comp");
			}
			bool flag = base.CompAmmo != null;
			if (flag)
			{
				bool flag2 = !base.CompAmmo.TryReduceAmmoCount(base.VerbPropsCE.ammoConsumedPerShotCount);
				if (flag2)
				{
					return false;
				}
			}
			bool flag3 = base.TryCastShot();
			bool result;
			if (flag3)
			{
				bool flag4 = base.ShooterPawn != null;
				if (flag4)
				{
					base.ShooterPawn.records.Increment(RecordDefOf.ShotsFired);
				}
				bool flag5 = base.VerbPropsCE.ejectsCasings && base.projectilePropsCE.dropsCasings;
				
				bool flag6 = base.CompAmmo != null && !base.CompAmmo.HasMagazine && base.CompAmmo.UseAmmo;
				if (flag6)
				{
					bool flag7 = !base.CompAmmo.Notify_ShotFired();
					if (flag7)
					{
						bool flag8 = base.VerbPropsCE.muzzleFlashScale > 0.01f;
						if (flag8)
						{
							MoteMaker.MakeStaticMote(this.caster.Position, this.caster.Map, ThingDefOf.Mote_ShotFlash, base.VerbPropsCE.muzzleFlashScale);
						}
						bool flag9 = base.VerbPropsCE.soundCast != null;
						if (flag9)
						{
							base.VerbPropsCE.soundCast.PlayOneShot(new TargetInfo(this.caster.Position, this.caster.Map, false));
						}
						bool flag10 = base.VerbPropsCE.soundCastTail != null;
						if (flag10)
						{
							base.VerbPropsCE.soundCastTail.PlayOneShotOnCamera(null);
						}
						bool flag11 = base.ShooterPawn != null;
						if (flag11)
						{
							bool flag12 = base.ShooterPawn.thinker != null;
							if (flag12)
							{
								base.ShooterPawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
							}
						}
					}
					result = base.CompAmmo.Notify_PostShotFired();
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x040000F5 RID: 245
		private const int AimTicksMin = 30;

		// Token: 0x040000F6 RID: 246
		private const int AimTicksMax = 240;

		// Token: 0x040000F7 RID: 247
		private const float PawnXp = 20f;

		// Token: 0x040000F8 RID: 248
		private const float HostileXp = 170f;

		// Token: 0x040000F9 RID: 249
		private const float SuppressionSwayFactor = 1.5f;

		// Token: 0x040000FA RID: 250
		private bool _isAiming;
	





		
		
		
		public Job TryMakeBipodJob()
		{
			return new Job(Myself.TryGetComp<Biped2>().yalla, Myself);
		}
		public Thing Myself
		{
			get
			{
				return this.EquipmentSource;
			}
		}
		public Pawn daPawn
		{
			get
			{
				return this.CompEquippable.PrimaryVerb.CasterPawn;
			}
		}
		public CompEquippable CompEquippable
		{
			get
			{
				return this.EquipmentSource.GetComp<CompEquippable>();
			}
		}
		
		
		



	}

	
	public class SetUpBipod : JobDriver
	{
		private ThingWithComps weapon
		{
			get
			{
				return base.TargetThingA as ThingWithComps;
			}
		}

		private const TargetIndex guntosetup = TargetIndex.A;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			//return true;	
			return this.pawn.Reserve(this.job.GetTarget(guntosetup), this.job, 1, -1, null);
		}
		public Biped2 Bipod
		{
			get
			{
				return this.weapon.TryGetComp<Biped2>();
			}
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = Toils_General.Wait(240);
			if (Bipod == null)
			{
				yield return null;
			}

			toil.AddFinishAction(delegate
			{
				Bipod.BipodSetUp = true;
				
			});
			yield return toil;

			
		}
	}
}
