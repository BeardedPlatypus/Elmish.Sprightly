namespace Sprightly.Application

/// <summary>
/// The <see cref="Application.App"/> module contains all business logic
/// related to the application, e.g. the initialisation, shutdown etc.
/// </summary>
module public App =
    /// <summary>
    /// The description of the InitialiseAppData function.
    /// </summary>
    type public InitialiseAppdataFunc = unit -> unit

    /// <summary>
    /// Initialise the Sprightly application.
    /// </summary>
    /// <param name="fInitialiseAppData">The function to initialise the app data.</param>
    let initialise (fInitialiseAppData: InitialiseAppdataFunc)
                   () : unit =
        fInitialiseAppData ()

