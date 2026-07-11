# Diagramas del Sistema — Gestión de Clínica

> Cómo generar las imágenes: pega cada bloque en https://mermaid.live (exporta PNG/SVG),
> o instala la extensión "Markdown Preview Mermaid Support" en VS Code y abre este archivo.

## 1. Diagrama de Casos de Uso

```mermaid
flowchart TB
    Admin([Administrador])
    Recep([Recepcionista])
    Medico([Médico])

    subgraph Sistema de Gestión de Clínica
        UC1[Iniciar sesión]
        UC2[Gestionar Pacientes\nCRUD]
        UC3[Gestionar Médicos\nCRUD]
        UC4[Agendar Cita]
        UC5[Verificar disponibilidad\ndel médico]
        UC6[Editar / Cancelar Cita]
        UC7[Atender Cita]
        UC8[Registrar Historial Médico]
        UC9[Consultar Historial\npor paciente]
        UC10[Generar Reporte\nde Pacientes]
        UC11[Generar Reporte\nde Citas por fecha]
    end

    Admin --> UC1
    Recep --> UC1
    Medico --> UC1

    Admin --> UC2
    Recep --> UC2
    Medico --> UC2

    Admin --> UC3

    Admin --> UC4
    Recep --> UC4
    UC4 --> UC5

    Admin --> UC6
    Recep --> UC6

    Admin --> UC7
    Medico --> UC7
    UC7 --> UC8

    Admin --> UC9
    Recep --> UC9
    Medico --> UC9

    Admin --> UC10
    Recep --> UC10
    Medico --> UC10

    Admin --> UC11
    Recep --> UC11
    Medico --> UC11
```

## 2. Diagrama de Clases

```mermaid
classDiagram
    class Rol {
        +int IdRol
        +string NombreRol
    }

    class Usuario {
        +int IdUsuario
        +string NombreUsuario
        +string Clave
        +int IdRol
        +bool Activo
    }

    class Paciente {
        +int IdPaciente
        +string Nombre
        +string Cedula
        +DateTime FechaNacimiento
        +string Telefono
        +string Direccion
        +string TipoSangre
        +DateTime FechaRegistro
    }

    class Medico {
        +int IdMedico
        +string Nombre
        +string Cedula
        +string Especialidad
        +string Telefono
        +string Turno
        +DateTime FechaRegistro
    }

    class Cita {
        +int IdCita
        +int IdPaciente
        +int IdMedico
        +DateTime Fecha
        +TimeOnly Hora
        +string Estado
        +string Observaciones
        +ValidarDisponibilidad() bool
    }

    class HistorialMedico {
        +int IdHistorial
        +int IdPaciente
        +int IdMedico
        +int? IdCita
        +DateTime FechaConsulta
        +string Sintomas
        +string Diagnostico
        +string Tratamiento
    }

    Usuario "N" --> "1" Rol : tiene
    Cita "N" --> "1" Paciente : pertenece a
    Cita "N" --> "1" Medico : atendida por
    HistorialMedico "N" --> "1" Paciente : pertenece a
    HistorialMedico "N" --> "1" Medico : registrado por
    HistorialMedico "0..1" --> "1" Cita : origina de
```
