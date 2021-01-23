languages = [ ("cs", "cs")
            , ("de", "de")
            , ("es", "es")
            , ("fr", "fr")
            , ("it", "it")
            , ("ja", "ja")
            , ("ko", "ko")
            , ("pl", "pl")
            , ("pt-BR", "ptBR")
            , ("ru", "ru")
            , ("tr", "tr")
            , ("zh-Hans", "zhHans")
            , ("zh-Hant", "zhHant")
            ]

template = """<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
	<Fragment>
    <ComponentGroup Id="NET5.resources.{1}" Directory="{1}ResourcesFolder">
      <Component>
        <File Source="{0}\\Microsoft.VisualBasic.Forms.resources.dll" 
                  Id="{1}.Microsoft.VisualBasic.Forms.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\PresentationCore.resources.dll"
                  Id="{1}.PresentationCore.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\PresentationFramework.resources.dll"
                  Id="{1}.PresentationFramework.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\PresentationUI.resources.dll"
                  Id="{1}.PresentationUI.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\ReachFramework.resources.dll"
                  Id="{1}.ReachFramework.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Windows.Controls.Ribbon.resources.dll"
                  Id="{1}.System.Windows.Controls.Ribbon.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Windows.Forms.Design.resources.dll" 
                  Id="{1}.System.Windows.Forms.Design.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Windows.Forms.Primitives.resources.dll"
                  Id="{1}.System.Windows.Forms.Primitives.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Windows.Forms.resources.dll"
                  Id="{1}.System.Windows.Forms.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Windows.Input.Manipulations.resources.dll"
                  Id="{1}.System.Windows.Input.Manipulations.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\System.Xaml.resources.dll"
                  Id="{1}.System.Xaml.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\UIAutomationClient.resources.dll"
                  Id="{1}.UIAutomationClient.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\UIAutomationClientSideProviders.resources.dll"
                  Id="{1}.UIAutomationClientSideProviders.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\UIAutomationProvider.resources.dll"
                  Id="{1}.UIAutomationProvider.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\UIAutomationTypes.resources.dll"
                  Id="{1}.UIAutomationTypes.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\WindowsBase.resources.dll"
                  Id="{1}.WindowsBase.resources.dll" />
      </Component>

      <Component>
        <File Source="{0}\\WindowsFormsIntegration.resources.dll"
                  Id="{1}.WindowsFormsIntegration.resources.dll" />
      </Component>
    </ComponentGroup>
	</Fragment>
</Wix>
"""

for la in languages:
    with open(f"NET5.resources.{la[0]}.wxs", 'w') as f:
        f.write(template.format(la[0], la[1]))