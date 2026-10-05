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
        set { if (SetField(ref _dateNaissance, value)) CalculerCommand.Rafraichir(); }
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
        int age = CalculerAge(DateNaissance);

        Resultat = $"{Nom}, vous avez {age} ans";
        MessageAge = age >= 18 ? "Majeur" : "Mineur";

        int jours = (int)(ProchainAnniversaire(DateNaissance) - DateTime.Today).TotalDays;
        JoursRestants = jours == 0
            ? "Joyeux anniversaire !"
            : $"{jours} jours restants avant le prochain anniversaire";

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
        var navigationParameter = new Dictionary<string, object>
        {
            { "nom", Nom },
            { "age", CalculerAge(DateNaissance).ToString() }
        };
        await Shell.Current.GoToAsync(nameof(Views.ResultatPage), navigationParameter);
    }

    private static int CalculerAge(DateTime dateNaissance)
    {
        int age = DateTime.Today.Year - dateNaissance.Year;
        if (dateNaissance.Date > DateTime.Today.AddYears(-age))
            age--;
        return age;
    }

    private static DateTime ProchainAnniversaire(DateTime dateNaissance)
    {
        DateTime anniv = dateNaissance.AddYears(DateTime.Today.Year - dateNaissance.Year);
        if (anniv.Date < DateTime.Today)
            anniv = anniv.AddYears(1);
        return anniv;
    }
}