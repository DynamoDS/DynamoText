# DynamoText

DynamoText is a Dynamo ZeroTouch package for generating text curves from installed system fonts.

## Current Release

- Package version: `3.0.1`
- Assembly name: `DynamoTextRD`
- Assembly version: `3.0.1.0`
- Customization file: `DynamoTextRD_DynamoCustomization.xml`
- Engine version in package manifest: `3.0.0.7186`

## Why The Assembly Name Changed

The assembly was renamed from `DynamoText.dll` to `DynamoTextRD.dll` to prevent filename collisions in environments that contain multiple packages with a `DynamoText.dll`. Dynamo's FFI module cache can collide on file name, so a unique assembly name ensures reliable package loading.

## Build

```powershell
dotnet restore .\DynamoText.csproj
dotnet build .\DynamoText.csproj -c Release
```

Build output:

- `bin\Release\DynamoTextRD.dll`
- `bin\Release\DynamoTextRD_DynamoCustomization.xml`

