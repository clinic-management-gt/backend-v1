using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;


public class Email
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public virtual ICollection<ContactEmail> ContactEmails { get; set; } = new List<ContactEmail>();
}
