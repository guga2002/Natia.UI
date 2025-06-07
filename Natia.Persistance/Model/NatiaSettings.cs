namespace Natia.Persistance.Model;

public class NatiaSettings
{
    public string Language { get; set; }

    public string Model { get; set; }

    public NatiaSettings()
    {
        Model = "Nati";
        Language = "ka-GE";
    }
}
