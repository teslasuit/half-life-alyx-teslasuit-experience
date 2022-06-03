using System;
using System.Collections.Generic;

namespace TeslasuitAlyx
{
    public partial class HLAlyxFeedbackEventProvider
    {
        public event Action<HLAlyxFeedbackEventArgs> OnFeedbackStart = delegate { };
        public event Action<HLAlyxFeedbackEventArgs> OnFeedbackStop = delegate { };

        private static List<HLAlyxFeedbackType> FEEDBACK_ENVIRONMENT = new List<HLAlyxFeedbackType>
        {
            HLAlyxFeedbackType.DefaultDamage,
            HLAlyxFeedbackType.EnvironmentExplosion,
            HLAlyxFeedbackType.EnvironmentFire,
            HLAlyxFeedbackType.EnvironmentLaser,
            HLAlyxFeedbackType.EnvironmentSpark,
            HLAlyxFeedbackType.EnvironmentPoison,
            HLAlyxFeedbackType.EnvironmentRadiation
        };

        private static List<HLAlyxFeedbackType> FEEDBACK_HEADCRAB = new List<HLAlyxFeedbackType>
        {
            HLAlyxFeedbackType.UnarmedHeadcrab,
            HLAlyxFeedbackType.UnarmedHeadcrabArmored,
            HLAlyxFeedbackType.UnarmedHeadcrabBlack,
            HLAlyxFeedbackType.UnarmedHeadcrabFast,
            HLAlyxFeedbackType.UnarmedHeadcrabRunner
        };

        private static List<HLAlyxFeedbackType> FEEDBACK_UNARMED_HEAD = new List<HLAlyxFeedbackType>
        {
            HLAlyxFeedbackType.UnarmedHeadcrab,
            HLAlyxFeedbackType.UnarmedHeadcrabArmored,
            HLAlyxFeedbackType.UnarmedHeadcrabBlack,
            HLAlyxFeedbackType.UnarmedHeadcrabFast,
            HLAlyxFeedbackType.UnarmedHeadcrabRunner,
            HLAlyxFeedbackType.UnarmedAntlion,
            HLAlyxFeedbackType.UnarmedAntlionGuard,
            HLAlyxFeedbackType.UnarmedFastZombie,
            HLAlyxFeedbackType.UnarmedPoisonZombie,
            HLAlyxFeedbackType.UnarmedManhack,
            HLAlyxFeedbackType.UnarmedZombie,
            HLAlyxFeedbackType.UnarmedZombieBlind,
            HLAlyxFeedbackType.UnarmedZombine
        };

        private static List<HLAlyxFeedbackType> FEEDBACK_GUN_HEAD = new List<HLAlyxFeedbackType>
        {
            HLAlyxFeedbackType.Combine,
            HLAlyxFeedbackType.CombineS,
            HLAlyxFeedbackType.CombineGantry,
            HLAlyxFeedbackType.MetroPolice,
            HLAlyxFeedbackType.FoliageTurret,
            HLAlyxFeedbackType.Turret,
            HLAlyxFeedbackType.Sniper,
            HLAlyxFeedbackType.Strider
        };

        public bool HeadCrabFeedback(HLAlyxFeedbackType feedback)
        {
            return FEEDBACK_HEADCRAB.Contains(feedback);
        }

        public bool EnvironmentFeedback(HLAlyxFeedbackType feedback)
        {
            return FEEDBACK_ENVIRONMENT.Contains(feedback);
        }

        public HLAlyxFeedbackType GetHeadFeedbackVersion(HLAlyxFeedbackType feedback)
        {
            if (FEEDBACK_UNARMED_HEAD.Contains(feedback))
            {
                return HLAlyxFeedbackType.UnarmedHead;
            }
            else if (FEEDBACK_GUN_HEAD.Contains(feedback))
            {
                return HLAlyxFeedbackType.GunHead;
            }
            else
            {
                return HLAlyxFeedbackType.NoFeedback;
            }
        }

