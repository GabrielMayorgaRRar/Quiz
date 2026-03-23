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

    private async Task CargarUsuariosAsync()
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

    private async Task LimpiarMensajeAsync(Action limpiarAccion)
    {
        await Task.Delay(3000);
        limpiarAccion();
    }
}