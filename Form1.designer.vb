﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton8 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton9 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton7 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.HelpToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnAutoUpdate = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.tsAutoUpdate = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.btnAutoCalculate = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton6 = New System.Windows.Forms.ToolStripButton()
        Me.btnAutoWrite = New System.Windows.Forms.ToolStripButton()
        Me.Grid1 = New System.Windows.Forms.DataGridView()
        Me.id = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.active = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.itemname = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.comment = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.associatedobject = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.associatedproperty = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.monitoreditem = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.selectitem = New System.Windows.Forms.DataGridViewButtonColumn()
        Me.linktype = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.currentvalue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.servervalue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.minimumvalue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.maximumvalue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.failsafevalue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.expression = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.result = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.unit = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.lastupdate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EndpointSelectorCTRL = New Opc.Ua.Client.Controls.EndpointSelectorCtrl()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SessionsCTRL = New Opc.Ua.Sample.Controls.SessionTreeCtrl()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ServerUrlLB = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ServerStatusLB = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.Grid1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton3, Me.ToolStripButton4, Me.ToolStripSeparator3, Me.ToolStripButton5, Me.ToolStripButton8, Me.ToolStripButton9, Me.ToolStripSeparator1, Me.ToolStripButton1, Me.ToolStripButton2, Me.ToolStripButton7, Me.ToolStripSeparator2, Me.HelpToolStripButton, Me.ToolStripSeparator4, Me.btnAutoUpdate, Me.ToolStripLabel1, Me.tsAutoUpdate, Me.ToolStripLabel2, Me.btnAutoCalculate, Me.ToolStripButton6, Me.btnAutoWrite})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(957, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripButton3
        '
        Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton3.Image = Global.DWSIMOPCClient.My.Resources.Resources.add
        Me.ToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton3.Name = "ToolStripButton3"
        Me.ToolStripButton3.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton3.Text = "Add"
        '
        'ToolStripButton4
        '
        Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton4.Image = Global.DWSIMOPCClient.My.Resources.Resources.minus
        Me.ToolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton4.Name = "ToolStripButton4"
        Me.ToolStripButton4.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton4.Text = "Remove"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton5
        '
        Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton5.Image = Global.DWSIMOPCClient.My.Resources.Resources.refresh
        Me.ToolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton5.Name = "ToolStripButton5"
        Me.ToolStripButton5.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton5.Text = "Update Values"
        '
        'ToolStripButton8
        '
        Me.ToolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton8.Image = Global.DWSIMOPCClient.My.Resources.Resources.icons8_download_from_ftp
        Me.ToolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton8.Name = "ToolStripButton8"
        Me.ToolStripButton8.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton8.Text = "Read from Server / Write to Flowsheet"
        '
        'ToolStripButton9
        '
        Me.ToolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton9.Image = Global.DWSIMOPCClient.My.Resources.Resources.icons8_upload_to_ftp
        Me.ToolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton9.Name = "ToolStripButton9"
        Me.ToolStripButton9.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton9.Text = "Read from Flowsheet / Write to Server"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.DWSIMOPCClient.My.Resources.Resources.folder_open
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "Open"
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton2.Image = Global.DWSIMOPCClient.My.Resources.Resources.save
        Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton2.Name = "ToolStripButton2"
        Me.ToolStripButton2.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton2.Text = "Save"
        '
        'ToolStripButton7
        '
        Me.ToolStripButton7.Checked = True
        Me.ToolStripButton7.CheckOnClick = True
        Me.ToolStripButton7.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ToolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton7.Image = Global.DWSIMOPCClient.My.Resources.Resources.yes
        Me.ToolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton7.Name = "ToolStripButton7"
        Me.ToolStripButton7.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton7.Text = "AutoSave"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'HelpToolStripButton
        '
        Me.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.HelpToolStripButton.Image = CType(resources.GetObject("HelpToolStripButton.Image"), System.Drawing.Image)
        Me.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.HelpToolStripButton.Name = "HelpToolStripButton"
        Me.HelpToolStripButton.Size = New System.Drawing.Size(23, 22)
        Me.HelpToolStripButton.Text = "Help"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
        '
        'btnAutoUpdate
        '
        Me.btnAutoUpdate.CheckOnClick = True
        Me.btnAutoUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnAutoUpdate.Image = CType(resources.GetObject("btnAutoUpdate.Image"), System.Drawing.Image)
        Me.btnAutoUpdate.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAutoUpdate.Name = "btnAutoUpdate"
        Me.btnAutoUpdate.Size = New System.Drawing.Size(156, 22)
        Me.btnAutoUpdate.Text = "AutoUpdate READ Variables"
        Me.btnAutoUpdate.ToolTipText = "Auto update variable values from server and write them to the flowsheet. "
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(46, 22)
        Me.ToolStripLabel1.Text = "Interval"
        '
        'tsAutoUpdate
        '
        Me.tsAutoUpdate.Name = "tsAutoUpdate"
        Me.tsAutoUpdate.Size = New System.Drawing.Size(30, 25)
        Me.tsAutoUpdate.Text = "60"
        Me.tsAutoUpdate.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(12, 22)
        Me.ToolStripLabel2.Text = "s"
        '
        'btnAutoCalculate
        '
        Me.btnAutoCalculate.CheckOnClick = True
        Me.btnAutoCalculate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnAutoCalculate.Image = CType(resources.GetObject("btnAutoCalculate.Image"), System.Drawing.Image)
        Me.btnAutoCalculate.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAutoCalculate.Name = "btnAutoCalculate"
        Me.btnAutoCalculate.Size = New System.Drawing.Size(86, 22)
        Me.btnAutoCalculate.Text = "AutoCalculate"
        Me.btnAutoCalculate.ToolTipText = "Requests a Flowsheet Calculation after a successful update cycle."
        '
        'ToolStripButton6
        '
        Me.ToolStripButton6.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripButton6.Image = Global.DWSIMOPCClient.My.Resources.Resources.App
        Me.ToolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton6.Name = "ToolStripButton6"
        Me.ToolStripButton6.Size = New System.Drawing.Size(164, 22)
        Me.ToolStripButton6.Text = "Start Sample Client/Server"
        Me.ToolStripButton6.ToolTipText = "Start Sample Client/Server"
        '
        'btnAutoWrite
        '
        Me.btnAutoWrite.CheckOnClick = True
        Me.btnAutoWrite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnAutoWrite.Image = CType(resources.GetObject("btnAutoWrite.Image"), System.Drawing.Image)
        Me.btnAutoWrite.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAutoWrite.Name = "btnAutoWrite"
        Me.btnAutoWrite.Size = New System.Drawing.Size(150, 22)
        Me.btnAutoWrite.Text = "AutoWrite WRITE Variables"
        Me.btnAutoWrite.ToolTipText = "Automatically updates WRITE variables after a successful flowsheet run"
        '
        'Grid1
        '
        Me.Grid1.AllowUserToAddRows = False
        Me.Grid1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.Grid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.Grid1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.id, Me.active, Me.itemname, Me.comment, Me.associatedobject, Me.associatedproperty, Me.monitoreditem, Me.selectitem, Me.linktype, Me.currentvalue, Me.servervalue, Me.minimumvalue, Me.maximumvalue, Me.failsafevalue, Me.expression, Me.result, Me.unit, Me.lastupdate})
        Me.Grid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Grid1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.Grid1.Location = New System.Drawing.Point(0, 0)
        Me.Grid1.Name = "Grid1"
        Me.Grid1.RowHeadersWidth = 25
        Me.Grid1.Size = New System.Drawing.Size(824, 336)
        Me.Grid1.TabIndex = 1
        '
        'id
        '
        Me.id.HeaderText = "ID"
        Me.id.MinimumWidth = 20
        Me.id.Name = "id"
        Me.id.Visible = False
        Me.id.Width = 43
        '
        'active
        '
        Me.active.FalseValue = "False"
        Me.active.FillWeight = 15.0!
        Me.active.HeaderText = ""
        Me.active.MinimumWidth = 20
        Me.active.Name = "active"
        Me.active.TrueValue = "True"
        Me.active.Width = 20
        '
        'itemname
        '
        Me.itemname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.itemname.FillWeight = 54.21845!
        Me.itemname.HeaderText = "Name"
        Me.itemname.Name = "itemname"
        Me.itemname.Width = 60
        '
        'comment
        '
        Me.comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.comment.FillWeight = 150.0!
        Me.comment.HeaderText = "Description"
        Me.comment.Name = "comment"
        Me.comment.Width = 85
        '
        'associatedobject
        '
        Me.associatedobject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.associatedobject.HeaderText = "Object"
        Me.associatedobject.Name = "associatedobject"
        Me.associatedobject.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.associatedobject.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.associatedobject.Width = 63
        '
        'associatedproperty
        '
        Me.associatedproperty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.associatedproperty.FillWeight = 200.0!
        Me.associatedproperty.HeaderText = "Property"
        Me.associatedproperty.Name = "associatedproperty"
        Me.associatedproperty.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.associatedproperty.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.associatedproperty.Width = 71
        '
        'monitoreditem
        '
        Me.monitoreditem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.monitoreditem.FillWeight = 144.5825!
        Me.monitoreditem.HeaderText = "Monitored Item"
        Me.monitoreditem.Name = "monitoreditem"
        Me.monitoreditem.Width = 102
        '
        'selectitem
        '
        Me.selectitem.HeaderText = "Select"
        Me.selectitem.Name = "selectitem"
        Me.selectitem.Text = "..."
        Me.selectitem.UseColumnTextForButtonValue = True
        Me.selectitem.Width = 43
        '
        'linktype
        '
        Me.linktype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.linktype.HeaderText = "Type"
        Me.linktype.Name = "linktype"
        Me.linktype.Width = 37
        '
        'currentvalue
        '
        Me.currentvalue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.currentvalue.DefaultCellStyle = DataGridViewCellStyle1
        Me.currentvalue.FillWeight = 54.21845!
        Me.currentvalue.HeaderText = "Current Val"
        Me.currentvalue.Name = "currentvalue"
        Me.currentvalue.ReadOnly = True
        Me.currentvalue.Width = 84
        '
        'servervalue
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.servervalue.DefaultCellStyle = DataGridViewCellStyle2
        Me.servervalue.HeaderText = "Server Val"
        Me.servervalue.Name = "servervalue"
        Me.servervalue.ReadOnly = True
        Me.servervalue.Width = 81
        '
        'minimumvalue
        '
        Me.minimumvalue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.minimumvalue.DefaultCellStyle = DataGridViewCellStyle3
        Me.minimumvalue.FillWeight = 54.21845!
        Me.minimumvalue.HeaderText = "Min Val"
        Me.minimumvalue.Name = "minimumvalue"
        Me.minimumvalue.Width = 67
        '
        'maximumvalue
        '
        Me.maximumvalue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.maximumvalue.DefaultCellStyle = DataGridViewCellStyle4
        Me.maximumvalue.FillWeight = 54.21845!
        Me.maximumvalue.HeaderText = "Max Val"
        Me.maximumvalue.Name = "maximumvalue"
        Me.maximumvalue.Width = 70
        '
        'failsafevalue
        '
        Me.failsafevalue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.failsafevalue.DefaultCellStyle = DataGridViewCellStyle5
        Me.failsafevalue.FillWeight = 54.21845!
        Me.failsafevalue.HeaderText = "Val on Error"
        Me.failsafevalue.Name = "failsafevalue"
        Me.failsafevalue.Width = 87
        '
        'expression
        '
        Me.expression.HeaderText = "Expression"
        Me.expression.Name = "expression"
        Me.expression.Width = 83
        '
        'result
        '
        Me.result.HeaderText = "Result"
        Me.result.Name = "result"
        Me.result.ReadOnly = True
        Me.result.Width = 62
        '
        'unit
        '
        Me.unit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.unit.FillWeight = 45.18204!
        Me.unit.HeaderText = "Units"
        Me.unit.Name = "unit"
        Me.unit.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.unit.Width = 56
        '
        'lastupdate
        '
        Me.lastupdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.lastupdate.FillWeight = 30.0!
        Me.lastupdate.HeaderText = "Last Update"
        Me.lastupdate.Name = "lastupdate"
        Me.lastupdate.ReadOnly = True
        Me.lastupdate.Width = 90
        '
        'EndpointSelectorCTRL
        '
        Me.EndpointSelectorCTRL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.EndpointSelectorCTRL.Location = New System.Drawing.Point(122, 3)
        Me.EndpointSelectorCTRL.MaximumSize = New System.Drawing.Size(2048, 27)
        Me.EndpointSelectorCTRL.MinimumSize = New System.Drawing.Size(100, 27)
        Me.EndpointSelectorCTRL.Name = "EndpointSelectorCTRL"
        Me.EndpointSelectorCTRL.Padding = New System.Windows.Forms.Padding(1, 0, 0, 0)
        Me.EndpointSelectorCTRL.SelectedEndpoint = Nothing
        Me.EndpointSelectorCTRL.Size = New System.Drawing.Size(832, 27)
        Me.EndpointSelectorCTRL.TabIndex = 4
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 57)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SessionsCTRL)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Grid1)
        Me.SplitContainer1.Size = New System.Drawing.Size(957, 336)
        Me.SplitContainer1.SplitterDistance = 129
        Me.SplitContainer1.TabIndex = 5
        '
        'SessionsCTRL
        '
        Me.SessionsCTRL.AddressSpaceCtrl = Nothing
        Me.SessionsCTRL.Configuration = Nothing
        Me.SessionsCTRL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SessionsCTRL.EnableDragging = False
        Me.SessionsCTRL.Location = New System.Drawing.Point(0, 0)
        Me.SessionsCTRL.MessageContext = Nothing
        Me.SessionsCTRL.Name = "SessionsCTRL"
        Me.SessionsCTRL.NotificationMessagesCtrl = Nothing
        Me.SessionsCTRL.PreferredLocales = Nothing
        Me.SessionsCTRL.ServerStatusCtrl = Nothing
        Me.SessionsCTRL.Size = New System.Drawing.Size(129, 336)
        Me.SessionsCTRL.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.EndpointSelectorCTRL, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 25)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(957, 32)
        Me.TableLayoutPanel1.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(113, 32)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Connect to Server"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ServerUrlLB, Me.ServerStatusLB})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 393)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(957, 22)
        Me.StatusStrip1.TabIndex = 7
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ServerUrlLB
        '
        Me.ServerUrlLB.Name = "ServerUrlLB"
        Me.ServerUrlLB.Size = New System.Drawing.Size(79, 17)
        Me.ServerUrlLB.Text = "Disconnected"
        '
        'ServerStatusLB
        '
        Me.ServerStatusLB.Name = "ServerStatusLB"
        Me.ServerStatusLB.Size = New System.Drawing.Size(0, 17)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(957, 415)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "Form1"
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom
        Me.ShowIcon = False
        Me.TabText = "DWSIM OPC UA Client Plugin"
        Me.Text = "DWSIM OPC UA Client Plugin"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        CType(Me.Grid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents HelpToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents Grid1 As System.Windows.Forms.DataGridView
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton3 As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton4 As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripButton5 As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton2 As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripButton7 As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAutoUpdate As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton8 As System.Windows.Forms.ToolStripButton
    Private WithEvents EndpointSelectorCTRL As Opc.Ua.Client.Controls.EndpointSelectorCtrl
    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Protected WithEvents SessionsCTRL As Opc.Ua.Sample.Controls.SessionTreeCtrl
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents ToolStripLabel1 As Windows.Forms.ToolStripLabel
    Friend WithEvents tsAutoUpdate As Windows.Forms.ToolStripTextBox
    Friend WithEvents ToolStripLabel2 As Windows.Forms.ToolStripLabel
    Friend WithEvents btnAutoCalculate As Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton6 As Windows.Forms.ToolStripButton
    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Private WithEvents ServerUrlLB As Windows.Forms.ToolStripStatusLabel
    Private WithEvents ServerStatusLB As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnAutoWrite As Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton9 As Windows.Forms.ToolStripButton
    Friend WithEvents id As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents active As Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents itemname As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents comment As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents associatedobject As Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents associatedproperty As Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents monitoreditem As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents selectitem As Windows.Forms.DataGridViewButtonColumn
    Friend WithEvents linktype As Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents currentvalue As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents servervalue As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents minimumvalue As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents maximumvalue As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents failsafevalue As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents expression As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents result As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents unit As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents lastupdate As Windows.Forms.DataGridViewTextBoxColumn
End Class
