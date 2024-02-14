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

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CornellPad.Popups;

namespace CornellPad.ViewModels.Popups;

public partial class CreateTopicViewModel : BaseViewModel
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    ResourceDictionary _glyphResourceDict;
    private IPopupService _popupService;

    // prevent a repopulate of the glyph collection, if no search occurred.
    bool _isSearchingGlyphLibrary;
    
    [ObservableProperty]
    string _topicName;

    [ObservableProperty]
    string _selectedGlyphLibrary;

    [ObservableProperty]
    ObservableCollection<GlyphCollectionItem> _glyphs;

    [ObservableProperty]
    string _iconASCII;

    [ObservableProperty]
    GlyphCollectionItem _selectedGlyph;

    [ObservableProperty]
    string _searchGlyphLibrary;

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public CreateTopicViewModel(IPopupService popupService)
    {
        _popupService = popupService;

        Glyphs = new ObservableCollection<GlyphCollectionItem>();
        _isSearchingGlyphLibrary = false;
    }

    [RelayCommand]
    public async Task GlyphLibrarySelected()
    {
        string selectedFontFamily;

        SelectedGlyph = null;

        selectedFontFamily = GetSelectedGlyphLibrary();

        if (string.IsNullOrEmpty(selectedFontFamily))
        {
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "Error: Empty String";
                vm.PopupMessage = "Something has gone wrong while attempting to select the glyph family. The glyph library name returned from:\n\nGetSelectedGlyphLibrary(),\n\nin:\n\nGlyphLibrarySelected(),\n\nis null or empty. Please report this issue to the developer, with steps on how to reproduce this error.";
            });

            SelectedGlyphLibrary = null;

            Glyphs.Clear();

            return;
        }

        AddGlyphsToCollection(selectedFontFamily);
    }

    [RelayCommand]
    public void GlyphSelected(GlyphCollectionItem glyph)
    {
        /* Used to check actual Hex value of selected glyph
        int value = Convert.ToInt32(glyph.GlyphValue[0]);

        Debug.WriteLine($"{value:X} was selected.");
        Debug.WriteLine($"{value} was selected.");
        // */

        SelectedGlyph = glyph;
    }

    [RelayCommand]
    public void CancelGlyphSearch()
    {
        if (string.IsNullOrEmpty(SelectedGlyphLibrary))
            return;

        #region debug_cancelglyphsearch
#if DEBUG

        if (string.IsNullOrEmpty(SearchGlyphLibrary))
            Debug.WriteLine("Glyph Search Canceled.");
#endif
        #endregion

        if (string.IsNullOrEmpty(SearchGlyphLibrary) && _isSearchingGlyphLibrary)
        {
            AddGlyphsToCollection( GetSelectedGlyphLibrary() );

            _isSearchingGlyphLibrary = false;
        }
    }

    [RelayCommand]
    public async Task PerformGlyphSearch(string query)
    {
        #region debug_glyphsearchquery
#if DEBUG
        Debug.WriteLine(query);
#endif
        #endregion

        if (string.IsNullOrEmpty(SelectedGlyphLibrary))
        {
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "No Library Selected";
                vm.PopupMessage = "Please select a glyph library before attempting to search for a glyph.";
            });

            return;
        }

        string selectedFontFamily;

        selectedFontFamily = GetSelectedGlyphLibrary();

        if (string.IsNullOrEmpty(selectedFontFamily))
        {
            // How we end up here, I don't know (we check for this in GlyphLibrarySelected), but we don't plan bugs...so, we do the NullEmpty check.
            await _popupService.ShowPopupAsync<ErrorWarningViewModel>(onPresenting: vm =>
            {
                vm.Title = "Error: Empty String";
                vm.PopupMessage = "Something has gone wrong while attempting to select the glyph family. The glyph library name returned from:\n\nGetSelectedGlyphLibrary(),\n\nin:\n\nPerformGlyphSearch(),\n\nis null or empty. Please report this issue to the developer, with steps on how to reproduce this error.";
            });

            return;
        }

        _isSearchingGlyphLibrary = true;

        AddGlyphsToCollection(selectedFontFamily, query.ToLower());
    }

    [RelayCommand]
    public async Task CreateTopicAsync()
    {
        bool isNameValid = false;
        bool isIconValid = false;

        if (TopicName is null || TopicName == "")
        {
            await Shell.Current.DisplayAlert("Empty Topic Name", "Please enter a topic name.", "Ok");
        }
        else
            isNameValid = true;

        if (SelectedGlyph is null)
        {
            await Shell.Current.DisplayAlert("No Icon Selected", "Please select an icon.", "Ok");
        }
        else
            isIconValid = true;

        if (isNameValid && isIconValid)
        {
            var topic = new TopicModel()
            {
                TopicName = TopicName,
                GlyphFamily = SelectedGlyph.GlyphFamily,
                GlyphValue = SelectedGlyph.GlyphValue
            };

            await Shell.Current.GoToAsync($"///{nameof(LibraryView)}", false,
                new Dictionary<string, object>
                {
                    {"NewTopicModel", topic}
                });  
        }
    }

    /// <summary>
    /// Sets the font family (a.k.a glyph library), according to the value
    /// contained in the <paramref name="SelectedGlyphLibrary"/> member.
    /// </summary>
    /// <returns>The string alias of the selected font family.</returns>
    string GetSelectedGlyphLibrary()
    {
        string selectedFontFamily;

        switch (SelectedGlyphLibrary)
        {
            case "FA Brands":
                _glyphResourceDict = new Resources.Styles.FABrands();
                selectedFontFamily = "FA_Brands";
                break;
            case "FA Regular":
                _glyphResourceDict = new Resources.Styles.FARegular();
                selectedFontFamily = "FA_Regular";
                break;
            case "FA Solid":
                _glyphResourceDict = new Resources.Styles.FASolid();
                selectedFontFamily = "FA_Solid";
                break;
            default:
                selectedFontFamily = string.Empty;
                break;
        }

        return selectedFontFamily;
    }

    /// <summary>
    /// Clears the <paramref name="Glyphs"/> member and adds all glyphs that
    /// are contained in the font who's string alias is passed in through
    /// <paramref name="selectedFontFamily"/>.
    /// </summary>
    /// <param name="selectedFontFamily">The string alias for the font
    /// family that contains the desired glyphs. <b>NOTE:</b> this must
    /// be one of the alias' used when registering the app's fonts. See the
    /// <paramref name="ConfigureFonts"/> member call in MauiProgram.cs</param>
    /// <param name="query">The value to search each glyph's Key with;
    /// this is optional and can be omitted.</param>
    void AddGlyphsToCollection(string selectedFontFamily, string query = null)
    {
        Glyphs.Clear();

        foreach (var item in _glyphResourceDict)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (!item.Key.ToLower().Contains(query))
                    continue;
            }

            Glyphs.Add(new GlyphCollectionItem
            {
                GlyphFamily = selectedFontFamily,
                GlyphValue = item.Value.ToString()
            });
        }
    }
}
