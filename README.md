# JoshaParser

**Beat Saber Map Parser**

[![C#](https://img.shields.io/badge/language-C%23-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/github/license/Joshabi/JoshaParser)](LICENSE)

---

JoshaParser is a C# library that was split from JoshaParity that handles Beat Saber map parsing. It can process V2 through to V4 map format by converting the parsed data into a common model for all versions, however cannot reserialize V4 data yet. This is mostly designed as an easy base to use for some of my other projects when I need to load Beat Saber Map Data. It currently still lacks support for V3 non-basic lighting events and custom data in general.

---

## Installation

Clone the repo and build the project or download from Releases. There is a build action with the provided VS 2022 solution that will automatically output the dll to your Beat Saber directory inside of the /libs/ folder. You may need to adjust the path in the csproj file if its non-standard.

```bash
git clone https://github.com/Joshabi/JoshaParser.git
```

---

## Usage

You can load a Beat Saber map directory (containing `info.dat` and difficulty files) using the `BeatmapLoader`:

```csharp
string mapFolder = ".\Maps Folder\Additional Memory";
Beatmap? map = BeatmapLoader.LoadMapFromDirectory(mapFolder);
```

This will load the SongInfo & AudioInfo (if applicable in V4) from which point you can fetch the difficulty data when it is actually required through a cache / lazy loading system.
