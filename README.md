# QuestCam-UnityPlugin

## How to setup:

### Unity
1. Install Unity Native Gallery through package manager:
    - Go to Window > Package Manager
    - Click the "+" icon and select "Add package from git URL"
    - Paste this URL: https://github.com/yasirkula/UnityNativeGallery
    - Wait for it to install

2. Install QuestCam SDK through package manager:
    - In Package Manager, click "+" again and select "Add package from git URL"
    - Paste this URL: https://github.com/LMNL-Org/Unity-QuestCam

3. Set up Quest Cam Prefab:
    - In Unity, navigate to `Packages/QuestCam/Runtime/Prefabs`
    - Drag and drop the QuestCam prefab into your scene
    - Do not modify the QuestCam prefab

### Developer account
1. Register at our site: https://questcam.io/register
2. Activate your email (check your spam folder)
3. After logging in, add your project at: https://questcam.io/dev/add_project
    - Fill in your project details (name, company name (optional), description, store URL (optional))
4. Copy the game key from your project page
5. Paste the game key into the QuestCam Recorder component on the root object of the QuestCam prefab in your Unity scene

![alt text](https://github.com/LMNL-Org/Unity-QuestCam/blob/main/Images/game_token_example.png?raw=true)

### Unity requirements (player settings)
- Use IL2CPP (it will not work without it)
- Set architecture to ARM64
- Enable internet access
- Set minimum Android SDK to 29 or higher
- Use only OpenGL backend, `Vulkan is not yet supported!`

![alt text](https://github.com/LMNL-Org/Unity-QuestCam/blob/main/Images/player_settings.png?raw=true)

### Join the QuestCam Developer Discord

QuestCam Developer Discord: https://discord.gg/QBj7mwXJuj

## Additional Notes
- QuestCam is currently `only supporting Meta Quest 2 and above.`
- The QuestCam prefab controls how the camera appears and disappears in your game. You'll have to link this to your game's interaction system so the player can press QuestCam's buttons.
- Once set up correctly, the camera will automatically save videos to the Quest gallery when you build your game with VR support.
- If you encounter issues, contact us through our Discord.

## Socials & Support

Please consider donating to support the project on Patreon: https://patreon.com/QuestCam

Community Discord: https://discord.gg/MNBaAJgFF2
TikTok: https://www.tiktok.com/@questcamvr
X: https://x.com/questcamvr


