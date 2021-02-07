# Script to generate a WiX fragment with assemblies that are compile-time dependent. 
# At the moment this is limited to mscordaccore_amd64_amd64_<version_number>.dll
$search_dir = $args[0]

$mscordaccore = Get-ChildItem -Path $search_dir -Include "mscordaccore_amd64_amd64_*.dll" | Select-Object -first 1
$mscordaccore_name = [System.IO.Path]::GetFileName($mscordaccore)

$fragment_contents = @"
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
    <Fragment>
        <Component Id="mscordaccore_amd64_amd64.dll" 
                   Directory="INSTALLFOLDER">
            <File Source="{0}" />
        </Component>
    </Fragment>
</Wix>
"@ -f $mscordaccore_name

$write_dir = $args[1]
$write_path = [System.IO.Path]::Combine($write_dir, "NET5.assemblies.generated.wxs")

Set-Content -Path $write_path -Value $fragment_contents