# 2.5 Deployment

Fuente de la sección: `Tesis/Tesis.csproj`, `Tesis/appsettings.json`, `bd/LEEME.md`
y el apartado «Hosting y Despliegue» del anteproyecto.

Sigue la forma del ejemplo del tutor: **qué necesita el servidor, qué necesita quien
usa el sistema, y quién lo opera con qué preparación.** Los comandos de puesta en
marcha son detalle de desarrollo y viven en `bd/LEEME.md`.

---

El sistema es una aplicación web y se despliega en el servicio de hosting compartido
**SmarterASP.NET**, elegido en el anteproyecto por su compatibilidad nativa con .NET,
su soporte de MySQL y un costo de aproximadamente **U$S 2,95 mensuales**, acorde a un
establecimiento con una sola usuaria.

Para funcionar, el servidor necesita **.NET 10** y un motor **MySQL**. La aplicación
usa tres bibliotecas externas, todas resueltas por el gestor de paquetes al publicar:
**MySql.Data** para el acceso a datos, **QuestPDF** para los reportes en PDF y
**ClosedXML** para los reportes en Excel. La base se crea con los dos scripts de
`bd/`: uno arma las tablas y los datos semilla, el otro carga un rodeo de prueba.

La cadena de conexión y las credenciales de acceso **no se escriben en el código**:
se leen de la configuración de la aplicación, que en producción se carga desde el
panel del hosting y en desarrollo desde el almacén de secretos local. Esto vale tanto
para el acceso a la base como para el token del bot de Telegram.

Del lado de quien usa el sistema no hay nada que instalar. Alcanza con **un navegador
moderno —Google Chrome, Mozilla Firefox o Microsoft Edge en versiones actualizadas—**
y conexión a internet. El sistema es responsive y su uso está verificado **a partir de
los 375 píxeles de ancho**, de modo que la encargada puede cargar los datos desde el
celular en el propio tambo, que es donde ocurren los eventos que se registran: el
celo que se detecta, el parto que se asiste, el ordeñe que se acaba de medir.

**Operar el sistema no requiere formación técnica.** La usuaria es la encargada del
establecimiento, que conoce el trabajo que el sistema representa; lo que necesita es
saber dónde se anota cada cosa, y eso lo cubren el manual de usuario de la sección
2.4 y la capacitación prevista en el plan correspondiente. El sistema no exige
administrar usuarios, permisos ni copias locales: hay una única versión centralizada
en el hosting y toda la información vive ahí.
