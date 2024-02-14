namespace CornellPad.Views;

public partial class SelectLibraryView : ContentPage
{
	public SelectLibraryView(SelectLibraryViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}