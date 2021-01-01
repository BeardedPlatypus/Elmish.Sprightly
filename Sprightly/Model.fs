namespace Sprightly

module Model =
    type public PageModel = 
        | StartingPage
        | NewProjectPage

    type public Model = 
        { PageModel : PageModel
          NewProjectPageModel : Presentation.Pages.NewProjectPage.Model option
        }
