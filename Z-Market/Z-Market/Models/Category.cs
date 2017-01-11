using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Z_Market.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [StringLength(30, ErrorMessage = "The field {0} must contain between {2} and {1} characteres", MinimumLength =3)]
        [Required(ErrorMessage = "You must enter the field {0}")]
        [Display(Name = "Document Description")]
        public string Description { get; set; }
    }
}