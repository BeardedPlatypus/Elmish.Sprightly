namespace Sprightly.Presentation.Components.Dialogs

open Sprightly

/// <summary>
/// <see cref="FileDialogConfiguration"/> defines the optional configuration
/// options of a file dialog.
/// </summary>
type public FileDialogConfiguration (?addExtension: bool,
                                     ?checkIfFileExists: bool,
                                     ?dereferenceLinks: bool,
                                     ?filter: string,
                                     ?filterIndex: int,
                                     ?initialDirectory: Common.Path.T,
                                     ?multiSelect: bool,
                                     ?readOnlyChecked: bool,
                                     ?restoreDirectory: bool,
                                     ?showReadOnly: bool,
                                     ?title: string,
                                     ?validateNames: bool) =
    /// <summary>
    /// Value indicating whether the dialog box automatically adds an 
    /// extension to a file name if the user omits the extension.
    /// </summary>
    member val AddExtension: bool option = addExtension

    /// <summary>
    /// Value indicating whether the dialog box displays a warning if the user 
    /// specifies a file name that does not exist.
    /// </summary>
    member val CheckIfFileExists: bool option = checkIfFileExists

    /// <summary>
    /// Value indicating whether the dialog box returns the location of the 
    /// file referenced by the shortcut or whether it returns the location of 
    /// the shortcut (.lnk).
    /// </summary>
    member val DereferenceLinks: bool option = dereferenceLinks

    /// <summary>
    /// The current file name filter string, which determines the choices that 
    /// appear in the "Save as file type" or "Files of type" box in the dialog box.
    /// </summary>
    member val Filter: string option = filter

    /// <summary>
    ///  The index of the filter currently selected in the file dialog box.
    /// </summary>
    member val FilterIndex: int option = filterIndex

    /// <summary>
    /// The initial directory displayed by the file dialog box.
    /// </summary>
    member val InitialDirectory: Common.Path.T option = initialDirectory

    /// <summary>
    /// Value indicating whether the dialog box allows multiple files to be selected.
    /// </summary>
    member val MultiSelect: bool option = multiSelect

    /// <summary>
    /// Value indicating whether the read-only check box is selected.
    /// </summary>
    member val ReadOnlyChecked: bool option = readOnlyChecked

    /// <summary>
    /// value indicating whether the dialog box restores the directory to the 
    /// previously selected directory before closing.
    /// </summary>
    member val RestoreDirectory: bool option = restoreDirectory

    /// <summary>
    /// Gets or sets a value indicating whether the dialog box contains a read-only check box.
    /// </summary>
    member val ShowReadOnly: bool option = showReadOnly

    /// <summary>
    /// The file dialog box title.
    /// </summary>
    member val Title: string option = title

    /// <summary>
    /// Value indicating whether the dialog box accepts only valid Win32 file names.
    /// </summary>
    member val ValidateNames: bool option = validateNames
