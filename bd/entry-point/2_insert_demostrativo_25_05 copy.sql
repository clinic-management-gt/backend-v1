-- ========================================
-- Script SQL Unificado y Ordenado
-- ========================================

-- ========================================
-- TENANTS
-- ========================================
-- 1) Tenants
INSERT INTO tenants (db_name, db_host, db_user, db_password)
VALUES 
  ('clinicaPediatrica', 'localhost', 'sudo', 'root');

-- ========================================
-- ROLES
-- ========================================
-- 2) Roles
INSERT INTO roles (type, name, can_edit)
VALUES 
  (1, 'Admin', TRUE),
  (2, 'Doctor', FALSE),
  (3, 'Secretaria', TRUE);

-- ========================================
-- MODULES
-- ========================================
-- 3) Modules
INSERT INTO modules (name, description)
VALUES 
  ('Pacientes', 'Gestión de pacientes');

-- ========================================
-- PERMISSIONS
-- ========================================
-- 4) Permissions
INSERT INTO permissions (role_id, module_id, can_view, can_edit, can_delete)
VALUES 
  (1, 1, TRUE, TRUE, TRUE);

-- ========================================
-- USERS
-- ========================================
-- 5) Demo Users

INSERT INTO users (first_name, last_name, email, role_id, password_hash, created_at) 
VALUES 
    ('Flor', 'Ramírez', 'flor.ramirez@example.com', 2, crypt('doctoraPass', gen_salt('bf')), NOW()),
    ('Nombre Secretaria', 'Apellido Secretaria', 'secretaria@example.com', 3, crypt('secretariaPass', gen_salt('bf')), NOW()),
    ('Ernesto', 'Ascencio', 'asc23009@uvg.edu.gt', 1, crypt('ernestoPass', gen_salt('bf')), NOW()),
    ('Esteban', 'Cárcamo', 'car23016@uvg.edu.gt', 1, crypt('estebanPass', gen_salt('bf')), NOW()),
    ('Nico', 'Concua', 'con23197@uvg.edu.gt', 1, crypt('nicoPass', gen_salt('bf')), NOW()),
    ('Hugo', 'Barillas', 'bar23556@uvg.edu.gt', 1, crypt('hugoPass', gen_salt('bf')), NOW());


-- ========================================
-- PATIENT_TYPES
-- ========================================
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

-- ========================================
-- BLOOD_TYPES
-- ========================================
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

