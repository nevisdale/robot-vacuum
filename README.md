# robot-vacuum (Inside the Room)
## available on [Itch.io](https://kusindia.itch.io/inside-the-room)

## setup
- clone the repository
- download [Godot Engine](https://godotengine.org/). project uses version 4.3
- open the project in Godot Engine

## Changelog V2
- rewrite the game using gdscript in order to make a web build. C# is not supported in web build yet.
- add camera shake effect for different events.
- improve UI.
- make Cat enemy slower.
- use audio buses in order to control the volume of the audio easily.
- use component based architecture for all scenes as much as possible.
- use global signals for communication between scenes instead of having direct references.
- use custom resource for managing the game levels easily.
- SaveManager uses level_id (NOT scene path) in save file. BREAKING CHANGE: old save files will not work.
- add "THANK YOU FOR PLAYING" message at the end of the game.

## running in web
- Win + R -> chrome.exe --user-data-dir="C://Chrome dev session" --disable-web-security (tested in Google Chrome in Windows 11)
