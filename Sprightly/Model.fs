namespace Sprightly

module Model =
    type public PageModel = 
        | StartingPage
        | NewProjectPage
        | ProjectPage

    type public Model = 
        { PageModel : PageModel
          StartingPageModel : Presentation.Pages.StartingPage.Model
          NewProjectPageModel : Presentation.Pages.NewProjectPage.Model option
        }
