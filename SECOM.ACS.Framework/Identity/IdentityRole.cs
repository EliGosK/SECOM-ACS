using Microsoft.AspNet.Identity;
using SECOM.ACS.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SECOM.ACS.Identity
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the role.</typeparam>
    public class IdentityRole : Role, IRole<int>
    {
        /// <summary>
        /// Default constructor for Role 
        /// </summary>
        public IdentityRole()
        {
           
        }
        /// <summary>
        /// Constructor that takes names as argument 
        /// </summary>
        /// <param name="name"></param>
        public IdentityRole(string name) : this()
        {
            Name = name;
        }

        public IdentityRole(string name, int id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// Role ID
        /// </summary>ana   .
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        [Required]
        public string Name { get; set; }

        public bool Enabled { get; set; }
    }
}
