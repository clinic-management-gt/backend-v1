namespace Clinica.Models.EntityFramework;

public partial class ContactEmail
{
    public int Id { get; set; }

    public int ContactId { get; set; }

    public int EmailId { get; set; }

    public virtual Email Email { get; set; } = null!;

    public virtual Contact Contact { get; set; } = null!;
}
