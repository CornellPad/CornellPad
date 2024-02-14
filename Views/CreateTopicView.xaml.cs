namespace CornellPad.Views;

public partial class CreateTopicView : ContentPage
{
	public CreateTopicView(CreateTopicViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}