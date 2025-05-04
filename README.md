# MC Skin Aseprite Previewer - WIP

A live Minecraft skin preview tool that renders your skin edits in real time using OpenGL and Aseprite integration.

## ✨ Features

- Real-time 3D preview of Minecraft skins as you draw in Aseprite
- Accurate UV mapping for head, body, arms, and legs
- Lua script for triggering live reloads on save
- OpenGL rendering using OpenTK (C#)

## 🖼 Requirements

- [Aseprite](https://www.aseprite.org/) (with scripting enabled)
- .NET Core or .NET Framework
- OpenTK (via NuGet)
- A Minecraft skin saved as `.png`

## 🛠 Setup

1. Clone the project and open in your C# IDE.
2. Install OpenTK via NuGet:  
   `dotnet add package OpenTK`
3. Place your `.png` skin file in the project directory.
4. In Aseprite, install the `live_preview_trigger.lua` script:
   - Go to `File > Scripts > Open Scripts Folder`
   - Save the script there and restart Aseprite
5. Open your `.png` skin in Aseprite and begin editing.
6. On saving the skin, the `preview_trigger.txt` file will update and the renderer will reload the texture.

## 🚀 Running

Run the C# OpenTK application. It will monitor the `preview_trigger.txt` file and reload the skin texture whenever you save in Aseprite.

---

Feel free to customize the 3D model, add outer layers, or extend animation support!
