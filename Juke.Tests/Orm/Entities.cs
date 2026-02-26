namespace Juke.Tests.Orm;

public class Company {
    public long? ID { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
}

public class Contact {
    public long? ID { get; set; }
    public long? CompanyID { get; set; }
    public string? Name { get; set; }
    public string? Post { get; set; }
}

public class ContactItem {
    public uint? ID { get; set; }
    public uint? ContactID { get; set; }
    public string? ContactType { get; set; }
    public string? ContactInfo { get; set; }
    public string? Description { get; set; }
}