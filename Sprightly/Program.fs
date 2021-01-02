﻿namespace Sprightly

open Elmish
open Elmish.WPF

open Sprightly
open Sprightly.Model


module public App =

    type public PageMsg =
        | NewProjectPage of Presentation.Pages.NewProjectPage.Msg

    /// This is a discriminated union of the available messages from the user interface
    type public Msg =
        | MoveToPage of PageModel
        | PageMsg of PageMsg

    type public CmdMsg = unit

    /// This is used to define the initial state of our application
    let public init () = 
        { PageModel = StartingPage
          NewProjectPageModel = None
        }, []

    let updatePage (msg: PageMsg) (model: Model) : Model * CmdMsg list =
        match msg, model.PageModel with 
        | (PageMsg.NewProjectPage pageMsg, PageModel.NewProjectPage) when model.NewProjectPageModel.IsSome ->
            let newPageModel, _ = Presentation.Pages.NewProjectPage.update pageMsg model.NewProjectPageModel.Value

            { model with NewProjectPageModel = Some newPageModel }, []
        | _ ->
            model, []

    /// This is the Reducer Elmish.WPF calls to generate a new model based on a message and an old model
    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | MoveToPage pageModel -> 
            match pageModel with
            | PageModel.NewProjectPage -> 
                { model with PageModel = pageModel 
                             NewProjectPageModel = Some ( model.NewProjectPageModel |> Option.defaultValue (Presentation.Pages.NewProjectPage.init ())) }, []
            | _ ->
                { model with PageModel = pageModel }, []
        | PageMsg pageMsg ->
            updatePage pageMsg model

    let public toCmd (cmdMsg: CmdMsg) = 
        Cmd.none

    let private toCommonPage (pageModel: PageModel) : Sprightly.Presentation.Common.PageType =
        match pageModel with 
        | Model.StartingPage   -> Sprightly.Presentation.Common.PageType.StartingPage
        | Model.NewProjectPage -> Sprightly.Presentation.Common.PageType.NewProjectPage

    /// Elmish uses this to provide the data context for your view based on a model
    let bindings () : Binding<Model, Msg> list = 
        [ // Bindings
          "PageType" |> Binding.oneWay (fun (m: Model) -> m.PageModel |> toCommonPage)

          // SubViewModels
          "NewProjectPageModel" |> 
            Binding.subModelOpt(
                (fun (m: Model) -> m.NewProjectPageModel ), 
                (fun (_, m) -> m),
                Msg.PageMsg << PageMsg.NewProjectPage, 
                Presentation.Pages.NewProjectPage.bindings)

          // Dispatch commands
          "RequestNewProjectPageCommand" |> Binding.cmd (MoveToPage (PageModel.NewProjectPage))
          "RequestStartPageCommand" |> Binding.cmd (MoveToPage PageModel.StartingPage)
        ]

            
            
