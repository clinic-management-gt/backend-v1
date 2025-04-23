-- 2.1. Secuencias para IDs automáticos en logs y history
CREATE SEQUENCE IF NOT EXISTS logs_id_seq OWNED BY logs.id;
ALTER TABLE logs    ALTER COLUMN id SET DEFAULT nextval('logs_id_seq');
CREATE SEQUENCE IF NOT EXISTS history_id_seq OWNED BY history.id;
ALTER TABLE history ALTER COLUMN id SET DEFAULT nextval('history_id_seq');

-- 2.2. Función para actualizar ONLY updated_at en UPDATE
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  IF TG_OP = 'UPDATE' THEN
    NEW.updated_at := now();
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 2.3. Función de auditoría para INSERT y UPDATE
CREATE OR REPLACE FUNCTION audit_changes()
RETURNS TRIGGER AS $$
DECLARE
  v_old   JSON;
  v_new   JSON;
  v_user  INT := current_setting('app.current_user_id', true)::INT;
BEGIN
  IF TG_OP = 'INSERT' THEN
    v_old := NULL;
    v_new := row_to_json(NEW);
  ELSE
    v_old := row_to_json(OLD);
    v_new := row_to_json(NEW);
  END IF;

  INSERT INTO logs(table_name, action, changed_at, user_id, old_data, new_data)
  VALUES (TG_TABLE_NAME, TG_OP::log_action_enum, now(), v_user, v_old, v_new);

  INSERT INTO history(table_name, record_id, action, changed_at, old_data, new_data)
  VALUES (
    TG_TABLE_NAME,
    COALESCE(NEW.id, OLD.id),
    TG_OP::log_action_enum,
    now(),
    v_old,
    v_new
  );

  RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- 2.4. Función de auditoría para DELETE
CREATE OR REPLACE FUNCTION audit_delete()
RETURNS TRIGGER AS $$
DECLARE
  v_old  JSON := row_to_json(OLD);
  v_user INT  := current_setting('app.current_user_id', true)::INT;
BEGIN
  INSERT INTO logs(table_name, action, changed_at, user_id, old_data, new_data)
  VALUES (TG_TABLE_NAME, 'DELETE'::log_action_enum, now(), v_user, v_old, NULL);

  INSERT INTO history(table_name, record_id, action, changed_at, old_data, new_data)
  VALUES (TG_TABLE_NAME, OLD.id, 'DELETE'::log_action_enum, now(), v_old, NULL);

  RETURN OLD;
END;
$$ LANGUAGE plpgsql;

-- 2.5. Función opcional para bloquear DELETE (comentada)
-- CREATE OR REPLACE FUNCTION block_delete()
-- RETURNS TRIGGER AS $$
-- BEGIN
--   RAISE EXCEPTION 'Delete not allowed on table %', TG_TABLE_NAME;
-- END;
-- $$ LANGUAGE plpgsql;

-- 2.6. Asignación dinámica de triggers a todas las tablas (excepto logs y history)
DO $$
DECLARE
  t RECORD;
BEGIN
  FOR t IN
    SELECT table_name
    FROM information_schema.tables
    WHERE table_schema = 'public'
      AND table_type   = 'BASE TABLE'
      AND table_name NOT IN ('logs','history')
  LOOP
    -- a) trigger de updated_at
    IF EXISTS (
      SELECT 1 FROM information_schema.columns
      WHERE table_schema='public'
        AND table_name = t.table_name
        AND column_name = 'updated_at'
    ) THEN
      EXECUTE format(
        'CREATE TRIGGER trg_set_updated_at_%1$I
           BEFORE UPDATE ON %1$I
           FOR EACH ROW EXECUTE FUNCTION set_updated_at();',
        t.table_name
      );
    END IF;

    -- b) audit INSERT y UPDATE
    EXECUTE format(
      'CREATE TRIGGER trg_audit_%1$I
         AFTER INSERT OR UPDATE ON %1$I
         FOR EACH ROW EXECUTE FUNCTION audit_changes();',
      t.table_name
    );

    -- c) audit DELETE
    EXECUTE format(
      'CREATE TRIGGER trg_audit_delete_%1$I
         AFTER DELETE ON %1$I
         FOR EACH ROW EXECUTE FUNCTION audit_delete();',
      t.table_name
    );

    -- d) bloqueo DELETE (opción comentada)
    -- EXECUTE format(
    --   'CREATE TRIGGER trg_block_delete_%1$I
    --      BEFORE DELETE ON %1$I
    --      FOR EACH ROW EXECUTE FUNCTION block_delete();',
    --   t.table_name
    -- );
  END LOOP;
END;
$$ LANGUAGE plpgsql;
