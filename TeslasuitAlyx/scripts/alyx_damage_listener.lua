require "utils.class"
require "utils.library"
require "utils.vscriptinit"
require "core.coreinit"
require "utils.utilsinit"
require "framework.frameworkinit"
require "framework.entities.entitiesinit"
require "game.globalsystems.timeofday_init"
require "game.gameinit"

local unarmedList = {
    "npc_antlion",
    "npc_barnacle_tongue_tip",
    "npc_antlionguard",
    "npc_clawscanner",
    "npc_concussiongrenade",
    "npc_cscanner",
    "npc_fastzombie",
    "npc_headcrab",
    "npc_headcrab_armored",
    "npc_headcrab_black",
    "npc_headcrab_fast",
    "npc_headcrab_runner",
    "npc_manhack",
    "npc_poisonzombie",
    "npc_zombie",
    "npc_zombie_blind",
    "npc_zombine",
    "xen_foliage_bloater", --armored headcrab zombie
    "env_explosion",
    "env_fire",
    "env_laser",
    "env_physexplosion",
    "env_physimpact",
    "env_spark"
}

local enemyList = {
    "npc_combine",
    "npc_combine_s",
    "npc_combinedropship",
    "npc_combinegunship",
    "npc_heli_nobomb",
    "npc_helicopter",
    "npc_helicoptersensor",
    "npc_metropolice",
    "npc_sniper",
    "npc_strider",
    "npc_hunter",
    "npc_hunter_invincible",
    "npc_turret_ceiling",
    "npc_turret_ceiling_pulse",
    "npc_turret_citizen",
    "npc_turret_floor",
    "xen_foliage_turret",
    "xen_foliage_turret_projectile"
}

local weaponList = {
    "hlvr_weapon_crowbar",
    "hlvr_weapon_crowbar_physics",
    "hlvr_weapon_energygun",
    "hlvr_weapon_rapidfire",
    "hlvr_weapon_shotgun"
}

local propPhysicsList = {}

local twoHandMode = 0
local menuOpen = 1
local lastPlayerHealth = 100
local mouthClosed = 0
local coughing = 0

