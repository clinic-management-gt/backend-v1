-- 1) Tenants
INSERT INTO tenants (db_name, db_host, db_user, db_password)
VALUES ('clinicaPediatrica', 'localhost', 'sudo', 'root');

-- 2) Roles
INSERT INTO roles (type, name, can_edit)
VALUES (1, 'Admin', TRUE);

-- 3) Modules
INSERT INTO modules (name, description)
VALUES ('Pacientes', 'Gestión de pacientes');

-- 4) Permissions
INSERT INTO permissions (role_id, module_id, can_view, can_edit, can_delete)
VALUES (1, 1, TRUE, TRUE, TRUE);

-- 5) Users
INSERT INTO users (first_name, last_name, email, role_id)
VALUES ('Juan', 'Pérez', 'juan.perez@example.com', 1);

-- 6) Patient Types
INSERT INTO patient_types (name, description)
VALUES
('Regular', 'Paciente pediátrico general'),
('Emergencia', 'Paciente pediátrico en situación de emergencia'),
('Crónico', 'Paciente pediátrico con enfermedades crónicas'),
('Control de Crecimiento', 'Paciente en seguimiento desarrollo y crecimiento'),
('Vacunación', 'Paciente en programa de vacunación pediátrica'),
('Neonatal', 'Paciente recién nacido en control'),
('Adolescente', 'Paciente adolescente en transición a medicina de adultos'),
('Especialidad', 'Paciente pediátrico atendido en especialidades (cardiología, neurología, etc.)');

-- 7) Blood Types
INSERT INTO blood_types (type, description)
VALUES
('A+', 'Tipo A positivo'),
('A-', 'Tipo A negativo'),
('B+', 'Tipo B positivo'),
('B-', 'Tipo B negativo'),
('AB+', 'Tipo AB positivo'),
('AB-', 'Tipo AB negativo'),
('O+', 'Tipo O positivo'),
('O-', 'Tipo O negativo');

-- 8) Patients
-- 1
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('María', 'Gómez', '2015-06-01', 'Calle Falsa 123', 'Female', 1, 1);

-- 2
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Juan', 'Pérez', '1990-04-15', 'Av. Central 456', 'Male', 2, 2);

-- 3
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Juan', 'González', '1992-03-24', 'Av. Flores 454', 'Male', 3, 1);

-- 4
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Lucía', 'Fernández', '1985-11-23', 'Calle del Sol 789', 'Female', 3, 1);

-- 5
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Carlos', 'Ramírez', '2000-02-10', 'Boulevard Norte 321', 'Male', 4, 2);

-- 6
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Ana', 'Martínez', '2012-07-30', 'Callejón Luna 12', 'Female', 1, 1);

-- 7
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Diego', 'López', '1978-09-12', 'Av. Las Rosas 555', 'Male', 5, 2);

-- 8
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Sofía', 'Torres', '1995-01-05', 'Calle Primavera 77', 'Female', 6, 1);

-- 9
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Miguel', 'Castro', '2003-12-22', 'Camino Real 888', 'Male', 7, 2);

-- 10
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Valentina', 'Mendoza', '2018-03-18', 'Calle Sauce 303', 'Female', 2, 1);

-- 11
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Andrés', 'Morales', '1980-06-29', 'Pasaje del Río 47', 'Male', 3, 2);

-- 12
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES ('Camila', 'Silva', '1999-10-09', 'Av. Libertad 910', 'Female', 4, 1);

-- 9) Alergias
INSERT INTO alergies (alergy_code, alergy_description)
VALUES ('ALG01', 'Alergia a polen');

-- 10) Chronic Diseases
INSERT INTO chronic_diseases (disease_code, disease_description)
VALUES ('CD01', 'Asma');

-- 11) Patient Allergies
INSERT INTO patient_alergies (patient_id, alergy_id)
VALUES (1, 1);

-- 12) Patient Chronic Diseases
INSERT INTO patient_chronic_diseases (patient_id, chronic_disease_id)
VALUES (1, 1);

-- 13) Medical Records
INSERT INTO medical_records (patient_id, weight, height, family_history, notes)
VALUES (1, 20.5, 1.15, 'Sin antecedentes familiares', 'Paciente en buen estado');

-- 14) Contacts
INSERT INTO contacts (patient_id, type, name, last_name)
VALUES (1, 'Padre', 'Carlos', 'Gómez');

-- 15) Phones
INSERT INTO phones (contact_id, phone)
VALUES (1, '55512345');

-- 16) Exams
INSERT INTO exams (name, description)
VALUES ('Hemograma', 'Examen de sangre completo');

-- 17) Patient Exams
INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path)
VALUES (1, 1, 'Hemoglobina: 13 g/dL', '/results/hemograma1.pdf');

-- 18) Appointments
INSERT INTO appointments (patient_id, doctor_id, appointment_date, reason, status)
VALUES (1, 1, '2025-04-23 09:00:00', 'Chequeo rutinario', 'Confirmado');

-- 19) Diagnosis
INSERT INTO diagnosis (appointment_id, description, diagnosis_date)
VALUES (1, 'Sin anomalías', '2025-04-23 09:30:00');

-- 20) Medicines
INSERT INTO medicines (name, provider, type)
VALUES ('Ibuprofeno', 'Proveedor X', 'Analgésico');

-- 21) Treatments
INSERT INTO treatments (appointment_id, medicine_id, dosis, duration, frequency, observaciones, status)
VALUES (1, 1, '200mg', '5 días', 'Cada 8h', 'Tomar con alimentos', 'No Terminado');

-- 22) Recipes
INSERT INTO recipes (treatment_id, prescription)
VALUES (1, 'Ibuprofeno 200mg c/8h x 5 días');

-- 23) Insurance
INSERT INTO insurance (patient_id, provider_name, policy_number, coverage_details)
VALUES (1, 'Seguros ABC', 'POL12345', 'Cobertura completa');

-- 24) Vaccines
INSERT INTO vaccines (name, brand)
VALUES ('BCG', 'Laboratorios XYZ');

-- 25) Patient Vaccines
INSERT INTO patient_vaccines (patient_id, vaccine_id, dosis, age_at_application, application_date)
VALUES (1, 1, '0.1ml', 1, '2016-01-01');

