namespace PalladiumBE;

public class UploadPicturesRequest
{
    public string[] Images {get; set; } = [String.Empty];
    public string ItemId {get; set;} = String.Empty;
}