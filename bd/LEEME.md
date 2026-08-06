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

Los datos de conexión ya no están en el código: se leen de `Tesis/appsettings.json`.

```json
"ConnectionStrings": {
  "Tambo": "server=localhost; port=3306; database=tambo; uid=root; pwd=; CharSet=utf8mb4;"
}
```

Ajustar servidor, puerto, usuario y contraseña según la instalación de MySQL.

Para no versionar la contraseña real conviene usar user-secrets, que la guarda
fuera del repositorio y pisa el valor de `appsettings.json`:

```bash
dotnet user-secrets init --project Tesis/Tesis.csproj
dotnet user-secrets set "ConnectionStrings:Tambo" "server=localhost; port=3306; database=tambo; uid=root; pwd=LA_CONTRASENA; CharSet=utf8mb4;" --project Tesis/Tesis.csproj
```

## 3. Credenciales del sistema

También salen de `Tesis/appsettings.json`:

```json
"Seguridad": {
  "Usuario": "sofia",
  "Contrasena": "tambo2026"
}
```

Si no se cargan estas dos claves el sistema no habilita el acceso a nadie.
Igual que la cadena de conexión, se pueden mover a user-secrets:

```bash
dotnet user-secrets set "Seguridad:Contrasena" "LA_CONTRASENA" --project Tesis/Tesis.csproj
```

Todo el sitio queda detrás del login: sin sesión iniciada cualquier página
redirige a `/PagesSeguridad/Login`.

## 4. Correr

```bash
dotnet run --project Tesis/Tesis.csproj
```

Queda en `http://localhost:5174`. El login está en `/PagesSeguridad/Login` y el
listado de animales en `/PagesAnimal/ListaAnimales`.