        public HLAlyxFeedbackType GetOtherHandFeedback(HLAlyxFeedbackType feedback)
        {
            switch (feedback)
            {
                case HLAlyxFeedbackType.PlayerShootPistol:
                    return HLAlyxFeedbackType.PlayerShootPistolLeft;
                case HLAlyxFeedbackType.PlayerShootShotgun:
                    return HLAlyxFeedbackType.PlayerShootShotgunLeft;
                case HLAlyxFeedbackType.PlayerShootSMG:
                    return HLAlyxFeedbackType.PlayerShootSMGLeft;
                case HLAlyxFeedbackType.PlayerShootPistolLeft:
                    return HLAlyxFeedbackType.PlayerShootPistol;
                case HLAlyxFeedbackType.PlayerShootShotgunLeft:
                    return HLAlyxFeedbackType.PlayerShootShotgun;
                case HLAlyxFeedbackType.PlayerShootSMGLeft:
                    return HLAlyxFeedbackType.PlayerShootSMG;
                case HLAlyxFeedbackType.PlayerShootDefault:
                    return HLAlyxFeedbackType.PlayerShootDefaultLeft;
                case HLAlyxFeedbackType.PlayerShootDefaultLeft:
                    return HLAlyxFeedbackType.PlayerShootDefault;
                default:
                    return HLAlyxFeedbackType.NoFeedback;
            }
        }

        public HLAlyxFeedbackType GetFallbackTypeOfWeaponFromPlayer(HLAlyxFeedbackType feedback, bool leftHanded)
        {
            switch (feedback)
            {
                case HLAlyxFeedbackType.PlayerShootPistol:
                    return leftHanded ? HLAlyxFeedbackType.FallbackPistolLeft : HLAlyxFeedbackType.FallbackPistol;
                case HLAlyxFeedbackType.PlayerShootShotgun:
                    return leftHanded ? HLAlyxFeedbackType.FallbackShotgunLeft : HLAlyxFeedbackType.FallbackShotgun;
                case HLAlyxFeedbackType.PlayerShootSMG:
                    return leftHanded ? HLAlyxFeedbackType.FallbackSMGLeft : HLAlyxFeedbackType.FallbackSMG;
                case HLAlyxFeedbackType.PlayerShootPistolLeft:
                    return leftHanded ? HLAlyxFeedbackType.FallbackPistolLeft : HLAlyxFeedbackType.FallbackPistol;
                case HLAlyxFeedbackType.PlayerShootShotgunLeft:
                    return leftHanded ? HLAlyxFeedbackType.FallbackShotgunLeft : HLAlyxFeedbackType.FallbackShotgun;
                case HLAlyxFeedbackType.PlayerShootSMGLeft:
                    return leftHanded ? HLAlyxFeedbackType.FallbackSMGLeft : HLAlyxFeedbackType.FallbackSMG;
                case HLAlyxFeedbackType.PlayerShootDefault:
                    return leftHanded ? HLAlyxFeedbackType.FallbackPistolLeft : HLAlyxFeedbackType.FallbackPistol;
                case HLAlyxFeedbackType.PlayerShootDefaultLeft:
                    return leftHanded ? HLAlyxFeedbackType.FallbackPistolLeft : HLAlyxFeedbackType.FallbackPistol;
                default:
                    return HLAlyxFeedbackType.NoFeedback;
            }
        }

        public HLAlyxFeedbackType GetKickbackOfWeaponFromPlayer(HLAlyxFeedbackType feedback, bool leftHanded)
        {
            switch (feedback)
            {
                case HLAlyxFeedbackType.PlayerShootPistol:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                case HLAlyxFeedbackType.PlayerShootShotgun:
                    return leftHanded ? HLAlyxFeedbackType.KickbackShotgunLeft : HLAlyxFeedbackType.KickbackShotgun;
                case HLAlyxFeedbackType.PlayerShootSMG:
                    return leftHanded ? HLAlyxFeedbackType.KickbackSMGLeft : HLAlyxFeedbackType.KickbackSMG;
                case HLAlyxFeedbackType.PlayerGrenadeLaunch:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                case HLAlyxFeedbackType.PlayerShootDefault:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                case HLAlyxFeedbackType.PlayerShootPistolLeft:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                case HLAlyxFeedbackType.PlayerShootShotgunLeft:
                    return leftHanded ? HLAlyxFeedbackType.KickbackShotgunLeft : HLAlyxFeedbackType.KickbackShotgun;
                case HLAlyxFeedbackType.PlayerShootSMGLeft:
                    return leftHanded ? HLAlyxFeedbackType.KickbackSMGLeft : HLAlyxFeedbackType.KickbackSMG;
                case HLAlyxFeedbackType.PlayerGrenadeLaunchLeft:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                case HLAlyxFeedbackType.PlayerShootDefaultLeft:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
                default:
                    return leftHanded ? HLAlyxFeedbackType.KickbackPistolLeft : HLAlyxFeedbackType.KickbackPistol;
            }
        }

