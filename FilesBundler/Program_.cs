////fib bundle --output -מיקום הקובץ

//using System.CommandLine;

//var bundleOption = new Option<FileInfo>("--output", "File path and name");
////-- פקודת bundle -----------------------------------------------
//var bundlerCommand = new Command("bundle", "bundle code files to a single file");
//bundlerCommand.AddOption(bundleOption);
//bundlerCommand.SetHandler((output) =>
//{
//    try
//    {
//        File.Create(output.FullName);
//    Console.WriteLine($"File {output.FullName} was created");

//    }
//    catch (DirectoryNotFoundException ex)
//    {
//        Console.WriteLine("Error : File Phat Invalid");
//        //Console.WriteLine($"Error : {ex}");

//    }
//    //Console.WriteLine("bundle command");
//}, bundleOption);

////-- rootCommand  ---------------------------------------------------------

//var rootCommand = new RootCommand("Root command for File Bundler CLI");
//rootCommand.AddCommand(bundlerCommand);
//rootCommand.InvokeAsync(args);


