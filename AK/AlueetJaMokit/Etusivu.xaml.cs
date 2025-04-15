using MySqlConnector;

namespace AlueetJaMokit;

public partial class Etusivu : ContentPage
{
	public Etusivu()
	{
		InitializeComponent();
	}



    //Tietokantayhteyden testausnappi
    //T�m� poistuu valmiista ty�st�, on vain itselleni k�yt�ss� testiss�
    private async void OnDatabaseClicked(object sender, EventArgs e)
    {
        DatabaseConnector dbc = new DatabaseConnector();

        try
        {
            var conn = dbc._getConnection();
            conn.Open();
            await DisplayAlert("Onnistui", "Tietokantayhteys aukesi!", "OK");
            conn.Close();
        }
        catch (MySqlException ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

}