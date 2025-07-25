-- Insertar medical records extensos para pruebas
INSERT INTO medical_records (patient_id, weight, height, family_history, notes, created_at) VALUES
-- Paciente 1: Nye Futty (multiple records over time)
(1, 70.5, 175.2, 'Antecedentes familiares: Padre con diabetes tipo 2 a los 55 años, madre con hipertensión arterial. Abuelo paterno falleció por infarto agudo de miocardio a los 68 años. Hermana mayor con hipotiroidismo diagnosticado a los 35 años.', 
'Paciente refiere cuadro de cefalea frontal de moderada intensidad, de 3 días de evolución. Niega fiebre, náuseas o vómitos. Al examen físico: TA 120/80 mmHg, FC 72 lpm, FR 16 rpm, Temperatura 36.5°C. Paciente alerta, orientado, sin signos neurológicos focales. Se indica analgesia y seguimiento.', 
'2024-01-15 10:30:00'),

(1, 71.0, 175.2, 'Antecedentes familiares: Padre con diabetes tipo 2 a los 55 años, madre con hipertensión arterial. Abuelo paterno falleció por infarto agudo de miocardio a los 68 años. Hermana mayor con hipotiroidismo diagnosticado a los 35 años.', 
'Control rutinario. Paciente asintomático. Refiere adherencia al tratamiento indicado previamente. Al examen físico: TA 118/75 mmHg, FC 68 lpm, peso estable. Se solicitan exámenes de laboratorio de control: hemograma completo, glicemia, perfil lipídico, función renal y hepática. Continúa con medidas dietéticas y ejercicio regular.', 
'2024-03-20 14:15:00'),

(1, 70.8, 175.2, 'Antecedentes familiares: Padre con diabetes tipo 2 a los 55 años, madre con hipertensión arterial. Abuelo paterno falleció por infarto agudo de miocardio a los 68 años. Hermana mayor con hipotiroidismo diagnosticado a los 35 años.', 
'Paciente acude por dolor abdominal epigástrico de 2 días de evolución, tipo cólico, irradiado a hipocondrio derecho. Refiere náuseas sin vómitos. Niega fiebre. Al examen: abdomen blando, doloroso a la palpación en epigastrio e hipocondrio derecho, Murphy negativo. Se indica ecografía abdominal y tratamiento sintomático con IBPs y antiespasmódicos.', 
'2024-06-10 16:45:00'),

-- Paciente 2: Stanislaw Palinke (records with different specialties)
(2, 85.3, 180.5, 'Antecedentes familiares: Madre con cáncer de mama diagnosticado a los 60 años, actualmente en remisión. Padre hipertenso y dislipidémico. Tío materno con enfermedad renal crónica. Abuela materna diabética.', 
'Consulta por control de hipertensión arterial. Paciente en tratamiento con Enalapril 10mg c/12h desde hace 2 años. Refiere buena adherencia al tratamiento. TA promedio en casa 135/85 mmHg. Al examen: TA 140/90 mmHg, FC 75 lpm. Fondo de ojo: cambios hipertensivos grado I. Se ajusta dosis de IECA y se indica MAPA.', 
'2024-02-05 09:20:00'),

(2, 84.8, 180.5, 'Antecedentes familiares: Madre con cáncer de mama diagnosticado a los 60 años, actualmente en remisión. Padre hipertenso y dislipidémico. Tío materno con enfermedad renal crónica. Abuela materna diabética.', 
'Control cardiológico. Resultados de MAPA: TA promedio diurna 138/88 mmHg, nocturna 125/80 mmHg. No patrón dipper. ECG: ritmo sinusal, sin alteraciones. Ecocardiograma: función sistólica conservada, FEVI 60%, hipertrofia ventricular izquierda leve. Se optimiza tratamiento antihipertensivo agregando HCTZ.', 
'2024-04-12 11:30:00'),

-- Paciente 3: Gasparo Bromley (respiratory issues)
(3, 92.1, 177.0, 'Antecedentes familiares: Padre fumador, falleció por EPOC a los 70 años. Madre con asma bronquial. Hermano menor asmático. Tía paterna con cáncer pulmonar.', 
'Paciente fumador de 20 paquetes/año consulta por disnea de esfuerzo progresiva de 6 meses de evolución, tos productiva matinal con expectoración blanquecina. Al examen: murmullo pulmonar disminuido en bases, sibilancias espiratorias bilaterales. SpO2 94% aire ambiente. Se solicita espirometría, Rx tórax y gasometría arterial.', 
'2024-01-28 15:40:00'),

