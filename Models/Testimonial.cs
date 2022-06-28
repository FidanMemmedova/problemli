using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lawyer.Models
{
    public class Testimonial
    {
        public int Id { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Position { get; set; }
        public string Image { get; set; }
        [NotMapped,Required]
        public IFormFile Photo { get; set; }
    }
}
