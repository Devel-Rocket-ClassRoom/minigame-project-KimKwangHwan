public enum DamageType
{
    Melee,
    Projectile,
    Magic,
    Flame,
    Electric
}
public struct DamageInfo
{
    public float damage;
    public DamageType type;
    public float knockBack;
}