using System.Windows.Input;

namespace CalculateurAge.ViewModels;

public class CalculateurViewModel : BaseViewModel
{
    private string _nom = "";
    private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private bool _resultatVisible;
    private string _messageAge = "";
    private string _joursRestants = "";

    public string Nom
    {
        get => _nom;
        set { if (SetField(ref _nom, value)) CalculerCommand.Rafraichir(); }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        set => SetField(ref _dateNaissance, value);
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    public string MessageAge
    {
        get => _messageAge;
        set => SetField(ref _messageAge, value);
    }

    public string JoursRestants
    {
        get => _joursRestants;
        set => SetField(ref _joursRestants, value);
    }

    public RelayCommand CalculerCommand { get; }
    public RelayCommand EffacerCommand { get; }
    public RelayCommand NaviguerResultatCommand { get; }

    public CalculateurViewModel()
    {
        CalculerCommand = new RelayCommand(Calculer, () => !string.IsNullOrWhiteSpace(Nom));
        EffacerCommand = new RelayCommand(Effacer);
        NaviguerResultatCommand = new RelayCommand(NaviguerResultat, () => ResultatVisible);
    }

    private void Calculer()
    {
        int age = DateTime.Today.Year - DateNaissance.Year;
        if (DateNaissance.Date > DateTime.Today.AddYears(-age)) age--;

        Resultat = $"{Nom}, vous avez {age} ans";
        
        MessageAge = age >= 18 ? "Majeur" : "Mineur";

        DateTime prochainAnniv = new DateTime(DateTime.Today.Year, DateNaissance.Month, DateNaissance.Day);
        if (prochainAnniv < DateTime.Today)
            prochainAnniv = prochainAnniv.AddYears(1);
        int jours = (int)(prochainAnniv - DateTime.Today).TotalDays;
        JoursRestants = $"{jours} jours restants avant le prochain anniversaire";

        ResultatVisible = true;
        NaviguerResultatCommand.Rafraichir();
    }

    private void Effacer()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        MessageAge = "";
        JoursRestants = "";
        ResultatVisible = false;
        NaviguerResultatCommand.Rafraichir();
    }

    private async void NaviguerResultat()
    {
        int age = DateTime.Today.Year - DateNaissance.Year;
        if (DateNaissance.Date > DateTime.Today.AddYears(-age)) age--;

        var navigationParameter = new Dictionary<string, object>
        {
            { "nom", Nom },
            { "age", age.ToString() }
        };
        await Microsoft.Maui.Controls.Shell.Current.GoToAsync(nameof(Views.ResultatPage), navigationParameter);
    }
}
