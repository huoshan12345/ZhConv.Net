# ZhConv.Net

A lightweight .NET library for converting between Simplified and Traditional Chinese characters.

## Features

- Convert between Simplified and Traditional Chinese text
- Multiple conversion options:
  - Dictionary-based conversion (works on all platforms)
  - Windows API-based conversion (only available on Windows)
- Simple and easy-to-use API
- No external dependencies

## Latest Builds

||TargetFramework(s)|Package|
|----|----|----|
|ZhConv.Net|![netstandard2.0](https://img.shields.io/badge/netstandard-2.0-30a14e.svg)|![NuGet Version](https://img.shields.io/nuget/v/ZhConv.Net)
|

## Installation

Install the package via NuGet:

```
dotnet add package ZhConv.Net
```

Or search for "ZhConv.Net" in the NuGet package manager.

## Usage

### Basic Usage

```csharp
using ZhConv.Net;

// Convert from Simplified to Traditional Chinese
string traditionalText = ChineseConverter.ToTraditional("简体中文");

// Convert from Traditional to Simplified Chinese
string simplifiedText = ChineseConverter.ToSimplified("繁體中文");
```

### Advanced Options

You can specify whether to use the Windows API for conversion (when available):

```csharp
// Prefer Windows API for conversion if running on Windows
string traditionalText = ChineseConverter.ToTraditional("简体中文", preferWinApi: true);

// Force dictionary-based conversion even on Windows
string simplifiedText = ChineseConverter.ToSimplified("繁體中文", preferWinApi: false);
```

### Generic Conversion Method

You can also use the generic `Convert` method:

```csharp
// Convert to Traditional
string traditionalText = ChineseConverter.Convert("简体中文", ChineseConversionDirection.ToTraditional);

// Convert to Simplified
string simplifiedText = ChineseConverter.Convert("繁體中文", ChineseConversionDirection.ToSimplified);
```

## How It Works

ZhConv.Net uses two different methods for Chinese character conversion:

1. **Dictionary-based conversion**: Uses embedded mapping files to convert characters. Works on all platforms.
2. **Windows API-based conversion**: Uses the Windows LCMapStringEx API for conversion. Only available on Windows platforms.

By default, the library uses dictionary-based conversion. On Windows, you can opt to use the Windows API by setting `preferWinApi` to `true`.

## Performance Considerations

- Windows API conversion is generally faster but is only available on Windows.
- Dictionary-based conversion works on all platforms but may be slightly slower for very large texts.
- The library uses lazy loading for dictionaries to minimize startup time and memory usage.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
