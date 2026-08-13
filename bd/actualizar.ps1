<#
.SYNOPSIS
    Actualiza la base de datos del Sistema de Gestion de Tambo y, si se
    pide, le carga el juego de datos de prueba.

.DESCRIPTION
    Encadena en un solo comando lo que si no hay que hacer a mano: leer
    la cadena de conexion, encontrar el cliente de MySQL, respaldar,
    correr bd/tambo_actualizacion.sql y -opcionalmente- volver a cargar
    bd/tambo_datos_prueba.sql.

    Los datos de conexion salen de Tesis/appsettings.json, que es de
    donde los lee el sistema: asi no hay dos lugares donde configurar lo
    mismo. Cualquiera de ellos se puede pisar por parametro.

    La contrasena nunca viaja en la linea de comandos -donde quedaria a
    la vista en el historial y en la lista de procesos-: se le pasa a
    mysql por la variable de entorno MYSQL_PWD, que el script borra al
    terminar.

.PARAMETER DatosPrueba
    Ademas de actualizar el esquema, carga el rodeo de prueba. ATENCION:
    ese script vacia las tablas de datos antes de cargar. No se usa sobre
    una base con datos reales.

.PARAMETER SinRespaldo
    Saltea el mysqldump previo. Solo para una base que se puede perder.

.PARAMETER Forzar
    No pide confirmacion para cargar los datos de prueba. Para uso
    desatendido.

.EXAMPLE
    .\bd\actualizar.ps1
    Actualiza el esquema y no toca los datos.

.EXAMPLE
    .\bd\actualizar.ps1 -DatosPrueba
    Actualiza el esquema y deja la base con el rodeo de prueba.

.EXAMPLE
    .\bd\actualizar.ps1 -Servidor 192.168.0.10 -Usuario tambo
    Contra otra instalacion de MySQL.
#>

[CmdletBinding()]
param(
    [string] $Servidor,
    [int]    $Puerto,
    [string] $Usuario,
    [string] $Contrasena,
    [string] $Base,
    [string] $MysqlExe,
    [string] $CarpetaRespaldos,
    [switch] $DatosPrueba,
    [switch] $SinRespaldo,
    [switch] $Forzar
)

$ErrorActionPreference = 'Stop'

# --------------------------------------------------------------------
# Ubicacion de las cosas
# El script se puede invocar desde cualquier carpeta: todo se resuelve
# relativo a donde esta el .ps1, no a donde esta parada la consola.
# --------------------------------------------------------------------
$carpetaBd     = Split-Path -Parent $MyInvocation.MyCommand.Path
$raiz          = Split-Path -Parent $carpetaBd
$sqlEsquema    = Join-Path $carpetaBd 'tambo_actualizacion.sql'
$sqlDatos      = Join-Path $carpetaBd 'tambo_datos_prueba.sql'
$appsettings   = Join-Path $raiz 'Tesis\appsettings.json'

function Write-Paso   ([string] $Texto) { Write-Host "`n== $Texto" -ForegroundColor Cyan }
function Write-Ok     ([string] $Texto) { Write-Host "   $Texto"   -ForegroundColor Green }
function Write-Aviso  ([string] $Texto) { Write-Host "   $Texto"   -ForegroundColor Yellow }

# --------------------------------------------------------------------
# 1. Los datos de conexion
#
# La cadena de appsettings.json tiene la forma
#     server=localhost; port=3306; database=tambo; uid=root; pwd=;
# Se parte en pares clave=valor y cada uno cede ante el parametro
# homonimo, si vino.
# --------------------------------------------------------------------
Write-Paso 'Datos de conexion'

$deArchivo = @{}
if (Test-Path $appsettings) {
    $json = Get-Content $appsettings -Raw -Encoding UTF8 | ConvertFrom-Json
    $cadena = $json.ConnectionStrings.Tambo
    if ($cadena) {
        foreach ($parte in $cadena -split ';') {
            if ($parte -match '^\s*([^=]+?)\s*=\s*(.*?)\s*$') {
                $deArchivo[$Matches[1].ToLower()] = $Matches[2]
            }
        }
        Write-Ok "Leidos de Tesis\appsettings.json"
    }
} else {
    Write-Aviso "No se encontro Tesis\appsettings.json: se usan los valores por defecto"
}

