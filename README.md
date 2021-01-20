<p align='center'><img align='center' src='https://raw.githubusercontent.com/BeardedPlatypus/Sprightly/c97e699823d44fe857f54f6561129bbfe8be9695/readme/sprightly_icon.svg' width='25%'></p>

*This is a work-in-progress and will be updated further soon*

# Sprightly - Organising Sprites
[![Build Status](https://dev.azure.com/mwtegelaers/Sprightly/_apis/build/status/BeardedPlatypus.Elmish.Sprightly?branchName=master)](https://dev.azure.com/mwtegelaers/Sprightly/_build/latest?definitionId=28&branchName=master)

Elmish.Sprightly is a small Elmish.WPF application to define sprite
sheets, sprites, and sprite animations and export the metadata as a json file,
so it can be consumed in another application. 

# Motivation

I wanted to have a small application that let me define sprites and sprite 
animations on image files, and then export these definitions to a 
simple human-readable format. These exported definitions will then be
consumed by my SDL2 applications. In order to ensure consistency between 
this application, and my SDL2 applications, Sprightly uses SDL2 to render 
any and all textures, sprites, and sprite animations. 

This is the second incarnation of this application. The first attempt
written with Fabulous for Xamarin.Forms can be found 
[here](https://github.com/BeardedPlatypus/Sprightly). In order to explore some 
more UI frameworks with F\#, I decided to reimplement it in 
[Elmish.WPF](https://github.com/elmish/Elmish.WPF) as well. The non-View logic 
should be rather similar.

This application is meant as a proof of concept / prototype and not a 
full-fledged production ready application. In order to allow me to maximise 
my time learning, I opted not to write any unit tests at this time, though 
they might be added in the future.

# Gallery

<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/Elmish.Sprightly/blob/master/.readme/starting_view.png?raw=true' width='75%'></p>
<p align='center'>
Start page with a recent projects list and the option to open an existing 
project or create a new one.  
</p>

<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/Elmish.Sprightly/blob/master/.readme/new_project_view.png?raw=true' width='75%'></p>
<p align='center'>
    New project page to set up an initial sprightly project.
</p>

<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/Elmish.Sprightly/blob/master/.readme/project_view.png?raw=true' width='75%'></p>
<p align='center'>
    Project page to manage textures.
</p>

# Dependencies

* [Elmish.WPF](https://github.com/elmish/Elmish.WPF): Elmish.WPF is a production-ready library that allows you to write WPF apps with the robust, simple, well-known, and battle-tested MVU architecture.
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro): A toolkit for creating modern WPF applications.
* [Material Design in XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit): Comprehensive and easy to use Material Design theme and control library for the Windows desktop.
* [ImageSharp](https://github.com/SixLabors/ImageSharp): ImageSharp is a new, fully featured, fully managed, cross-platform, 2D graphics API.
* [SDL2](https://www.libsdl.org/): Simple DirectMedia Layer is a cross-platform development library designed to provide low level access to audio, keyboard, mouse, joystick, and graphics hardware via OpenGL and Direct3D.