using System;
using System.Linq;

namespace Eiffel.Messaging.Core.Attributes
{
    public class TopicNameAttribute : Attribute
    {
        public string TopicName { get; private set; }

        public TopicNameAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }

    public static class TopicNameExtensions
    {
        public static string GetTopicName(this object obj)
        {
            var attribute = obj.GetType().CustomAttributes.Single(x => x.AttributeType == typeof(TopicNameAttribute));
            var topicName = attribute.ConstructorArguments[0].Value as string;
            return topicName;
        }

        public static string GetTopic(this Type type)
        {
            var attribute = type.CustomAttributes.Single(x => x.AttributeType == typeof(TopicNameAttribute));
            var topicName = attribute.ConstructorArguments[0].Value as string;
            return topicName;
        }
    }
}
