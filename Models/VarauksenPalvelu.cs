using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillageNewbies.Models
{
    /// <summary>
    /// VarauksenPalvelu-luokka edustaa Village Newbies mökkivarausjärjestelmän
    /// varauksiin liittyviä palveluita
    /// </summary>
    public class VarauksenPalvelu
    {
        /// <summary>
        /// VarauksenPalvelu-olion yksilöivä tunniste
        /// </summary>
        public int VarauksenPalveluID { get; set; }

        /// <summary>
        /// Varauksen ID
        /// </summary>
        [Required(ErrorMessage = "Varaus on pakollinen")]
        public int VarausID { get; set; }

        /// <summary>
        /// Palvelun ID
        /// </summary>
        [Required(ErrorMessage = "Palvelu on pakollinen")]
        public int PalveluID { get; set; }

        /// <summary>
        /// Palveluiden lukumäärä
        /// </summary>
        [Required(ErrorMessage = "Lukumäärä on pakollinen")]
        [Range(1, 100, ErrorMessage = "Lukumäärän tulee olla välillä 1-100")]
        public int Lukumaara { get; set; } = 1;

        /// <summary>
        /// Palvelun hinta tässä varauksessa
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Hinta { get; set; }

        /// <summary>
        /// Viittaus Varaus-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Varaus Varaus { get; set; }

        /// <summary>
        /// Viittaus Palvelu-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Palvelu Palvelu { get; set; }

        /// <summary>
        /// Laskee palveluiden kokonaishinnan
        /// </summary>
        [NotMapped]
        public decimal KokonaisHinta => Lukumaara * Hinta;
    }
} 