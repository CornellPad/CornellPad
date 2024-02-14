/*******************************************************************
 Copyright 2024 Digital Brain Lice

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 or implied. See the License for the specific language governing
 permissions and limitations under the License.
 *******************************************************************/

using CommunityToolkit.Maui.Views;

namespace CornellPad.Popups;

public partial class CreateTopicPopup : Popup
{
    private CreateTopicViewModel _viewModel;

    public CreateTopicPopup(CreateTopicViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	async void OnOKButtonClicked(object sender, EventArgs e) => await CloseAsync(new TopicModel
    {
        TopicName = (_viewModel.TopicName is null) ? string.Empty : _viewModel.TopicName,
        GlyphFamily = (_viewModel.SelectedGlyph?.GlyphFamily is null) ? string.Empty : _viewModel.SelectedGlyph.GlyphFamily,
        GlyphValue = (_viewModel.SelectedGlyph?.GlyphValue is null) ? string.Empty : _viewModel.SelectedGlyph.GlyphValue
    });

    async void OnDismissButtonClicked(object sender, EventArgs e) => await CloseAsync(null);
}