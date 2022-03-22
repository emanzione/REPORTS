namespace MHLab.Reports.Backend.Sample;

public struct Attachment
{
    public string Name;
    public byte[] Content;
}
    
public class Report
{
    public string Type;
    public string Message;
    public string Email;

    public List<Attachment> Attachments;

    public Report()
    {
        Attachments = new List<Attachment>();
    }

    public void AddAttachment(Attachment attachment)
    {
        Attachments.Add(attachment);
    }
}