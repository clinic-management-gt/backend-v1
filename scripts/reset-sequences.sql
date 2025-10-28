-- ========================================
-- SCRIPT: Reset PostgreSQL Sequences
-- ========================================
-- Este script resetea todas las secuencias de autoincremento
-- para evitar conflictos de "duplicate key" después del seeding
-- o restauración de backups.
--
-- CUÁNDO USAR:
-- - Después de ejecutar seeders con IDs explícitos
-- - Después de restaurar un backup de la base de datos
-- - Cuando obtengas errores de "duplicate key value violates unique constraint"
--
-- CÓMO EJECUTAR:
-- docker exec -i backend-v1-db-1 psql -U escu -d clinicaPediatrica < scripts/reset-sequences.sql
-- ========================================

\echo '🔧 Resetting database sequences...'
\echo ''

-- Appointments
SELECT setval(
    pg_get_serial_sequence('appointments', 'id'),
    COALESCE((SELECT MAX(id) FROM appointments), 0) + 1,
    false
);
\echo '✓ Reset sequence for appointments'

-- Diagnoses
SELECT setval(
    pg_get_serial_sequence('diagnoses', 'id'),
    COALESCE((SELECT MAX(id) FROM diagnoses), 0) + 1,
    false
);
\echo '✓ Reset sequence for diagnoses'

-- Treatments
SELECT setval(
    pg_get_serial_sequence('treatments', 'id'),
    COALESCE((SELECT MAX(id) FROM treatments), 0) + 1,
    false
);
\echo '✓ Reset sequence for treatments'

-- Recipes
SELECT setval(
    pg_get_serial_sequence('recipes', 'id'),
    COALESCE((SELECT MAX(id) FROM recipes), 0) + 1,
    false
);
\echo '✓ Reset sequence for recipes'

-- Patients
SELECT setval(
    pg_get_serial_sequence('patients', 'id'),
    COALESCE((SELECT MAX(id) FROM patients), 0) + 1,
    false
);
\echo '✓ Reset sequence for patients'

-- Patient Contacts
SELECT setval(
    pg_get_serial_sequence('patient_contacts', 'id'),
    COALESCE((SELECT MAX(id) FROM patient_contacts), 0) + 1,
    false
);
\echo '✓ Reset sequence for patient_contacts'

-- Patient Phones
SELECT setval(
    pg_get_serial_sequence('patient_phones', 'id'),
    COALESCE((SELECT MAX(id) FROM patient_phones), 0) + 1,
    false
);
\echo '✓ Reset sequence for patient_phones'

-- Patient Emails
SELECT setval(
    pg_get_serial_sequence('patient_emails', 'id'),
    COALESCE((SELECT MAX(id) FROM patient_emails), 0) + 1,
    false
);
\echo '✓ Reset sequence for patient_emails'

-- Medical Records
SELECT setval(
    pg_get_serial_sequence('medical_records', 'id'),
    COALESCE((SELECT MAX(id) FROM medical_records), 0) + 1,
    false
);
\echo '✓ Reset sequence for medical_records'

-- Alergies
SELECT setval(
    pg_get_serial_sequence('alergies', 'id'),
    COALESCE((SELECT MAX(id) FROM alergies), 0) + 1,
    false
);
\echo '✓ Reset sequence for alergies'

-- Syndromes
SELECT setval(
    pg_get_serial_sequence('syndromes', 'id'),
    COALESCE((SELECT MAX(id) FROM syndromes), 0) + 1,
    false
);
\echo '✓ Reset sequence for syndromes'

-- Vaccines
SELECT setval(
    pg_get_serial_sequence('vaccines', 'id'),
    COALESCE((SELECT MAX(id) FROM vaccines), 0) + 1,
    false
);
\echo '✓ Reset sequence for vaccines'

-- Chronic Diseases
SELECT setval(
    pg_get_serial_sequence('chronic_diseases', 'id'),
    COALESCE((SELECT MAX(id) FROM chronic_diseases), 0) + 1,
    false
);
\echo '✓ Reset sequence for chronic_diseases'

-- Pending Patients
SELECT setval(
    pg_get_serial_sequence('pending_patients', 'id'),
    COALESCE((SELECT MAX(id) FROM pending_patients), 0) + 1,
    false
);
\echo '✓ Reset sequence for pending_patients'

-- Pending Patient Contacts
SELECT setval(
    pg_get_serial_sequence('pending_patient_contacts', 'id'),
    COALESCE((SELECT MAX(id) FROM pending_patient_contacts), 0) + 1,
    false
);
\echo '✓ Reset sequence for pending_patient_contacts'

-- Pending Patient Phones
SELECT setval(
    pg_get_serial_sequence('pending_patient_phones', 'id'),
    COALESCE((SELECT MAX(id) FROM pending_patient_phones), 0) + 1,
    false
);
\echo '✓ Reset sequence for pending_patient_phones'

\echo ''
\echo '✅ All sequences have been reset successfully!'
\echo '📊 Next IDs will be generated correctly'
