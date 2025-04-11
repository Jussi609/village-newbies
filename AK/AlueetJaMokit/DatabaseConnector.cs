using System;
using System.Collections.Generic;
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




        //Lisätään tietokantametodi alueiden muokkaamiseen tietokannassa
        public void PaivitaAlueTietokantaan(Alue alue)
        {
            using var conn = _getConnection();
            conn.Open();

            string sql = "UPDATE alue SET nimi = @nimi WHERE alue_id = @alueId";
            using var cmd = new MySqlCommand( sql, conn);
            cmd.Parameters.AddWithValue("@nimi", alue.Nimi);
            cmd.Parameters.AddWithValue("@alueId", alue.AlueId);

            cmd.ExecuteNonQuery();
        }


    }

}

