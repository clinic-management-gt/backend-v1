namespace Clinica.Domain.Entities;


public class Email
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public virtual ICollection<ContactEmail> ContactEmails { get; set; } = new List<ContactEmail>();
}
