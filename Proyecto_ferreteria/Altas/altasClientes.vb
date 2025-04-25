Public Class altasClientes

    Private Sub altasClientes_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        DniTextBox.Focus()
    End Sub
    Private Sub LimpiarCampos()
        DniTextBox.Text = ""
        NombreTextBox.Text = ""
        ApellidoTextBox.Text = ""
        DomicilioTextBox.Text = ""
        AlturaTextBox.Text = ""
        Id_localidadTextBox.Text = ""
    End Sub
    Private Function ConfirmarAccion(ByVal mensaje As String, Optional ByVal titulo As String = "Confirmar") As Boolean
        Return MsgBox(mensaje, vbYesNo + vbQuestion, titulo) = vbYes
    End Function

    Private Function PedirCodigoCliente(ByVal titulo As String, ByVal mensaje As String) As String
        Return InputBox(mensaje, titulo)
    End Function

    Private Sub ClientesBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Validate()
        Me.ClientesBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
    End Sub

    Private Sub altasClientes_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        RecargarClientes()

        ClientesBindingSource.AddNew()

        If ClientesBindingSource.Current IsNot Nothing AndAlso Not IsDBNull(ClientesBindingSource.Current("id_cliente")) Then
            Id_clienteTextBox.Text = ClientesBindingSource.Current("id_cliente").ToString()
        End If

        LimpiarCampos()

        DniTextBox.Focus()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNuevo.Click

        If String.IsNullOrWhiteSpace(DniTextBox.Text) OrElse String.IsNullOrWhiteSpace(NombreTextBox.Text) Then
            MostrarMensaje("Debe completar los campos DNI y Nombre.", "Atención", vbExclamation)
            DniTextBox.Focus()
            Return
        End If

        Try

            ClientesBindingSource.EndEdit()

            ClientesTableAdapter.Update(FerreteriaDataSet.Clientes)

            RecargarClientes()

            MostrarMensaje("Cliente agregado correctamente.")

            If ClientesBindingSource.Count > 0 Then
                ClientesBindingSource.MoveLast()
            End If

            ClientesBindingSource.AddNew()
            DniTextBox.Focus()

        Catch ex As Exception
            MostrarMensaje("Error al agregar el cliente: " & ex.Message, "Error", vbCritical)
        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim CodConsulta As String
        Dim fila As Integer

        CodConsulta = PedirCodigoCliente("Búsqueda de Cliente", "Ingrese el ID del cliente:")

        If CodConsulta Is Nothing Then Exit Sub

        If CodConsulta.Trim() = "" Then
            MostrarMensaje("Debe ingresar un número de ID para continuar.", "Campo obligatorio", vbExclamation)
            DniTextBox.Focus()
            Exit Sub
        End If

        Dim codigoNumerico As Integer
        If Not Integer.TryParse(CodConsulta, codigoNumerico) Then
            MostrarMensaje("El ID ingresado no es un número válido.", "Dato inválido", vbExclamation)
            Exit Sub
        End If

        fila = Me.ClientesBindingSource.Find("id_cliente", codigoNumerico)

        If fila = -1 Then
            MostrarMensaje("No se encontró el cliente con el ID ingresado.", "Atención", vbInformation)
            DniTextBox.Focus()
        Else

            Me.ClientesBindingSource.Position = fila
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim CodConsulta As String
        Dim fila As Integer

        CodConsulta = PedirCodigoCliente("Búsqueda de Cliente", "Ingrese el ID del cliente:")

        If CodConsulta Is Nothing Then Exit Sub

        If CodConsulta.Trim() = "" Then
            MostrarMensaje("Debe ingresar un número de ID para continuar.", "Campo obligatorio", vbExclamation)
            DniTextBox.Focus()
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(CodConsulta) Then
            MostrarMensaje("Debe ingresar un número de ID para continuar.", "Campo obligatorio", vbExclamation)
            DniTextBox.Focus()
            Exit Sub
        End If

        Dim codigoNumerico As Integer
        If Not Integer.TryParse(CodConsulta, codigoNumerico) Then
            MostrarMensaje("El ID ingresado no es un número válido.", "Dato inválido", vbExclamation)
            DniTextBox.Focus()
            Exit Sub
        End If

        fila = Me.ClientesBindingSource.Find("id_cliente", codigoNumerico)

        If fila = -1 Then
            MostrarMensaje("No se encontró el cliente con el ID ingresado.", "Información", vbInformation)
            DniTextBox.Focus()
        Else

            Me.ClientesBindingSource.Position = fila

            If ConfirmarAccion("¿Está seguro que desea eliminar al cliente con ID " & CodConsulta & "?", "Eliminar Cliente") Then
                Try
                    Me.ClientesBindingSource.RemoveCurrent()
                    Me.ClientesBindingSource.EndEdit()

                    Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)

                    Me.FerreteriaDataSet.AcceptChanges()

                    RecargarClientes()

                    If ClientesBindingSource.Count = 0 OrElse ClientesBindingSource.Current Is Nothing Then
                        ClientesBindingSource.AddNew()

                        If ClientesBindingSource.Current IsNot Nothing Then
                            If Not IsDBNull(ClientesBindingSource.Current("id_cliente")) Then
                                Id_clienteTextBox.Text = ClientesBindingSource.Current("id_cliente").ToString()
                            End If
                        End If
                    End If

                    DniTextBox.Focus()
                Catch ex As Exception
                    MostrarMensaje("Ocurrió un error al eliminar el cliente: " & ex.Message, "Error", vbCritical)
                End Try
            End If
        End If
    End Sub

    Private Sub HabilitarCampos(ByVal habilitar As Boolean)
        DNITextBox.Enabled = habilitar
        NombreTextBox.Enabled = habilitar
        ApellidoTextBox.Enabled = habilitar
        DomicilioTextBox.Enabled = habilitar
        AlturaTextBox.Enabled = habilitar
        Id_localidadTextBox.Enabled = habilitar

        GuardarButton.Enabled = habilitar
        ButtonEditar.Enabled = Not habilitar
    End Sub

    Private Sub ButtonEditar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonEditar.Click
        Dim CodConsulta As String = PedirCodigoCliente("Búsqueda de Cliente", "Ingrese el ID del cliente:")

        If String.IsNullOrWhiteSpace(CodConsulta) Then
            MostrarMensaje("Debe ingresar un código de cliente para editar.", "Edición de cliente", vbExclamation)
            Return
        End If

        Dim fila As Integer = Me.ClientesBindingSource.Find("id_cliente", CodConsulta)

        If fila = -1 Then
            MostrarMensaje("No se encontró el cliente con el ID ingresado.", "Atención", vbInformation)
        Else
            Me.ClientesBindingSource.Position = fila
            HabilitarCampos(True)
            MostrarMensaje("Puede editar los datos del cliente. Presione 'Guardar' al finalizar.")
        End If
    End Sub

    Private Sub ButtonGuardar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GuardarButton.Click

        If String.IsNullOrWhiteSpace(DniTextBox.Text) OrElse String.IsNullOrWhiteSpace(NombreTextBox.Text) Then
            MostrarMensaje("Debe completar los campos DNI y Nombre.", "Atención", vbExclamation)
            DniTextBox.Focus()
            Exit Sub
        End If

        Try

            Me.Validate()
            Me.ClientesBindingSource.EndEdit()

            Dim cambios = Me.FerreteriaDataSet.HasChanges()
            If cambios Then
                Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
                MostrarMensaje("Los cambios se guardaron con éxito.")
                HabilitarCampos(False)
            Else
                MostrarMensaje("No hay cambios para guardar.", "Información", vbInformation)

            End If
        Catch ex As Exception
            MostrarMensaje("Error al guardar los cambios: " & ex.Message, "Error", vbCritical)
        End Try
    End Sub

    Private Sub MostrarMensaje(ByVal mensaje As String, Optional ByVal titulo As String = "Información", Optional ByVal tipo As MsgBoxStyle = vbInformation)
        MsgBox(mensaje, tipo, titulo)
    End Sub
    Private Sub RecargarClientes()
        Me.ClientesTableAdapter.Fill(Me.FerreteriaDataSet.Clientes)
    End Sub
End Class