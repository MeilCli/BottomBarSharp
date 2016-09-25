# BottomBarSharp

This library is translated from [BottomBar](https://github.com/roughike/BottomBar)

## Thanks

[BottomBar](https://github.com/roughike/BottomBar) is under [Apache License 2.0](https://github.com/roughike/BottomBar/blob/master/LICENSE)  
Copyright (c) 2016 Iiro Krankka ([http://github.com/roughike](http://github.com/roughike)).

## About

- Xamarin.Android
- Translating BottomBar v2.0.2
- Now, this library is not distribution NuGet
- If use this, you build library on your local PC

## Different from BottomBar

- Namespace, write in xml, to bottombarsharp.BottomBar from com.roughike.bottombar.BottomBar
- Add event, tab select and reselect EventHandler(other, include Java like listeners)
- some field to property

## Usage

1. Download this Solution or clone to your PC
2. Open VisualStudio 2015(include Xamarin Environment), this solution
3. Build solution(Debug or Release mode)
4. See file exproler to solution, /BottomBar/bin/(Debug or Release)/
5. you look dll file
6. Add dll your project and add reference dll

## Sample

see BottomBarSharp.App project

## TroubleShooting

### not found resource

maybe, resource id character need lowercace.

Example) @drawable/IconButtonGrey24dp → @drawable/iconbuttongrey24dp

on Tab xml and BottomBar layout xml

## License

This library is under MIT License.