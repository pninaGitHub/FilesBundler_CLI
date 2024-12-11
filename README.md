# CLI Code Bundler

## Overview

CLI Code Bundler is a .NET Console Application that streamlines the process of bundling code files into a single file. The application supports multiple customization options, making it an efficient tool for developers who work with various file types and need to consolidate their code for easier management.

The application is built using the `System.CommandLine` library and provides a robust Command-Line Interface (CLI) for bundling operations and creating reusable commands.

## Features

### 1. **Bundle Command (********`bundle`********):**

The `bundle` command allows users to consolidate code files into a single output file with the following options:

- **`--languages`**\*\* (****`-l`****)\*\* *(required)*: Specifies the programming languages to include in the bundle (e.g., `cs`, `py`). Use `all` to include files of all types.
- **`--output`**\*\* (****`-o`****)\*\* *(required)*: Specifies the path and name of the output file.
- **`--note`**\*\* (****`-n`****)\*\*: Includes the source file paths as comments in the bundled file.
- **`--sort`**\*\* (****`-s`****)\*\*: Specifies the sorting method for files: `name` (default) or `type`.
- **`--remove-empty-lines`**\*\* (****`-r`****)\*\*: Removes empty lines from source files before bundling.
- **`--author`**\*\* (****`-a`****)\*\*: Adds the author’s name as a comment at the top of the bundled file.
- **`--overwrite`**\*\* (****`-w`****)\*\*: Enables overwriting the output file if it already exists.

### 2. **Interactive File Handling:**

If the output file already exists, the application provides options to:

- Overwrite the file.
- Specify a new file name.
- Cancel the operation.

### 3. **File Exclusion:**

Files in directories like `bin`, `debug`, or similar are automatically excluded from the bundling process.

### 4. **Response File Command (********`create-rsp`********):**

Generates a response file that stores a preconfigured `bundle` command. This simplifies repeated execution by allowing users to run:

```bash
dotnet @fileName.rsp
```

### 5. **Global Access via PATH Configuration:**

Add the compiled CLI application to the system PATH for easy access from any directory.

## Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/your-username/cli-code-bundler.git
   cd cli-code-bundler
   ```

2. **Install Dependencies:**
   Install the `System.CommandLine` library via NuGet:

   ```bash
   dotnet add package System.CommandLine --prerelease
   ```

3. **Build the Project:**

   ```bash
   dotnet build
   ```

4. **Publish the Executable:**

   ```bash
   dotnet publish -c Release -o publish
   ```

5. **Add to PATH:**
   Add the `publish` directory to your system PATH to access the CLI globally.

## Usage

### Example Commands

#### **Bundling Files**

```bash
Fib bundle -l "cs" -o "C:\Users\USER\Downloads\output_file.txt" -n -s "type" -r -a "Pnina Horowitz" -w
```

**Explanation:**

- `-l "cs"`: Bundles only C# files.
- `-o "C:\Users\USER\Downloads\output_file.txt"`: Specifies the output file path.
- `-n`: Includes the source file paths as comments.
- `-s "type"`: Sorts files by their type.
- `-r`: Removes empty lines from the source files.
- `-a "Pnina Horowitz"`: Adds the author name to the bundled file.
- `-w`: Overwrites the output file if it already exists.

#### **Creating a Response File**

```bash
Fib create-rsp
```

Follow the interactive prompts to generate a response file for later use.

#### **Using the Response File**

```bash
dotnet @fileName.rsp
```

Executes the command stored in the response file.

## Code Example

Below is an excerpt from the main implementation:

```csharp
static async Task BundleFilesAsync(
    string[] languages,
    FileInfo output,
    bool note,
    string sort,
    bool removeEmptyLines,
    string? author,
    bool overwrite)
{
    if (output.Exists)
    {
        Console.WriteLine($"\nThe file '{output.FullName}' already exists.");
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("1. Overwrite the file");
        Console.WriteLine("2. Create a new file with a different name");
        Console.WriteLine("3. Cancel the operation\n");

        string userChoice = Console.ReadLine()?.Trim();
        switch (userChoice)
        {
            case "1":
                Console.WriteLine("Overwriting the existing file...");
                break;
            case "2":
                Console.Write("Enter a new name for the output file (including path): ");
                string newFileName = Console.ReadLine()?.Trim();
                output = new FileInfo(newFileName);
                break;
            case "3":
            default:
                Console.WriteLine("Operation canceled.");
                return;
        }
    }

    var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
                         .Where(f => languages.Contains("all") || languages.Contains(Path.GetExtension(f).TrimStart('.')))
                         .ToList();

    using var writer = new StreamWriter(output.FullName, append: false);

    if (!string.IsNullOrWhiteSpace(author))
        await writer.WriteLineAsync($"// Author : {author}\n");

    foreach (var file in files)
    {
        await writer.WriteLineAsync($"// File: {Path.GetFileName(file)}\n");
        if (note) await writer.WriteLineAsync($"// Source: {file}\n");
        var lines = await File.ReadAllLinesAsync(file);
        if (removeEmptyLines) lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        foreach (var line in lines) await writer.WriteLineAsync(line);
        await writer.WriteLineAsync("\n/* -------------------- */\n");
    }

    Console.WriteLine($"Bundle created successfully at: {output.FullName}");
}
```

##

