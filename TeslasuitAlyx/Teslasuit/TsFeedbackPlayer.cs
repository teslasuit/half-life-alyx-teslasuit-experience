using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TsApi.Types;
using TsSDK;

namespace TeslasuitAlyx
{
    public class TsFeedbackPlayer
    {
        public IHapticPlayer CurrentPlayer { get; set; }

        private IHapticAssetManager m_assetManager = null;

        private Dictionary<HLAlyxFeedbackType, TsFeedbackAnimInfo> feedbackMap = new Dictionary<HLAlyxFeedbackType, TsFeedbackAnimInfo>()
        {
            {HLAlyxFeedbackType.DefaultHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DefaultHead, "DefaultHead_")},
            {HLAlyxFeedbackType.UnarmedHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHead, "UnarmedHead_")},
            {HLAlyxFeedbackType.GunHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GunHead, "GunHead_")},

            {HLAlyxFeedbackType.UnarmedBloater, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedBloater, "UnarmedBloater_")},
            {HLAlyxFeedbackType.UnarmedHeadcrab, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrab, "UnarmedHeadcrab_")},
            {HLAlyxFeedbackType.UnarmedHeadcrabArmored, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabArmored, "UnarmedHeadcrabArmored_")},
            {HLAlyxFeedbackType.UnarmedHeadcrabBlack, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabBlack, "UnarmedHeadcrabBlack_")},
            {HLAlyxFeedbackType.UnarmedHeadcrabFast, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabFast, "UnarmedHeadcrabFast_")},
            {HLAlyxFeedbackType.UnarmedHeadcrabRunner, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabRunner, "UnarmedHeadcrabRunner_")},
            {HLAlyxFeedbackType.UnarmedFastZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedFastZombie, "UnarmedFastZombie_")},
            {HLAlyxFeedbackType.UnarmedPoisonZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedPoisonZombie, "UnarmedPoisonZombie_")},
            {HLAlyxFeedbackType.UnarmedZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombie, "UnarmedZombie_")},
            {HLAlyxFeedbackType.UnarmedZombieBlind, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombieBlind, "UnarmedZombieBlind_")},
            {HLAlyxFeedbackType.UnarmedZombine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombine, "UnarmedZombine_")},
            {HLAlyxFeedbackType.UnarmedAntlion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedAntlion, "UnarmedAntlion_")},
            {HLAlyxFeedbackType.UnarmedAntlionGuard, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedAntlionGuard, "UnarmedAntlionGuard_")},
            {HLAlyxFeedbackType.UnarmedManhack, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedManhack, "UnarmedManhack_")},

            {HLAlyxFeedbackType.GrabbedByBarnacle, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GrabbedByBarnacle, "GrabbedByBarnacle_")},

            {HLAlyxFeedbackType.ConcussionGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ConcussionGrenade, "ConcussionGrenade_")},
            {HLAlyxFeedbackType.BugBaitGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BugBaitGrenade, "BugBaitGrenade_")},
            {HLAlyxFeedbackType.FragGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FragGrenade, "FragGrenade_")},
            {HLAlyxFeedbackType.SpyGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.SpyGrenade, "SpyGrenade_")},
            {HLAlyxFeedbackType.HandGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HandGrenade, "HandGrenade_")},
            {HLAlyxFeedbackType.RollerGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.RollerGrenade, "RollerGrenade_")},
            {HLAlyxFeedbackType.RollerMine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.RollerMine, "RollerMine_")},

            {HLAlyxFeedbackType.Combine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Combine, "Combine_")},
            {HLAlyxFeedbackType.CombineS, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CombineS, "CombineS_")},
            {HLAlyxFeedbackType.CombineGantry, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CombineGantry, "CombineGantry_")},
            {HLAlyxFeedbackType.MetroPolice, new TsFeedbackAnimInfo(HLAlyxFeedbackType.MetroPolice, "MetroPolice_")},
            {HLAlyxFeedbackType.Sniper, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Sniper, "Sniper_")},
            {HLAlyxFeedbackType.Strider, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Strider, "Strider_")},
            {HLAlyxFeedbackType.Turret, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Turret, "Turret_")},
            {HLAlyxFeedbackType.FoliageTurret, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FoliageTurret, "FoliageTurret_")},

            {HLAlyxFeedbackType.EnvironmentExplosion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentExplosion, "EnvironmentExplosion_")},
            {HLAlyxFeedbackType.EnvironmentLaser, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentLaser, "EnvironmentLaser_")},
            {HLAlyxFeedbackType.EnvironmentFire, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentFire, "EnvironmentFire_")},
            {HLAlyxFeedbackType.EnvironmentSpark, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentSpark, "EnvironmentSpark_")},
            {HLAlyxFeedbackType.EnvironmentPoison, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentPoison, "EnvironmentPoison_")},
            {HLAlyxFeedbackType.EnvironmentRadiation, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentRadiation, "EnvironmentRadiation_")},

            {HLAlyxFeedbackType.DamageExplosion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageExplosion, "DamageExplosion_")},
            {HLAlyxFeedbackType.DamageLaser, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageLaser, "DamageLaser_")},
            {HLAlyxFeedbackType.DamageFire, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageFire, "DamageFire_")},
            {HLAlyxFeedbackType.DamageSpark, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageSpark, "DamageSpark_")},

            {HLAlyxFeedbackType.PlayerShootPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootPistol, "PlayerShootPistol_")},
            {HLAlyxFeedbackType.PlayerShootShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootShotgun, "PlayerShootShotgun_")},
            {HLAlyxFeedbackType.PlayerShootSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootSMG, "PlayerShootSMG_")},
            {HLAlyxFeedbackType.PlayerShootDefault, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootDefault, "PlayerShootDefault_")},
            {HLAlyxFeedbackType.PlayerGrenadeLaunch, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerGrenadeLaunch, "PlayerGrenadeLaunch_")},

            {HLAlyxFeedbackType.PlayerShootPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootPistolLeft, "PlayerShootPistolLeft_")},
            {HLAlyxFeedbackType.PlayerShootShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootShotgunLeft, "PlayerShootShotgunLeft_")},
            {HLAlyxFeedbackType.PlayerShootSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootSMGLeft, "PlayerShootSMGLeft_")},
            {HLAlyxFeedbackType.PlayerShootDefaultLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootDefaultLeft, "PlayerShootDefaultLeft_")},
            {HLAlyxFeedbackType.PlayerGrenadeLaunchLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerGrenadeLaunchLeft, "PlayerGrenadeLaunchLeft_")},

            {HLAlyxFeedbackType.FallbackPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackPistol, "FallbackPistol_")},
            {HLAlyxFeedbackType.FallbackShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackShotgun, "FallbackShotgun_")},
            {HLAlyxFeedbackType.FallbackSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackSMG, "FallbackSMG_")},

            {HLAlyxFeedbackType.FallbackPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackPistolLeft, "FallbackPistolLeft_")},
            {HLAlyxFeedbackType.FallbackShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackShotgunLeft, "FallbackShotgunLeft_")},
            {HLAlyxFeedbackType.FallbackSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackSMGLeft, "FallbackSMGLeft_")},

            {HLAlyxFeedbackType.KickbackPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackPistol, "KickbackPistol_")},
            {HLAlyxFeedbackType.KickbackShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackShotgun, "KickbackShotgun_")},
            {HLAlyxFeedbackType.KickbackSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackSMG, "KickbackSMG_")},

            {HLAlyxFeedbackType.KickbackPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackPistolLeft, "KickbackPistolLeft_")},
            {HLAlyxFeedbackType.KickbackShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackShotgunLeft, "KickbackShotgunLeft_")},
            {HLAlyxFeedbackType.KickbackSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackSMGLeft, "KickbackSMGLeft_")},

            {HLAlyxFeedbackType.HeartBeat, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HeartBeat, "HeartBeat_")},
            {HLAlyxFeedbackType.HeartBeatFast, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HeartBeatFast, "HeartBeatFast_")},

            {HLAlyxFeedbackType.HealthPenUse, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthPenUse, "HealthPenUse_")},
            {HLAlyxFeedbackType.HealthStationUse, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUse, "HealthStationUse_")},
            {HLAlyxFeedbackType.HealthStationUseLeftArm, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUseLeftArm, "HealthStationUseLeftArm_")},
            {HLAlyxFeedbackType.HealthStationUseRightArm, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUseRightArm, "HealthStationUseRightArm_")},

            {HLAlyxFeedbackType.BackpackStoreClip, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreClip, "BackpackStoreClipRight_")},
            {HLAlyxFeedbackType.BackpackStoreResin, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreResin, "BackpackStoreResinRight_")},
            {HLAlyxFeedbackType.BackpackRetrieveClip, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveClip, "BackpackRetrieveClipRight_")},
            {HLAlyxFeedbackType.BackpackRetrieveResin, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveResin, "BackpackRetrieveResinRight_")},
            {HLAlyxFeedbackType.ItemHolderStore, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderStore, "ItemHolderStore_")},
            {HLAlyxFeedbackType.ItemHolderRemove, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderRemove, "ItemHolderRemove_")},

            {HLAlyxFeedbackType.BackpackStoreClipLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreClipLeft, "BackpackStoreClipLeft_")},
            {HLAlyxFeedbackType.BackpackStoreResinLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreResinLeft, "BackpackStoreResinLeft_")},
            {HLAlyxFeedbackType.BackpackRetrieveClipLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveClipLeft, "BackpackRetrieveClipLeft_")},
            {HLAlyxFeedbackType.BackpackRetrieveResinLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveResinLeft, "BackpackRetrieveResinLeft_")},
            {HLAlyxFeedbackType.ItemHolderStoreLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderStoreLeft, "ItemHolderStoreLeft_")},
            {HLAlyxFeedbackType.ItemHolderRemoveLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderRemoveLeft, "ItemHolderRemoveLeft_")},

            {HLAlyxFeedbackType.GravityGloveLockOn, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveLockOn, "GravityGloveLockOn_")},
            {HLAlyxFeedbackType.GravityGlovePull, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGlovePull, "GravityGlovePull_")},
            {HLAlyxFeedbackType.GravityGloveCatch, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveCatch, "GravityGloveCatch_")},

            {HLAlyxFeedbackType.GravityGloveLockOnLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveLockOnLeft, "GravityGloveLockOnLeft_")},
            {HLAlyxFeedbackType.GravityGlovePullLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGlovePullLeft, "GravityGlovePullLeft_")},
            {HLAlyxFeedbackType.GravityGloveCatchLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveCatchLeft, "GravityGloveCatchLeft_")},

            {HLAlyxFeedbackType.ClipInserted, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ClipInserted, "ClipInserted_")},
            {HLAlyxFeedbackType.ChamberedRound, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ChamberedRound, "ChamberedRound_")},
            {HLAlyxFeedbackType.ClipInsertedLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ClipInsertedLeft, "ClipInsertedLeft_")},
            {HLAlyxFeedbackType.ChamberedRoundLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ChamberedRoundLeft, "ChamberedRoundLeft_")},

            {HLAlyxFeedbackType.Cough, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Cough, "Cough_")},
            {HLAlyxFeedbackType.CoughHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CoughHead, "CoughHead_")},

            {HLAlyxFeedbackType.ShockOnHandLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ShockOnHandLeft, "ShockOnHandLeft_")},
            {HLAlyxFeedbackType.ShockOnHandRight, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ShockOnHandRight, "ShockOnHandRight_")},


            {HLAlyxFeedbackType.DefaultDamage, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DefaultDamage, "DefaultDamage_")}
        };

        private struct TsFeedbackAnimInfo
        {
            public HLAlyxFeedbackType feedbackType;
            public string filename;
            public bool enabled;
            public IHapticAsset hapticAsset;

            public TsFeedbackAnimInfo(HLAlyxFeedbackType _feedbackType, string name)
            {
                feedbackType = _feedbackType;
                filename = name;
                enabled = false;
                hapticAsset = null;
            }
        }

        public TsFeedbackPlayer(IHapticAssetManager hapticAssetManager)
        {
            this.m_assetManager = hapticAssetManager;
            ScanAnimations();
        }

        public void ScanAnimations()
        {
            var path = Directory.GetCurrentDirectory() + "\\animations";
            if (!Directory.Exists(path))
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] anims = dir.GetFiles("*.ts_asset");

            for (int i = 0; i < anims.Length; i++)
            {
                string filename = anims[i].Name;

                var fileIter = feedbackMap.Where((item) => item.Value.filename == filename);
                if (fileIter.Any())
                {
                    var feedback = fileIter.First().Value;
                    feedback.enabled = true;
                    if(feedback.hapticAsset == null && m_assetManager != null)
                    {
                        feedback.hapticAsset = m_assetManager.Load(File.ReadAllBytes(filename));
                    }
                    feedbackMap[feedback.feedbackType] = feedback;
                }
            }
        }

        public void Play(HLAlyxFeedbackEventArgs feedback)
        {
            if(feedbackMap.TryGetValue(feedback.FeedbackType, out var anim))
            {
                Console.WriteLine($"Playing {anim.filename}");
                if(!anim.enabled)
                {
                    Console.WriteLine($"No asset with filename: {anim.filename}. Skipping...");
                    return;
                }
                if(CurrentPlayer != null && anim.hapticAsset != null)
                {
                    if(anim.hapticAsset != null)
                    {
                        var playable = m_assetManager.GetPlayable(CurrentPlayer.Device, anim.hapticAsset);
                        var multiplierPW = new TsHapticParamMultiplier(TsHapticParamType.PulseWidth, feedback.Multiplier);
                        var multiplierA = new TsHapticParamMultiplier(TsHapticParamType.Amplitude, feedback.Multiplier);
                        playable.SetMultiplier(multiplierPW);
                        playable.SetMultiplier(multiplierA);
                        CurrentPlayer.Play(playable);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to play {anim.filename}: Asset not found.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to play {anim.filename}: No device found.");
                }
            }
        }

        public void Stop(HLAlyxFeedbackEventArgs feedback)
        {
            if(feedbackMap.TryGetValue(feedback.FeedbackType, out var anim))
            {
                Console.WriteLine($"Stopping {anim.filename}");
                if(!anim.enabled)
                {
                    Console.WriteLine($"No asset with filename: {anim.filename}. Skipping...");
                    return;
                }

                if(CurrentPlayer != null && anim.hapticAsset != null)
                {
                    if(anim.hapticAsset != null)
                    {
                        var playable = m_assetManager.GetPlayable(CurrentPlayer.Device, anim.hapticAsset);
                        CurrentPlayer.Stop(playable);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to play {anim.filename}: Asset not found.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to play {anim.filename}: No device found.");
                }
            }
        }

    }
}
