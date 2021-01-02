<p align='center'><img align='center' src='https://raw.githubusercontent.com/BeardedPlatypus/Sprightly/c97e699823d44fe857f54f6561129bbfe8be9695/readme/sprightly_icon.svg' width='25%'></p>

*This is a work-in-progress and will be updated further soon*

# Sprightly - Organising Sprites

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
written with Fabulous for Xamarin.Forms can be found [here](https://github.com/BeardedPlatypus/Sprightly).
Unfortunately, I ran into some problems with the dynamic update behaviour,
that made it impossible to build the application I envisioned. Because I 
still want to stick with a MVU-like architecture, and learn more about
F\#, I chose [Elmish.WPF](https://github.com/elmish/Elmish.WPF) for this 
attempt.

This application is meant as a proof of concept / prototype and not a 
full-fledged production ready application. In order to allow me to maximise 
my time learning, I opted not to write any unit tests at this time, though 
they might be added in the future.
