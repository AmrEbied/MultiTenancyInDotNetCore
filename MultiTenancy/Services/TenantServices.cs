using Microsoft.Extensions.Options;
using MultiTenancy.Setting;

namespace MultiTenancy.Services
{
    public class TenantServices : ITenantServices
    {
        private readonly TenantSetting _tenantSettings;
        private HttpContext? _httpContext;
        private Tenant? _currentTenant;

        public TenantServices(IHttpContextAccessor contextAccessor, IOptions<TenantSetting> tenantSettings)
        {
            _httpContext = contextAccessor.HttpContext;
            _tenantSettings = tenantSettings.Value;

            if (_httpContext is not null)
            {
                //Check in _httpContext if have tenant
                if (_httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
                {
                    SetCurrentTenant(tenantId!);
                }
                else
                {
                    throw new Exception("No tenant provided!");
                }
            }
        }
        public string? GetConnectionString()
        {
            var currentConnectionString = _currentTenant is null
            ? _tenantSettings.Defaults.ConnectionString
            : _currentTenant.ConnectionString;

            return currentConnectionString;
        }

        public Tenant? GetCurrentTenant()
        {
            return _currentTenant;
        }

        public string? GetDBProvider()
        {
            return _tenantSettings.Defaults.DBProvider;
        }
        private void SetCurrentTenant(string tenantId)
        {
            _currentTenant = _tenantSettings.Tenant.FirstOrDefault(t => t.Id == tenantId);

            if (_currentTenant is null)
            {
                throw new Exception("Invalid tenant ID");
            }

            if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
            }
        }
    }
}
