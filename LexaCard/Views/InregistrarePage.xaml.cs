using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class InregistrarePage : ContentPage
{
    public InregistrarePage(InregistrareViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
