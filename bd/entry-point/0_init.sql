CREATE EXTENSION IF NOT EXISTS unaccent;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- ===================================================================
-- 1) ENUM TYPES
-- ===================================================================
CREATE TYPE appointment_status_enum   AS ENUM ('Confirmado','Pendiente','Completado','Cancelado', 'Espera');
CREATE TYPE treatment_status_enum     AS ENUM ('Terminado','No Terminado');
CREATE TYPE log_action_enum           AS ENUM ('INSERT','UPDATE','DELETE');

-- ===================================================================
-- 2) TABLES (con DEFAULT now() en created_at)
-- ===================================================================

CREATE TABLE tenants (
  id           SERIAL PRIMARY KEY,
  db_name      VARCHAR(50)  NOT NULL,
  db_host      TEXT         NOT NULL,
  db_user      VARCHAR(50)  NOT NULL,
  db_password  TEXT         NOT NULL,
  created_at   TIMESTAMP    DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE roles (
  id           SERIAL PRIMARY KEY,
  type         INT          NOT NULL,
  name         VARCHAR(50)  NOT NULL,
  can_edit     BOOLEAN      DEFAULT FALSE,
  created_at   TIMESTAMP    DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE modules (
  id           SERIAL PRIMARY KEY,
  name         VARCHAR(50)  NOT NULL,
  description  TEXT,
  created_at   TIMESTAMP    DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE permissions (
  id           SERIAL PRIMARY KEY,
  role_id      INT          NOT NULL REFERENCES roles(id),
  module_id    INT          NOT NULL REFERENCES modules(id),
  can_view     BOOLEAN      DEFAULT FALSE,
  can_edit     BOOLEAN      DEFAULT FALSE,
  can_delete   BOOLEAN      DEFAULT FALSE,
  created_at   TIMESTAMP    DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE users (
  id           SERIAL PRIMARY KEY,
  first_name   VARCHAR(50)  NOT NULL,
  last_name    VARCHAR(50)  NOT NULL,
  email        VARCHAR(100),
  role_id      INT          NOT NULL REFERENCES roles(id),
  password_hash  TEXT        NOT NULL,         
  created_at   TIMESTAMP    DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE patient_types (
  id           SERIAL PRIMARY KEY,
  name         VARCHAR(50)  NOT NULL,
  description  TEXT
);

CREATE TABLE blood_types (
  id           SERIAL PRIMARY KEY,
  type         VARCHAR(5)   NOT NULL,
  description  VARCHAR(50)
);

CREATE TABLE patients (
  id               SERIAL PRIMARY KEY,
  name             VARCHAR(50)    NOT NULL,
  last_name        VARCHAR(50)    NOT NULL,
  birthdate        DATE           NOT NULL,
  address          TEXT           NOT NULL,
  gender           VARCHAR(50)    NOT NULL,
  blood_type_id    INT            NOT NULL REFERENCES blood_types(id),
  patient_type_id  INT            NOT NULL REFERENCES patient_types(id),
  created_at       TIMESTAMP      DEFAULT now(),
  updated_at       TIMESTAMP
);

CREATE TABLE alergies (
  id               SERIAL PRIMARY KEY,
  alergy_code      VARCHAR(10)  NOT NULL,
  alergy_description TEXT
);

CREATE TABLE chronic_diseases (
  id                  SERIAL PRIMARY KEY,
  disease_code        VARCHAR(10) NOT NULL,
  disease_description TEXT
);

CREATE TABLE patient_alergies (
  id           SERIAL PRIMARY KEY,
  patient_id   INT NOT NULL REFERENCES patients(id),
  alergy_id    INT NOT NULL REFERENCES alergies(id)
);

CREATE TABLE patient_chronic_diseases (
  id                 SERIAL PRIMARY KEY,
  patient_id         INT NOT NULL REFERENCES patients(id),
  chronic_disease_id INT NOT NULL REFERENCES chronic_diseases(id)
);

CREATE TABLE medical_records (
  id               SERIAL PRIMARY KEY,
  patient_id       INT  NOT NULL REFERENCES patients(id),
  weight           DECIMAL(5,2),
  height           DECIMAL(5,2),
  family_history   TEXT,
  notes            TEXT,
  created_at       TIMESTAMP DEFAULT now(),
  updated_at       TIMESTAMP
);

CREATE TABLE contacts (
  id           SERIAL PRIMARY KEY,
  patient_id   INT NOT NULL REFERENCES patients(id),
  type         VARCHAR(20) NOT NULL,
  name         VARCHAR(255) NOT NULL,
  last_name    VARCHAR(255) NOT NULL,
  created_at   TIMESTAMP DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE phones (
  id           SERIAL PRIMARY KEY,
  contact_id   INT NOT NULL REFERENCES contacts(id),
  phone        VARCHAR(15) NOT NULL,
  created_at   TIMESTAMP DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE appointments (
  id               SERIAL PRIMARY KEY,
  patient_id       INT       NOT NULL REFERENCES patients(id),
  doctor_id        INT       NOT NULL REFERENCES users(id),
  appointment_date TIMESTAMP NOT NULL,
  reason           TEXT,
  status           appointment_status_enum NOT NULL,
  created_at       TIMESTAMP DEFAULT now(),
  updated_at       TIMESTAMP
);

CREATE TABLE exams (
  id           SERIAL PRIMARY KEY,
  name         VARCHAR(50) NOT NULL,
  description  TEXT
);

CREATE TABLE patient_exams (
  id               SERIAL PRIMARY KEY,
  patient_id       INT NOT NULL REFERENCES patients(id),
  exam_id          INT NOT NULL REFERENCES exams(id),
  result_text      TEXT,
  result_file_path VARCHAR(255),
  created_at       TIMESTAMP DEFAULT now()
);

CREATE TABLE diagnosis (
  id               SERIAL PRIMARY KEY,
  appointment_id   INT NOT NULL REFERENCES appointments(id),
  description      TEXT,
  diagnosis_date   TIMESTAMP,
  created_at       TIMESTAMP DEFAULT now()
);

CREATE TABLE medicines (
  id           SERIAL PRIMARY KEY,
  name         VARCHAR(100) NOT NULL,
  provider     VARCHAR(100),
  type         VARCHAR(255) NOT NULL,
  created_at   TIMESTAMP DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE treatments (
  id               SERIAL PRIMARY KEY,
  appointment_id   INT NOT NULL REFERENCES appointments(id),
  medicine_id      INT NOT NULL REFERENCES medicines(id),
  dosis            TEXT    NOT NULL,
  duration         VARCHAR(50) NOT NULL,
  frequency        VARCHAR(50),
  observaciones    TEXT,
  status           treatment_status_enum NOT NULL,
  created_at       TIMESTAMP DEFAULT now(),
  updated_at       TIMESTAMP
);

CREATE TABLE recipes (
  id           SERIAL PRIMARY KEY,
  treatment_id INT NOT NULL REFERENCES treatments(id),
  prescription TEXT,
  created_at   TIMESTAMP DEFAULT now()
);

CREATE TABLE insurance (
  id               SERIAL PRIMARY KEY,
  patient_id       INT NOT NULL REFERENCES patients(id),
  provider_name    VARCHAR(100) NOT NULL,
  policy_number    VARCHAR(50)  NOT NULL,
  coverage_details TEXT,
  created_at       TIMESTAMP DEFAULT now(),
  updated_at       TIMESTAMP
);

CREATE TABLE logs (
  id           SERIAL PRIMARY KEY,
  table_name   VARCHAR(50) NOT NULL,
  action       log_action_enum NOT NULL,
  changed_at   TIMESTAMP   NOT NULL,
  user_id      INT         REFERENCES users(id),
  old_data     JSON,
  new_data     JSON
);

CREATE TABLE history (
  id           SERIAL PRIMARY KEY,
  table_name   VARCHAR(50) NOT NULL,
  record_id    INT         NOT NULL,
  action       log_action_enum NOT NULL,
  changed_at   TIMESTAMP   NOT NULL,
  old_data     JSON,
  new_data     JSON
);

CREATE TABLE vaccines (
  id           SERIAL PRIMARY KEY,
  name         VARCHAR(100) NOT NULL,
  brand        VARCHAR(100) NOT NULL,
  created_at   TIMESTAMP DEFAULT now(),
  updated_at   TIMESTAMP
);

CREATE TABLE patient_vaccines (
  id               SERIAL PRIMARY KEY,
  patient_id       INT NOT NULL REFERENCES patients(id),
  vaccine_id       INT NOT NULL REFERENCES vaccines(id),
  dosis            VARCHAR(50),
  age_at_application INT,
  application_date DATE,
  created_at       TIMESTAMP DEFAULT now()
);

