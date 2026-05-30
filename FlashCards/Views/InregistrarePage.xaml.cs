using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class InregistrarePage : ContentPage
{
    public InregistrarePage(InregistrareViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
