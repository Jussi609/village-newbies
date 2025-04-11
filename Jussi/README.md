# Village Newbies - Mökkivarausjärjestelmä

Village Newbies on .NET MAUI -pohjainen mökkivarausjärjestelmä, joka on toteutettu MVVM-arkkitehtuurilla. Sovellus mahdollistaa asiakkaiden hallinnan sekä ostettujen palvelujen raportoinnin.

## Toteutetut toiminnallisuudet

### 1. Asiakashallinta

Asiakashallintanäkymässä voi:
- Selata asiakaslistan asiakkaita
- Hakea asiakkaita nimen, sähköpostin tai puhelinnumeron perusteella
- Lisätä uusia asiakkaita
- Muokata asiakastietoja
- Poistaa asiakkaita (jos heillä ei ole varauksia)

### 2. Ostettujen palvelujen raportointi

Palveluraportointinäkymässä voi:
- Hakea tietoja ostetuista palveluista tietyltä aikaväliltä
- Rajata hakua alueen mukaan
- Nähdä yhteenvedon ostettujen palveluiden kokonaishinnoista
- Tarkastella palveluita, joita ei ole varattu valitulla aikavälillä

## Tekniset yksityiskohdat

Sovellus on rakennettu seuraavilla komponenteilla:

### Models - Tietomallit
- `Asiakas.cs` - Asiakkaiden hallintaan
- `Posti.cs` - Postinumeroihin ja toimipaikkoihin
- `Alue.cs` - Alueiden tiedot
- `Palvelu.cs` - Palveluiden hallintaan
- `Varaus.cs` - Varausten tiedot
- `VarauksenPalvelu.cs` - Varausten ja palveluiden yhdistämiseen
- `PalveluRaportti.cs` - Ostettujen palveluiden raportointiin

### Services - Tietokantahallinta
- `DatabaseConnection.cs` - MariaDB-yhteyden hallinta
- `AsiakasService.cs` - Asiakkaiden CRUD-toiminnot
- `PalveluRaporttiService.cs` - Ostettujen palvelujen raportointi

### ViewModels - Näkymämallit
- `ViewModelBase.cs` - Kaikkien ViewModeleiden perusta
- `AsiakasViewModel.cs` - Asiakashallinnan logiikka
- `PalveluRaporttiViewModel.cs` - Raportoinnin logiikka

### Views - Käyttöliittymä
- `AsiakasPage.xaml` - Asiakkaiden hallinnan näkymä
- `PalveluRaporttiPage.xaml` - Palveluraporttien näkymä

### Helpers - Apuluokat
- `RelayCommand.cs` - Komentojen hallinta MVVM-mallissa
- `Converters.cs` - Näkymien konvertterit

## Asennus ja käyttöönotto

1. Varmista, että sinulla on .NET MAUI -kehitysympäristö asennettuna
2. Kloonaa repositorio
3. Muokkaa tarvittaessa tietokantayhteysasetuksia `DatabaseConnection.cs`-tiedostossa
4. Varmista, että MariaDB-tietokanta on käynnissä ja saatavilla
5. Aja sovellus

## Tietokantarakenne

Sovellus käyttää MariaDB-tietokantaa, jonka rakenne sisältää seuraavat taulut:
- `asiakas` - Asiakastiedot
- `posti` - Postinumerot ja toimipaikat
- `alue` - Aluetiedot
- `palvelu` - Palvelutiedot
- `varaus` - Varaustiedot
- `varauksen_palvelut` - Varauksiin liittyvät palvelut

## Ohjeet muiden toiminnallisuuksien lisäämiseksi

### 1. Varausten hallinta

Varausten hallintaa varten:

1. Luo `VarausService.cs` Services-hakemistoon
   - Toteuta CRUD-toiminnot varausten hallintaan
   - Lisää metodit mökkien saatavuuden tarkistamiseen

2. Luo `VarausViewModel.cs` ViewModels-hakemistoon
   - Toteuta varausten hakeminen, lisääminen, muokkaaminen ja poistaminen
   - Toteuta mökkien saatavuushaku

