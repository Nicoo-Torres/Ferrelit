Public Class altasProducto
    Private comboBoxCargado As Boolean = False
    Private buscandoArticulo As Boolean = False
    Private Sub LimpiarCampos()

        DescripcionTextBox.Clear()
        PrecioTextBox.Clear()
        CantidadTextBox.Clear()

    End Sub

    Private Function ValidarCampos() As Boolean

        If String.IsNullOrWhiteSpace(DescripcionTextBox.Text) Then
            MsgBox("El campo de descripción no puede estar vacío.", vbExclamation, "Error de validación")
            DescripcionTextBox.Focus()
            Return False
        End If

        Dim precio As Decimal
        If Not Decimal.TryParse(PrecioTextBox.Text, precio) OrElse precio <= 0 Then
            MsgBox("Debe ingresar un precio válido mayor a 0.")
            DescripcionTextBox.Focus()
            Return False
        End If
        Return True
    End Function

    Private Sub HabilitarControlesEdicion(ByVal habilitar As Boolean)
        DescripcionTextBox.ReadOnly = Not habilitar
        PrecioTextBox.ReadOnly = Not habilitar
        CantidadTextBox.ReadOnly = Not habilitar

    End Sub
    Private Sub StockBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Validate()
        Me.StockBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
    End Sub
    Private Sub altasProducto_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'FerreteriaDataSet.Stock' table. You can move, or remove it, as needed.
        Me.StockTableAdapter.Fill(Me.FerreteriaDataSet.Stock)

        ComboBoxBusqueda.DataSource = Me.FerreteriaDataSet.Stock
        ComboBoxBusqueda.DisplayMember = "descripcion"
        ComboBoxBusqueda.ValueMember = "id_articulo"
        ComboBoxBusqueda.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBoxBusqueda.AutoCompleteSource = AutoCompleteSource.ListItems

        comboBoxCargado = True
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNuevo.Click

        If DescripcionTextBox.Text.Trim() <> "" AndAlso PrecioTextBox.Text.Trim() <> "" Then
            MessageBox.Show("Por favor, presione el botón 'Limpiar' antes de agregar un nuevo artículo.",
                            "Campos llenos", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DescripcionTextBox.Focus()
            Exit Sub
        End If

        If DescripcionTextBox.Text.Trim() = "" OrElse PrecioTextBox.Text.Trim() = "" Then
            MessageBox.Show("Por favor complete todos los campos obligatorios.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            DescripcionTextBox.Focus()
            Exit Sub
        End If

        Dim precio As Decimal
        If Not Decimal.TryParse(PrecioTextBox.Text, precio) OrElse precio <= 0 Then
            MessageBox.Show("Debe ingresar un precio válido mayor a 0.", "Precio inválido", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            PrecioTextBox.Focus()
            Exit Sub
        End If

        Try

            Dim nuevaFila As DataRow = FerreteriaDataSet.Stock.NewRow()
            nuevaFila("descripcion") = DescripcionTextBox.Text.Trim()
            nuevaFila("precio") = precio

            FerreteriaDataSet.Stock.Rows.Add(nuevaFila)

            StockTableAdapter.Update(FerreteriaDataSet.Stock)

            StockTableAdapter.Fill(FerreteriaDataSet.Stock)
            Form1.StockTableAdapter.Fill(Form1.FerreteriaDataSet.Stock)

            StockBindingSource.MoveLast()

            MessageBox.Show("Artículo agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

            StockDataGridView.Enabled = True

            DescripcionTextBox.Clear()
            PrecioTextBox.Clear()
            DescripcionTextBox.Focus()

        Catch ex As Exception
            MessageBox.Show("Error al agregar el artículo: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim CodConsulta As String = InputBox("Por favor, ingrese el código del artículo que desea editar:", "Editar Artículo")

        If CodConsulta = "" Then

            If DescripcionTextBox.CanFocus Then
                DescripcionTextBox.Focus()
            End If

            MsgBox("Debe ingresar un código de artículo para editar.", vbExclamation, "Error de validación")
            Return
        End If

        Dim fila As Integer = Me.StockBindingSource.Find("id_articulo", CodConsulta)

        If fila = -1 Then
            MsgBox("No se encontró el artículo con el código ingresado.", vbInformation, "Información")
        Else
            Me.StockBindingSource.Position = fila
            HabilitarControlesEdicion(True)
            MsgBox("Puede editar los datos del artículo. Realice las modificaciones necesarias y presione 'Guardar' para confirmarlas.", vbInformation, "Información")
        End If
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        Dim CodConsulta As String = InputBox("Ingrese el código del artículo:", "Ingreso de código")
        Dim codArticulo As Integer
        Dim fila As Integer
        Dim aux As Integer

        If Not Integer.TryParse(CodConsulta, codArticulo) Then
            MsgBox("Debe ingresar un código numérico válido.", vbExclamation, "Entrada inválida")
            Return
        End If

        fila = Me.StockBindingSource.Find("id_articulo", codArticulo)

        If fila = -1 Then
            MsgBox("No se encontró el artículo.", vbInformation, "Información")
        Else
            Me.StockBindingSource.Position = fila
            aux = MsgBox("¿Está seguro de que desea eliminar el artículo " & codArticulo & "?", 32 + 1, "Eliminar")
            If aux = 1 Then
                Me.StockBindingSource.RemoveCurrent()
                Me.StockBindingSource.EndEdit()
                Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
                Me.StockTableAdapter.Fill(Me.FerreteriaDataSet.Stock)

                MsgBox("El artículo ha sido eliminado con éxito.", vbInformation, "Eliminación exitosa")
                limpiar(Me)
            End If
        End If
    End Sub
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If Not ValidarCampos() Then Return

        Dim precio As Decimal
        If Not Decimal.TryParse(PrecioTextBox.Text, precio) OrElse precio <= 0 Then
            MsgBox("Debe ingresar un precio válido mayor a 0.")
            Return
        End If

        Dim aux As Integer = MsgBox("¿Está seguro de que desea guardar los cambios?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Guardar")

        If aux = MsgBoxResult.Yes Then
            Try
                Me.StockBindingSource.EndEdit()
                Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
                Me.StockTableAdapter.Fill(Me.FerreteriaDataSet.Stock)
                Form1.StockTableAdapter.Fill(Form1.FerreteriaDataSet.Stock)

                HabilitarControlesEdicion(False)

                If ComboBoxBusqueda.SelectedValue Is Nothing Then
                    MsgBox("Nuevo artículo guardado correctamente.")
                Else
                    MsgBox("Artículo actualizado correctamente.", vbInformation, "Información")
                End If

            Catch ex As Exception
                MsgBox("Ocurrió un error al intentar guardar los cambios.")
            End Try
        End If
    End Sub
    Private Sub ComboBoxBusqueda_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ComboBoxBusqueda.SelectedIndexChanged
         If Not comboBoxCargado Then Exit Sub
        If ComboBoxBusqueda.SelectedValue Is Nothing Then Exit Sub

        Try
            Dim idSeleccionado As Integer = CInt(DirectCast(ComboBoxBusqueda.SelectedItem, DataRowView)("id_articulo"))
            Dim fila As Integer = Me.StockBindingSource.Find("id_articulo", idSeleccionado)

            If fila <> -1 Then
                buscandoArticulo = True
                Me.StockBindingSource.Position = fila
                HabilitarControlesEdicion(True)
            End If
        Catch ex As Exception
            MsgBox("Error al seleccionar el artículo. Verifique el ComboBox.")
        End Try
    End Sub
    Private Sub ButtonLimpiar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLimpiar.Click

        DescripcionTextBox.Clear()
        PrecioTextBox.Clear()
        CantidadTextBox.Clear()
        Id_proveedorTextBox.Clear()

        StockBindingSource.AddNew()

        DescripcionTextBox.Focus()
    End Sub
End Class

