namespace cCoder.AppSecurity.Models;

public class AppSecurityPackage
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string SourceApi { get; set; }

    public virtual ICollection<AppSecurityPackageItem> Items { get; set; }

    public AppSecurityPackage() { }

    public AppSecurityPackage(string name)
    {
        Name = name;
    }
}

