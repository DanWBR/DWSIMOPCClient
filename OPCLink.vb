﻿Imports Opc.Ua

<System.Serializable()> Public Class OPCLink

    Public Enum LType
        Read = 0
        Write = 1
    End Enum

    Public Property ID As String = ""
    Public Property Active As Boolean = True
    Public Property Name As String = ""
    Public Property MonitoredVariable As ReferenceDescription
    Public Property Expression As String = "val"
    Public Property ExpressionResult As Double = -1000.0
    Public Property MaximumValue As Double = 1000.0
    Public Property MinimumValue As Double = 0.0#
    Public Property FailSafeValue As Double = 0.0#
    Public Property ObjectID As String = ""
    Public Property PropertyID As String = ""
    Public Property ExpressionUnits As String = ""
    Public Property CurrentValue As Double = 0.0#
    Public Property PreviousValue As Double = 0.0#
    Public Property LastUpdate As Date
    Public Property Comment As String = ""
    Public Property LinkType As LType = LType.Read
    Public Property ServerValue As Double = 0.0

    Sub New()

    End Sub

End Class
