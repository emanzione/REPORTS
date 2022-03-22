using Microsoft.AspNetCore.Mvc;

namespace MHLab.Reports.Backend.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ILogger<ReportsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveReport()
    {
        if (Request.HasFormContentType == false)
            return BadRequest();

        var report = new Report();

        await PopulateReportWithFiles(report, Request.Form.Files);
        PopulateReportWithData(report, Request.Form);

        // Do whatever you want with the report!
        
        return Ok();
    }

    private async Task PopulateReportWithFiles(Report report, IFormFileCollection files)
    {
        
        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            
            var attachment = new Attachment()
            {
                Name = file.FileName,
                Content = memoryStream.ToArray(),
            };
            report.AddAttachment(attachment);
        }
    }

    private void PopulateReportWithData(Report report, IFormCollection form)
    {
        foreach (var store in form)
        {
            var value = store.Value;
            var key   = store.Key;

            switch (key)
            {
                case "report-type":
                    report.Type = value;
                    break;
                case "report-email":
                    report.Email = value;
                    break;
                case "report-message":
                    report.Message = value;
                    break;
            }
        }
    }
}