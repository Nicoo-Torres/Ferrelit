Module Module1
    Public Sub limpiar(ByVal formx As Form)
        Dim aux As Control
        For Each aux In formx.Controls
            If TypeOf aux Is TextBox Then
                aux.Text = "" ' Asigna una cadena vacía
            End If
        Next
    End Sub
End Module
