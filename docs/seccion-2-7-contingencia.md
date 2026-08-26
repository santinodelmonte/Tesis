# 2.7 Plan de contingencia

Fuente de la sección: el análisis de riesgos del anteproyecto, en particular **R6**
—dependencia de servicios externos de notificación— y **R9** —fallas del hosting o
pérdida de información—.

Sigue la forma del ejemplo del tutor: **dos párrafos.** Remite al análisis de riesgos
ya hecho y se hace cargo de lo que pueda pasar después de entregar. No inventa una
lista nueva: el plan de contingencia es la respuesta a los riesgos que el anteproyecto
identificó.

---

El plan de contingencia a aplicar durante el desarrollo del sistema quedó descrito en
el análisis y plan de riesgo del anteproyecto, donde cada riesgo identificado tiene sus
controles preventivos y correctivos. Dos de esos riesgos siguen vigentes una vez que el
sistema está en producción, y por eso se retoman acá. **R9 —falla del servicio de
hosting o pérdida de información—** se cubre con los respaldos descritos en la sección
anterior: la copia diaria del hosting más la copia local del establecimiento permiten
volver a levantar el sistema sobre otro servidor con los scripts de `bd/` y el último
respaldo disponible. **R6 —dependencia del servicio de Telegram—** no interrumpe la
operación: las notificaciones son un aviso adicional y no la única vía de acceso a la
información. Si el servicio deja de responder, las mismas alertas que el bot envía
siguen visibles en el tablero de inicio y en las pantallas de alertas de cada módulo,
que es de donde el bot las toma.

Para los problemas que puedan surgir después de la entrega, el equipo de desarrollo
queda a disposición del establecimiento para resolverlos hasta que el sistema se
encuentre completamente operativo y la encargada trabaje con él con autonomía.
