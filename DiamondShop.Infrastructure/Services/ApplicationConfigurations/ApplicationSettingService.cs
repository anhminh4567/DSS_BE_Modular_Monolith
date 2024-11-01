using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Infrastructure.Databases;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiamondShop.Infrastructure.Services.ApplicationConfigurations
{
    internal class ApplicationSettingService : IApplicationSettingService
    {
        //private readonly IMemoryCache _cache;
        private readonly DiamondShopDbContext _context;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public ApplicationSettingService(DiamondShopDbContext context, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _context = context;
            _optionsMonitor = optionsMonitor;
        }

        public object? Get(string key)
        {
            //var isThere = _cache.TryGetValue(key, out object value);
            //if (isThere is false)
            //{
            var getFromDb = _context.ApplicationSettings.Where(a => a.Name == key).FirstOrDefault();
            if (getFromDb is null)
                return null;
            return getFromDb.GetObjectFromJsonString();
            //var setValue = _cache.Set(key, getFromDb.GetObjectFromJsonString(), TimeSpan.FromDays(365 * 10));
            //return setValue;
            //}
            //return value;
        }

        public void ReloadAllConfiguration()
        {
            var settings = _context.ApplicationSettings.ToList();
            //foreach (var setting in settings)
            //{
            //    _cache.Remove(setting.Name);
            //    _cache.Set(setting.Name, setting.GetObjectFromJsonString(), TimeSpan.FromDays(365 * 10));
            //}
        }

        public object? Set(string key, object? value)
        {
            if (value is null)
                return null;
            var Type = value.GetType().AssemblyQualifiedName;
            var newValueAsString = JsonConvert.SerializeObject(value);//, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, }
            var getFromDb = _context.ApplicationSettings.FirstOrDefault(a => a.Name == key);
            if (getFromDb != null && getFromDb.Type == Type)
            {
                getFromDb.Value = newValueAsString;
                getFromDb.Type = Type;
                _context.Update(getFromDb);
                _context.SaveChanges();
            }
            else
            {
                _context.ApplicationSettings.Add(new ApplicationSettings() { Type = Type, Name = key, Value = newValueAsString });
                _context.SaveChanges();
            }
            //_cache.Remove(key);
            //var setValue = _cache.Set(key, value, TimeSpan.FromDays(365 * 10));
            MapObjectToSetting(value, _optionsMonitor.CurrentValue);
            return value;
            //return setValue;
        }

        public void InitConfiguration()
        {
            foreach (var kvp in ApplicationSettingGlobal.DEFAULTS)
            {
                var Type = kvp.Value.GetType().AssemblyQualifiedName;
                //_cache.Remove(kvp.Key);
                var getFromDb = _context.ApplicationSettings.FirstOrDefault(a => a.Name == kvp.Key);
                if (getFromDb is null || getFromDb.Type != Type)
                {
                    Set(kvp.Key, kvp.Value);
                }
                else
                {
                    var objectDb = getFromDb.GetObjectFromJsonString();
                    Set(kvp.Key, objectDb);
                }
            }
        }
        private void MapObjectToSetting(object tobeMappedObject, ApplicationSettingGlobal applicationSetting)
        {
            if (tobeMappedObject == null) return;

            var tobeMappedObjectType = tobeMappedObject.GetType();
            var targetProperties = typeof(ApplicationSettingGlobal).GetProperties();
            foreach (var targetProp in targetProperties)
            {
                if (tobeMappedObjectType == targetProp.PropertyType)
                {
                    targetProp.SetValue(applicationSetting, tobeMappedObject);
                    break; // Move to the next source property
                }
            }
        }
    }
}
