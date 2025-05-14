
-- ===================================================================
-- 0) PARA GUARDAR EL USUARIO QUE REALIZA LAS ACCIONES, EJECUTAR EL
--    SIGUIENTE COMANDO, ESE REFERENCIA AL DE LA TABLA USERS
-- ===================================================================

-- SET app.current_user_id TO 1;

-- ===================================================================
-- 1) TRIGGERS TEST - Test de trigger set_updated_at
-- ===================================================================

-- Insertar un nuevo paciente
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at)
VALUES ('John', 'Doe', '1990-01-01', '123 Main St', 'Male', 1, 1, NOW());

-- Actualizar la dirección de un paciente
UPDATE patients
SET address = '456 New St'
WHERE id = 1;

-- Verificar si el campo `updated_at` fue actualizado
SELECT * FROM patients WHERE id = 1;

-- ===================================================================
-- 2) TRIGGERS TEST - Test de auditoría para INSERT y UPDATE
-- ===================================================================

-- Insertar un nuevo paciente (debería ser auditado)
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id, created_at)
VALUES ('Alice', 'Smith', '1985-06-10', '789 Oak St', 'Female', 2, 2, NOW());

-- Actualizar la dirección del paciente insertado (debería ser auditado)
UPDATE patients
SET address = '101 Maple St'
WHERE id = (SELECT id FROM patients WHERE name = 'Alice' AND last_name = 'Smith');

-- Verificar si los registros de auditoría existen en `logs` y `history`
SELECT * FROM logs WHERE table_name = 'patients' AND action IN ('INSERT', 'UPDATE');
SELECT * FROM history WHERE table_name = 'patients' AND action IN ('INSERT', 'UPDATE');

-- ===================================================================
-- 3) TRIGGERS TEST - Test de auditoría para DELETE
-- ===================================================================

-- Eliminar el paciente insertado (debería ser auditado)
DELETE FROM patients WHERE name = 'Alice' AND last_name = 'Smith';

-- Verificar si el registro de eliminación existe en `logs` y `history`
SELECT * FROM logs WHERE table_name = 'patients' AND action = 'DELETE';
SELECT * FROM history WHERE table_name = 'patients' AND action = 'DELETE';

-- ===================================================================
-- 4) TRIGGERS TEST - Test de bloqueo de DELETE (si está habilitado)
-- ===================================================================

-- Intentar eliminar un paciente (esto debería fallar si el trigger de bloqueo de DELETE está habilitado)
-- en ESTE CASO, NO SE ENCUENTRA HABILITADO, por lo cual si deja eliminar, mas la data queda guardada en los logs
 DELETE FROM patients WHERE id = 113;

-- ===================================================================
-- 5) TRIGGERS TEST - Test de auditoría en `patient_vaccines`
-- ===================================================================

-- Insertar un nuevo registro en `patient_vaccines` (debería ser auditado)
INSERT INTO patient_vaccines (patient_id, vaccine_id, dosis, age_at_application, application_date, created_at)
VALUES (1, 1, '1 dosis', 30, '2025-05-13', NOW());

-- Eliminar el registro de `patient_vaccines` (debería ser auditado)
DELETE FROM patient_vaccines WHERE patient_id = 1 AND vaccine_id = 1;

-- Verificar si los registros de auditoría de `patient_vaccines` existen en `logs` y `history`
SELECT * FROM logs WHERE table_name = 'patient_vaccines' AND action IN ('INSERT', 'DELETE');
SELECT * FROM history WHERE table_name = 'patient_vaccines' AND action IN ('INSERT', 'DELETE');

-- ===================================================================
-- 6) TRIGGERS TEST - Test de comportamiento con `updated_at` (si está configurado)
-- ===================================================================

-- Actualizar un paciente y verificar si `updated_at` fue actualizado
UPDATE patients
SET address = '222 Pine St'
WHERE id = 1;

-- Verificar que el campo `updated_at` fue actualizado
SELECT id, name, address, updated_at FROM patients WHERE id = 1;

-- ===================================================================
-- 7) TRIGGERS TEST - Test de la función de auditoría con `logs` y `history`
-- ===================================================================

-- Realizar una operación en la tabla `patients` (ya realizada antes)
UPDATE patients
SET address = '333 Birch St'
WHERE id = 1;

-- Verificar el registro de `logs` y `history` después del `UPDATE`
SELECT * FROM logs WHERE table_name = 'patients' AND action = 'UPDATE';
SELECT * FROM history WHERE table_name = 'patients' AND action = 'UPDATE';

-- ===================================================================
-- 8) TRIGGERS TEST - Test de `DELETE` en tablas que tienen triggers
-- ===================================================================

-- Eliminar un paciente y verificar que se ha registrado correctamente en `logs` y `history`
DELETE FROM patients WHERE id = 1;

-- Verificar que la eliminación se ha registrado en `logs` y `history`
SELECT * FROM logs WHERE table_name = 'patients' AND action = 'DELETE';
SELECT * FROM history WHERE table_name = 'patients' AND action = 'DELETE';

-- ===================================================================
-- 9) TRIGGERS TEST - Test de `INSERT` y `UPDATE` en `patient_vaccines`
-- ===================================================================

-- Insertar un nuevo registro de vacuna para un paciente
INSERT INTO patient_vaccines (patient_id, vaccine_id, dosis, age_at_application, application_date, created_at)
VALUES (1, 2, '1 dosis', 35, '2025-06-01', NOW());

-- Actualizar la dosis de la vacuna para el paciente
UPDATE patient_vaccines
SET dosis = '2 dosis'
WHERE patient_id = 1 AND vaccine_id = 2;

-- Verificar si se registraron correctamente los cambios en `logs` y `history`
SELECT * FROM logs WHERE table_name = 'patient_vaccines' AND action IN ('INSERT', 'UPDATE');
SELECT * FROM history WHERE table_name = 'patient_vaccines' AND action IN ('INSERT', 'UPDATE');



