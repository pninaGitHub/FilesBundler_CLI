//using System.CommandLine;
//using System.CommandLine.Invocation;
//using System.IO;
//using System.Linq;

//class Program_________
//{
//    static async Task<int> Main(string[] args)
//    {
//        var rootCommand = new RootCommand("CLI for bundling code files");

//        // פקודת bundle
//        var bundleCommand = new Command("bundle", "Bundle code files into a single file")
//        {
//            new Option<string>("--language", "Programming languages to include (e.g., C#, Python, etc.)")
//            {
//                IsRequired = true,
//                Alias = "-l"
//            },
//            new Option<string>("--output", "Output bundle file path")
//            {
//                Alias = "-o"
//            },
//            new Option<bool>("--note", "Include source file paths as comments")
//            {
//                Alias = "-n"
//            },
//            new Option<string>("--sort", "Sorting order: 'name' or 'type' (default: 'name')", () => "name")
//            {
//                Alias = "-s"
//            },
//            new Option<bool>("--remove-empty-lines", "Remove empty lines from the source files")
//            {
//                Alias = "-r"
//            },
//            new Option<string>("--author", "Author name to include in the bundle")
//            {
//                Alias = "-a"
//            }
//        };

//        bundleCommand.Handler = CommandHandler.Create<string, string, bool, string, bool, string>(BundleFiles);

//        rootCommand.AddCommand(bundleCommand);

//        // פקודת create-rsp
//        var rspCommand = new Command("create-rsp", "Create a response file for bundling command");
//        rspCommand.Handler = CommandHandler.Create(CreateRspFile);

//        rootCommand.AddCommand(rspCommand);

//        return await rootCommand.InvokeAsync(args);
//    }

//    static void BundleFiles(string language, string output, bool note, string sort, bool removeEmptyLines, string author)
//    {
//        // קריאה לקבצים מסוננים
//        var allowedExtensions = language == "all" ? "*" : language.Split(',').Select(l => $".{l.Trim()}").ToArray();
//        var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
//            .Where(f => !f.Contains("bin") && !f.Contains("debug") && allowedExtensions.Contains(Path.GetExtension(f)))
//            .OrderBy(f => sort == "name" ? Path.GetFileName(f) : Path.GetExtension(f))
//            .ToList();

//        if (!files.Any())
//        {
//            Console.WriteLine("No matching files found.");
//            return;
//        }

//        using var writer = new StreamWriter(output);

//        if (!string.IsNullOrEmpty(author))
//        {
//            writer.WriteLine($"// Author: {author}");
//        }

//        foreach (var file in files)
//        {
//            if (note)
//            {
//                writer.WriteLine($"// Source: {file}");
//            }

//            var lines = File.ReadAllLines(file);
//            if (removeEmptyLines)
//            {
//                lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
//            }

//            foreach (var line in lines)
//            {
//                writer.WriteLine(line);
//            }
//        }

//        Console.WriteLine($"Bundle created successfully: {output}");
//    }

//    static void CreateRspFile()
//    {
//        Console.Write("Enter languages (comma-separated or 'all'): ");
//        var language = Console.ReadLine();

//        Console.Write("Enter output file path: ");
//        var output = Console.ReadLine();

//        Console.Write("Include source file paths as comments? (yes/no): ");
//        var note = Console.ReadLine()?.ToLower() == "yes";

//        Console.Write("Sort files by 'name' or 'type': ");
//        var sort = Console.ReadLine();

//        Console.Write("Remove empty lines? (yes/no): ");
//        var removeEmptyLines = Console.ReadLine()?.ToLower() == "yes";

//        Console.Write("Enter author name (optional): ");
//        var author = Console.ReadLine();

//        var rspContent = $"bundle --language {language} --output \"{output}\" --note {note} --sort {sort} --remove-empty-lines {removeEmptyLines} --author \"{author}\"";
//        var rspFileName = "bundleCommand.rsp";
//        File.WriteAllText(rspFileName, rspContent);

//        Console.WriteLine($"Response file created: {rspFileName}");
//    }
//}
