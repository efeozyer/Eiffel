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
        public static string GetTopic(this Type type)
        {
            var attribute = type.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(TopicNameAttribute));
            if (attribute == null)
            {
                throw new ArgumentNullException($"TopicName attribute must be specified on {type.AssemblyQualifiedName}");
            }

            var topicName = attribute.ConstructorArguments[0].Value as string;
            return topicName;
        }
    }
}
