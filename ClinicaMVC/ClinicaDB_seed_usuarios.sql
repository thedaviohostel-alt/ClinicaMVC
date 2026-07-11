-- Script para crear el primer usuario administrador.
-- El campo "clave" DEBE contener un hash BCrypt, nunca la contraseña en texto plano.
--
-- Cómo generar el hash (elige una opción):
--   1) En este mismo proyecto, crea un archivo Program temporal o un test unitario con:
--        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin123!"));
--      y copia el resultado aquí abajo.
--   2) O usa cualquier generador BCrypt confiable (work factor 11 o 12).
--
-- Reemplaza 'PEGA_AQUI_EL_HASH_BCRYPT' antes de ejecutar este script.

INSERT INTO usuarios (nombre_usuario, clave, id_rol, activo)
VALUES ('admin', 'PEGA_AQUI_EL_HASH_BCRYPT', 1, TRUE);
