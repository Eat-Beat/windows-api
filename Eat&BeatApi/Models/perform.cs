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
    
    public partial class perform
    {
        public int idRestaurant { get; set; }
        public int idMusician { get; set; }
        public int idPerform { get; set; }
        public Nullable<System.DateTime> dateTime { get; set; }
        public Nullable<decimal> budget { get; set; }
        public Nullable<byte> musicianRate { get; set; }
        public Nullable<byte> restaurantRate { get; set; }
    
        public virtual musician musician { get; set; }
        public virtual restaurant restaurant { get; set; }
    }
}
