# Esquema de Base de Datos - Proyecto Quiz

A continuación se detalla el análisis de las entidades (modelos) extraídas de la carpeta `Models`. Se especifican las tablas, sus atributos principales, llaves primarias (PK), llaves foráneas (FK) y sus relaciones.

---

## 1. Tabla `Usuario`
Representa a los jugadores o usuarios registrados en la aplicación.

- **Atributos:**
  - `Id` (int)
  - `Nombre` (string)
  - `Apodo` (string)
  - `FechaRegistro` (DateTime)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** Ninguna explícita en su tabla.
- **Relaciones:**
  - **Uno a Muchos (1:N)** con la tabla `Partida` (Un usuario puede jugar varias partidas).

---

## 2. Tabla `Categoria`
Agrupa las preguntas y los juegos por temáticas.

- **Atributos:**
  - `Id` (int)
  - `Nombre` (string)
  - `Descripcion` (string)
  - `Imagen` (string)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** Ninguna explícita.
- **Relaciones:**
  - **Uno a Muchos (1:N)** con la tabla `Juego` (Una categoría puede tener varios juegos creados).
  - **Uno a Muchos (1:N)** con la tabla `Pregunta` (Una categoría abarca múltiples preguntas).

---

## 3. Tabla `Juego`
Representa una sesión o pin de juego específico de una categoría.

- **Atributos:**
  - `Id` (int)
  - `Clave` (string)
  - `CategoriaId` (int)
  - `nombre` (string)
  - `FechaCreacion` (DateTime)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** 
  - `CategoriaId` referencia a `Categoria.Id`
- **Relaciones:**
  - **Muchos a Uno (N:1)** con la tabla `Categoria` (Varios juegos pertenecen a una categoría).
  - **Uno a Muchos (1:N)** con la tabla `Partida` (Un juego puede ser jugado múltiples veces originando varias partidas).

---

## 4. Tabla `Pregunta`
Define las preguntas individuales por categoría.

- **Atributos:**
  - `Id` (int)
  - `Enunciado` (string)
  - `CategoriaId` (int)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** 
  - `CategoriaId` referencia a `Categoria.Id`
- **Relaciones:**
  - **Muchos a Uno (N:1)** con la tabla `Categoria` (La pregunta pertenece a una categoría).
  - **Uno a Muchos (1:N)** con la tabla `Opciones` (Una pregunta tiene múltiples opciones de respuesta).

---

## 5. Tabla `Opciones`
Opciones de respuesta correspondientes a una pregunta. Admite respuestas en texto, imagen o audio.

- **Atributos:**
  - `Id` (int)
  - `contenido` (string)
  - `esCorrecta` (bool)
  - `_tipoRespuesta` (TipoRespuesta Enum: Texto, Imagen, Audio)
  - `PreguntaId` (int)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** 
  - `PreguntaId` referencia a `Pregunta.Id`
- **Relaciones:**
  - **Muchos a Uno (N:1)** con la tabla `Pregunta` (Varias opciones corresponden a una misma pregunta).

---

## 6. Tabla `Partida`
Registra el juego de un usuario, sus puntos obtenidos y cuándo se realizó.

- **Atributos:**
  - `Id` (int)
  - `JuegoId` (int)
  - `UsuarioId` (int)
  - `Puntos` (int)
  - `Fecha` (DateTime)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** 
  - `JuegoId` referencia a `Juego.Id`
  - `UsuarioId` referencia a `Usuario.Id`
- **Relaciones:**
  - **Muchos a Uno (N:1)** con la tabla `Juego` (Varias partidas de diferentes usuarios o momentos apuntan al mismo juego).
  - **Muchos a Uno (N:1)** con la tabla `Usuario` (El puntaje y sesión de juego le pertenece a un usuario).
  - **Uno a Muchos (1:N)** con la tabla `DetallePartida` (Una partida genera un historial de opciones elegidas).

---

## 7. Tabla `DetallePartida`
Lleva el historial detallado de las respuestas que elige el usuario por cada pregunta en su partida.

- **Atributos:**
  - `Id` (int)
  - `PartidaId` (int)
  - `PreguntaId` (int)
  - `OpcionElegida` (int)
  - `EsCorrecta` (bool)
- **Primary Key (PK):** `Id`
- **Llaves Foráneas (FK):** 
  - `PartidaId` referencia a `Partida.Id`
  - `PreguntaId` referencia a `Pregunta.Id`
- **Relaciones:**
  - **Muchos a Uno (N:1)** con la tabla `Partida` (Pertenece a la partida en la que se generó la respuesta escalada).
  - **Muchos a Uno (N:1)** con la tabla `Pregunta` (Apunta a la pregunta específica que se estaba contestando).
