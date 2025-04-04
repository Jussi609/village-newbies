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
    /// VarausController vastaa varausten käsittelystä ja hallinnasta
    /// </summary>
    public class VarausController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public VarausController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee kaikki varaukset tietokannasta
        /// </summary>
        /// <returns>Lista varauksista</returns>
        public List<Varaus> HaeKaikkiVaraukset()
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Include(v => v.Mokki)
                    .Include(v => v.Mokki.Alue)
                    .OrderByDescending(v => v.VarausPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varausten haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Hakee varauksen ID:n perusteella
        /// </summary>
        /// <param name="varausId">Varauksen ID</param>
        /// <returns>Varaus-olio tai null jos varausta ei löydy</returns>
        public Varaus HaeVarausIdlla(int varausId)
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Include(v => v.Mokki)
                    .Include(v => v.Mokki.Alue)
                    .Include(v => v.VarauksenPalvelut)
                    .Include(v => v.VarauksenPalvelut.Select(vp => vp.Palvelu))
                    .FirstOrDefault(v => v.VarausID == varausId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varauksen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Hakee varaukset aikavälin perusteella
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Lista varauksista annetulla aikavälillä</returns>
        public List<Varaus> HaeVarauksetAikavali(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Include(v => v.Mokki)
                    .Include(v => v.Mokki.Alue)
                    .Where(v => 
                        (v.AlkuPvm >= alkuPvm && v.AlkuPvm <= loppuPvm) || 
                        (v.LoppuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm) ||
                        (v.AlkuPvm <= alkuPvm && v.LoppuPvm >= loppuPvm))
                    .OrderBy(v => v.AlkuPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varausten haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Tarkistaa, onko mökki vapaana annetulla aikavälillä
        /// </summary>
        /// <param name="mokkiId">Mökin ID</param>
        /// <param name="alkuPvm">Varauksen alkupäivämäärä</param>
        /// <param name="loppuPvm">Varauksen loppupäivämäärä</param>
        /// <param name="varausId">Varauksen ID, jos päivitetään olemassa olevaa varausta</param>
        /// <returns>True jos mökki on vapaa, muuten false</returns>
        public bool OnkoMokkiVapaa(int mokkiId, DateTime alkuPvm, DateTime loppuPvm, int varausId = 0)
        {
            try
            {
                // Tarkistetaan onko mökkiä olemassa
                var mokki = _context.Mokit.Find(mokkiId);
                if (mokki == null)
                {
                    MessageBox.Show("Valittua mökkiä ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko päivämäärät loogisesti oikein
                if (alkuPvm >= loppuPvm)
                {
                    MessageBox.Show("Varauksen loppupäivämäärän on oltava myöhäisempi kuin alkupäivämäärä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko mökkiä varattu kyseiselle aikavälille
                var paallekkainen = _context.Varaukset
                    .Where(v => v.MokkiID == mokkiId && v.VarausID != varausId)
                    .Any(v => 
                        (v.AlkuPvm <= alkuPvm && v.LoppuPvm > alkuPvm) || 
                        (v.AlkuPvm < loppuPvm && v.LoppuPvm >= loppuPvm) ||
                        (v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm));

                return !paallekkainen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe mökin vapauden tarkistuksessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Hakee kaikki vapaat mökit annetulla aikavälillä
        /// </summary>
        /// <param name="alkuPvm">Varauksen alkupäivämäärä</param>
        /// <param name="loppuPvm">Varauksen loppupäivämäärä</param>
        /// <param name="alueId">Alueen ID, jos halutaan rajata tiettyyn alueeseen</param>
        /// <returns>Lista vapaista mökeistä</returns>
        public List<Mokki> HaeVapaatMokit(DateTime alkuPvm, DateTime loppuPvm, int? alueId = null)
        {
            try
            {
                // Tarkistetaan onko päivämäärät loogisesti oikein
                if (alkuPvm >= loppuPvm)
                {
                    MessageBox.Show("Varauksen loppupäivämäärän on oltava myöhäisempi kuin alkupäivämäärä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return new List<Mokki>();
                }

                // Haetaan kaikki mökit
                var query = _context.Mokit
                    .Include(m => m.Alue);

                // Rajataan alueella jos alueId annettu
                if (alueId.HasValue)
                {
                    query = query.Where(m => m.AlueID == alueId.Value);
                }

                // Haetaan kaikki mökit
                var mokit = query.ToList();

                // Haetaan varattujen mökkien ID:t annetulla aikavälillä
                var varatutMokkiIdt = _context.Varaukset
                    .Where(v => 
                        (v.AlkuPvm <= alkuPvm && v.LoppuPvm > alkuPvm) || 
                        (v.AlkuPvm < loppuPvm && v.LoppuPvm >= loppuPvm) ||
                        (v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm))
                    .Select(v => v.MokkiID)
                    .Distinct()
                    .ToList();

                // Palautetaan mökit, jotka eivät ole varattujen listalla
                return mokit.Where(m => !varatutMokkiIdt.Contains(m.MokkiID)).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe vapaiden mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Lisää uuden varauksen tietokantaan
        /// </summary>
        /// <param name="varaus">Lisättävä Varaus-olio</param>
        /// <returns>Varauksen ID jos lisäys onnistui, muuten -1</returns>
        public int LisaaVaraus(Varaus varaus)
        {
            if (varaus == null)
                return -1;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (varaus.AsiakasID <= 0 || varaus.MokkiID <= 0 || string.IsNullOrEmpty(varaus.Tila))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return -1;
                }

                // Tarkistetaan onko mökki vapaa
                if (!OnkoMokkiVapaa(varaus.MokkiID, varaus.AlkuPvm, varaus.LoppuPvm))
                {
                    MessageBox.Show("Valittu mökki ei ole vapaa valitulla aikavälillä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return -1;
                }

                // Asetetaan varauksen luontipäivämäärä
                varaus.VarausPvm = DateTime.Now;

                // Lisätään varaus tietokantaan
                _context.Varaukset.Add(varaus);
                _context.SaveChanges();
                return varaus.VarausID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varauksen lisäyksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        /// <summary>
        /// Päivittää olemassa olevan varauksen tiedot
        /// </summary>
        /// <param name="varaus">Päivitettävä Varaus-olio</param>
        /// <returns>True jos päivitys onnistui, muuten false</returns>
        public bool PaivitaVaraus(Varaus varaus)
        {
            if (varaus == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (varaus.AsiakasID <= 0 || varaus.MokkiID <= 0 || string.IsNullOrEmpty(varaus.Tila))
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan päivitettävä varaus tietokannasta
                var paivitettavaVaraus = _context.Varaukset.Find(varaus.VarausID);
                if (paivitettavaVaraus == null)
                {
                    MessageBox.Show("Päivitettävää varausta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko mökki vapaa, jos mökkiä tai päivämääriä on muutettu
                if (paivitettavaVaraus.MokkiID != varaus.MokkiID || 
                    paivitettavaVaraus.AlkuPvm != varaus.AlkuPvm ||
                    paivitettavaVaraus.LoppuPvm != varaus.LoppuPvm)
                {
                    if (!OnkoMokkiVapaa(varaus.MokkiID, varaus.AlkuPvm, varaus.LoppuPvm, varaus.VarausID))
                    {
                        MessageBox.Show("Valittu mökki ei ole vapaa valitulla aikavälillä", "Virhe", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // Päivitetään varauksen tiedot
                paivitettavaVaraus.AsiakasID = varaus.AsiakasID;
                paivitettavaVaraus.MokkiID = varaus.MokkiID;
                paivitettavaVaraus.AlkuPvm = varaus.AlkuPvm;
                paivitettavaVaraus.LoppuPvm = varaus.LoppuPvm;
                paivitettavaVaraus.Tila = varaus.Tila;
                paivitettavaVaraus.Lisatiedot = varaus.Lisatiedot;

                // Tallennetaan muutokset
                _context.Entry(paivitettavaVaraus).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varauksen päivityksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa varauksen tietokannasta
        /// </summary>
        /// <param name="varausId">Poistettavan varauksen ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaVaraus(int varausId)
        {
            try
            {
                // Haetaan poistettava varaus
                var varaus = _context.Varaukset.Find(varausId);
                if (varaus == null)
                {
                    MessageBox.Show("Poistettavaa varausta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan ensin kaikki varauksen palvelut
                var varauksenPalvelut = _context.VaraustenPalvelut.Where(vp => vp.VarausID == varausId).ToList();
                foreach (var vp in varauksenPalvelut)
                {
                    _context.VaraustenPalvelut.Remove(vp);
                }

                // Poistetaan varaus
                _context.Varaukset.Remove(varaus);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varauksen poistossa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Lisää palvelun varaukseen
        /// </summary>
        /// <param name="varauksenPalvelu">Lisättävä VarauksenPalvelu-olio</param>
        /// <returns>True jos lisäys onnistui, muuten false</returns>
        public bool LisaaPalveluVaraukseen(VarauksenPalvelu varauksenPalvelu)
        {
            if (varauksenPalvelu == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (varauksenPalvelu.VarausID <= 0 || varauksenPalvelu.PalveluID <= 0 || varauksenPalvelu.Lukumaara <= 0)
                {
                    MessageBox.Show("Kaikki pakolliset tiedot on täytettävä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan palvelun hinta
                var palvelu = _context.Palvelut.Find(varauksenPalvelu.PalveluID);
                if (palvelu == null)
                {
                    MessageBox.Show("Valittua palvelua ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Asetetaan palvelun hinta
                varauksenPalvelu.Hinta = palvelu.Hinta;

                // Lisätään palvelu varaukseen
                _context.VaraustenPalvelut.Add(varauksenPalvelu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun lisäyksessä varaukseen: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa palvelun varauksesta
        /// </summary>
        /// <param name="varauksenPalveluId">Poistettavan varauksen palvelun ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaPalveluVarauksesta(int varauksenPalveluId)
        {
            try
            {
                // Haetaan poistettava varauksen palvelu
                var varauksenPalvelu = _context.VaraustenPalvelut.Find(varauksenPalveluId);
                if (varauksenPalvelu == null)
                {
                    MessageBox.Show("Poistettavaa palvelua ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan palvelu varauksesta
                _context.VaraustenPalvelut.Remove(varauksenPalvelu);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelun poistossa varauksesta: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Laskee varauksen kokonaishinnan (mökki + palvelut)
        /// </summary>
        /// <param name="varausId">Varauksen ID</param>
        /// <returns>Varauksen kokonaishinta</returns>
        public decimal LaskeVarauksenKokonaishinta(int varausId)
        {
            try
            {
                // Haetaan varaus
                var varaus = HaeVarausIdlla(varausId);
                if (varaus == null)
                    return 0;

                // Mökin hinta
                decimal mokkiHinta = varaus.MokkiHinta;

                // Palveluiden hinnat
                decimal palvelutHinta = 0;
                if (varaus.VarauksenPalvelut != null)
                {
                    palvelutHinta = varaus.VarauksenPalvelut.Sum(vp => vp.KokonaisHinta);
                }

                return mokkiHinta + palvelutHinta;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varauksen kokonaishinnan laskemisessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Hakee varaukset tilan perusteella
        /// </summary>
        /// <param name="tila">Varauksen tila (esim. "Vahvistettu", "Peruttu")</param>
        /// <returns>Lista varauksista tilan perusteella</returns>
        public List<Varaus> HaeVarauksetTila(string tila)
        {
            try
            {
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Include(v => v.Mokki)
                    .Include(v => v.Mokki.Alue)
                    .Where(v => v.Tila == tila)
                    .OrderByDescending(v => v.VarausPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varausten haussa tilan perusteella: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }
    }
} 