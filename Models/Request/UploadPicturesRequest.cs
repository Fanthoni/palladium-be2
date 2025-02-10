namespace PalladiumBE;

public class UploadPicturesRequest
{
    public string[] Images { get; set; } = new string[] { String.Empty };
    public string ItemId { get; set; } = String.Empty;
}