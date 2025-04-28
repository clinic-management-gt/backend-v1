-- 1) Tenants
INSERT INTO tenants (id, db_name, db_host, db_user, db_password)
VALUES (1, 'clinicaPediatrica', 'localhost', 'sudo', 'root');

-- 2) Roles
INSERT INTO roles (id, type, name, can_edit)
VALUES (1, 1, 'Admin', TRUE);

-- 3) Modules
INSERT INTO modules (id, name, description)
VALUES (1, 'Pacientes', 'Gestión de pacientes');

-- 4) Permissions
INSERT INTO permissions (id, role_id, module_id, can_view, can_edit, can_delete)
VALUES (1, 1, 1, TRUE, TRUE, TRUE);

-- 5) Users
INSERT INTO users (id, first_name, last_name, email, role_id)
VALUES (1, 'Juan', 'Pérez', 'juan.perez@example.com', 1);

-- 6) Patient Types
INSERT INTO patient_types (id, name, description)
VALUES
(1, 'Regular', 'Paciente pediátrico general'),
(2, 'Emergencia', 'Paciente pediátrico en situación de emergencia'),
(3, 'Crónico', 'Paciente pediátrico con enfermedades crónicas'),
(4, 'Control de Crecimiento', 'Paciente en seguimiento de desarrollo y crecimiento'),
(5, 'Vacunación', 'Paciente en programa de vacunación pediátrica'),
(6, 'Neonatal', 'Paciente recién nacido en control'),
(7, 'Adolescente', 'Paciente adolescente en transición a medicina de adultos'),
(8, 'Especialidad', 'Paciente pediátrico atendido en especialidades (cardiología, neurología, etc.)');


-- 7) Blood Types
INSERT INTO blood_types (id, type, description)
VALUES
(1, 'A+', 'Tipo A positivo'),
(2, 'A-', 'Tipo A negativo'),
(3, 'B+', 'Tipo B positivo'),
(4, 'B-', 'Tipo B negativo'),
(5, 'AB+', 'Tipo AB positivo'),
(6, 'AB-', 'Tipo AB negativo'),
(7, 'O+', 'Tipo O positivo'),
(8, 'O-', 'Tipo O negativo');

-- 8) Patients
-- 1
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (1, 'María', 'Gómez', '2015-06-01', 'Calle Falsa 123', 'Female', 1, 1);

-- 2
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (2, 'Juan', 'Pérez', '1990-04-15', 'Av. Central 456', 'Male', 2, 2);

-- 3
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (12, 'Juan', 'González', '1992-03-24', 'Av. Flores 454', 'Male', 3, 1);

-- 4
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (3, 'Lucía', 'Fernández', '1985-11-23', 'Calle del Sol 789', 'Female', 3, 1);

-- 5
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (4, 'Carlos', 'Ramírez', '2000-02-10', 'Boulevard Norte 321', 'Male', 4, 2);

-- 6
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (5, 'Ana', 'Martínez', '2012-07-30', 'Callejón Luna 12', 'Female', 1, 1);

-- 7
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (6, 'Diego', 'López', '1978-09-12', 'Av. Las Rosas 555', 'Male', 5, 2);

-- 8
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (7, 'Sofía', 'Torres', '1995-01-05', 'Calle Primavera 77', 'Female', 6, 1);

-- 9
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (8, 'Miguel', 'Castro', '2003-12-22', 'Camino Real 888', 'Male', 7, 2);

-- 10
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (9, 'Valentina', 'Mendoza', '2018-03-18', 'Calle Sauce 303', 'Female', 2, 1);

-- 11
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (10, 'Andrés', 'Morales', '1980-06-29', 'Pasaje del Río 47', 'Male', 3, 2);

-- 12
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (11, 'Camila', 'Silva', '1999-10-09', 'Av. Libertad 910', 'Female', 4, 1);


-- 9) Alergias
INSERT INTO alergies (id, alergy_code, alergy_description)
VALUES (1, 'ALG01', 'Alergia a polen');

-- 10) Chronic Diseases
INSERT INTO chronic_diseases (id, disease_code, disease_description)
VALUES (1, 'CD01', 'Asma');

-- 11) Patient Allergies
INSERT INTO patient_alergies (id, patient_id, alergy_id)
VALUES (1, 1, 1);

-- 12) Patient Chronic Diseases
INSERT INTO patient_chronic_diseases (id, patient_id, chronic_disease_id)
VALUES (1, 1, 1);

-- 13) Medical Records
INSERT INTO medical_records (id, patient_id, weight, height, family_history, notes)
VALUES (1, 1, 20.5, 1.15, 'Sin antecedentes familiares', 'Paciente en buen estado');

-- 14) Contacts
INSERT INTO contacts (id, patient_id, type, name, last_name)
VALUES (1, 1, 'Padre', 'Carlos', 'Gómez');

-- 15) Phones
INSERT INTO phones (id, contact_id, phone)
VALUES (1, 1, '55512345');

-- 16) Exams
INSERT INTO exams (id, name, description)
VALUES (1, 'Hemograma', 'Examen de sangre completo');

-- 17) Patient Exams
INSERT INTO patient_exams (id, patient_id, exam_id, result_text, result_file_path)
VALUES (1, 1, 1, 'Hemoglobina: 13 g/dL', '/results/hemograma1.pdf');

-- 18) Appointments
INSERT INTO appointments (id, patient_id, doctor_id, appointment_date, reason, status)
VALUES (1, 1, 1, '2025-04-23 09:00:00', 'Chequeo rutinario', 'Confirmado');

-- 19) Diagnosis
INSERT INTO diagnosis (id, appointment_id, description, diagnosis_date)
VALUES (1, 1, 'Sin anomalías', '2025-04-23 09:30:00');

-- 20) Medicines
INSERT INTO medicines (id, name, provider, type)
VALUES (1, 'Ibuprofeno', 'Proveedor X', 'Analgésico');

-- 21) Treatments
INSERT INTO treatments (id, appointment_id, medicine_id, dosis, duration, frequency, observaciones, status)
VALUES (1, 1, 1, '200mg', '5 días', 'Cada 8h', 'Tomar con alimentos', 'No Terminado');

-- 22) Recipes
INSERT INTO recipes (id, treatment_id, prescription)
VALUES (1, 1, 'Ibuprofeno 200mg c/8h x 5 días');

-- 23) Insurance
INSERT INTO insurance (id, patient_id, provider_name, policy_number, coverage_details)
VALUES (1, 1, 'Seguros ABC', 'POL12345', 'Cobertura completa');

-- 24) Vaccines
INSERT INTO vaccines (id, name, brand)
VALUES (1, 'BCG', 'Laboratorios XYZ');

-- 25) Patient Vaccines
INSERT INTO patient_vaccines (id, patient_id, vaccine_id, dosis, age_at_application, application_date)
VALUES (1, 1, 1, '0.1ml', 1, '2016-01-01');
