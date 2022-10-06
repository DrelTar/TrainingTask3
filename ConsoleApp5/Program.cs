using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Transactions;
using System.Text;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;

// Open in Release verison. I'm sure there is some problem in debug for vs.
// This is console filemanager. It has many different features and little cool things.
// Main of them ofcourse is user setted colors which saved after session. However there are still a lot of work. 
// I hope you'll like it and understand what is going on.
// Sorry for my English and enjoy.
// P.S. Remember, it knows your name). 

class Program
{
    // Global variable shows path to current directory. Will be set in EnterNewDirectory function.
    static string currentPath;
    // Three global variables shows which console colors are in use at the moment. Will be set in Start function.
    static ConsoleColor mainColor;
    static ConsoleColor secondaryColor;
    static ConsoleColor errorColor;
    // Global array which ease storage of user's colors. Maybe there is way to do it without this array but i haven't found it. 
    static string[] colors = new string[3];

    /// <summary>
    /// Just Main function. Do nothing except starting program.
    /// </summary>
    static void Main()
    {
        Start();
    }

    /// <summary>
    /// Function which welcome user, give him some information and set colors and starting path.
    /// </summary>
    public static void Start()
    {
        // Setting global variables.
        colors = File.ReadAllText("colors.txt").Split();
        ColorMain(colors[0]);
        ColorSecondary(colors[1]);
        ColorError(colors[2]);
        // Just making some beauty and setting currentPath.
        EnterNewDirectory();
        // Welcoming user and tell them about 'help' command.
        WriteText("Welcome to Console File Manager or just CFM");
        WriteText("I hope you'll understand how to use it");
        WriteText("I tried to make interface quite intuitive and easy");
        WriteText("But if you'll have some problems I sincerely apologise");
        WriteText("For more information type 'help'");
        // Reading for new command
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which read and calulate user input. Sorry for long method, but I have no idea how to short it. 
    /// </summary>
    public static void ReadAndCalculateCommand()
    {
        // Setting color of user input.
        Console.ForegroundColor = secondaryColor;
        // Making some beauty.
        Console.Write("# ");
        // Reading user command.
        string[] command = Console.ReadLine().Split();
        // Checking if command take a lot of parameters or not. If it is not, adding Length of command to first word.
        // Its made to make user not to add trash at the end of command.
        if (command[0] != "mix" && command[0] != "concatenate")
        {
            command[0] += command.Length;
        }
        // Calulating user command.
        switch (command[0])
        {
            case "help1":
                Help();
                return;
            case "show2":
                Show(command[1]);
                return;
            case "change2":
                Change(command[1]);
                return;
            // If you know how to short open2 and open3 in one it's cool. I don't. That's not cool. 
            case "open2":
                Open(command[1], "utf-8");
                return;
            case "open3":
                Open(command[1], command[2]);
                return;
            case "copy3":
                Copy(command[1], command[2]);
                return;
            case "move3":
                Move(command[1], command[2]);
                return;
            case "delete2":
                Delete(command[1]);
                return;
            // Same as with open2 and open3.
            case "create2":
                Create(command[1], "utf-8");
                return;
            case "create3":
                Create(command[1], command[2]);
                return;
            case "concatenate":
                // Taking all elements of command except first. Look a bit complicated, maybe there is easier way.
                var tmpArray = new ArraySegment<string>(command, 1, command.Length - 1);
                Concatenate(tmpArray.ToArray());
                return;
            case "mix":
                // Same as above.
                tmpArray = new ArraySegment<string>(command, 1, command.Length - 1);
                Mix(tmpArray.ToArray());
                return;
            case "color3":
                Color(command[1], command[2]);
                return;
            case "clear1":
                Clear();
                return;
            case "exit1":
                Exit();
                return;
            // If user command is wrong, we just trying to read another one.
            default:
                ReadAndCalculateCommand();
                return;
        }
    }

    /// <summary>
    /// Function which tell user about all commands and rules. 
    /// </summary>
    public static void Help()
    {
        // Just telling user what is going on and which commands they can use. User. Can use. Hehe. 
        // Every command have structure of one WriteCommand with general information and maybe some WriteTexts with details.
        WriteText("Commands list: ");
        WriteCommand("'help'", "to... well... ugh... you know you already here so that's it");
        WriteCommand("'show *type*'", "to show list of drives or local directoies/files");
        WriteText("Types: 'drives', 'directories', 'files', 'all'");
        WriteText("'all' contains only directories and files, not drives");
        WriteCommand("'change *directory name*'", "to change current directory");
        WriteCommand("'open *file name*'" ,"to open chosen file in UTF-8 encoding");
        WriteCommand("'open *file name* *encoding name*'", "to open chosen file in chosen encoding");
        WriteText("Currently available encodings: 'utf-8', 'ascii', 'BigEndianUnicode', 'Unicode', 'utf-32', 'utf-7'");
        WriteCommand("'copy *copied file name* *destination file name*'", "to copy one file's content to another file");
        WriteText("Destination file can be only named by its local name without '../' stuff but copied file can have any path");
        WriteCommand("'move *file name* *directory name*'", "to take file from current directory and put into another");
        WriteText("File can only be named by its local name without '../' stuff but directory can have any path");
        WriteCommand("'delete *file/directory name*'", "to delete chosen file or directory");
        WriteCommand("'create *file name*'", "to create file in UTF-8 encoding");
        WriteCommand("'create *file name* *encoding name*'", "to create file in chosen encoding");
        WriteText("Currently available encodings: 'utf-8', 'ascii', 'BigEndianUnicode', 'Unicode', 'utf-32', 'utf-7'");
        WriteCommand("'concatenate *file names*'", "to concatenate two or more files in one");
        WriteText("You should write file names splitted by space");
        WriteText("Files' content will added to the first file and after that printed to the console");
        WriteCommand("'mix *file names*", "to do the same thing as concatenate but without printing result");
        WriteCommand("'color *type* *color*", "to change text color. Default: main - green, secondary - blue, error - red");
        WriteText("Types: 'main', 'secondary', 'error'");
        WriteText("Available colors: 'blue', 'cyan', 'green', 'white', 'yellow', 'red', 'magenta', 'gray'");
        WriteCommand("'clear'", "to clear console");
        WriteCommand("'exit'", "to exit file manager");
        WriteText("P.S. All file and directory can be writen as full path or as '../../filename' variant");
        WriteText("     Or you can use just 'filename' which means what you chosing from local files");
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculating users input for 'show' command and calling functions which print needed data to the console.
    /// </summary>
    /// <param name="type"> Shows which type of data we show to user. Can be 'all' 'directories' 'files' 'drives' and trash.
    /// </param>
    public static void Show(string type) 
    {
        // Checking user input. In case of wrong input do nothing.
        switch (type)
        {
            case "drives":
                ShowDrives();
                break;
            case "directories":
                ShowDirectories();
                break;
            case "files":
                ShowFiles();
                break;
            case "all":
                ShowDirectories();
                ShowFiles();
                break;
        }
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculating user input for 'change' command and setting new currentPath.
    /// </summary>
    /// <param name="path"> Shows which new path user tring to set. Can be any path string. </param>
    public static void Change(string path)
    {
        // Checking if directory exists. Telling user if it's not.
        if (!ParseDirectory(ref path))
        {
            WriteError("Error 404. Bad path. Check it and try again");
            ReadAndCalculateCommand();
        }
        // Setting new directory.
        Directory.SetCurrentDirectory(path);
        // Changing currentPath and making some beauty.
        EnterNewDirectory();
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'open' input and print chosen file in chosen encoding to console.
    /// Sorry for long method.  
    /// </summary>
    /// <param name="file"> Shows which file user tring to open. Can be any filename string. </param>
    /// <param name="encoding"> Shows in which encoding user trying to open file. Can be any encoding from below switch.
    /// </param>
    public static void Open(string file, string encoding)
    {
        // Checking if file exists. Telling user if it's not.
        if (!ParseFile(ref file))
        {
            WriteError("Error 404. Bad file name. Check it and try again");
            ReadAndCalculateCommand();
        }
        // Reading file content in needed encoding.
        string[] lines;
        switch (encoding.ToLower()) {
            case "utf-8":
                lines = File.ReadAllLines(file);
                break;
            case "ascii":
                lines = File.ReadAllLines(file, Encoding.ASCII);
                break;
            case "bigendianunicode":
                lines = File.ReadAllLines(file, Encoding.BigEndianUnicode);
                break;
            case "unicode":
                lines = File.ReadAllLines(file, Encoding.Unicode);
                break;
            case "utf-32":
                lines = File.ReadAllLines(file, Encoding.UTF32);
                break;
            case "utf-7":
                lines = File.ReadAllLines(file, Encoding.UTF7);
                break;
            // Telling user if encoding unexist.
            default:
                WriteError("Sorry, unsupported encoding. Check it and try again");
                ReadAndCalculateCommand();
                return;
        }
        // Printing file content to console.
        foreach (var line in lines)
        {
            WriteText(line);
        }
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'copy' file and trying to cope one file to another. 
    /// </summary>
    /// <param name="copiedFile"> Shows which file user trying to copy. Can be any filename string. </param>
    /// <param name="destinationFile"> Shows in which file user trying to copy. Can be only local filename string. Without '/',
    /// '\' and ':'. </param>
    public static void Copy(string copiedFile, string destinationFile)
    {
        // Checking if copied file exist and telling user if not.
        if (!ParseFile(ref copiedFile))
        {
            WriteError("Error 404. Bad copied file name. Check it and try again");
            ReadAndCalculateCommand();
        }
        // Checking if destination file exists.
        if (ParseFile(ref destinationFile))
        {
            // If it is, telling user about it and asking if they want to delete existed file.
            WriteError("Destination file already exists. Coping will delete all its content");
            if (!ValidateDeletion(destinationFile))
            {
                ReadAndCalculateCommand();
            }
            // If they want, deleting existing file.
            File.Delete(destinationFile);
        }
        else
        {
            // If it isn't, checking if filename is valid. I tell users what they can only use local names. 
            // But in fact I already forgot why. However answer surely exist. 
            if (!IsLocalName(destinationFile))
            {
                WriteError("Bad file name. Destination file can be only named by its local name, without using '../ stuff'");
                ReadAndCalculateCommand();
            }
            // Making path to destination file.
            destinationFile = currentPath + "/" + destinationFile;
        }
        // Coping file content to another, just created, file. 
        File.Copy(copiedFile, destinationFile);
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'move' input and trying to move file from one directory to another.
    /// </summary>
    /// <param name="file"> Shows which file user trying to move. Can only be local filename. </param>
    /// <param name="path"> Shows where user trying to move file. Can be any path string. </param>
    public static void Move(string file, string path)
    {
        // Checking if chosen directory exists.
        if (!ParseDirectory(ref path))
        {
            WriteError("Error 404. Bad path. Check it and try again");
            ReadAndCalculateCommand();
        }
        // Checking if filename is valid.
        if (!IsLocalName(file))
        {
            WriteError("Bad file name. File can be only named by its local name, without using '../ stuff'");
            ReadAndCalculateCommand();
        }
        // Checking if same filename already exist in chosen directory.
        path = path + "/" + file;
        if (ParseFile(ref path))
        {
            // If it is, asking for deletion of existed file.
            WriteError("File already exists in chosen direcectory. Moving will delete all its content");
            if (!ValidateDeletion(path))
            {
                ReadAndCalculateCommand();
            }
            File.Delete(path);
        }
        // Coping file to new directory and deleting it from old.
        File.Copy(file, path);
        File.Delete(file);
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'delete' input and trying to delete file or directory.
    /// </summary>
    /// <param name="path"> Shows which file or directory user trying to delete. Can be any filename or path string. </param>
    public static void Delete(string path)
    {
        // Telling user that file or directory unexist and there is nothing to delete. Your battle is over little deleter.
        if (!ParseDirectory(ref path) && !ParseFile(ref path))
        {
            WriteError("Error 404. Bad path. Check it and try again");
        }
        // Checking if file exist and asking for deletion.
        else if (ParseFile(ref path) && ValidateDeletion(path))
        { 
            File.Delete(path);
        }
        // Checking if directory exist and asking for deletion.
        else if (ParseDirectory(ref path) && ValidateDeletion(path))
        {
            Directory.Delete(path);
        }
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'create' input and trying to create file in chosen encoding.
    /// </summary>
    /// <param name="file"> Shows which file user tring to create. Can be any filename string. </param>
    /// <param name="encoding"> Shows in which encoding user trying to create file. Can be any encoding from below switch.
    public static void Create(string file, string encoding)
    {
        //Checking if file already exists and asking for deletin of existed one.
        if (ParseFile(ref file)) 
        {
            WriteError("File already exists. Creating will delete all its content");
            if (!ValidateDeletion(file))
            {
                ReadAndCalculateCommand();
            }
            File.Delete(file);
        }
        // Taling user text which will be in created file.
        string[] lines = ReadText();
        // Creating file and writing user text.
        switch (encoding.ToLower())
        {
            case "utf-8":
                File.WriteAllLines(file, lines);
                break;
            case "ascii":
                File.WriteAllLines(file, lines, Encoding.ASCII);
                break;
            case "bigendianunicode":
                File.WriteAllLines(file, lines, Encoding.BigEndianUnicode);
                break;
            case "unicode":
                File.WriteAllLines(file, lines, Encoding.Unicode);
                break;
            case "utf-32":
                File.WriteAllLines(file, lines, Encoding.UTF32);
                break;
            case "utf-7":
                File.WriteAllLines(file, lines, Encoding.UTF7);
                break;
            default:
                // Telling user about unsupported encoding. Although existed file already deleted. So sad.
                WriteError("Sorry, unsupported encoding. Check it and try again");
                break;
        }
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'concatenate' input and trying to concatenate some files and print them.
    /// </summary>
    /// <param name="files"> Shows which files user trying to concatenate. Can be array of any filename strings. </param>
    public static void Concatenate(string[] files)
    {
        // Creatin variable for result.
        string result = "";
        // Take file. Check file for existence. Add file content. Repeat.
        for (int i = 0; i < files.Length; ++i)
        {
            if (!ParseFile(ref files[i]))
            {
                WriteError($"Error 404. Bad {i + 1} file name. Check it and try again");
                ReadAndCalculateCommand();
            }
            result += File.ReadAllText(files[i]);
        }
        // Writing result to first file.
        File.WriteAllText(files[0], result);
        // Printing result to the console. 
        Open(files[0], "utf-8");
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Same as Concatenate but without printing.
    /// </summary>
    /// <param name="files"> Same as in Concatenate. </param>
    public static void Mix(string[] files) 
    {
        // Look into Concatenate function.
        string result = "";
        for (int i = 0; i < files.Length; ++i)
        {
            if (!ParseFile(ref files[i]))
            {
                WriteError($"Error 404. Bad {i + 1} file name. Check it and try again");
                ReadAndCalculateCommand();
            }
            result += File.ReadAllText(files[i]);
        }
        File.WriteAllText(files[0], result);
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which calculate user input for 'color' input and trying to call functions to change colors of console.
    /// </summary>
    /// <param name="type"> Shows which type of console colors user trying to change. Can be 'main' 'secondary' and 'error'.
    /// </param>
    /// <param name="color"> Shows which color user truing to set. Can be any color from long below switches. </param>
    public static void Color(string type, string color)
    {
        // Calculating type and calling needed functions.
        if (type == "main")
        {
            ColorMain(color);
        }
        else if (type == "secondary")
        {
            ColorSecondary(color);
        }
        else if (type == "error")
        {
            ColorError(color);
        }
        // Telling if type is wrong.
        else
        {
            WriteError("Bad type. Check it and try again");
        }
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which clear app console.
    /// </summary>
    public static void Clear()
    {
        // Clearing console.
        Console.Clear();
        // Making some beauty.
        EnterNewDirectory();
        ReadAndCalculateCommand();
    }

    /// <summary>
    /// Function which saving user setted colors into colors.txt file.
    /// </summary>
    public static void Exit()
    {
        // Deleting previous colors.txt.
        File.Delete("colors.txt");
        // Creating a new one.
        File.WriteAllText("colors.txt", $"{colors[0]} {colors[1]} {colors[2]}");
    }

    /// <summary>
    /// Function which printing all directories from the current one to the console.
    /// </summary>
    public static void ShowDirectories()
    {
        // Finding all directories.
        string[] directories = Directory.GetDirectories(currentPath);
        // Checking if there is at least one.
        if (directories.Length > 0)
        {
            // Printint directories one by one.
            WriteText("Local directories:");
            foreach (var directory in directories)
            {
                WriteText(directory);
            }
        }
        else
        {
            // Telling user that there is no directories.
            WriteText("No directories");
        }
    }

    /// <summary>
    /// Same thing as ShowDirectories but for files. Nothing intresting. For comments see Show Directories.
    /// </summary>
    public static void ShowFiles()
    {
        string[] files = Directory.GetFiles(currentPath);
        if (files.Length > 0)
        {
            WriteText("Local files:");
            foreach (var file in files)
            {
                WriteText(file);
            }
        }
        else
        {
            WriteText("No files");
        }
    }

    /// <summary>
    /// Function which printing all computer drives to the console.
    /// </summary>
    public static void ShowDrives()
    {
        // Finding all drives.
        DriveInfo[] drives = DriveInfo.GetDrives();
        // Printing them.
        WriteText("Available drives: ");
        foreach (var drive in drives)
        {
            WriteText(drive.Name);
        }
    }

    /// <summary>
    /// Function which just make beautiful things and setting currentPath. Nothing inresting.
    /// </summary>
    public static void EnterNewDirectory()
    {
        // Setting new currentPath.
        currentPath = Directory.GetCurrentDirectory();
        // Making beauty and telling user who are they and who are their computer.
        Console.Title = "CFM " + currentPath;
        Console.ForegroundColor = mainColor;
        Console.Write(Environment.UserName + "@" + Environment.MachineName);
        Console.ForegroundColor = secondaryColor;
        Console.Write(" :: ");
        Console.ForegroundColor = mainColor;
        Console.WriteLine(currentPath);
    }

    /// <summary>
    /// Function whichs read user text and returns it.
    /// </summary>
    /// <returns> User input splitted by lines. </returns>
    public static string[] ReadText()
    {
        // Telling them what to do.
        WriteText("Write your text. Empty line will end writing");
        // Creating list, cuase we don't no what they think. Maybe they will write "War and peace" there.
        List<string> lines = new List<string>();
        // Reading lines while there is no empty line.
        string line;
        do
        {
            Console.Write("# ");
            line = Console.ReadLine();
            lines.Add(line);
        } while (line != "");
        // Removing empty line.
        lines.RemoveAt(lines.Count() - 1);
        return lines.ToArray();
    }

    /// <summary>
    /// Function which make text better and pretier.
    /// </summary>
    /// <param name="text"> Shows which text gonna look beutiful today. </param>
    public static void WriteText(string text)
    {
        // Just making perfect look.
        Console.ForegroundColor = secondaryColor;
        Console.Write("# ");
        Console.ForegroundColor = mainColor;
        Console.WriteLine(text);
    }

    /// <summary>
    /// Just as the function above, but for commands to ease their reading.
    /// </summary>
    /// <param name="command"> Command which we want to look understandable. </param>
    /// <param name="text"> Explanation of command. </param>
    public static void WriteCommand(string command, string text)
    {
        Console.ForegroundColor = secondaryColor;
        Console.Write("# ");
        Console.ForegroundColor = mainColor;
        Console.Write(command);
        Console.ForegroundColor = secondaryColor;
        Console.Write(" :: ");
        Console.ForegroundColor = mainColor;
        Console.WriteLine(text);
    }

    /// <summary>
    /// Function which make errors look scary.
    /// </summary>
    /// <param name="error"> Spooky scary error which gonna tell user where they made a mistake. </param>
    public static void WriteError(string error)
    {
        Console.ForegroundColor = errorColor;
        Console.WriteLine($"# {error}");
    }

    /// <summary>
    /// Function which ask user if they really want to delete file or directory. 
    /// </summary>
    /// <param name="path"> Shows which file or directory gonna be deleted. Can be any filename or path string. </param>
    /// <returns> True if file or directory should be deleted. False if not. </returns>
    public static bool ValidateDeletion(string path)
    {
        // Asking user.
        Console.ForegroundColor = errorColor;
        Console.WriteLine($"# Are you sure you want to delete {path}? It can't be undone!");
        Console.WriteLine("# y/n");
        // Taking user answer. They must say 'y' or 'n'. Where is no maybe in killing files.
        string answer;
        do
        {
            Console.Write("# ");
            answer = Console.ReadLine();
        } while (answer != "y" && answer != "n");
        return (answer == "y") ? true : false;
    }

    /// <summary>
    /// Function which check if file exists and parse it to normal look. 
    /// </summary>
    /// <param name="file"> Shwos whcih file we gonna parse and check. </param>
    /// <returns> True if file exists. False if not. </returns>
    public static bool ParseFile(ref string file)
    {
        // Checking if filename was local.
        if (File.Exists(currentPath + "/" + file))
        {
            file = currentPath + "/" + file;
        }
        // Checking if file unexist.
        else if (!File.Exists(file))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Same as for ParseFile but for directories.
    /// </summary>
    /// <param name="directory"> Shows which directory we gonna check and parse. </param>
    /// <returns> True if directory exists. False if not. </returns>
    public static bool ParseDirectory(ref string directory)
    {
        if (Directory.Exists(currentPath + "/" + directory))
        {
            directory = currentPath + "/" + directory;
        }
        else if (!Directory.Exists(directory))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Function which check if filename is local(don't full and doesn't have '../' in itself). Surely can be extended.
    /// </summary>
    /// <param name="file"> Shows which file we gonna check. </param>
    /// <returns> True if it is. False if it isn't. </returns>
    public static bool IsLocalName(string file) => !file.Contains(":") &&
        !file.Contains("/") && !file.Contains("\\");

    /// <summary>
    /// Function which change color of mainColor and colors. Variation of colors can be extended.
    /// </summary>
    /// <param name="color">Shows which color user trying to set for mainColor. </param>
    public static void ColorMain(string color)
    {
        // Calculating color. 
        switch (color.ToLower())
        {
            case "blue":
                mainColor = ConsoleColor.Blue;
                break;
            case "cyan":
                mainColor = ConsoleColor.Cyan;
                break;
            case "gray":
                mainColor = ConsoleColor.Gray;
                break;
            case "green":
                mainColor = ConsoleColor.Green;
                break;
            case "red":
                mainColor = ConsoleColor.Red;
                break;
            case "white":
                mainColor = ConsoleColor.White;
                break;
            case "yellow":
                mainColor = ConsoleColor.Yellow;
                break;
            case "magenta":
                mainColor = ConsoleColor.Magenta;
                break;
            // Telling user that there is no such color.
            default:
                WriteError("Bad color. Check it and try again");
                return;
        }
        // Changing colors.
        colors[0] = color;
    }

    /// <summary>
    /// Everything is absolutely the same as for ColorMain but for secondary.
    /// </summary>
    /// <param name="color"> As above. </param>
    public static void ColorSecondary(string color)
    {
        switch (color.ToLower())
        {
            case "blue":
                secondaryColor = ConsoleColor.Blue;
                break;
            case "cyan":
                secondaryColor = ConsoleColor.Cyan;
                break;
            case "gray":
                secondaryColor = ConsoleColor.Gray;
                break;
            case "green":
                secondaryColor = ConsoleColor.Green;
                break;
            case "red":
                secondaryColor = ConsoleColor.Red;
                break;
            case "white":
                secondaryColor = ConsoleColor.White;
                break;
            case "yellow":
                secondaryColor = ConsoleColor.Yellow;
                break;
            case "magenta":
                secondaryColor = ConsoleColor.Magenta;
                break;
            default:
                WriteError("Bad color. Check it and try again");
                return;
        }
        colors[1] = color;
    }

    /// <summary>
    /// As above.
    /// </summary>
    /// <param name="color"> As above. </param>
    public static void ColorError(string color)
    {
        switch (color.ToLower())
        {
            case "blue":
                errorColor = ConsoleColor.Blue;
                break;
            case "cyan":
                errorColor = ConsoleColor.Cyan;
                break;
            case "gray":
                errorColor = ConsoleColor.Gray;
                break;
            case "green":
                errorColor = ConsoleColor.Green;
                break;
            case "red":
                errorColor = ConsoleColor.Red;
                break;
            case "white":
                errorColor = ConsoleColor.White;
                break;
            case "yellow":
                errorColor = ConsoleColor.Yellow;
                break;
            case "magenta":
                errorColor = ConsoleColor.Magenta;
                break;
            default:
                WriteError("Bad color. Check it and try again");
                return;
        }
        colors[2] = color;
    }
}