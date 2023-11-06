namespace MultiTenancy.Services
{
    public interface ITenantServices
    {
        public string? GetDBProvider();
        public string? GetConnectionString();
        public Tenant? GetCurrentTenant();
    }
}
