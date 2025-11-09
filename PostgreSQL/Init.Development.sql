SELECT 'CREATE DATABASE osu_czechia' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'osu_czechia')\gexec

DO $$ BEGIN
    IF NOT EXISTS (SELECT * FROM pg_user WHERE usename = 'osu_czechia_bot') THEN
        CREATE ROLE osu_czechia_bot LOGIN SUPERUSER password '9BsmTF0UUHb1v4xSWcdoMRPsy3TqDj';
    END IF;
END $$;
