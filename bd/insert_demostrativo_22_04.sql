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
VALUES (1, 'Regular', 'Paciente pediátrico general');

-- 7) Blood Types
INSERT INTO blood_types (id, type, description)
VALUES (1, 'A+', 'Tipo A positivo');

-- 8) Patients
INSERT INTO patients (id, name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES (1, 'María', 'Gómez', '2015-06-01', 'Calle Falsa 123', 'Female', 1, 1);

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
