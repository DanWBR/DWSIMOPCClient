﻿Imports System.Windows.Forms
Imports DWSIM
Imports Ciloci.Flee
Imports System.Globalization
Imports DWSIM.FlowsheetSolver
Imports DWSIM.Interfaces
Imports Opc.Ua.Client
Imports Opc.Ua.Configuration
Imports Opc.Ua
Imports Opc.Ua.Client.Controls
Imports Opc.Ua.Sample.Controls
Imports System.Reflection
Imports Org.BouncyCastle.Crypto.Engines
Imports System.ComponentModel
Imports Org.BouncyCastle.Security.Certificates
Imports System.Drawing
Imports DWSIM.ExtensionMethods
Imports DWSIM.SharedClassesCSharp.FilePicker
Imports System.IO
Imports DWSIM.GlobalSettings

Public Class Form1

    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    'flowsheet reference
    Public fsheet As IFlowsheet

    Private WithEvents UpdateTimer As Timer

    Public LinkList As Dictionary(Of String, OPCLink)

    Public cbc2, cbc3, cbc4, cbc5 As DataGridViewComboBoxCell

    Private m_session As Session
    Private m_reconnectHandler As SessionReconnectHandler
    Private m_reconnectPeriod As Integer
    Private m_application As ApplicationInstance
    Private m_server As Opc.Ua.Server.StandardServer
    Private m_endpoints As ConfiguredEndpointCollection
    Private m_configuration As ApplicationConfiguration
    Private m_context As ServiceMessageContext

    Private adding As Boolean = False

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        'remove SelectedObjectChanged event handlers

        Dim myeventhandler As CustomEvent = AddressOf readhandler
        Dim myeventhandler2 As CustomEvent = AddressOf writehandler

        RemoveHandler FlowsheetSolver.FlowsheetCalculationStarted, myeventhandler
        RemoveHandler FlowsheetSolver.FlowsheetCalculationFinished, myeventhandler2

        My.Settings.Save()

    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Settings.DpiScale > 1.0 Then
            Me.ToolStrip1.AutoSize = False
            Me.ToolStrip1.Size = New Size(ToolStrip1.Width, 28 * Settings.DpiScale)
            Me.ToolStrip1.ImageScalingSize = New Size(20 * Settings.DpiScale, 20 * Settings.DpiScale)
            For Each item In Me.ToolStrip1.Items
                If TryCast(item, ToolStripButton) IsNot Nothing Then
                    DirectCast(item, ToolStripButton).Size = New Size(ToolStrip1.ImageScalingSize.Width, ToolStrip1.ImageScalingSize.Height)
                End If
            Next
            Me.ToolStrip1.AutoSize = True
            Me.ToolStrip1.Invalidate()
        End If

        Dim myeventhandler As CustomEvent = AddressOf readhandler
        Dim myeventhandler2 As CustomEvent = AddressOf writehandler

        AddHandler FlowsheetSolver.FlowsheetCalculationStarted, myeventhandler
        AddHandler FlowsheetSolver.FlowsheetCalculationFinished, myeventhandler2

        LinkList = New Dictionary(Of String, OPCLink)

        cbc2 = New DataGridViewComboBoxCell
        cbc2.Sorted = False
        cbc2.MaxDropDownItems = 10
        cbc2.DropDownWidth = 200
        cbc2.Items.Add("Spreadsheet")
        For Each obj In fsheet.SimulationObjects.Values.OrderBy(Function(o) o.GraphicObject.Tag)
            cbc2.Items.Add(obj.GraphicObject.Tag)
        Next

        cbc3 = New DataGridViewComboBoxCell
        cbc3.MaxDropDownItems = 10
        cbc3.DropDownWidth = 200

        cbc5 = New DataGridViewComboBoxCell()
        cbc5.MaxDropDownItems = 2

        cbc5.Items.AddRange(New String() {"Read", "Write"})

        Me.Grid1.Columns("associatedobject").CellTemplate = cbc2
        Me.Grid1.Columns("associatedproperty").CellTemplate = cbc3
        Me.Grid1.Columns("linktype").CellTemplate = cbc5

        Try
            Dim opcfile As String = IO.Path.GetDirectoryName(fsheet.Options.FilePath) & "\" & IO.Path.GetFileNameWithoutExtension(fsheet.Options.FilePath) & ".dwopc"
            If IO.File.Exists(opcfile) Then
                Dim jsonstring = IO.File.ReadAllText(opcfile)
                LinkList = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, OPCLink))(jsonstring)
                ReadData()
            End If
        Catch ex As Exception
        End Try

        Grid1.ChangeEditModeToOnPropertyChanged()

        Dim app As New ApplicationInstance()
        app.ApplicationName = "DWSIMOPCClient"
        app.ApplicationType = ApplicationType.Client
        app.ConfigSectionName = "DWSIMOPCClient"

        m_application = app

        Dim cpath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        cpath = System.IO.Path.Combine(cpath, "DWSIMOPCClient.Config.xml")

        app.LoadApplicationConfiguration(cpath, False).Wait()
        ' check the application certificate.
        Dim certOK As Boolean = app.CheckApplicationInstanceCertificate(False, 0).Result
        If Not certOK Then
            Throw New Exception("Application instance certificate invalid!")
        End If

        m_configuration = app.ApplicationConfiguration

        SessionsCTRL.Configuration = m_configuration
        'SessionsCTRL.MessageContext = context

        ' get list of cached endpoints.
        m_endpoints = m_configuration.LoadCachedEndpoints(True)
        m_endpoints.DiscoveryUrls = m_configuration.ClientConfiguration.WellKnownDiscoveryUrls
        EndpointSelectorCTRL.Initialize(m_endpoints, m_configuration)

        If Not m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates Then
            AddHandler m_configuration.CertificateValidator.CertificateValidation, AddressOf CertificateValidator_CertificateValidation
        End If

        DWSIM.ExtensionMethods.ChangeDefaultFont(Me)

        'initialize control state.
        Disconnect()

        UpdateTimer = New Timer With {.Enabled = True, .Interval = 60000}
        UpdateTimer.Start()

    End Sub

    Private Sub CertificateValidator_CertificateValidation(ByVal validator As CertificateValidator, ByVal e As CertificateValidationEventArgs)

        If InvokeRequired Then
            Invoke(Sub(v0, e0) CertificateValidator_CertificateValidation(v0, e0), New Object() {validator, e})
            Return
        End If

        Try
            GuiUtils.HandleCertificateValidationError(Me, validator, e)
        Catch exception As Exception
            GuiUtils.HandleException(Me.Text, MethodBase.GetCurrentMethod, exception)
        End Try

    End Sub

    Public Sub Disconnect()

        If m_session IsNot Nothing Then
            If m_reconnectHandler IsNot Nothing Then
                m_reconnectHandler.Dispose()
                m_reconnectHandler = Nothing
            End If
            m_session.Close()
            m_session = Nothing
        End If
        ServerUrlLB.Text = ""
    End Sub

    Private Sub EndpointSelectorCTRL_ConnectEndpoint(ByVal sender As Object, ByVal e As ConnectEndpointEventArgs) Handles EndpointSelectorCTRL.ConnectEndpoint
        Try
            Connect(e.Endpoint)
        Catch exception As Exception
            e.UpdateControl = False
            Throw exception
        End Try

    End Sub

    Private Sub EndpointSelectorCTRL_OnChange(ByVal sender As Object, ByVal e As EventArgs) Handles EndpointSelectorCTRL.EndpointsChanged

        m_endpoints.Save()

    End Sub

    ''' <summary>
    ''' Connects to a server.
    ''' </summary>
    Public Async Sub Connect(ByVal endpoint As ConfiguredEndpoint)
        If (endpoint Is Nothing) Then
            Return
        End If

        Dim session As Session = Await SessionsCTRL.Connect(endpoint)
        If (Not (session) Is Nothing) Then
            ' stop any reconnect operation.
            If (Not (m_reconnectHandler) Is Nothing) Then
                m_reconnectHandler.Dispose()
                m_reconnectHandler = Nothing
            End If

            m_session = session
            AddHandler m_session.KeepAlive, AddressOf StandardClient_KeepAlive
            StandardClient_KeepAlive(m_session, Nothing)
        End If

    End Sub

    Private Sub StandardClient_KeepAlive(ByVal sender As Session, ByVal e As KeepAliveEventArgs)
        If InvokeRequired Then
            BeginInvoke(Sub(s2, e2) StandardClient_KeepAlive(s2, e2), New Object() {sender, e})
            Return
        ElseIf Not IsHandleCreated Then
            Return
        End If

        If ((Not (sender) Is Nothing) _
            AndAlso (Not (sender.Endpoint) Is Nothing)) Then
            ServerUrlLB.Text = Utils.Format("{0} ({1}) {2}", sender.Endpoint.EndpointUrl, sender.Endpoint.SecurityMode, IIf(sender.EndpointConfiguration.UseBinaryEncoding, "UABinary", "XML"))
        Else
            ServerUrlLB.Text = "None"
        End If

        If ((Not (e) Is Nothing) _
            AndAlso (Not (m_session) Is Nothing)) Then
            If ServiceResult.IsGood(e.Status) Then
                ServerStatusLB.Text = Utils.Format("Server Status: {0} {1:yyyy-MM-dd HH:mm:ss} {2}/{3}", e.CurrentState, e.CurrentTime.ToLocalTime, m_session.OutstandingRequestCount, m_session.DefunctRequestCount)
                ServerStatusLB.ForeColor = Color.Empty
                ServerStatusLB.Font = New Font(ServerStatusLB.Font, FontStyle.Regular)
            Else
                ServerStatusLB.Text = String.Format("{0} {1}/{2}", e.Status, m_session.OutstandingRequestCount, m_session.DefunctRequestCount)
                ServerStatusLB.ForeColor = Color.Red
                ServerStatusLB.Font = New Font(ServerStatusLB.Font, FontStyle.Bold)
                If (m_reconnectPeriod <= 0) Then
                    Return
                End If

                If ((m_reconnectHandler Is Nothing) _
                    AndAlso (m_reconnectPeriod > 0)) Then
                    m_reconnectHandler = New SessionReconnectHandler
                    m_reconnectHandler.BeginReconnect(m_session, (m_reconnectPeriod * 1000), Sub() StandardClient_Server_ReconnectComplete(sender, e))
                End If

            End If

        End If

    End Sub

    Private Sub StandardClient_Server_ReconnectComplete(ByVal sender As Object, ByVal e As EventArgs)
        If InvokeRequired Then
            BeginInvoke(Sub(s2, e2) Me.StandardClient_Server_ReconnectComplete(s2, e2), New Object() {sender, e})
            Return
        End If

        Try
            ' ignore callbacks from discarded objects.
            If Not Object.ReferenceEquals(sender, m_reconnectHandler) Then
                Return
            End If

            m_session = m_reconnectHandler.Session
            m_reconnectHandler.Dispose()
            m_reconnectHandler = Nothing
            SessionsCTRL.Reload(m_session)
            StandardClient_KeepAlive(m_session, Nothing)
        Catch exception As Exception
            GuiUtils.HandleException(Me.Text, MethodBase.GetCurrentMethod, exception)
        End Try

    End Sub

    Private Sub ToolStripButton3_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton3.Click

        adding = True

        Dim id As String = Guid.NewGuid.ToString

        Dim newitem As New OPCLink() With {.ID = id, .Name = "OPCLink" & LinkList.Count, .Comment = "ItemDescription"}

        LinkList.Add(id, newitem)

        Me.Grid1.Rows.Add()
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("id").Value = id
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("itemname").Value = newitem.Name
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("comment").Value = newitem.Comment
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("active").Value = True
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("itemname").Value = newitem.Name
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("expression").Value = newitem.Expression
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("minimumvalue").Value = newitem.MinimumValue
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("maximumvalue").Value = newitem.MaximumValue
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("currentvalue").Value = newitem.MaximumValue
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("failsafevalue").Value = newitem.FailSafeValue

        If newitem.LinkType = OPCLink.LType.Read Then
            Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("linktype").Value = "Read"
        Else
            Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("linktype").Value = "Write"
        End If

        adding = False

    End Sub

    Private Sub ToolStripButton4_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton4.Click

        If Me.Grid1.SelectedRows.Count > 0 Then
            For i As Integer = 0 To Me.Grid1.SelectedRows.Count - 1
                LinkList.Remove(Me.Grid1.SelectedRows(0).Cells("id").Value)
                Me.Grid1.Rows.Remove(Me.Grid1.SelectedRows(0))
            Next
        End If

    End Sub

    Private Sub Grid1_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Grid1.CellValueChanged

        If e.RowIndex >= 0 Then
            Dim id As String = Grid1.Rows(e.RowIndex).Cells("id").Value
            If id Is Nothing Then Exit Sub
            If Not LinkList.ContainsKey(id) Then Exit Sub
            Dim item = LinkList(id)
            Select Case e.ColumnIndex
                Case 4
                    Dim cbc As DataGridViewComboBoxCell = Me.Grid1.Rows(e.RowIndex).Cells("associatedproperty")
                    cbc.Items.Clear()
                    With cbc.Items
                        If Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString <> "" Then
                            Dim props As String()
                            If Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString = "Spreadsheet" Then
                                props = fsheet.FormSpreadsheet.GetCellString()
                                item.ObjectID = "Spreadsheet"
                            Else
                                Dim objtag = Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value.ToString
                                item.ObjectID = fsheet.SimulationObjects.Values.Where(Function(x) x.GraphicObject.Tag = objtag).FirstOrDefault.Name
                                props = Me.ReturnProperties(objtag, False)
                            End If
                            For Each prop As String In props
                                .Add(fsheet.GetTranslatedString(prop))
                            Next
                        End If
                    End With
                Case 5
                    If Not Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                        If Not Me.Grid1.Rows(e.RowIndex).Cells(4).Value.ToString = "Spreadsheet" Then
                            Dim props As String() = Me.ReturnProperties(Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value, False)
                            For Each prop As String In props
                                If fsheet.GetTranslatedString(prop) = Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString Then
                                    item.PropertyID = prop
                                    Dim obj As DWSIM.Interfaces.ISimulationObject = ReturnObject(Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value)
                                    Me.Grid1.Rows(e.RowIndex).Cells("unit") = GetUnits(obj.GetPropertyUnit(prop))
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Case 8
                    If Not Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                        If Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = "Read" Then
                            item.LinkType = OPCLink.LType.Read
                        Else
                            item.LinkType = OPCLink.LType.Write
                        End If
                    End If
                Case 11
                    Try
                        item.MinimumValue = Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    Catch ex As Exception
                    End Try
                Case 12
                    Try
                        item.MaximumValue = Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    Catch ex As Exception
                    End Try
                Case 13
                    Try
                        item.FailSafeValue = Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    Catch ex As Exception
                    End Try
                Case 14
                    Try
                        Dim expr As String = Grid1.Rows(e.RowIndex).Cells("expression").Value
                        Dim econtext As New ExpressionContext
                        econtext.Imports.AddType(GetType(System.Math))
                        econtext.Variables.Add("val", item.CurrentValue)
                        Dim exbase As IGenericExpression(Of Double) = econtext.CompileGeneric(Of Double)(expr)
                        Dim result As Object = exbase.Evaluate
                        If Double.TryParse(result, New Double) Then
                            item.ExpressionResult = result
                            Grid1.Rows(e.RowIndex).Cells("result").Value = result
                        Else
                            item.ExpressionResult = Double.NaN
                            Grid1.Rows(e.RowIndex).Cells("result").Value = Double.NaN
                        End If
                    Catch ex As Exception
                        Grid1.Rows(e.RowIndex).Cells("result").Value = "Error evaluating expression: " + ex.Message.ToString
                    End Try
                Case 16
                    item.ExpressionUnits = Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            End Select
        End If

    End Sub

    Private Function ReturnObject(ByVal objectTAG As String) As DWSIM.Interfaces.ISimulationObject

        For Each obj In fsheet.SimulationObjects.Values
            If objectTAG = obj.GraphicObject.Tag Then
                Return obj
                Exit Function
            End If
        Next

        Return Nothing

    End Function

    Private Function ReturnPropertyID(ByVal objectID As String, ByVal propTAG As String) As String

        Dim props As String() = fsheet.SimulationObjects(objectID).GetProperties(Interfaces.Enums.PropertyType.ALL)
        For Each prop As String In props
            If fsheet.GetTranslatedString(prop) = propTAG Then
                Return prop
            End If
        Next

        Return Nothing
    End Function

    Private Function ReturnPropertyName(ByVal objectID As String, ByVal propName As String) As String

        Dim props As String() = fsheet.SimulationObjects(objectID).GetProperties(Interfaces.Enums.PropertyType.ALL)
        For Each prop As String In props
            If prop = propName Then
                Return fsheet.GetTranslatedString(prop)
            End If
        Next

        Return Nothing

    End Function

    Private Function ReturnProperties(ByVal objectTAG As String, ByVal dependent As Boolean) As String()

        For Each obj In fsheet.SimulationObjects.Values
            If objectTAG = obj.GraphicObject.Tag Then
                Return obj.GetProperties(Interfaces.Enums.PropertyType.ALL)
                Exit Function
            End If
        Next

        Return Nothing

    End Function

    Function GetUnits(propertyunit As String) As DataGridViewComboBoxCell

        Dim cb As New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"K", "R", "C", "F"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"Pa", "atm", "kgf/cm2", "kgf/cm2g", "lbf/ft2", "kPa", "kPag", "bar", "barg", "ftH2O", "inH2O", "inHg", "mbar", "mH2O", "mmH2O", "mmHg", "MPa", "psi", "psig"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"g/s", "lbm/h", "kg/s", "kg/h", "kg/d", "kg/min", "lb/min", "lb/s"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"mol/s", "lbmol/h", "mol/h", "mol/d", "kmol/s", "kmol/h", "kmol/d", "m3/d @ BR", "m3/d @ NC", "m3/d @ CNTP", "m3/d @ SC"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"m3/s", "ft3/s", "cm3/s", "m3/h", "m3/d", "bbl/h", "bbl/d", "ft3/min", "gal[UK]/h", "gal[UK]/s", "gal[US]/h", "gal[US]/min", "L/h", "L/min", "L/s"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kJ/kg", "cal/g", "BTU/lbm", "kcal/kg"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kJ/[kg.K]", "cal/[g.C]", "BTU/[lbm.R]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kg/kmol", "g/mol", "lbm/lbmol"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"N/m", "dyn/cm", "lbf/in"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kg/m3", "g/cm3", "lbm/ft3"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kJ/[kg.K]", "cal/[g.C]", "BTU/[lbm.R]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"W/[m.K]", "cal/[cm.s.C]", "BTU/[ft.h.R]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"m2/s", "cSt", "ft2/s", "mm2/s"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kg/[m.s]", "Pa.s", "cP", "lbm/[ft.h]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"Pa", "atm", "lbf/ft2", "kgf/cm2", "kPa", "bar", "ftH2O", "inH2O", "inHg", "mbar", "mH2O", "mmH2O", "mmHg", "MPa", "psi"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"C.", "K.", "F.", "R."})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"m", "ft", "cm"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New Object() {"kW", "kcal/h", "BTU/h", "BTU/s", "cal/s", "HP", "kJ/h", "kJ/d", "MW", "W"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"s", "min.", "h"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m3", "cm3", "L", "ft3"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m3/kmol", "cm3/mmol", "ft3/lbmol"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m2", "cm2", "ft2"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m", "mm", "cm", "in", "ft"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"N", "dyn", "kgf", "lbf"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"W/[m2.K]", "cal/[cm2.s.C]", "BTU/[ft2.h.R]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m/s2", "cm/s2", "ft/s2"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m3/kg", "cm3/g", "ft3/lbm"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"kmol/m3", "mol/m3", "mol/L", "mol/cm3", "mol/mL", "lbmol/ft3"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"kg/m3", "g/L", "g/cm3", "g/mL", "lbm/ft3"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"kmol/[m3.s]", "kmol/[m3.min.]", "kmol/[m3.h]", "mol/[m3.s]", "mol/[m3.min.]", "mol/[m3.h]", "mol/[L.s]", "mol/[L.min.]", "mol/[L.h]", "mol/[cm3.s]", "mol/[cm3.min.]", "mol/[cm3.h]", "lbmol.[ft3.h]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"kJ/kmol", "cal/mol", "BTU/lbmol"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"kJ/[kmol.K]", "cal/[mol.C]", "BTU/[lbmol.R]"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m/s", "cm/s", "mm/s", "km/h", "ft/h", "ft/min", "ft/s", "in/s"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"K.m2/W", "C.cm2.s/cal", "ft2.h.F/BTU"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m/kg", "ft/lbm", "cm/g"})

        If cb.Items.Contains(propertyunit) Then Return cb

        cb = New DataGridViewComboBoxCell
        cb.Items.AddRange(New String() {"m-1", "cm-1", "ft-1"})

        Return cb

    End Function

    Private Sub Grid1_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles Grid1.DataError

    End Sub

    Private Sub ToolStripButton5_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton5.Click

        Me.Grid1.Columns(9).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat
        Me.Grid1.Columns(10).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat
        Me.Grid1.Columns(11).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat
        Me.Grid1.Columns(12).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat
        Me.Grid1.Columns(13).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat
        Me.Grid1.Columns(15).DefaultCellStyle.Format = fsheet.FlowsheetOptions.NumberFormat

        UpdateValues()

    End Sub

    Sub UpdateValues()

        If m_session Is Nothing Then
            fsheet.ShowMessage("OPC Plugin: please connect to a server to create a session.", IFlowsheet.MessageType.GeneralError)
            Exit Sub
        End If

        For Each r As DataGridViewRow In Grid1.Rows

            If r.Cells("active").Value = True Then

                Dim item = LinkList(r.Cells("id").Value)

                If item.MonitoredVariable IsNot Nothing Then

                    Try

                        r.Cells("servervalue").Value = ReadCurrentValue(item.MonitoredVariable.NodeId).Value

                    Catch ex As Exception

                        fsheet.ShowMessage("OPC Plugin: Error reading current value of node '" & r.Cells("monitoreditem").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                    End Try

                End If

                Dim value As Double

                If r.Cells("linktype").Value = "Write" Then

                    Dim propval As Double
                    Dim selectedprop As String = r.Cells("associatedproperty").Value

                    Try

                        If r.Cells("associatedobject").Value <> "Spreadsheet" Then

                            Dim objID As String = ReturnObject(r.Cells("associatedobject").Value).Name

                            Dim props As String() = Me.ReturnProperties(r.Cells("associatedobject").Value, False)

                            Dim propunit As String = r.Cells("unit").Value

                            For Each prop As String In props
                                If fsheet.GetTranslatedString(prop) = selectedprop Then
                                    value = Convert.ToDouble(fsheet.SimulationObjects(objID).GetPropertyValue(prop)).ConvertFromSI(propunit)
                                    Exit For
                                End If
                            Next

                            r.Cells("currentvalue").Value = value

                            Try

                                If value < item.MinimumValue Then value = item.MinimumValue
                                If value > item.MaximumValue Then value = item.MaximumValue

                                item.CurrentValue = value

                                Dim expr As String = r.Cells("expression").Value

                                Dim econtext As New ExpressionContext
                                econtext.Imports.AddType(GetType(System.Math))
                                econtext.Variables.Add("val", value)
                                Dim exbase As IGenericExpression(Of Double) = econtext.CompileGeneric(Of Double)(expr)

                                Dim result As Object = exbase.Evaluate

                                If Me.InvokeRequired Then
                                    Me.Invoke(Sub()
                                                  If Double.TryParse(result, New Double) Then
                                                      item.ExpressionResult = result
                                                      r.Cells("result").Value = result
                                                  Else
                                                      item.ExpressionResult = Double.NaN
                                                      r.Cells("result").Value = Double.NaN
                                                  End If
                                              End Sub)
                                Else
                                    If Double.TryParse(result, New Double) Then
                                        item.ExpressionResult = result
                                        r.Cells("result").Value = result
                                    Else
                                        item.ExpressionResult = Double.NaN
                                        r.Cells("result").Value = Double.NaN
                                    End If
                                End If

                            Catch ex As Exception

                                r.Cells("result").Value = "Error evaluating expression: " + ex.Message.ToString

                            End Try

                            fsheet.UpdateOpenEditForms()

                        Else

                            propval = r.Cells("result").Value

                            fsheet.FormSpreadsheet.SetCellValue(selectedprop, propval)

                            fsheet.UpdateSpreadsheet(Nothing)

                        End If

                    Catch ex As Exception

                        fsheet.ShowMessage("Error writing '" & propval & "' to property '" & selectedprop & "' from object '" & r.Cells("associatedobject").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                    End Try

                Else

                    Try
                        value = m_session.ReadValue(item.MonitoredVariable.NodeId).Value
                        r.Cells("currentvalue").Value = value
                    Catch ex As Exception
                        fsheet.ShowMessage(String.Format("Error reading OPC Variable '{0}': {1}", item.MonitoredVariable.NodeId.ToString(), ex.Message), IFlowsheet.MessageType.GeneralError)
                        r.Cells("currentvalue").Value = Double.NaN
                        value = item.FailSafeValue
                    End Try

                    Try

                        If value < item.MinimumValue Then value = item.MinimumValue
                        If value > item.MaximumValue Then value = item.MaximumValue

                        item.CurrentValue = value

                        Dim expr As String = r.Cells("expression").Value

                        Dim econtext As New ExpressionContext
                        econtext.Imports.AddType(GetType(System.Math))
                        econtext.Variables.Add("val", value)
                        Dim exbase As IGenericExpression(Of Double) = econtext.CompileGeneric(Of Double)(expr)

                        Dim result As Object = exbase.Evaluate

                        If Double.TryParse(result, New Double) Then
                            item.ExpressionResult = result
                            r.Cells("result").Value = result
                        Else
                            item.ExpressionResult = Double.NaN
                            r.Cells("result").Value = Double.NaN
                        End If

                        r.Cells("lastupdate").Value = Date.Now.ToString

                    Catch ex As Exception

                        r.Cells("result").Value = "Error evaluating expression: " + ex.Message.ToString

                    End Try

                End If

            End If

        Next

    End Sub

    Public Sub readhandler(ByVal sender As Object, e As System.EventArgs, extraargs As Object)

        If btnAutoUpdate.Checked Then ToolStripButton5_Click(sender, e)

        For Each r As DataGridViewRow In Grid1.Rows

            If r.Cells("active").Value = True Then

                Dim propval As Double
                Dim selectedprop As String = r.Cells("associatedproperty").Value

                If r.Cells("linktype").Value = "Read" Then

                    Try

                        If r.Cells("associatedobject").Value <> "Spreadsheet" Then

                            Dim objID As String = ReturnObject(r.Cells("associatedobject").Value).Name

                            Dim props As String() = Me.ReturnProperties(r.Cells("associatedobject").Value, False)

                            Dim propunit As String = r.Cells("unit").Value

                            propval = r.Cells("result").Value

                            For Each prop As String In props
                                If fsheet.GetTranslatedString(prop) = selectedprop Then
                                    fsheet.SimulationObjects(objID).SetPropertyValue(prop, SharedClasses.SystemsOfUnits.Converter.ConvertToSI(propunit, propval))
                                    Exit For
                                End If
                            Next

                            fsheet.UpdateOpenEditForms()

                        Else

                            propval = r.Cells("result").Value

                            fsheet.FormSpreadsheet.SetCellValue(selectedprop, propval)

                            fsheet.UpdateSpreadsheet(Nothing)

                        End If

                    Catch ex As Exception

                        fsheet.ShowMessage("OPC Plugin: Error writing '" & propval & "' to property '" & selectedprop & "' from object '" & r.Cells("associatedobject").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                    End Try

                End If

            End If

        Next


    End Sub

    Public Sub writehandler(ByVal sender As Object, e As System.EventArgs, extraargs As Object)

        If btnAutoUpdate.Checked Then ToolStripButton5_Click(sender, e)

        For Each r As DataGridViewRow In Grid1.Rows

            If r.Cells("active").Value = True Then

                Dim propval As Double
                Dim selectedprop As String = r.Cells("associatedproperty").Value

                If r.Cells("linktype").Value = "Write" Then

                    Dim item = LinkList(r.Cells("id").Value)

                    Try

                        propval = r.Cells("result").Value

                        WriteValue(item.MonitoredVariable.NodeId, propval)

                        r.Cells("lastupdate").Value = Date.Now.ToString

                        fsheet.ShowMessage(String.Format("OPC Plugin: Wrote value '{0}' to node '{1}'.", propval, r.Cells("monitoreditem").Value), IFlowsheet.MessageType.Information)

                    Catch ex As Exception

                        fsheet.ShowMessage("OPC Plugin: Error writing '" & propval & "' to node '" & r.Cells("monitoreditem").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                    End Try

                    Try

                        If Me.InvokeRequired Then
                            Me.Invoke(Sub()
                                          r.Cells("servervalue").Value = ReadCurrentValue(item.MonitoredVariable.NodeId).Value
                                      End Sub)
                        Else
                            r.Cells("servervalue").Value = ReadCurrentValue(item.MonitoredVariable.NodeId).Value
                        End If

                    Catch ex As Exception

                        fsheet.ShowMessage("OPC Plugin: Error reading current value of node '" & r.Cells("monitoreditem").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                    End Try

                End If

            End If

        Next


    End Sub

    Sub StoreData()

        For Each r As DataGridViewRow In Grid1.Rows
            With LinkList(r.Cells("id").Value)
                .Active = r.Cells("active").Value
                .CurrentValue = r.Cells("currentvalue").Value
                .ExpressionUnits = r.Cells("unit").Value
                .FailSafeValue = r.Cells("failsafevalue").Value
                .LastUpdate = r.Cells("lastupdate").Value
                .MaximumValue = r.Cells("maximumvalue").Value
                .MinimumValue = r.Cells("minimumvalue").Value
                .Name = r.Cells("itemname").Value
                If r.Cells("associatedobject").Value = "Planilha" Then
                    .ObjectID = r.Cells("associatedobject").Value
                    .PropertyID = r.Cells("associatedproperty").Value
                Else
                    .ObjectID = ReturnObject(r.Cells("associatedobject").Value).Name
                    .PropertyID = ReturnPropertyID(.ObjectID, r.Cells("associatedproperty").Value)
                End If
                .Expression = r.Cells("expression").Value
                .PreviousValue = 0.0#
                .Comment = r.Cells("comment").Value
                If r.Cells("linktype").Value = "Write" Then
                    .LinkType = OPCLink.LType.Write
                Else
                    .LinkType = OPCLink.LType.Read
                End If
            End With
        Next

    End Sub

    Sub ReadData()

        Grid1.Rows.Clear()

        adding = True

        For Each p As OPCLink In LinkList.Values

            Try
                If p.MonitoredVariable IsNot Nothing Then
                    Grid1.Rows.Add(New Object() {p.ID, p.Active, p.Name, p.Comment, "", "",
                                             p.MonitoredVariable.DisplayName.ToString() + " [" + p.MonitoredVariable.NodeId.ToString() + "]",
                                             "", IIf(p.LinkType = OPCLink.LType.Read, "Read", "Write"), p.CurrentValue,
                                             p.ServerValue, p.MinimumValue, p.MaximumValue, p.FailSafeValue,
                                             p.Expression, p.ExpressionResult, "", p.LastUpdate})
                Else
                    Grid1.Rows.Add(New Object() {p.ID, p.Active, p.Name, p.Comment, "", "",
                                             "", "", IIf(p.LinkType = OPCLink.LType.Read, "Read", "Write"), p.CurrentValue,
                                             p.ServerValue, p.MinimumValue, p.MaximumValue, p.FailSafeValue,
                                             p.Expression, p.ExpressionResult, "", p.LastUpdate})
                End If
                If p.ObjectID = "Spreadsheet" Then
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedobject").Value = "Spreadsheet"
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedproperty").Value = p.PropertyID
                Else
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedobject").Value = fsheet.SimulationObjects(p.ObjectID).GraphicObject.Tag
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedproperty").Value = ReturnPropertyName(p.ObjectID, p.PropertyID)
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("unit").Value = p.ExpressionUnits
                End If
            Catch ex As Exception
                fsheet.ShowMessage("OPC Plugin: Error: " & ex.ToString, IFlowsheet.MessageType.GeneralError)
            End Try

        Next

        adding = False

    End Sub

    Private Sub ToolStripButton2_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton2.Click

        Dim filePickerForm As IFilePicker = FilePickerService.GetInstance().GetFilePicker()

        Dim handler As IVirtualFile = filePickerForm.ShowSaveDialog(
            New List(Of FilePickerAllowedType) From {New FilePickerAllowedType("OPC Variable List", "*.dwopc")})

        If handler IsNot Nothing Then
            Try
                Dim jsonstring = Newtonsoft.Json.JsonConvert.SerializeObject(LinkList, Newtonsoft.Json.Formatting.Indented)
                Using stream As New IO.MemoryStream()
                    Using writer As New StreamWriter(stream) With {.AutoFlush = True}
                        writer.Write(jsonstring)
                        handler.Write(stream)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show(fsheet.GetFlowsheet.GetTranslatedString("Erroaosalvararquivo") & " " & ex.Message,
                                fsheet.GetFlowsheet.GetTranslatedString("Erro"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Sub Grid1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Grid1.CellClick

        If e.ColumnIndex = 7 Then

            Dim selector As New ItemList
            selector.BrowseCTRL.SetView(m_session, BrowseViewType.Objects, Nothing)
            selector.ShowDialog()

            Dim ritem = selector.MonItem

            If ritem IsNot Nothing Then

                Dim item = LinkList(Grid1.Rows(e.RowIndex).Cells("id").Value)

                item.MonitoredVariable = ritem
                item.PreviousValue = item.CurrentValue
                item.CurrentValue = m_session.ReadValue(ritem.NodeId).Value
                item.LastUpdate = Date.Now

                Dim expr As String = Grid1.Rows(e.RowIndex).Cells("expression").Value

                Dim econtext As New ExpressionContext
                econtext.Imports.AddType(GetType(System.Math))
                econtext.Variables.Add("val", item.CurrentValue)
                Dim exbase As IGenericExpression(Of Double) = econtext.CompileGeneric(Of Double)(expr)

                Dim result As Object = exbase.Evaluate

                If Double.TryParse(result, New Double) Then
                    item.ExpressionResult = result
                    Grid1.Rows(e.RowIndex).Cells("result").Value = result
                Else
                    item.ExpressionResult = Double.NaN
                    Grid1.Rows(e.RowIndex).Cells("result").Value = Double.NaN
                End If

                Grid1.Rows(e.RowIndex).Cells("lastupdate").Value = Date.Now.ToString

                Grid1.Rows(e.RowIndex).Cells("currentvalue").Value = item.CurrentValue.ToString
                Grid1.Rows(e.RowIndex).Cells("monitoreditem").Value = ritem.DisplayName.ToString() + " [" + ritem.NodeId.ToString() + "]"
                Grid1.Rows(e.RowIndex).Cells("lastupdate").Value = Date.Now.ToString

            End If

        End If

    End Sub

    Private Sub HelpToolStripButton_Click(sender As Object, e As EventArgs) Handles HelpToolStripButton.Click

        Process.Start("https://dwsim.org/wiki/index.php?title=OPCPlugin")

    End Sub

    Private Sub tsAutoUpdate_TextChanged(sender As Object, e As EventArgs) Handles tsAutoUpdate.TextChanged

        If UpdateTimer IsNot Nothing Then
            If Double.TryParse(tsAutoUpdate.Text, New Double) Then
                UpdateTimer.Interval = Convert.ToDouble(tsAutoUpdate.Text) * 1000
            End If
        End If

    End Sub

    Private Sub ToolStripButton1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton1.Click

        Dim filePickerForm As IFilePicker = FilePickerService.GetInstance().GetFilePicker()

        Dim handler As IVirtualFile = filePickerForm.ShowOpenDialog(
            New List(Of FilePickerAllowedType) From {New FilePickerAllowedType("OPC Variable List", "*.dwopc")})

        If handler IsNot Nothing Then
            Try
                Dim text = handler.ReadAllText()
                LinkList = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, OPCLink))(text)
                ReadData()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub

    Private Sub btnAutoUpdate_CheckedChanged(sender As Object, e As EventArgs) Handles btnAutoUpdate.CheckedChanged
        If UpdateTimer IsNot Nothing Then
            UpdateTimer.Enabled = btnAutoUpdate.Checked
        End If
        btnAutoCalculate.Enabled = btnAutoUpdate.Checked
    End Sub

    Private Sub ToolStripButton8_Click(sender As Object, e As EventArgs) Handles ToolStripButton8.Click

        readhandler(Me, New EventArgs, Nothing)

    End Sub

    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        Process.Start(IO.Path.Combine(IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location), "opclibraries", "Opc.Ua.SampleClient.exe"))
    End Sub

    Private Sub UpdateTimer_Tick(sender As Object, e As EventArgs) Handles UpdateTimer.Tick

        UpdateValues()

        readhandler(Me, New EventArgs, Nothing)

        If btnAutoCalculate.Checked Then fsheet.RequestCalculation2(False)

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        Disconnect()

    End Sub

    Private Sub ToolStripButton9_Click(sender As Object, e As EventArgs) Handles ToolStripButton9.Click

        writehandler(Me, New EventArgs, Nothing)

    End Sub

    Private Sub WriteValue(node As NodeId, value As Double)

        Dim valueToWrite As WriteValue = New WriteValue
        valueToWrite.NodeId = node
        valueToWrite.AttributeId = Attributes.Value
        valueToWrite.Value.Value = ChangeType(node, value)
        valueToWrite.Value.StatusCode = StatusCodes.Good
        valueToWrite.Value.ServerTimestamp = DateTime.MinValue
        valueToWrite.Value.SourceTimestamp = DateTime.MinValue
        Dim valuesToWrite As WriteValueCollection = New WriteValueCollection
        valuesToWrite.Add(valueToWrite)
        'write current value.
        Dim results As StatusCodeCollection = Nothing
        Dim diagnosticInfos As DiagnosticInfoCollection = Nothing
        m_session.Write(Nothing, valuesToWrite, results, diagnosticInfos)
        ClientBase.ValidateResponse(results, valuesToWrite)
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite)
        If StatusCode.IsBad(results(0)) Then
            Throw New ServiceResultException(results(0))
        End If

    End Sub

    Private Function ChangeType(node As NodeId, value As Object) As Object

        Dim currval = ReadCurrentValue(node)

        Select Case (currval.WrappedValue.TypeInfo.BuiltInType)
            Case BuiltInType.Boolean
                value = Convert.ToBoolean(value)
                Exit Select
            Case BuiltInType.SByte
                value = Convert.ToSByte(value)
                Exit Select
            Case BuiltInType.Byte
                value = Convert.ToByte(value)
                Exit Select
            Case BuiltInType.Int16
                value = Convert.ToInt16(value)
                Exit Select
            Case BuiltInType.UInt16
                value = Convert.ToUInt16(value)
                Exit Select
            Case BuiltInType.Int32
                value = Convert.ToInt32(value)
                Exit Select
            Case BuiltInType.UInt32
                value = Convert.ToUInt32(value)
                Exit Select
            Case BuiltInType.Int64
                value = Convert.ToInt64(value)
                Exit Select
            Case BuiltInType.UInt64
                value = Convert.ToUInt64(value)
                Exit Select
            Case BuiltInType.Float
                value = Convert.ToSingle(value)
                Exit Select
            Case BuiltInType.Double
                value = Convert.ToDouble(value)
                Exit Select
            Case Else
                value = value
                Exit Select
        End Select

        Return value

    End Function

    Private Function ReadCurrentValue(node As NodeId) As DataValue

        Dim nodeToRead As ReadValueId = New ReadValueId
        nodeToRead.NodeId = node
        nodeToRead.AttributeId = Attributes.Value
        Dim nodesToRead As ReadValueIdCollection = New ReadValueIdCollection
        nodesToRead.Add(nodeToRead)
        Dim results As DataValueCollection = Nothing
        Dim diagnosticInfos As DiagnosticInfoCollection = Nothing
        m_session.Read(Nothing, 0, TimestampsToReturn.Neither, nodesToRead, results, diagnosticInfos)
        ClientBase.ValidateResponse(results, nodesToRead)
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead)
        Return results(0)

    End Function

End Class