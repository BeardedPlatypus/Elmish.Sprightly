namespace Sprightly.Presentation.Components.Dialogs

open System.Threading
open System.Windows

open Elmish

open Sprightly

module public FileDialog =
    let private configureProperty (optionalValue: 'T option) (propAssign: 'T -> unit) : unit =
        if optionalValue |> Option.isSome then propAssign optionalValue.Value

    let private configureDialogWith (config: FileDialogConfiguration)
                                    (dialog: Microsoft.Win32.FileDialog) : unit =
        configureProperty config.AddExtension 
                          (fun b -> dialog.AddExtension <- b)
        configureProperty config.CheckIfFileExists 
                          (fun b -> dialog.CheckFileExists <- b)
        configureProperty config.DereferenceLinks 
                          (fun b -> dialog.DereferenceLinks <- b)
        configureProperty config.Filter
                          (fun f -> dialog.Filter <- f)
        configureProperty config.FilterIndex
                          (fun fi -> dialog.FilterIndex <- fi)
        configureProperty config.InitialDirectory
                          (fun p -> dialog.InitialDirectory <- p |> Common.Path.toString)
        configureProperty config.RestoreDirectory
                          (fun b -> dialog.RestoreDirectory <- b)
        configureProperty config.Title
                          (fun t -> dialog.Title <- t)
        configureProperty config.ValidateNames
                          (fun b -> dialog.ValidateNames <- b)

    let private configureSaveDialogWith (config: FileDialogConfiguration) 
                                        (dialog: Microsoft.Win32.SaveFileDialog) : Microsoft.Win32.SaveFileDialog =
        dialog |> (configureDialogWith config)
        dialog

    let private configureOpenDialogWith (config: FileDialogConfiguration)
                                        (dialog: Microsoft.Win32.OpenFileDialog) : Microsoft.Win32.OpenFileDialog =
        dialog |> (configureDialogWith config)
        configureProperty config.MultiSelect
                          (fun p -> dialog.Multiselect <- p)
        configureProperty config.ReadOnlyChecked
                          (fun p -> dialog.ReadOnlyChecked <- p)
        configureProperty config.ReadOnlyChecked
                          (fun p -> dialog.ShowReadOnly <- p)

        dialog

    type public DialogType =
        | Open
        | Save

    let private getDialog (dialogType: DialogType) (config: FileDialogConfiguration) : Microsoft.Win32.FileDialog =
        match dialogType with 
        | Save -> (Microsoft.Win32.SaveFileDialog () |> (configureSaveDialogWith config)) :> Microsoft.Win32.FileDialog
        | Open -> (Microsoft.Win32.OpenFileDialog () |> (configureOpenDialogWith config)) :> Microsoft.Win32.FileDialog

    let private showDialogAsync (toSuccessMsg: Common.Path.T -> 'Msg)
                                (toCancelMsg: unit -> 'Msg)
                                (guiContext: SynchronizationContext) 
                                (dialogType : DialogType)
                                (config: FileDialogConfiguration) =
        async {
            do! Async.SwitchToContext guiContext

            let dlg = getDialog dialogType config
            let isOk = dlg.ShowDialog ()
            
            if isOk.HasValue && isOk.Value then 
                return dlg.FileName |> Common.Path.fromString |> toSuccessMsg
            else 
                return toCancelMsg ()
        }

    let public showDialogCmd (toSuccessMsg: Common.Path.T -> 'Msg)
                             (toCancelMsg: unit -> 'Msg)
                             (toErrMsg: exn -> 'Msg)
                             (dialogType: DialogType)
                             (config: FileDialogConfiguration) =
        let asyncCmd () =
            Application.Current.Dispatcher.Invoke(
                fun () -> showDialogAsync toSuccessMsg 
                                          toCancelMsg
                                          SynchronizationContext.Current 
                                          dialogType 
                                          config)
        Cmd.OfAsync.either asyncCmd () id toErrMsg