local function starts_with(str, start)
    return str:sub(1, #start) == start
end

local function has_value(tab, val)
    for index, value in ipairs(tab) do
        if value == val then
            return true
        end
    end

    return false
end

local function getDistance(a, b)
    local x, y, z = a.x - b.x, a.y - b.y, a.z - b.z
    return x * x + y * y + z * z
end

local function LogToConsole(line)
    --SendToServerConsole("teslasuit_log " .. line)
    Msg("[Teslasuit] " .. line)
    --  local file = io.open("teslasuitLog.txt", "a")
    --  io.output(file)
    --  io.write("[Teslasuit] " .. line)
    --  io.close(file)
end

local function LogCmd(...)
    local t = {...}
    local cmd = t[1]
    local line = "[Teslasuit] {" .. cmd
    for i = 2, #t do
        line = line .. ", " .. t[i]
    end
    line = line .. "}\n"
    Msg(line)
end

local function GetAngleOfItem(itemClassName, pos, maxDistance)
    local angles = Entities:GetLocalPlayer():GetAngles()

    local closestPosition = pos
    local closestHandDist = maxDistance * maxDistance + 1

    --    local hmd_avatar = Entities:GetLocalPlayer():GetHMDAvatar()

    --    local leftHand= hmd_avatar:GetVRHand(0)
    --    local leftHandPos = leftHand:GetCenter()
    --    Msg(" -> LeftHand Class: " .. tostring(leftHand:GetClassname()) .. " Pos: " .. tostring(leftHandPos.x) .. " " .. tostring(leftHandPos.y) .. " " .. tostring(leftHandPos.z) .. "\n")
    --    local rightHand= hmd_avatar:GetVRHand(1)
    --    local rightHandPos = rightHand:GetCenter()
    --    Msg(" -> RightHand Class: " .. tostring(rightHand:GetClassname()) .. " Pos: " .. tostring(rightHandPos.x) .. " " .. tostring(rightHandPos.y) .. " " .. tostring(rightHandPos.z) .. "\n")

    local gloveEntities = Entities:FindAllByClassnameWithin(itemClassName, pos, maxDistance)
    for k, v in pairs(gloveEntities) do
        local dist = getDistance(pos, v:GetCenter())

        if dist < closestHandDist then
            closestHandDist = dist
            closestPosition = v:GetCenter()
        end
    end

    if closestHandDist == maxDistance * maxDistance + 1 then
        return -1
    end

    local playerAngle = angles.y

    if playerAngle < 0 then
        playerAngle = (-1 * playerAngle) + 180
    else
        playerAngle = 180 - playerAngle
    end

    local angle =
        (((math.atan2(closestPosition.y - pos.y, closestPosition.x - pos.x) - math.atan2(1, 0)) * (180 / math.pi)) * -1) +
        90
    if (angle < 0) then
        angle = angle + 360
    end

    angle = angle - playerAngle

    angle = 360 - angle

    if angle < 0 then
        angle = angle + 360
    elseif angle > 360 then
        angle = angle - 360
    end

    return angle
end

function OnPlayerHurt(dmginfo)
    --Msg("Player has been hurt. Remaining Health: " .. tostring(dmginfo.health) .. " Damage Done: " .. tostring(damage) .. "\n")

    local center = Entities:GetLocalPlayer():GetCenter()
    --Msg(" -> Player Pos: " .. tostring(center.x) .. " " .. tostring(center.y) .. " " .. tostring(center.z) .. "\n")

    local angles = Entities:GetLocalPlayer():GetAngles()
    --Msg(" -> Player Angles: " .. tostring(angles.x) .. " " .. tostring(angles.y) .. " " .. tostring(angles.z) .. "\n")

    local closestDistance = 2500000000
    local closestEntityClass = "unknown"
    local closestEntityName = "unknown"
    local closestEntityDebugName = "unknown"

    local closestPosition = center

    --  local everyEntities = Entities:FindAllInSphere(center, 3000)
    --  for k,v in pairs(everyEntities) do
    --      local entpos = v:GetCenter()
    --      Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(getDistance(center, entpos)) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")

    --  end
    --  for _k,_v in pairs(dmginfo) do
    --      print("\t" .. _v .. " ---> " .._k.."="..tostring(_v))
    --  end

    local allEntities = Entities:FindAllInSphere(center, 100000)
    for k, v in pairs(allEntities) do
        local entpos = v:GetCenter()
        local dist = getDistance(center, entpos)
        --     if dist <50000 then

        --        Msg(" -> Entity " .. tostring(k) .. ": " .. " Distance: " .. tostring(dist) .. "\tClass: " .. tostring(v:GetClassname()) ..  "", tostring(v:GetName()), tostring(v:GetDebugName()), tostring(v:GetModelName()) .. "\n")

        --      end
        if v:IsAlive() == true then
            if
                has_value(enemyList, v:GetClassname()) or (has_value(unarmedList, v:GetClassname()) and dist < 15000) or
                    (starts_with(v:GetClassname(), "npc_antlion") and dist < 40000) or
                    ((v:GetModelName() == nil or v:GetModelName() == "") and
                        (string.match(v:GetClassname(), "item_hlvr_grenade") or
                            string.match(v:GetClassname(), "npc_grenade") or
                            string.match(v:GetClassname(), "npc_roller") or
                            string.match(v:GetClassname(), "npc_concussiongrenade")) and
                        dist < 50000)
             then
                if dist < closestDistance then
                    closestEntityClass = v:GetClassname()
                    closestEntityName = v:GetName()
                    closestEntityDebugName = v:GetDebugName()
                    closestDistance = dist
                    closestPosition = entpos
                end
            end
        end
    end

    local playerAngle = angles.y

    if playerAngle < 0 then
        playerAngle = (-1 * playerAngle) + 180
    else
        playerAngle = 180 - playerAngle
    end

    local angle =
        (((math.atan2(closestPosition.y - center.y, closestPosition.x - center.x) - math.atan2(1, 0)) * (180 / math.pi)) *
        -1) +
        90
    if (angle < 0) then
        angle = angle + 360
    end

    angle = angle - playerAngle

    angle = 360 - angle

    if angle < 0 then
        angle = angle + 360
    elseif angle > 360 then
        angle = angle - 360
    end

    --Msg("Player angle: " .. tostring(playerAngle) .. " HeadingAngle: " .. tostring(angle) .. "\n")

    LogCmd(
        "PlayerHurt",
        tostring(dmginfo["health"]),
        tostring(closestEntityClass),
        tostring(math.floor(angle)),
        tostring(closestEntityName),
        tostring(closestEntityDebugName)
    )
end

function OnPlayerShootWeapon(shootinfo)
    LogCmd("PlayerShootWeapon", tostring(lastWeapon))
    local pos = Entities:GetLocalPlayer():EyePosition()

    local closestEntityClass = "unknown"
    if lastWeapon == "unknown" then
        -- Check entities to find weapon on hand.

        local closestDistance = 1000000
        local closestPosition = pos

        local allEntities = Entities:FindAllInSphere(pos, 50)
        for k, v in pairs(allEntities) do
            if v:IsAlive() == true then
                local entpos = v:GetCenter()
                if has_value(weaponList, v:GetClassname()) then
                    local dist = getDistance(pos, entpos)
                    if dist < closestDistance then
                        closestEntityClass = v:GetClassname()
                        closestDistance = dist
                        closestPosition = entpos
                    end
                --Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(dist) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")
                end
            end
        end

        lastWeapon = closestEntityClass
    end
end

function OnGrabbityGlovePull(gginfo)
    --Msg("Grabbity Glove Pull\n")
    LogCmd("PlayerGrabbityPull", tostring((gginfo["hand_is_primary"])))
end

function OnGrabbityGloveLockStart(gginfo)
    --Msg("Grabbity Glove Lock Start\n")
    LogCmd("PlayerGrabbityLockStart", tostring((gginfo["hand_is_primary"])))
end

function OnGrabbityGloveLockStop(gginfo)
    --Msg("Grabbity Glove Lock Start\n")
    LogCmd("PlayerGrabbityLockStop", tostring((gginfo["hand_is_primary"])))
end

function OnGrabbedByBarnacle(gbinfo)
    --Msg("Player is grabbed by barnacle\n")
    LogCmd("PlayerGrabbedByBarnacle")
end

function OnReleasedByBarnacle(rbinfo)
    --Msg("Player is released by barnacle\n")
    LogCmd("PlayerReleasedByBarnacle")
end

function OnWeaponSwitch(weaponInfo)
    --Msg("Weapon switched to: " .. tostring(weaponInfo.item) .. "\n")
    lastWeapon = tostring(weaponInfo.item)
end

function OnGameNewMap(newMap)
    Msg("New map loading: " .. newMap.mapname .. "\n")
end

function OnEntityKilled(info)
    if info["entindex_killed"] == 1 then
        LogCmd("PlayerDeath", tostring(info["damagebits"]))
    end
end

function OnItemPickup(param)
    --Msg("Item picked up: " .. tostring(param["item"]) .. "\n")

    if param["item"] == "item_hlvr_crafting_currency_large" or param["item"] == "item_hlvr_crafting_currency_small" then
        local pos = Entities:GetLocalPlayer():EyePosition()

        local angle = GetAngleOfItem("hl_prop_vr_hand", pos, 19)

        if (angle > 45 and angle < 135) or (angle > 225 and angle < 315) then
            local leftShoulder = 1

            if angle > 180 then
                leftShoulder = 0
            end

            LogCmd("ItemPickup", tostring(param["item"]), tostring(leftShoulder))
        end
    end
end

function OnItemReleased(param)
    --Msg("Item released: " .. tostring(param["item"]) .. "\n")

    if param["item"] == "item_hlvr_prop_battery" then
        local center = Entities:GetLocalPlayer():GetCenter()

        local closestDistance = 100000
        local closestPosition = center

        local batteryStationEntities = Entities:FindAllByClassnameWithin("info_particle_system", center, 300)
        for k, v in pairs(batteryStationEntities) do
            if string.match(v:GetDebugName(), "battery_hologram") then
                local dist = getDistance(center, v:GetCenter())
                --Msg(" -> Entity " .. tostring(k) .. ": " .. " Distance: " .. tostring(dist) .. "\tClass: " .. tostring(v:GetClassname()) ..  "", tostring(v:GetName()), tostring(v:GetDebugName()), tostring(v:GetModelName()) .. "\n")

                if dist < closestDistance then
                    closestDistance = dist
                    closestPosition = v:GetCenter()
                end
            end
        end

        if closestDistance ~= 100000 then
            local leftHandUsed = 0
            local hmd_avatar = Entities:GetLocalPlayer():GetHMDAvatar()

            local leftHand = hmd_avatar:GetVRHand(0)
            local leftHandPos = leftHand:GetCenter()

            local rightHand = hmd_avatar:GetVRHand(1)
            local rightHandPos = rightHand:GetCenter()

            local distLeftHandToBatteryStation = getDistance(leftHandPos, closestPosition)
            local distRightHandToBatteryStation = getDistance(rightHandPos, closestPosition)

            --Msg("Left Hand dist to station: " .. tostring(distLeftHandToBatteryStation) .."\n")
            --Msg("Right Hand dist to station: " .. tostring(distRightHandToBatteryStation) .."\n")

            if distRightHandToBatteryStation < 400 or distLeftHandToBatteryStation < 400 then
                if distRightHandToBatteryStation > distLeftHandToBatteryStation then
                    leftHandUsed = 1
                end

                LogCmd("ItemReleased", tostring(param["item"]), tostring(leftHandUsed))
            end
        end
    end
end
function OnGrabbityGloveCatch(param)
    LogCmd("GrabbityGloveCatch", tostring(param["hand_is_primary"]))
end
--function  OnPlayerPickedUpWeaponOffHand (param) LogCmd("PlayerPickedUpWeaponOffHand") end
--function  OnPlayerPickedUpWeaponOffHandCrafting (param) LogCmd("PlayerPickedUpWeaponOffHandCrafting") end
--function  OnPlayerEjectClip (param) LogCmd("PlayerEjectClip") end
--function  OnPlayerArmedGrenade  (param) LogCmd("PlayerArmedGrenade") end
--function  OnPlayerHealthPenPrepare  (param) LogCmd("PlayerHealthPenPrepare") end
--function  OnPlayerHealthPenRetract  (param) LogCmd("PlayerHealthPenRetract") end
function OnPlayerPistolClipInserted(param)
    LogCmd("PlayerPistolClipInserted")
end
--function  OnPlayerPistolEmptyChamber  (param) LogCmd("PlayerPistolEmptyChamber") end
function OnPlayerPistolChamberedRound(param)
    LogCmd("PlayerPistolChamberedRound")
end
--function  OnPlayerPistolSlideLock (param) LogCmd("PlayerPistolSlideLock") end
--function  OnPlayerPistolBoughtLasersight  (param) LogCmd("PlayerPistolBoughtLasersight") end
--function  OnPlayerPistolToggleLasersight  (param) LogCmd("PlayerPistolToggleLasersight") end
--function  OnPlayerPistolBoughtBurstfire (param) LogCmd("PlayerPistolBoughtBurstfire") end
--function  OnPlayerPistolToggleBurstfire (param) LogCmd("PlayerPistolToggleBurstfire") end
--function  OnPlayerPistolPickedupChargedClip (param) LogCmd("PlayerPistolPickedupChargedClip") end
--function  OnPlayerPistolArmedChargedClip  (param) LogCmd("PlayerPistolArmedChargedClip") end
--function  OnPlayerPistolClipChargeEnded (param) LogCmd("PlayerPistolClipChargeEnded") end

function OnPlayerRetrievedBackpackClip(param)
    local pos = Entities:GetLocalPlayer():EyePosition()

    local angle = GetAngleOfItem("hl_prop_vr_hand", pos, 30)

    local leftShoulder = 1

    if angle > 180 then
        leftShoulder = 0
    end

    LogCmd("PlayerRetrievedBackpackClip", tostring(leftShoulder))
end

function OnPlayerDropAmmoInBackpack(param)
    local pos = Entities:GetLocalPlayer():EyePosition()

    local angle = GetAngleOfItem("hl_prop_vr_hand", pos, 30)

    local leftShoulder = 1

    if angle > 180 then
        leftShoulder = 0
    end

    LogCmd("PlayerDropAmmoInBackpack", tostring(leftShoulder))
end
function OnPlayerDropResinInBackpack(param)
    local pos = Entities:GetLocalPlayer():EyePosition()

    local angle = GetAngleOfItem("hl_prop_vr_hand", pos, 30)

    local leftShoulder = 1

    if angle > 180 then
        leftShoulder = 0
    end

    LogCmd("PlayerDropResinInBackpack", tostring(leftShoulder))
end

function OnPlayerHealthPenUsed(hpinfo)
    --Msg("Player has used health pen\n")
    local pos = Entities:GetLocalPlayer():GetCenter()

    local angle = GetAngleOfItem("item_healthvial", pos, 100)

    LogCmd("PlayerHeal", tostring(math.floor(angle)))

    local playerHealth = Entities:GetLocalPlayer():GetHealth()

    if playerHealth ~= nil then
        if playerHealth ~= lastPlayerHealth then
            lastPlayerHealth = playerHealth

            LogCmd("PlayerHealth", tostring(playerHealth))
        end
    end
end

function OnPlayerUsingHealthstation(param)
    local pos = Entities:GetLocalPlayer():GetCenter()

    local leftHandUsed = 0
    local hmd_avatar = Entities:GetLocalPlayer():GetHMDAvatar()

    local leftHand = hmd_avatar:GetVRHand(0)
    local leftHandPos = leftHand:GetCenter()

    local rightHand = hmd_avatar:GetVRHand(1)
    local rightHandPos = rightHand:GetCenter()

    local closestHealthStationPos
    local closestDist = 1000000

    local healthStationEntities = Entities:FindAllByClassnameWithin("item_health_station_charger", pos, 1000)
    for k, v in pairs(healthStationEntities) do
        local dist = getDistance(pos, v:GetCenter())

        if dist < closestDist then
            closestDist = dist
            closestHealthStationPos = v:GetCenter()
        end
    end

    local distLeftHandToHealthStation = getDistance(leftHandPos, closestHealthStationPos)
    local distRightHandToHealthStation = getDistance(rightHandPos, closestHealthStationPos)

    if distRightHandToHealthStation > distLeftHandToHealthStation then
        leftHandUsed = 1
    end

    LogCmd("PlayerUsingHealthstation", tostring(leftHandUsed))

    local playerHealth = Entities:GetLocalPlayer():GetHealth()
    if playerHealth ~= nil then
        lastPlayerHealth = playerHealth
        LogCmd("PlayerHealth", tostring(playerHealth))
    end
end

local function PlayerCoughFunc()
    local pos = Entities:GetLocalPlayer():EyePosition()

    local poisonous = false
    local allEntities = Entities:FindAllInSphere(pos, 50)
    for k, v in pairs(allEntities) do
        local dist = getDistance(pos, v:GetCenter())

        if string.match(v:GetClassname(), "trigger") and string.match(v:GetName(), "cough_volume") then
            --Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(dist) .. " Name: " .. tostring(v:GetName()) .. " DebugName: " .. tostring(v:GetDebugName()) .. "\n")
            if dist < 2100 then
                poisonous = true
            end
        end
    end

    if poisonous == true then
        local newMouthClosed = mouthClosed

        if newMouthClosed == 1 then
            local hmd_avatar = Entities:GetLocalPlayer():GetHMDAvatar()

            local leftHand = hmd_avatar:GetVRHand(0)
            local leftHandPos = leftHand:GetCenter()

            local rightHand = hmd_avatar:GetVRHand(1)
            local rightHandPos = rightHand:GetCenter()

            local distLeftHandToEye = getDistance(leftHandPos, pos)
            local distRightHandToEye = getDistance(rightHandPos, pos)

            if distLeftHandToEye > 100 and distRightHandToEye > 100 then
                newMouthClosed = 0
            end
        end

        if newMouthClosed == 0 then
            local allstuff = Entities:FindAllByClassnameWithin("prop_physics", pos, 10)
            for k, v in pairs(allstuff) do
                if string.match(v:GetModelName(), "respirator") then
                    local dist = getDistance(pos, v:GetCenter())
                    if dist < 17 then
                        newMouthClosed = 1
                    end
                end
            end
        end

        mouthClosed = newMouthClosed

        if mouthClosed == 0 then
            if coughing == 0 then
                coughing = 1
                LogCmd("PlayerCoughStart")
            end
        else
            if coughing == 1 then
                coughing = 0
                LogCmd("PlayerCoughEnd")
            end
        end
    else
        if coughing == 1 then
            coughing = 0
            LogCmd("PlayerCoughEnd")
        end
    end
