namespace MultiTenancy.Setting
{
    public class TenantSetting
    {
        public Configrations Defaults { get; set; } = default!;
        public List<Tenant> Tenant { get; set; } = new();
    }
}
