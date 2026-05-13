using LexaCard.ViewModels;

namespace LexaCard.Views;

public partial class SetariPage : ContentPage
{
    public SetariPage(SetariViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
