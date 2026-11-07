using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SistemaSencillo
{

    public class ExcepcionValidacion : Exception
    {
        public ExcepcionValidacion(string mensaje) : base(mensaje) { }
    }

    // Modelo POO
    public class Transaccion
    {
        public int Id { get; set; }
        public string Cuenta { get; set; }
        public decimal Monto { get; set; }

        public Transaccion(int id, string cuenta, decimal monto)
        {
            Id = id;
            Cuenta = cuenta;
            Monto = monto;
        }
    }

    // Estructura de Datos Avanzada 
    public class NodoArbol
    {
        public Transaccion Dato;
        public NodoArbol Izquierdo;
        public NodoArbol Derecho;

        public NodoArbol(Transaccion dato) { Dato = dato; }
    }

    public class ArbolTransacciones
    {
        public NodoArbol Raiz;
        public int Cantidad;

        public void Insertar(Transaccion nueva)
        {
            Raiz = InsertarRec(Raiz, nueva);
            Cantidad++;
        }

        private NodoArbol InsertarRec(NodoArbol nodo, Transaccion nueva)
        {
            if (nodo == null) return new NodoArbol(nueva);

            if (nueva.Id < nodo.Dato.Id)
                nodo.Izquierdo = InsertarRec(nodo.Izquierdo, nueva);
            else if (nueva.Id > nodo.Dato.Id)
                nodo.Derecho = InsertarRec(nodo.Derecho, nueva);
            else
                throw new ExcepcionValidacion($"ID duplicado en el árbol: {nueva.Id}");

            return nodo;
        }

        public void ObtenerListaOrdenada(NodoArbol nodo, List<Transaccion> lista)
        {
            if (nodo != null)
            {
                ObtenerListaOrdenada(nodo.Izquierdo, lista);
                lista.Add(nodo.Dato);
                ObtenerListaOrdenada(nodo.Derecho, lista);
            }
        }
    }

    // Interfaz
    public class FormPrincipal : Form
    {
        private ArbolTransacciones arbol = new ArbolTransacciones();
        private TabControl pestañas;
        private TextBox txtEntrada;
        private DataGridView tablaDatos;
        private ListBox lstErrores;
        private TextBox txtAuditoria;

        public FormPrincipal()
        {
            this.Text = "Sistema AndreDorian - Estructura & Seguridad";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            pestañas = new TabControl { Dock = DockStyle.Fill };

            // Pestaña 1
            TabPage tab1 = new TabPage("1. Entrada Legada");
            txtEntrada = new TextBox { Multiline = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 10F) };
            txtEntrada.Text = "101,ACC-100,150.50\r\n103,ACC-200,300.00\r\n102,ACC-300,invalid_monto\r\n101,ACC-100,50.00\r\n104,ACC-400,500.00";

            Button btnProcesar = new Button { Text = "Procesar y Validar Datos", Dock = DockStyle.Bottom, Height = 40, BackColor = Color.LightSkyBlue };
            btnProcesar.Click += ProcesarDatos;

            tab1.Controls.Add(txtEntrada);
            tab1.Controls.Add(btnProcesar);

            // Pestaña 2
            TabPage tab2 = new TabPage("2. Resultados Árbol");
            TableLayoutPanel panelTab2 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            panelTab2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panelTab2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            tablaDatos = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            lstErrores = new ListBox { Dock = DockStyle.Fill, ForeColor = Color.Red };

            panelTab2.Controls.Add(tablaDatos, 0, 0);
            panelTab2.Controls.Add(lstErrores, 1, 0);
            tab2.Controls.Add(panelTab2);

            // Pestaña 3
            TabPage tab3 = new TabPage("3. Auditoría SHA-256");
            txtAuditoria = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, Font = new Font("Consolas", 9.5F) };
            Button btnGuardar = new Button { Text = "Generar Hash y Persistir", Dock = DockStyle.Bottom, Height = 40 };
            btnGuardar.Click += GenerarAuditoria;

            tab3.Controls.Add(txtAuditoria);
            tab3.Controls.Add(btnGuardar);

            pestañas.TabPages.Add(tab1);
            pestañas.TabPages.Add(tab2);
            pestañas.TabPages.Add(tab3);

            this.Controls.Add(pestañas);
        }

        private void ProcesarDatos(object sender, EventArgs e)
        {
            arbol = new ArbolTransacciones();
            lstErrores.Items.Clear();

            string[] lineas = txtEntrada.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string linea in lineas)
            {
                string[] partes = linea.Split(',');
                if (partes.Length != 3)
                {
                    lstErrores.Items.Add($"Error de formato: '{linea}'");
                    continue;
                }

                try
                {
                    if (!int.TryParse(partes[0].Trim(), out int id) || id <= 0)
                        throw new ExcepcionValidacion($"ID inválido: '{partes[0]}'");

                    string cuenta = partes[1].Trim();
                    if (string.IsNullOrEmpty(cuenta))
                        throw new ExcepcionValidacion("Cuenta vacía");

                    if (!decimal.TryParse(partes[2].Trim(), out decimal monto) || monto < 0)
                        throw new ExcepcionValidacion($"Monto inválido: '{partes[2]}'");

                    arbol.Insertar(new Transaccion(id, cuenta, monto));
                }
                catch (ExcepcionValidacion ex)
                {
                    lstErrores.Items.Add(ex.Message);
                }
            }

            List<Transaccion> listaOrdenada = new List<Transaccion>();
            arbol.ObtenerListaOrdenada(arbol.Raiz, listaOrdenada);
            tablaDatos.DataSource = null;
            tablaDatos.DataSource = listaOrdenada;

            pestañas.SelectedIndex = 1;
        }

        private void GenerarAuditoria(object sender, EventArgs e)
        {
            if (arbol.Cantidad == 0)
            {
                MessageBox.Show("Primero debes procesar datos válidos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Transaccion> lista = new List<Transaccion>();
            arbol.ObtenerListaOrdenada(arbol.Raiz, lista);

            StringBuilder texto = new StringBuilder();
            texto.AppendLine("--- INFORME DE AUDITORÍA ---");
            foreach (var t in lista)
            {
                texto.AppendLine($"ID: {t.Id} | Cuenta: {t.Cuenta} | Monto: ${t.Monto}");
            }

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto.ToString()));
                StringBuilder hashHex = new StringBuilder();
                foreach (byte b in hashBytes) hashHex.Append(b.ToString("x2"));

                txtAuditoria.Text = texto.ToString() + "\r\nFIRMA DIGITAL SHA-256:\r\n" + hashHex.ToString();
            }

            MessageBox.Show("Informe firmado correctamente con SHA-256.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}