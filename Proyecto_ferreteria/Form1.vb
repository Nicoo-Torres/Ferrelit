Public Class Form1

    Private Sub EmailTextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles EmailTextBox.Validating

        If ProveedoresBindingSource.Current IsNot Nothing Then

            Me.Validate()
            Me.ProveedoresBindingSource.EndEdit()
            Me.ProveedoresTableAdapter.Update(Me.FerreteriaDataSet.Proveedores)


            MsgBox("Los datos del proveedor se han editado con éxito.", vbInformation, "Éxito")
        End If
    End Sub

    Private Function PedirCodigoCliente(ByVal titulo As String, ByVal mensaje As String) As String
        Return InputBox(mensaje, titulo)
    End Function
    Private Sub MostrarMensaje(ByVal mensaje As String, Optional ByVal titulo As String = "Información", Optional ByVal tipo As MsgBoxStyle = vbInformation)
        MsgBox(mensaje, tipo, titulo)
    End Sub
    Private Sub LimpiarCampos()

        NombreTextBox.Clear()
        DireccionTextBox.Clear()
        AlturaTextBox.Clear()
        TelefonoTextBox.Clear()
        EmailTextBox.Clear()

    End Sub
    Private Function CamposVentaValidos() As Boolean
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MsgBox("Debe ingresar un código de artículo.", vbExclamation, "Campo obligatorio")
            TextBox2.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TextBox3.Text) Then
            MsgBox("Debe ingresar una cantidad a vender.", vbExclamation, "Campo obligatorio")
            TextBox3.Focus()
            Return False
        End If

        If Not IsNumeric(TextBox2.Text) Or Not IsNumeric(TextBox3.Text) Then
            MsgBox("Los campos deben ser numéricos.", vbExclamation, "Error de formato")
            Return False
        End If

        If Val(TextBox3.Text) <= 0 Then
            MsgBox("La cantidad debe ser mayor a cero.", vbExclamation, "Cantidad inválida")
            TextBox3.Focus()
            Return False
        End If

        Return True
    End Function
    Private Sub StockBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Validate()
        Me.StockBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.FerreteriaDataSet)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'FerreteriaDataSet.Ventas' table. You can move, or remove it, as needed.
        Me.VentasTableAdapter.Fill(Me.FerreteriaDataSet.Ventas)
        'TODO: This line of code loads data into the 'FerreteriaDataSet.Proveedores' table. You can move, or remove it, as needed.
        Me.ProveedoresTableAdapter.Fill(Me.FerreteriaDataSet.Proveedores)
        'TODO: This line of code loads data into the 'FerreteriaDataSet.Clientes' table. You can move, or remove it, as needed.
        Me.ClientesTableAdapter.Fill(Me.FerreteriaDataSet.Clientes)
        'TODO: This line of code loads data into the 'FerreteriaDataSet.Stock' table. You can move, or remove it, as needed.
        Me.StockTableAdapter.Fill(Me.FerreteriaDataSet.Stock)

        ProveedoresDataGridView.DataSource = ProveedoresBindingSource

        MostrarVentasDelDia()
        MostrarProductosBajosEnStock()

        ProveedoresBindingSource.Position = -1
        LimpiarCampos()
        NombreTextBox.Focus()

        For Each col As DataGridViewColumn In ProveedoresDataGridView.Columns
            Debug.WriteLine("Columna: " & col.Name)
        Next

        Me.BeginInvoke(New MethodInvoker(Sub()
                                             TabControl1.SelectedTab = TabPage1
                                             NombreTextBox.Focus()
                                             NombreTextBox.SelectAll()
                                         End Sub))

        With ProveedoresDataGridView
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .EditMode = DataGridViewEditMode.EditProgrammatically

        End With

        Try
            Dim frases() As String = IO.File.ReadAllLines("Archives\Frases.txt")
            Dim rand As New Random()
            Dim index As Integer = rand.Next(frases.Length)
            lblFrases.Text = frases(index)
        Catch ex As Exception
            lblFrases.Text = "No se pudo cargar la frase."
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        altasProducto.Show()
        altasProducto.StockBindingSource.AddNew()
        altasProducto.StockBindingSource.MoveLast()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        altasClientes.Show()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Desarrollo.Show()
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        Dim vista As New DataView
        vista.Table = Me.FerreteriaDataSet.Stock
        vista.RowFilter = "id_articulo = " & Val(TextBox2.Text)
        Me.StockDataGridView1.DataSource = vista

    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

        If Not CamposVentaValidos() Then Exit Sub

        Dim CodConsulta As Integer = Val(TextBox2.Text)
        Dim fila As Integer = Me.StockBindingSource.Find("id_articulo", CodConsulta)

        If fila = -1 Then
            MsgBox("No se encontró el artículo.", vbExclamation, "Error de validación")
            Exit Sub
        End If

        Me.StockBindingSource.Position = fila

        Dim aux As Integer = MsgBox("¿Desea vender el producto con código: " & CodConsulta & "?", vbQuestion + vbOKCancel, "Confirmar venta")
        If aux <> vbOK Then Exit Sub

        Dim cantidadVendida As Integer = Val(TextBox3.Text)
        Dim precioUnitario As Decimal = CDec(Me.StockBindingSource.Current("precio"))
        Dim stockDisponible As Integer = CInt(Me.StockBindingSource.Current("cantidad"))

        If stockDisponible >= cantidadVendida Then
            Dim totalVenta As Decimal = cantidadVendida * precioUnitario

            Me.VentasBindingSource.AddNew()
            Me.VentasBindingSource.Current("id_articulo") = Me.StockBindingSource.Current("id_articulo")
            Me.VentasBindingSource.Current("cant_compra") = cantidadVendida
            Me.VentasBindingSource.Current("tot_facturado") = totalVenta
            Me.VentasBindingSource.Current("fecha") = Date.Now.Date
            Me.VentasBindingSource.Current("precio") = precioUnitario
            Me.VentasBindingSource.EndEdit()

            Me.StockBindingSource.Current("cantidad") = stockDisponible - cantidadVendida
            Me.StockBindingSource.EndEdit()

            Me.VentasTableAdapter.Update(Me.FerreteriaDataSet.Ventas)
            Me.StockTableAdapter.Update(Me.FerreteriaDataSet.Stock)

            Me.StockTableAdapter.Fill(Me.FerreteriaDataSet.Stock)
            Me.VentasTableAdapter.Fill(Me.FerreteriaDataSet.Ventas)

            MsgBox("Importe total: " & totalVenta.ToString("C"), vbInformation, "Venta exitosa")

            TextBox2.Clear()
            TextBox3.Clear()
            TextBox2.Focus()

            MostrarVentasDelDia()
            MostrarProductosBajosEnStock()
        Else
            MsgBox("No hay stock suficiente para realizar la venta." & vbCrLf & "Por favor, verifique la cantidad disponible en el inventario.", vbExclamation + vbOKOnly, "Error de stock insuficiente")
            TextBox2.Clear()
            TextBox3.Clear()
            TextBox2.Focus()
        End If

    End Sub
    Private Sub TabPage6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Me.ProveedoresTableAdapter.Fill(Me.FerreteriaDataSet.Proveedores)

        With Me.FerreteriaDataSet.Tables("Proveedores").Columns("id_proveedor")
            .AutoIncrement = True
            .AutoIncrementSeed = 1
            .AutoIncrementStep = 1
        End With
    End Sub
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
    Private Sub MostrarVentasDelDia()
        Try

            Dim vistaVentas As New DataView(Me.FerreteriaDataSet.Ventas)
            Dim hoy As Date = Date.Today

            vistaVentas.RowFilter = "fecha = #" & hoy.ToString("MM/dd/yyyy") & "#"
            vistaVentas.Sort = "fecha DESC, id_venta DESC" ' Orden descendente por fecha e id

            VentasDataGridView.DataSource = Nothing
            VentasDataGridView.DataSource = vistaVentas

            Dim cantidadVentas As Integer = vistaVentas.Count
            Dim totalFacturado As Decimal = 0

            For Each fila As DataRowView In vistaVentas
                totalFacturado += Convert.ToDecimal(fila("tot_facturado"))
            Next

            lblVentasDelDia.Text = "$" & totalFacturado.ToString("N2")
            lblCantidadDeVentas.Text = cantidadVentas.ToString()

            VentasDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Catch ex As Exception
            MsgBox("Error al mostrar ventas del día: " & ex.Message)
        End Try
    End Sub
    Private Sub MostrarProductosBajosEnStock()
        Try

            Dim vistaBajoStock As New DataView(Me.FerreteriaDataSet.Stock)
            vistaBajoStock.RowFilter = "cantidad < 10"

            dgvBajoStock.AutoGenerateColumns = True

            dgvBajoStock.DataSource = Nothing ' Esto limpia cualquier configuración previa
            dgvBajoStock.DataSource = vistaBajoStock

            dgvBajoStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            dgvBajoStock.DefaultCellStyle.BackColor = Color.White
            dgvBajoStock.DefaultCellStyle.ForeColor = Color.Black
            dgvBajoStock.Refresh()

            If vistaBajoStock.Count = 0 Then
                MsgBox("No hay productos bajos en stock.")
            End If

        Catch ex As Exception
            MsgBox("Error al cargar productos bajos en stock: " & ex.Message)
        End Try
    End Sub
    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TabControl1.SelectedIndexChanged

        If TabControl1.SelectedTab.Name = "TabPage2" Then

            TextBox2.Focus()
        End If
    End Sub
    Private Sub btnAgregar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Me.Validate()
            ProveedoresBindingSource.EndEdit()
            ProveedoresTableAdapter.Update(FerreteriaDataSet.Proveedores)
            MsgBox("Datos guardados con éxito")

            ProveedoresTableAdapter.Fill(FerreteriaDataSet.Proveedores)
        Catch ex As Exception
            MsgBox("Error al guardar: " & ex.Message)
        End Try
    End Sub
    Private Sub btnEliminar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If MessageBox.Show("¿Seguro que querés eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                ProveedoresBindingSource.RemoveCurrent()
                ProveedoresTableAdapter.Update(FerreteriaDataSet.Proveedores)
                MsgBox("Proveedor eliminado")
            Catch ex As Exception
                MsgBox("Error al eliminar: " & ex.Message)
            End Try
        End If
    End Sub
    Private Sub btnBuscar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim nombreBuscado As String = InputBox("Ingrese el nombre del proveedor a buscar")

        Dim encontrados = FerreteriaDataSet.Proveedores.Select("nombre LIKE '%" & nombreBuscado & "%'")
        If encontrados.Length > 0 Then
            Dim indice = ProveedoresBindingSource.Find("id_proveedor", encontrados(0)("id_proveedor"))
            If indice <> -1 Then
                ProveedoresBindingSource.Position = indice
            End If
        Else
            MsgBox("Proveedor no encontrado")
        End If
    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

    End Sub

    Private Sub btnAgregar_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAgregar.Click

        If NombreTextBox.Text.Trim() <> "" AndAlso
           DireccionTextBox.Text.Trim() <> "" AndAlso
           AlturaTextBox.Text.Trim() <> "" AndAlso
           TelefonoTextBox.Text.Trim() <> "" AndAlso
           EmailTextBox.Text.Trim() <> "" Then

            MessageBox.Show("Por favor, presione el botón 'Limpiar' para borrar los campos antes de agregar un nuevo proveedor.",
                            "Campos llenos", MessageBoxButtons.OK, MessageBoxIcon.Information)

            NombreTextBox.Focus()

            Exit Sub
        End If

        If NombreTextBox.Text.Trim() = "" Or
           DireccionTextBox.Text.Trim() = "" Or
           AlturaTextBox.Text.Trim() = "" Or
           TelefonoTextBox.Text.Trim() = "" Or
           EmailTextBox.Text.Trim() = "" Then

            MessageBox.Show("Por favor complete todos los campos obligatorios.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            NombreTextBox.Focus()
            Exit Sub
        End If

        Dim nuevaFila As DataRow = FerreteriaDataSet.Proveedores.NewRow()
        nuevaFila("Nombre") = NombreTextBox.Text.Trim()
        nuevaFila("Direccion") = DireccionTextBox.Text.Trim()
        nuevaFila("Altura") = AlturaTextBox.Text.Trim()
        nuevaFila("Telefono") = TelefonoTextBox.Text.Trim()
        nuevaFila("Email") = EmailTextBox.Text.Trim()

        FerreteriaDataSet.Proveedores.Rows.Add(nuevaFila)

        ProveedoresTableAdapter.Update(FerreteriaDataSet.Proveedores)

        MessageBox.Show("Proveedor agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ProveedoresDataGridView.Enabled = True

        LimpiarCampos()

        NombreTextBox.Focus()
    End Sub

    Private Sub btnEliminar_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEliminar.Click

        If ProveedoresDataGridView.SelectedRows.Count > 0 Then

            ProveedoresBindingSource.SuspendBinding()

            Dim confirmacion As DialogResult = MessageBox.Show(
                "¿Estás seguro de que querés eliminar esta fila?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)

            If confirmacion = DialogResult.Yes Then

                Dim filaSeleccionada As DataGridViewRow = ProveedoresDataGridView.SelectedRows(0)
                Dim index As Integer = filaSeleccionada.Index

                ProveedoresBindingSource.RemoveAt(index)
                MessageBox.Show("Fila eliminada con éxito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

                ProveedoresBindingSource.CancelEdit()
                FerreteriaDataSet.Proveedores.RejectChanges()
            End If

            ' Rehabilitamos la vinculación de datos
            ProveedoresBindingSource.ResumeBinding()

        Else
            MessageBox.Show("Seleccioná una fila para eliminar.")
        End If

        LimpiarCampos()
        NombreTextBox.Focus()
    End Sub

    Private Sub btnBuscar_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuscar.Click

        Dim criterio As String = InputBox("Ingrese el código o nombre del proveedor:", "Buscar Proveedor").Trim()

        If criterio = "" Then
            MsgBox("Debe ingresar un valor para buscar.", vbExclamation, "Búsqueda cancelada")
            Return
        End If

        Dim fila As Integer = -1

        If IsNumeric(criterio) Then
            fila = Me.ProveedoresBindingSource.Find("id_proveedor", criterio)
        End If

        If fila = -1 Then
            fila = Me.ProveedoresBindingSource.Find("Nombre", criterio)
        End If

        If fila = -1 Then
            MsgBox("Proveedor no encontrado.", vbInformation, "Resultado de búsqueda")
        Else
            Me.ProveedoresBindingSource.Position = fila
        End If
    End Sub
    Private Sub btnEditar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditar.Click

        Dim CodConsulta As String = InputBox("Por favor, ingrese el código del proveedor que desea editar:", "Editar Proveedor")

        If CodConsulta = "" Then
            MsgBox("Debe ingresar un código de proveedor.", vbExclamation, "Error de validación")
            Return
        End If

        Dim fila As Integer = Me.ProveedoresBindingSource.Find("id_proveedor", CodConsulta)

        If fila = -1 Then
            MsgBox("No se encontró el proveedor con el código ingresado.", vbInformation, "Información")
        Else

            Me.ProveedoresBindingSource.Position = fila
            HabilitarControlesEdicion(True)  ' Llamada a la función para habilitar controles
            MsgBox("Proveedor encontrado. Puede editar los datos y presionar 'Guardar' para confirmarlos.", vbInformation, "Información")

            NombreTextBox.Focus()
        End If
    End Sub
    Private Sub HabilitarControlesEdicion(ByVal estado As Boolean)

        NombreTextBox.Enabled = estado
        DireccionTextBox.Enabled = estado
        AlturaTextBox.Enabled = estado
        TelefonoTextBox.Enabled = estado
        EmailTextBox.Enabled = estado

    End Sub

    Private Sub btnLimpiar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLimpiar.Click

        NombreTextBox.Clear()
        DireccionTextBox.Clear()
        AlturaTextBox.Clear()
        TelefonoTextBox.Clear()
        EmailTextBox.Clear()

        ProveedoresBindingSource.AddNew()

        NombreTextBox.Focus()

        ProveedoresDataGridView.Enabled = False
    End Sub
End Class

