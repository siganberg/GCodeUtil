namespace GCodeUtil;

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
        {
            ShowHelp();
            return;
        }
     
     
        var inputPath = args[0];
        var insertGCodes = args.Length >= 2 ? args[1].Split(',') : new[] { "M8\nG4 P1" };

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

        while (reader.ReadLine() is { } line)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("M6", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("M98", StringComparison.OrdinalIgnoreCase))
            {
                insertPending = true;
                writer.WriteLine(line);
                continue;
            }

            writer.WriteLine(line);
            
            if (insertPending && IsG0XyLine(trimmed))
            {
                foreach (var gcode in insertGCodes)
                {
                    writer.WriteLine(gcode.Trim());
                }
                insertPending = false;
            }
       
        }

        Console.WriteLine("Modified file saved to: " + outputPath);
    }

    static bool IsG0XyLine(string line)
    {
        if (!line.StartsWith("G0", StringComparison.OrdinalIgnoreCase) &&
            !line.StartsWith("G00", StringComparison.OrdinalIgnoreCase))
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
              gcode_to_insert      (Optional) One or more G-code commands to insert, comma-separated.
                                   If omitted, defaults to: M8

            Behavior:
              - Scans the file line-by-line.
              - After every M6 or M98, finds the first 'G0 X.. Y..' line and inserts the specified G-code(s) before it.
              - Writes the result to a new file: '<original_filename>_modified.gcode'

            Examples:
              GCodeModifier myfile.gcode
                → Inserts 'M8' before first G0 X.. Y.. after every M6 or M98

              GCodeModifier toolpath.gcode ""M8,G4 P1""
                → Inserts M8 and G4 P1 before first G0 X.. Y.. after M6 or M98

              GCodeModifier --help
                → Displays this help screen

            ");
    }
}