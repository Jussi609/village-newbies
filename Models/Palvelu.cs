using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VillageNewbies.Models
{
    /// <summary>
    /// Palvelu-luokka edustaa Village Newbies mökkivarausjärjestelmän palvelutietoja
    /// </summary>
    public class Palvelu
    {
        /// <summary>
        /// Palvelun yksilöivä tunniste
        /// </summary>
        public int PalveluID { get; set; }

        /// <summary>
        /// Alueen ID, johon palvelu kuuluu
        /// </summary>
        [Required(ErrorMessage = "Alue on pakollinen")]
        public int AlueID { get; set; }

        /// <summary>
        /// Palvelun nimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Nimi on pakollinen")]
        [StringLength(100, ErrorMessage = "Nimi voi olla enintään 100 merkkiä pitkä")]
        public string Nimi { get; set; }

        /// <summary>
        /// Palvelun kuvaus
        /// </summary>
        [StringLength(500, ErrorMessage = "Kuvaus voi olla enintään 500 merkkiä pitkä")]
        public string Kuvaus { get; set; }

        /// <summary>
        /// Palvelun hinta (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Hinta on pakollinen")]
        [Range(0.01, 10000.00, ErrorMessage = "Hinnan tulee olla välillä 0.01 - 10000.00")]
        public decimal Hinta { get; set; }

        /// <summary>
        /// Palvelun tyyppi (esim. Safari, Vuokraus)
        /// </summary>
        [Required(ErrorMessage = "Tyyppi on pakollinen")]
        [StringLength(100, ErrorMessage = "Tyyppi voi olla enintään 100 merkkiä pitkä")]
        public string Tyyppi { get; set; }

        /// <summary>
        /// Viittaus Alue-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Alue Alue { get; set; }

        /// <summary>
        /// Lista varauksista, joissa palvelu on käytössä (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<VarauksenPalvelu> VarauksenPalvelut { get; set; } = new List<VarauksenPalvelu>();
    }
} 