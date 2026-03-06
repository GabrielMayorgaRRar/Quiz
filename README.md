# Quiz-

# Postgresql-E1

Aplicación de escritorio construida con **Avalonia UI**, **Entity Framework Core** y **PostgreSQL**, usando el patrón **MVVM** con CommunityToolkit.

## Stack tecnológico

| Tecnología                                                                         | Uso                                                                    |
| ---------------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| [Avalonia UI](https://avaloniaui.net/)                                             | Framework de UI multiplataforma                                        |
| [Entity Framework Core](https://learn.microsoft.com/ef/core/)                      | ORM para acceso a datos                                                |
| [Npgsql](https://www.npgsql.org/)                                                  | Proveedor PostgreSQL para EF Core                                      |
| [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) | Source generators para MVVM (`[ObservableProperty]`, `[RelayCommand]`) |
| .NET 9                                                                             | Runtime                                                                |

## Configuración de la base de datos

Las credenciales se leen desde variables de entorno con valores por defecto:

```bash
DB_HOST=localhost
DB_PORT=5432
DB_NAME=net_psql
DB_USER=user_net
DB_PASS=tu_password
```

Para crear y aplicar las migraciones:

```bash
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

---

## Arquitectura por features (para equipos)

El proyecto se organiza en **features verticales**: cada entidad tiene su propia carpeta con vista y ViewModel. Esto permite que cada integrante del equipo trabaje de forma independiente sin conflictos.

### Estructura recomendada

```
📁 Features/
├── 📁 Usuarios/
│   ├── UsuarioView.axaml
│   ├── UsuarioView.axaml.cs
│   └── UsuarioViewModel.cs
│
├── 📁 Preguntas/
│   ├── PreguntaView.axaml
│   ├── PreguntaView.axaml.cs
│   └── PreguntaViewModel.cs
│
├── 📁 Categorias/
│   ├── CategoriaView.axaml
│   ├── CategoriaView.axaml.cs
│   └── CategoriaViewModel.cs
│
📁 Models/                  ← Entidades compartidas (DbContext + clases)
📁 Views/
│   └── MainWindow.axaml    ← Solo importa las vistas de cada feature
```

### Integración en MainWindow

```xml
<TabControl>
    <TabItem Header="Usuarios">
        <features:UsuarioView />
    </TabItem>
    <TabItem Header="Preguntas">
        <features:PreguntaView />
    </TabItem>
    <TabItem Header="Categorías">
        <features:CategoriaView />
    </TabItem>
</TabControl>
```

### División de trabajo

| Dev   | Feature    | Archivos                                |
| ----- | ---------- | --------------------------------------- |
| Dev 1 | Usuarios   | `Features/Usuarios/*`                   |
| Dev 2 | Preguntas  | `Features/Preguntas/*`                  |
| Dev 3 | Categorías | `Features/Categorias/*`                 |
| Dev 4 | Core       | `MainWindow`, `AppDbContext`, `Models/` |

> **Nota:** El único archivo compartido es `AppDbContext.cs`. Cada dev agrega su `DbSet<>` y corre su migración. Coordinen este paso para evitar conflictos.

### Ventajas

- Sin conflictos en git — cada uno trabaja en su propia carpeta
- Features autocontenidas e independientes
- Fácil integración final en `MainWindow`
