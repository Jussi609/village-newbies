using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VillageNewbies.Models
{
    /// <summary>
    /// Alue-luokka edustaa Village Newbies mökkivarausjärjestelmän aluetietoja
    /// </summary>
    public class Alue
    {
        /// <summary>
        /// Alueen yksilöivä tunniste
        /// </summary>
        public int AlueID { get; set; }

        /// <summary>
        /// Alueen nimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Nimi on pakollinen")]
        [StringLength(100, ErrorMessage = "Nimi voi olla enintään 100 merkkiä pitkä")]
        public string Nimi { get; set; }

        /// <summary>
        /// Alueen kuvaus
        /// </summary>
        [StringLength(500, ErrorMessage = "Kuvaus voi olla enintään 500 merkkiä pitkä")]
        public string Kuvaus { get; set; }

        /// <summary>
        /// Alueen sijainti
        /// </summary>
        [StringLength(200, ErrorMessage = "Sijainti voi olla enintään 200 merkkiä pitkä")]
        public string Sijainti { get; set; }

        /// <summary>
        /// Lista alueella olevista mökeistä (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<Mokki> Mokit { get; set; } = new List<Mokki>();

        /// <summary>
        /// Lista alueella tarjottavista palveluista (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<Palvelu> Palvelut { get; set; } = new List<Palvelu>();
    }
} 