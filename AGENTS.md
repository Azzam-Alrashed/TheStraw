# Contributor Guide

## Project Overview

The Straw is a short Unity 6 narrative game about workplace stress and the emotional breaking point created by accumulated interruptions. Read [GAME_LOOP.md](GAME_LOOP.md) before changing gameplay behavior.

## Project Structure

- `TheStraw/Assets/Scenes/` — Unity scenes; `Office.unity` is the main gameplay scene.
- `TheStraw/Assets/Scripts/` — C# gameplay code, organized by feature (`Player`, `Interactions`, `Gameplay`, `Stress`, `Tasks`, and `Managers`).
- `TheStraw/Assets/Art/` — sprites, tiles, palettes, and animations.
- `TheStraw/Assets/Settings/` — input and URP configuration.

## Development Guidelines

- Use C# and Unity APIs compatible with Unity 6.
- Keep scripts focused on one responsibility and place them in the appropriate feature folder.
- Preserve existing namespaces and naming conventions when extending an area of the project.
- Prefer serialized inspector fields for scene-specific references; validate required references early and fail safely when they are missing.
- Keep gameplay changes aligned with the core loop: actions cost time, create stress, and can offer recovery or tradeoffs.
- Do not manually edit generated Unity files, Library files, or project settings unless the change requires it.

## Unity Asset Rules

- Commit the matching `.meta` file whenever a Unity asset or script is added, moved, or removed.
- Do not delete or regenerate third-party LimeZu art assets without explicit approval.
- Avoid broad scene rewrites: make the smallest targeted changes possible to `Office.unity`.

## Verification

- Open the project in Unity and confirm there are no Console errors after code or scene changes.
- Test player movement and relevant interactions in `Office.unity`.
- For changes to stress, tasks, or timing, verify that the player can still progress toward the 5:00 PM ending.