(3, 91.5, 177.0, 'Antecedentes familiares: Padre fumador, falleció por EPOC a los 70 años. Madre con asma bronquial. Hermano menor asmático. Tía paterna con cáncer pulmonar.', 
'Resultados de estudios: Espirometría con patrón obstructivo moderado, FEV1 65% del predicho. Rx tórax: hiperinsuflación pulmonar, sin consolidaciones. Se confirma diagnóstico de EPOC GOLD II. Se inicia tratamiento con broncodilatadores de acción prolongada y se enfatiza cesación tabáquica. Derivación a programa de rehabilitación pulmonar.', 
'2024-03-15 10:25:00'),

-- Paciente 4: Juan Gonzalez (diabetes management)
(4, 78.9, 172.3, 'Antecedentes familiares: Ambos padres diabéticos tipo 2. Hermana con diabetes gestacional. Abuelos paternos diabéticos. Tío paterno con complicaciones diabéticas (retinopatía y nefropatía).', 
'Paciente con diabetes tipo 2 de 5 años de evolución, en tratamiento con Metformina 850mg c/12h. Acude para control rutinario. Refiere poliuria y polidipsia de 2 semanas. Glicemia capilar: 280 mg/dl. Al examen: mucosas secas, signo del pliegue positivo. Se solicita HbA1c, función renal y se ajusta tratamiento antidiabético.', 
'2024-02-18 08:45:00'),

(4, 77.2, 172.3, 'Antecedentes familiares: Ambos padres diabéticos tipo 2. Hermana con diabetes gestacional. Abuelos paternos diabéticos. Tío paterno con complicaciones diabéticas (retinopatía y nefropatía).', 
'Control diabetológico. HbA1c: 8.9%. Función renal normal. Fondo de ojo: sin retinopatía diabética. Examen de pies: sensibilidad conservada, pulsos presentes. Se intensifica tratamiento agregando Gliclazida y se refuerza educación diabetológica. Control con nutricionista programado.', 
'2024-04-25 14:20:00'),

-- Paciente 5: Carlos Ramirez (multiple conditions)
(5, 95.6, 185.1, 'Antecedentes familiares: Padre con infarto agudo de miocardio a los 58 años. Madre con osteoporosis. Hermano con hipercolesterolemia. Abuelo paterno hipertenso.', 
'Paciente consulta por dolor precordial atípico de 1 semana de evolución, no relacionado con esfuerzo. Refiere estrés laboral reciente. Al examen: TA 150/95 mmHg, FC 88 lpm. ECG: ritmo sinusal, sin alteraciones isquémicas agudas. Se solicita perfil lipídico, troponinas seriadas y prueba de esfuerzo. Manejo inicial con ansiolíticos y seguimiento.', 
'2024-01-10 16:30:00'),

-- Más registros para otros pacientes...
(6, 68.4, 165.8, 'Antecedentes familiares: Madre con hipotiroidismo. Abuela materna con osteoporosis severa. Tía materna con fibromialgia.', 
'Paciente femenina de 35 años consulta por fatiga crónica de 3 meses de evolución, acompañada de caída del cabello y ganancia de peso (5 kg en 2 meses). Refiere intolerancia al frío y estreñimiento. Al examen: piel seca, bradilalia, reflejos osteotendinosos lentos. Se solicita perfil tiroideo completo (TSH, T4L, T3, anticuerpos antitiroideos).', 
'2024-03-08 11:15:00'),

(7, 73.2, 168.9, 'Antecedentes familiares: Sin antecedentes patológicos familiares relevantes.', 
'Control ginecológico anual. Paciente de 28 años, nulípara, ciclos menstruales regulares. Último PAP hace 1 año: negativo. Al examen: mamas sin nódulos palpables, examen pélvico normal. Se realiza PAP de control y se solicita ecografía mamaria y pélvica de rutina. Se refuerza importancia de autoexamen mamario mensual.', 
'2024-05-22 09:45:00'),

(8, 82.7, 176.4, 'Antecedentes familiares: Padre con artritis reumatoide. Madre con migraña crónica. Hermano con lumbalgia crónica.', 
'Paciente masculino de 42 años consulta por dolor lumbar crónico de 2 años de evolución, irradiado a miembro inferior derecho. Empeora con sedestación prolongada y mejora con reposo. Al examen: limitación para flexión lumbar, Lasègue positivo a 45° derecha. Se solicita RMN lumbar y se indica fisioterapia y analgesia multimodal.', 
'2024-04-07 13:50:00');

