# QuestCam-UnityPlugin


## Video Tutorial:
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/aJuM1bQ7Uxg/0.jpg)](https://www.youtube.com/watch?v=aJuM1bQ7Uxg)


## How to setup:

### Unity
1. Install `git` 
    - Download git from here and install it https://git-scm.com/downloads (you will need to restart unity after installing)

2. Install `Unity Native Gallery` through package manager:
    - Go to Window > Package Manager
    - Click the "+" icon and select "Add package from git URL"
    - Paste this URL: https://github.com/yasirkula/UnityNativeGallery.git
    - Wait for it to install

3. Install `QuestCam SDK` through package manager:
    - In Package Manager, click "+" again and select "Add package from git URL"
    - Paste this URL: https://github.com/LMNL-Org/Unity-QuestCam.git

4. Set up `Quest Cam Prefab`:
    - In Unity, navigate to `Packages/QuestCam/Runtime/Prefabs`
    - Drag and drop the QuestCam prefab into your scene
    - if you choose to modify the prefab, please keep the model generally the same and include our logo on the back unless we've discussed otherwise (we're making QuestCam for free, after all!)

### Developer account
1. Register at our site: https://questcam.io/register
2. Activate your email (check your spam folder)
3. After logging in, add your project at: https://questcam.io/dev/add_project
    - Fill in your project details (name, company name (optional), description, store URL (optional))
4. Copy the game key from your project page
5. Paste the game key into the QuestCam Recorder component on the root object of the QuestCam prefab in your Unity scene

![alt text](https://github.com/LMNL-Org/Unity-QuestCam/blob/main/Images/game_token_example.png?raw=true)

### Unity requirements (player settings)
- Unity 2022.3+
- Android API Level 24+
- Use IL2CPP (it will not work without it)
- Set architecture to ARM64
- Enable internet access
- Vulkan or OpenGL backend

![alt text](https://github.com/LMNL-Org/Unity-QuestCam/blob/main/Images/player_settings.png?raw=true)

### Join the QuestCam Developer Discord

QuestCam Developer Discord: https://discord.gg/QBj7mwXJuj

## Additional Notes
- QuestCam is currently `only supporting Meta Quest 2 and above.`
- The QuestCam prefab controls how the camera appears and disappears in your game. You'll have to link this to your game's interaction system so the player can press QuestCam's buttons. You'll also need to add a gesture or button for summoning QuestCam.
- Once set up correctly, the camera will automatically save videos to the Quest gallery when you build your game with VR support.
- If you encounter issues, contact us through our Discord.

## Terms of Use
- You must keep the QuestCam logo clearly visible on the camera somewhere if you choose to modify the camera model.
- If you choose to sell QuestCam in your game for USD or in-game currency, you must acitvely be supporting our Patreon at $15/month or more (link below).

## Socials & Support

Please consider donating to support the project on Patreon: https://patreon.com/QuestCam

Community Discord: https://discord.gg/MNBaAJgFF2
TikTok: https://www.tiktok.com/@questcamvr
X: https://x.com/questcamvr


