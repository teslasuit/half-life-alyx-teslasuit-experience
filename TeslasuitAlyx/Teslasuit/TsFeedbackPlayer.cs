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
        private const string ANIMATIONS_DIR = "animations";

        public IHapticPlayer CurrentPlayer { get; set; }

        private IHapticAssetManager m_assetManager = null;

        private Dictionary<HLAlyxFeedbackType, TsFeedbackAnimInfo> feedbackMap = new Dictionary<HLAlyxFeedbackType, TsFeedbackAnimInfo>()
        {
            {HLAlyxFeedbackType.DefaultHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DefaultHead, "DefaultHead")},
            {HLAlyxFeedbackType.UnarmedHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHead, "UnarmedHead")},
            {HLAlyxFeedbackType.GunHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GunHead, "GunHead")},

            {HLAlyxFeedbackType.UnarmedBloater, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedBloater, "UnarmedBloater")},
            {HLAlyxFeedbackType.UnarmedHeadcrab, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrab, "UnarmedHeadcrab")},
            {HLAlyxFeedbackType.UnarmedHeadcrabArmored, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabArmored, "UnarmedHeadcrabArmored")},
            {HLAlyxFeedbackType.UnarmedHeadcrabBlack, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabBlack, "UnarmedHeadcrabBlack")},
            {HLAlyxFeedbackType.UnarmedHeadcrabFast, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabFast, "UnarmedHeadcrabFast")},
            {HLAlyxFeedbackType.UnarmedHeadcrabRunner, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedHeadcrabRunner, "UnarmedHeadcrabRunner")},
            {HLAlyxFeedbackType.UnarmedFastZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedFastZombie, "UnarmedFastZombie")},
            {HLAlyxFeedbackType.UnarmedPoisonZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedPoisonZombie, "UnarmedPoisonZombie")},
            {HLAlyxFeedbackType.UnarmedZombie, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombie, "UnarmedZombie")},
            {HLAlyxFeedbackType.UnarmedZombieBlind, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombieBlind, "UnarmedZombieBlind")},
            {HLAlyxFeedbackType.UnarmedZombine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedZombine, "UnarmedZombine")},
            {HLAlyxFeedbackType.UnarmedAntlion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedAntlion, "UnarmedAntlion")},
            {HLAlyxFeedbackType.UnarmedAntlionGuard, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedAntlionGuard, "UnarmedAntlionGuard")},
            {HLAlyxFeedbackType.UnarmedManhack, new TsFeedbackAnimInfo(HLAlyxFeedbackType.UnarmedManhack, "UnarmedManhack")},

            {HLAlyxFeedbackType.GrabbedByBarnacle, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GrabbedByBarnacle, "GrabbedByBarnacle")},

            {HLAlyxFeedbackType.ConcussionGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ConcussionGrenade, "ConcussionGrenade")},
            {HLAlyxFeedbackType.BugBaitGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BugBaitGrenade, "BugBaitGrenade")},
            {HLAlyxFeedbackType.FragGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FragGrenade, "FragGrenade")},
            {HLAlyxFeedbackType.SpyGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.SpyGrenade, "SpyGrenade")},
            {HLAlyxFeedbackType.HandGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HandGrenade, "HandGrenade")},
            {HLAlyxFeedbackType.RollerGrenade, new TsFeedbackAnimInfo(HLAlyxFeedbackType.RollerGrenade, "RollerGrenade")},
            {HLAlyxFeedbackType.RollerMine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.RollerMine, "RollerMine")},

            {HLAlyxFeedbackType.Combine, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Combine, "Combine")},
            {HLAlyxFeedbackType.CombineS, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CombineS, "CombineS")},
            {HLAlyxFeedbackType.CombineGantry, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CombineGantry, "CombineGantry")},
            {HLAlyxFeedbackType.MetroPolice, new TsFeedbackAnimInfo(HLAlyxFeedbackType.MetroPolice, "MetroPolice")},
            {HLAlyxFeedbackType.Sniper, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Sniper, "Sniper")},
            {HLAlyxFeedbackType.Strider, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Strider, "Strider")},
            {HLAlyxFeedbackType.Turret, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Turret, "Turret")},
            {HLAlyxFeedbackType.FoliageTurret, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FoliageTurret, "FoliageTurret")},

            {HLAlyxFeedbackType.EnvironmentExplosion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentExplosion, "EnvironmentExplosion")},
            {HLAlyxFeedbackType.EnvironmentLaser, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentLaser, "EnvironmentLaser")},
            {HLAlyxFeedbackType.EnvironmentFire, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentFire, "EnvironmentFire")},
            {HLAlyxFeedbackType.EnvironmentSpark, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentSpark, "EnvironmentSpark")},
            {HLAlyxFeedbackType.EnvironmentPoison, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentPoison, "EnvironmentPoison")},
            {HLAlyxFeedbackType.EnvironmentRadiation, new TsFeedbackAnimInfo(HLAlyxFeedbackType.EnvironmentRadiation, "EnvironmentRadiation")},

            {HLAlyxFeedbackType.DamageExplosion, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageExplosion, "DamageExplosion")},
            {HLAlyxFeedbackType.DamageLaser, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageLaser, "DamageLaser")},
            {HLAlyxFeedbackType.DamageFire, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageFire, "DamageFire")},
            {HLAlyxFeedbackType.DamageSpark, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DamageSpark, "DamageSpark")},

            {HLAlyxFeedbackType.PlayerShootPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootPistol, "PlayerShootPistol")},
            {HLAlyxFeedbackType.PlayerShootShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootShotgun, "PlayerShootShotgun")},
            {HLAlyxFeedbackType.PlayerShootSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootSMG, "PlayerShootSMG")},
            {HLAlyxFeedbackType.PlayerShootDefault, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootDefault, "PlayerShootDefault")},
            {HLAlyxFeedbackType.PlayerGrenadeLaunch, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerGrenadeLaunch, "PlayerGrenadeLaunch")},

            {HLAlyxFeedbackType.PlayerShootPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootPistolLeft, "PlayerShootPistolLeft")},
            {HLAlyxFeedbackType.PlayerShootShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootShotgunLeft, "PlayerShootShotgunLeft")},
            {HLAlyxFeedbackType.PlayerShootSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootSMGLeft, "PlayerShootSMGLeft")},
            {HLAlyxFeedbackType.PlayerShootDefaultLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerShootDefaultLeft, "PlayerShootDefaultLeft")},
            {HLAlyxFeedbackType.PlayerGrenadeLaunchLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.PlayerGrenadeLaunchLeft, "PlayerGrenadeLaunchLeft")},

            {HLAlyxFeedbackType.FallbackPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackPistol, "FallbackPistol")},
            {HLAlyxFeedbackType.FallbackShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackShotgun, "FallbackShotgun")},
            {HLAlyxFeedbackType.FallbackSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackSMG, "FallbackSMG")},

            {HLAlyxFeedbackType.FallbackPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackPistolLeft, "FallbackPistolLeft")},
            {HLAlyxFeedbackType.FallbackShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackShotgunLeft, "FallbackShotgunLeft")},
            {HLAlyxFeedbackType.FallbackSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.FallbackSMGLeft, "FallbackSMGLeft")},

            {HLAlyxFeedbackType.KickbackPistol, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackPistol, "KickbackPistol")},
            {HLAlyxFeedbackType.KickbackShotgun, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackShotgun, "KickbackShotgun")},
            {HLAlyxFeedbackType.KickbackSMG, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackSMG, "KickbackSMG")},

            {HLAlyxFeedbackType.KickbackPistolLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackPistolLeft, "KickbackPistolLeft")},
            {HLAlyxFeedbackType.KickbackShotgunLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackShotgunLeft, "KickbackShotgunLeft")},
            {HLAlyxFeedbackType.KickbackSMGLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.KickbackSMGLeft, "KickbackSMGLeft")},

            {HLAlyxFeedbackType.HeartBeat, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HeartBeat, "HeartBeat")},
            {HLAlyxFeedbackType.HeartBeatFast, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HeartBeatFast, "HeartBeatFast")},

            {HLAlyxFeedbackType.HealthPenUse, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthPenUse, "HealthPenUse")},
            {HLAlyxFeedbackType.HealthStationUse, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUse, "HealthStationUse")},
            {HLAlyxFeedbackType.HealthStationUseLeftArm, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUseLeftArm, "HealthStationUseLeftArm")},
            {HLAlyxFeedbackType.HealthStationUseRightArm, new TsFeedbackAnimInfo(HLAlyxFeedbackType.HealthStationUseRightArm, "HealthStationUseRightArm")},

            {HLAlyxFeedbackType.BackpackStoreClip, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreClip, "BackpackStoreClipRight")},
            {HLAlyxFeedbackType.BackpackStoreResin, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreResin, "BackpackStoreResinRight")},
            {HLAlyxFeedbackType.BackpackRetrieveClip, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveClip, "BackpackRetrieveClipRight")},
            {HLAlyxFeedbackType.BackpackRetrieveResin, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveResin, "BackpackRetrieveResinRight")},
            {HLAlyxFeedbackType.ItemHolderStore, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderStore, "ItemHolderStore")},
            {HLAlyxFeedbackType.ItemHolderRemove, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderRemove, "ItemHolderRemove")},

            {HLAlyxFeedbackType.BackpackStoreClipLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreClipLeft, "BackpackStoreClipLeft")},
            {HLAlyxFeedbackType.BackpackStoreResinLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackStoreResinLeft, "BackpackStoreResinLeft")},
            {HLAlyxFeedbackType.BackpackRetrieveClipLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveClipLeft, "BackpackRetrieveClipLeft")},
            {HLAlyxFeedbackType.BackpackRetrieveResinLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.BackpackRetrieveResinLeft, "BackpackRetrieveResinLeft")},
            {HLAlyxFeedbackType.ItemHolderStoreLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderStoreLeft, "ItemHolderStoreLeft")},
            {HLAlyxFeedbackType.ItemHolderRemoveLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ItemHolderRemoveLeft, "ItemHolderRemoveLeft")},

            {HLAlyxFeedbackType.GravityGloveLockOn, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveLockOn, "GravityGloveLockOn")},
            {HLAlyxFeedbackType.GravityGlovePull, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGlovePull, "GravityGlovePull")},
            {HLAlyxFeedbackType.GravityGloveCatch, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveCatch, "GravityGloveCatch")},

            {HLAlyxFeedbackType.GravityGloveLockOnLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveLockOnLeft, "GravityGloveLockOnLeft")},
            {HLAlyxFeedbackType.GravityGlovePullLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGlovePullLeft, "GravityGlovePullLeft")},
            {HLAlyxFeedbackType.GravityGloveCatchLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.GravityGloveCatchLeft, "GravityGloveCatchLeft")},

            {HLAlyxFeedbackType.ClipInserted, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ClipInserted, "ClipInserted")},
            {HLAlyxFeedbackType.ChamberedRound, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ChamberedRound, "ChamberedRound")},
            {HLAlyxFeedbackType.ClipInsertedLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ClipInsertedLeft, "ClipInsertedLeft")},
            {HLAlyxFeedbackType.ChamberedRoundLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ChamberedRoundLeft, "ChamberedRoundLeft")},

            {HLAlyxFeedbackType.Cough, new TsFeedbackAnimInfo(HLAlyxFeedbackType.Cough, "Cough")},
            {HLAlyxFeedbackType.CoughHead, new TsFeedbackAnimInfo(HLAlyxFeedbackType.CoughHead, "CoughHead")},

            {HLAlyxFeedbackType.ShockOnHandLeft, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ShockOnHandLeft, "ShockOnHandLeft")},
            {HLAlyxFeedbackType.ShockOnHandRight, new TsFeedbackAnimInfo(HLAlyxFeedbackType.ShockOnHandRight, "ShockOnHandRight")},


            {HLAlyxFeedbackType.DefaultDamage, new TsFeedbackAnimInfo(HLAlyxFeedbackType.DefaultDamage, "DefaultDamage")}
        };

        private struct TsFeedbackAnimInfo
        {
            public HLAlyxFeedbackType feedbackType;
            public string animName;
            public bool enabled;
            public IHapticAsset hapticAsset;

            public TsFeedbackAnimInfo(HLAlyxFeedbackType _feedbackType, string name)
            {
                feedbackType = _feedbackType;
                animName = name;
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

            var path = Path.Combine(Directory.GetCurrentDirectory(), ANIMATIONS_DIR);
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Failed to scan animations directory: not found.");
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] anims = dir.GetFiles("*.ts_asset");

            for (int i = 0; i < anims.Length; i++)
            {
                string animName = Path.GetFileNameWithoutExtension(anims[i].Name);
                string animPath = Path.Combine(ANIMATIONS_DIR, anims[i].Name);
                var fileIter = feedbackMap.Where((item) => item.Value.animName == animName);
                if (fileIter.Any())
                {
                    var feedback = fileIter.First().Value;
                    feedback.enabled = true;
                    if (feedback.hapticAsset == null && m_assetManager != null)
                    {
                        feedback.hapticAsset = m_assetManager.Load(File.ReadAllBytes(animPath));
                        Console.WriteLine($"Animation loaded: {anims[i].Name}");
                    }
                    feedbackMap[feedback.feedbackType] = feedback;
                }
            }
        }

        public void Play(HLAlyxFeedbackEventArgs feedback)
        {
            if (feedbackMap.TryGetValue(feedback.FeedbackType, out var anim))
            {
                if (!anim.enabled)
                {
                    Console.WriteLine($"No asset with filename: {anim.animName}. Skipping...");
                    TryDefaultDamagePlay(feedback);
                    return;
                }
                if (CurrentPlayer != null && anim.hapticAsset != null)
                {
                    var playable = m_assetManager.GetPlayable(CurrentPlayer.Device, anim.hapticAsset);
                    var multiplierPW = new TsHapticParamMultiplier(TsHapticParamType.PulseWidth, feedback.Multiplier);
                    var multiplierA = new TsHapticParamMultiplier(TsHapticParamType.Amplitude, feedback.Multiplier);

                    playable.SetMultiplier(multiplierPW);
                    playable.SetMultiplier(multiplierA);

                    if (!feedback.DontReplay || (feedback.DontReplay && !playable.IsPlaying))
                    {
                        Console.WriteLine($"Playing {anim.animName}");
                        //CurrentPlayer.Play(playable);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to play {anim.animName}: No device found.");
                }
            }
        }
        
        public void Stop(HLAlyxFeedbackEventArgs feedback)
        {
            if (feedbackMap.TryGetValue(feedback.FeedbackType, out var anim))
            {
                if (!anim.enabled)
                {
                    Console.WriteLine($"No asset with filename: {anim.animName}. Skipping...");
                    TryDefaultDamageStop(feedback);
                    return;
                }

                if (CurrentPlayer != null && anim.hapticAsset != null)
                {
                    Console.WriteLine($"Stopping {anim.animName}");
                    var playable = m_assetManager.GetPlayable(CurrentPlayer.Device, anim.hapticAsset);
                    //CurrentPlayer.Stop(playable);
                }
                else
                {
                    Console.WriteLine($"Failed to play {anim.animName}: No device found.");
                }
            }
        }

        public void TryDefaultDamagePlay(HLAlyxFeedbackEventArgs feedback)
        {
            if (feedback.FeedbackType == HLAlyxFeedbackType.DefaultDamage)
            {
                return;
            }

            if ((feedback.FeedbackType >= HLAlyxFeedbackType.EnemyWithoutGunBegin && feedback.FeedbackType <= HLAlyxFeedbackType.EnemyWithoutGunEnd) ||
                (feedback.FeedbackType >= HLAlyxFeedbackType.EnemyWithGunBegin && feedback.FeedbackType <= HLAlyxFeedbackType.EnemyWithGunEnd))
            {
                feedback = new HLAlyxFeedbackEventArgs(HLAlyxFeedbackType.DefaultDamage, feedback.Angle, feedback.Height, feedback.Multiplier, feedback.DontReplay);
                Play(feedback);
            }
        }

        public void TryDefaultDamageStop(HLAlyxFeedbackEventArgs feedback)
        {
            if (feedback.FeedbackType == HLAlyxFeedbackType.DefaultDamage)
            {
                return;
            }

            if ((feedback.FeedbackType >= HLAlyxFeedbackType.EnemyWithoutGunBegin && feedback.FeedbackType <= HLAlyxFeedbackType.EnemyWithoutGunEnd) ||
                (feedback.FeedbackType >= HLAlyxFeedbackType.EnemyWithGunBegin && feedback.FeedbackType <= HLAlyxFeedbackType.EnemyWithGunEnd))
            {
                feedback = new HLAlyxFeedbackEventArgs(HLAlyxFeedbackType.DefaultDamage, feedback.Angle, feedback.Height, feedback.Multiplier, feedback.DontReplay);
                Play(feedback);
            }
        }

    }
}