end

function OnPlayerShotgunShellLoaded(param)
    LogCmd("PlayerShotgunShellLoaded")
end
--function  OnPlayerShotgunStateChanged (param) LogCmd("PlayerShotgunStateChanged", tostring(param["shotgun_state"])) end
function OnPlayerShotgunUpgradeGrenadeLauncherState(param)
    LogCmd("PlayerShotgunUpgradeGrenadeLauncherState", param["state"])
end -- state 0: grenade launched - state 1: grenade attached - state 2: grenade ready -> Check and wait for 2 to 0
function OnPlayerShotgunAutoloaderState(param)
    LogCmd("PlayerShotgunAutoloaderState", tostring(param["state"]))
end
function OnPlayerShotgunAutoloaderShellsAdded(param)
    LogCmd("PlayerShotgunAutoloaderShellsAdded")
end
--function  OnPlayerShotgunUpgradeQuickfire (param) LogCmd("PlayerShotgunUpgradeQuickfire") end
--function  OnPlayerShotgunIsReady  (param) LogCmd("PlayerShotgunIsReady") end
--function  OnPlayerShotgunOpen (param) LogCmd("PlayerShotgunOpen") end
function OnPlayerShotgunLoadedShells(param)
    LogCmd("PlayerShotgunLoadedShells")
end
--function  OnPlayerShotgunUpgradeGrenadeLong (param) LogCmd("PlayerShotgunUpgradeGrenadeLong") end
--function  OnPlayerRapidfireCapsuleChamberEmpty  (param) LogCmd("PlayerRapidfireCapsuleChamberEmpty") end
function OnPlayerRapidfireCycledCapsule(param)
    LogCmd("PlayerRapidfireCycledCapsule")
end
--function  OnPlayerRapidfireMagazineEmpty  (param) LogCmd("PlayerRapidfireMagazineEmpty") end
function OnPlayerRapidfireOpenedCasing(param)
    LogCmd("PlayerRapidfireOpenedCasing")
end
function OnPlayerRapidfireClosedCasing(param)
    LogCmd("PlayerRapidfireClosedCasing")
end
function OnPlayerRapidfireInsertedCapsuleInChamber(param)
    LogCmd("PlayerRapidfireInsertedCapsuleInChamber")
end
function OnPlayerRapidfireInsertedCapsuleInMagazine(param)
    LogCmd("PlayerRapidfireInsertedCapsuleInMagazine")
