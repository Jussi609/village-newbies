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
    /// MokkiController vastaa mökkien käsittelystä ja hallinnasta
    /// </summary>
    public class MokkiController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public MokkiController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee kaikki mökit tietokannasta
        /// </summary>
        /// <returns>Lista mökeistä</returns>
        public List<Mokki> HaeKaikkiMokit()
        {
            try
            {
                return _context.Mokit
                    .Include(m => m.Alue)
                    .OrderBy(m => m.Alue.Nimi)
                    .ThenBy(m => m.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Hakee mökin ID:n perusteella
        /// </summary>
        /// <param name="mokkiId">Mökin ID</param>
        /// <returns>Mokki-olio tai null jos mökkiä ei löydy</returns>
        public Mokki HaeMokkiIdlla(int mokkiId)
        {
            try
            {
                return _context.Mokit
                    .Include(m => m.Alue)
                    .FirstOrDefault(m => m.MokkiID == mokkiId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Hakee mökit alueen perusteella
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Lista alueen mökeistä</returns>
        public List<Mokki> HaeMokitAlueella(int alueId)
        {
            try
            {
                return _context.Mokit
                    .Where(m => m.AlueID == alueId)
                    .Include(m => m.Alue)
                    .OrderBy(m => m.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökkien haussa alueelta: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Hakee mökkejä annetulla hakusanalla
        /// </summary>
        /// <param name="hakusana">Hakusana, jolla mökkejä haetaan</param>
        /// <returns>Lista hakuehtoja vastaavista mökeistä</returns>
        public List<Mokki> HaeMokkejaHakusanalla(string hakusana)
        {
            if (string.IsNullOrEmpty(hakusana))
                return HaeKaikkiMokit();

            try
            {
                hakusana = hakusana.ToLower();
                return _context.Mokit
                    .Include(m => m.Alue)
                    .Where(m => m.Nimi.ToLower().Contains(hakusana) ||
                                m.Osoite.ToLower().Contains(hakusana) ||
                                m.Kuvaus.ToLower().Contains(hakusana) ||
                                m.Varustelu.ToLower().Contains(hakusana) ||
                                m.Alue.Nimi.ToLower().Contains(hakusana))
                    .OrderBy(m => m.Alue.Nimi)
                    .ThenBy(m => m.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Hakee mökit henkilömäärän perusteella
        /// </summary>
        /// <param name="henkilomaara">Haluttu henkilömäärä</param>
        /// <returns>Lista riittävän kapasiteetin mökeistä</returns>
        public List<Mokki> HaeMokitHenkilomaaralla(int henkilomaara)
        {
            try
            {
                return _context.Mokit
                    .Include(m => m.Alue)
                    .Where(m => m.Henkilomaara >= henkilomaara)
                    .OrderBy(m => m.Henkilomaara)
                    .ThenBy(m => m.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Lisää uuden mökin tietokantaan
        /// </summary>
        /// <param name="mokki">Lisättävä Mokki-olio</param>
        /// <returns>True jos lisäys onnistui, muuten false</returns>
        public bool LisaaMokki(Mokki mokki)
        {
            if (mokki == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (mokki.AlueID <= 0 || 
                    string.IsNullOrEmpty(mokki.Nimi) ||
                    string.IsNullOrEmpty(mokki.Osoite) ||
                    mokki.Henkilomaara <= 0 ||
                    mokki.Hinta <= 0 ||
                    string.IsNullOrEmpty(mokki.Omistaja) ||
                    string.IsNullOrEmpty(mokki.OmistajaTiedot))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alue olemassa
                var alue = _context.Alueet.Find(mokki.AlueID);
                if (alue == null)
                {
                    MessageBox.Show("Valittua aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Lisätään mökki tietokantaan
                _context.Mokit.Add(mokki);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin lisäyksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Päivittää olemassa olevan mökin tiedot
        /// </summary>
        /// <param name="mokki">Päivitettävä Mokki-olio</param>
        /// <returns>True jos päivitys onnistui, muuten false</returns>
        public bool PaivitaMokki(Mokki mokki)
        {
            if (mokki == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (mokki.AlueID <= 0 || 
                    string.IsNullOrEmpty(mokki.Nimi) ||
                    string.IsNullOrEmpty(mokki.Osoite) ||
                    mokki.Henkilomaara <= 0 ||
                    mokki.Hinta <= 0 ||
                    string.IsNullOrEmpty(mokki.Omistaja) ||
                    string.IsNullOrEmpty(mokki.OmistajaTiedot))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alue olemassa
                var alue = _context.Alueet.Find(mokki.AlueID);
                if (alue == null)
                {
                    MessageBox.Show("Valittua aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan päivitettävä mökki tietokannasta
                var paivitettavaMokki = _context.Mokit.Find(mokki.MokkiID);
                if (paivitettavaMokki == null)
                {
                    MessageBox.Show("Päivitettävää mökkiä ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Päivitetään mökin tiedot
                paivitettavaMokki.AlueID = mokki.AlueID;
                paivitettavaMokki.Nimi = mokki.Nimi;
                paivitettavaMokki.Osoite = mokki.Osoite;
                paivitettavaMokki.Kuvaus = mokki.Kuvaus;
                paivitettavaMokki.Henkilomaara = mokki.Henkilomaara;
                paivitettavaMokki.Varustelu = mokki.Varustelu;
                paivitettavaMokki.Hinta = mokki.Hinta;
                paivitettavaMokki.Omistaja = mokki.Omistaja;
                paivitettavaMokki.OmistajaTiedot = mokki.OmistajaTiedot;

                // Tallennetaan muutokset
                _context.Entry(paivitettavaMokki).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin päivityksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa mökin tietokannasta
        /// </summary>
        /// <param name="mokkiId">Poistettavan mökin ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaMokki(int mokkiId)
        {
            try
            {
                // Haetaan poistettava mökki
                var mokki = _context.Mokit.Find(mokkiId);
                if (mokki == null)
                {
                    MessageBox.Show("Poistettavaa mökkiä ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko mökillä varauksia
                bool onVarauksia = _context.Varaukset.Any(v => v.MokkiID == mokkiId);
                if (onVarauksia)
                {
                    MessageBox.Show("Mökkiä ei voi poistaa, koska sille on varauksia", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan mökki
                _context.Mokit.Remove(mokki);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin poistossa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Hakee mökin tulevat varaukset
        /// </summary>
        /// <param name="mokkiId">Mökin ID</param>
        /// <returns>Lista mökin tulevista varauksista</returns>
        public List<Varaus> HaeMokinTulevatVaraukset(int mokkiId)
        {
            try
            {
                var nyt = DateTime.Now;
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Where(v => v.MokkiID == mokkiId && v.LoppuPvm >= nyt)
                    .OrderBy(v => v.AlkuPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin varausten haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Hakee mökin varaushistorian
        /// </summary>
        /// <param name="mokkiId">Mökin ID</param>
        /// <returns>Lista mökin varauksista</returns>
        public List<Varaus> HaeMokinVaraushistoria(int mokkiId)
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Where(v => v.MokkiID == mokkiId)
                    .OrderByDescending(v => v.AlkuPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin varaushistorian haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Hakee alueiden tiedot listana
        /// </summary>
        /// <returns>Lista alueista</returns>
        public List<Alue> HaeKaikkiAlueet()
        {
            try
            {
                return _context.Alueet
                    .OrderBy(a => a.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Alue>();
            }
        }

        /// <summary>
        /// Laskee mökin keskimääräisen käyttöasteen (varatut päivät / kokonaispäivät)
        /// </summary>
        /// <param name="mokkiId">Mökin ID</param>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Käyttöaste prosentteina</returns>
        public double LaskeMokinKayttoaste(int mokkiId, DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                // Tarkistetaan onko aikaväli validi
                int kokonaisPaivat = (int)(loppuPvm - alkuPvm).TotalDays;
                if (kokonaisPaivat <= 0)
                {
                    MessageBox.Show("Virheellinen aikaväli", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return 0;
                }

                // Haetaan varaukset, jotka koskevat mökkiä ja osuvat aikavälille
                var varaukset = _context.Varaukset
                    .Where(v => v.MokkiID == mokkiId && 
                            ((v.AlkuPvm >= alkuPvm && v.AlkuPvm < loppuPvm) || 
                             (v.LoppuPvm > alkuPvm && v.LoppuPvm <= loppuPvm) ||
                             (v.AlkuPvm <= alkuPvm && v.LoppuPvm >= loppuPvm)))
                    .ToList();

                // Lasketaan varattujen päivien määrä
                int varatutPaivat = 0;
                foreach (var varaus in varaukset)
                {
                    // Määritetään varauksen alku- ja loppupäivät annetun aikavälin sisällä
                    DateTime varausAlku = varaus.AlkuPvm < alkuPvm ? alkuPvm : varaus.AlkuPvm;
                    DateTime varausLoppu = varaus.LoppuPvm > loppuPvm ? loppuPvm : varaus.LoppuPvm;

                    // Lisätään päivien määrä
                    varatutPaivat += (int)(varausLoppu - varausAlku).TotalDays;
                }

                // Lasketaan käyttöaste (varatut päivät / kokonaispäivät)
                return Math.Round((double)varatutPaivat / kokonaisPaivat * 100, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin käyttöasteen laskemisessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
} 