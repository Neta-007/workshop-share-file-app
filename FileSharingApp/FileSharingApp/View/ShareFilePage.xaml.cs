using FileSharingApp.ViewModel;
using FileShareConnectivity;

namespace FileSharingApp.View;

public partial class ShareFilePage : ContentPage
{
    public ShareFilePage(ShareFileViewModel sfvm)
    {
        InitializeComponent();
        BindingContext = sfvm;
    }
}