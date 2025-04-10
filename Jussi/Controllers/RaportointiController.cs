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
    /// RaportointiController vastaa raporttien ja tilastojen tuottamisesta
    /// </summary>
    public class RaportointiController
    {
        private readonly VillageNewbiesContext _context;

        /// <summary>
        /// Konstruktori
        /// </summary>
        public RaportointiController()
        {
            _context = new VillageNewbiesContext();
        }

        /// <summary>
        /// Hakee mökkien varausasteet aluetta kohti tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Dictionary, jossa avaimena alueen nimi ja arvona varausaste prosentteina</returns>
        public Dictionary<string, double> HaeMokkienVarausasteetAlueittain(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                var tulokset = new Dictionary<string, double>();
                var alueet = _context.Alueet.Include(a => a.Mokit).ToList();

                // Lasketaan aikavälin kokonaispäivien määrä
                int kokonaisPaivat = (int)(loppuPvm - alkuPvm).TotalDays;
                if (kokonaisPaivat <= 0)
                {
                    MessageBox.Show("Virheellinen aikaväli", "Virhe", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return tulokset;
                }

                foreach (var alue in alueet)
                {
                    if (alue.Mokit == null || !alue.Mokit.Any())
                    {
                        tulokset.Add(alue.Nimi, 0);
                        continue;
                    }

                    // Haetaan alueen mökkien ID:t
                    var mokkiIdt = alue.Mokit.Select(m => m.MokkiID).ToList();

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

                    // Lasketaan varausaste (varatut päivät / (kokonaispäivät * mökkien määrä))
                    double varausaste = (double)varatutPaivat / (kokonaisPaivat * alue.Mokit.Count) * 100;
                    tulokset.Add(alue.Nimi, Math.Round(varausaste, 2));
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe varausasteiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, double>();
            }
        }

        /// <summary>
        /// Hakee palveluiden käyttöasteet tiettynä ajanjaksona tietyllä alueella
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <param name="alueId">Alueen ID (null = kaikki alueet)</param>
        /// <returns>Dictionary, jossa avaimena palvelun nimi ja arvona varausten määrä</returns>
        public Dictionary<string, int> HaePalveluidenKayttoasteet(DateTime alkuPvm, DateTime loppuPvm, int? alueId = null)
        {
            try
            {
                // Haetaan palveluiden tiedot
                var palvelut = _context.Palvelut
                    .Where(p => !alueId.HasValue || p.AlueID == alueId.Value)
                    .ToList();
                var tulokset = palvelut.ToDictionary(p => p.Nimi, p => 0);

                // Haetaan varaukset aikavälillä
                var varaukset = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .Select(v => v.VarausID)
                    .ToList();

                if (varaukset.Any())
                {
                    // Haetaan varausten palvelut ja niiden lukumäärät
                    var varaustenPalvelut = _context.VaraustenPalvelut
                        .Include(vp => vp.Palvelu)
                        .Where(vp => varaukset.Contains(vp.VarausID) && 
                               (!alueId.HasValue || vp.Palvelu.AlueID == alueId.Value))
                        .ToList();

                    // Lasketaan kunkin palvelun käyttökerrat
                    foreach (var vp in varaustenPalvelut)
                    {
                        if (vp.Palvelu != null && tulokset.ContainsKey(vp.Palvelu.Nimi))
                        {
                            tulokset[vp.Palvelu.Nimi] += vp.Lukumaara;
                        }
                    }
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palveluiden käyttöasteiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Hakee majoitustulot alueittain tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Dictionary, jossa avaimena alueen nimi ja arvona majoitustulot</returns>
        public Dictionary<string, decimal> HaeMajoitustulotAlueittain(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                var tulokset = new Dictionary<string, decimal>();
                var alueet = _context.Alueet.ToList();

                foreach (var alue in alueet)
                {
                    // Haetaan alueen mökkien ID:t
                    var mokkiIdt = _context.Mokit
                        .Where(m => m.AlueID == alue.AlueID)
                        .Select(m => m.MokkiID)
                        .ToList();

                    // Haetaan varaukset, jotka koskevat alueen mökkejä ja osuvat aikavälille
                    var varaukset = _context.Varaukset
                        .Include(v => v.Mokki)
                        .Where(v => mokkiIdt.Contains(v.MokkiID) && 
                                    v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                        .ToList();

                    // Lasketaan majoitustulot
                    decimal tulot = 0;
                    foreach (var varaus in varaukset)
                    {
                        int vrk = (int)(varaus.LoppuPvm - varaus.AlkuPvm).TotalDays;
                        tulot += varaus.Mokki.Hinta * vrk;
                    }

                    tulokset.Add(alue.Nimi, tulot);
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe majoitustulojen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, decimal>();
            }
        }

        /// <summary>
        /// Hakee palvelutulot alueittain tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Dictionary, jossa avaimena alueen nimi ja arvona palvelutulot</returns>
        public Dictionary<string, decimal> HaePalvelutulotAlueittain(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                var tulokset = new Dictionary<string, decimal>();
                var alueet = _context.Alueet.ToList();

                foreach (var alue in alueet)
                {
                    // Haetaan alueen palveluiden ID:t
                    var palveluIdt = _context.Palvelut
                        .Where(p => p.AlueID == alue.AlueID)
                        .Select(p => p.PalveluID)
                        .ToList();

                    // Haetaan varaukset, jotka osuvat aikavälille
                    var varausIdt = _context.Varaukset
                        .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                        .Select(v => v.VarausID)
                        .ToList();

                    // Haetaan varausten palvelut, jotka koskevat alueen palveluita
                    var varaustenPalvelut = _context.VaraustenPalvelut
                        .Where(vp => varausIdt.Contains(vp.VarausID) && palveluIdt.Contains(vp.PalveluID))
                        .ToList();

                    // Lasketaan palvelutulot
                    decimal tulot = 0;
                    foreach (var vp in varaustenPalvelut)
                    {
                        tulot += vp.Hinta * vp.Lukumaara;
                    }

                    tulokset.Add(alue.Nimi, tulot);
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe palvelutulojen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, decimal>();
            }
        }

        /// <summary>
        /// Hakee kokonaistulot (majoitus + palvelut) alueittain tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Dictionary, jossa avaimena alueen nimi ja arvona kokonaistulot</returns>
        public Dictionary<string, decimal> HaeKokonaistulotAlueittain(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                var majoitustulot = HaeMajoitustulotAlueittain(alkuPvm, loppuPvm);
                var palvelutulot = HaePalvelutulotAlueittain(alkuPvm, loppuPvm);
                var tulokset = new Dictionary<string, decimal>();

                // Yhdistetään tulokset
                foreach (var alue in majoitustulot.Keys)
                {
                    decimal kokonaistulot = majoitustulot[alue];
                    if (palvelutulot.ContainsKey(alue))
                    {
                        kokonaistulot += palvelutulot[alue];
                    }
                    tulokset.Add(alue, kokonaistulot);
                }

                return tulokset;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe kokonaistulojen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<string, decimal>();
            }
        }

        /// <summary>
        /// Hakee suosituimmat mökit tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <param name="maara">Palautettavien mökkien määrä</param>
        /// <returns>Lista mökeistä varausmäärän mukaan järjestettynä</returns>
        public List<KeyValuePair<Mokki, int>> HaeSuosituimmatMokit(DateTime alkuPvm, DateTime loppuPvm, int maara = 10)
        {
            try
            {
                // Haetaan mökit ja niihin liittyvät varaukset
                var mokit = _context.Mokit
                    .Include(m => m.Alue)
                    .ToList();

                var varaukset = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .ToList();

                // Lasketaan kunkin mökin varausten määrä
                var mokkienVaraukset = new Dictionary<Mokki, int>();
                foreach (var mokki in mokit)
                {
                    int varaustenMaara = varaukset.Count(v => v.MokkiID == mokki.MokkiID);
                    mokkienVaraukset.Add(mokki, varaustenMaara);
                }

                // Järjestetään mökit varausten määrän mukaan ja rajataan tulokset
                return mokkienVaraukset
                    .OrderByDescending(kv => kv.Value)
                    .Take(maara)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe suosituimpien mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<KeyValuePair<Mokki, int>>();
            }
        }

        /// <summary>
        /// Hakee suosituimmat palvelut tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <param name="maara">Palautettavien palveluiden määrä</param>
        /// <param name="alueId">Alueen ID (null = kaikki alueet)</param>
        /// <returns>Lista palveluista käyttömäärän mukaan järjestettynä</returns>
        public List<KeyValuePair<Palvelu, int>> HaeSuosituimmatPalvelut(DateTime alkuPvm, DateTime loppuPvm, int maara = 10, int? alueId = null)
        {
            try
            {
                // Haetaan varaukset aikavälillä
                var varausIdt = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .Select(v => v.VarausID)
                    .ToList();

                // Haetaan palvelut ja niihin liittyvät varausten palvelut
                var palvelut = _context.Palvelut
                    .Include(p => p.Alue)
                    .Where(p => !alueId.HasValue || p.AlueID == alueId.Value)
                    .ToList();

                var varaustenPalvelut = _context.VaraustenPalvelut
                    .Where(vp => varausIdt.Contains(vp.VarausID))
                    .ToList();

                // Lasketaan kunkin palvelun käyttökerrat
                var palveluidenKaytto = new Dictionary<Palvelu, int>();
                foreach (var palvelu in palvelut)
                {
                    int kayttokerrat = varaustenPalvelut
                        .Where(vp => vp.PalveluID == palvelu.PalveluID)
                        .Sum(vp => vp.Lukumaara);
                    palveluidenKaytto.Add(palvelu, kayttokerrat);
                }

                // Järjestetään palvelut käyttökertojen mukaan ja rajataan tulokset
                return palveluidenKaytto
                    .OrderByDescending(kv => kv.Value)
                    .Take(maara)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe suosituimpien palveluiden haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<KeyValuePair<Palvelu, int>>();
            }
        }

        /// <summary>
        /// Hakee kaikki asiakkaat, joilla on varauksia tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <returns>Lista asiakkaista</returns>
        public List<Asiakas> HaeAsiakkaatJoillaVarauksia(DateTime alkuPvm, DateTime loppuPvm)
        {
            try
            {
                // Haetaan asiakkaat, joilla on varauksia annetulla aikavälillä
                var asiakasIdt = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .Select(v => v.AsiakasID)
                    .Distinct()
                    .ToList();

                return _context.Asiakkaat
                    .Where(a => asiakasIdt.Contains(a.AsiakasID))
                    .OrderBy(a => a.Sukunimi)
                    .ThenBy(a => a.Etunimi)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe asiakastietojen haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Asiakas>();
            }
        }

        /// <summary>
        /// Hakee tuottoisimmat mökit (eniten tuloja tuoneet) tiettynä ajanjaksona
        /// </summary>
        /// <param name="alkuPvm">Aikavälin alkupäivämäärä</param>
        /// <param name="loppuPvm">Aikavälin loppupäivämäärä</param>
        /// <param name="maara">Palautettavien mökkien määrä</param>
        /// <returns>Lista mökeistä tulojen mukaan järjestettynä</returns>
        public List<KeyValuePair<Mokki, decimal>> HaeTuottoisimmatMokit(DateTime alkuPvm, DateTime loppuPvm, int maara = 10)
        {
            try
            {
                // Haetaan mökit ja niihin liittyvät varaukset
                var mokit = _context.Mokit
                    .Include(m => m.Alue)
                    .ToList();

                var varaukset = _context.Varaukset
                    .Where(v => v.AlkuPvm >= alkuPvm && v.LoppuPvm <= loppuPvm)
                    .ToList();

                // Lasketaan kunkin mökin tuottamat tulot
                var mokkienTulot = new Dictionary<Mokki, decimal>();
                foreach (var mokki in mokit)
                {
                    decimal tulot = 0;
                    var mokinVaraukset = varaukset.Where(v => v.MokkiID == mokki.MokkiID);
                    
                    foreach (var varaus in mokinVaraukset)
                    {
                        int vrk = (int)(varaus.LoppuPvm - varaus.AlkuPvm).TotalDays;
                        tulot += mokki.Hinta * vrk;
                    }
                    
                    mokkienTulot.Add(mokki, tulot);
                }

                // Järjestetään mökit tulojen mukaan ja rajataan tulokset
                return mokkienTulot
                    .OrderByDescending(kv => kv.Value)
                    .Take(maara)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Virhe tuottoisimpien mökkien haussa: {ex.Message}", "Virhe", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<KeyValuePair<Mokki, decimal>>();
            }
        }
    }
} 