-- ========================================
-- PATIENTS
-- ========================================
-- 8) Demo Patients
INSERT INTO patients (name, last_name, birthdate, address, gender, blood_type_id, patient_type_id)
VALUES
  ('María', 'Gómez', '2015-06-01', 'Calle Falsa 123', 'Female', 1, 1),
  ('Juan', 'Pérez', '1990-04-15', 'Av. Central 456', 'Male', 2, 2),
  ('Juan', 'González', '1992-03-24', 'Av. Flores 454', 'Male', 3, 1),
  ('Lucía', 'Fernández', '1985-11-23', 'Calle del Sol 789', 'Female', 3, 1),
  ('Carlos', 'Ramírez', '2000-02-10', 'Boulevard Norte 321', 'Male', 4, 2),
  ('Ana', 'Martínez', '2012-07-30', 'Callejón Luna 12', 'Female', 1, 1),
  ('Diego', 'López', '1978-09-12', 'Av. Las Rosas 555', 'Male', 5, 2),
  ('Sofía', 'Torres', '1995-01-05', 'Calle Primavera 77', 'Female', 6, 1),
  ('Miguel', 'Castro', '2003-12-22', 'Camino Real 888', 'Male', 7, 2),
  ('Valentina', 'Mendoza', '2018-03-18', 'Calle Sauce 303', 'Female', 2, 1),
  ('Andrés', 'Morales', '1980-06-29', 'Pasaje del Río 47', 'Male', 3, 2),
  ('Camila', 'Silva', '1999-10-09', 'Av. Libertad 910', 'Female', 4, 1),
  ('Anette', 'Legister', '1993-02-08 04:57:02', '72636 Dakota Park', 'Female', 6, 3),
  ('Karlie', 'Brook', '2013-06-18 13:00:09', '1061 Lindbergh Junction', 'Female', 7, 3),
  ('Dorotea', 'Pierce', '2010-06-10 22:55:07', '4030 Melvin Park', 'Female', 7, 7),
  ('Corbin', 'Orwell', '1998-01-11 07:53:33', '9 Springview Place', 'Male', 7, 2),
  ('Maribel', 'Emery', '2013-09-25 04:17:38', '601 Meadow Ridge Alley', 'Female', 7, 1),
  ('Jaquith', 'Woodwin', '2005-02-10 01:06:20', '1 Weeping Birch Circle', 'Female', 4, 2),
  ('Stefa', 'Fortesquieu', '2000-11-19 22:06:15', '4632 Novick Place', 'Female', 8, 2),
  ('Franz', 'Hancorn', '2004-12-06 02:39:46', '1 Stephen Pass', 'Male', 6, 1),
  ('Dane', 'Renbold', '2005-02-25 15:56:42', '9 Russell Parkway', 'Male', 1, 2),
  ('Wilmar', 'Upcraft', '2014-12-06 03:18:04', '1213 Esker Center', 'Male', 3, 3),
  ('Nye', 'Futty', '2012-05-23 22:43:02', '3747 Talmadge Circle', 'Male', 4, 3),
  ('Taryn', 'Laidler', '2017-11-06 01:00:49', '7588 Golf Course Parkway', 'Female', 5, 5),
  ('Janelle', 'Kun', '2013-01-15 17:10:44', '4 Golf View Parkway', 'Female', 6, 3),
  ('Arne', 'Streeting', '1992-02-05 00:17:24', '4045 Carey Avenue', 'Male', 6, 6),
  ('Orazio', 'Grioli', '2011-05-27 11:12:07', '227 Ryan Alley', 'Male', 1, 4),
  ('Shell', 'Ebenezer', '1994-11-02 21:27:54', '4173 Cambridge Avenue', 'Female', 6, 1),
  ('Shem', 'Maulin', '2001-08-08 11:18:28', '7 Clemons Hill', 'Male', 4, 7),
  ('Jakie', 'Giraudo', '2009-04-23 20:34:23', '2 Boyd Plaza', 'Male', 6, 6),
  ('Gilburt', 'Picott', '1998-06-27 04:02:31', '7146 Jay Hill', 'Male', 3, 3),
  ('Lyn', 'Oganesian', '2012-03-04 08:14:29', '4 Gulseth Trail', 'Female', 4, 2),
  ('Myrna', 'Scheffel', '2002-04-24 08:37:50', '8436 Prairieview Way', 'Female', 8, 3),
  ('Stanislaw', 'Kalinke', '1998-05-29 08:23:39', '703 Maywood Place', 'Male', 8, 2),
  ('Orren', 'Warhurst', '2011-09-19 13:38:44', '49 Westend Trail', 'Male', 8, 5),
  ('Sheffie', 'Ruller', '2002-12-06 06:54:32', '48 Montana Point', 'Male', 7, 8),
  ('Chuck', 'Reymers', '1999-05-27 16:10:59', '8555 Browning Avenue', 'Male', 2, 3),
  ('Jocelyne', 'Clayson', '2017-07-07 21:26:12', '1868 Judy Lane', 'Female', 1, 3),
  ('Zolly', 'Marre', '2010-07-21 21:32:06', '28 Swallow Point', 'Male', 4, 1),
  ('Urbano', 'Klimuk', '2005-08-16 22:12:21', '14 Morrow Center', 'Male', 1, 4),
  ('Helena', 'Kubach', '2015-02-22 00:42:17', '0 Kensington Pass', 'Female', 8, 2),
  ('Donni', 'Piller', '2014-06-13 08:13:05', '7041 Graceland Drive', 'Female', 2, 6),
  ('Tabby', 'Gallego', '1999-09-02 07:58:39', '26350 Pine View Park', 'Male', 5, 3),
  ('Dev', 'Patience', '1994-09-18 21:24:29', '46 Namekagon Drive', 'Male', 4, 7),
  ('Gasparo', 'Bromley', '1998-04-13 18:48:16', '69 Straubel Street', 'Male', 3, 1),
  ('Darrel', 'Ten Broek', '2003-06-19 10:54:42', '7327 Browning Plaza', 'Male', 5, 8),
  ('Addie', 'Hammonds', '2013-06-27 15:12:51', '00594 Ludington Pass', 'Male', 3, 7),
  ('Renaud', 'Kinzel', '2013-12-18 03:01:53', '2 Pankratz Hill', 'Male', 4, 2),
  ('Rori', 'Arnatt', '1993-01-02 19:33:14', '58 Anzinger Plaza', 'Female', 5, 8),
  ('Si', 'Geoghegan', '1992-11-28 16:37:51', '46 School Street', 'Male', 6, 1),
  ('Feodora', 'McGillecole', '2014-07-11 02:54:00', '9899 Old Gate Pass', 'Female', 6, 4),
  ('Pat', 'Comini', '1999-01-14 14:10:21', '31 Canary Point', 'Female', 3, 2),
  ('Candide', 'Gaiger', '1998-02-22 15:02:44', '2 Hollow Ridge Pass', 'Female', 5, 4),
  ('Winnifred', 'Waszczykowski', '1998-03-23 21:38:20', '69 Reindahl Center', 'Female', 3, 4),
  ('Octavius', 'Iashvili', '2001-05-27 11:52:44', '6619 Anthes Drive', 'Male', 6, 4),
  ('Idalia', 'Zammitt', '2008-05-03 16:06:16', '223 Gateway Street', 'Female', 3, 4),
  ('Sheffield', 'Caspell', '2002-08-16 13:37:23', '29951 Hauk Plaza', 'Male', 1, 3),
  ('Garrick', 'Jiruca', '2010-01-13 12:27:46', '9913 Troy Court', 'Male', 3, 3),
  ('Tiphanie', 'Tuddenham', '1996-05-12 06:24:45', '92619 Ridge Oak Drive', 'Female', 6, 2),
  ('Judye', 'Begwell', '2012-07-26 14:19:33', '2 Warner Trail', 'Female', 3, 4),
  ('Maury', 'Battman', '2010-11-23 04:26:19', '01712 Morning Circle', 'Male', 4, 5),
  ('Abbe', 'Laise', '2013-04-06 23:49:11', '7 Express Pass', 'Male', 6, 7),
  ('Britta', 'Tebbutt', '2014-08-17 10:14:29', '62008 Rowland Way', 'Female', 5, 5),
  ('Cassi', 'Sabatier', '1995-08-03 12:18:20', '0 Dunning Park', 'Female', 5, 3),
  ('Rahel', 'Kedslie', '2010-08-25 20:43:28', '7672 Oxford Hill', 'Female', 3, 5),
  ('Athene', 'Deverill', '2015-07-09 15:34:00', '8 Grasskamp Court', 'Female', 1, 5),
  ('Carney', 'Santhouse', '2014-04-07 21:16:53', '925 Transport Park', 'Male', 5, 5),
  ('Perla', 'Oakenfall', '1996-06-21 08:58:02', '0 Tennyson Plaza', 'Female', 3, 5),
  ('Adolph', 'Lambole', '1994-09-01 15:52:25', '24 Duke Road', 'Male', 4, 5),
  ('Cordey', 'Lagden', '1997-05-11 20:26:34', '44 Monument Court', 'Female', 2, 1),
  ('Jessalyn', 'Mose', '2015-08-17 16:15:47', '9 Beilfuss Way', 'Female', 1, 7),
  ('Kimbell', 'Gress', '2012-06-26 12:42:07', '7 Kinsman Point', 'Male', 3, 8),
  ('Olag', 'Juan', '1993-04-11 16:30:48', '80055 Arrowood Drive', 'Male', 1, 6),
  ('Earlie', 'Levesque', '2001-10-08 14:11:17', '284 Green Point', 'Male', 4, 3),
  ('Harp', 'Bartalini', '2012-07-24 07:14:55', '62 Northport Crossing', 'Male', 3, 4),
  ('Christopher', 'Shoppee', '1995-12-23 16:18:57', '3 Oak Parkway', 'Male', 5, 8),
  ('Sadie', 'Bohl', '2018-12-15 09:39:36', '8548 Vernon Parkway', 'Female', 8, 8),
  ('Niko', 'Meardon', '2020-01-28 23:00:47', '57 Merrick Park', 'Male', 4, 8),
  ('Daryn', 'Shelf', '2017-08-21 02:32:11', '46924 Northland Alley', 'Female', 5, 7),
  ('Neila', 'M''Quharg', '2020-03-09 18:33:33', '84 Nevada Drive', 'Female', 6, 6),
  ('Tyrus', 'Howship', '2012-01-08 20:23:31', '65945 Scofield Alley', 'Male', 4, 5),
  ('Bertie', 'Boulstridge', '1990-06-22 12:20:02', '28 Coleman Pass', 'Male', 1, 5),
  ('Jeremiah', 'Cowely', '2000-10-06 11:03:55', '9 Reinke Terrace', 'Male', 6, 3),
  ('Gladi', 'Holson', '2003-05-07 06:41:13', '2857 Lillian Point', 'Female', 2, 1),
  ('Kimble', 'Dedmam', '2014-05-08 10:39:22', '346 Warner Court', 'Male', 4, 3),
  ('Bart', 'McReath', '2016-04-03 12:23:50', '5437 Chive Terrace', 'Male', 1, 5),
  ('Thatcher', 'Braycotton', '1997-02-26 03:28:45', '89 Talisman Drive', 'Male', 1, 2),
  ('Reinald', 'Dickens', '2000-11-14 13:04:14', '4 Park Meadow Pass', 'Male', 8, 1),
  ('Carey', 'Luke', '2001-09-01 11:07:49', '101 Meadow Vale Avenue', 'Male', 1, 4),
  ('Tiebold', 'Hryniewicki', '2006-04-20 17:15:02', '0 Dorton Point', 'Male', 3, 2),
  ('Enriqueta', 'Blything', '1999-02-26 14:18:09', '89 Sullivan Way', 'Female', 6, 5),
  ('Rhona', 'Van der Daal', '1998-05-31 16:27:25', '1 Hanson Hill', 'Female', 2, 1),
  ('Lenka', 'Novic', '2011-12-20 10:09:18', '5905 Katie Center', 'Female', 1, 2),
  ('Myriam', 'Marl', '1994-01-30 15:00:10', '7 Lindbergh Road', 'Female', 1, 8),
  ('Nathanil', 'Cant', '2011-06-02 14:29:47', '8 Monument Parkway', 'Male', 6, 1),
  ('Frasco', 'Dudley', '2005-12-02 21:45:24', '9 Jana Street', 'Male', 2, 1),
  ('Eadie', 'Dumsday', '2015-03-27 15:13:36', '7 Schlimgen Terrace', 'Female', 1, 6),
  ('Antonia', 'Rigardeau', '2020-01-20 19:53:58', '4 Oakridge Court', 'Female', 2, 4),
  ('Murry', 'Kubica', '2004-12-12 18:06:31', '55 Hintze Drive', 'Male', 5, 1),
  ('Aarika', 'Franck', '2008-11-19 17:34:50', '1372 Atwood Terrace', 'Female', 2, 4),
  ('Trev', 'Devote', '2003-08-23 14:17:57', '0 Southridge Center', 'Male', 1, 2),
  ('Emanuel', 'Raleston', '1997-07-12 21:31:12', '18 Green Circle', 'Male', 2, 5),
  ('Thacher', 'Flacke', '2007-06-07 09:24:17', '3 Shelley Center', 'Male', 5, 6),
  ('Wanids', 'Deplacido', '1995-05-27 04:53:50', '90 4th Crossing', 'Female', 8, 5),
  ('Costanza', 'Scarlon', '1992-08-13 08:07:32', '313 Tony Alley', 'Female', 7, 1),
  ('Huntington', 'Ciccotto', '2012-03-10 04:20:46', '1 Sunfield Hill', 'Male', 6, 2),
  ('Patricia', 'Ionnisian', '2005-08-10 16:13:53', '45150 Johnson Alley', 'Female', 8, 2),
  ('Jarret', 'Simic', '2002-05-21 15:21:00', '437 Kenwood Terrace', 'Male', 8, 8),
  ('Vladamir', 'Galea', '2011-10-04 06:57:18', '1167 Parkside Way', 'Male', 5, 4),
  ('Earle', 'Iles', '1996-11-06 18:17:48', '5548 Westend Way', 'Male', 6, 7),
  ('Bradford', 'Gontier', '2003-10-29 19:33:01', '3092 Brown Point', 'Male', 8, 3),
  ('Whitney', 'Brunskill', '2007-05-10 11:10:29', '703 Clove Junction', 'Male', 6, 2);

