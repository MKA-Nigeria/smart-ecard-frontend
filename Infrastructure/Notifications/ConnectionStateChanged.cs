using Shared.Notifications;

namespace Infrastructure.Notifications;
public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;