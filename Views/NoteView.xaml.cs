/*******************************************************************
 Copyright 2024 CornellPad

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

namespace CornellPad.Views;

public partial class NoteView : ContentPage, IQueryAttributable
{
    private NoteViewModel _viewModel;

    public NoteView(NoteViewModel vm)
    {
        InitializeComponent();

        _viewModel = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var noteModel = query["NoteModel"] as NoteModel;
        _viewModel.SetNoteModel( noteModel );
        _viewModel.Title = noteModel.Title;

        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        _viewModel.SaveNoteData();

        base.OnDisappearing();
    }

    /* COLLECTIONVIEW_CHILDADDED() EVENT HANDLER
    // This method has been left, just in case there is ever a need to come
    // back to this for review. Currently, calling Focus() doesn't actually
    // set the focus to the newly added 'NoteEditor' control. This is a
    // desirable effect, keeping the user from needing to switch focus from
    // their previously active note editor to the new one. This would allow
    // for a much smoother experience and would allow the app to get out of
    // the user's way. But, for now, this feature looks dead-in-the-water.
    // 
    // See the following for the Focus() method:
    //      https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.visualelement.focus?view=net-maui-7.0#microsoft-maui-controls-visualelement-focus
    private void CollectionView_ChildAdded(object sender, ElementEventArgs e)
    {
        var newChild = e.Element as Grid;

        if (newChild != null)
        {
            var newEditor = newChild.FindByName("NoteEditor") as Editor;

            if (newEditor != null)
            {
                var result = newEditor.Focus();
            }
        }
    }
    // */
}