namespace TeslasuitAlyx
{
    public enum HLAlyxFeedbackType
    {
        //Attacks on Player's head
        DefaultHead,
        UnarmedHead,
        GunHead,

        //Unarmed Enemies
        EnemyWithoutGunBegin,
        UnarmedBloater = EnemyWithoutGunBegin,
        UnarmedHeadcrab,
        UnarmedHeadcrabArmored,
        UnarmedHeadcrabBlack, //Toxic
        UnarmedHeadcrabFast, //lightning dog
        UnarmedHeadcrabRunner,
        UnarmedFastZombie,
        UnarmedPoisonZombie,
        UnarmedZombie,
        UnarmedZombieBlind,
        UnarmedZombine,
        UnarmedAntlion,
        UnarmedAntlionGuard,
        UnarmedManhack,

        GrabbedByBarnacle,
        EnemyWithoutGunEnd = GrabbedByBarnacle,
        //Grenade/Mine
        ConcussionGrenade,
        BugBaitGrenade,
        FragGrenade,
        SpyGrenade,
        HandGrenade,
        RollerGrenade,
        RollerMine,

        //Enemies with guns
        EnemyWithGunBegin,
        Combine = EnemyWithGunBegin,
        CombineS,

        CombineGantry,


        MetroPolice,
        Sniper,
        Strider,
        Turret,
        FoliageTurret,
        EnemyWithGunEnd = FoliageTurret,

        //On whole body
        EnvironmentExplosion,
        EnvironmentLaser,
        EnvironmentFire,
        EnvironmentSpark,
        EnvironmentPoison,
        EnvironmentRadiation,

        //Uses location
        DamageExplosion,
        DamageLaser,
        DamageFire,
        DamageSpark,

        //Player weapon shoot
        PlayerShootPistol,
        PlayerShootShotgun,
        PlayerShootSMG,

        PlayerShootDefault,

        PlayerGrenadeLaunch,

        PlayerShootPistolLeft,
        PlayerShootShotgunLeft,
        PlayerShootSMGLeft,

        PlayerShootDefaultLeft,

        PlayerGrenadeLaunchLeft,

        FallbackPistol,
        FallbackShotgun,
        FallbackSMG,

        FallbackPistolLeft,
        FallbackShotgunLeft,
        FallbackSMGLeft,

        KickbackPistol,
        KickbackShotgun,
        KickbackSMG,

        KickbackPistolLeft,
        KickbackShotgunLeft,
        KickbackSMGLeft,

        //Special stuff
        HeartBeat,
        HeartBeatFast,

        HealthPenUse,
        HealthStationUse,
        HealthStationUseLeftArm,
        HealthStationUseRightArm,

        BackpackStoreClip,
        BackpackStoreResin,
        BackpackRetrieveClip,
        BackpackRetrieveResin,
        ItemHolderStore,
        ItemHolderRemove,

        BackpackStoreClipLeft,
        BackpackStoreResinLeft,
        BackpackRetrieveClipLeft,
        BackpackRetrieveResinLeft,
        ItemHolderStoreLeft,
        ItemHolderRemoveLeft,

        GravityGloveLockOn,
        GravityGlovePull,
        GravityGloveCatch,

        GravityGloveLockOnLeft,
        GravityGlovePullLeft,
        GravityGloveCatchLeft,

        ClipInserted,
        ChamberedRound,

        ClipInsertedLeft,
        ChamberedRoundLeft,

        Cough,
        CoughHead,

        ShockOnHandLeft,
        ShockOnHandRight,

        DefaultDamage,

        NoFeedback
    }
}
