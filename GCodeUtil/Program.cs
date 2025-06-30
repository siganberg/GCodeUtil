using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.RegularExpressions.Regex;

namespace GCodeUtil;

static class Program
{
    static void Main(string[] args)
    {
        var settings = GetSettings();
        
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
        {
            ShowHelp();
            return;
        }
     
        var inputPath = args[0];


        if (!File.Exists(inputPath))
        {
            Console.WriteLine("File not found: " + inputPath);
            return;
        }

        var outputPath = Path.Combine(
            Path.GetDirectoryName(inputPath) ?? string.Empty,
            Path.GetFileNameWithoutExtension(inputPath) + "_modified" + Path.GetExtension(inputPath)
        );

        var insertPending = false;

        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);

        var commandQueue = new Queue<string>(); 
        while (reader.ReadLine() is { } line)
        {
            var trimmed = line.Trim();

            if (trimmed.Contains("M6 ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Contains("M98 ", StringComparison.OrdinalIgnoreCase))
            {
                InsertGCodeBeforeToolChange(settings, trimmed, writer);
                insertPending = true;
                writer.WriteLine(line);
                commandQueue.Clear();
                continue;
            }

            //-- Moved other delay found, can also move spindle start if desired.
            if (trimmed.Contains("G4"))
            {
                commandQueue.Enqueue(trimmed);
                continue;
            }
            
            if (IsCoolantCommand(trimmed))
                continue;
            
            writer.WriteLine(trimmed);
            
            if (!insertPending || !IsG0XyLine(trimmed)) continue;

            //-- flushed all queue command. 
            while (commandQueue.Count > 0)
                writer.WriteLine(commandQueue.Dequeue());
            
            var gCodes = settings.GCodeToInsertBeforeToolChange.Split(",", StringSplitOptions.RemoveEmptyEntries);
            InsertGCodesAfterToolChange(settings, trimmed, writer);
            
            insertPending = false;

        }

        Console.WriteLine("Modified file saved to: " + outputPath);
    }

    private static void InsertGCodesAfterToolChange(Settings settings, string trimmed, StreamWriter writer)
    {
        var gCodes = settings.GCodeToInsertAfterToolChange.Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (gCodes.Length == 0) return; 
        var lineNumber = GetLineNumber(trimmed)+1;
        foreach (var gcode in gCodes)
        {
            writer.WriteLine((lineNumber > 0 ? $"N{lineNumber++} " : "") + gcode.Trim());
        }
    }

    private static void InsertGCodeBeforeToolChange(Settings settings, string trimmed, StreamWriter writer)
    {
        var gCodes = settings.GCodeToInsertBeforeToolChange.Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (gCodes.Length == 0) return; 
        var lineNumber = GetLineNumber(trimmed)-gCodes.Length;
        foreach (var gcode in gCodes)
        {
            writer.WriteLine((lineNumber > 0 ? $"N{lineNumber++} " : "") + gcode.Trim());
        }
    }

    public static Settings GetSettings()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var settingPath = currentDirectory + "/" + "settings.json";
        
        if (!File.Exists(settingPath))
        {
            Console.WriteLine("settings.json cannot be found.");
            Console.WriteLine("Generating new settings.json with default value.");
            using var file = File.CreateText(settingPath);
            var settings = new Settings();
            file.Write(JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault }));
            file.Close();
            return new Settings();
        }

        var configFile = File.ReadAllText(settingPath);
        var configModel = JsonSerializer.Deserialize<Settings>(configFile);
        if (configModel != null) return configModel;
        
        Console.WriteLine("settings.json has invalid content. Restoring default.");
        return new Settings();
    }


    private static bool IsCoolantCommand(string line)
    {
        return line.Contains("M8") || line.Contains("M08");
    }

    static int GetLineNumber(string line)
    {
        var match = Match(line, @"^N(\d+)\b");
        if (!match.Success) return 0;
        return int.TryParse(match.Groups[1].Value, out var result) 
            ? result 
            : 0;
    }

    static bool IsG0XyLine(string line)
    {
        if (!line.Contains("G0 ", StringComparison.OrdinalIgnoreCase) &&
            !line.Contains("G00 ", StringComparison.OrdinalIgnoreCase))
            return false;

        return line.IndexOf("X", StringComparison.OrdinalIgnoreCase) >= 0 &&
               line.IndexOf("Y", StringComparison.OrdinalIgnoreCase) >= 0;
    }
    
    static void ShowHelp()
    {
        Console.WriteLine(@"
            🛠 GCodeModifier - Insert G-code after M6 or M98

            Usage:
              GCodeModifier <input_file_path> [gcode_to_insert]

            Arguments:
              input_file_path      Path to the input .gcode file.
           
            Behavior:
              - Scans the file line-by-line.
              - Insert custom g-code command before or after every M6 or M98. 
              - Writes the result to a new file: '<original_filename>_modified.gcode'

            Examples:
              GCodeModifier myfile.gcode
                → Inserts 'M8' after the tool-change command.

              GCodeModifier toolpath.gcode ""M8,G4 P1""
                → Inserts M8 and G4 P1 after the tool-change command.

              GCodeModifier --help
                → Displays this help screen

            ");
    }
}