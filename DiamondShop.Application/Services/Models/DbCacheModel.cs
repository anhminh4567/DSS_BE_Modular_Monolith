using DiamondShop.Domain;
using Newtonsoft.Json;
using System.Reflection;

namespace DiamondShop.Application.Services.Models
{
    public class DbCacheModel
    {
        public string KeyId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime CreationTime { get; set; }
        public T? GetObjectFromJsonString<T>()
        {
            Assembly asm = typeof(DomainLayer).Assembly;
            var messageType = System.Type.GetType(this.Type);
            if (messageType == null)
                throw new InvalidOperationException($"Type {Type} not found in assembly {asm.FullName}");
            var parsedObject = JsonConvert.DeserializeObject<T>(Value);
            return parsedObject;
        }
    }
}
