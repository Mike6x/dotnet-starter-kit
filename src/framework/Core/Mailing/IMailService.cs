namespace FSH.Framework.Core.Mailing;
public interface IMailService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}