Imports System.Windows.Forms
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

Public Class Form1

    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    'flowsheet reference
    Public fsheet As IFlowsheet

    Public LinkList As Dictionary(Of String, OPCLink)

    Public cbc2, cbc3, cbc4 As DataGridViewComboBoxCell

    Private m_session As Session
    Private m_reconnectHandler As SessionReconnectHandler
    Private m_reconnectPeriod As Integer
    Private m_application As ApplicationInstance
    Private m_server As Opc.Ua.Server.StandardServer
    Private m_endpoints As ConfiguredEndpointCollection
    Private m_configuration As ApplicationConfiguration
    Private m_context As ServiceMessageContext

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        'remove SelectedObjectChanged event handlers

        Dim myeventhandler As CustomEvent = AddressOf eventhandler

        RemoveHandler FlowsheetSolver.FlowsheetCalculationStarted, myeventhandler

        My.Settings.Save()

    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Dim myeventhandler As CustomEvent = AddressOf eventhandler

        AddHandler FlowsheetSolver.FlowsheetCalculationStarted, myeventhandler

        LinkList = New Dictionary(Of String, OPCLink)

        cbc2 = New DataGridViewComboBoxCell
        cbc2.Sorted = False
        cbc2.MaxDropDownItems = 10
        cbc2.DropDownWidth = 200
        cbc2.Items.Add("Planilha")
        For Each obj In fsheet.SimulationObjects.Values
            cbc2.Items.Add(obj.GraphicObject.Tag)
        Next

        cbc3 = New DataGridViewComboBoxCell
        cbc3.MaxDropDownItems = 10
        cbc3.DropDownWidth = 200

        Me.Grid1.Columns("associatedobject").CellTemplate = cbc2
        Me.Grid1.Columns("associatedproperty").CellTemplate = cbc3

        Dim xpifile As String = IO.Path.GetDirectoryName(fsheet.Options.FilePath) & "\" & IO.Path.GetFileNameWithoutExtension(fsheet.Options.FilePath) & ".dwxpi"

        If IO.File.Exists(xpifile) Then
            Dim xdoc As XDocument = XDocument.Load(xpifile)
            Dim data As List(Of XElement) = xdoc.Element("PILinkData").Element("PILinkList").Elements.ToList
            LoadList()
            ReadData()
        End If

        Grid1.ChangeEditModeToOnPropertyChanged()

        Dim app As New ApplicationInstance()
        app.ApplicationName = "DWSIMOPCClient"
        app.ApplicationType = ApplicationType.Client
        app.ConfigSectionName = "DWSIMOPCClient"

        m_application = app

        Dim cpath = AppDomain.CurrentDomain.BaseDirectory
        cpath = System.IO.Path.Combine(cpath, "plugins", "DWSIMOPCClient.Config.xml")

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

        'initialize control state.
        Disconnect()

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

        If ((Not (e) Is Nothing) _
                    AndAlso (Not (m_session) Is Nothing)) Then
            If ServiceResult.IsGood(e.Status) Then
            Else
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

        Dim id As String = Guid.NewGuid.ToString

        Me.Grid1.Rows.Add()
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).HeaderCell.Value = Me.Grid1.Rows.Count.ToString
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("id").Value = id
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("active").Value = True
        Me.Grid1.Rows(Me.Grid1.Rows.Count - 1).Cells("itemname").Value = "OPCLink" & Me.Grid1.Rows.Count

        LinkList.Add(id, New OPCLink() With {.ID = id, .Name = "OPCLink" & Me.Grid1.Rows.Count})

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
            Select Case e.ColumnIndex
                Case 4
                    Dim cbc As DataGridViewComboBoxCell = Me.Grid1.Rows(e.RowIndex).Cells("associatedproperty")
                    cbc.Items.Clear()
                    With cbc.Items
                        If Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString <> "" Then
                            Dim props As String()
                            If Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString = "Planilha" Then
                                props = fsheet.FormSpreadsheet.GetCellString()
                            Else
                                props = Me.ReturnProperties(Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value, False)
                            End If
                            For Each prop As String In props
                                .Add(fsheet.GetTranslatedString(prop))
                            Next
                        End If
                    End With
                Case 5
                    If Not Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                        If Not Me.Grid1.Rows(e.RowIndex).Cells(4).Value.ToString = "Planilha" Then
                            Dim props As String() = Me.ReturnProperties(Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value, False)
                            For Each prop As String In props
                                If fsheet.GetTranslatedString(prop) = Me.Grid1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString Then
                                    Dim obj As DWSIM.Interfaces.ISimulationObject = ReturnObject(Me.Grid1.Rows(e.RowIndex).Cells("associatedobject").Value)
                                    Me.Grid1.Rows(e.RowIndex).Cells("unit") = GetUnits(obj.GetPropertyUnit(prop))
                                    Exit For
                                End If
                            Next
                        End If
                    End If
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
                If dependent Then
                    Return obj.GetProperties(Interfaces.Enums.PropertyType.ALL)
                Else
                    Return obj.GetProperties(Interfaces.Enums.PropertyType.WR)
                End If
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

        Dim refdate As Date, delta As TimeSpan
        If btnRealTime.Checked Then
            refdate = Date.Now
        End If

        For Each r As DataGridViewRow In Grid1.Rows

            If r.Cells("active").Value = True Then

                Try

                    delta = TimeSpan.Parse(r.Cells("interval").Value)

                    Dim expr As String = r.Cells("expression").Value

                    Dim pivars() = expr.Split(Chr(39))

                    Dim i As Integer, pival As Object
                    For i = 1 To pivars.Length - 1
                        If i Mod 2 Then
                            'pival = RetrievePIExpVal(cbPIServer.SelectedItem, "'" + pivars(i) + "'", refdate.Add(-delta), refdate)
                            expr = expr.Replace("'" + pivars(i) + "'", pival)
                        End If
                    Next

                    Dim econtext As New ExpressionContext
                    econtext.Imports.AddType(GetType(System.Math))
                    Dim exbase As IGenericExpression(Of Double) = econtext.CompileGeneric(Of Double)(expr)

                    Dim result As Object = exbase.Evaluate

                    If Double.TryParse(result, New Double) Then
                        r.Cells("currentvalue").Value = result
                    Else
                        r.Cells("currentvalue").Value = Double.NaN
                    End If

                    r.Cells("lastupdate").Value = Date.Now.ToString

                Catch ex As Exception

                    r.Cells("currentvalue").Value = ex.Message.ToString

                End Try

            Else



            End If

        Next

    End Sub

    Public Sub eventhandler(ByVal sender As Object, e As System.EventArgs, extraargs As Object)

        If btnAutoUpdate.Checked Then ToolStripButton5_Click(sender, e)

        For Each r As DataGridViewRow In Grid1.Rows

            If r.Cells("active").Value = True Then

                Dim propval As Double
                Dim selectedprop As String = r.Cells("associatedproperty").Value

                Try

                    If r.Cells("associatedobject").Value <> "Spreadsheet" Then

                        Dim objID As String = ReturnObject(r.Cells("associatedobject").Value).Name

                        Dim props As String() = Me.ReturnProperties(r.Cells("associatedobject").Value, False)

                        Dim propunit As String = r.Cells("unit").Value

                        Dim currentvalue As Object = r.Cells("currentvalue").Value
                        Dim maximumvalue As Double = r.Cells("maximumvalue").Value
                        Dim minimumvalue As Double = r.Cells("minimumvalue").Value
                        Dim failsafevalue As Double = r.Cells("failsafevalue").Value

                        If Double.TryParse(currentvalue, New Double) = False Then
                            propval = failsafevalue
                        Else
                            If Double.IsNaN(currentvalue) Or Double.IsInfinity(currentvalue) Then
                                propval = failsafevalue
                            Else
                                propval = currentvalue
                                If currentvalue > maximumvalue Then propval = maximumvalue
                                If currentvalue < minimumvalue Then propval = minimumvalue
                            End If
                        End If

                        For Each prop As String In props
                            If fsheet.GetTranslatedString(prop) = selectedprop Then
                                fsheet.SimulationObjects(objID).SetPropertyValue(prop, SharedClasses.SystemsOfUnits.Converter.ConvertToSI(propunit, propval))
                                Exit For
                            End If
                        Next

                    Else

                        Dim currentvalue As Object = r.Cells("currentvalue").Value
                        Dim maximumvalue As Double = r.Cells("maximumvalue").Value
                        Dim minimumvalue As Double = r.Cells("minimumvalue").Value
                        Dim failsafevalue As Double = r.Cells("failsafevalue").Value

                        If Double.TryParse(currentvalue, New Double) = False Then
                            propval = failsafevalue
                        Else
                            If Double.IsNaN(currentvalue) Or Double.IsInfinity(currentvalue) Then
                                propval = failsafevalue
                            Else
                                propval = currentvalue
                                If currentvalue > maximumvalue Then propval = maximumvalue
                                If currentvalue < minimumvalue Then propval = minimumvalue
                            End If
                        End If

                        fsheet.FormSpreadsheet.SetCellValue(selectedprop, propval)

                    End If

                Catch ex As Exception

                    fsheet.ShowMessage("Error writing '" & propval & "' to property '" & selectedprop & "' from object '" & r.Cells("associatedobject").Value & "': " & ex.ToString, IFlowsheet.MessageType.GeneralError)

                End Try

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
            End With
        Next

    End Sub

    Sub ReadData()

        Grid1.Rows.Clear()

        For Each p As OPCLink In LinkList.Values

            Try
                Grid1.Rows.Add(New Object() {p.ID, p.Active, p.Name, p.Comment, "",
                                             "", p.Expression, p.CurrentValue,
                                             p.MinimumValue, p.MaximumValue, p.FailSafeValue, "", p.LastUpdate})
                If p.ObjectID = "Spreadsheet" Then
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedobject").Value = "Spreadsheet"
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedproperty").Value = p.PropertyID
                Else
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedobject").Value = fsheet.SimulationObjects(p.ObjectID).GraphicObject.Tag
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("associatedproperty").Value = ReturnPropertyName(p.ObjectID, p.PropertyID)
                    Grid1.Rows(Grid1.Rows.Count - 1).Cells("unit").Value = p.ExpressionUnits
                End If
            Catch ex As Exception
                fsheet.ShowMessage("Erro: " & ex.ToString, IFlowsheet.MessageType.GeneralError)
            End Try

        Next

    End Sub

    Sub LoadList()

        LinkList.Clear()

    End Sub

    Function SaveList()


    End Function


    Private Sub ToolStripButton2_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton2.Click

        If Me.SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then

            Dim xdoc As New XDocument()
            Dim ci As CultureInfo = CultureInfo.InvariantCulture

            StoreData()

            xdoc.Add(New XElement("PILinkData"))
            'xdoc.Element("PILinkData").Add(New XElement("PIServer", Me.cbPIServer.SelectedItem.ToString))
            xdoc.Element("PILinkData").Add(SaveList)

            xdoc.Save(Me.SaveFileDialog1.FileName)

        End If

    End Sub

    Private Sub Grid1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Grid1.CellClick

        If e.ColumnIndex = 7 Then

            Dim selector As New ItemList
            selector.BrowseCTRL.SetView(m_session, BrowseViewType.Objects, Nothing)
            selector.ShowDialog()

            Dim ritem = selector.MonItem

            MessageBox.Show(m_session.ReadValue(ritem.NodeId).Value.ToString)

        End If

    End Sub

    Private Sub ToolStripButton1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton1.Click

        If Me.OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then

            Dim xdoc As XDocument = XDocument.Load(Me.OpenFileDialog1.FileName)

            LoadList()

            ReadData()

        End If

    End Sub

    Private Sub ToolStripButton8_Click(sender As Object, e As EventArgs) Handles ToolStripButton8.Click

        eventhandler(Me, New EventArgs, Nothing)

    End Sub

End Class