end
--function  OnPlayerRapidfireUpgradeSelectorCanUse  (param) LogCmd("PlayerRapidfireUpgradeSelectorCanUse") end
--function  OnPlayerRapidfireUpgradeSelectorUsed  (param) LogCmd("PlayerRapidfireUpgradeSelectorUsed") end
--function  OnPlayerRapidfireUpgradeCanCharge (param) LogCmd("PlayerRapidfireUpgradeCanCharge") end
--function  OnPlayerRapidfireUpgradeCanNotCharge  (param) LogCmd("PlayerRapidfireUpgradeCanNotCharge") end
--function  OnPlayerRapidfireUpgradeFullyCharged  (param) LogCmd("PlayerRapidfireUpgradeFullyCharged") end
--function  OnPlayerRapidfireUpgradeNotFullyCharged (param) LogCmd("PlayerRapidfireUpgradeNotFullyCharged") end
function OnPlayerRapidfireUpgradeFired(param)
    LogCmd("PlayerRapidfireUpgradeFired")
end
--function  OnPlayerRapidfireEnergyBallCanCharge  (param) LogCmd("PlayerRapidfireEnergyBallCanCharge") end
--function  OnPlayerRapidfireEnergyBallFullyCharged (param) LogCmd("PlayerRapidfireEnergyBallFullyCharged") end
--function  OnPlayerRapidfireEnergyBallNotFullyCharged  (param) LogCmd("PlayerRapidfireEnergyBallNotFullyCharged") end
--function  OnPlayerRapidfireEnergyBallCanPickUp  (param) LogCmd("PlayerRapidfireEnergyBallCanPickUp") end
--function  OnPlayerRapidfireEnergyBallPickedUp (param) LogCmd("PlayerRapidfireEnergyBallPickedUp") end
--function  OnPlayerRapidfireStunGrenadeReady (param) LogCmd("PlayerRapidfireStunGrenadeReady") end
--function  OnPlayerRapidfireStunGrenadeNotReady  (param) LogCmd("PlayerRapidfireStunGrenadeNotReady") end
--function  OnPlayerRapidfireStunGrenadePickedUp  (param) LogCmd("PlayerRapidfireStunGrenadePickedUp") end
--function  OnPlayerRapidfireExplodeButtonReady (param) LogCmd("PlayerRapidfireExplodeButtonReady") end
--function  OnPlayerRapidfireExplodeButtonNotReady  (param) LogCmd("PlayerRapidfireExplodeButtonNotReady") end
function OnPlayerRapidfireExplodeButtonPressed(param)
    LogCmd("PlayerRapidfireExplodeButtonPressed")
end
function OnPlayerStarted2hLevitate(param)
    LogCmd("PlayerStarted2hLevitate")
end

local function GetItemHolderCloseToHand()
    local leftHolder = 0
    local hmd_avatar = Entities:GetLocalPlayer():GetHMDAvatar()

    local leftHand = hmd_avatar:GetVRHand(0)
    local leftHandPos = leftHand:GetCenter()
    --Msg(" -> LeftHand Class: " .. tostring(leftHand:GetClassname()) .. " Pos: " .. tostring(leftHandPos.x) .. " " .. tostring(leftHandPos.y) .. " " .. tostring(leftHandPos.z) .. "\n")
    local rightHand = hmd_avatar:GetVRHand(1)
    local rightHandPos = rightHand:GetCenter()
    --Msg(" -> RightHand Class: " .. tostring(rightHand:GetClassname()) .. " Pos: " .. tostring(rightHandPos.x) .. " " .. tostring(rightHandPos.y) .. " " .. tostring(rightHandPos.z) .. "\n")

    local leftItemHolderPos
    local rightItemHolderPos
    local holderEntities = Entities:FindAllByClassname("hlvr_hand_item_holder")
    for k, v in pairs(holderEntities) do
        local itemHolderPos = v:GetCenter()
        if v:GetDebugName() == "item_holder_l" then
            leftItemHolderPos = v:GetCenter()
        elseif v:GetDebugName() == "item_holder_r" then
            rightItemHolderPos = v:GetCenter()
        end
    end

    local distLeftHandToRightHolder = getDistance(leftHandPos, rightItemHolderPos)
    local distRightHandToLeftHolder = getDistance(rightHandPos, leftItemHolderPos)

    if distRightHandToLeftHolder < distLeftHandToRightHolder then
        leftHolder = 1
    end

    return leftHolder
end

function OnPlayerStoredItemInItemholder(param)
    local leftHolder = GetItemHolderCloseToHand()

    LogCmd("PlayerStoredItemInItemholder", tostring(leftHolder))
end

function OnPlayerRemovedItemFromItemholder(param)
    local leftHolder = GetItemHolderCloseToHand()

    LogCmd("PlayerRemovedItemFromItemholder", tostring(leftHolder))
end

--function  OnPlayerAttachedFlashlight  (param) LogCmd("PlayerAttachedFlashlight") end
function OnHealthPenTeachStorage(param)
    LogCmd("HealthPenTeachStorage")
end
function OnHealthVialTeachStorage(param)
    LogCmd("HealthVialTeachStorage")
end
function OnPlayerCoveredMouth(param)
    mouthClosed = 1
    coughing = 0
    LogCmd("PlayerCoveredMouth")
end
--function  OnPlayerUpgradedWeapon  (param) LogCmd("PlayerUpgradedWeapon") end
function OnTripmineHackStarted(param)
    LogCmd("TripmineHackStarted")
end
function OnTripmineHacked(param)
    LogCmd("TripmineHacked")
end
function OnPrimaryHandChanged(param)
    LogCmd("PrimaryHandChanged", tostring((param["is_primary_left"])) .. "")
end
function OnSingleControllerModeChanged(param)
    LogCmd("SingleControllerModeChanged", tostring((param["is_primary_left"])) .. "")
end
function OnMovementHandChanged(param)
    LogCmd("MovementHandChanged")
end
function OnCombineTankMovedByPlayer(param)
    LogCmd("CombineTankMovedByPlayer")
end
function OnPlayerContinuousJumpFinish(param)
    LogCmd("PlayerContinuousJumpFinish")
end
function OnPlayerContinuousMantleFinish(param)
    LogCmd("PlayerContinuousMantleFinish")
end
function OnPlayerGrabbedLadder(param)
    --  local center = Entities:GetLocalPlayer():GetCenter()
    --  local everyEntities = Entities:FindAllInSphere(center, 400)
    --  for k,v in pairs(everyEntities) do
    --      local entpos = v:GetCenter()
    --      Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " DebugName: " .. tostring(v:GetDebugName()) .." Distance: " .. tostring(getDistance(center, entpos)) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")
    --  end
    --  LogCmd("PlayerGrabbedLadder")
end

function OnTwoHandStart(param)
    if twoHandMode == 0 then
        twoHandMode = 1
        LogCmd("TwoHandStart")
    end
    mouthClosed = 0

    if Entities:GetLocalPlayer() ~= nil then
        local playerHealth = Entities:GetLocalPlayer():GetHealth()
        if playerHealth ~= nil then
            if playerHealth ~= lastPlayerHealth then
                lastPlayerHealth = playerHealth
                if lastPlayerHealth <= 30 then
                    LogCmd("PlayerHealth", tostring(playerHealth))
                end
            end
        end
    end
end
function OnTwoHandEnd(param)
    if twoHandMode == 1 then
        twoHandMode = 0
        LogCmd("TwoHandEnd")
    end
    --  local center = Entities:GetLocalPlayer():GetCenter()

    --  local allEntities = Entities:FindAllInSphere(center, 2000)
    --  for k,v in pairs(allEntities) do
    --      local entpos = v:GetCenter()
    --      local dist = getDistance(center, entpos)

    --      if dist <5000 then
    --        Msg(" -,-,-> Entity " .. tostring(k) .. ": " .. " Distance: " .. tostring(dist) .. "\tClass: " .. tostring(v:GetClassname()) ..  "", tostring(v:GetName()), tostring(v:GetDebugName()) ..  "", tostring(v:GetModelName()) .. "\n")

    --      end
    --  end

    PlayerCoughFunc()

    if lastWeapon == "unknown" then
        -- Check entities to find weapon on hand.
        local closestEntityClass = "unknown"

        local pos = Entities:GetLocalPlayer():GetCenter()
        local closestDistance = 1000000
        local closestPosition = pos

        local allEntities = Entities:FindAllInSphere(pos, 1000)
        for k, v in pairs(allEntities) do
            if v:IsAlive() == true then
                local entpos = v:GetCenter()
                if has_value(weaponList, v:GetClassname()) then
                    local dist = getDistance(pos, entpos)
                    if dist < closestDistance then
                        closestEntityClass = v:GetClassname()
                        closestDistance = dist
                        closestPosition = entpos
                    end
                --Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(dist) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")
                end
            end
        end

        lastWeapon = closestEntityClass
    end

    if Entities:GetLocalPlayer() ~= nil then
        local playerHealth = Entities:GetLocalPlayer():GetHealth()
        if playerHealth ~= nil then
            if playerHealth ~= lastPlayerHealth then
                lastPlayerHealth = playerHealth
                if lastPlayerHealth <= 30 then
                    LogCmd("PlayerHealth", tostring(playerHealth))
                end
            end
        end
    end
