using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VillageNewbies.Models;

namespace VillageNewbies.Controllers
{
    /// <summary>
    /// AsiakasController vastaa asiakastietojen käsittelystä ja asiakashallinnasta
    /// </summary>
    public class AsiakasController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public AsiakasController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee kaikki asiakkaat tietokannasta
        /// </summary>
        /// <returns>Lista asiakkaista</returns>
        public List<Asiakas> HaeKaikkiAsiakkaat()
        {
            try
            {
                return _context.Asiakkaat
                    .OrderBy(a => a.Sukunimi)
                    .ThenBy(a => a.Etunimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Asiakas>();
            }
        }

        /// <summary>
        /// Hakee asiakkaan ID:n perusteella
        /// </summary>
        /// <param name="asiakasId">Asiakkaan ID</param>
        /// <returns>Asiakas-olio tai null jos asiakasta ei löydy</returns>
        public Asiakas HaeAsiakasIdlla(int asiakasId)
        {
            try
            {
                return _context.Asiakkaat.Find(asiakasId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaan haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Hakee asiakkaita annetulla hakusanalla
        /// </summary>
        /// <param name="hakusana">Hakusana, jolla asiakkaita haetaan</param>
        /// <returns>Lista hakuehtoja vastaavista asiakkaista</returns>
        public List<Asiakas> HaeAsiakkaitaHakusanalla(string hakusana)
        {
            if (string.IsNullOrEmpty(hakusana))
                return HaeKaikkiAsiakkaat();

            try
            {
                hakusana = hakusana.ToLower();
                return _context.Asiakkaat
                    .Where(a => a.Etunimi.ToLower().Contains(hakusana) ||
                                a.Sukunimi.ToLower().Contains(hakusana) ||
                                a.Sahkoposti.ToLower().Contains(hakusana) ||
                                a.Puhelinnumero.Contains(hakusana) ||
                                (a.Yritys != null && a.Yritys.ToLower().Contains(hakusana)))
                    .OrderBy(a => a.Sukunimi)
                    .ThenBy(a => a.Etunimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Asiakas>();
            }
        }

        /// <summary>
        /// Lisää uuden asiakkaan tietokantaan
        /// </summary>
        /// <param name="asiakas">Lisättävä Asiakas-olio</param>
        /// <returns>True jos lisäys onnistui, muuten false</returns>
        public bool LisaaAsiakas(Asiakas asiakas)
        {
            if (asiakas == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (string.IsNullOrEmpty(asiakas.Etunimi) ||
                    string.IsNullOrEmpty(asiakas.Sukunimi) ||
                    string.IsNullOrEmpty(asiakas.Osoite) ||
                    string.IsNullOrEmpty(asiakas.Postinumero) ||
                    string.IsNullOrEmpty(asiakas.Postitoimipaikka) ||
                    string.IsNullOrEmpty(asiakas.Puhelinnumero) ||
                    string.IsNullOrEmpty(asiakas.Sahkoposti) ||
                    string.IsNullOrEmpty(asiakas.Tyyppi))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan jos asiakas on yritys, että yrityksen nimi ja Y-tunnus ovat olemassa
                if (asiakas.Tyyppi == "Yritys" &&
                    (string.IsNullOrEmpty(asiakas.Yritys) || string.IsNullOrEmpty(asiakas.YTunnus)))
                {
                    MessageBox.Show("Yritysasiakkaalle on annettava yrityksen nimi ja Y-tunnus", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Lisätään asiakas tietokantaan
                _context.Asiakkaat.Add(asiakas);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaan lisäyksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Päivittää olemassa olevan asiakkaan tiedot
        /// </summary>
        /// <param name="asiakas">Päivitettävä Asiakas-olio</param>
        /// <returns>True jos päivitys onnistui, muuten false</returns>
        public bool PaivitaAsiakas(Asiakas asiakas)
        {
            if (asiakas == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (string.IsNullOrEmpty(asiakas.Etunimi) ||
                    string.IsNullOrEmpty(asiakas.Sukunimi) ||
                    string.IsNullOrEmpty(asiakas.Osoite) ||
                    string.IsNullOrEmpty(asiakas.Postinumero) ||
                    string.IsNullOrEmpty(asiakas.Postitoimipaikka) ||
                    string.IsNullOrEmpty(asiakas.Puhelinnumero) ||
                    string.IsNullOrEmpty(asiakas.Sahkoposti) ||
                    string.IsNullOrEmpty(asiakas.Tyyppi))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan jos asiakas on yritys, että yrityksen nimi ja Y-tunnus ovat olemassa
                if (asiakas.Tyyppi == "Yritys" &&
                    (string.IsNullOrEmpty(asiakas.Yritys) || string.IsNullOrEmpty(asiakas.YTunnus)))
                {
                    MessageBox.Show("Yritysasiakkaalle on annettava yrityksen nimi ja Y-tunnus", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan päivitettävä asiakas tietokannasta
                var paivitettavaAsiakas = _context.Asiakkaat.Find(asiakas.AsiakasID);
                if (paivitettavaAsiakas == null)
                {
                    MessageBox.Show("Päivitettävää asiakasta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Päivitetään asiakkaan tiedot
                paivitettavaAsiakas.Etunimi = asiakas.Etunimi;
                paivitettavaAsiakas.Sukunimi = asiakas.Sukunimi;
                paivitettavaAsiakas.Osoite = asiakas.Osoite;
                paivitettavaAsiakas.Postinumero = asiakas.Postinumero;
                paivitettavaAsiakas.Postitoimipaikka = asiakas.Postitoimipaikka;
                paivitettavaAsiakas.Puhelinnumero = asiakas.Puhelinnumero;
                paivitettavaAsiakas.Sahkoposti = asiakas.Sahkoposti;
                paivitettavaAsiakas.Tyyppi = asiakas.Tyyppi;
                paivitettavaAsiakas.Yritys = asiakas.Yritys;
                paivitettavaAsiakas.YTunnus = asiakas.YTunnus;

                // Tallennetaan muutokset
                _context.Entry(paivitettavaAsiakas).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaan päivityksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa asiakkaan tietokannasta
        /// </summary>
        /// <param name="asiakasId">Poistettavan asiakkaan ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaAsiakas(int asiakasId)
        {
            try
            {
                // Haetaan poistettava asiakas
                var asiakas = _context.Asiakkaat.Find(asiakasId);
                if (asiakas == null)
                {
                    MessageBox.Show("Poistettavaa asiakasta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko asiakkaalla varauksia
                bool onVarauksia = _context.Varaukset.Any(v => v.AsiakasID == asiakasId);
                if (onVarauksia)
                {
                    MessageBox.Show("Asiakasta ei voi poistaa, koska hänellä on varauksia", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan asiakas
                _context.Asiakkaat.Remove(asiakas);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaan poistossa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Hakee asiakkaan varaushistorian
        /// </summary>
        /// <param name="asiakasId">Asiakkaan ID</param>
        /// <returns>Lista asiakkaan varauksista</returns>
        public List<Varaus> HaeAsiakkaanVaraukset(int asiakasId)
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Mokki)
                    .Include(v => v.VarauksenPalvelut)
                    .Where(v => v.AsiakasID == asiakasId)
                    .OrderByDescending(v => v.AlkuPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakkaan varausten haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Hakee asiakasmäärän asiakastyypin mukaan
        /// </summary>
        /// <returns>Dictionary, jossa avaimena asiakastyyppi ja arvona asiakkaiden määrä</returns>
        public Dictionary<string, int> HaeAsiakasmaaratTyypeittain()
        {
            try
            {
                return _context.Asiakkaat
                    .GroupBy(a => a.Tyyppi)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakasmäärien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, int>();
            }
        }
    }
} 