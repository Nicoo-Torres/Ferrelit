Public Class Desarrollo
    Private Sub Desarrollo_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        TextBoxUsuario.Focus()
    End Sub
    Private Sub TextBoxContraseña_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxContraseña.KeyPress
        If Char.IsLetter(e.KeyChar) Then
            e.Handled = True
        End If
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        End If
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxContraseña.TextChanged

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub ButtonAceptar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAceptar.Click
        Dim usuario As String
        Dim contraseña As Integer
        usuario = TextBoxUsuario.Text
        contraseña = TextBoxContraseña.Text
        If (usuario = "dev") And (contraseña = "123") Then
            MsgBox("Opción de desarrollador habilitada.", vbInformation, "Felicidades!")
            Me.Close()
        Else
            MsgBox("Contraseña o usuario incorrecto", vbExclamation, "Error")
        End If
    End Sub
    Private Sub TextBoxUsuario_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxUsuario.KeyPress
        If Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        End If
    End Sub
End Class