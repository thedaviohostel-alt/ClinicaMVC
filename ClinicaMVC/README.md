# Sistema de Gestión de Clínica

Aplicación web desarrollada en **ASP.NET MVC (.NET 8)** para centralizar la gestión de pacientes, médicos, citas médicas e historiales clínicos de una clínica, con control de acceso por roles (Administrador, Recepcionista, Médico).

Proyecto final de **Programación III — UAPA, Trimestre Mayo-Julio 2026.**

## Objetivo
Desarrollar un sistema informático funcional que permita gestionar de forma centralizada y segura la información de pacientes, médicos, citas médicas e historiales clínicos de una clínica, aplicando principios de programación orientada a objetos, manejo de bases de datos relacionales y buenas prácticas de desarrollo de software.

## Tecnologías utilizadas
- **Backend:** ASP.NET Core MVC (.NET 8), C#
- **Base de datos:** PostgreSQL + Entity Framework Core 8 (Npgsql)
- **Autenticación:** Cookies de ASP.NET Core + BCrypt.Net-Next para hash de contraseñas
- **Reportes:** QuestPDF (PDF) y ClosedXML (Excel)
- **Diagramas:** Mermaid
- **Control de versiones:** Git y GitHub

## Módulos implementados
- [x] Autenticación con roles (Administrador, Recepcionista, Médico)
- [x] Gestión de Pacientes (CRUD completo, búsqueda, validaciones)
- [x] Gestión de Médicos (CRUD completo, búsqueda, validaciones)
- [x] Citas Médicas (agendar, editar, cancelar, verificación de disponibilidad del médico)
- [x] Historial Médico / Consultas (flujo "Atender cita", historial por paciente)
- [x] Generación de reportes PDF/Excel (pacientes y citas por rango de fechas)
- [x] Diagramas UML (casos de uso, clases) — ver `docs/diagramas.md`
- [x] Documentación técnica completa — ver `Documentacion_Tecnica_ClinicaMVC.docx`
- [ ] Video de presentación *(enlace se agrega antes de la entrega final)*

## Instrucciones de instalación y ejecución

### Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL instalado y en ejecución
- Visual Studio 2022 / VS Code (opcional)

## Flujo funcional resumido
1. **Recepcionista/Administrador** registra pacientes y médicos.
2. **Recepcionista/Administrador** agenda una cita (el sistema valida que el médico no tenga otra cita activa en la misma fecha/hora).
3. **Médico/Administrador** "atiende" la cita: registra síntomas, diagnóstico y tratamiento. El sistema crea el historial médico y cambia el estado de la cita a *Atendida*.
4. Cualquier usuario autorizado puede **consultar el historial médico** de un paciente.
5. **Reportes**: listado de pacientes y citas por rango de fechas, exportables en PDF y Excel.

## Capturas de pantalla
> Reemplaza estos placeholders con capturas reales antes de la entrega. Instrucciones abajo.

| Login | Módulo de Citas |
|---|---|
| `docs/screenshots/login.png` | `docs/screenshots/citas.png` |

| Atender cita | Reporte PDF |
|---|---|
| `docs/screenshots/atender-cita.png` | `docs/screenshots/reporte-pdf.png` |

**Cómo generarlas:** ejecuta el proyecto (`dotnet run`), navega a cada pantalla clave (login, listado de pacientes, listado de médicos, listado de citas, formulario "Atender cita", historial de un paciente, y un reporte PDF/Excel descargado) y toma una captura de cada una (Win+Shift+S en Windows, Cmd+Shift+4 en Mac). Guárdalas en `docs/screenshots/` con esos nombres y actualiza esta tabla.

## Diagramas
Ver `docs/diagramas.md` (código Mermaid del diagrama de casos de uso y del diagrama de clases, listo para pegar en https://mermaid.live).

## Documentación técnica
El documento completo con introducción, objetivos, alcance, análisis del problema, requisitos, herramientas, diagramas, estructura de la base de datos, funcionalidades clave, pruebas funcionales, conclusión y bibliografía está en `Documentacion_Tecnica_ClinicaMVC.docx`.

## Estructura del proyecto
```
ClinicaMVC/
├── Controllers/        # Account, Home, Pacientes, Medicos, Citas, HistorialMedico, Reportes
├── Models/              # Rol, Usuario, Paciente, Medico, Cita, HistorialMedico, ViewModels
├── Data/                # ClinicaContext (Entity Framework)
├── Services/            # ReporteService (generación de PDF y Excel)
├── Views/               # Vistas Razor organizadas por controlador
├── wwwroot/css/         # Estilos
├── docs/                # Diagramas, planificación (semana 1 y 2), capturas de pantalla
├── ClinicaDB.sql         # Script de creación de la base de datos
└── ClinicaDB_seed_usuarios.sql  # Script para crear el primer usuario
```


## Bibliografía
- Microsoft. (2024). *ASP.NET Core documentation.* https://learn.microsoft.com/aspnet/core/
- Microsoft. (2024). *Entity Framework Core documentation.* https://learn.microsoft.com/ef/core/
- PostgreSQL Global Development Group. (2024). *PostgreSQL Documentation.* https://www.postgresql.org/docs/
- Npgsql. (2024). *Npgsql Entity Framework Core Provider Documentation.* https://www.npgsql.org/efcore/
- QuestPDF. (2024). *QuestPDF Documentation.* https://www.questpdf.com/
- ClosedXML. (2024). *ClosedXML Wiki.* https://github.com/ClosedXML/ClosedXML/wiki
- BCrypt.Net-Next. (2024). *NuGet package documentation.* https://www.nuget.org/packages/BCrypt.Net-Next/
- Mermaid. (2024). *Mermaid documentation.* https://mermaid.js.org/
- Candelario, H. (2026). *Sistemas Propuestos — Trimestre Mayo-Julio 2026, Programación 3.* UAPA.
