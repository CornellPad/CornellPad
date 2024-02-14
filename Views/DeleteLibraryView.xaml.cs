namespace CornellPad.Views;

public partial class DeleteLibraryView : ContentPage
{
	public DeleteLibraryView(DeleteLibraryViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}