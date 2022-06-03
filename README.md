# Half-Life Alyx Teslasuit Experience

Application for haptic feedback with Teslasuit for Half-Life Alyx game. Haptic feedback generation based on stream of feedback events and game state from console.

## Setup Environment
1. Install Teslasuit Studio build #18791 or higher - https://gitlab.vrweartek.com/software/teslasuit-studio/-/pipelines.
2. Install Steam and Half-Life Alyx game from it - https://store.steampowered.com/.
3. Download the following folder - https://gitlab.vrweartek.com/a.belekhow/half-life-alyx-teslasuit-experience/-/tree/master/TeslasuitAlyx/bin/Release

## Configuration 1
1. Set launch option for Half-Life Alyx in game properties in steam: "-console -netconport 2121".
2. Run Half-Life Alyx game
3. Run **TeslasuitAlyx.exe**

## Configuration 2
1. Run **TeslasuitAlyx.exe** and wait while game will run.

## Command line arguments
- _**-port**_ Port used by telnet connection (_**2121**_ by default)
- _**-appid**_ Steam application id reserved for Half-Life Alyx game (_**546560**_ by default)
- _**-path**_ Path to Half-Life Alyx game directory
- _**-filename**_ Half-Life Alyx game executable name (_**hlvr**_ by default)

## Running
1. Connect calibrated Teslasuit to PC.
2. Follow steps from [Configuration 1](/#configuration-1) or [Configuration 2](/#configuration-2)

## Troubleshooting
If there are message with text "**Failed to init game prerequisites. Game dir not found.**", add **-path path/to/the/game/dir** option to TeslastuitAlyx.exe

## Development
-

### Project structure
-

### Links