function Resolver ([string] $Valor, [string] $Clave, [string] $PorDefecto) {
    if ($Valor)                  { return $Valor }
    if ($deArchivo.ContainsKey($Clave) -and $deArchivo[$Clave] -ne '') { return $deArchivo[$Clave] }
    return $PorDefecto
}

$Servidor = Resolver $Servidor 'server'   'localhost'
$Base     = Resolver $Base     'database' 'tambo'
$Usuario  = Resolver $Usuario  'uid'      'root'
if (-not $Puerto) { $Puerto = [int] (Resolver $null 'port' '3306') }

# La contrasena vacia en appsettings.json es un valor legitimo -una
# instalacion de desarrollo sin contrasena-, asi que no se pregunta por
# ella salvo que la clave directamente no este.
if (-not $Contrasena) {
    if ($deArchivo.ContainsKey('pwd')) {
        $Contrasena = $deArchivo['pwd']
    } else {
        $segura = Read-Host "   Contrasena de MySQL para '$Usuario'" -AsSecureString
        $Contrasena = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
            [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segura))
    }
}

Write-Ok "$Usuario@${Servidor}:$Puerto, base '$Base'"

# --------------------------------------------------------------------
# 2. El cliente de MySQL
#
# Se busca primero en el PATH y despues en las instalaciones habituales
# de Windows, que es donde queda si se instalo con el instalador oficial
# y nadie toco las variables de entorno.
# --------------------------------------------------------------------
Write-Paso 'Cliente de MySQL'

function Buscar-Ejecutable ([string] $Nombre) {
    $enPath = Get-Command $Nombre -ErrorAction SilentlyContinue
    if ($enPath) { return $enPath.Source }

    $candidatos = @(
        "$env:ProgramFiles\MySQL\MySQL Server*\bin\$Nombre.exe",
        "${env:ProgramFiles(x86)}\MySQL\MySQL Server*\bin\$Nombre.exe",
        "$env:ProgramFiles\MySQL\MySQL Workbench*\$Nombre.exe",
        "C:\xampp\mysql\bin\$Nombre.exe",
        "C:\laragon\bin\mysql\*\bin\$Nombre.exe"
    )
    foreach ($patron in $candidatos) {
        $hallado = Get-ChildItem $patron -ErrorAction SilentlyContinue |
                   Sort-Object FullName -Descending | Select-Object -First 1
        if ($hallado) { return $hallado.FullName }
    }
    return $null
}

if (-not $MysqlExe) { $MysqlExe = Buscar-Ejecutable 'mysql' }
if (-not $MysqlExe) {
    throw "No se encontro mysql.exe. Instalar MySQL, agregar su carpeta bin al PATH, o indicar la ruta con -MysqlExe."
}
$mysqldumpExe = Buscar-Ejecutable 'mysqldump'
Write-Ok $MysqlExe

# --------------------------------------------------------------------
# 3. Correr
#
# MYSQL_PWD mantiene la contrasena fuera de la linea de comandos. El
# finally de mas abajo la borra pase lo que pase.
#
# Los .sql se le pasan a mysql con 'source' y no por la entrada estandar
# para que los lea el mismo, en bytes: canalizarlos desde PowerShell 5.1
# los reconvierte a la pagina de codigos de la consola y rompe los
# acentos de los datos ('Preñada' es un estado reproductivo, no un
# adorno). Por lo mismo van con barra normal: en el comando 'source' la
# barra invertida es un escape.
# --------------------------------------------------------------------
$env:MYSQL_PWD = $Contrasena

