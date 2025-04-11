using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VillageNewbies.Models;

namespace VillageNewbies.Services
{
    public class PalveluRaporttiService
    {
        private readonly DatabaseConnection _dbConnection;

        public PalveluRaporttiService()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        // Hae ostetut palvelut tietyltä aikaväliltä ja tietyllä alueella
        public async Task<List<PalveluRaportti>> GetOstetutPalvelutRaporttiAsync(DateTime alkuPvm, DateTime loppuPvm, int alueId = 0)
        {
            string query = @"
                SELECT 
                    p.palvelu_id,
                    p.nimi AS palvelun_nimi,
                    a.nimi AS alueen_nimi,
                    v.varattu_alkupvm,
                    v.varattu_loppupvm,
                    CONCAT(as.etunimi, ' ', as.sukunimi) AS asiakkaan_nimi,
                    vp.lkm,
                    p.hinta,
                    p.alv
                FROM 
                    varauksen_palvelut vp
                JOIN 
                    palvelu p ON vp.palvelu_id = p.palvelu_id
                JOIN 
                    varaus v ON vp.varaus_id = v.varaus_id
                JOIN 
                    asiakas as ON v.asiakas_id = as.asiakas_id
                JOIN 
                    alue a ON p.alue_id = a.alue_id
                WHERE 
                    v.varattu_alkupvm <= @loppuPvm
                    AND v.varattu_loppupvm >= @alkuPvm";

            // Jos alue on valittu, lisätään se hakuehtona
            if (alueId > 0)
            {
                query += " AND p.alue_id = @alueId";
            }

            query += " ORDER BY v.varattu_alkupvm, p.nimi";

            var parameters = new Dictionary<string, object>
            {
                { "@alkuPvm", alkuPvm },
                { "@loppuPvm", loppuPvm }
            };

            if (alueId > 0)
            {
                parameters.Add("@alueId", alueId);
            }

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query, parameters);
            
            return ConvertDataTableToPalveluRaporttiList(dataTable);
        }

        // Hae ei-varatut palvelut tietyltä aikaväliltä ja tietyllä alueella
        public async Task<List<Palvelu>> GetEiVaratutPalvelutAsync(DateTime alkuPvm, DateTime loppuPvm, int alueId = 0)
        {
            string query = @"
                SELECT 
                    p.*,
                    a.nimi AS alueen_nimi
                FROM 
                    palvelu p
                JOIN 
                    alue a ON p.alue_id = a.alue_id
                WHERE 
                    p.palvelu_id NOT IN (
                        SELECT DISTINCT vp.palvelu_id
                        FROM varauksen_palvelut vp
                        JOIN varaus v ON vp.varaus_id = v.varaus_id
                        WHERE v.varattu_alkupvm <= @loppuPvm
                        AND v.varattu_loppupvm >= @alkuPvm
                    )";

            // Jos alue on valittu, lisätään se hakuehtona
            if (alueId > 0)
            {
                query += " AND p.alue_id = @alueId";
            }

            query += " ORDER BY p.nimi";

            var parameters = new Dictionary<string, object>
            {
                { "@alkuPvm", alkuPvm },
                { "@loppuPvm", loppuPvm }
            };

            if (alueId > 0)
            {
                parameters.Add("@alueId", alueId);
            }

            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query, parameters);
            List<Palvelu> palvelut = new List<Palvelu>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                Palvelu palvelu = new Palvelu
                {
                    Palvelu_id = Convert.ToInt32(row["palvelu_id"]),
                    Alue_id = Convert.ToInt32(row["alue_id"]),
                    Nimi = row["nimi"].ToString(),
                    Kuvaus = row["kuvaus"].ToString(),
                    Hinta = Convert.ToDouble(row["hinta"]),
                    Alv = Convert.ToDouble(row["alv"])
                };
                
                palvelut.Add(palvelu);
            }
            
            return palvelut;
        }

        // Hae kaikki alueet
        public async Task<List<Alue>> GetAllAlueetAsync()
        {
            string query = "SELECT * FROM alue ORDER BY nimi";
            DataTable dataTable = await _dbConnection.ExecuteQueryAsync(query);
            List<Alue> alueet = new List<Alue>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                Alue alue = new Alue
                {
                    Alue_id = Convert.ToInt32(row["alue_id"]),
                    Nimi = row["nimi"].ToString()
                };
                
                alueet.Add(alue);
            }
            
            return alueet;
        }

        // Apumetodi: Muuntaa DataTable:n PalveluRaportti-listaksi
        private List<PalveluRaportti> ConvertDataTableToPalveluRaporttiList(DataTable dataTable)
        {
            List<PalveluRaportti> raportit = new List<PalveluRaportti>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                PalveluRaportti raportti = new PalveluRaportti
                {
                    Palvelu_id = Convert.ToInt32(row["palvelu_id"]),
                    PalvelunNimi = row["palvelun_nimi"].ToString(),
                    AlueenNimi = row["alueen_nimi"].ToString(),
                    VarattuAlkupvm = Convert.ToDateTime(row["varattu_alkupvm"]),
                    VarattuLoppupvm = Convert.ToDateTime(row["varattu_loppupvm"]),
                    AsiakaanNimi = row["asiakkaan_nimi"].ToString(),
                    Lkm = Convert.ToInt32(row["lkm"]),
                    Hinta = Convert.ToDouble(row["hinta"]),
                    Alv = Convert.ToDouble(row["alv"])
                };
                
                raportit.Add(raportti);
            }
            
            return raportit;
        }

        // Laske raportin yhteishinta
        public double LaskeRaportinYhteishinta(List<PalveluRaportti> raportit)
        {
            double yhteishinta = 0;
            foreach (var raportti in raportit)
            {
                yhteishinta += raportti.Yhteishinta;
            }
            return yhteishinta;
        }
    }
} 