        public HLAlyxFeedbackType GetFeedbackTypeOfWeaponFromPlayer(string weapon, bool leftHanded)
        {
            switch (weapon)
            {
                case "hlvr_weapon_crowbar":
                case "hlvr_weapon_crowbar_physics":
                case "hlvr_weapon_energygun":
                    return leftHanded ? HLAlyxFeedbackType.PlayerShootPistolLeft : HLAlyxFeedbackType.PlayerShootPistol;
                case "hlvr_weapon_shotgun":
                    return leftHanded ? HLAlyxFeedbackType.PlayerShootShotgunLeft : HLAlyxFeedbackType.PlayerShootShotgun;
                case "hlvr_weapon_rapidfire":
                case "hlvr_weapon_rapidfire_ammo_capsule":
                case "hlvr_weapon_rapidfire_bullets_manager":
                case "hlvr_weapon_rapidfire_energy_ball":
                case "hlvr_weapon_rapidfire_extended_magazine":
                case "hlvr_weapon_rapidfire_tag_dart":
                case "hlvr_weapon_rapidfire_tag_marker":
                case "hlvr_weapon_rapidfire_upgrade_model":
                    return leftHanded ? HLAlyxFeedbackType.PlayerShootSMGLeft : HLAlyxFeedbackType.PlayerShootSMG;
                default:
                    return leftHanded ? HLAlyxFeedbackType.PlayerShootDefaultLeft : HLAlyxFeedbackType.PlayerShootDefault;
            }
        }

