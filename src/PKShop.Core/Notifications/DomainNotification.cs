using System;
using MediatR;
using PKShop.Core.Events;

namespace PKShop.Core.Notifications
{
    public class DomainNotification : Event
    {
        public Guid DomainNotificationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int Version { get; set; }

        public DomainNotification(string key, string value)
        {
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}