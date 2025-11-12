# Auto-Formatting Setup Guide

This monorepo has been configured with automatic formatting for both TypeScript/Svelte and C# code.

## Prerequisites

### Required VSCode Extensions

1. **For TypeScript/Svelte:**
   - [Prettier - Code formatter](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)
   - [Svelte for VS Code](https://marketplace.visualstudio.com/items?itemName=svelte.svelte-vscode)

2. **For C#:**
   - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) (recommended)
   - OR [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension

## Setup Instructions

### 1. TypeScript/Svelte (Web Client)

**Location:** `apps/web/marbles-web-client`

#### Install Dependencies

```bash
cd apps/web/marbles-web-client
npm install
```

This will install Prettier and the Svelte plugin.

#### Configuration Files
- `.prettierrc` - Prettier configuration
- `.prettierignore` - Files to ignore

#### Manual Formatting
You can manually format files using:
```bash
npm run format        # Format all files
npm run format:check  # Check formatting without changing files
```

#### Auto-formatting
Files will automatically format on save when you're working in the web client directory.

### 2. C# (Unity Project)

**Location:** `MarblesUnityProject`

#### Configuration Files
- `.editorconfig` - C# formatting rules for Unity project

#### Features
- Automatically formats C# files on save
- Organizes imports/usings on save
- Follows Unity-specific conventions
- Uses 4-space indentation
- Allman brace style (braces on new lines)

### 3. C# (SpacetimeDB Module)

**Location:** `spacetimedb`

#### Configuration Files
- `.editorconfig` - C# formatting rules for SpacetimeDB module

#### Features
- Same formatting rules as Unity project
- Consistent C# style across both projects

## VSCode Settings

The workspace settings (`.vscode/settings.json`) have been configured to:
- Enable format on save globally
- Use Prettier for TypeScript, JavaScript, and JSON files
- Use Svelte extension for Svelte files
- Use C# extension for C# files
- Enable EditorConfig support
- Organize imports on save for C# files

## Usage

### Automatic Formatting
Once the extensions are installed, formatting happens automatically when you:
- Save a file (Ctrl+S / Cmd+S)
- The appropriate formatter will be applied based on file type

### Manual Formatting
You can also manually trigger formatting:
- **Format Document:** Shift+Alt+F (Windows/Linux) or Shift+Option+F (Mac)
- **Format Selection:** Ctrl+K Ctrl+F (Windows/Linux) or Cmd+K Cmd+F (Mac)

## Customization

### Prettier Settings
Modify `apps/web/marbles-web-client/.prettierrc` to adjust:
- Line width (currently 100)
- Tab width (currently 2)
- Quote style (currently double quotes)
- Semicolons (currently required)
- Trailing commas (currently ES5 style)

### C# Settings
Modify the `.editorconfig` files in `MarblesUnityProject/` and `spacetimedb/` to adjust:
- Indentation size
- Brace style
- Space preferences
- Naming conventions
- Code style rules

## Troubleshooting

### Prettier not working
1. Ensure the Prettier extension is installed
2. Check that `.prettierrc` exists in the web client folder
3. Run `npm install` in the web client directory
4. Reload VSCode

### C# formatting not working
1. Ensure C# Dev Kit or C# extension is installed
2. Check that `.editorconfig` files exist in the C# project folders
3. Verify `omnisharp.enableEditorConfigSupport` is `true` in settings
4. Reload VSCode or restart the C# language server

### Format on save not working
1. Check that `"editor.formatOnSave": true` is set in VSCode settings
2. Ensure the correct formatter is set for the file type
3. Check that the file type is not excluded in settings

## Additional Notes

- The C# formatter respects `.editorconfig` files, which are standard across .NET projects
- Prettier configuration is specific to the web client and won't affect other parts of the monorepo
- All configuration files are committed to the repository for team consistency

