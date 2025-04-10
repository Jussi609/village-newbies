using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VillageNewbies.Models
{
    /// <summary>
    /// Mokki-luokka edustaa Village Newbies mökkivarausjärjestelmän mökkitietoja
    /// </summary>
    public class Mokki
    {
        /// <summary>
        /// Mökin yksilöivä tunniste
        /// </summary>
        public int MokkiID { get; set; }

        /// <summary>
        /// Alueen ID, johon mökki kuuluu
        /// </summary>
        [Required(ErrorMessage = "Alue on pakollinen")]
        public int AlueID { get; set; }

        /// <summary>
        /// Mökin nimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Nimi on pakollinen")]
        [StringLength(100, ErrorMessage = "Nimi voi olla enintään 100 merkkiä pitkä")]
        public string Nimi { get; set; }

        /// <summary>
        /// Mökin osoite (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Osoite on pakollinen")]
        [StringLength(200, ErrorMessage = "Osoite voi olla enintään 200 merkkiä pitkä")]
        public string Osoite { get; set; }

        /// <summary>
        /// Mökin kuvaus
        /// </summary>
        [StringLength(500, ErrorMessage = "Kuvaus voi olla enintään 500 merkkiä pitkä")]
        public string Kuvaus { get; set; }

        /// <summary>
        /// Mökin henkilömäärä (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Henkilömäärä on pakollinen")]
        [Range(1, 50, ErrorMessage = "Henkilömäärän tulee olla välillä 1-50")]
        public int Henkilomaara { get; set; }

        /// <summary>
        /// Mökin varustelu
        /// </summary>
        [StringLength(500, ErrorMessage = "Varustelu voi olla enintään 500 merkkiä pitkä")]
        public string Varustelu { get; set; }

        /// <summary>
        /// Mökin vuokrahinta per vuorokausi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Hinta on pakollinen")]
        [Range(0.01, 10000.00, ErrorMessage = "Hinnan tulee olla välillä 0.01 - 10000.00")]
        public decimal Hinta { get; set; }

        /// <summary>
        /// Mökin omistajan nimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Omistajan nimi on pakollinen")]
        [StringLength(200, ErrorMessage = "Omistajan nimi voi olla enintään 200 merkkiä pitkä")]
        public string Omistaja { get; set; }

        /// <summary>
        /// Mökin omistajan yhteystiedot (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Omistajan yhteystiedot ovat pakollisia")]
        [StringLength(500, ErrorMessage = "Omistajan yhteystiedot voivat olla enintään 500 merkkiä pitkä")]
        public string OmistajaTiedot { get; set; }

        /// <summary>
        /// Viittaus Alue-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Alue Alue { get; set; }

        /// <summary>
        /// Lista mökin varauksista (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<Varaus> Varaukset { get; set; } = new List<Varaus>();
    }
} 