using FlashCards.ViewModels;

namespace FlashCards.Views;

public partial class SetariPage : ContentPage
{
    public SetariPage(SetariViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
