namespace GCodeUtil;

public class Settings
{
    public string GCodeToInsertBeforeToolChange { get; set; } = "";
    public string GCodeToInsertAfterToolChange { get; set; } = "M8,G4 P1";
}