function Invoke-Sql ([string] $Archivo, [string] $BaseDestino) {
    $ruta = (Resolve-Path $Archivo).Path -replace '\\', '/'
    $argumentos = @(
        "--host=$Servidor", "--port=$Puerto", "--user=$Usuario",
        '--default-character-set=utf8mb4',
        # --table para que los informes del final salgan en cuadro. Las
        # advertencias no se piden: MySQL 8 marca como obsoleto el ancho
        # de INT(11) y taparia el informe con una linea por columna.
        '--table'
    )
    if ($BaseDestino) { $argumentos += "--database=$BaseDestino" }
    $argumentos += @('-e', "source $ruta")

    & $MysqlExe @argumentos
    if ($LASTEXITCODE -ne 0) {
        throw "mysql termino con codigo $LASTEXITCODE al correr $(Split-Path -Leaf $Archivo). La base quedo como estaba antes de esa sentencia."
    }
}

try {
    # --- 3.a Conexion ------------------------------------------------
    Write-Paso 'Probando la conexion'
    & $MysqlExe "--host=$Servidor" "--port=$Puerto" "--user=$Usuario" `
                '--silent' '-e' 'SELECT VERSION();' | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo conectar a MySQL en ${Servidor}:$Puerto como '$Usuario'. Revisar que el servicio este corriendo y que la contrasena de appsettings.json sea la correcta."
    }
    Write-Ok 'Responde'

    # --- 3.b Respaldo ------------------------------------------------
    # Solo si la base ya existe: no hay nada que respaldar de una base
    # que este script esta por crear.
    $existe = & $MysqlExe "--host=$Servidor" "--port=$Puerto" "--user=$Usuario" '--silent' '--skip-column-names' `
                          '-e' "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = '$Base';"

    if ($SinRespaldo) {
        Write-Paso 'Respaldo'
        Write-Aviso 'Salteado por -SinRespaldo'
    } elseif ($existe -eq '0') {
        Write-Paso 'Respaldo'
        Write-Ok "La base '$Base' todavia no existe: no hay nada que respaldar"
    } elseif (-not $mysqldumpExe) {
        Write-Paso 'Respaldo'
        Write-Aviso 'No se encontro mysqldump.exe: se sigue sin respaldar'
    } else {
        Write-Paso 'Respaldo'
        if (-not $CarpetaRespaldos) { $CarpetaRespaldos = Join-Path $raiz 'respaldos' }
        if (-not (Test-Path $CarpetaRespaldos)) {
            New-Item -ItemType Directory -Path $CarpetaRespaldos | Out-Null
        }
        $archivo = Join-Path $CarpetaRespaldos ("{0}_{1:yyyyMMdd_HHmmss}.sql" -f $Base, (Get-Date))

        & $mysqldumpExe "--host=$Servidor" "--port=$Puerto" "--user=$Usuario" `
                        '--default-character-set=utf8mb4' '--single-transaction' `
                        '--routines' '--events' $Base --result-file=$archivo
        if ($LASTEXITCODE -ne 0) { throw "mysqldump fallo: no se sigue sin respaldo." }

        $kb = [math]::Round((Get-Item $archivo).Length / 1KB, 1)
        Write-Ok "$archivo ($kb KB)"
        Write-Aviso 'El respaldo no incluye Tesis\wwwroot\fotos, que hay que copiar aparte.'
    }

    # --- 3.c Esquema -------------------------------------------------
    Write-Paso 'Actualizando el esquema'
    Invoke-Sql $sqlEsquema $null

    # --- 3.d Datos de prueba -----------------------------------------
    if ($DatosPrueba) {
        Write-Paso 'Datos de prueba'
        if (-not $Forzar) {
            Write-Aviso "Esto VACIA las tablas de datos de '$Base' y carga el rodeo de prueba."
            Write-Aviso 'Razas, categorias y configuracion no se tocan.'
            $r = Read-Host '   Escribir SI para continuar'
            if ($r -ne 'SI') {
                Write-Aviso 'Cancelado. El esquema quedo actualizado igual.'
                return
            }
        }
        Invoke-Sql $sqlDatos $Base
        Write-Ok 'Cargado. Las dos consultas de verificacion del final tienen que venir vacias:'
        Write-Ok 'la de stock contra movimientos y la de litros de lote contra controles individuales.'
    }

    Write-Host "`nListo." -ForegroundColor Green
    Write-Host "Para levantar el sistema: dotnet run --project Tesis/Tesis.csproj`n"
}
finally {
    Remove-Item Env:\MYSQL_PWD -ErrorAction SilentlyContinue
}
