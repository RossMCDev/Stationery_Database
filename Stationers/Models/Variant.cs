//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stationers.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Variant
    {
        public int Variant_ID { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string Colour { get; set; }
        public string ColourDisplay { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public string Status { get; set; }
        public bool Stocked { get; set; }
        public int Stationary_ID { get; set; }

        public virtual Stationary Stationary { get; set; }
        public bool creationsucess { get; set; }
        public bool submit { get; set; }
        public bool markedfordeletion { get; set; }
        [Range(0, 1000)]
        public int Amount { get; set; }

    }
}