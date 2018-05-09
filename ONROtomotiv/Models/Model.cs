//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ONROtomotiv.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Model
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Model()
        {
            this.Sub_Model = new HashSet<Sub_Model>();
            this.Vehicles = new HashSet<Vehicle>();
        }
    
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public int SerieID { get; set; }
        public string ModelName { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual Series Series { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sub_Model> Sub_Model { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
