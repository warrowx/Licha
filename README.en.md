# Licha (Life + Gacha)

🇷🇺 [Читать на русском](README.md)

**Licha** is a gacha-style to-do app for real life. Complete tasks, earn gems,
and pull from banners filled with rewards you wrote yourself: a movie night,
a favorite dessert, a day off.

## About
The idea came from wanting to make everyday routine more fun. Instead of
boring lists: excitement, collecting rewards, and satisfying pull animations.

## Features
- **Tasks:** one-time and daily, streaks, rewards scale with difficulty
- **Custom banners:** 3★/4★/5★ rewards, images for 4★ and 5★, per-banner pull cost and drop rates
- **Pity system:** guaranteed rewards, like in real gacha games
- **Pull animations:** rarity-based glow effects, tunable in a single file
- **Pull history**
- **Local storage:** your data stays on your computer (`%AppData%\GachaLife`)

## Getting started
**Prebuilt (Windows):**
1. Download the latest release from **Releases**.
2. Unzip and run `Licha.exe`.

## Tech stack
C# / .NET, WPF, MVVM (CommunityToolkit.Mvvm), System.Text.Json

## Architecture
- `Models`, `Services`: data and logic (pull engine, persistence), no WPF dependencies
- `ViewModels`: page state and commands (MVVM)
- `Views`, `Animations`: UI and animations

Logic is kept separate from the UI so the app can later be ported to Avalonia.

## Roadmap
- [ ] Collection page
- [ ] Achievements
- [ ] Pull sound effects
- [ ] UI customization
- [ ] Stats export
- [ ] Cross-platform version (Avalonia)