-- ========================================
-- ALERGIES
-- ========================================
-- 9) Alergias
INSERT INTO alergies (alergy_code, alergy_description)
VALUES 
  ('ALG01', 'Alergia a polen');

-- ========================================
-- CHRONIC_DISEASES
-- ========================================
-- 10) Chronic Diseases
INSERT INTO chronic_diseases (disease_code, disease_description)
VALUES 
  ('CD01', 'Asma');

-- ========================================
-- PATIENT_ALERGIES
-- ========================================
-- 11) Patient Allergies
INSERT INTO patient_alergies (patient_id, alergy_id)
VALUES 
  (1, 1);

-- ========================================
-- PATIENT_CHRONIC_DISEASES
-- ========================================
-- 12) Patient Chronic Diseases
INSERT INTO patient_chronic_diseases (patient_id, chronic_disease_id)
VALUES 
  (1, 1);

-- ========================================
-- MEDICAL_RECORDS
-- ========================================
-- 13) Medical Records
INSERT INTO medical_records (patient_id, weight, height, family_history, notes)
VALUES 
  (1, 20.5, 1.15, 'Sin antecedentes familiares', 'Paciente en buen estado');

-- ========================================
-- CONTACTS
-- ========================================
-- 14) Contacts
INSERT INTO contacts (patient_id, type, name, last_name)
VALUES 
  (1, 'Padre', 'Carlos', 'Gómez');

