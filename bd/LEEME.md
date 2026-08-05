# Puesta en marcha — Módulos 0 y 1

## 1. Crear la base

Con MySQL instalado y corriendo, ejecutar el script desde la consola:

```bash
mysql -u root -p < bd/tambo_m0_m1.sql
```

O abrirlo en MySQL Workbench y ejecutarlo entero. Crea la base `tambo`, las cinco
tablas del Módulo 1 (`razas`, `categorias`, `animales`, `hembras`, `machos`) y
carga las razas y categorías semilla.

El Módulo 0 no necesita tablas: las credenciales son fijas y se validan en memoria.

## 2. Completar la cadena de conexión

En `Tesis/Persistencia/pConexion.cs`, arriba del todo, están los datos de conexión:

```csharp
private static string servidor = "localhost";
private static string puerto = "3306";
private static string baseDeDatos = "tambo";
private static string usuario = "root";
private static string contrasena = "CAMBIAR_POR_LA_CONTRASENA";
```

Reemplazar `CAMBIAR_POR_LA_CONTRASENA` por la contraseña real y ajustar servidor,
puerto y usuario si hace falta.

## 3. Credenciales del sistema

Están en `Tesis/Dominio/Controladora.cs`, en la región `SEGURIDAD`:

- Usuario: `sofia`
- Contraseña: `tambo2026`

## 4. Correr

```bash
dotnet run --project Tesis/Tesis.csproj
```

Queda en `http://localhost:5174`. El login está en `/PagesSeguridad/Login` y el
listado de animales en `/PagesAnimal/ListaAnimales`.
