//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eat_BeatApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class chat
    {
        public int idRestaurant { get; set; }
        public int idMusician { get; set; }
        public int idSender { get; set; }
        public Nullable<bool> isMultimedia { get; set; }
        public string message { get; set; }
        public System.DateTime timestamp { get; set; }
    
        public virtual musician musician { get; set; }
        public virtual restaurant restaurant { get; set; }
        public virtual user user { get; set; }
    }
}