end

function OnPlayerOpenedGameMenu(param)
    if menuOpen == 0 then
        menuOpen = 1
        LogCmd("PlayerOpenedGameMenu")
    end
end

function OnPlayerClosedGameMenu(param)
    if menuOpen ~= 0 then
        menuOpen = 0

        LogCmd("PlayerClosedGameMenu")
    end
end

ListenToGameEvent(
    "player_spawn",
    function(info)
        --    local included = DoIncludeScript("user/playerEntity.lua", Entities:GetLocalPlayer():GetOrCreatePrivateScriptScope())
        --    local included2 = DoIncludeScript("user/playerEntity.lua", Entities:GetLocalPlayer():GetOrCreatePublicScriptScope())
        --    local ent_table = {
        --        origin = Entities:GetLocalPlayer():EyePosition(),
        --        vscripts = "user/playerEntity.lua"
        --    }
        --    ent = SpawnEntityFromTableSynchronous("prop_physics", ent_table)
        --    if ent ~= nil then
        --      ent:SetParent(Entities:GetLocalPlayer(), "")
        --    end

        Msg("------> Player spawned: " .. tostring(info["userid"]) .. "\n")
        LogCmd("Reset")
    end,
    nil
)

function OnPlayerTeleportStart(param)
    PlayerCoughFunc()

    if Entities:GetLocalPlayer() ~= nil then
        local playerHealth = Entities:GetLocalPlayer():GetHealth()
        if playerHealth ~= nil then
            if playerHealth ~= lastPlayerHealth then
                lastPlayerHealth = playerHealth
                LogCmd("PlayerHealth", tostring(playerHealth))
            end
        end
    end

    if lastWeapon == "unknown" then
        -- Check entities to find weapon on hand.
        local closestEntityClass = "unknown"

        local pos = Entities:GetLocalPlayer():GetCenter()
        local closestDistance = 1000000
        local closestPosition = pos

        local allEntities = Entities:FindAllInSphere(pos, 1000)
        for k, v in pairs(allEntities) do
            if v:IsAlive() == true then
                local entpos = v:GetCenter()
                if has_value(weaponList, v:GetClassname()) then
                    local dist = getDistance(pos, entpos)
                    if dist < closestDistance then
                        closestEntityClass = v:GetClassname()
                        closestDistance = dist
                        closestPosition = entpos
                    end
                --Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(dist) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")
                end
            end
        end

        lastWeapon = closestEntityClass
    end
end

function OnPlayerTeleportFinish(param)
    PlayerCoughFunc()

    if Entities:GetLocalPlayer() ~= nil then
        local playerHealth = Entities:GetLocalPlayer():GetHealth()
        if playerHealth ~= nil then
            if playerHealth ~= lastPlayerHealth then
                lastPlayerHealth = playerHealth
                LogCmd("PlayerHealth", tostring(playerHealth))
            end
        end
    end

    if lastWeapon == "unknown" then
        -- Check entities to find weapon on hand.
        local closestEntityClass = "unknown"

        local pos = Entities:GetLocalPlayer():GetCenter()
        local closestDistance = 1000000
        local closestPosition = pos

        local allEntities = Entities:FindAllInSphere(pos, 1000)
        for k, v in pairs(allEntities) do
            if v:IsAlive() == true then
                local entpos = v:GetCenter()
                if has_value(weaponList, v:GetClassname()) then
                    local dist = getDistance(pos, entpos)
                    if dist < closestDistance then
                        closestEntityClass = v:GetClassname()
                        closestDistance = dist
                        closestPosition = entpos
                    end
                --Msg(" -> Entity " .. tostring(k) .. ": " .. " Class: " .. tostring(v:GetClassname()) .. " Distance: " .. tostring(dist) .. " Pos: " .. tostring(entpos.x) .. " " .. tostring(entpos.y) .. " " .. tostring(entpos.z) .. "\n")
                end
            end
        end

        lastWeapon = closestEntityClass
    end
end

function TryStopListeningToGameEvent(event_handle)
  if event_handle ~=nil then
    TryStopListeningToGameEvent(event_handle)
  end
end

function TryListenToGameEvent(event_name, func, context, current_handle)
  if current_handle == nil then
    current_handle = ListenToGameEvent(event_name, func, context)
  end
  return current_handle
end

