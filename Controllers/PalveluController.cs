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
    /// PalveluController vastaa palveluiden käsittelystä ja hallinnasta
    /// </summary>
    public class PalveluController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public PalveluController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee kaikki palvelut tietokannasta
        /// </summary>
        /// <returns>Lista palveluista</returns>
        public List<Palvelu> HaeKaikkiPalvelut()
        {
            try
            {
                return _context.Palvelut
                    .Include(p => p.Alue)
                    .OrderBy(p => p.Alue.Nimi)
                    .ThenBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }

        /// <summary>
        /// Hakee palvelun ID:n perusteella
        /// </summary>
        /// <param name="palveluId">Palvelun ID</param>
        /// <returns>Palvelu-olio tai null jos palvelua ei löydy</returns>
        public Palvelu HaePalveluIdlla(int palveluId)
        {
            try
            {
                return _context.Palvelut
                    .Include(p => p.Alue)
                    .FirstOrDefault(p => p.PalveluID == palveluId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Hakee palvelut alueen perusteella
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Lista alueen palveluista</returns>
        public List<Palvelu> HaePalvelutAlueella(int alueId)
        {
            try
            {
                return _context.Palvelut
                    .Where(p => p.AlueID == alueId)
                    .Include(p => p.Alue)
                    .OrderBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden haussa alueelta: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }

        /// <summary>
        /// Hakee palveluita annetulla hakusanalla
        /// </summary>
        /// <param name="hakusana">Hakusana, jolla palveluita haetaan</param>
        /// <returns>Lista hakuehtoja vastaavista palveluista</returns>
        public List<Palvelu> HaePalveluitaHakusanalla(string hakusana)
        {
            if (string.IsNullOrEmpty(hakusana))
                return HaeKaikkiPalvelut();

            try
            {
                hakusana = hakusana.ToLower();
                return _context.Palvelut
                    .Include(p => p.Alue)
                    .Where(p => p.Nimi.ToLower().Contains(hakusana) ||
                                p.Kuvaus.ToLower().Contains(hakusana) ||
                                p.Alue.Nimi.ToLower().Contains(hakusana))
                    .OrderBy(p => p.Alue.Nimi)
                    .ThenBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }

        /// <summary>
        /// Hakee palvelut hintaluokan perusteella
        /// </summary>
        /// <param name="minHinta">Minimi hinta</param>
        /// <param name="maxHinta">Maksimi hinta</param>
        /// <returns>Lista palveluista hintaluokassa</returns>
        public List<Palvelu> HaePalvelutHintaluokassa(decimal minHinta, decimal maxHinta)
        {
            try
            {
                return _context.Palvelut
                    .Include(p => p.Alue)
                    .Where(p => p.Hinta >= minHinta && p.Hinta <= maxHinta)
                    .OrderBy(p => p.Hinta)
                    .ThenBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }

        /// <summary>
        /// Lisää uuden palvelun tietokantaan
        /// </summary>
        /// <param name="palvelu">Lisättävä Palvelu-olio</param>
        /// <returns>True jos lisäys onnistui, muuten false</returns>
        public bool LisaaPalvelu(Palvelu palvelu)
        {
            if (palvelu == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (palvelu.AlueID <= 0 || 
                    string.IsNullOrEmpty(palvelu.Nimi) ||
                    string.IsNullOrEmpty(palvelu.Tyyppi) ||
                    palvelu.Hinta <= 0)
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alue olemassa
                var alue = _context.Alueet.Find(palvelu.AlueID);
                if (alue == null)
                {
                    MessageBox.Show("Valittua aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Lisätään palvelu tietokantaan
                _context.Palvelut.Add(palvelu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun lisäyksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Päivittää olemassa olevan palvelun tiedot
        /// </summary>
        /// <param name="palvelu">Päivitettävä Palvelu-olio</param>
        /// <returns>True jos päivitys onnistui, muuten false</returns>
        public bool PaivitaPalvelu(Palvelu palvelu)
        {
            if (palvelu == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (palvelu.AlueID <= 0 || 
                    string.IsNullOrEmpty(palvelu.Nimi) ||
                    string.IsNullOrEmpty(palvelu.Tyyppi) ||
                    palvelu.Hinta <= 0)
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alue olemassa
                var alue = _context.Alueet.Find(palvelu.AlueID);
                if (alue == null)
                {
                    MessageBox.Show("Valittua aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan päivitettävä palvelu tietokannasta
                var paivitettavaPalvelu = _context.Palvelut.Find(palvelu.PalveluID);
                if (paivitettavaPalvelu == null)
                {
                    MessageBox.Show("Päivitettävää palvelua ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Päivitetään palvelun tiedot
                paivitettavaPalvelu.AlueID = palvelu.AlueID;
                paivitettavaPalvelu.Nimi = palvelu.Nimi;
                paivitettavaPalvelu.Tyyppi = palvelu.Tyyppi;
                paivitettavaPalvelu.Kuvaus = palvelu.Kuvaus;
                paivitettavaPalvelu.Hinta = palvelu.Hinta;

                // Tallennetaan muutokset
                _context.Entry(paivitettavaPalvelu).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun päivityksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa palvelun tietokannasta
        /// </summary>
        /// <param name="palveluId">Poistettavan palvelun ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaPalvelu(int palveluId)
        {
            try
            {
                // Haetaan poistettava palvelu
                var palvelu = _context.Palvelut.Find(palveluId);
                if (palvelu == null)
                {
                    MessageBox.Show("Poistettavaa palvelua ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko palvelulla varauksia
                bool onVarauksia = _context.VaraustenPalvelut.Any(vp => vp.PalveluID == palveluId);
                if (onVarauksia)
                {
                    MessageBox.Show("Palvelua ei voi poistaa, koska sitä on käytetty varauksissa", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan palvelu
                _context.Palvelut.Remove(palvelu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun poistossa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Hakee palveluiden määrän tyypeittäin
        /// </summary>
        /// <returns>Dictionary, jossa avaimena palvelun tyyppi ja arvona palveluiden määrä</returns>
        public Dictionary<string, int> HaePalveluidenMaaratTyypeittain()
        {
            try
            {
                return _context.Palvelut
                    .GroupBy(p => p.Tyyppi)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden määrien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Hakee palveluiden määrän alueittain
        /// </summary>
        /// <returns>Dictionary, jossa avaimena alueen nimi ja arvona palveluiden määrä</returns>
        public Dictionary<string, int> HaePalveluidenMaaratAlueittain()
        {
            try
            {
                var alueet = _context.Alueet.ToList();
                var tulokset = new Dictionary<string, int>();

                foreach (var alue in alueet)
                {
                    int maara = _context.Palvelut.Count(p => p.AlueID == alue.AlueID);
                    tulokset.Add(alue.Nimi, maara);
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden määrien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Hakee palvelun käyttötiedot (kuinka monta kertaa varattu)
        /// </summary>
        /// <param name="palveluId">Palvelun ID</param>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä (null = ei aikarajausta)</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä (null = ei aikarajausta)</param>
        /// <returns>Käyttökertojen määrä</returns>
        public int HaePalvelunKayttomaara(int palveluId, DateTime? alkuPvm = null, DateTime? loppuPvm = null)
        {
            try
            {
                // Haetaan varaukset aikavälillä
                var query = _context.Varaukset.AsQueryable();
                
                if (alkuPvm.HasValue)
                {
                    query = query.Where(v => v.AlkuPvm >= alkuPvm.Value);
                }
                
                if (loppuPvm.HasValue)
                {
                    query = query.Where(v => v.LoppuPvm <= loppuPvm.Value);
                }
                
                var varausIdt = query.Select(v => v.VarausID).ToList();

                // Lasketaan palvelun käyttökerrat
                return _context.VaraustenPalvelut
                    .Where(vp => vp.PalveluID == palveluId && varausIdt.Contains(vp.VarausID))
                    .Sum(vp => vp.Lukumaara);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun käyttömäärän haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Hakee palvelun tulot tiettynä ajanjaksona
        /// </summary>
        /// <param name="palveluId">Palvelun ID</param>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Palvelusta saadut tulot</returns>
        public decimal HaePalvelunTulot(int palveluId, DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                // Haetaan varaukset aikavälillä
                var varausIdt = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .Select(v => v.VarausID)
                    .ToList();

                // Lasketaan palvelun tulot
                return _context.VaraustenPalvelut
                    .Where(vp => vp.PalveluID == palveluId && varausIdt.Contains(vp.VarausID))
                    .Sum(vp => vp.Hinta * vp.Lukumaara);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun tulojen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Hakee tietyn tyypin palvelut
        /// </summary>
        /// <param name="tyyppi">Palvelun tyyppi (esim. "Ohjelmapalvelu", "Ruokailu")</param>
        /// <returns>Lista tietyn tyypin palveluista</returns>
        public List<Palvelu> HaePalvelutTyypilla(string tyyppi)
        {
            try
            {
                return _context.Palvelut
                    .Include(p => p.Alue)
                    .Where(p => p.Tyyppi == tyyppi)
                    .OrderBy(p => p.Alue.Nimi)
                    .ThenBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }
    }
} 