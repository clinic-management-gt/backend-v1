INSERT INTO appointments (
  patient_id,
  doctor_id,
  appointment_date,
  reason,
  status,
  created_at
) VALUES
  (1,  1, (CURRENT_DATE || ' 08:00:00')::timestamp, 'Chequeo general',         'Pendiente', NOW()),
  (3,  1, (CURRENT_DATE || ' 09:30:00')::timestamp, 'Revisión de vacunas',     'Confirmado', NOW()),
  (5,  1, (CURRENT_DATE || ' 10:15:00')::timestamp, 'Consulta dental',         'Pendiente', NOW()),
  (7,  1, (CURRENT_DATE || ' 11:00:00')::timestamp, 'Control de tratamiento',  'Confirmado', NOW()),
  (9,  1, (CURRENT_DATE || ' 12:45:00')::timestamp, 'Revisión de exámenes',    'Pendiente', NOW()),
  (11, 1, (CURRENT_DATE || ' 14:00:00')::timestamp, 'Consulta de seguimiento', 'Confirmado', NOW()),
  (13, 1, (CURRENT_DATE || ' 15:30:00')::timestamp, 'Evaluación neurológica',  'Pendiente', NOW()),
  (15, 1, (CURRENT_DATE || ' 16:15:00')::timestamp, 'Odontología pediátrica',  'Confirmado', NOW()),
  (17, 1, (CURRENT_DATE || ' 17:00:00')::timestamp, 'Asesoría nutricional',     'Pendiente', NOW()),
  (19, 1, (CURRENT_DATE || ' 18:30:00')::timestamp, 'Terapia respiratoria',    'Confirmado', NOW());
