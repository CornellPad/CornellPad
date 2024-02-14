namespace CornellPad.Views;

public partial class CreateNoteView : ContentPage
{
	public CreateNoteView(CreateNoteViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}