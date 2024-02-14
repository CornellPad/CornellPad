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

namespace CornellPad;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(CreateTopicView), typeof(CreateTopicView));
        Routing.RegisterRoute(nameof(TopicView), typeof(TopicView));
        Routing.RegisterRoute(nameof(NoteView), typeof(NoteView));
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        /* Provided by BING AI, but wasn't the needed solution
        var shellItem = Shell.Current?.CurrentItem;
        string title = shellItem?.Title;

        int iterationCount = 0;
        while(shellItem != null && title == null)
        {
            title = shellItem.Title;
            shellItem = shellItem.CurrentItem;
            if (iterationCount > 10)
                break; // max nesting reached

            iterationCount++;
        }

        ShellTitleLabel.Text = title;
        // */

        ShellTitleLabel.Text = Shell.Current?.CurrentPage.Title;
    }
}