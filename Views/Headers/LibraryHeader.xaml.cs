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

namespace CornellPad.Views.Headers;

public partial class LibraryHeader : ContentView
{
    /////////////////////////////////////////////////////////////////////////////////
    // Members & Member Mutators
    /////////////////////////////////////////////////////////////////////////////////
    public static readonly BindableProperty HeaderTitleProperty = BindableProperty.Create(nameof(HeaderTitle), typeof(string), typeof(LibraryHeader), string.Empty);
    public static readonly BindableProperty HeaderDescriptionProperty = BindableProperty.Create(nameof(HeaderDescription), typeof(string), typeof(LibraryHeader), string.Empty);

    public string HeaderTitle
    {
        get => (string)GetValue(HeaderTitleProperty);
        set => SetValue(HeaderTitleProperty, value);
    }

    public string HeaderDescription
    {
        get => (String)GetValue(HeaderDescriptionProperty);
        set => SetValue(HeaderDescriptionProperty, value);
    }

    /////////////////////////////////////////////////////////////////////////////////
    // Methods
    /////////////////////////////////////////////////////////////////////////////////
    public LibraryHeader()
	{
		InitializeComponent();
	}
}