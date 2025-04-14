using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace AlueetJaMokit
{
    public class DatabaseConnector
    {
        //Tietokannan tiedot yhdistämiseen
        private readonly string server = "127.0.0.1";
        private readonly string port = "3307";
        private readonly string uid = "root";
        private readonly string pwd = "Ruutti";
        private readonly string database = "vn";

        public DatabaseConnector() { }

        public MySqlConnection _getConnection()
        {
            string connectionString = $"Server={server};Port={port};uid={uid};password={pwd};database={database}";
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }




        //Lisätään tietokantametodi tietojen lataamiseksi tietokannasta kun sovellus avataan
        public List<Alue> HaeAlueetTietokannasta()
        {
            List<Alue> alueet = new List<Alue>();

            using var conn = _getConnection();
            conn.Open();

            string sql = "SELECT * FROM alue";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Alue a = new Alue
                {
                    AlueId = reader.GetInt32("alue_id"),
                    Nimi = reader.GetString("nimi"),
                    Mokit = new ObservableCollection<Mokki>()
                };
                alueet.Add(a);
            }
            conn.Close();

            conn.Open();

            string mokkiSql = "SELECT * FROM mokki";
            using var mokkiCmd = new MySqlCommand(mokkiSql, conn);
            using var mokkiReader = mokkiCmd.ExecuteReader();

            while (mokkiReader.Read())
            {
                Mokki m = new Mokki
                {
                    MokkiId = mokkiReader.GetInt32("mokki_id"),
                    AlueId = mokkiReader.GetInt32("alue_id"),
                    Postinumero = mokkiReader.GetString("postinro"),
                    Mokkinimi = mokkiReader.GetString("mokkinimi"),
                    Katuosoite = mokkiReader.GetString("katuosoite"),
                    Hinta = mokkiReader.GetDouble("hinta"),
                    Kuvaus = mokkiReader.GetString("kuvaus"),
                    Henkilomaara = mokkiReader.GetInt32("henkilomaara"),
                    Varustelu = mokkiReader.GetString("varustelu")
                };

                Alue? kohde = alueet.FirstOrDefault(a => a.AlueId == m.AlueId);
                kohde?.Mokit.Add(m);
            }
            return alueet;
        }







        //Lisätään tietokantametodi alueiden tallentamiseksi tietokantaan 
        public void TallennaAlueTietokantaan(Alue alue)
        {
            using var conn = _getConnection();
            conn.Open();

            string sql = "INSERT INTO alue (nimi) VALUES (@nimi)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nimi", alue.Nimi);

            cmd.ExecuteNonQuery();
        }




        //Lisätään tietokantametodi alueiden poistamiseksi tietokannasta
        public void PoistaAlueTietokannasta(int alueId)
        {
            using var conn = _getConnection();
            conn.Open();

            string sql = "DELETE FROM alue WHERE alue_id = @alueId";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@alueId", alueId);

            cmd.ExecuteNonQuery();
        }



        //Lisätään metodi alueiden päivittämiseen tietokannassa
        public void PaivitaAlueTietokantaan(Alue alue)
        {
            using var conn = _getConnection();
            conn.Open();

            string sql = "UPDATE alue SET nimi = @nimi WHERE alue_id = @alueId";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nimi", alue.Nimi);
            cmd.Parameters.AddWithValue("@alueId", alue.AlueId);

            cmd.ExecuteNonQuery();
        }




        //Lisätään tietokantametodi mökkien lisäämiseksi tai niiden tietojen
        //päivittämiseksi tietokannassa

        public void TallennaMokkiTietokantaan(Mokki mokki)
        {
            using var conn = _getConnection();
            conn.Open();


            string checkSql = "SELECT COUNT(*) FROM posti WHERE postinro = @postinro";
            using (var checkCmd = new MySqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@postinro", mokki.Postinumero);
                long count = (long)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    string insertPosti = "INSERT INTO posti (postinro, toimipaikka) VALUES (@postinro, @toimipaikka)";
                    using var insertCmd = new MySqlCommand(insertPosti, conn);
                    insertCmd.Parameters.AddWithValue("@postinro", mokki.Postinumero);
                    insertCmd.Parameters.AddWithValue("@toimipaikka", "Tuntematon");
                    insertCmd.ExecuteNonQuery();
                }
            }


            if (mokki.MokkiId == 0)
            {

                string insertsql = @"INSERT INTO mokki (alue_id, postinro, mokkinimi, katuosoite, hinta, kuvaus, henkilomaara, varustelu)
                   VALUES (@alueId, @postinro, @mokkinimi, @katuosoite, @hinta, @kuvaus, @henkilomaara, @varustelu)";

                using var insertcmd = new MySqlCommand(insertsql, conn);

                insertcmd.Parameters.AddWithValue("@alueId", mokki.AlueId);
                insertcmd.Parameters.AddWithValue("@postinro", mokki.Postinumero);
                insertcmd.Parameters.AddWithValue("@mokkinimi", mokki.Mokkinimi);
                insertcmd.Parameters.AddWithValue("@katuosoite", mokki.Katuosoite);
                insertcmd.Parameters.AddWithValue("@hinta", mokki.Hinta);
                insertcmd.Parameters.AddWithValue("@kuvaus", mokki.Kuvaus);
                insertcmd.Parameters.AddWithValue("@henkilomaara", mokki.Henkilomaara);
                insertcmd.Parameters.AddWithValue("@varustelu", mokki.Varustelu);

                insertcmd.ExecuteNonQuery();

                mokki.MokkiId = (int)insertcmd.LastInsertedId;

            }
            else
            {
                string updateSql = @"UPDATE mokki SET 
                                    postinro = @postinro,
                                    mokkinimi = @mokkinimi,
                                    katuosoite = @katuosoite,
                                    hinta = @hinta,
                                    kuvaus = @kuvaus,
                                    henkilomaara = @henkilomaara,
                                    varustelu = @varustelu
                                    WHERE mokki_id = @mokkiId";

                using var updateCmd = new MySqlCommand(updateSql, conn);

                updateCmd.Parameters.AddWithValue("@alueId", mokki.AlueId);
                updateCmd.Parameters.AddWithValue("@postinro", mokki.Postinumero);
                updateCmd.Parameters.AddWithValue("@mokkinimi", mokki.Mokkinimi);
                updateCmd.Parameters.AddWithValue("@katuosoite", mokki.Katuosoite);
                updateCmd.Parameters.AddWithValue("@hinta", mokki.Hinta);
                updateCmd.Parameters.AddWithValue("@kuvaus", mokki.Kuvaus);
                updateCmd.Parameters.AddWithValue("@henkilomaara", mokki.Henkilomaara);
                updateCmd.Parameters.AddWithValue("@varustelu", mokki.Varustelu);
                updateCmd.Parameters.AddWithValue("@mokkiId", mokki.MokkiId);

                updateCmd.ExecuteNonQuery();
            }
        }




        //Lisätään tietokantametodi mökkien poistamiseksi tietokannasta
        public void PoistaMokkiTietokannasta (int mokkiId)
        {
            using var conn = _getConnection();
            conn.Open ();

            string sql = "DELETE FROM mokki WHERE mokki_id = @mokkiId";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@mokkiId", mokkiId);

            cmd.ExecuteNonQuery ();
        }




    }
}

    