3. Luo `VarausPage.xaml` Views-hakemistoon
   - Toteuta käyttöliittymä varausten hallintaan
   - Lisää kalenteri mökkien saatavuuden näyttämiseen

4. Rekisteröi VarausPage AppShell.xaml-tiedostoon:
   ```xml
   <FlyoutItem Title="Varausten hallinta" Icon="calendar.png">
       <ShellContent ContentTemplate="{DataTemplate views:VarausPage}" Route="varaukset" />
   </FlyoutItem>
   ```

5. Lisää reitti AppShell.xaml.cs-tiedostoon:
   ```csharp
   Routing.RegisterRoute("varaukset", typeof(Views.VarausPage));
   ```

### 2. Laskujen käsittely

Laskutusjärjestelmää varten:

1. Luo `Lasku.cs` Models-hakemistoon
   - Toteuta laskun tiedot (laskun numero, asiakas, summa, laskun päivämäärä, eräpäivä)

2. Luo `LaskuService.cs` Services-hakemistoon
   - Toteuta laskujen luominen, hakeminen ja merkintä maksetuksi
   - Varauksista laskujen generointi

3. Luo `LaskuViewModel.cs` ViewModels-hakemistoon
   - Toteuta laskujen listaus, haku ja maksetuksi merkintä

4. Luo `LaskuPage.xaml` Views-hakemistoon
   - Toteuta käyttöliittymä laskujen hallintaan ja laskutusten tulostamiseen

5. Rekisteröi LaskuPage AppShell.xaml-tiedostoon

### 3. Alueiden ja mökkien hallinta

Alueiden ja mökkien hallintaa varten:

1. Luo `MokkiService.cs` Services-hakemistoon
   - Toteuta mökkien CRUD-toiminnot
   - Toteuta alueiden CRUD-toiminnot

2. Luo `AlueJaMokkiViewModel.cs` ViewModels-hakemistoon
   - Toteuta alueiden ja mökkien hallintalogiikka

3. Luo `AlueJaMokkiPage.xaml` Views-hakemistoon
   - Toteuta käyttöliittymä mökkien ja alueiden hallintaan

4. Rekisteröi AlueJaMokkiPage AppShell.xaml-tiedostoon

### 4. Toimintotilastot ja raportit

Laajempien raporttien toteuttaminen:

1. Luo `RaporttiService.cs` Services-hakemistoon
   - Toteuta monipuoliset raportointitoiminnot, kuten tulostoimintoja eri aikaväleillä

2. Luo `ToimintoTilastoViewModel.cs` ViewModels-hakemistoon
   - Toteuta tilastojen esittämislogiikka

3. Luo `ToimintoTilastoPage.xaml` Views-hakemistoon
   - Toteuta käyttöliittymä, jossa mm. kaavioita ja taulukoita datan visualisointiin

## Vinkkejä toteutukseen

1. **Uusien näkymien lisääminen**
   - Seuraa olemassa olevien näkymien rakennetta
   - Noudata MVVM-arkkitehtuuria: Model-View-ViewModel

2. **Tietokantakyselyiden toteuttaminen**
   - Hyödynnä olemassa olevaa `DatabaseConnection.cs`-luokkaa
   - Käytä parametrisoituja kyselyitä SQL-injektioiden estämiseksi

3. **Komponenttien uudelleenkäyttö**
   - Hyödynnä jo luotuja konverttereita, komentoja ja apuluokkia
   - Laajenna tarvittaessa `ViewModelBase`-luokkaa uusilla yhteisillä toiminnoilla

4. **Käyttöliittymän yhtenäisyys**
   - Noudata samaa tyyliä kuin olemassa olevissa näkymissä
   - Hyödynnä App.xaml-tiedostossa määriteltyjä resursseja

5. **Testaus**
   - Testaa jokainen uusi ominaisuus huolellisesti ennen integraatiota
   - Varmista että toiminnallisuus toimii myös virhetilanteissa 