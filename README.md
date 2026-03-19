# Quiz-

#### Requisitos previos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/9.0) o [.NET 8.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)
- Windows 10/11 (64-bit)

#### Pasos de compilación

```bash
# 1. Clonar el repositorio
git clone https://github.com/GabrielMayorgaRRar/Quiz.git
cd Quiz

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar en modo desarrollo
dotnet run

# 4. O publicar para producción
dotnet publish -c Release -r win-x64 --self-contained true -o publish-win
```

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

## Configuración de Entity Framework Core

### 1. Instalar la herramienta global de EF Core

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
```

### 2. Agregar los paquetes NuGet al proyecto

```bash
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.4
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.4
```

### 3. Crear las tablas inicialmente

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Agregar una nueva tabla

```bash
dotnet ef migrations add AddVendedor
dotnet ef database update
```

### 5. Modificar una tabla existente

```bash
dotnet ef migrations add AgregarNuevoAtributo
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

---

## Flujo de trabajo con ramas (Git)

### Escenario A: Creando una nueva rama (inicio)

> No necesitas merge, solo el `pull` para preparar `main`.

```bash
git switch main
git pull origin main        # ¡Clave! Trae los cambios más recientes
git switch -c feature/nueva-tarea
git push -u origin feature/nueva-tarea
```

---

### Escenario B: Trayendo cambios posteriores a tu rama

> Usas `merge` cuando se hayan hecho cambios en la rama `main` después de que creaste tu rama.

```bash
git switch main
git pull origin main        # Trae los cambios nuevos
git switch feature/tu-rama
git merge main              # Aplica esos cambios nuevos a tu rama de trabajo
```

---

### Guardar cambios temporalmente (stash)

Si tienes cambios sin terminar y necesitas moverte a otra rama:

```bash
# Guardar cambios
git stash push -m "Mensaje descriptivo para estos cambios"

# Recuperar los cambios guardados
git stash pop
```

---

### Subir cambios desde tu rama

```bash
git add .           # Puedes dejar el punto para subir todo, o especificar un archivo
git commit -m "mensaje"
git push
```
