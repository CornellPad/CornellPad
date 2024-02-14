namespace CornellPad.Views;

public partial class CreateLibraryView : ContentPage
{
	public CreateLibraryView(CreateLibraryViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}