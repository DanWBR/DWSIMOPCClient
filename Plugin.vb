Imports System.IO
Imports System.Reflection
Imports Ciloci.Flee
Imports DWSIM.Interfaces

<System.Serializable()> Public Class Plugin

    Implements DWSIM.Interfaces.IUtilityPlugin2

    'this variable will reference the active flowsheet in DWSIM, set before plugin's window is opened.
    Public fsheet As IFlowsheet

    Sub New()

        If Not Initialized Then
            ' sets the assembly resolver to find remaining DWSIM libraries on demand
            Dim currentDomain As AppDomain = AppDomain.CurrentDomain
            AddHandler currentDomain.AssemblyResolve, AddressOf LoadFromNestedFolder
            Initialized = True
        End If

    End Sub

    Private Shared Function LoadFromNestedFolder(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly

        Dim assemblyPath As String = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location), "opclibraries", New AssemblyName(args.Name).Name + ".dll")
        If Not File.Exists(assemblyPath) Then
            Return Nothing
        Else
            Dim assembly As Assembly = Assembly.LoadFrom(assemblyPath)
            Return assembly
        End If

    End Function

    Public ReadOnly Property Author() As String Implements DWSIM.Interfaces.IUtilityPlugin.Author
        Get
            Return "Daniel Wagner O. de Medeiros"
        End Get
    End Property

    Public ReadOnly Property ContactInfo() As String Implements DWSIM.Interfaces.IUtilityPlugin.ContactInfo
        Get
            Return "danielwag@gmail.com"
        End Get
    End Property

    Public ReadOnly Property CurrentFlowsheet() As DWSIM.Interfaces.IFlowsheet Implements DWSIM.Interfaces.IUtilityPlugin.CurrentFlowsheet
        Get
            Return fsheet
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements DWSIM.Interfaces.IUtilityPlugin.Description
        Get
            Return "DWSIM OPC Plugin"
        End Get
    End Property

    Public ReadOnly Property DisplayMode() As DWSIM.Interfaces.IUtilityPlugin.DispMode Implements DWSIM.Interfaces.IUtilityPlugin.DisplayMode
        Get
            Return DWSIM.Interfaces.IUtilityPlugin.DispMode.Dockable
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements DWSIM.Interfaces.IUtilityPlugin.Name
        Get
            Return "OPC Plugin"
        End Get
    End Property

    Public Function SetFlowsheet(form As DWSIM.Interfaces.IFlowsheet) As Boolean Implements DWSIM.Interfaces.IUtilityPlugin.SetFlowsheet
        fsheet = form
        Return True
    End Function

    Public ReadOnly Property UniqueID() As String Implements DWSIM.Interfaces.IUtilityPlugin.UniqueID
        Get
            Return "175F2519-9B3E-462F-95A3-B9613F35D078"
        End Get
    End Property

    'this is called by DWSIM to open the form, so we need to pass the reference to the flowsheet to the form BEFORE returning it.
    Public ReadOnly Property UtilityForm() As Object Implements DWSIM.Interfaces.IUtilityPlugin.UtilityForm
        Get
            Dim f As New Form1
            f.fsheet = Me.fsheet
            Return f
        End Get
    End Property

    Public ReadOnly Property WebSite() As String Implements DWSIM.Interfaces.IUtilityPlugin.WebSite
        Get
            Return "http://dwsim.inforside.com.br"
        End Get
    End Property

    Public Function Run(args As Object) As Object Implements DWSIM.Interfaces.IUtilityPlugin2.Run


    End Function

End Class
