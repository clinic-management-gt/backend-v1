-- Notas de evolución detalladas para seguimiento de pacientes
INSERT INTO medical_records (patient_id, weight, height, family_history, notes, created_at) VALUES

-- Evoluciones del paciente con EPOC
(3, 90.8, 177.0, 'Antecedentes familiares: Padre fumador, falleció por EPOC a los 70 años. Madre con asma bronquial. Hermano menor asmático. Tía paterna con cáncer pulmonar.', 
'NOTA DE EVOLUCIÓN - Control a los 30 días: Paciente refiere mejoría parcial de la disnea con el tratamiento iniciado. Ha logrado cesación tabáquica hace 15 días con apoyo psicológico. Persiste tos matinal pero con menor expectoración. Al examen: SpO2 96% aire ambiente (previo 94%), murmullo pulmonar mejor ventilado. Continúa con broncodilatadores y se programa espirometría de control en 3 meses.', 
'2024-04-28 11:20:00'),

-- Evolución del paciente diabético
(4, 76.5, 172.3, 'Antecedentes familiares: Ambos padres diabéticos tipo 2. Hermana con diabetes gestacional. Abuelos paternos diabéticos. Tío paterno con complicaciones diabéticas (retinopatía y nefropatía).', 
'NOTA DE EVOLUCIÓN - Control a los 15 días: Paciente trae registro de glicemias capilares: promedio 180 mg/dL (rango 140-220). Refiere adherencia al nuevo esquema terapéutico. Pérdida de peso de 1.7 kg. Al examen: mucosas hidratadas, sin signos de descompensación. Se mantiene tratamiento actual y se programa HbA1c de control en 6 semanas. Continúa seguimiento nutricional.', 
'2024-05-10 16:40:00'),

-- Evolución del paciente hipertenso
(2, 84.2, 180.5, 'Antecedentes familiares: Madre con cáncer de mama. Padre hipertenso y dislipidémico. Tío materno con enfermedad renal crónica. Abuela materna diabética.', 
'NOTA DE EVOLUCIÓN - Control a los 21 días: Paciente trae registro de TA domiciliaria: promedio 128/82 mmHg. Excelente respuesta al ajuste de tratamiento antihipertensivo. Niega efectos adversos medicamentosos. Al examen: TA 125/80 mmHg, FC 68 lpm. Edemas maleolares leves. Se mantiene esquema actual y se programa nuevo MAPA en 3 meses para evaluar control sostenido.', 
'2024-05-03 09:15:00'),

-- Nota de interconsulta - Cardiología
(5, 94.8, 185.1, 'Antecedentes familiares: Padre con infarto agudo de miocardio a los 58 años. Madre con osteoporosis. Hermano con hipercolesterolemia. Abuelo paterno hipertenso.', 
'INTERCONSULTA CARDIOLOGÍA - Evaluación de dolor precordial: Paciente completó prueba de esfuerzo que resultó negativa para isquemia. Ecocardiograma: función sistólica conservada, FEVI 65%, sin alteraciones segmentarias. Perfil lipídico: CT 245 mg/dL, LDL 165 mg/dL. Se descarta síndrome coronario agudo. Dolor probablemente de origen músculo-esquelético/ansioso. Se indica estatina por dislipidemia e inicio de programa de ejercicio supervisado.', 
'2024-02-14 15:30:00'),

-- Nota de seguimiento endocrinológico
(6, 70.2, 165.8, 'Antecedentes familiares: Madre con hipotiroidismo. Abuela materna con osteoporosis severa. Tía materna con fibromialgia.', 
'SEGUIMIENTO ENDOCRINOLÓGICO - Resultados función tiroidea: TSH 12.8 mUI/L (VN: 0.4-4.0), T4L 0.8 ng/dL (VN: 0.9-1.7), Anti-TPO 156 UI/mL (VN: <34). Diagnóstico: Hipotiroidismo primario autoinmune (Tiroiditis de Hashimoto). Se inicia levotiroxina 75 mcg/día en ayunas. Control de función tiroidea en 6-8 semanas. Se explica cronicidad del cuadro y necesidad de tratamiento de por vida.', 
'2024-04-05 10:45:00'),

-- Nota de control ginecológico
(7, 72.8, 168.9, 'Antecedentes familiares: Sin antecedentes patológicos familiares relevantes.', 
'CONTROL GINECOLÓGICO - Resultados de estudios: PAP: Negativo para lesión intraepitelial. Ecografía mamaria: Tejido fibroglandular heterogéneo, sin nódulos sospechosos. Ecografía pélvica: Útero y ovarios de características normales, endometrio 8mm (fase proliferativa). Se programa próximo control en 1 año. Se refuerza educación sobre anticoncepción y planificación familiar.', 
'2024-06-15 14:20:00'),

-- Nota de seguimiento traumatológico
(8, 81.9, 176.4, 'Antecedentes familiares: Padre con artritis reumatoide. Madre con migraña crónica. Hermano con lumbalgia crónica.', 
'SEGUIMIENTO TRAUMATOLOGÍA - RMN lumbar: Protrusión discal L4-L5 con contacto radicular S1 derecha. Leve estenosis del receso lateral. Sin signos de compresión medular. Paciente completó 8 sesiones de fisioterapia con mejoría parcial del dolor (EVA 8/10 → 5/10). Persiste parestesias en dermatoma S1. Se continúa con fisioterapia y se considera infiltración epidural si no hay mejoría en 4 semanas.', 
'2024-05-18 16:10:00');