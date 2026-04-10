# DynamoText

Convert text strings into Dynamo geometry. Each character is flattened into line segments using WPF's font rendering engine, giving you real curves you can use in any downstream geometry workflow.

![DynamoText example](docs/dynamoTextExample.gif)

---

## What it does

`Text.FromStringOriginAndScale` takes a string and returns a collection of `Curve` objects — one per line segment in the flattened glyph outlines. The result is placeable geometry: set an origin point, set a scale, and the text lands exactly where you want it in model space.

---

## Nodes

### `Text.FromStringOriginAndScale`

| Input | Type | Default | Description |
|---|---|---|---|
| `text` | string | — | The string to convert |
| `origin` | Point | — | Bottom-left anchor in model space |
| `scale` | double | — | Scale factor (1.0 = roughly 1 unit tall) |
| `fontFamily` | string | `"Arial"` | Any installed system font |
| `bold` | bool | `false` | Bold weight |
| `italic` | bool | `false` | Italic style |

**Returns:** `IEnumerable<Curve>` — the line segments that make up the text outlines.

If `fontFamily` is not installed on the system, throws an `ArgumentException` and tells you to call `GetInstalledFontNames()`.

---

### `Text.GetInstalledFontNames`

Returns a sorted list of all font family names installed on the current machine. Use this to discover valid values for the `fontFamily` input.

**Returns:** `IList<string>`

---

## Installation

### Dynamo Package Manager

Search for **Dynamo Text** in the Dynamo Package Manager and install directly.

### Manual

1. Build the project (see [Building](#building))
2. Copy `dist/DynamoText/` into your Dynamo packages directory:
   - Dynamo Sandbox: `%APPDATA%\Dynamo\Dynamo Core\<version>\packages\`
   - Revit: `%APPDATA%\Dynamo\Dynamo Revit\<version>\packages\`
3. Restart Dynamo

---

## Building

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8), Windows (WPF dependency)

```bash
dotnet build src/DynamoText.sln --configuration Release
```

The `CreatePackage` MSBuild target runs automatically after build and assembles the installable package at `dist/DynamoText/`.

```
dist/DynamoText/
├── pkg.json
└── bin/
    ├── DynamoText.dll
    ├── DynamoText.pdb
    └── DynamoText_DynamoCustomization.xml
```

---

## How it works

WPF's `FormattedText` class converts a string into a `Geometry` object using the system font renderer. That geometry is flattened into a `PathGeometry` made up of `PathFigure` segments. Each `LineSegment` and `PolyLineSegment` becomes a Dynamo `Line`, scaled and translated to the requested origin point.

Y-axis is flipped on the way out (`-y + 1`) so text reads left-to-right and bottom-up in Dynamo's coordinate system.

---

## Requirements

- Dynamo 2.x / 3.x (ZeroTouch library)
- Windows only (WPF font rendering)
- .NET 8

---

## License

[Apache 2.0](../LICENSE.txt)