-- Insertar más appointments para tener más contexto
INSERT INTO appointments (patient_id, doctor_id, appointment_date, status, reason, created_at) VALUES
(1, 1, '2024-01-15 10:30:00', 'Completado', 'Control y evaluación de cefalea crónica', '2024-01-14 15:20:00'),
(1, 1, '2024-03-20 14:15:00', 'Completado', 'Control rutinario y solicitud de exámenes', '2024-03-19 10:30:00'),
(1, 2, '2024-06-10 16:45:00', 'Completado', 'Evaluación dolor abdominal', '2024-06-09 14:20:00'),
(2, 3, '2024-02-05 09:20:00', 'Completado', 'Control hipertensión arterial', '2024-02-04 16:45:00'),
(2, 3, '2024-04-12 11:30:00', 'Completado', 'Control cardiológico - resultados MAPA', '2024-04-11 09:15:00'),
(3, 4, '2024-01-28 15:40:00', 'Completado', 'Evaluación disnea y tos crónica', '2024-01-27 12:30:00'),
(3, 4, '2024-03-15 10:25:00', 'Completado', 'Control neumológico - resultados estudios', '2024-03-14 14:50:00'),
(4, 1, '2024-02-18 08:45:00', 'Completado', 'Control diabetes tipo 2', '2024-02-17 16:20:00'),
(4, 5, '2024-04-25 14:20:00', 'Completado', 'Control diabetológico especializado', '2024-04-24 11:30:00'),
(5, 3, '2024-01-10 16:30:00', 'Completado', 'Evaluación dolor precordial', '2024-01-09 13:45:00');

-- Insertar tratamientos asociados a estas citas
INSERT INTO treatments (appointment_id, medicine_id, dosis, duration, frequency, observaciones, status, created_at) VALUES
(11, 1, '500mg', '7 días', 'Cada 8 horas', 'Tomar con alimentos para evitar molestias gástricas', 'Terminado', '2024-01-15 10:45:00'),
(12, 2, '20mg', '30 días', '1 vez al día', 'Tomar en ayunas, control de función hepática en 1 mes', 'Terminado', '2024-03-20 14:30:00'),
(13, 3, '40mg', '14 días', 'Cada 12 horas', 'Omeprazol para protección gástrica', 'Terminado', '2024-06-10 17:00:00'),
(14, 4, '10mg', '30 días', 'Cada 12 horas', 'Control de tensión arterial semanal', 'Terminado', '2024-02-05 09:35:00'),
(15, 4, '15mg', '30 días', 'Cada 12 horas', 'Ajuste de dosis por TA elevada, agregar HCTZ', 'No Terminado', '2024-04-12 11:45:00'),
(16, 8, '2 puff', '30 días', 'Cada 12 horas', 'Broncodilatador de acción prolongada, técnica inhalatoria correcta', 'No Terminado', '2024-01-28 16:00:00'),
(17, 8, '2 puff', '60 días', 'Cada 12 horas', 'Continuar tratamiento, excelente respuesta clínica', 'No Terminado', '2024-03-15 10:40:00'),
(18, 6, '850mg', '30 días', 'Cada 12 horas', 'Ajuste de dosis, monitoreo de glicemia capilar', 'Terminado', '2024-02-18 09:00:00'),
(19, 7, '80mg', '30 días', '1 vez al día', 'Gliclazida de liberación prolongada, tomar con desayuno', 'No Terminado', '2024-04-25 14:35:00');

-- Insertar más exámenes de laboratorio
INSERT INTO patient_exams (patient_id, exam_id, result_text, created_at) VALUES
(1, 1, 'Hemoglobina: 14.2 g/dL (VN: 12-16), Leucocitos: 7800/mm3 (VN: 4000-10000), Plaquetas: 285000/mm3 (VN: 150000-450000). Hemograma dentro de parámetros normales.', '2024-03-21 08:30:00'),
(1, 2, 'Glicemia: 92 mg/dL (VN: 70-100), Colesterol total: 185 mg/dL (VN: <200), HDL: 48 mg/dL (VN: >40), LDL: 115 mg/dL (VN: <130), Triglicéridos: 110 mg/dL (VN: <150). Perfil lipídico adecuado.', '2024-03-21 08:30:00'),
(2, 3, 'Creatinina: 1.0 mg/dL (VN: 0.7-1.3), BUN: 18 mg/dL (VN: 7-20), Depuración creatinina: 95 mL/min (VN: >90). Función renal normal.', '2024-04-13 10:15:00'),
(3, 4, 'FEV1: 1.85L (65% predicho), FVC: 3.2L (78% predicho), FEV1/FVC: 58% (VN: >70%). Patrón obstructivo moderado compatible con EPOC.', '2024-03-16 14:20:00'),
(4, 5, 'HbA1c: 8.9% (VN: <7% en diabéticos), Glicemia: 245 mg/dL (VN: 70-100). Control diabético subóptimo, requiere intensificación de tratamiento.', '2024-04-26 09:45:00');