## SolutionCop

A tool written in F# which generates FxCop project files for Visual Studio solutions (optionally based on an existing FxCop project).

### Basic syntax

solutioncop.exe -sln:"C:\MySolution\MySolution.sln" -configuration:Debug -platform:AnyCPU

### Including and excluding targets

Targets can optionally be included or excluded based on regular expressions using the -include and -exclude switches, e.g.

solutioncop.exe ... -include:"\.MyCompany\."

Multiple include/exclude switches can be used. Any target matching one or more include will be selected, while those that match one or more exclude will be ignored.

### Naming

The resulting project can be named using the -name switch, e.g.

solutioncop.exe ... -name:Analysis.fxcop

### Templates

An existing FxCop project can be used as a template by using the -based-on switch, eg..

solutioncop.exe ... -based-on:"C:\Existing.fxcop"

If no existing FxCop is provided then a default template is used which is a slightly less stringent version of the default FxCop ruleset.

