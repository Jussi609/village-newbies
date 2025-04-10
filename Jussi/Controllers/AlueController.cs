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
    /// AlueController vastaa alueiden käsittelystä ja hallinnasta
    /// </summary>
    public class AlueController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public AlueController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee kaikki alueet tietokannasta
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
        /// Hakee alueen ID:n perusteella
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Alue-olio tai null jos aluetta ei löydy</returns>
        public Alue HaeAlueIdlla(int alueId)
        {
            try
            {
                return _context.Alueet
                    .FirstOrDefault(a => a.AlueID == alueId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Hakee alueita annetulla hakusanalla
        /// </summary>
        /// <param name="hakusana">Hakusana, jolla alueita haetaan</param>
        /// <returns>Lista hakuehtoja vastaavista alueista</returns>
        public List<Alue> HaeAlueitaHakusanalla(string hakusana)
        {
            if (string.IsNullOrEmpty(hakusana))
                return HaeKaikkiAlueet();

            try
            {
                hakusana = hakusana.ToLower();
                return _context.Alueet
                    .Where(a => a.Nimi.ToLower().Contains(hakusana))
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
        /// Lisää uuden alueen tietokantaan
        /// </summary>
        /// <param name="alue">Lisättävä Alue-olio</param>
        /// <returns>True jos lisäys onnistui, muuten false</returns>
        public bool LisaaAlue(Alue alue)
        {
            if (alue == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (string.IsNullOrEmpty(alue.Nimi))
                {
                    MessageBox.Show("Alueen nimi on pakollinen tieto", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko samannimistä aluetta jo olemassa
                bool onOlemassa = _context.Alueet.Any(a => a.Nimi.ToLower() == alue.Nimi.ToLower());
                if (onOlemassa)
                {
                    MessageBox.Show("Samanniminen alue on jo olemassa", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Lisätään alue tietokantaan
                _context.Alueet.Add(alue);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen lisäyksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Päivittää olemassa olevan alueen tiedot
        /// </summary>
        /// <param name="alue">Päivitettävä Alue-olio</param>
        /// <returns>True jos päivitys onnistui, muuten false</returns>
        public bool PaivitaAlue(Alue alue)
        {
            if (alue == null)
                return false;

            try
            {
                // Tarkistetaan pakollisten tietojen olemassaolo
                if (string.IsNullOrEmpty(alue.Nimi))
                {
                    MessageBox.Show("Alueen nimi on pakollinen tieto", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Haetaan päivitettävä alue tietokannasta
                var paivitettavaAlue = _context.Alueet.Find(alue.AlueID);
                if (paivitettavaAlue == null)
                {
                    MessageBox.Show("Päivitettävää aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko samannimistä aluetta jo olemassa (paitsi tämä alue itse)
                bool onOlemassa = _context.Alueet.Any(a => 
                    a.AlueID != alue.AlueID && 
                    a.Nimi.ToLower() == alue.Nimi.ToLower());
                if (onOlemassa)
                {
                    MessageBox.Show("Samanniminen alue on jo olemassa", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Päivitetään alueen tiedot
                paivitettavaAlue.Nimi = alue.Nimi;

                // Tallennetaan muutokset
                _context.Entry(paivitettavaAlue).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen päivityksessä: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Poistaa alueen tietokannasta
        /// </summary>
        /// <param name="alueId">Poistettavan alueen ID</param>
        /// <returns>True jos poisto onnistui, muuten false</returns>
        public bool PoistaAlue(int alueId)
        {
            try
            {
                // Haetaan poistettava alue
                var alue = _context.Alueet.Find(alueId);
                if (alue == null)
                {
                    MessageBox.Show("Poistettavaa aluetta ei löydy", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alueella mökkejä
                bool onMokkeja = _context.Mokit.Any(m => m.AlueID == alueId);
                if (onMokkeja)
                {
                    MessageBox.Show("Aluetta ei voi poistaa, koska sillä on mökkejä", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Tarkistetaan onko alueella palveluita
                bool onPalveluita = _context.Palvelut.Any(p => p.AlueID == alueId);
                if (onPalveluita)
                {
                    MessageBox.Show("Aluetta ei voi poistaa, koska sillä on palveluita", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Poistetaan alue
                _context.Alueet.Remove(alue);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen poistossa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Hakee alueen mökit
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Lista alueen mökeistä</returns>
        public List<Mokki> HaeAlueenMokit(int alueId)
        {
            try
            {
                return _context.Mokit
                    .Where(m => m.AlueID == alueId)
                    .OrderBy(m => m.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Mokki>();
            }
        }

        /// <summary>
        /// Hakee alueen palvelut
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Lista alueen palveluista</returns>
        public List<Palvelu> HaeAlueenPalvelut(int alueId)
        {
            try
            {
                return _context.Palvelut
                    .Where(p => p.AlueID == alueId)
                    .OrderBy(p => p.Nimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Palvelu>();
            }
        }

        /// <summary>
        /// Laskee alueen mökkien lukumäärän
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Mökkien lukumäärä</returns>
        public int LaskeAlueenMokkienLukumaara(int alueId)
        {
            try
            {
                return _context.Mokit.Count(m => m.AlueID == alueId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen mökkien laskemisessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Laskee alueen palveluiden lukumäärän
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <returns>Palveluiden lukumäärä</returns>
        public int LaskeAlueenPalveluidenLukumaara(int alueId)
        {
            try
            {
                return _context.Palvelut.Count(p => p.AlueID == alueId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen palveluiden laskemisessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Hakee alueen varaukset tiettynä ajanjaksona
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Lista alueen varauksista</returns>
        public List<Varaus> HaeAlueenVaraukset(int alueId, DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                // Haetaan alueen mökkien ID:t
                var mokkiIdt = _context.Mokit
                    .Where(m => m.AlueID == alueId)
                    .Select(m => m.MokkiID)
                    .ToList();

                // Haetaan varaukset, jotka koskevat alueen mökkejä ja osuvat aikavälille
                return _context.Varaukset
                    .Include(v => v.Asiakas)
                    .Include(v => v.Mokki)
                    .Where(v => mokkiIdt.Contains(v.MokkiID) && 
                               v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .OrderBy(v => v.AlkuPvm)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen varausten haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Varaus>();
            }
        }

        /// <summary>
        /// Laskee alueen mökkien keskimääräisen käyttöasteen tiettynä ajanjaksona
        /// </summary>
        /// <param name="alueId">Alueen ID</param>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Alueen mökkien keskimääräinen käyttöaste prosentteina</returns>
        public double LaskeAlueenKayttoaste(int alueId, DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                // Lasketaan aikavälin kokonaispäivien määrä
                int kokonaisPaivat = (int)(loppuPvm - alkuPvm).TotalDays;
                if (kokonaisPaivat <= 0)
                {
                    MessageBox.Show("Virheellinen aikaväli", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return 0;
                }

                // Haetaan alueen mökit
                var mokit = _context.Mokit.Where(m => m.AlueID == alueId).ToList();
                if (mokit.Count == 0)
                    return 0;

                // Haetaan mökkien ID:t
                var mokkiIdt = mokit.Select(m => m.MokkiID).ToList();

                // Haetaan varaukset, jotka koskevat alueen mökkejä ja osuvat aikavälille
                var varaukset = _context.Varaukset
                    .Where(v => mokkiIdt.Contains(v.MokkiID) && 
                            ((v.AlkuPvm >= alkuPvm && v.AlkuPvm < loppuPvm) || 
                             (v.LoppuPvm > alkuPvm && v.LoppuPvm <= loppuPvm) ||
                             (v.AlkuPvm <= alkuPvm && v.LoppuPvm >= loppuPvm)))
                    .ToList();

                // Lasketaan varattujen mökkipäivien määrä
                int varatutPaivat = 0;
                foreach (var varaus in varaukset)
                {
                    // Määritetään varauksen alku- ja loppupäivät annetun aikavälin sisällä
                    DateTime varausAlku = varaus.AlkuPvm < alkuPvm ? alkuPvm : varaus.AlkuPvm;
                    DateTime varausLoppu = varaus.LoppuPvm > loppuPvm ? loppuPvm : varaus.LoppuPvm;

                    // Lisätään päivien määrä
                    varatutPaivat += (int)(varausLoppu - varausAlku).TotalDays;
                }

                // Lasketaan käyttöaste (varatut päivät / (kokonaispäivät * mökkien määrä))
                return Math.Round((double)varatutPaivat / (kokonaisPaivat * mokit.Count) * 100, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe alueen käyttöasteen laskemisessa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }
    }
} 