using CornellPad.Services.Interfaces;

namespace CornellPad.Views;

public partial class TopicView : ContentPage
{
    public TopicView(TopicViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}