# 2.6 Política de Seguridad y Respaldos

Fuente de la sección: el RNF de Seguridad y el de Fiabilidad del anteproyecto, RF0.1,
el control preventivo del riesgo **R9**, y el código de `Tesis/Persistencia/`.

Sigue la forma del ejemplo del tutor: **acceso, cómo se protegen los datos, y
respaldos con su periodicidad y su destino.**

> **Un párrafo depende del hallazgo H6 de la auditoría** —hoy `appsettings.json` está
> versionado con las credenciales en texto plano— y está escrito suponiendo que el
> código se acomoda al RNF. Si esa decisión cambia, cambia el párrafo.

---

## Acceso

El sistema **restringe el acceso mediante un único par de credenciales fijas**,
correspondiente a la encargada del establecimiento. No administra múltiples usuarios
ni perfiles de permisos, y esto es una decisión de alcance, no una omisión: el
establecimiento tiene una sola persona a cargo de los registros, y un esquema de roles
habría agregado complejidad sin resolver ningún problema real.

La consecuencia hay que decirla con todas las letras: **quien tiene esas credenciales
puede hacer todo lo que el sistema permite**, y no queda registro de quién hizo cada
cosa, porque el sistema asume que siempre es la misma persona. Si en el futuro el
tambo incorpora personal que también cargue datos, el esquema de acceso es lo primero
que habría que revisar.

**Todo el sitio está detrás del inicio de sesión**, y no pantalla por pantalla sino
por configuración: la aplicación exige autenticación para la carpeta raíz completa y
declara como excepción únicamente la propia pantalla de inicio de sesión y la de
error. La sesión se sostiene con una cookie de autenticación. La consecuencia es la
que se busca: escribir en el navegador la dirección de cualquier pantalla interna sin
haber iniciado sesión lleva al login, y lo mismo pasa al volver atrás con el navegador
después de cerrar la sesión. No hay forma de olvidarse de proteger una pantalla
nueva —queda protegida por estar donde está—.

## Dónde viven los secretos

Ni las credenciales de acceso ni la cadena de conexión a la base están escritas en el
código. Se leen de la configuración de la aplicación, que se provee desde afuera del
repositorio: **el panel del hosting en producción y el almacén de secretos local en
desarrollo**. Lo mismo vale para el token del bot de Telegram, que da control sobre el
canal por el que el sistema envía las alertas.

El repositorio versiona únicamente un archivo de configuración con los marcadores de
cada valor, sin los valores. Así, el código se puede clonar y publicar sin que ningún
secreto viaje con él.

## Integridad de la información

**Ninguna consulta se arma pegando texto.** Las 78 variables que la aplicación envía a
la base viajan como parámetros con nombre, y hay un único punto en todo el sistema
donde un valor se carga en un comando SQL. No existe, por construcción, la posibilidad
de que un dato escrito en un formulario altere la consulta que se ejecuta.

**Las operaciones que escriben en más de una tabla se resuelven dentro de una
transacción.** Es lo que exige el RNF de Fiabilidad y lo que el dominio necesita: dar
de alta un animal escribe en `animales` y en `hembras` o `machos`; registrar un parto
escribe el parto, abre la lactancia, da de alta la cría y actualiza el estado de la
madre. Si algo falla a mitad de camino, no queda media operación guardada.

## Respaldos

La información del establecimiento —el rodeo, la producción, la sanidad y el stock—
es el activo que el sistema cuida, y es irrecuperable si se pierde: son años de
registros que nadie tiene en otro lado.

El servicio de hosting realiza **respaldos automáticos**, que es el control preventivo
comprometido en el riesgo **R9** del análisis de riesgos. Sobre esa base, las
recomendaciones a la encargada son dos:

- **Periodicidad.** Un respaldo diario, tomado de madrugada, que es cuando el sistema
  no se está usando. La operativa del tambo empieza con el ordeñe de la mañana, así
  que a esa hora no hay nadie cargando datos y la copia sale consistente.

- **Dónde queda cada copia.** Dos copias, y no en el mismo lugar: la del hosting, y
  una descarga periódica guardada en una computadora del establecimiento. Un respaldo
  que vive únicamente en el servidor no protege del escenario en que el problema es el
  servidor.

La restauración se hace con los mismos scripts de `bd/` que crean la base, cargando
encima el respaldo más reciente.
