using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Quiz.Models;

namespace Quiz.Features.Usuarios;

public partial class UsuarioViewModel : ObservableObject
{
    private readonly AppDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Usuario> _usuarios = new();

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _apodo = string.Empty;

    [ObservableProperty]
    private Usuario? _usuarioSeleccionado;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private string _mensajeExito = string.Empty;

    public UsuarioViewModel(AppDbContext context)
    {
       
        _context = context;
        _ = CargarUsuariosAsync();
        
    }

    /* private async Task CargarUsuariosAsync()
     {
         try
         {
             var lista = await _context.Usuarios.ToListAsync();
             Usuarios = new ObservableCollection<Usuario>(lista);
         }
         catch (Exception ex)
         {
             MensajeError = $"Error al cargar usuarios: {ex.Message}";
             _ = LimpiarMensajeAsync(() => MensajeError = "");
         }
     }*/

  

    /*private async Task CargarUsuariosAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== CargarUsuariosAsync INICIADO ===");

            var lista = await _context.Usuarios.ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Se encontraron {lista.Count} usuarios en BD");

            foreach (var u in lista)
            {
                System.Diagnostics.Debug.WriteLine($"Usuario: {u.Id} - {u.Nombre} ({u.Apodo})");
            }

            Usuarios = new ObservableCollection<Usuario>(lista);

            System.Diagnostics.Debug.WriteLine($"Usuarios en ObservableCollection: {Usuarios.Count}");
            System.Diagnostics.Debug.WriteLine("=== CargarUsuariosAsync FINALIZADO ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR EN CARGA: {ex.Message}");
            MensajeError = $"Error al cargar usuarios: {ex.Message}";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
        }
    }*/
    private async Task CargarUsuariosAsync()
    {
        try
        {
            // Ejecutar en el hilo UI
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var lista = await _context.Usuarios.OrderBy(u => u.Id).ToListAsync();

                Usuarios.Clear();
                foreach (var usuario in lista)
                {
                    Usuarios.Add(usuario);
                }
            });
        }
        catch (Exception ex)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                MensajeError = $"Error al cargar: {ex.Message}";
            });
        }
    }

    [RelayCommand]
    private async Task AgregarAsync()
    {
        MensajeError = "";
        MensajeExito = "";

        // Validaciones
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MensajeError = "Debe ingresar un nombre.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        if (string.IsNullOrWhiteSpace(Apodo))
        {
            MensajeError = "Debe ingresar un apodo.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        // Validar si ya existe un usuario con el mismo apodo
        bool existe = await _context.Usuarios.AnyAsync(u => u.Apodo.ToLower() == Apodo.ToLower());

        if (existe)
        {
            MensajeError = "Ya existe un usuario con ese apodo.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        try
        {
            var usuario = new Usuario
            {
                Nombre = Nombre.Trim(),
                Apodo = Apodo.Trim()
                // FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            Usuarios.Add(usuario);

            // Limpiar formulario
            Nombre = string.Empty;
            Apodo = string.Empty;

            MensajeExito = "Usuario guardado correctamente.";
            _ = LimpiarMensajeAsync(() => MensajeExito = "");
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al guardar: {ex.Message}";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
        }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        MensajeError = "";
        MensajeExito = "";

        if (UsuarioSeleccionado is null)
        {
            MensajeError = "Seleccione un usuario para eliminar.";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
            return;
        }

        try
        {
            _context.Usuarios.Remove(UsuarioSeleccionado);
            await _context.SaveChangesAsync();
            Usuarios.Remove(UsuarioSeleccionado);
            UsuarioSeleccionado = null;

            MensajeExito = "Usuario eliminado correctamente.";
            _ = LimpiarMensajeAsync(() => MensajeExito = "");
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al eliminar: {ex.Message}";
            _ = LimpiarMensajeAsync(() => MensajeError = "");
        }
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        Nombre = string.Empty;
        Apodo = string.Empty;
        MensajeError = "";
        MensajeExito = "";
    }
    [RelayCommand]
    private async Task ProbarCargaAsync()
    {
        try
        {
            var count = await _context.Usuarios.CountAsync();
            MensajeError = $"Hay {count} usuarios en la BD";

            // También mostrar en un MessageBox
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var message = $"Total usuarios en BD: {count}\n\n";
                var usuarios = await _context.Usuarios.ToListAsync();
                foreach (var u in usuarios)
                {
                    message += $"- {u.Nombre} ({u.Apodo})\n";
                }
                MensajeExito = message;
            });
        }
        catch (Exception ex)
        {
            MensajeError = $"Error: {ex.Message}";
        }
    }

    private async Task LimpiarMensajeAsync(Action limpiarAccion)
    {
        await Task.Delay(3000);
        limpiarAccion();
    }
}