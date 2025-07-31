using System.Data.Common;

namespace Juke.Tests;

public class Company {
    public uint? ID { get; set; }
    public string? Name { get; set; }
}

public class Contact {
    public uint? ID { get; set; }
    public uint? CompanyID { get; set; }
    public string? Name { get; set; }
}

public class ContactItem {
    public uint? ID { get; set; }
    public uint? ContactID { get; set; }
    public string? ContactType { get; set; }
    public string? ContactInfo { get; set; }
    public string? Description { get; set; }
}