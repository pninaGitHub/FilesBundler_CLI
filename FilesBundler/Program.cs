using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BundleCLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // יצירת פקודת הבסיס
            var rootCommand = new RootCommand("CLI to bundle code files into a single file");

            // יצירת אפשרויות
            var languagesOption = new Option<string[]>(
                aliases: new[] { "--languages", "-l" },
                description: "Languages to include (e.g., C#, Python, etc. or 'all')",
                getDefaultValue: () => new string[] { "all" });

            var outputOption = new Option<FileInfo>(
                aliases: new[] { "--output", "-o" },
                description: "Path and name to the output bundled file")
            {
                IsRequired = true
            };

            var noteOption = new Option<bool>(
                aliases: new[] { "--note", "-n" },
                description: "Include source file paths as comments");

            var sortOption = new Option<string>(
                aliases: new[] { "--sort", "-s" },
                description: "Sort files by 'name' or 'type' (default: 'name')",
                getDefaultValue: () => "name");

            var removeEmptyLinesOption = new Option<bool>(
                aliases: new[] { "--remove-empty-lines", "-r" },
                description: "Remove empty lines from the files");

            var authorOption = new Option<string>(
                aliases: new[] { "--author", "-a" },
                description: "Author name to include in the bundled file");

            var overwriteOption = new Option<bool>(
                aliases: new[] { "--overwrite", "-w" },
                description: "Overwrite the output file if it already exists");

            // הגדרת פקודה "bundle"
            var bundleCommand = new Command("bundle", "Bundle multiple files into one")
            {
               // bundlerCommand.AddOption(bundleOption);
               // bundlerCommand.AddOption(bundleOption);

                languagesOption,
                outputOption,
                noteOption,
                sortOption,
                removeEmptyLinesOption,
                authorOption,
                overwriteOption
            };

            // הגדרת Handler לפקודה
            bundleCommand.SetHandler(
                async (string[] languages, FileInfo output, bool note, string sort, bool removeEmptyLines, string? author, bool overwrite) =>
                {
                    await BundleFilesAsync(languages, output, note, sort, removeEmptyLines, author, overwrite);
                },
                languagesOption,
                outputOption,
                noteOption,
                sortOption,
                removeEmptyLinesOption,
                authorOption,
                overwriteOption);

            rootCommand.AddCommand(bundleCommand);

            // הרצת הפקודה
            return await rootCommand.InvokeAsync(args);
        }

        static async Task BundleFilesAsync(
            string[] languages,
            FileInfo output,
            bool note,
            string sort,
            bool removeEmptyLines,
            string? author,
            bool overwrite)
        {
            // בדיקת חוקיות של הנתיב
            try
            {
                string fullPath = Path.GetFullPath(output.FullName);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Error: The output path '{output.FullName}' is invalid. Details: {ex.Message}");
                return;
            }
            //2 דרכים
            //ראשונה רק שואל האם לדרוס 

            // בדיקת קיום קובץ פלט והצעת אפשרות לדרוס
            //if (output.Exists && !overwrite)
            //{
            //    Console.WriteLine($"The file '{output.FullName}' already exists.");
            //    Console.Write("Do you want to overwrite it? (yes/no): ");
            //    string response = Console.ReadLine()?.Trim().ToLower();

            //    if (response != "yes")
            //    {
            //        Console.WriteLine("Operation canceled. File was not overwritten.");
            //        return;
            //    }
            //}

            //2
            // שואל האם לדרוס ליצור חדש או לבטל
            if (output.Exists && !overwrite)
            {
                Console.WriteLine($"\nThe file '{output.FullName}' already exists.");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Overwrite the file");
                Console.WriteLine("2. Create a new file with a different name");
                Console.WriteLine("3. Cancel the operation\n");


                Console.Write("Enter your choice (1/2/3): ");
                string userChoice = Console.ReadLine()?.Trim();

                switch (userChoice)
                {
                    case "1":
                        Console.WriteLine("Overwriting the existing file...");
                        break;

                    case "2":
                        Console.Write("Enter a new name for the output file (including path): ");
                        string newFileName = Console.ReadLine()?.Trim();

                        if (string.IsNullOrWhiteSpace(newFileName))
                        {
                            Console.WriteLine("Error: Invalid file name provided.\n");
                            return;
                        }

                        output = new FileInfo(newFileName);
                        Console.WriteLine($"The new file will be created at: {output.FullName}\n");
                        break;

                    case "3":
                        Console.WriteLine("Operation canceled by the user.\n");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Operation canceled.");
                        return;
                }
            }
            else
            {
                Console.WriteLine("Overwriting the existing file...");
            }


            // סינון קבצים לפי שפה
            var allowedExtensions = languages.Contains("all")
                ? new[] { "*" }
                : languages.Select(l => $".{l.Trim()}").ToArray();

            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
                                 .Where(f => allowedExtensions.Contains(Path.GetExtension(f)) &&
                                             !f.Contains("bin") && !f.Contains("debug"))
                                 .ToList();

            if (!files.Any())
            {
                Console.WriteLine("No files found matching the criteria.");
                return;
            }

            // מיון קבצים
            files = sort.ToLower() switch
            {
                "name" => files.OrderBy(f => Path.GetFileName(f)).ToList(),
                "type" => files.OrderBy(f => Path.GetExtension(f)).ToList(),
                _ => throw new ArgumentException("Invalid sort option. Must be 'name' or 'type'.")
            };

            // כתיבת תוכן לקובץ הפלט
            try
            {
                using var writer = new StreamWriter(output.FullName, append: false);

                if (!string.IsNullOrWhiteSpace(author))
                {
                    await writer.WriteLineAsync($"// Author : {author}\n");
                }


                foreach (var file in files)
                {
                    //שם הקובץ
                    var fileName = Path.GetFileName(file);
                    var fileType = Path.GetExtension(file);
                    await writer.WriteLineAsync($"// File: {fileName} ( Type : {fileType} )");// הדפסה + שורה ריקה בין הכותרת לתוכן

                    //await writer.WriteLineAsync();


                    if (note)
                    {
                        await writer.WriteLineAsync($"// Source: {file}\n\n");
                    }

                    var lines = await File.ReadAllLinesAsync(file);
                    if (removeEmptyLines)
                    {
                        lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                    }

                    foreach (var line in lines)
                    {
                        await writer.WriteLineAsync(line);
                    }
                    //בשביל הסדר שיהיה מופרד ובולט שיש הבדל ביין הקבצים
                    // שורה ריקה בין קבצים
                    await writer.WriteLineAsync("\n/* --------------------------------------------------------------------------------------------------------------------------------- */\n"); 


                }

                Console.WriteLine($"Bundle created successfully at: {output.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Unable to write to the file '{output.FullName}'. Details: {ex.Message}");

            }
        }
    }
    }
//Fib bundle -l "cs" -o "C:\Users\USER\Downloads\output_file.txt" -n -s "type" -r -a "פנינה הורביץ" -w
