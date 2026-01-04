# MyHome Application - Advanced .NET MAUI Setup

## 🎯 Project Overview

MyHome is a modern .NET MAUI application featuring a sophisticated Home Dashboard with Indoor and Outdoor environmental monitoring. The application showcases advanced MAUI configurations, localization, theming, and MVVM architecture using CommunityToolkit.

## 🚀 Key Features

### 1. Advanced Project Configuration
- **XAML Source Generators**: Enabled for compile-time XAML validation
- **AOT Compilation**: Ahead-of-Time compilation enabled for Release builds
- **CoreCLR Optimizations**: Full optimization suite for Android (R8, ProGuard, ProfiledAOT)
- **Code Quality**: ImplicitUsings and Nullable reference types enabled

### 2. Global XAML Namespaces
All XAML files use a single unified namespace: `xmlns:app="http://myhome.com/schemas"`

**File**: `GlobalXmlns.cs`
- Maps all namespaces (Views, ViewModels, Controls, Services, etc.) to one URL
- Eliminates repetitive `clr-namespace` declarations
- Cleaner, more maintainable XAML code

### 3. Localization System
**Supported Languages**:
- English (en-US) - Default
- Ukrainian (uk-UA)

**Implementation**:
- Resource files: `AppResources.resx` and `AppResources.uk.resx`
- Custom markup extension: `{localization:Translate KeyName}`
- Runtime language switching via `LocalizationResourceManager.Maui`
- Persisted language preference using `Preferences` API

**Localized Keys**:
- DashboardTitle, Indoor, Outdoor
- Temperature, Humidity, Pressure
- Settings, Language, Theme
- LightMode, DarkMode, English, Ukrainian

### 4. Modern Green Theme
**Color Palette**:
- **Light Mode**: Deep greens (#2E7D32, #1B5E20) with light accents (#81C784, #A5D6A7)
- **Dark Mode**: Bright greens (#66BB6A, #81C784) with medium tones (#388E3C, #2E7D32)

**Theme Features**:
- Dynamic theme switching (Light/Dark)
- `CommunityToolkit.Maui` AppThemeColor bindings
- Smooth transitions between themes
- Persisted theme preference

### 5. Dashboard UI

**Layout Structure**:
Two visually distinct cards with modern styling:

#### Indoor Card 🏠
- Temperature (°C)
- Humidity (%)

#### Outdoor Card 🌤️
- Temperature (°C)
- Humidity (%)
- Atmospheric Pressure (hPa)

**Design Elements**:
- Rounded corners (20px radius)
- Subtle shadows for depth
- Icon badges with gradient backgrounds
- Responsive grid layout
- Green color accents throughout

### 6. Shell Footer Settings
Interactive settings panel in the Shell flyout footer:
- **Theme Switch**: Toggle between Light and Dark modes
- **Language Picker**: Select between English and Ukrainian
- Real-time application of changes
- Persistent settings storage

### 7. Architecture

**MVVM Pattern**:
- `DashboardViewModel`: Observable properties with `[ObservableProperty]` attributes
- `SettingsService`: Centralized settings management
- Dependency Injection: All services and views registered in DI container

**Libraries Used**:
- `CommunityToolkit.Maui` (10.0.0): UI toolkit and markup extensions
- `CommunityToolkit.Mvvm` (8.3.2): MVVM helpers and source generators
- `FluentColors.Maui` (1.0.0.6): Color palette management
- `LocalizationResourceManager.Maui` (1.3.0-alpha.1): Localization framework

## 📁 Project Structure

```
MyHomeApp/
├── GlobalXmlns.cs                          # Global XAML namespace definitions
├── MauiProgram.cs                          # App initialization and DI setup
├── App.xaml / App.xaml.cs                  # Application root
├── AppShell.xaml / AppShell.xaml.cs        # Shell navigation with footer
│
├── Views/
│   ├── DashboardPage.xaml                  # Main dashboard UI
│   └── DashboardPage.xaml.cs               # Dashboard code-behind
│
├── ViewModels/
│   └── DashboardViewModel.cs               # Dashboard data and logic
│
├── Services/
│   └── SettingsService.cs                  # Theme and language management
│
├── Extensions/
│   └── localization:TranslateExtension.cs               # Localization markup extension
│
├── Converters/
│   └── ThemeConverters.cs                  # Theme-related value converters
│
├── Resources/
│   ├── Localization/
│   │   ├── AppResources.resx               # English resources
│   │   ├── AppResources.uk.resx            # Ukrainian resources
│   │   └── AppResources.Designer.cs        # Generated resource accessor
│   │
│   └── Styles/
│       ├── Colors.xaml                     # Green theme color definitions
│       └── Styles.xaml                     # Common styles
```

## 🛠️ Build Configuration

### Debug Build
- Standard debug symbols
- Logging enabled
- Hot Reload support

### Release Build
- AOT compilation enabled
- Tree trimming (partial mode)
- LLVM optimizations
- R8 code shrinker (Android)
- ProGuard enabled
- Per-ABI APKs for Android
- No debug symbols

## 🎨 XAML Best Practices Implemented

1. **Compiled Bindings**: `x:DataType` specified on all data-bound pages
2. **Global Namespace**: Single `xmlns:app` declaration
3. **Markup Extensions**: Custom `localization:TranslateExtension` for localization
4. **AppTheme Bindings**: Dynamic color switching without code
5. **Resource Management**: Centralized colors and styles

## 🔧 Dependency Injection Setup

**Registered Services**:
- `SettingsService` (Singleton): Application settings
- `DashboardViewModel` (Transient): Dashboard data
- `DashboardPage` (Transient): Dashboard view
- `AppShell` (Singleton): Navigation shell

## 📱 Platform Support

- ✅ Android (API 21+)
- ✅ iOS (15.0+)
- ✅ macOS Catalyst (15.0+)
- ✅ Windows (10.0.17763.0+)

## 🚦 Getting Started

1. **Restore Packages**:
   ```bash
   dotnet restore
   ```

2. **Build**:
   ```bash
   dotnet build
   ```

3. **Run** (Select your target platform):
   ```bash
   # Android
   dotnet build -t:Run -f net10.0-android
   
   # iOS Simulator
   dotnet build -t:Run -f net10.0-ios
   
   # macOS
   dotnet build -t:Run -f net10.0-maccatalyst
   ```

## 🎯 Key Implementation Details

### Global XAML Namespace Usage
```xml
<ContentPage xmlns:app="http://myhome.com/schemas"
             x:DataType="app:DashboardViewModel">
    <Label Text="{localization:Translate DashboardTitle}" />
</ContentPage>
```

### Localization in XAML
```xml
<Label Text="{localization:Translate Temperature}" />
```

### Theme-Aware Colors
```xml
<Border BackgroundColor="{StaticResource Card}">
    <Label TextColor="{StaticResource TextPrimary}" />
</Border>
```

### ViewModel with Source Generators
```csharp
public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
     double indoorTemperature;  // Generates property automatically
}
```

## 📝 Future Enhancements

- Real sensor data integration (MQTT, WebSockets)
- Historical data charts and trends
- Multiple location support
- Push notifications for threshold alerts
- Widget support for Android/iOS
- Apple Watch and WearOS companion apps

## 📄 License

This is a demo project for .NET MAUI advanced features showcase.

---

**Built with** ❤️ **using .NET MAUI 10.0**
