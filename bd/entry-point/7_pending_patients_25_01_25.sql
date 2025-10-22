-- ===================================================================
-- 7) PENDING PATIENTS TABLES
-- ===================================================================
-- Script para crear las tablas de pacientes pendientes de confirmar
-- Fecha: 25/01/2025

CREATE TABLE pending_patients (
  id               SERIAL PRIMARY KEY,
  name             VARCHAR(50)    NOT NULL,
  last_name        VARCHAR(50)    NOT NULL,
  birthdate        DATE           NOT NULL,
  gender           VARCHAR(50)    NOT NULL,
  created_at       TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at       TIMESTAMP WITH TIME ZONE
);

CREATE TABLE pending_patient_contacts (
  id                    SERIAL PRIMARY KEY,
  pending_patient_id    INT NOT NULL REFERENCES pending_patients(id) ON DELETE CASCADE,
  type                  VARCHAR(20) NOT NULL,
  name                  VARCHAR(255) NOT NULL,
  last_name             VARCHAR(255) NOT NULL,
  created_at            TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at            TIMESTAMP WITH TIME ZONE
);

CREATE TABLE pending_patient_phones (
  id                            SERIAL PRIMARY KEY,
  pending_patient_contact_id    INT NOT NULL REFERENCES pending_patient_contacts(id) ON DELETE CASCADE,
  phone                         VARCHAR(15) NOT NULL,
  created_at                    TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at                    TIMESTAMP WITH TIME ZONE
);

CREATE TABLE pending_patient_emails (
  id                            SERIAL PRIMARY KEY,
  pending_patient_contact_id    INT NOT NULL REFERENCES pending_patient_contacts(id) ON DELETE CASCADE,
  email                         VARCHAR(255) NOT NULL,
  created_at                    TIMESTAMP WITH TIME ZONE DEFAULT now(),
  updated_at                    TIMESTAMP WITH TIME ZONE
);

-- Índices para mejorar el rendimiento
CREATE INDEX idx_pending_patients_name ON pending_patients(name);
CREATE INDEX idx_pending_patients_last_name ON pending_patients(last_name);
CREATE INDEX idx_pending_patient_contacts_patient_id ON pending_patient_contacts(pending_patient_id);
CREATE INDEX idx_pending_patient_phones_contact_id ON pending_patient_phones(pending_patient_contact_id);
CREATE INDEX idx_pending_patient_emails_contact_id ON pending_patient_emails(pending_patient_contact_id);

-- Comentarios en las tablas
COMMENT ON TABLE pending_patients IS 'Pacientes que han agendado citas pero aún no han confirmado su información completa';
COMMENT ON TABLE pending_patient_contacts IS 'Contactos de emergencia para pacientes pendientes';
COMMENT ON TABLE pending_patient_phones IS 'Teléfonos de los contactos de pacientes pendientes';
COMMENT ON TABLE pending_patient_emails IS 'Emails de los contactos de pacientes pendientes';
