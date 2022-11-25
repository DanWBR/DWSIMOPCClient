<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ItemList
    Inherits System.Windows.Forms.Form

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
        Me.BrowseCTRL = New Opc.Ua.Sample.Controls.BrowseTreeCtrl()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BrowseCTRL
        '
        Me.BrowseCTRL.AttributesCtrl = Nothing
        Me.BrowseCTRL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BrowseCTRL.EnableDragging = False
        Me.BrowseCTRL.Location = New System.Drawing.Point(3, 3)
        Me.BrowseCTRL.Name = "BrowseCTRL"
        Me.BrowseCTRL.SessionTreeCtrl = Nothing
        Me.BrowseCTRL.Size = New System.Drawing.Size(660, 365)
        Me.BrowseCTRL.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.BrowseCTRL, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(666, 401)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button1.Location = New System.Drawing.Point(576, 374)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(87, 24)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = " Select"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ItemList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(666, 401)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ItemList"
        Me.ShowIcon = False
        Me.Text = "Select Item"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Button1 As Windows.Forms.Button
    Public WithEvents BrowseCTRL As Opc.Ua.Sample.Controls.BrowseTreeCtrl
End Class
