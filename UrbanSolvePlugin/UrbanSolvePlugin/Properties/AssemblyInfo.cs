using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rhino.PlugIns;
using System.Text;
using System;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Rhino.Geometry.Collections;
using System.Security.AccessControl;
using System.Linq;
using System.ComponentModel;
using MetaheuristicsLibrary.SolversMO;


// Plug-in Description Attributes - all of these are optional
// These will show in Rhino's option dialog, in the tab Plug-ins
[assembly: PlugInDescription(DescriptionType.Address, "-")]
[assembly: PlugInDescription(DescriptionType.Country, "Switzerland")]
[assembly: PlugInDescription(DescriptionType.Email, "-")]
[assembly: PlugInDescription(DescriptionType.Phone, "-")]
[assembly: PlugInDescription(DescriptionType.Fax, "-")]
[assembly: PlugInDescription(DescriptionType.Organization, "-")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "-")]
[assembly: PlugInDescription(DescriptionType.WebSite, "-")]


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UrbanSOLve")] // Plug-In title is extracted from this
[assembly: AssemblyDescription("Developed by Dr. Emilie Nault and Mélanie Huck within" 
    + " the Interdisciplinary Laboratory of Performance-Integrated Design (LIPID)"
    + " at the Ecole polytechnique fédérale de Lausanne (EPFL)"
    + " with additional financial support from the InnoSeed program. This project was "
    + " initiated during the PhD thesis of Emilie Nault, completed in 2016, and supported "
    + " in part by the CCEM SECURE project and the EuroTech Universities Alliance.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("UrbanSOLve")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ca2c018a-51bc-4f46-8b20-46bfe9e86a81")] // This will also be the Guid of the Rhino plug-in

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en")]