-- ========================================
-- PHONES
-- ========================================
-- 15) Phones
INSERT INTO phones (contact_id, phone)
VALUES 
  (1, '55512345');

-- ========================================
-- EXAMS
-- ========================================
-- 16) Exams
INSERT INTO exams (name, description)
VALUES 
  ('Hemograma', 'Examen de sangre completo');

-- ========================================
-- PATIENT_EXAMS
-- ========================================
-- 17) Patient Exams
INSERT INTO patient_exams (patient_id, exam_id, result_text, result_file_path)
VALUES 
  (1, 1, 'Hemoglobina: 13 g/dL', '/results/hemograma1.pdf');

-- ========================================
-- APPOINTMENTS
-- ========================================
-- 18) Demo Appointments
INSERT INTO appointments (patient_id, doctor_id, appointment_date, reason, status)
VALUES
  (1, 1, '2025-04-23 09:00:00', 'Chequeo rutinario', 'Confirmado'),
  (2, 1, '2025-05-01 10:00:00', 'Consulta de seguimiento', 'Pendiente'),
  (47, 1, '2025-03-21 10:58:24', 'Donec ut dolor.', 'Cancelado'),
  (49, 1, '2024-08-02 10:40:37', 'Curabitur convallis. Duis consequat dui nec nisi volutpat eleifend. Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus.', 'Cancelado'),
  (48, 1, '2024-10-31 09:45:39', 'Pellentesque viverra pede ac diam. Cras pellentesque volutpat dui. Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam.', 'Espera'),
  (7, 1, '2024-09-08 03:15:41', 'Nulla tempus. Vivamus in felis eu sapien cursus vestibulum.', 'Espera'),
  (26, 1, '2024-11-04 22:26:29', 'Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.', 'Espera'),
  (39, 1, '2024-06-09 15:27:22', 'Integer ac neque.', 'Espera'),
  (3, 1, '2024-05-17 12:35:37', 'Morbi non quam nec dui luctus rutrum.', 'Confirmado'),
  (8, 1, '2024-09-11 05:07:22', 'Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi.', 'Cancelado'),
  (3, 1, '2025-04-29 01:03:14', 'Aenean lectus. Pellentesque eget nunc. Donec quis orci eget orci vehicula condimentum.', 'Confirmado'),
  (49, 1, '2024-07-26 21:22:24', 'In congue.', 'Cancelado'),
  (43, 1, '2024-09-24 04:30:47', 'Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam. Suspendisse potenti. Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum.', 'Cancelado'),
  (9, 1, '2024-07-10 00:23:14', 'Duis mattis egestas metus. Aenean fermentum.', 'Completado'),
  (3, 1, '2024-08-07 15:22:01', 'Morbi odio odio, elementum eu, interdum eu, tincidunt in, leo. Maecenas pulvinar lobortis est. Phasellus sit amet erat. Nulla tempus.', 'Espera'),
  (6, 1, '2025-05-06 03:14:50', 'Integer a nibh.', 'Espera'),
  (48, 1, '2024-11-25 06:56:37', 'Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst.', 'Confirmado'),
  (25, 1, '2024-12-26 20:39:36', 'Sed ante. Vivamus tortor.', 'Pendiente'),
  (11, 1, '2024-05-31 00:29:19', 'Aliquam sit amet diam in magna bibendum imperdiet.', 'Espera'),
  (7, 1, '2024-10-14 06:09:19', 'Etiam pretium iaculis justo.', 'Confirmado'),
  (25, 1, '2024-12-21 19:14:39', 'In blandit ultrices enim. Lorem ipsum dolor sit amet.', 'Pendiente'),
  (21, 1, '2024-11-29 16:03:42', 'Nunc nisl. Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi.', 'Confirmado'),
  (2, 1, '2025-04-13 00:22:06', 'Curabitur at ipsum ac tellus semper interdum.', 'Espera'),
  (90, 1, '2025-05-04 22:18:51', 'Nam nulla. Integer pede justo, lacinia eget, tincidunt eget.', 'Cancelado'),
  (42, 1, '2025-03-22 16:11:53', 'Nunc purus. Phasellus in felis. Donec semper sapien a libero.', 'Espera'),
  (6, 1, '2025-03-21 00:03:04', 'Vestibulum quam sapien, varius ut, blandit non, interdum in, ante.', 'Confirmado'),
  (49, 1, '2025-04-08 22:57:12', 'Nulla nisl. Nunc nisl. Duis bibendum, felis sed interdum venenatis.', 'Espera'),
  (41, 1 , '2024-12-03 19:33:32', 'Proin risus. Praesent lectus. Vestibulum quam sapien, varius ut, blandit non, interdum in, ante.', 'Confirmado'),
  (43, 1, '2024-10-30 11:01:18', 'Nullam molestie nibh in lectus. Pellentesque at nulla. Suspendisse potenti.', 'Pendiente'),
  (5, 1, '2025-01-07 16:02:09', 'In blandit ultrices enim. Lorem ipsum dolor sit amet.', 'Completado'),
  (5, 1, '2025-02-13 06:31:07', 'Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio.', 'Espera'),
  (19, 1, '2025-02-13 10:26:47', 'Nullam varius. Nulla facilisi. Cras non velit nec nisi vulputate nonummy.', 'Espera'),
  (68, 1, '2024-12-08 09:30:36', 'Pellentesque eget nunc. Donec quis orci eget orci vehicula condimentum.', 'Confirmado'),
  (96, 1, '2024-10-01 23:31:59', 'Vestibulum rutrum rutrum neque. Aenean auctor gravida sem.', 'Confirmado'),
  (51, 1, '2024-12-07 09:12:33', 'Duis mattis egestas metus. Aenean fermentum. Donec ut mauris.', 'Espera'),
  (92, 1, '2024-11-25 16:16:51', 'Nunc nisl. Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi.', 'Completado'),
  (38, 1, '2025-01-11 14:08:39', 'Mauris ullamcorper purus sit amet nulla.', 'Completado'),
    (1, 1, NOW(), 'Chequeo general', 'Confirmado'),
    (3, 1, NOW(), 'Control de vacunas', 'Cancelado'),
    (5, 1, NOW(), 'Consulta nutricional', 'Confirmado'),
    (2, 1, NOW(), 'Revisión de resultados', 'Espera'),
    (6, 1, NOW(), 'Dolor de garganta', 'Pendiente'),
    (4, 1, NOW(), 'Examen de sangre', 'Confirmado'),
    (7, 1, NOW(), 'Control de crecimiento', 'Confirmado'),
    (8, 1, NOW(), 'Consulta de seguimiento', 'Pendiente'),
    (20, 1, NOW(), 'Problemas dermatológicos', 'Espera'),
    (10, 1, NOW(), 'Evaluación psicomotriz', 'Confirmado'),
    (45, 1, NOW() + INTERVAL '1 day', 'Problemas dermatológicos', 'Confirmado'),
    (23, 1, NOW() + INTERVAL '1 day', 'Problemas dermatológicos', 'Espera'),
    (34, 1, NOW() + INTERVAL '1 day', 'Problemas dermatológicos', 'Confirmado'),
    (3, 1, NOW() + INTERVAL '3 day', 'Problemas dermatológicos', 'Espera');

