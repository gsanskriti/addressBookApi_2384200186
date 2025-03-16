using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Avoids nullability warning

        [Required, Phone, MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;  // Ensures default value

        [EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")] // Defines UserId as a foreign key to the User table
        public int UserId { get; set; }

        // Navigation property to establish the relationship
        public virtual User User { get; set; } = null!;  // Avoids nullability warning
    }
}
