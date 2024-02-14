# DISCLAIMER
CornellPad is in no way associated with the university of the same name. It is not develped, endorsed, or supported in any way by the university or any of it's affiliates (if any).

# Introduction
CornellPad is a cross-platform note-taking application that utilizes the Cornell Note-Taking method. The app was created because there seemed to be no apps that specifically implemented this note-taking approach, and many people find this method to be very effective for them.

I've successfully used this note-taking technique several times, but found that I couldn't get the right proportions. I either left too little space for the 'Notes' section, or not enough space for the 'Ques' section. On top of that, like most people today, I can type much faster than I can write...and for much longer periods of time as well. I would quickly get distracted by the ache in my writing hand as I tried to concentrate on the video lecture that I was watching. I personally needed a better way to use this note-taking technique.

The CornellPad application has packages that will run on Android, iOS, MacOS (via MacCatalyst), and Windows. This is accomplished by utilizing the .NET MAUI framework to build the application.

CornellPad is free of cost and is open source. You will never be asked to pay for it, and you never should pay for it. It's free, and it always will be.

## Technical Details (for developers)
The following in an overview of the technical details for CornellPad, with a brief description of some of the key technologies used and why they were chosen.

### Architecture
CornellPad is built using an MVVM architecture, which .NET MAUI facilitates to a great degree 'out of the box'. But, to cut down on the repetative boilerplate, the [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/MVVM-Samples) library is used. This provides code generators that will create all of the boilerplate for the developer so that we can focus on implementing features, rather than writing repetative code.

### UI/UX
The UI is constructed with XAML mark-up files to define the interface layout and data bindings. Navigation through the application is done with the AppShell that .NET MAUI provides. This navigation was very convenient from a development stand-point, and provides some very nice mechanisms for passing data from one view to another.

To make UI/UX design easier, the CommunityToolkit.Maui library was used. This library provides several features that was used in this project. The first is the toolkit's 'Event to Command' behavior, which is incredibly useful when using a control (referred to as 'Views' in .NET MAUI parlance) that doesn't implement commands for some of it's events. Without this feature, registering a command that is contained in a ViewModel would be much harder (I don't know how that would be done, to be honest). It could be argued that this isn't really a UI/UX related feature, but since it is tying the ViewModel to the View, I am including it here. The second, much more obvious, feature from the CommunityToolkit.Mvvm library that was used is the custom Popups. This allowed all of the application's popups to be stylized in the same color scheme and layout that the rest of the app is using. I was very unhappy with the standard popup's appearance not matching the main application, and these custom popups really visually tied everything together.

There are some controls in the application that exhibit customized, underlined, focus behavior. These controls are provided by [MauiDynamicUnderlinedControls](https://github.com/Rabidgoalie/MauiDynamicUnderlineControls) library by Rabidgoalie (me), which is licensed under the MIT license, so you have the freedon to do what you like with it. This library provides several 'Views' that have customizable focus visuals in the form of an underline at the bottom of the border that wraps the 'View'.

### Data Storage
All data is (currently) stored within a database that is managed with the SQLite database management system. This is an extremely easy to use DBMS that is available on just about anything that has a CPU (this is only a slight exaggeration). SQLite is also a very light-weight DBMS, consuming a relatively small amount of RAM and compute cycles. Due to the cross-platform nature of .NET MAUI, these features were a necessity.

To ease the use of SQLite from .NET MAUI, several excellent libraries were used. The first is [SQLite-Net-Pcl](https://github.com/praeclarum/sqlite-net) by praeclarum, which delivers it's promise on "providing easy SQLite database storage for .NET, Mono, and Xamarin applications". This app use the non-Async version of the library, because issues were encountered when trying to use it (but this is something that I would like to revisit in the future).

The other library is [SQLite-Net Extensions](https://bitbucket.org/twincoders/sqlite-net-extensions/src/master/) by TwinCoders. In their own words, "SQLite-Net Extensions is a very simple ORM that provides one-to-one, one-to-many, many-to-one, many-to-many, inverse and text-blobbed relationships on top of the sqlite-net library". This library was key to easily working with the one-to-many relationships that exist in CornellPad. Every Topic has many notes that are associated with it; with SQLite-Net Extensions, these database entries were very easy to work with.