-- ========================================
-- DIAGNOSIS
-- ========================================
-- 19) Diagnosis
INSERT INTO diagnosis (appointment_id, description, diagnosis_date)
VALUES 
  (1, 'Sin anomalías', '2025-04-23 09:30:00');

-- ========================================
-- MEDICINES
-- ========================================
-- 20) Medicines
INSERT INTO medicines (name, provider, type)
VALUES 
  ('Ibuprofeno', 'Proveedor X', 'Analgésico');

-- ========================================
-- TREATMENTS
-- ========================================
-- 21) Treatments
INSERT INTO treatments (appointment_id, medicine_id, dosis, duration, frequency, observaciones, status)
VALUES 
  (1, 1, '200mg', '5 días', 'Cada 8h', 'Tomar con alimentos', 'No Terminado'),
  (2, 1, '500mg', '7 días', 'Cada 12h', 'Tomar después de las comidas', 'No Terminado');

-- ========================================
-- RECIPES
-- ========================================
-- 22) Recipes
INSERT INTO recipes (treatment_id, prescription)
VALUES 
  (1, 'Ibuprofeno 200mg c/8h x 5 días'),
  (2, 'Amoxicilina 500mg c/12h x 7 días');

-- ========================================
-- INSURANCE
-- ========================================
-- 23) Insurance
INSERT INTO insurance (patient_id, provider_name, policy_number, coverage_details)
VALUES 
  (1, 'Seguros ABC', 'POL12345', 'Cobertura completa');

-- ========================================
-- VACCINES
-- ========================================
-- 24) Vaccines
INSERT INTO vaccines (name, brand)
VALUES 
  ('BCG', 'Laboratorios XYZ');

-- ========================================
-- PATIENT_VACCINES
-- ========================================
-- 25) Patient Vaccines
INSERT INTO patient_vaccines (patient_id, vaccine_id, dosis, age_at_application, application_date)
VALUES 
  (1, 1, '0.1ml', 1, '2016-01-01');
