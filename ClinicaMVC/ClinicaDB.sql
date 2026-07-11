
CREATE TABLE roles (
    id_rol SERIAL PRIMARY KEY,
    nombre_rol VARCHAR(20) NOT NULL UNIQUE
);
SELECT * FROM roles;

CREATE TABLE usuarios (
    id_usuario SERIAL PRIMARY KEY,
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    clave VARCHAR(255) NOT NULL,
    id_rol INTEGER NOT NULL,
    activo BOOLEAN DEFAULT TRUE,
 CONSTRAINT fk_usuario_rol
        FOREIGN KEY (id_rol)
        REFERENCES roles(id_rol)
);
SELECT * FROM usuarios;

CREATE TABLE pacientes (
    id_paciente SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    cedula VARCHAR(20) UNIQUE NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    telefono VARCHAR(20),
    direccion VARCHAR(200),
    tipo_sangre VARCHAR(5),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
SELECT * FROM pacientes;

CREATE TABLE medicos (
    id_medico SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    cedula VARCHAR(20) UNIQUE NOT NULL,
    especialidad VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    turno VARCHAR(20) NOT NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
SELECT * FROM medicos;

CREATE TABLE citas (
    id_cita SERIAL PRIMARY KEY,
    id_paciente INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    estado VARCHAR(20) DEFAULT 'Pendiente',
    observaciones TEXT,
    CONSTRAINT fk_cita_paciente
        FOREIGN KEY (id_paciente)
        REFERENCES pacientes(id_paciente),
   CONSTRAINT fk_cita_medico
        FOREIGN KEY (id_medico)
        REFERENCES medicos(id_medico)
);
SELECT * FROM citas;

CREATE TABLE historial_medico (
    id_historial SERIAL PRIMARY KEY,
    id_paciente INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    id_cita INTEGER,
    fecha_consulta TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    sintomas TEXT,
    diagnostico TEXT,
    tratamiento TEXT,
    CONSTRAINT fk_historial_paciente
        FOREIGN KEY (id_paciente)
        REFERENCES pacientes(id_paciente),
    CONSTRAINT fk_historial_medico
        FOREIGN KEY (id_medico)
        REFERENCES medicos(id_medico),
    CONSTRAINT fk_historial_cita
        FOREIGN KEY (id_cita)
        REFERENCES citas(id_cita)
);
SELECT * FROM historial_medico;





