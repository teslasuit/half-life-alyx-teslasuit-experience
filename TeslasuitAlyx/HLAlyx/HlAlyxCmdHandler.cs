using System;
using System.Collections.Generic;
using System.Threading;

namespace TeslasuitAlyx
{
    public class HlAlyxCmdHandler
    {
        private HLAlyxFeedbackEventProvider eventManager;

        private int playerRemainingHealth;

        private bool lowHealthFeedbackPlaying = false;
        private bool veryLowHealthFeedbackPlaying = false;

        private int grenadeLauncherState = 0;

        public bool twoHandedMode = false;

        public bool menuOpen = false;

        public bool leftHandedMode = false;

        public bool gravityPrimaryLock = false;
        public bool gravitySecondaryLock = false;

        public bool barnacleGrab = false;

        public bool coughing = false;

        private const int sleepDurationHeartBeat = 900;
        private const int sleepDurationHeartBeatFast = 450;
        private const int sleepDurationHeartBeatTooFast = 250;
        private const int sleepDurationGrabbityLock = 780;
        private const int sleepDurationBarnacleGrab = 900;
        private const int intensityMultiplierFoliageTurret = 2;
        private const int sleepDurationCoughing = 1500;


        public HlAlyxCmdHandler(HLAlyxFeedbackEventProvider eventManager)
        {
            this.eventManager = eventManager;
        }

        private void LowHealthFeedback()
        {
            while (playerRemainingHealth > HLAlyxConfig.VeryLowHealthAmount && playerRemainingHealth <= HLAlyxConfig.LowHealthAmount)
            {
                if (!menuOpen)
                {
                    eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.HeartBeat, false);
                }

                Thread.Sleep(sleepDurationHeartBeat);
            }

            lowHealthFeedbackPlaying = false;
        }

        private void VeryLowHealthFeedback(bool tooLow)
        {
            while (playerRemainingHealth < HLAlyxConfig.VeryLowHealthAmount && playerRemainingHealth > 0)
            {
                if (!menuOpen)
                {
                    eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.HeartBeatFast, false);
                }

                Thread.Sleep(tooLow ? sleepDurationHeartBeatTooFast : sleepDurationHeartBeatFast);
            }

