if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
 if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) {
  $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
  Start-Process -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
  Exit
 }
}
try {
	# Set env var
	[Environment]::SetEnvironmentVariable("Path",[Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::Machine) + ";C:\Program Files\godot\",[EnvironmentVariableTarget]::Machine)

	# Bootstrap
	echo 'C:\Program Files\godot\Godot_v4.1-stable_mono_win64_console.exe' | out-file 'C:\Program Files\godot\godot.bat'

	echo "Successfully fixed your shitty PC"
	Start-Sleep -Seconds 5
}
catch {
	Write-Host "An error occurred:"
	Write-Host $_.ScriptStackTrace
	Start-Sleep -Seconds 5
} 
