using Shared.Notifications;

namespace Infrastructure.Notifications;
public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}