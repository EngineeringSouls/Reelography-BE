using Microsoft.Extensions.Hosting;
using Reelography.Notification.Interfaces;

namespace Reelography.Notification.Services;

public class TemplateLoaderService(IHostEnvironment env): ITemplateLoaderService
{
    private readonly string _templatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
    public async Task<string> LoadTemplate(string templatePath,CancellationToken cancellationToken= default)
    {
        var absolute = Path.Combine(_templatesRoot, templatePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        var tpl = await File.ReadAllTextAsync(absolute, cancellationToken).ConfigureAwait(false);
        return tpl;
    }
}