namespace Chess.FabUI.Android


open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Android.App


// the name of the type here needs to match the name inside the ResourceDesigner attribute
type Resources = Chess.FabUI.Android.Resource
[<assembly: Android.Runtime.ResourceDesigner("Chess.FabUI.Android.Resource", IsApplication=true)>]
// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[<assembly: AssemblyTitle("Chess.FabUI.Android")>]
[<assembly: AssemblyDescription("")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("HP Inc.")>]
[<assembly: AssemblyProduct("Chess.FabUI.Android")>]
[<assembly: AssemblyCopyright("Copyright © HP Inc. 2021")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]
[<assembly: ComVisible(false)>]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[<assembly: AssemblyVersion("1.0.0.0")>]
[<assembly: AssemblyFileVersion("1.0.0.0")>]
  
// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.

//[<assembly: AssemblyDelaySign(false)>]
//[<assembly: AssemblyKeyFile("")>]

do()