            veryLowHealthFeedbackPlaying = false;
        }

        public void HealthRemaining(int healthRemaining)
        {
            playerRemainingHealth = healthRemaining;

            if (playerRemainingHealth <= HLAlyxConfig.TooLowHealthAmount)
            {
                if (!veryLowHealthFeedbackPlaying)
                {
                    veryLowHealthFeedbackPlaying = true;
                    Thread thread = new Thread(() => VeryLowHealthFeedback(true));
                    thread.Start();
                }
            }
            else if (playerRemainingHealth <= HLAlyxConfig.VeryLowHealthAmount)
            {
                if (!veryLowHealthFeedbackPlaying)
                {
                    veryLowHealthFeedbackPlaying = true;
                    Thread thread = new Thread(() => VeryLowHealthFeedback(false));
                    thread.Start();
                }
            }
            else if (playerRemainingHealth <= HLAlyxConfig.LowHealthAmount)
            {
                if (!lowHealthFeedbackPlaying)
                {
                    lowHealthFeedbackPlaying = true;
                    Thread thread = new Thread(LowHealthFeedback);
                    thread.Start();
                }
            }
        }

        public void PlayerHurt(int healthRemaining, string enemy, float locationAngle, string enemyName, string enemyDebugName)
        {
            //Heartbeat stuff
            HealthRemaining(healthRemaining);

            //Damage stuff

            float locationHeight = 0.5f;

            HLAlyxFeedbackType feedback = eventManager.GetFeedbackTypeOfEnemyAttack(enemy, enemyName);

            bool headcrab = (eventManager.HeadCrabFeedback(feedback));

            if (headcrab)
            {
                locationHeight = 0.35f;
            }

            if (!headcrab && !eventManager.EnvironmentFeedback(feedback))
            {

                locationHeight = ((float)(new Random(100).Next()) / 100.0f) - 0.5f;
            }

            if (locationHeight > 0.485f || headcrab)
            {
                eventManager.FireHapticFeedbackStart(locationAngle, 0, eventManager.GetHeadFeedbackVersion(feedback), false);
            }
            eventManager.FireHapticFeedbackStart(locationAngle, locationHeight, feedback, false);
        }

        public void PlayerShoot(string weapon)
        {
            HLAlyxFeedbackType feedback = eventManager.GetFeedbackTypeOfWeaponFromPlayer(weapon, leftHandedMode);

            feedback = eventManager.GetFallbackTypeOfWeaponFromPlayer(feedback, leftHandedMode);

            eventManager.FireHapticFeedbackStart(0, 0, feedback, false, twoHandedMode ? eventManager.GetOtherHandFeedback(feedback) : HLAlyxFeedbackType.NoFeedback);
            eventManager.FireHapticFeedbackStart(0, 0, eventManager.GetKickbackOfWeaponFromPlayer(feedback, leftHandedMode), false);
        }

        private void GrabbityLock(bool primaryHand)
        {
            while ((primaryHand && gravityPrimaryLock) || (!primaryHand && gravitySecondaryLock))
            {
                if (!menuOpen)
                {
                    float locationHeight = ((float)(new Random(100).Next()) / 100.0f) - 0.5f;
                    eventManager.FireHapticFeedbackStart(0, locationHeight, (leftHandedMode ? primaryHand : !primaryHand) ? HLAlyxFeedbackType.GravityGloveLockOnLeft : HLAlyxFeedbackType.GravityGloveLockOn, false);
                }

                Thread.Sleep(sleepDurationGrabbityLock);
            }
        }

        public void GrabbityLockStart(bool primaryHand)
        {
            if (primaryHand && !gravityPrimaryLock)
            {
                gravityPrimaryLock = true;
                Thread thread = new Thread(() => GrabbityLock(true));
                thread.Start();
            }
            else if (!primaryHand && !gravitySecondaryLock)
            {
                gravitySecondaryLock = true;
                Thread thread = new Thread(() => GrabbityLock(false));
                thread.Start();
            }
        }

        public void GrabbityLockStop(bool primaryHand)
        {
            if (primaryHand)
            {
                gravityPrimaryLock = false;
            }
            else
            {
                gravitySecondaryLock = false;
            }
            eventManager.FireHapticFeedbackStop((leftHandedMode ? primaryHand : !primaryHand) ? HLAlyxFeedbackType.GravityGloveLockOnLeft : HLAlyxFeedbackType.GravityGloveLockOn);
        }

        public void GrabbityGlovePull(bool primaryHand)
        {
            eventManager.FireHapticFeedbackStart(0, 0, (leftHandedMode ? primaryHand : !primaryHand) ? HLAlyxFeedbackType.GravityGlovePullLeft : HLAlyxFeedbackType.GravityGlovePull, true);
        }

        public void GrenadeLauncherStateChange(int newState)
        {
            if (grenadeLauncherState == 2 && newState == 0)
            {
                HLAlyxFeedbackType feedback = leftHandedMode
                    ? HLAlyxFeedbackType.PlayerGrenadeLaunchLeft
                    : HLAlyxFeedbackType.PlayerGrenadeLaunch;
                eventManager.FireHapticFeedbackStart(0, 0, feedback, false, eventManager.GetOtherHandFeedback(feedback));
                eventManager.FireHapticFeedbackStart(0, 0, eventManager.GetKickbackOfWeaponFromPlayer(HLAlyxFeedbackType.PlayerGrenadeLaunch, leftHandedMode), false);
            }

            grenadeLauncherState = newState;
        }

        private void BarnacleGrab()
        {
            while (barnacleGrab)
            {
                if (!menuOpen)
                {
                    eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.GrabbedByBarnacle, false);
                }

                Thread.Sleep(sleepDurationBarnacleGrab);
            }
        }

        public void BarnacleGrabStart()
        {
            if (!barnacleGrab)
            {
                barnacleGrab = true;
                Thread thread = new Thread(BarnacleGrab);
                thread.Start();
            }
        }

        public void Reset()
        {
            playerRemainingHealth = 100;
            grenadeLauncherState = 0;
            barnacleGrab = false;
            coughing = false;
            gravityPrimaryLock = false;
            gravitySecondaryLock = false;
            lowHealthFeedbackPlaying = false;
            veryLowHealthFeedbackPlaying = false;
            eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.HeartBeat, true);
        }

        public void PlayerDeath(int damagebits)
        {
            playerRemainingHealth = 0;
            barnacleGrab = false;
            coughing = false;
            gravityPrimaryLock = false;
            gravitySecondaryLock = false;
            lowHealthFeedbackPlaying = false;
            veryLowHealthFeedbackPlaying = false;
            var damageMask = (HLAlyxDamageTypeMask)damagebits;

            damageMask.ForEach((item) =>
            {
                switch (item)
                {
                    case HLAlyxDamageTypeMask.DMG_GENERIC:
                        break;
                    case HLAlyxDamageTypeMask.DMG_CRUSH:
                    case HLAlyxDamageTypeMask.DMG_VEHICLE:
                    case HLAlyxDamageTypeMask.DMG_FALL:
                    case HLAlyxDamageTypeMask.DMG_CLUB:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.DefaultDamage, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_BULLET:
                        break;
                    case HLAlyxDamageTypeMask.DMG_SLASH:
                        break;
                    case HLAlyxDamageTypeMask.DMG_BURN:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentFire, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_BLAST:
                    case HLAlyxDamageTypeMask.DMG_SONIC:
                    case HLAlyxDamageTypeMask.DMG_BLAST_SURFACE:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentExplosion, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_SHOCK:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentSpark, false);
                        break;

                    case HLAlyxDamageTypeMask.DMG_ENERGYBEAM:
                    case HLAlyxDamageTypeMask.DMG_PLASMA:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentLaser, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_PREVENT_PHYSICS_FORCE:
                        break;
                    case HLAlyxDamageTypeMask.DMG_NEVERGIB:
                        break;
                    case HLAlyxDamageTypeMask.DMG_ALWAYSGIB:
                        break;
                    case HLAlyxDamageTypeMask.DMG_DROWN:
                        break;
                    case HLAlyxDamageTypeMask.DMG_PARALYZE:
                        break;
                    case HLAlyxDamageTypeMask.DMG_NERVEGAS:
                    case HLAlyxDamageTypeMask.DMG_POISON:
                    case HLAlyxDamageTypeMask.DMG_ACID:
                    case HLAlyxDamageTypeMask.DMG_SLOWBURN:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentPoison, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_RADIATION:
                    case HLAlyxDamageTypeMask.DMG_DISSOLVE:
                        eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.EnvironmentRadiation, false);
                        break;
                    case HLAlyxDamageTypeMask.DMG_DROWNRECOVER:
                        break;
                    case HLAlyxDamageTypeMask.DMG_REMOVENORAGDOLL:
                        break;
                    case HLAlyxDamageTypeMask.DMG_PHYSGUN:
                        break;
                    case HLAlyxDamageTypeMask.DMG_AIRBOAT:
                        break;
                    case HLAlyxDamageTypeMask.DMG_DIRECT:
                        break;
                    case HLAlyxDamageTypeMask.DMG_BUCKSHOT:
                        break;
                }
            });

            //TODO ADD EFFECT CALLS HERE
        }

        public void GrabbityGloveCatch(bool primaryHand)
        {
            HLAlyxFeedbackType feedback = (leftHandedMode ? primaryHand : !primaryHand)
                ? HLAlyxFeedbackType.GravityGloveCatchLeft
                : HLAlyxFeedbackType.GravityGloveCatch;
            eventManager.FireHapticFeedbackStart(0, 0, feedback, false);
        }

        public void DropAmmoInBackpack(bool leftShoulder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftShoulder ? HLAlyxFeedbackType.BackpackStoreClipLeft : HLAlyxFeedbackType.BackpackStoreClip, false);
        }

        public void DropResinInBackpack(bool leftShoulder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftShoulder ? HLAlyxFeedbackType.BackpackStoreResinLeft : HLAlyxFeedbackType.BackpackStoreResin, false);
        }

        public void RetrievedBackpackClip(bool leftShoulder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftShoulder ? HLAlyxFeedbackType.BackpackRetrieveClipLeft : HLAlyxFeedbackType.BackpackRetrieveClip, false);
        }

        public void RetrievedBackpackResin(bool leftShoulder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftShoulder ? HLAlyxFeedbackType.BackpackRetrieveResinLeft : HLAlyxFeedbackType.BackpackRetrieveResin, false);
        }

        public void StoredItemInItemholder(bool leftHolder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftHolder ? HLAlyxFeedbackType.ItemHolderStoreLeft : HLAlyxFeedbackType.ItemHolderStore, false);
        }

        public void RemovedItemFromItemholder(bool leftHolder)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftHolder ? HLAlyxFeedbackType.ItemHolderRemoveLeft : HLAlyxFeedbackType.ItemHolderRemove, false);
        }

        public void HealthPenUse(float angle)
        {
            eventManager.FireHapticFeedbackStart(angle, 0, HLAlyxFeedbackType.HealthPenUse, false);
        }

        private void HealthStationUseFunc(bool leftArm)
        {
            Thread.Sleep(2000);
            for (int i = 0; i < (100 - playerRemainingHealth) / 12; i++)
            {
                eventManager.FireHapticFeedbackStart(0, 0,
                    leftArm
                        ? HLAlyxFeedbackType.HealthStationUseLeftArm
                        : HLAlyxFeedbackType.HealthStationUseRightArm, false);
                eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.HealthStationUse, false,
                    HLAlyxFeedbackType.NoFeedback);
                Thread.Sleep(2000);
            }
        }

        public void HealthStationUse(bool leftArm)
        {
            Thread thread = new Thread(() => HealthStationUseFunc(leftArm));
            thread.Start();
        }

        public void ClipInserted()
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftHandedMode ? HLAlyxFeedbackType.ClipInsertedLeft : HLAlyxFeedbackType.ClipInserted, false);
        }

        public void ChamberedRound()
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftHandedMode ? HLAlyxFeedbackType.ChamberedRoundLeft : HLAlyxFeedbackType.ChamberedRound, false);
        }

        private void CoughFunc()
        {
            while (coughing)
            {
                if (!menuOpen)
                {
                    eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.CoughHead, false);

                    eventManager.FireHapticFeedbackStart(0, 0, HLAlyxFeedbackType.Cough, false);
                }

                Thread.Sleep(sleepDurationCoughing);
            }
        }

        public void Cough()
        {
            coughing = true;
            Thread thread = new Thread(CoughFunc);
            thread.Start();
        }

        public void ShockOnArm(bool leftArm)
        {
            eventManager.FireHapticFeedbackStart(0, 0, leftArm ? HLAlyxFeedbackType.ShockOnHandLeft : HLAlyxFeedbackType.ShockOnHandRight, false);
        }

        ~HlAlyxCmdHandler()
        {
            isRunning = false;
            waitCmdLock.Set();
            if(cmdThread.IsAlive)
            {
                cmdThread.Join();
            }
        }

        private Queue<HLAlyxCmd> commands = new Queue<HLAlyxCmd>();
        private Thread cmdThread = null;
        private ManualResetEvent waitCmdLock = new ManualResetEvent(false);
        private object commandsLock = new object();
        private bool isRunning = false;

        public void EnqueueHandleCmd(HLAlyxCmd cmd)
        {
            if(cmdThread == null)
            {
                cmdThread = new Thread(RunCmdWorker);
                isRunning = true;
                cmdThread.Start();
            }
            lock(commandsLock)
            {
                commands.Enqueue(cmd);
                waitCmdLock.Set();
            }
        }

        private void RunCmdWorker()
        {
            while (isRunning)
            {
                lock (commandsLock)
                {
                    while (commands.Count > 0)
                    {
                        var cmd = commands.Dequeue();
                        HandleCmd(cmd);
                    }
                }
                waitCmdLock.Reset();
                waitCmdLock.WaitOne();
            }
        }

        public void HandleCmd(HLAlyxCmd cmd)
        {
            string[] args = cmd.args;

            string command = cmd.header;

            switch (command)
            {
                case "PlayerHealth":
                    {
                        int health = args.TryGetInt(0, -1);
                        if (health >= 0)
                        {
                            HealthRemaining(health);
                        }
                        break;
                    }
                case "PlayerHurt":
                    {
                        int healthRemaining = args.TryGetInt(0);
                        string enemy = args.TryGetString(1);
                        float playerAngle = args.TryGetFloat(2);
                        string enemyName = args.TryGetString(3);
                        string enemyDebugName = args.TryGetString(4);
                        PlayerHurt(healthRemaining, enemy, playerAngle, enemyName, enemyDebugName);
                        break;
                    }
                case "PlayerShootWeapon":
                    {
                        PlayerShoot(args.TryGetString(0));
                        break;
                    }
                case "TwoHandStart":
                    {
                        twoHandedMode = true;
                        break;
                    }
                case "TwoHandEnd":
                    {
                        twoHandedMode = false;
                        break;
                    }
                case "PlayerOpenedGameMenu":
                    {
                        menuOpen = true;
                        break;
                    }
                case "PlayerClosedGameMenu":
                    {
                        menuOpen = false;
                        break;
                    }
                case "PlayerShotgunUpgradeGrenadeLauncherState":
                    {
                        GrenadeLauncherStateChange(args.TryGetInt(0));
                        break;
                    }
                case "PlayerGrabbityLockStart":
                    {
                        GrabbityLockStart(args.TryGetInt(0) == 1);
                        break;
                    }
                case "PlayerGrabbityLockStop":
                    {
                        GrabbityLockStop(args.TryGetInt(0) == 1);
                        break;
                    }
                case "PlayerGrabbityPull":
                    {
                        GrabbityGlovePull(args.TryGetInt(0) == 1);
                        break;
                    }
                case "PlayerGrabbedByBarnacle":
                    {
                        BarnacleGrabStart();
                        break;
                    }
                case "PlayerReleasedByBarnacle":
                    {
                        barnacleGrab = false;
                        break;
                    }
                case "PlayerDeath":
                    {
                        PlayerDeath(args.TryGetInt(0));
                        break;
                    }
                case "Reset":
                    {
                        Reset();
                        break;
                    }
                case "PlayerCoughStart":
                    {
                        Cough();
                        break;
                    }
                case "PlayerCoughEnd":
                    {
                        coughing = false;
                        break;
                    }
                case "PlayerCoveredMouth":
                    {
                        coughing = false;
                        break;
                    }
                case "GrabbityGloveCatch":
                    {
                        GrabbityGloveCatch(args.TryGetInt(0) == 1);
                        break;
                    }
                case "PlayerDropAmmoInBackpack":
                    {
                        DropAmmoInBackpack(args.TryGetInt(0) == 1);
                        break;
                    }
                case "PlayerDropResinInBackpack":
                    {
                        DropResinInBackpack(args.TryGetInt(0) == 1);
                        break;
                    }

                case "PlayerRetrievedBackpackClip":
                    {
                        RetrievedBackpackClip(args.TryGetInt(0) == 1);
                        break;
                    }

                case "PlayerStoredItemInItemholder":
                case "HealthPenTeachStorage":
                    {
                        StoredItemInItemholder(args.TryGetInt(0) == 1);
                        break;
                    }

                case "PlayerRemovedItemFromItemholder":
                    {
                        RemovedItemFromItemholder(args.TryGetInt(0) == 1);
                        break;
                    }

                case "PrimaryHandChanged":
                case "SingleControllerModeChanged":
                    {
                        leftHandedMode = args.TryGetInt(0) == 1;
                        break;
                    }
                case "PlayerHeal":
                    {
                        HealthPenUse(args.TryGetFloat(0));
                        break;
                    }
                case "PlayerUsingHealthstation":
                    {
                        HealthStationUse(args.TryGetInt(0) == 1);
                        break;
                    }
                case "ItemPickup":
                    {
                        string item = args.TryGetString(0);

                        if (item == "item_hlvr_crafting_currency_large" || item == "item_hlvr_crafting_currency_small")
                        {
                            RetrievedBackpackResin(args.TryGetInt(1) == 1);
                        }
                        break;
                    }
                case "ItemReleased":
                    {
                        string item = args.TryGetString(0);

                        if (item == "item_hlvr_prop_battery")
                        {
                            ShockOnArm(args.TryGetInt(1) == 1);
                        }
                        break;
                    }
                case "PlayerPistolClipInserted":
                case "PlayerShotgunShellLoaded":
                case "PlayerRapidfireInsertedCapsuleInChamber":
                case "PlayerRapidfireInsertedCapsuleInMagazine":
                    {
                        ClipInserted();
                        break;
                    }
                case "PlayerPistolChamberedRound":
                case "PlayerShotgunLoadedShells":
                case "PlayerRapidfireClosedCasing":
                case "PlayerRapidfireOpenedCasing":
                    {
                        ChamberedRound();
                        break;
                    }

            }
            GC.Collect();
        }
    }
}