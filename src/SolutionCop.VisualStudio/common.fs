namespace SolutionCop.VisualStudio

open System
open SolutionCop.Common

module Schema = 

    ///Prefix used for project file XML namespaces
    [<Literal>]
    let Prefix = "ms"

    ///Namespace URI for project files
    [<Literal>]
    let Uri = "http://schemas.microsoft.com/developer/msbuild/2003"

    ///Registers the project file XML schema name with the given context
    let register = 
        Xml.register Prefix Uri

    ///Prefixes the given name with the namespace prefix
    let prefix (name : string) = 
        String.Format ("{0}:{1}", Prefix, name)

    ///Performs a String.Format on the given string, passing the project namespace prefix as argument 0
    let prefixMany (str : string) = 
        String.Format (str, Prefix)