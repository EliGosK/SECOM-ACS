
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SECOM.ACS.Identity
{
    /// <summary>
    /// Represents the link between a user and a role.
    /// </summary>
    public class IdentityUserRole  
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        [Key]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        [Required]
        public int RoleId { get; set; }
    }
}