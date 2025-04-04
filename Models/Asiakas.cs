using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VillageNewbies.Models
{
    /// <summary>
    /// Asiakas-luokka edustaa Village Newbies mökkivarausjärjestelmän asiakastietoja
    /// </summary>
    public class Asiakas
    {
        /// <summary>
        /// Asiakkaan yksilöivä tunniste
        /// </summary>
        public int AsiakasID { get; set; }

        /// <summary>
        /// Asiakkaan etunimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Etunimi on pakollinen")]
        [StringLength(50, ErrorMessage = "Etunimi voi olla enintään 50 merkkiä pitkä")]
        public string Etunimi { get; set; }

        /// <summary>
        /// Asiakkaan sukunimi (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Sukunimi on pakollinen")]
        [StringLength(50, ErrorMessage = "Sukunimi voi olla enintään 50 merkkiä pitkä")]
        public string Sukunimi { get; set; }

        /// <summary>
        /// Asiakkaan osoite (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Osoite on pakollinen")]
        [StringLength(200, ErrorMessage = "Osoite voi olla enintään 200 merkkiä pitkä")]
        public string Osoite { get; set; }

        /// <summary>
        /// Asiakkaan postinumero (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Postinumero on pakollinen")]
        [StringLength(10, ErrorMessage = "Postinumero voi olla enintään 10 merkkiä pitkä")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postinumeron tulee olla 5 numeroa")]
        public string Postinumero { get; set; }

        /// <summary>
        /// Asiakkaan postitoimipaikka (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Postitoimipaikka on pakollinen")]
        [StringLength(50, ErrorMessage = "Postitoimipaikka voi olla enintään 50 merkkiä pitkä")]
        public string Postitoimipaikka { get; set; }

        /// <summary>
        /// Asiakkaan puhelinnumero (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Puhelinnumero on pakollinen")]
        [StringLength(20, ErrorMessage = "Puhelinnumero voi olla enintään 20 merkkiä pitkä")]
        [Phone(ErrorMessage = "Puhelinnumero on virheellinen")]
        public string Puhelinnumero { get; set; }

        /// <summary>
        /// Asiakkaan sähköpostiosoite (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Sähköposti on pakollinen")]
        [StringLength(100, ErrorMessage = "Sähköposti voi olla enintään 100 merkkiä pitkä")]
        [EmailAddress(ErrorMessage = "Sähköpostiosoite on virheellinen")]
        public string Sahkoposti { get; set; }

        /// <summary>
        /// Asiakkaan tyyppi (Yksityishenkilö, Yritys, Yhteisö)
        /// </summary>
        [Required(ErrorMessage = "Tyyppi on pakollinen")]
        [StringLength(20, ErrorMessage = "Tyyppi voi olla enintään 20 merkkiä pitkä")]
        public string Tyyppi { get; set; }

        /// <summary>
        /// Yrityksen nimi (pakollinen jos tyyppi on Yritys)
        /// </summary>
        [StringLength(100, ErrorMessage = "Yrityksen nimi voi olla enintään 100 merkkiä pitkä")]
        public string Yritys { get; set; }

        /// <summary>
        /// Y-tunnus (pakollinen jos tyyppi on Yritys)
        /// </summary>
        [StringLength(10, ErrorMessage = "Y-tunnus voi olla enintään 10 merkkiä pitkä")]
        [RegularExpression(@"^\d{7}-\d$", ErrorMessage = "Y-tunnus on virheellinen (muoto: 1234567-8)")]
        public string YTunnus { get; set; }

        /// <summary>
        /// Asiakkaan luontipäivämäärä
        /// </summary>
        public DateTime LuontiPvm { get; set; } = DateTime.Now;

        /// <summary>
        /// Lista asiakkaan varauksista (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<Varaus> Varaukset { get; set; } = new List<Varaus>();

        /// <summary>
        /// Palauttaa asiakkaan koko nimen (etunimi + sukunimi)
        /// </summary>
        public string KokoNimi => $"{Etunimi} {Sukunimi}";

        /// <summary>
        /// Palauttaa asiakkaan täyden osoitteen (osoite, postinumero, postitoimipaikka)
        /// </summary>
        public string TaysiOsoite => $"{Osoite}, {Postinumero} {Postitoimipaikka}";
    }
} 