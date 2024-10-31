using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Infrastructure.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases
{
    internal class ApplicationSettingsConfiguration : IEntityTypeConfiguration<ApplicationSettings>
    {
        public void Configure(EntityTypeBuilder<ApplicationSettings> builder)
        {
            builder.HasKey(a => a.Id);
        }
    }
    public class ApplicationSettings
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public object? GetObjectFromJsonString()
        {
            Assembly asm = typeof(Diamond).Assembly;
            var messageType = asm.GetType(Type);
            if(messageType == null)
                throw new InvalidOperationException($"Type {Type} not found in assembly {asm.FullName}");
            var parsedObject = JsonConvert.DeserializeObject(Value, messageType);
            return parsedObject;
        }
    }
}