if IsServer() then
    -- Stop listening to the events if we're already listening to them (this is so we can safely reload the script)
    TryStopListeningToGameEvent(ongamenewmap_handle)
    TryStopListeningToGameEvent(onplayershootweapon_handle)

    TryStopListeningToGameEvent(onplayerhurt_handle)

    TryStopListeningToGameEvent(onplayergrabbityglovepull_handle)
    TryStopListeningToGameEvent(onplayergrabbedbybarnacle_handle)
    TryStopListeningToGameEvent(onplayerreleasedbybarnacle_handle)
    TryStopListeningToGameEvent(onplayerhealthpenused_handle)
    TryStopListeningToGameEvent(onweaponswitch_handle)
    TryStopListeningToGameEvent(ongamenewmap_handle)
    TryStopListeningToGameEvent(onentity_killed_handle)
    TryStopListeningToGameEvent(onentity_hurt_handle)

    TryStopListeningToGameEvent(onplayer_teleport_start_handle)
    TryStopListeningToGameEvent(onplayer_teleport_finish_handle)

    TryStopListeningToGameEvent(onitem_pickup_handle)
    TryStopListeningToGameEvent(onitem_released_handle)
    TryStopListeningToGameEvent(ongrabbity_glove_catch_handle)
    TryStopListeningToGameEvent(onplayer_picked_up_weapon_off_hand_handle)
    TryStopListeningToGameEvent(onplayer_picked_up_weapon_off_hand_crafting_handle)
    TryStopListeningToGameEvent(onplayer_eject_clip_handle)
    TryStopListeningToGameEvent(onplayer_armed_grenade_handle)
    TryStopListeningToGameEvent(onplayer_health_pen_prepare_handle)
    TryStopListeningToGameEvent(onplayer_health_pen_retract_handle)
    TryStopListeningToGameEvent(onplayer_pistol_clip_inserted_handle)
    TryStopListeningToGameEvent(onplayer_pistol_empty_chamber_handle)
    TryStopListeningToGameEvent(onplayer_pistol_chambered_round_handle)
    TryStopListeningToGameEvent(onplayer_pistol_slide_lock_handle)
    TryStopListeningToGameEvent(onplayer_pistol_bought_lasersight_handle)
    TryStopListeningToGameEvent(onplayer_pistol_toggle_lasersight_handle)
    TryStopListeningToGameEvent(onplayer_pistol_bought_burstfire_handle)
    TryStopListeningToGameEvent(onplayer_pistol_toggle_burstfire_handle)
    TryStopListeningToGameEvent(onplayer_pistol_pickedup_charged_clip_handle)
    TryStopListeningToGameEvent(onplayer_pistol_armed_charged_clip_handle)
    TryStopListeningToGameEvent(onplayer_pistol_clip_charge_ended_handle)
    TryStopListeningToGameEvent(onplayer_retrieved_backpack_clip_handle)
    TryStopListeningToGameEvent(onplayer_drop_ammo_in_backpack_handle)
    TryStopListeningToGameEvent(onplayer_drop_resin_in_backpack_handle)
    TryStopListeningToGameEvent(onplayer_using_healthstation_handle)
    TryStopListeningToGameEvent(onhealth_station_open_handle)
    TryStopListeningToGameEvent(onplayer_looking_at_wristhud_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_shell_loaded_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_state_changed_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_upgrade_grenade_launcher_state_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_autoloader_state_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_autoloader_shells_added_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_upgrade_quickfire_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_is_ready_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_open_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_loaded_shells_handle)
    TryStopListeningToGameEvent(onplayer_shotgun_upgrade_grenade_long_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_capsule_chamber_empty_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_cycled_capsule_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_magazine_empty_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_opened_casing_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_closed_casing_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_inserted_capsule_in_chamber_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_inserted_capsule_in_magazine_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_selector_can_use_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_selector_used_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_can_charge_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_can_not_charge_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_fully_charged_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_not_fully_charged_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_upgrade_fired_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_energy_ball_can_charge_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_energy_ball_fully_charged_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_energy_ball_not_fully_charged_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_energy_ball_can_pick_up_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_energy_ball_picked_up_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_stun_grenade_ready_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_stun_grenade_not_ready_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_stun_grenade_picked_up_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_explode_button_ready_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_explode_button_not_ready_handle)
    TryStopListeningToGameEvent(onplayer_rapidfire_explode_button_pressed_handle)
    TryStopListeningToGameEvent(onplayer_started_2h_levitate_handle)
    TryStopListeningToGameEvent(onplayer_stored_item_in_itemholder_handle)
    TryStopListeningToGameEvent(onplayer_removed_item_from_itemholder_handle)
    TryStopListeningToGameEvent(onplayer_attached_flashlight_handle)
    TryStopListeningToGameEvent(onhealth_pen_teach_storage_handle)
    TryStopListeningToGameEvent(onhealth_vial_teach_storage_handle)
    TryStopListeningToGameEvent(onplayer_pickedup_storable_clip_handle)
    TryStopListeningToGameEvent(onplayer_pickedup_insertable_clip_handle)
    TryStopListeningToGameEvent(onplayer_covered_mouth_handle)
    TryStopListeningToGameEvent(onplayer_upgraded_weapon_handle)
    TryStopListeningToGameEvent(ontripmine_hack_started_handle)
    TryStopListeningToGameEvent(ontripmine_hacked_handle)
    TryStopListeningToGameEvent(onprimary_hand_changed_handle)
    TryStopListeningToGameEvent(onsingle_controller_mode_changed_handle)
    TryStopListeningToGameEvent(onmovement_hand_changed_handle)
    TryStopListeningToGameEvent(oncombine_tank_moved_by_player_handle)
    TryStopListeningToGameEvent(onplayer_continuous_jump_finish_handle)
    TryStopListeningToGameEvent(onplayer_continuous_mantle_finish_handle)
    TryStopListeningToGameEvent(onplayer_grabbed_ladder_handle)

    TryStopListeningToGameEvent(ontwo_hand_pistol_grab_start_handle)
    TryStopListeningToGameEvent(ontwo_hand_pistol_grab_end_handle)
    TryStopListeningToGameEvent(ontwo_hand_rapidfire_grab_start_handle)
    TryStopListeningToGameEvent(ontwo_hand_rapidfire_grab_end_handle)
    TryStopListeningToGameEvent(ontwo_hand_shotgun_grab_start_handle)
    TryStopListeningToGameEvent(ontwo_hand_shotgun_grab_end_handle)

    TryListenToGameEvent("game_newmap", OnGameNewMap, nil, ongamenewmap_handle)
    TryListenToGameEvent("player_shoot_weapon", OnPlayerShootWeapon, nil, onplayershootweapon_handle)
    TryListenToGameEvent("player_hurt", OnPlayerHurt, nil, onplayerhurt_handle)
    TryListenToGameEvent("grabbity_glove_pull", OnGrabbityGlovePull, nil, onplayergrabbityglovepull_handle)
    TryListenToGameEvent("grabbity_glove_locked_on_start", OnGrabbityGloveLockStart, nil, onplayergrabbityglovelockstart_handle)
    TryListenToGameEvent("grabbity_glove_locked_on_stop", OnGrabbityGloveLockStop, nil, onplayergrabbityglovelockstop_handle)
    TryListenToGameEvent("player_grabbed_by_barnacle", OnGrabbedByBarnacle, nil, onplayergrabbedbybarnacle_handle)
    TryListenToGameEvent("player_released_by_barnacle", OnReleasedByBarnacle, nil, onplayerreleasedbybarnacle_handle)
    TryListenToGameEvent("player_health_pen_used", OnPlayerHealthPenUsed, nil, onplayerhealthpenused_handle)
    TryListenToGameEvent("weapon_switch", OnWeaponSwitch, nil, onweaponswitch_handle)
    TryListenToGameEvent("entity_killed", OnEntityKilled, nil, onentity_killed_handle)

    TryListenToGameEvent("player_teleport_start", OnPlayerTeleportStart, nil, onplayer_teleport_start_handle)
    TryListenToGameEvent("player_teleport_finish", OnPlayerTeleportFinish, nil, onplayer_teleport_finish_handle)

    TryListenToGameEvent("item_pickup", OnItemPickup, nil, onitem_pickup_handle)
    TryListenToGameEvent("item_released", OnItemReleased, nil, onitem_released_handle)
    TryListenToGameEvent("weapon_switch", OnWeaponSwitch, nil, onweapon_switch_handle)
    TryListenToGameEvent("grabbity_glove_pull", OnGrabbityGlovePull, nil, ongrabbity_glove_pull_handle)
    TryListenToGameEvent("grabbity_glove_catch", OnGrabbityGloveCatch, nil, ongrabbity_glove_catch_handle)
    TryListenToGameEvent("grabbity_glove_locked_on_start", OnGrabbityGloveLockedOnStart, nil, ongrabbity_glove_locked_on_start_handle)
    TryListenToGameEvent("grabbity_glove_locked_on_stop", OnGrabbityGloveLockedOnStop, nil, ongrabbity_glove_locked_on_stop_handle)
    --if onplayer_gestured_handle == nil then onplayer_gestured_handle=TryListenToGameEvent("player_gestured",OnPlayerGestured, nil) end
    TryListenToGameEvent("player_picked_up_weapon_off_hand", OnPlayerPickedUpWeaponOffHand, nil, onplayer_picked_up_weapon_off_hand_handle)
    TryListenToGameEvent("player_picked_up_weapon_off_hand_crafting", OnPlayerPickedUpWeaponOffHandCrafting, nil, onplayer_picked_up_weapon_off_hand_crafting_handle)
    --if onplayer_eject_clip_handle == nil then onplayer_eject_clip_handle=TryListenToGameEvent("player_eject_clip",OnPlayerEjectClip, nil) end
    --if onplayer_armed_grenade_handle == nil then onplayer_armed_grenade_handle=TryListenToGameEvent("player_armed_grenade",OnPlayerArmedGrenade, nil) end
    --if onplayer_health_pen_prepare_handle == nil then onplayer_health_pen_prepare_handle=TryListenToGameEvent("player_health_pen_prepare",OnPlayerHealthPenPrepare, nil) end
    --if onplayer_health_pen_retract_handle == nil then onplayer_health_pen_retract_handle=TryListenToGameEvent("player_health_pen_retract",OnPlayerHealthPenRetract, nil) end
    TryListenToGameEvent("player_health_pen_used", OnPlayerHealthPenUsed, nil, onplayer_health_pen_used_handle)
    TryListenToGameEvent("player_pistol_clip_inserted", OnPlayerPistolClipInserted, nil, onplayer_pistol_clip_inserted_handle)
    --if onplayer_pistol_empty_chamber_handle == nil then onplayer_pistol_empty_chamber_handle=TryListenToGameEvent("player_pistol_empty_chamber",OnPlayerPistolEmptyChamber, nil) end
    TryListenToGameEvent("player_pistol_chambered_round", OnPlayerPistolChamberedRound, nil, onplayer_pistol_chambered_round_handle)
    --if onplayer_pistol_slide_lock_handle == nil then onplayer_pistol_slide_lock_handle=TryListenToGameEvent("player_pistol_slide_lock",OnPlayerPistolSlideLock, nil) end
    --if onplayer_pistol_bought_lasersight_handle == nil then onplayer_pistol_bought_lasersight_handle=TryListenToGameEvent("player_pistol_bought_lasersight",OnPlayerPistolBoughtLasersight, nil) end
    --if onplayer_pistol_toggle_lasersight_handle == nil then onplayer_pistol_toggle_lasersight_handle=TryListenToGameEvent("player_pistol_toggle_lasersight",OnPlayerPistolToggleLasersight, nil) end
    --if onplayer_pistol_bought_burstfire_handle == nil then onplayer_pistol_bought_burstfire_handle=TryListenToGameEvent("player_pistol_bought_burstfire",OnPlayerPistolBoughtBurstfire, nil) end
    --if onplayer_pistol_toggle_burstfire_handle == nil then onplayer_pistol_toggle_burstfire_handle=TryListenToGameEvent("player_pistol_toggle_burstfire",OnPlayerPistolToggleBurstfire, nil) end
    --if onplayer_pistol_pickedup_charged_clip_handle == nil then onplayer_pistol_pickedup_charged_clip_handle=TryListenToGameEvent("player_pistol_pickedup_charged_clip",OnPlayerPistolPickedupChargedClip, nil) end
    --if onplayer_pistol_armed_charged_clip_handle == nil then onplayer_pistol_armed_charged_clip_handle=TryListenToGameEvent("player_pistol_armed_charged_clip",OnPlayerPistolArmedChargedClip, nil) end
    --if onplayer_pistol_clip_charge_ended_handle == nil then onplayer_pistol_clip_charge_ended_handle=TryListenToGameEvent("player_pistol_clip_charge_ended",OnPlayerPistolClipChargeEnded, nil) end
    TryListenToGameEvent("player_retrieved_backpack_clip", OnPlayerRetrievedBackpackClip, nil, onplayer_retrieved_backpack_clip_handle)
    TryListenToGameEvent("player_drop_ammo_in_backpack", OnPlayerDropAmmoInBackpack, nil, onplayer_drop_ammo_in_backpack_handle)
    TryListenToGameEvent("player_drop_resin_in_backpack", OnPlayerDropResinInBackpack, nil, onplayer_drop_resin_in_backpack_handle)
    TryListenToGameEvent("player_using_healthstation", OnPlayerUsingHealthstation, nil, onplayer_using_healthstation_handle)
    --if onhealth_station_open_handle == nil then onhealth_station_open_handle=TryListenToGameEvent("health_station_open",OnHealthStationOpen, nil) end
    TryListenToGameEvent("player_shotgun_shell_loaded", OnPlayerShotgunShellLoaded, nil, onplayer_shotgun_shell_loaded_handle)
    --if onplayer_shotgun_state_changed_handle == nil then onplayer_shotgun_state_changed_handle=TryListenToGameEvent("player_shotgun_state_changed",OnPlayerShotgunStateChanged, nil) end
    TryListenToGameEvent("player_shotgun_upgrade_grenade_launcher_state", OnPlayerShotgunUpgradeGrenadeLauncherState, nil, onplayer_shotgun_upgrade_grenade_launcher_state_handle)
    TryListenToGameEvent("player_shotgun_autoloader_state", OnPlayerShotgunAutoloaderState, nil, onplayer_shotgun_autoloader_state_handle)
    TryListenToGameEvent("player_shotgun_autoloader_shells_added", OnPlayerShotgunAutoloaderShellsAdded, nil, onplayer_shotgun_autoloader_shells_added_handle)
    TryListenToGameEvent("player_shotgun_upgrade_quickfire", OnPlayerShotgunUpgradeQuickfire, nil, onplayer_shotgun_upgrade_quickfire_handle)
    --if onplayer_shotgun_is_ready_handle == nil then onplayer_shotgun_is_ready_handle=TryListenToGameEvent("player_shotgun_is_ready",OnPlayerShotgunIsReady, nil) end
    --if onplayer_shotgun_open_handle == nil then onplayer_shotgun_open_handle=TryListenToGameEvent("player_shotgun_open",OnPlayerShotgunOpen, nil) end
    TryListenToGameEvent("player_shotgun_loaded_shells", OnPlayerShotgunLoadedShells, nil, onplayer_shotgun_loaded_shells_handle)
    --if onplayer_shotgun_upgrade_grenade_long_handle == nil then onplayer_shotgun_upgrade_grenade_long_handle=TryListenToGameEvent("player_shotgun_upgrade_grenade_long",OnPlayerShotgunUpgradeGrenadeLong, nil) end
    --if onplayer_rapidfire_capsule_chamber_empty_handle == nil then onplayer_rapidfire_capsule_chamber_empty_handle=TryListenToGameEvent("player_rapidfire_capsule_chamber_empty",OnPlayerRapidfireCapsuleChamberEmpty, nil) end
    --if onplayer_rapidfire_cycled_capsule_handle == nil then onplayer_rapidfire_cycled_capsule_handle=TryListenToGameEvent("player_rapidfire_cycled_capsule",OnPlayerRapidfireCycledCapsule, nil) end
    --if onplayer_rapidfire_magazine_empty_handle == nil then onplayer_rapidfire_magazine_empty_handle=TryListenToGameEvent("player_rapidfire_magazine_empty",OnPlayerRapidfireMagazineEmpty, nil) end
    TryListenToGameEvent("player_rapidfire_opened_casing", OnPlayerRapidfireOpenedCasing, nil, onplayer_rapidfire_opened_casing_handle)
    TryListenToGameEvent("player_rapidfire_closed_casing", OnPlayerRapidfireClosedCasing, nil, onplayer_rapidfire_closed_casing_handle)
    TryListenToGameEvent("player_rapidfire_inserted_capsule_in_chamber", OnPlayerRapidfireInsertedCapsuleInChamber, nil, onplayer_rapidfire_inserted_capsule_in_chamber_handle)
    TryListenToGameEvent("player_rapidfire_inserted_capsule_in_magazine", OnPlayerRapidfireInsertedCapsuleInMagazine, nil, onplayer_rapidfire_inserted_capsule_in_magazine_handle)
    --if onplayer_rapidfire_upgrade_selector_can_use_handle == nil then onplayer_rapidfire_upgrade_selector_can_use_handle=TryListenToGameEvent("player_rapidfire_upgrade_selector_can_use",OnPlayerRapidfireUpgradeSelectorCanUse, nil) end
    --if onplayer_rapidfire_upgrade_selector_used_handle == nil then onplayer_rapidfire_upgrade_selector_used_handle=TryListenToGameEvent("player_rapidfire_upgrade_selector_used",OnPlayerRapidfireUpgradeSelectorUsed, nil) end
    --if onplayer_rapidfire_upgrade_can_charge_handle == nil then onplayer_rapidfire_upgrade_can_charge_handle=TryListenToGameEvent("player_rapidfire_upgrade_can_charge",OnPlayerRapidfireUpgradeCanCharge, nil) end
    --if onplayer_rapidfire_upgrade_can_not_charge_handle == nil then onplayer_rapidfire_upgrade_can_not_charge_handle=TryListenToGameEvent("player_rapidfire_upgrade_can_not_charge",OnPlayerRapidfireUpgradeCanNotCharge, nil) end
    --if onplayer_rapidfire_upgrade_fully_charged_handle == nil then onplayer_rapidfire_upgrade_fully_charged_handle=TryListenToGameEvent("player_rapidfire_upgrade_fully_charged",OnPlayerRapidfireUpgradeFullyCharged, nil) end
    --if onplayer_rapidfire_upgrade_not_fully_charged_handle == nil then onplayer_rapidfire_upgrade_not_fully_charged_handle=TryListenToGameEvent("player_rapidfire_upgrade_not_fully_charged",OnPlayerRapidfireUpgradeNotFullyCharged, nil) end
    TryListenToGameEvent("player_rapidfire_upgrade_fired", OnPlayerRapidfireUpgradeFired, nil, onplayer_rapidfire_upgrade_fired_handle)
    --if onplayer_rapidfire_energy_ball_can_charge_handle == nil then onplayer_rapidfire_energy_ball_can_charge_handle=TryListenToGameEvent("player_rapidfire_energy_ball_can_charge",OnPlayerRapidfireEnergyBallCanCharge, nil) end
    --if onplayer_rapidfire_energy_ball_fully_charged_handle == nil then onplayer_rapidfire_energy_ball_fully_charged_handle=TryListenToGameEvent("player_rapidfire_energy_ball_fully_charged",OnPlayerRapidfireEnergyBallFullyCharged, nil) end
    --if onplayer_rapidfire_energy_ball_not_fully_charged_handle == nil then onplayer_rapidfire_energy_ball_not_fully_charged_handle=TryListenToGameEvent("player_rapidfire_energy_ball_not_fully_charged",OnPlayerRapidfireEnergyBallNotFullyCharged, nil) end
    --if onplayer_rapidfire_energy_ball_can_pick_up_handle == nil then onplayer_rapidfire_energy_ball_can_pick_up_handle=TryListenToGameEvent("player_rapidfire_energy_ball_can_pick_up",OnPlayerRapidfireEnergyBallCanPickUp, nil) end
    --if onplayer_rapidfire_energy_ball_picked_up_handle == nil then onplayer_rapidfire_energy_ball_picked_up_handle=TryListenToGameEvent("player_rapidfire_energy_ball_picked_up",OnPlayerRapidfireEnergyBallPickedUp, nil) end
    --if onplayer_rapidfire_stun_grenade_ready_handle == nil then onplayer_rapidfire_stun_grenade_ready_handle=TryListenToGameEvent("player_rapidfire_stun_grenade_ready",OnPlayerRapidfireStunGrenadeReady, nil) end
    --if onplayer_rapidfire_stun_grenade_not_ready_handle == nil then onplayer_rapidfire_stun_grenade_not_ready_handle=TryListenToGameEvent("player_rapidfire_stun_grenade_not_ready",OnPlayerRapidfireStunGrenadeNotReady, nil) end
    --if onplayer_rapidfire_stun_grenade_picked_up_handle == nil then onplayer_rapidfire_stun_grenade_picked_up_handle=TryListenToGameEvent("player_rapidfire_stun_grenade_picked_up",OnPlayerRapidfireStunGrenadePickedUp, nil) end
    --if onplayer_rapidfire_explode_button_ready_handle == nil then onplayer_rapidfire_explode_button_ready_handle=TryListenToGameEvent("player_rapidfire_explode_button_ready",OnPlayerRapidfireExplodeButtonReady, nil) end
    --if onplayer_rapidfire_explode_button_not_ready_handle == nil then onplayer_rapidfire_explode_button_not_ready_handle=TryListenToGameEvent("player_rapidfire_explode_button_not_ready",OnPlayerRapidfireExplodeButtonNotReady, nil) end
    TryListenToGameEvent("player_rapidfire_explode_button_pressed", OnPlayerRapidfireExplodeButtonPressed, nil, onplayer_rapidfire_explode_button_pressed_handle)
    TryListenToGameEvent("player_started_2h_levitate", OnPlayerStarted2hLevitate, nil, onplayer_started_2h_levitate_handle)
    TryListenToGameEvent("player_stored_item_in_itemholder", OnPlayerStoredItemInItemholder, nil, onplayer_stored_item_in_itemholder_handle)
    TryListenToGameEvent("player_removed_item_from_itemholder", OnPlayerRemovedItemFromItemholder, nil, onplayer_removed_item_from_itemholder_handle)
    --if onplayer_attached_flashlight_handle == nil then onplayer_attached_flashlight_handle=TryListenToGameEvent("player_attached_flashlight",OnPlayerAttachedFlashlight, nil) end
    TryListenToGameEvent("health_pen_teach_storage", OnHealthPenTeachStorage, nil, onhealth_pen_teach_storage_handle)
    TryListenToGameEvent("health_vial_teach_storage", OnHealthVialTeachStorage, nil, onhealth_vial_teach_storage_handle)
    --if onplayer_pickedup_storable_clip_handle == nil then onplayer_pickedup_storable_clip_handle=TryListenToGameEvent("player_pickedup_storable_clip",OnPlayerPickedupStorableClip, nil) end
    --if onplayer_pickedup_insertable_clip_handle == nil then onplayer_pickedup_insertable_clip_handle=TryListenToGameEvent("player_pickedup_insertable_clip",OnPlayerPickedupInsertableClip, nil) end
    TryListenToGameEvent("player_covered_mouth", OnPlayerCoveredMouth, nil, onplayer_covered_mouth_handle)
    --if onplayer_upgraded_weapon_handle == nil then onplayer_upgraded_weapon_handle=TryListenToGameEvent("player_upgraded_weapon",OnPlayerUpgradedWeapon, nil) end
    TryListenToGameEvent("tripmine_hack_started", OnTripmineHackStarted, nil, ontripmine_hack_started_handle)
    TryListenToGameEvent("tripmine_hacked", OnTripmineHacked, nil, ontripmine_hacked_handle)
    TryListenToGameEvent("primary_hand_changed", OnPrimaryHandChanged, nil, onprimary_hand_changed_handle)
    TryListenToGameEvent("single_controller_mode_changed", OnSingleControllerModeChanged, nil, onsingle_controller_mode_changed_handle)
    TryListenToGameEvent("movement_hand_changed", OnMovementHandChanged, nil, onmovement_hand_changed_handle)
    TryListenToGameEvent("combine_tank_moved_by_player", OnCombineTankMovedByPlayer, nil, oncombine_tank_moved_by_player_handle)
    TryListenToGameEvent("player_continuous_jump_finish", OnPlayerContinuousJumpFinish, nil, onplayer_continuous_jump_finish_handle)
    TryListenToGameEvent("player_continuous_mantle_finish", OnPlayerContinuousMantleFinish, nil, onplayer_continuous_mantle_finish_handle)
    TryListenToGameEvent("player_grabbed_ladder", OnPlayerGrabbedLadder, nil, onplayer_grabbed_ladder_handle)

    TryListenToGameEvent("two_hand_pistol_grab_start", OnTwoHandStart, nil, ontwo_hand_pistol_grab_start_handle)
    TryListenToGameEvent("two_hand_pistol_grab_end", OnTwoHandEnd, nil, ontwo_hand_pistol_grab_end_handle)
    TryListenToGameEvent("two_hand_rapidfire_grab_start", OnTwoHandStart, nil, ontwo_hand_rapidfire_grab_start_handle)
    TryListenToGameEvent("two_hand_rapidfire_grab_end", OnTwoHandEnd, nil, ontwo_hand_rapidfire_grab_end_handle)
    TryListenToGameEvent("two_hand_shotgun_grab_start", OnTwoHandStart, nil, ontwo_hand_shotgun_grab_start_handle)
    TryListenToGameEvent("two_hand_shotgun_grab_end", OnTwoHandEnd, nil, ontwo_hand_shotgun_grab_end_handle)

    lastWeapon = "unknown"

    Msg("Teslasuit Listeners registered. " .. _VERSION .. " \n")
else
    TryStopListeningToGameEvent(onplayer_opened_game_menu_handle)
    TryStopListeningToGameEvent(onplayer_closed_game_menu_handle)

    TryListenToGameEvent("player_opened_game_menu", OnPlayerOpenedGameMenu, nil, onplayer_opened_game_menu_handle)
    TryListenToGameEvent("player_closed_game_menu", OnPlayerClosedGameMenu, nil, onplayer_closed_game_menu_handle)
end
