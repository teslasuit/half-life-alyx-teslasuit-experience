using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAlyx
{
    public static class HLAlyxConfig
    {
        public static int LowHealthAmount = 33;
        public static int VeryLowHealthAmount = 20;
        public static int TooLowHealthAmount = 10;
        
        public static float GetIntensityMultiplier(HLAlyxFeedbackType feedbackType)
        {
            switch (feedbackType)
            {
                case HLAlyxFeedbackType.DefaultHead: return 1.0f;//DefaultHead; break;
                case HLAlyxFeedbackType.UnarmedHead: return 1.0f;//UnarmedHead; break;
                case HLAlyxFeedbackType.GunHead: return 1.0f;//GunHead; break;
                case HLAlyxFeedbackType.UnarmedBloater: return 1.0f;//UnarmedBloater; break;
                case HLAlyxFeedbackType.UnarmedHeadcrab: return 1.0f;//UnarmedHeadcrab; break;
                case HLAlyxFeedbackType.UnarmedHeadcrabArmored: return 1.0f;//UnarmedHeadcrabArmored; break;
                case HLAlyxFeedbackType.UnarmedHeadcrabBlack: return 1.0f;//UnarmedHeadcrabBlack; break;
                case HLAlyxFeedbackType.UnarmedHeadcrabFast: return 1.0f;//UnarmedHeadcrabFast; break;
                case HLAlyxFeedbackType.UnarmedHeadcrabRunner: return 1.0f;//UnarmedHeadcrabRunner; break;
                case HLAlyxFeedbackType.UnarmedFastZombie: return 1.0f;//UnarmedFastZombie; break;
                case HLAlyxFeedbackType.UnarmedPoisonZombie: return 1.0f;//UnarmedPoisonZombie; break;
                case HLAlyxFeedbackType.UnarmedZombie: return 1.0f;//UnarmedZombie; break;
                case HLAlyxFeedbackType.UnarmedZombieBlind: return 1.0f;//UnarmedZombieBlind; break;
                case HLAlyxFeedbackType.UnarmedZombine: return 1.0f;//UnarmedZombine; break;
                case HLAlyxFeedbackType.UnarmedAntlion: return 1.0f;//UnarmedAntlion; break;
                case HLAlyxFeedbackType.UnarmedAntlionGuard: return 1.0f;//UnarmedAntlionGuard; break;
                case HLAlyxFeedbackType.UnarmedManhack: return 1.0f;//UnarmedManhack; break;
                case HLAlyxFeedbackType.GrabbedByBarnacle: return 1.0f;//GrabbedByBarnacle; break;
                case HLAlyxFeedbackType.ConcussionGrenade: return 1.0f;//ConcussionGrenade; break;
                case HLAlyxFeedbackType.BugBaitGrenade: return 1.0f;//BugBaitGrenade; break;
                case HLAlyxFeedbackType.FragGrenade: return 1.0f;//FragGrenade; break;
                case HLAlyxFeedbackType.SpyGrenade: return 1.0f;//SpyGrenade; break;
                case HLAlyxFeedbackType.HandGrenade: return 1.0f;//HandGrenade; break;
                case HLAlyxFeedbackType.RollerGrenade: return 1.0f;//RollerGrenade; break;
                case HLAlyxFeedbackType.RollerMine: return 1.0f;//RollerMine; break;
                case HLAlyxFeedbackType.Combine: return 1.0f;//Combine; break;
                case HLAlyxFeedbackType.CombineS: return 1.0f;//CombineS; break;
                case HLAlyxFeedbackType.CombineGantry: return 1.0f;//CombineGantry; break;
                case HLAlyxFeedbackType.MetroPolice: return 1.0f;//MetroPolice; break;
                case HLAlyxFeedbackType.Sniper: return 1.0f;//Sniper; break;
                case HLAlyxFeedbackType.Strider: return 1.0f;//Strider; break;
                case HLAlyxFeedbackType.Turret: return 1.0f;//Turret; break;
                case HLAlyxFeedbackType.FoliageTurret: return 1.0f;//FoliageTurret; break;
                case HLAlyxFeedbackType.EnvironmentExplosion: return 1.0f;//EnvironmentExplosion; break;
                case HLAlyxFeedbackType.EnvironmentLaser: return 1.0f;//EnvironmentLaser; break;
                case HLAlyxFeedbackType.EnvironmentFire: return 1.0f;//EnvironmentFire; break;
                case HLAlyxFeedbackType.EnvironmentSpark: return 1.0f;//EnvironmentSpark; break;
                case HLAlyxFeedbackType.EnvironmentPoison: return 1.0f;//EnvironmentPoison; break;
                case HLAlyxFeedbackType.EnvironmentRadiation: return 1.0f;//EnvironmentRadiation; break;
                case HLAlyxFeedbackType.DamageExplosion: return 1.0f;//DamageExplosion; break;
                case HLAlyxFeedbackType.DamageLaser: return 1.0f;//DamageLaser; break;
                case HLAlyxFeedbackType.DamageFire: return 1.0f;//DamageFire; break;
                case HLAlyxFeedbackType.DamageSpark: return 1.0f;//DamageSpark; break;
                case HLAlyxFeedbackType.PlayerShootPistol: return 1.0f;//PlayerShootPistol; break;
                case HLAlyxFeedbackType.PlayerShootShotgun: return 1.0f;//PlayerShootShotgun; break;
                case HLAlyxFeedbackType.PlayerShootSMG: return 1.0f;//PlayerShootSMG; break;
                case HLAlyxFeedbackType.PlayerShootDefault: return 1.0f;//PlayerShootDefault; break;
                case HLAlyxFeedbackType.PlayerGrenadeLaunch: return 1.0f;//PlayerGrenadeLaunch; break;
                case HLAlyxFeedbackType.PlayerShootPistolLeft: return 1.0f;//PlayerShootPistol; break;
                case HLAlyxFeedbackType.PlayerShootShotgunLeft: return 1.0f;//PlayerShootShotgun; break;
                case HLAlyxFeedbackType.PlayerShootSMGLeft: return 1.0f;//PlayerShootSMG; break;
                case HLAlyxFeedbackType.PlayerShootDefaultLeft: return 1.0f;//PlayerShootDefault; break;
                case HLAlyxFeedbackType.PlayerGrenadeLaunchLeft: return 1.0f;//PlayerGrenadeLaunch; break;
                case HLAlyxFeedbackType.FallbackPistol: return 1.0f;//FallbackPistol; break;
                case HLAlyxFeedbackType.FallbackShotgun: return 1.0f;//FallbackShotgun; break;
                case HLAlyxFeedbackType.FallbackSMG: return 1.0f;//FallbackSMG; break;
                case HLAlyxFeedbackType.FallbackPistolLeft: return 1.0f;//FallbackPistol; break;
                case HLAlyxFeedbackType.FallbackShotgunLeft: return 1.0f;//FallbackShotgun; break;
                case HLAlyxFeedbackType.FallbackSMGLeft: return 1.0f;//FallbackSMG; break;
                case HLAlyxFeedbackType.KickbackPistol: return 1.0f;//KickbackPistol; break;
                case HLAlyxFeedbackType.KickbackShotgun: return 1.0f;//KickbackShotgun; break;
                case HLAlyxFeedbackType.KickbackSMG: return 1.0f;//KickbackSMG; break;
                case HLAlyxFeedbackType.KickbackPistolLeft: return 1.0f;//KickbackPistol; break;
                case HLAlyxFeedbackType.KickbackShotgunLeft: return 1.0f;//KickbackShotgun; break;
                case HLAlyxFeedbackType.KickbackSMGLeft: return 1.0f;//KickbackSMG; break;
                case HLAlyxFeedbackType.HeartBeat: return 1.0f;//HeartBeat; break;
                case HLAlyxFeedbackType.HeartBeatFast: return 1.0f;//HeartBeatFast; break;
                case HLAlyxFeedbackType.HealthPenUse: return 1.0f;//HealthPenUse; break;
                case HLAlyxFeedbackType.HealthStationUse: return 1.0f;//HealthStationUse; break;
                case HLAlyxFeedbackType.BackpackStoreClip: return 1.0f;//BackpackStoreClip; break;
                case HLAlyxFeedbackType.BackpackStoreResin: return 1.0f;//BackpackStoreResin; break;
                case HLAlyxFeedbackType.BackpackRetrieveClip: return 1.0f;//BackpackRetrieveClip; break;
                case HLAlyxFeedbackType.BackpackRetrieveResin: return 1.0f;//BackpackRetrieveResin; break;
                case HLAlyxFeedbackType.ItemHolderStore: return 1.0f;//ItemHolderStore; break;
                case HLAlyxFeedbackType.ItemHolderRemove: return 1.0f;//ItemHolderRemove; break;
                case HLAlyxFeedbackType.BackpackStoreClipLeft: return 1.0f;//BackpackStoreClip; break;
                case HLAlyxFeedbackType.BackpackStoreResinLeft: return 1.0f;//BackpackStoreResin; break;
                case HLAlyxFeedbackType.BackpackRetrieveClipLeft: return 1.0f;//BackpackRetrieveClip; break;
                case HLAlyxFeedbackType.BackpackRetrieveResinLeft: return 1.0f;//BackpackRetrieveResin; break;
                case HLAlyxFeedbackType.ItemHolderStoreLeft: return 1.0f;//ItemHolderStore; break;
                case HLAlyxFeedbackType.ItemHolderRemoveLeft: return 1.0f;//ItemHolderRemove; break;
                case HLAlyxFeedbackType.GravityGloveLockOn: return 1.0f;//GravityGloveLockOn; break;
                case HLAlyxFeedbackType.GravityGlovePull: return 1.0f;//GravityGlovePull; break;
                case HLAlyxFeedbackType.GravityGloveCatch: return 1.0f;//GravityGloveCatch; break;
                case HLAlyxFeedbackType.GravityGloveLockOnLeft: return 1.0f;//GravityGloveLockOn; break;
                case HLAlyxFeedbackType.GravityGlovePullLeft: return 1.0f;//GravityGlovePull; break;
                case HLAlyxFeedbackType.GravityGloveCatchLeft: return 1.0f;//GravityGloveCatch; break;

                case HLAlyxFeedbackType.ClipInserted: return 1.0f;//ClipInserted; break;
                case HLAlyxFeedbackType.ClipInsertedLeft: return 1.0f;//ClipInserted; break;
                case HLAlyxFeedbackType.ChamberedRound: return 1.0f;//ChamberedRound; break;
                case HLAlyxFeedbackType.ChamberedRoundLeft: return 1.0f;//ChamberedRound; break;

                case HLAlyxFeedbackType.Cough: return 1.0f;//Cough; break;
                case HLAlyxFeedbackType.CoughHead: return 1.0f;//CoughHead; break;

                case HLAlyxFeedbackType.ShockOnHandLeft: return 1.0f;//ShockOnHand; break;
                case HLAlyxFeedbackType.ShockOnHandRight: return 1.0f;//ShockOnHand; break;


                case HLAlyxFeedbackType.DefaultDamage: return 1.0f;//Default; break;
            }

            return 1.0f;//Default;
        }
    }
}