        public HLAlyxFeedbackType GetFeedbackTypeOfEnemyAttack(string enemy, string enemyName)
        {
            if (enemy == "npc_combine_s" && enemyName.Contains("gantry"))
            {
                return HLAlyxFeedbackType.CombineGantry;
            }

            if (enemy.Contains("grenade") || enemy.Contains("mine"))
            {
                if (enemy.Contains("concussion"))
                {
                    return HLAlyxFeedbackType.ConcussionGrenade;
                }
                else if (enemy.Contains("hand") || enemy.Contains("xen"))
                {
                    return HLAlyxFeedbackType.HandGrenade;
                }
                else if (enemy.Contains("bugbait"))
                {
                    return HLAlyxFeedbackType.BugBaitGrenade;
                }
                else if (enemy.Contains("frag"))
                {
                    return HLAlyxFeedbackType.FragGrenade;
                }
                else if (enemy.Contains("spy"))
                {
                    return HLAlyxFeedbackType.SpyGrenade;
                }
                else if (enemy.Contains("rollergrenade"))
                {
                    return HLAlyxFeedbackType.RollerGrenade;
                }
                else if (enemy.Contains("rollermine"))
                {
                    return HLAlyxFeedbackType.RollerMine;
                }
                else if (enemy.Contains("hand"))
                {
                    return HLAlyxFeedbackType.HandGrenade;
                }
                else
                {
                    return HLAlyxFeedbackType.FragGrenade;
                }
            }

            switch (enemy)
            {
                case "npc_headcrab":
                    return HLAlyxFeedbackType.UnarmedHeadcrab;
                case "npc_headcrab_armored":
                    return HLAlyxFeedbackType.UnarmedHeadcrabArmored;
                case "npc_headcrab_black":
                    return HLAlyxFeedbackType.UnarmedHeadcrabBlack;
                case "npc_headcrab_fast":
                    return HLAlyxFeedbackType.UnarmedHeadcrabFast;
                case "npc_headcrab_runner":
                    return HLAlyxFeedbackType.UnarmedHeadcrabRunner;
                case "npc_fastzombie":
                    return HLAlyxFeedbackType.UnarmedFastZombie;
                case "npc_poisonzombie":
                    return HLAlyxFeedbackType.UnarmedPoisonZombie;
                case "npc_zombie":
                    return HLAlyxFeedbackType.UnarmedZombie;
                case "npc_zombie_blind":
                    return HLAlyxFeedbackType.UnarmedZombieBlind;
                case "npc_zombine":
                    return HLAlyxFeedbackType.UnarmedZombine;
                case "npc_manhack":
                    return HLAlyxFeedbackType.UnarmedManhack;
                case "npc_antlion":
                    return HLAlyxFeedbackType.UnarmedAntlion;
                case "npc_antlionguard":
                case "npc_barnacle":
                case "npc_barnacle_tongue_tip":
                    return HLAlyxFeedbackType.UnarmedAntlionGuard;
                case "xen_foliage_bloater":
                    return HLAlyxFeedbackType.UnarmedBloater;
                case "env_explosion":
                    return HLAlyxFeedbackType.DamageExplosion;
                case "env_fire":
                    return HLAlyxFeedbackType.DamageFire;
                case "env_laser":
                    return HLAlyxFeedbackType.DamageLaser;
                case "env_physexplosion":
                    return HLAlyxFeedbackType.DamageExplosion;
                case "env_physimpact":
                    return HLAlyxFeedbackType.DamageExplosion;
                case "env_spark":
                    return HLAlyxFeedbackType.DamageSpark;
                case "npc_combine": return HLAlyxFeedbackType.Combine;
                case "npc_combine_s": return HLAlyxFeedbackType.CombineS;
                case "npc_metropolice": return HLAlyxFeedbackType.MetroPolice;
                case "npc_sniper": return HLAlyxFeedbackType.Sniper;
                case "npc_strider": return HLAlyxFeedbackType.Strider;
                case "npc_hunter": return HLAlyxFeedbackType.DamageExplosion;
                case "npc_hunter_invincible": return HLAlyxFeedbackType.DamageExplosion;
                case "npc_turret_ceiling": return HLAlyxFeedbackType.Turret;
                case "npc_turret_ceiling_pulse": return HLAlyxFeedbackType.Turret;
                case "npc_turret_citizen": return HLAlyxFeedbackType.Turret;
                case "npc_turret_floor": return HLAlyxFeedbackType.Turret;
                case "xen_foliage_turret": return HLAlyxFeedbackType.FoliageTurret;
                case "xen_foliage_turret_projectile": return HLAlyxFeedbackType.FoliageTurret;
            }

            if (enemy == "prop_physics" && enemyName.Contains("grenade"))
            {
                if (enemyName.Contains("concussion"))
                    return HLAlyxFeedbackType.ConcussionGrenade;
                else if (enemyName.Contains("hand") || enemyName.Contains("xen"))
                    return HLAlyxFeedbackType.HandGrenade;
                else if (enemyName.Contains("bugbait"))
                    return HLAlyxFeedbackType.BugBaitGrenade;
                else if (enemyName.Contains("frag"))
                    return HLAlyxFeedbackType.FragGrenade;
                else if (enemyName.Contains("spy"))
                    return HLAlyxFeedbackType.SpyGrenade;
                else if (enemyName.Contains("roller"))
                    return HLAlyxFeedbackType.RollerGrenade;
            }
            else if (enemy == "prop_physics" && enemyName.Contains("mine"))
            {
                return HLAlyxFeedbackType.RollerMine;
            }

            return HLAlyxFeedbackType.DefaultDamage;
        }

        private void FireFeedbackEvent(float locationAngle, float locationHeight, HLAlyxFeedbackType effect, float intensityMultiplier, bool overwrite)
        {
            if (intensityMultiplier < 0.01 || effect == HLAlyxFeedbackType.NoFeedback)
                return;

            if (locationHeight < -0.5f)
                locationHeight = -0.5f;
            else if (locationHeight > 0.5f)
                locationHeight = 0.5f;

            OnFeedbackStart(new HLAlyxFeedbackEventArgs(effect, locationAngle, locationHeight, intensityMultiplier, overwrite));
        }

        public void FireHapticFeedbackStart(float locationAngle, float locationHeight, HLAlyxFeedbackType primaryEffect, bool overwrite, HLAlyxFeedbackType secondaryEffect = HLAlyxFeedbackType.NoFeedback)
        {
            FireFeedbackEvent(locationAngle, locationHeight, primaryEffect, HLAlyxConfig.GetIntensityMultiplier(primaryEffect), overwrite);
            FireFeedbackEvent(locationAngle, locationHeight, secondaryEffect, HLAlyxConfig.GetIntensityMultiplier(secondaryEffect), overwrite);
        }

        public void FireHapticFeedbackStop(HLAlyxFeedbackType effect)
        {
            OnFeedbackStop(new HLAlyxFeedbackEventArgs(effect));
        }
    }
}
