using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VillageNewbies.Models
{
    /// <summary>
    /// Varaus-luokka edustaa Village Newbies mökkivarausjärjestelmän varaustietoja
    /// </summary>
    public class Varaus
    {
        /// <summary>
        /// Varauksen yksilöivä tunniste
        /// </summary>
        public int VarausID { get; set; }

        /// <summary>
        /// Asiakkaan ID
        /// </summary>
        [Required(ErrorMessage = "Asiakas on pakollinen")]
        public int AsiakasID { get; set; }

        /// <summary>
        /// Mökin ID
        /// </summary>
        [Required(ErrorMessage = "Mökki on pakollinen")]
        public int MokkiID { get; set; }

        /// <summary>
        /// Varauksen alkupäivä (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Alkupäivä on pakollinen")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlkuPvm { get; set; }

        /// <summary>
        /// Varauksen loppupäivä (pakollinen)
        /// </summary>
        [Required(ErrorMessage = "Loppupäivä on pakollinen")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LoppuPvm { get; set; }

        /// <summary>
        /// Varauksen tekopäivä
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime VarausPvm { get; set; } = DateTime.Now;

        /// <summary>
        /// Varauksen vahvistuspäivämäärä
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? VahvistusPvm { get; set; }

        /// <summary>
        /// Varauksen tila (Vahvistettu, Peruttu)
        /// </summary>
        [Required(ErrorMessage = "Tila on pakollinen")]
        [StringLength(20, ErrorMessage = "Tila voi olla enintään 20 merkkiä pitkä")]
        public string Tila { get; set; } = "Vahvistettu";

        /// <summary>
        /// Varauksen lisätiedot
        /// </summary>
        [StringLength(500, ErrorMessage = "Lisätiedot voivat olla enintään 500 merkkiä pitkä")]
        public string Lisatiedot { get; set; }

        /// <summary>
        /// Viittaus Asiakas-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Asiakas Asiakas { get; set; }

        /// <summary>
        /// Viittaus Mokki-olioon (navigointiominaisuus)
        /// </summary>
        public virtual Mokki Mokki { get; set; }

        /// <summary>
        /// Lista varaukseen liittyvistä palveluista (navigointiominaisuus)
        /// </summary>
        public virtual ICollection<VarauksenPalvelu> VarauksenPalvelut { get; set; } = new List<VarauksenPalvelu>();

        /// <summary>
        /// Laskee varauksen keston vuorokausina
        /// </summary>
        public int KestoVrk => (LoppuPvm - AlkuPvm).Days;

        /// <summary>
        /// Laskee varauksen mökin hinnan ilman palveluita
        /// </summary>
        public decimal MokkiHinta => (Mokki != null) ? Mokki.Hinta * KestoVrk : 0;
    }
} 