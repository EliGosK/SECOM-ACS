using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SECOM.ACS.Identity
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the user.</typeparam>
    public class IdentityUser :  IUser 
    {
        [Key]
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        [Required]
        [StringLength(10)]
        public string UserName { get; set; }

        public DateTime LastSignedInDateTime { get; set; }
        public DateTime RegisteredDateTime { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

   
}
