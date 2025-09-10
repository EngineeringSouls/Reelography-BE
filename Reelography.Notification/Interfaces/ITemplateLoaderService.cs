namespace Reelography.Notification.Interfaces;

public interface ITemplateLoaderService
{
    Task<string> LoadTemplate(string templatePath,CancellationToken cancellationToken= default);
}